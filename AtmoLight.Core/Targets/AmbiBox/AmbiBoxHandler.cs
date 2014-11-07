using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

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
    #endregion

    #region Constructor
    public AmbiBoxHandler()
    {
      Log.Debug("AmbiBoxHandler - AmbiBox as target added.");
    }
    #endregion

    public void Initialise(bool force = false)
    {
      Log.Error("init");
      // Initialise your target
    }

    public void ReInitialise(bool force = false)
    {
      Log.Error("reinit");
      // Reinitialise your target
    }

    public void Dispose()
    {
      Log.Error("dispose");
      // Close connections, applications (if needed), ...
    }

    public bool IsConnected()
    {
      return true;
    }

    public bool ChangeEffect(ContentEffect effect)
    {
      return true;
    }

    public void ChangeProfile()
    {
      Log.Error("profile");
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
        viewStream.WriteByte((byte)(((byte)coreObject.GetCaptureWidth() >> 8) & 0xff)); // Width
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
  }
}
