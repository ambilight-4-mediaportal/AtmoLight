using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using MinimalisticTelnet;

namespace AtmoLight
{
  class AmbiBoxHandler : ITargets
  {
    #region Fields
    public Target Name { get { return Target.AmbiBox; } }
    public TargetType Type { get { return TargetType.Local; } }
    public bool AllowDelay { get { return true; } }

    public List<ContentEffect> SupportedEffects
    {
      get
      {
        return new List<ContentEffect> {  ContentEffect.ExternalLiveMode,
                                          ContentEffect.GIFReader,
                                          ContentEffect.LEDsDisabled,
                                          ContentEffect.MediaPortalLiveMode,
                                          ContentEffect.StaticColor,
                                          ContentEffect.VUMeter,
                                          ContentEffect.VUMeterRainbow
        };
      }
    }

    private Core coreObject = Core.GetInstance();
    private volatile bool changeImageLock = false;
    private volatile bool initLock = false;

    private Thread initThreadHelper;

    private TelnetConnection ambiBoxConnection;
    private List<string> profileList = new List<string>();
    private string currentProfile;
    private int reconnectAttempts;
    private int ledCount;
    #endregion

    #region Constructor
    public AmbiBoxHandler()
    {
      Log.Debug("AmbiBoxHandler - AmbiBox as target added.");
    }
    #endregion

    #region ITargets Methods
    public void Initialise(bool force = false)
    {
      if (!initLock)
      {
        initThreadHelper = new Thread(() => InitThreaded(force));
        initThreadHelper.Name = "AtmoLight AmbiBox Init";
        initThreadHelper.IsBackground = true;
        initThreadHelper.Start();
      }

    }

    public void ReInitialise(bool force = false)
    {
      if (coreObject.reInitOnError || force)
      {
        Initialise(force);
      }
    }

    public void Dispose()
    {
      Log.Debug("AmbiBoxHandler - Disposing AmbiBox handler.");
      Disconnect();

      if (coreObject.ambiBoxAutoStop)
      {
        StopAmbiBox();
      }
    }

    public bool IsConnected()
    {
      if (ambiBoxConnection == null)
      {
        return false;
      }
      return ambiBoxConnection.IsConnected && !initLock;
    }

    public bool ChangeEffect(ContentEffect effect)
    {
      switch (effect)
      {
        case ContentEffect.ExternalLiveMode:
          ChangeEffect(ContentEffect.LEDsDisabled);
          if (profileList.Contains(coreObject.ambiBoxExternalProfile))
          {
            SendCommand("lock");
            SendCommand("setprofile:" + coreObject.ambiBoxExternalProfile);
            currentProfile = coreObject.ambiBoxExternalProfile;
            SendCommand("setstatus:on");
            SendCommand("unlock");
          }
          return true;
        case ContentEffect.MediaPortalLiveMode:
        case ContentEffect.GIFReader:
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          ChangeEffect(ContentEffect.LEDsDisabled);
          SendCommand("lock");
          System.Threading.Thread.Sleep(50);
          if (profileList.Contains(coreObject.ambiBoxMediaPortalProfile))
          {
            SendCommand("setprofile:" + coreObject.ambiBoxMediaPortalProfile);
            currentProfile = coreObject.ambiBoxMediaPortalProfile;
          }
          SendCommand("setstatus:on");
          SendCommand("unlock");
          return true;
        case ContentEffect.StaticColor:
          SendCommand("lock");
          string staticColorString = "setcolor:";
          for (int i = 1; i <= ledCount; i++)
          {
            staticColorString += i + "-" + coreObject.staticColor[0] + "," + coreObject.staticColor[1] + "," + coreObject.staticColor[2] + ";";
          }
          SendCommand(staticColorString);
          System.Threading.Thread.Sleep(50);
          SendCommand(staticColorString);
          System.Threading.Thread.Sleep(50);
          SendCommand(staticColorString);
          SendCommand("setstatus:off");
          return true;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          SendCommand("lock");
          string disableString = "setcolor:";
          for (int i = 1; i <= ledCount; i++)
          {
            disableString += i + "-" + 0 + "," + 0 + "," + 0 + ";";
          }
          SendCommand(disableString);
          System.Threading.Thread.Sleep(50);
          SendCommand(disableString);
          System.Threading.Thread.Sleep(50);
          SendCommand(disableString);
          SendCommand("setstatus:off");
          return true;
      }
    }
    public void ChangeProfile()
    {
      int pos = profileList.IndexOf(currentProfile);
      if (pos != profileList.Count - 1)
      {
        currentProfile = profileList[pos + 1];
      }
      else
      {
        currentProfile = profileList[0];
      }
      Log.Info("AmbiBoxHandler - Changing profile to: {0}", currentProfile);
      SendCommand("lock");
      SendCommand("setprofile:" + currentProfile);
      SendCommand("unlock");
    }

    #region ChangeImage

    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      try
      {
        MemoryMappedFile mmap = MemoryMappedFile.CreateOrOpen("AmbiBox_XBMC_SharedMemory", pixeldata.Length + 11,
          MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, null, HandleInheritability.Inheritable);

        MemoryMappedViewStream viewStream = mmap.CreateViewStream();

        Stopwatch sw = new Stopwatch();
        sw.Start();

        while (true)
        {
          viewStream.Seek(0, SeekOrigin.Begin);
          if (viewStream.ReadByte() == 0xF8)
          {
            break;
          }

          if (sw.ElapsedMilliseconds > coreObject.ambiBoxChangeImageDelay)
          {
            sw.Stop();
            viewStream.Dispose();
            mmap.Dispose();
            return;
          }
        }

        viewStream.Seek(0, SeekOrigin.Begin);
        viewStream.WriteByte(0xF0); // Begin
        viewStream.WriteByte((byte)(coreObject.GetCaptureWidth() & 0xff)); // Width
        viewStream.WriteByte((byte)((coreObject.GetCaptureWidth() >> 8) & 0xff)); // Width
        viewStream.WriteByte((byte)(coreObject.GetCaptureHeight() & 0xff)); // Height
        viewStream.WriteByte((byte)((coreObject.GetCaptureHeight() >> 8) & 0xff)); // Height
        viewStream.WriteByte((byte)(int)(coreObject.GetCaptureWidth() / coreObject.GetCaptureHeight() * 100)); // Aspect ratio
        viewStream.WriteByte(0x00); // Image format (RGBA)
        viewStream.WriteByte((byte)(pixeldata.Length & 0xff)); // Length
        viewStream.WriteByte((byte)((pixeldata.Length >> 8) & 0xff)); // Length
        viewStream.WriteByte((byte)((pixeldata.Length >> 16) & 0xff)); // Length
        viewStream.WriteByte((byte)((pixeldata.Length >> 24) & 0xff)); // Length

        viewStream.Write(pixeldata, 0, pixeldata.Length); // Copy pixeldata into mmap
        viewStream.Close();
      }
      catch (Exception e)
      {
        //Log.Error("Error occurred durin AmbiBox changeImage()");
        //Log.Error(e.Message);
      }
    }

    #endregion

    public void PowerModeChanged(PowerModes powerMode)
    {
      if (powerMode == PowerModes.Resume)
      {
        Disconnect();
        Initialise();
      }
      else if (powerMode == PowerModes.Suspend)
      {
        ChangeEffect(ContentEffect.LEDsDisabled);
      }
    }
    #endregion

    #region AmbiBox API
    private void InitThreaded(bool force = false)
    {
      if (initLock)
      {
        Log.Debug("AmbiBoxHandler - Initialising locked.");
        return;
      }
      initLock = true;
      reconnectAttempts++;
      try
      {
        if (coreObject.ambiBoxAutoStart && !Win32API.IsProcessRunning("AmbiBox.exe"))
        {
          if (!StartAmbiBox())
          {
            Log.Error("AmbiBoxHandler - Error connecting to {0}:{1}", coreObject.ambiBoxIP, coreObject.ambiBoxPort);
            if ((coreObject.reInitOnError || force) && reconnectAttempts < coreObject.ambiBoxMaxReconnectAttempts)
            {
              System.Threading.Thread.Sleep(coreObject.ambiBoxReconnectDelay);
              initLock = false;
              InitThreaded();
              return;
            }
          }
        }

        if (!Connect())
        {
          Log.Error("AmbiBoxHandler - Error connecting to {0}:{1}", coreObject.ambiBoxIP, coreObject.ambiBoxPort);
          if ((coreObject.reInitOnError || force) && reconnectAttempts < coreObject.ambiBoxMaxReconnectAttempts)
          {
            System.Threading.Thread.Sleep(coreObject.ambiBoxReconnectDelay);
            initLock = false;
            InitThreaded();
            return;
          }
        }
        reconnectAttempts = 0;
        initLock = false;

        ChangeEffect(coreObject.GetCurrentEffect());
      }
      catch (Exception ex)
      {
        Log.Error("AmbiBoxHandler - Error connecting to {0}:{1}", coreObject.ambiBoxIP, coreObject.ambiBoxPort);
        Log.Error("AmbiBoxHandler - Exception: {0}", ex.Message);
        if ((coreObject.reInitOnError || force) && reconnectAttempts < coreObject.ambiBoxMaxReconnectAttempts)
        {
          System.Threading.Thread.Sleep(coreObject.ambiBoxReconnectDelay);
          initLock = false;
          InitThreaded();
        }
      }
    }

    private bool Connect()
    {
      ambiBoxConnection = new TelnetConnection(coreObject.ambiBoxIP, coreObject.ambiBoxPort);
      if (ambiBoxConnection == null || !ambiBoxConnection.IsConnected)
      {
        return false;
      }
      ambiBoxConnection.Read();
      char[] separators = { ':', ';' };
      string profilesString = SendCommand("getprofiles", true);
      if (string.IsNullOrEmpty(profilesString))
      {
        return false;
      }
      string[] profiles = profilesString.Split(separators);
      for (int i = 1; i < profiles.Length; i++)
      {
        if (!string.IsNullOrEmpty(profiles[i]))
        {
          profileList.Add(profiles[i]);
        }
      }
      currentProfile = SendCommand("getprofile", true).Split(separators)[1];

      ledCount = int.Parse(SendCommand("getcountleds", true).Split(separators)[1]);
      if (ledCount <= 0)
      {
        return false;
      }

      SendCommand("unlock", true);

      Log.Info("AmbiBoxHandler - Successfully connected to {0}:{1}", coreObject.ambiBoxIP, coreObject.ambiBoxPort);
      return true;
    }

    private void Disconnect()
    {
      if (ambiBoxConnection != null)
      {
        ambiBoxConnection.Dispose();
        ambiBoxConnection = null;
      }
    }

    private string SendCommand(string command, bool noReinit = false)
    {
      try
      {
        ambiBoxConnection.WriteLine(command);
        string rcvd = ambiBoxConnection.Read();
        if (rcvd.Length < 2)
        {
          return null;
        }
        return rcvd.Remove(rcvd.Length - 2, 2);
      }
      catch (Exception ex)
      {
        Log.Error("AmbiBoxHanlder - Error communicating with API server.");
        Log.Error("AmbiBoxHanlder - Command: {0}", command);
        Log.Error("AmbiBoxHandler - Exception: {0}", ex.Message);
        if (!noReinit)
        {
          ReInitialise();
        }
        return null;
      }
    }
    #endregion

    #region AmbiBox
    public bool StartAmbiBox()
    {
      Log.Debug("AmbiBoxHandler - Trying to start AmbiBox.");
      if (!System.IO.File.Exists(coreObject.ambiBoxPath))
      {
        Log.Error("AmbiBoxHandler - AmbiBox.exe not found!");
        return false;
      }
      Process AmbiBox = new Process();
      AmbiBox.StartInfo.FileName = coreObject.ambiBoxPath;
      AmbiBox.StartInfo.UseShellExecute = true;
      AmbiBox.StartInfo.Verb = "open";
      try
      {
        AmbiBox.Start();
      }
      catch (Exception)
      {
        Log.Error("AmbiBoxHandler - Starting AmbiBox failed.");
        return false;
      }
      Log.Info("AmbiBoxHandler - AmbiBox successfully started.");
      return true;
    }

    public bool StopAmbiBox()
    {
      Log.Debug("AmbiBoxHandler - Trying to stop AmbiBox.");

      foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("AmbiBox")))
      {
        try
        {
          process.Kill();
          process.WaitForExit();
          Win32API.RefreshTrayArea();
        }
        catch (Exception ex)
        {
          Log.Error("AmbiBoxHandler - Stopping AmbiBox failed.");
          Log.Error("AmbiBoxHandler - Exception: {0}", ex.Message);
          return false;
        }
      }
      Log.Info("AmbiBoxHandler - AmbiBox successfully stopped.");
      return true;
    }
    #endregion
  }
}