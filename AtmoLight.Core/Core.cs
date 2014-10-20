using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using AtmoWinRemoteControl;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Linq;
using proto;
namespace AtmoLight
{
  public enum ContentEffect
  {
     
    LEDsDisabled = 0,
    AtmoWinLiveMode,
    Colorchanger,
    ColorchangerLR,
    MediaPortalLiveMode,
    StaticColor,
    GIFReader,
    VUMeter,
    VUMeterRainbow,
    Undefined = -1
  }

  public enum Target
  {
    AtmoWin,
    Hyperion
  }

  public class Core
  {

    #region Fields
    
    private bool delayEnabled = false;
    private int delayTime = 0;    
    private bool reinitialiseOnError = true;
    private bool startAtmoWin = true;
    private int[] staticColor = { 0, 0, 0 }; // RGB code for static color
    private string gifPath = "";

    private Thread setPixelDataThreadHelper;
    private Thread getAtmoLiveViewSourceThreadHelper;
    private Thread reinitialiseThreadHelper;
    private Thread gifReaderThreadHelper;
    private Thread vuMeterThreadHelper;



    // States
    private bool currentState = false; // State of the LEDs
    private ContentEffect currentEffect = ContentEffect.Undefined; // Current aktive effect
    private ContentEffect changeEffect = ContentEffect.Undefined; // Effect ChangeEffect() should change to (need for Reinitialise() if ChangeEffect() fails)


    AtmoWinHandler atmoWinHandler;
    HyperionHander hyperionHandler;

    // Lists
    private List<Target> targets = new List<Target>();
    private List<Object> targetObjects = new List<Object>();

    private List<byte[]> pixelDataList = new List<byte[]>(); // List for pixelData (Delay)
    private List<byte[]> bmiInfoHeaderList = new List<byte[]>(); // List for bmiInfoHeader (Delay)
    private List<long> delayTimingList = new List<long>(); // List for timings (Delay)

    // Locks
    private readonly object listLock = new object(); // Lock object for the above lists
    private volatile bool getAtmoLiveViewSourceLock = true; // Lock for liveview source checks
    private volatile bool setPixelDataLock = true; // Lock for SetPixelData thread
    private volatile bool reinitialiseLock = false;
    private volatile bool gifReaderLock = true;
    private volatile bool vuMeterLock = true;

    // VU Meter
    private int[] vuMeterThresholds = new int[] { -2, -5, -8, -10, -11, -12, -14, -18, -20, -22 };
    private bool vuMeterRainbowColorScheme = false;
    private List<SolidBrush> vuMeterBrushes = new List<SolidBrush>();

    private int captureWidth = 0; // AtmoWins capture width
    private int captureHeight = 0; // AtmoWins capture height

    public delegate void NewConnectionLostHandler();
    public static event NewConnectionLostHandler OnNewConnectionLost;

    public delegate double[] NewVUMeterHander();
    public static event NewVUMeterHander OnNewVUMeter;



    #endregion

    #region class Win32API
    public sealed class Win32API
    {
      [StructLayout(LayoutKind.Sequential)]
      public struct RECT
      {
        public int left;
        public int top;
        public int right;
        public int bottom;
      }

      [StructLayout(LayoutKind.Sequential)]
      private struct PROCESSENTRY32
      {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
      }

      private const uint TH32CS_SNAPPROCESS = 0x00000002;

      [DllImport("user32.dll")]
      public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);

      [DllImport("user32.dll")]
      public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

      [DllImport("user32.dll")]
      public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

      private const int WM_CLOSE = 0x10;
      private const int WM_DESTROY = 0x2;

      [DllImport("user32.dll")]
      public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern Int64 GetTickCount();

      [DllImport("kernel32.dll")]
      private static extern int Process32First(IntPtr hSnapshot,
                                       ref PROCESSENTRY32 lppe);

      [DllImport("kernel32.dll")]
      private static extern int Process32Next(IntPtr hSnapshot,
                                      ref PROCESSENTRY32 lppe);

      [DllImport("kernel32.dll", SetLastError = true)]
      private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags,
                                                     uint th32ProcessID);

      [DllImport("kernel32.dll", SetLastError = true)]
      private static extern bool CloseHandle(IntPtr hSnapshot);
      private const int WM_MouseMove = 0x0200;

      public static void RefreshTrayArea()
      {

        RECT rect;

        IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
        IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
        IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
        IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", null);
        GetClientRect(notificationAreaHandle, out rect);
        for (var x = 0; x < rect.right; x += 5)
          for (var y = 0; y < rect.bottom; y += 5)
            SendMessage(notificationAreaHandle, WM_MouseMove, 0, (y << 16) + x);
      }

      public static bool IsProcessRunning(string applicationName)
      {
        IntPtr handle = IntPtr.Zero;
        try
        {
          // Create snapshot of the processes
          handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
          PROCESSENTRY32 info = new PROCESSENTRY32();
          info.dwSize = (uint)System.Runtime.InteropServices.
                        Marshal.SizeOf(typeof(PROCESSENTRY32));

          // Get the first process
          int first = Process32First(handle, ref info);

          // While there's another process, retrieve it
          do
          {
            if (string.Compare(info.szExeFile,
                  applicationName, true) == 0)
            {
              return true;
            }
          }
          while (Process32Next(handle, ref info) != 0);
        }
        catch
        {
          throw;
        }
        finally
        {
          // Release handle of the snapshot
          CloseHandle(handle);
          handle = IntPtr.Zero;
        }
        return false;
      }
    }
    #endregion

    #region Constructor
    public Core()
    {
      return;
    }
    #endregion

    public void AddTarget(Target target)
    {
      targets.Add(target);
    }

    public bool Initialise()
    {
      for (int i = 0; i <= targets.Count; i++)
      {
        if (targets[i] == Target.AtmoWin)
        {
          atmoWinHandler = new AtmoWinHandler();

        }
        else if (targets[i] == Target.Hyperion)
        {
          targetObjects.Add(new HyperionHandler());
        }
      }
      return true;
    }

    #region Delay Lists
    /// <summary>
    /// Add new Items to the delay lists.
    /// </summary>
    /// <param name="bmiInfoHeader">Info Header</param>
    /// <param name="pixelData">Pixel Data</param>
    private void AddDelayListItem(byte[] bmiInfoHeader, byte[] pixelData)
    {
      if (delayTimingList.Count <= 60)
      {
        lock (listLock)
        {
          delayTimingList.Add(Win32API.GetTickCount());
          pixelDataList.Add(pixelData);
          bmiInfoHeaderList.Add(bmiInfoHeader);
        }
      }
      else
      {
        Log.Error("Delay buffer overflow.");
        ReinitialiseThreaded();
      }
    }

    /// <summary>
    /// Clear all delay lists.
    /// </summary>
    private void ClearDelayLists()
    {
      Log.Debug("Clearing delay lists.");
      lock (listLock)
      {
        delayTimingList.Clear();
        pixelDataList.Clear();
        bmiInfoHeaderList.Clear();
      }
      Log.Debug("Delay lists cleared.");
    }

    /// <summary>
    /// Trim all delay lists.
    /// </summary>
    private void TrimDelayLists()
    {
      lock (listLock)
      {
        delayTimingList.TrimExcess();
        pixelDataList.TrimExcess();
        bmiInfoHeaderList.TrimExcess();
      }
    }

    /// <summary>
    /// Delete first entry in all delay lists.
    /// </summary>
    private void DeleteFirstDelayListsItems()
    {
      lock (listLock)
      {
        delayTimingList.RemoveAt(0);
        pixelDataList.RemoveAt(0);
        bmiInfoHeaderList.RemoveAt(0);
      }
    }
    #endregion

    #region Utilities


    public void CalculateBitmap(Stream stream)
    {
      // Debug file output
      // new Bitmap(stream).Save("C:\\ProgramData\\Team MediaPortal\\MediaPortal\\" + Win32API.GetTickCount() + ".bmp");
      BinaryReader reader = new BinaryReader(stream);
      stream.Position = 0; // ensure that what start at the beginning of the stream. 
      reader.ReadBytes(14); // skip bitmap file info header
      byte[] bmiInfoHeader = reader.ReadBytes(4 + 4 + 4 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 4);

      int rgbL = (int)(stream.Length - stream.Position);
      int rgb = (int)(rgbL / (GetCaptureWidth() * GetCaptureHeight()));

      byte[] pixelData = reader.ReadBytes((int)(stream.Length - stream.Position));

      byte[] h1pixelData = new byte[GetCaptureWidth() * rgb];
      byte[] h2pixelData = new byte[GetCaptureWidth() * rgb];

      //now flip horizontally, we do it always to prevent microstudder
      int i;
      for (i = 0; i < ((GetCaptureHeight() / 2) - 1); i++)
      {
        Array.Copy(pixelData, i * GetCaptureWidth() * rgb, h1pixelData, 0, GetCaptureWidth() * rgb);
        Array.Copy(pixelData, (GetCaptureHeight() - i - 1) * GetCaptureWidth() * rgb, h2pixelData, 0, GetCaptureWidth() * rgb);
        Array.Copy(h1pixelData, 0, pixelData, (GetCaptureHeight() - i - 1) * GetCaptureWidth() * rgb, GetCaptureWidth() * rgb);
        Array.Copy(h2pixelData, 0, pixelData, i * GetCaptureWidth() * rgb, GetCaptureWidth() * rgb);
      }
      //send scaled and fliped frame to atmowin

      if (hyperionSocket.Connected)
      {
        HyperionSendImage(pixelData, Settings.hyperionPriority);
      }

      SetPixelData(bmiInfoHeader, pixelData);
    }

    public void UpdateGIFPath(string path)
    {
      gifPath = path;
    }
    #endregion

    

    #region Control LEDs
    /// <summary>
    /// Change effect.
    /// </summary>
    /// <param name="effect">Effect to change to</param>
    /// <param name="force">Force the effect change</param>
    /// <returns></returns>
    public bool ChangeEffect(ContentEffect effect, bool force = false)
    {
      if (!IsConnected())
      {
        return false;
      }
      // Static color gets excluded so we can actually change it.
      if ((effect == currentEffect) && (!force))
      {
        Log.Debug("Effect \"{0}\" is already active. Nothing to do.", effect);
        return false;
      }
      currentEffect = ContentEffect.Undefined;
      changeEffect = effect;
      Log.Info("Changing AtmoLight effect to: {0}", effect.ToString());
      StopAllThreads();
      switch (effect)
      {
        case ContentEffect.AtmoWinLiveMode:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI)) return false;
          break;
        case ContentEffect.Colorchanger:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemColorChange)) return false;
          break;
        case ContentEffect.ColorchangerLR:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLrColorChange)) return false;
          break;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
          currentState = false;

          if (hyperionSocket.Connected)
          {
              HyperionClearPriority(Settings.hyperionPriority);
          }

          if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor(0, 0, 0)) return false;
          break;
        case ContentEffect.MediaPortalLiveMode:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;

          if (delayEnabled)
          {
            Log.Debug("Adding {0}ms delay to the LEDs.", delayTime);
            StartSetPixelDataThread();
          }

          StartGetAtmoLiveViewSourceThread();
          break;
        case ContentEffect.StaticColor:
          currentState = true;

          if (hyperionSocket.Connected)
          {
            HyperionChangeColor(staticColor[0], staticColor[1], staticColor[2], Settings.hyperionPriority);
          }

          if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2])) return false;
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2])) return false;
          break;
        case ContentEffect.GIFReader:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
          StartGetAtmoLiveViewSourceThread();
          StartGIFReaderThread();
          break;
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          currentState = true;
          vuMeterRainbowColorScheme = (effect == ContentEffect.VUMeterRainbow) ? true : false;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
          StartGetAtmoLiveViewSourceThread();
          StartVUMeterThread();
          break;
      }
      currentEffect = changeEffect;
      changeEffect = ContentEffect.Undefined;
      return true;
    }

    /// <summary>
    /// Returns the current effect.
    /// </summary>
    /// <returns>Current effect</returns>
    public ContentEffect GetCurrentEffect()
    {
      return currentEffect;
    }

    /// <summary>
    /// Returns if AtmoLight/LEDs are on.
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsAtmoLightOn()
    {
      return currentState;
    }
 
    /// <summary>
    /// Change to AtmoWin profile.
    /// </summary>
    /// <returns>true or false</returns>
    public bool ChangeAtmoWinProfile()
    {
      if (!SetColorMode(ComEffectMode.cemColorMode)) return false;

      // Change the effect to the desired effect.
      // Needed for AtmoWin 1.0.0.5+
      if (!ChangeEffect(currentEffect, true)) return false;
      return true;
    }

    /// <summary>
    /// Changes the delay time.
    /// </summary>
    /// <param name="delay">Delay in ms.</param>
    /// <returns>true or false</returns>
    public bool ChangeDelay(int delay)
    {
      if (delay > 0)
      {
        Log.Debug("Changing delay to {0}ms.", delay);
        delayTime = delay;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Enables the delay.
    /// </summary>
    /// <param name="delay">Delay in ms.</param>
    /// <returns>true or false</returns>
    public bool EnableDelay(int delay = -1)
    {
      if (delay > 0)
      {
        delayTime = delay;
      }
      Log.Info("Adding {0}ms delay to LEDs.", delayTime);
      delayEnabled = true;
      StartSetPixelDataThread();
      return false;
    }

    /// <summary>
    /// Disables the delay.
    /// </summary>
    public void DisableDelay()
    {
      Log.Info("Removing delay.");
      delayEnabled = false;
      StopSetPixelDataThread();
    }

    /// <summary>
    /// Returns if the delay in enabled.
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsDelayEnabled()
    {
      return delayEnabled;
    }

    /// <summary>
    /// Returns the delay time.
    /// </summary>
    /// <returns>Delay in ms.</returns>
    public int GetDelayTime()
    {
      return delayTime;
    }

    /// <summary>
    /// Changes the static color.
    /// </summary>
    /// <param name="red">Red in RGB format.</param>
    /// <param name="green">Green  in RGB format.</param>
    /// <param name="blue">Blue  in RGB format.</param>
    /// <returns>true or false</returns>
    public bool ChangeStaticColor(int red, int green, int blue)
    {
      if ((red >= 0 && red <= 255) && (green >= 0 && green <= 255) && (blue >= 0 && blue <= 255))
      {
        staticColor[0] = red;
        staticColor[1] = green;
        staticColor[2] = blue;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Returns the static color.
    /// </summary>
    /// <returns>Static Color as int array</returns>
    public int[] GetStaticColor()
    {
      return staticColor;
    }
    #endregion

    #region Threads
    /// <summary>
    /// Start the SetPixelData thread.
    /// </summary>
    private void StartSetPixelDataThread()
    {
      setPixelDataLock = false;
      setPixelDataThreadHelper = new Thread(() => SetPixelDataThread());
      setPixelDataThreadHelper.Name = "AtmoLight SetPixelData";
      setPixelDataThreadHelper.Start();
    }

    /// <summary>
    /// Stop the SetPixelData thread.
    /// </summary>
    private void StopSetPixelDataThread()
    {
      setPixelDataLock = true;
    }

    /// <summary>
    /// Start the GetAtmoLiveViewSource thread.
    /// </summary>
    private void StartGetAtmoLiveViewSourceThread()
    {
      getAtmoLiveViewSourceLock = false;
      getAtmoLiveViewSourceThreadHelper = new Thread(() => GetAtmoLiveViewSourceThread());
      getAtmoLiveViewSourceThreadHelper.Name = "AtmoLight GetAtmoLiveViewSource";
      getAtmoLiveViewSourceThreadHelper.Start();
    }

    /// <summary>
    /// Stop the GetAtmoLiveViewSource thread.
    /// </summary>
    private void StopGetAtmoLiveViewSourceThread()
    {
      getAtmoLiveViewSourceLock = true;
    }

    private void StartGIFReaderThread()
    {
      gifReaderLock = false;
      gifReaderThreadHelper = new Thread(() => GIFReaderThread());
      gifReaderThreadHelper.Name = "AtmoLight GIFReader";
      gifReaderThreadHelper.Start();
    }

    private void StopGIFReaderThread()
    {
      gifReaderLock = true;
    }

    private void StartVUMeterThread()
    {
      vuMeterLock = false;
      vuMeterThreadHelper = new Thread(() => VUMeterThread());
      vuMeterThreadHelper.Name = "AtmoLight VUMeter";
      vuMeterThreadHelper.Start();
    }

    private void StopVUMeterThread()
    {
      vuMeterLock = true;
    }

    private void StopAllThreads()
    {
      StopSetPixelDataThread();
      StopGetAtmoLiveViewSourceThread();
      StopGIFReaderThread();
      StopVUMeterThread();
    }

    /// <summary>
    /// Send pixel data to AtmoWin when MediaPortal liveview is used (external liveview source).
    /// Also add a delay specified in settings.
    /// This method is designed to run as its own thread.
    /// </summary>
    private void SetPixelDataThread()
    {
      if (atmoRemoteControl == null)
      {
        return;
      }
      try
      {
        Log.Debug("Starting delay thread.");
        while (!setPixelDataLock && IsConnected())
        {
          if (delayTimingList.Count >= 1)
          {
            if (Win32API.GetTickCount() >= (delayTimingList[0] + delayTime))
            {
              SetPixelData(bmiInfoHeaderList[0], pixelDataList[0], true);
              DeleteFirstDelayListsItems();

              // Trim the lists, to prevent a memory leak.
              TrimDelayLists();
            }
          }
          // Sleep 5ms to reduce cpu load.
          System.Threading.Thread.Sleep(5);
        }
        ClearDelayLists();
      }
      catch (Exception ex)
      {
        Log.Error("Could not send data to AtmoWin.");
        Log.Error("Exception: {0}", ex.Message);

        ClearDelayLists();
      }
    }

    /// <summary>
    /// Check if the AtmoWin liveview source is set to external when MediaPortal liveview is used.
    /// Set liveview source back to external if needed.
    /// This method is designed to run as its own thread.
    /// </summary>
    private void GetAtmoLiveViewSourceThread()
    {
      try
      {
        while (!getAtmoLiveViewSourceLock && IsConnected())
        {
          GetAtmoLiveViewSource();
          if (atmoLiveViewSource != ComLiveViewSource.lvsExternal)
          {
            Log.Debug("AtmoWin Liveview Source is not lvsExternal");
            SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal);
          }
          System.Threading.Thread.Sleep(delayGetAtmoLiveViewSource);
        }
      }
      catch (Exception ex)
      {
        Log.Error("Error in GetAtmoLiveViewSourceThread.");
        Log.Error("Exception: {0}", ex.Message);
      }
    }

    private void GIFReaderThread()
    {
      try
      {
        // Get gif as stream
        Stream gifSource = new FileStream(gifPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        // Decode gif
        GifBitmapDecoder gifDecoder = new GifBitmapDecoder(gifSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
        BitmapMetadata gifDecoderMetadata = (BitmapMetadata)gifDecoder.Metadata;
        int gifWidth = Convert.ToInt32(gifDecoderMetadata.GetQuery("/logscrdesc/Width"));
        int gifHeight = Convert.ToInt32(gifDecoderMetadata.GetQuery("/logscrdesc/Height"));
        int gifBackgroundColor = Convert.ToInt32(gifDecoderMetadata.GetQuery("/logscrdesc/BackgroundColorIndex"));
        while (!gifReaderLock)
        {
          for (int index = 0; index < gifDecoder.Frames.Count; index++)
          {
            if (gifReaderLock)
            {
              break;
            }
            // Select frame
            BitmapSource gifBitmapSource = gifDecoder.Frames[index];
            gifBitmapSource.Freeze();
            BitmapMetadata gifBitmapMetadata = (BitmapMetadata)gifBitmapSource.Metadata;
            int gifDelay = Convert.ToInt32(gifBitmapMetadata.GetQuery("/grctlext/Delay")) * 10;
            int gifOffsetLeft = Convert.ToInt32(gifBitmapMetadata.GetQuery("/imgdesc/Left"));
            int gifOffsetTop = Convert.ToInt32(gifBitmapMetadata.GetQuery("/imgdesc/Top"));

            if (gifDelay == 0)
            {
              gifDelay = 20;
            }

            Bitmap gifBitmap;
            // Convert frame to Bitmap
            using (MemoryStream outStream = new MemoryStream())
            {
              BitmapEncoder gifEncoder = new BmpBitmapEncoder();
              gifEncoder.Frames.Add(BitmapFrame.Create(gifBitmapSource));
              gifEncoder.Save(outStream);
              gifBitmap = new Bitmap(outStream);
            }
            // Correct position of this frame, as gifs dont have to have fixed dimensions and positions
            if (gifBitmap.Width != gifWidth || gifBitmap.Height != gifHeight || gifOffsetLeft > 0 || gifOffsetTop > 0)
            {
              using (Bitmap gifBitmapOffset = new Bitmap(gifWidth, gifHeight))
              {
                using (Graphics gifBitmapOffsetGFX = Graphics.FromImage(gifBitmapOffset))
                {
                  // Fill Bitmap with background color
                  gifBitmapOffsetGFX.FillRectangle(new SolidBrush(Color.FromArgb(gifDecoder.Palette.Colors[gifBackgroundColor].A, gifDecoder.Palette.Colors[gifBackgroundColor].R, gifDecoder.Palette.Colors[gifBackgroundColor].G, gifDecoder.Palette.Colors[gifBackgroundColor].B)), 0, 0, gifWidth, gifHeight);
                  // Draw in original picture
                  gifBitmapOffsetGFX.DrawImage(gifBitmap, gifOffsetLeft, gifOffsetTop);
                  // Copy Bitmap
                  gifBitmap = gifBitmapOffset.Clone(new Rectangle(0, 0, gifWidth, gifHeight), PixelFormat.Undefined);
                }
              }
            }
            
            // Resize Bitmap
            gifBitmap = new Bitmap(gifBitmap, new Size(GetCaptureWidth(), GetCaptureHeight()));

            // Convert Bitmap to stream
            MemoryStream gifStream = new MemoryStream();
            gifBitmap.Save(gifStream, ImageFormat.Bmp);

            // Calculations to prepare data for AtmoWin and then send data
            CalculateBitmap(gifStream);

            // Cleanup
            gifStream.Close();
            gifStream.Dispose();
            gifBitmap.Dispose();

            // Sleep until next frame
            System.Threading.Thread.Sleep(gifDelay);
          }
        }
        gifSource.Close();
        gifSource.Dispose();
      }
      catch (Exception ex)
      {
        Log.Error("Error in GIFReaderThread.");
        Log.Error("Exception: {0}", ex.Message);
      }
    }

    private void VUMeterThread()
    {
      try
      {
        if (vuMeterRainbowColorScheme)
        {
          vuMeterBrushes.Clear();
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 77, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 128, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 204, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(230, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(102, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 153)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 230, 255)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 128, 255)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 0, 255)));
        }
        else
        {
          vuMeterBrushes.Clear();
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 128, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
        }

        Rectangle rectFull = new Rectangle(0, 0, GetCaptureWidth(), GetCaptureHeight());

        Bitmap vuMeterBitmap = new Bitmap(GetCaptureWidth(), GetCaptureHeight());
        Graphics vuMeterGFX = Graphics.FromImage(vuMeterBitmap);

        double[] dbLevel = new double[] { 0.0, 0.0 };

        while (!vuMeterLock)
        {
          vuMeterGFX.FillRectangle(vuMeterBrushes[0], rectFull);
          dbLevel = OnNewVUMeter();

          for (int channel = 0; channel <= 1; channel++)
          {
            for (int index = 0; index < vuMeterThresholds.Length; index++)
            {
              if (dbLevel[channel] >= vuMeterThresholds[index])
              {
                vuMeterGFX.FillRectangle(vuMeterBrushes[index + 1], (int)((double)channel * (double)vuMeterBitmap.Width / (double)4 * (double)3), (int)((double)index * (double)vuMeterBitmap.Height / (double)10), (int)((double)vuMeterBitmap.Width / (double)4), (int)(((double)vuMeterBitmap.Height / (double)10) + (double)1));
              }
            }
          }

          MemoryStream vuMeterStream = new MemoryStream();
          vuMeterBitmap.Save(vuMeterStream, ImageFormat.Bmp);
          CalculateBitmap(vuMeterStream);
          vuMeterStream.Close();
          vuMeterStream.Dispose();

          System.Threading.Thread.Sleep(50);
        }
        vuMeterBitmap.Dispose();
        vuMeterGFX.Dispose();
      }
      catch (Exception ex)
      {
        Log.Error("Error in VUMeterThread.");
        Log.Error("Exception: {0}", ex.Message);
      }
    }
    #endregion
  }
}
