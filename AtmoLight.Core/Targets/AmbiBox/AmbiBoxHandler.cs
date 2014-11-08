using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using MinimalisticTelnet;

namespace AtmoLight
{
  class AmbiBoxHandler : ITargets
  {
    #region Fields
    public Target Name { get { return Target.AmbiBox; } }
    public TargetType Type { get { return TargetType.Local; } }

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
    private int reconnectAttempts = 0;
    #endregion

    #region Constructor
    public AmbiBoxHandler()
    {
      Log.Debug("AmbiBoxHandler - AmbiBox as target added.");
      Initialise();
    }
    #endregion

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
      ambiBoxConnection.Dispose();
    }

    public bool IsConnected()
    {
      return ambiBoxConnection.IsConnected && !initLock;
    }

    public bool ChangeEffect(ContentEffect effect)
    {
      switch (effect)
      {
        case ContentEffect.MediaPortalLiveMode:
        case ContentEffect.GIFReader:
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          return true;
        case ContentEffect.StaticColor:
          string staticColorString = "setcolor:";
          for (int i = 0; i <= 256; i++)
          {
            staticColorString += i + "-" + coreObject.staticColor[0] + "," + coreObject.staticColor[1] + "," + coreObject.staticColor[2] + ";";
          }
          if (SendCommand("lock") != "lock:success")
          {
            Log.Error("AmbiBoxHandler - Error changing static color.");
            return false;
          }
          if (SendCommand(staticColorString) != "ok")
          {
            Log.Error("AmbiBoxHandler - Error changing static color.");
            return false;
          }
          if (SendCommand("unlock") != "unlock:success")
          {
            Log.Error("AmbiBoxHandler - Error changing static color.");
            return false;
          }
          return true;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          if (SendCommand("lock") != "lock:success")
          {
            Log.Error("AmbiBoxHandler - Error switching off leds.");
            return false;
          }
          if (SendCommand("setstatus: off") != "ok")
          {
            Log.Error("AmbiBoxHandler - Error switching off leds.");
            return false;
          }
          if (SendCommand("unlock") != "unlock:success")
          {
            Log.Error("AmbiBoxHandler - Error switching off leds.");
            return false;
          }
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
      if (SendCommand("lock") != "lock:success")
      {
        Log.Error("AmbiBoxHandler - Error changing profile.");
        return;
      }
      if (SendCommand("setprofile:" + currentProfile) != "ok")
      {
        Log.Error("AmbiBoxHandler - Error changing profile.");
        return;
      }
      if (SendCommand("unlock") != "unlock:success")
      {
        Log.Error("AmbiBoxHandler - Error changing profile.");
        return;
      }
    }
    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      if (changeImageLock)
      {
        return;
      }
      Task.Factory.StartNew(() => { ChangeImageTask(pixeldata); });
    }

    private void ChangeImageTask(byte[] pixeldata)
    {
      changeImageLock = true;
      using (MemoryMappedFile mmap = MemoryMappedFile.CreateOrOpen("AmbiBox_XBMC_SharedMemory", pixeldata.Length + 11, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, null, HandleInheritability.Inheritable))
      {
        var viewStream = mmap.CreateViewStream();
        // Wait for AmbiBox to be ready.
        while (true)
        {
          viewStream.Seek(0, SeekOrigin.Begin);
          if (viewStream.ReadByte() == 0xF8)
          {
            break;
          }
          System.Threading.Thread.Sleep(5);
        }

        viewStream.Seek(0, SeekOrigin.Begin);
        viewStream.WriteByte(0xF0); // Begin
        viewStream.WriteByte((byte)(coreObject.GetCaptureWidth() & 0xff)); // Width
        viewStream.WriteByte((byte)((coreObject.GetCaptureWidth() >> 8) & 0xff)); // With
        viewStream.WriteByte((byte)(coreObject.GetCaptureHeight() & 0xff)); // Height
        viewStream.WriteByte((byte)((coreObject.GetCaptureHeight() >> 8) & 0xff)); // Height
        viewStream.WriteByte((byte)(int)(coreObject.GetCaptureWidth() / coreObject.GetCaptureHeight() * 100)); // Aspect radio
        viewStream.WriteByte(0x00); // Image format (RGBA)
        viewStream.WriteByte((byte)(pixeldata.Length & 0xff)); // Length
        viewStream.WriteByte((byte)((pixeldata.Length >> 8) & 0xff)); // Length
        viewStream.WriteByte((byte)((pixeldata.Length >> 16) & 0xff)); // Length
        viewStream.WriteByte((byte)((pixeldata.Length >> 24) & 0xff)); // Length
        viewStream.Write(pixeldata, 0, pixeldata.Length); // Copy pixeldata into mmap
        viewStream.Close();
      }
      changeImageLock = false;
    }

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
        ambiBoxConnection = new TelnetConnection(coreObject.ambiBoxIP, coreObject.ambiBoxPort);
        if (ambiBoxConnection.IsConnected)
        {
          ambiBoxConnection.Read();
          char[] separators = { ':', ';' };
          string[] profiles = SendCommand("getprofiles").Split(separators);
          for (int i = 1; i < profiles.Length; i++)
          {
            if (!string.IsNullOrEmpty(profiles[i]))
            {
              profileList.Add(profiles[i]);
            }
          }
          currentProfile = SendCommand("getprofile").Split(separators)[1];

          Log.Info("AmbiBoxHandler - Successfully connected to {0}:{1}", coreObject.ambiBoxIP, coreObject.ambiBoxPort);
          reconnectAttempts = 0;
          initLock = false;

          ChangeEffect(coreObject.GetCurrentEffect());
          coreObject.SetAtmoLightOn(coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled || coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ? false : true);
        }
        else
        {
          Log.Error("AmbiBoxHandler - Error connecting to {0}:{1}", coreObject.ambiBoxIP, coreObject.ambiBoxPort);
          if ((coreObject.reInitOnError || force) && reconnectAttempts < coreObject.ambiBoxMaxReconnectAttempts)
          {
            System.Threading.Thread.Sleep(coreObject.ambiBoxtReconnectDelay);
            initLock = false;
            InitThreaded();
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("AmbiBoxHandler - Error connecting to {0}:{1}", coreObject.ambiBoxIP, coreObject.ambiBoxPort);
        Log.Error("AmbiBoxHandler - Exception: {0}", ex.Message);
        if ((coreObject.reInitOnError || force) && reconnectAttempts < coreObject.ambiBoxMaxReconnectAttempts)
        {
          System.Threading.Thread.Sleep(coreObject.ambiBoxtReconnectDelay);
          initLock = false;
          InitThreaded();
        }
      }
    }

    private string SendCommand(string command)
    {
      ambiBoxConnection.WriteLine(command);
      string rcvd = ambiBoxConnection.Read();
      return rcvd.Remove(rcvd.Length - 2, 2);
    }
  }
}
