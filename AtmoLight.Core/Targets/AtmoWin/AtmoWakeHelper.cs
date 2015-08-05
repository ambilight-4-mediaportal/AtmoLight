using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AtmoLight.Targets.AtmoWin
{
  class AtmoWakeHelper
  {
    #region COM reconnection
    public static int disconnectDelay = 1500;
    public static int connectDelay = 1500;
    public static void reconnectCOM(string comPort, int resumeDelay)
    {
      Log.Debug("AtmoWinHandler - [AtmoWakeHelper] [reconnectCOM] delay (ms): " + resumeDelay.ToString());
      Thread.Sleep(resumeDelay);
      disconnectCOM(comPort);
      connectCOM(comPort);
    }

    public static void disconnectCOM(string comPort)
    {
      try
      {
        string appUSBDeviewPath = @"C:\Program Files (x86)\Team MediaPortal\MediaPortal\";
        string appUSBDeviewExe = "USBDeview.exe";

        // Detect x86 systems and use other executable if required.
        if (!Is64BitOperatingSystem())
        {
          appUSBDeviewPath = @"C:\Program Files\Team MediaPortal\MediaPortal\";
          appUSBDeviewExe = "USBDeview-x86.exe";
        }

        string appUSBDeview = Path.Combine(appUSBDeviewPath, appUSBDeviewExe);

        if (File.Exists(appUSBDeview))
        {
          // Disconnect COM port
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] [disconnectCOM] disconnecting " + comPort);
          startProcess(appUSBDeview, "/disable_by_drive " + comPort, true);
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] [disconnectCOM] disconnected " + comPort);

          // Sleep timer to avoid Windows being to quick upon COM port unlocking
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] [disconnectCOM] sleeping for (ms): " + disconnectDelay);
          Thread.Sleep(disconnectDelay);
        }
        else
        {
          Log.Error("AtmoWinHandler - [AtmoWakeHelper] [disconnectCOM] missing USBdeview executable: " + appUSBDeview);
        }
      }
      catch (Exception e)
      {
        Log.Error("AtmoWinHandler - [AtmoWakeHelper] [disconnectCOM] error during disconnect COM");
        Log.Error(e.Message);
      }
    }

    public static void connectCOM(string comPort)
    {
      try
      {
        string appUSBDeviewPath = @"C:\Program Files (x86)\Team MediaPortal\MediaPortal\";
        string appUSBDeviewExe = "USBDeview.exe";

        // Detect x86 systems and use other executable if required.
        if (!Is64BitOperatingSystem())
        {
          appUSBDeviewPath = @"C:\Program Files\Team MediaPortal\MediaPortal\";
          appUSBDeviewExe = "USBDeview-x86.exe";
        }

        string appUSBDeview = Path.Combine(appUSBDeviewPath, appUSBDeviewExe);

        if (File.Exists(appUSBDeview))
        {
          // Connect COM port
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] [connectCOM] connecting " + comPort);
          startProcess(appUSBDeview, "/enable_by_drive " + comPort, true);
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] [connectCOM] connected " + comPort);

          // Sleep timer to allow it to be recognized by Windows
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] [connectCOM] sleeping for (ms): " + connectDelay);
          Thread.Sleep(connectDelay);
        }
        else
        {
          Log.Error("AtmoWinHandler - [AtmoWakeHelper] [connectCOM] missing USBdeview executable: " + appUSBDeview);
        }
      }
      catch (Exception e)
      {
        Log.Error("AtmoWinHandler - [AtmoWakeHelper] [connectCOM] error during connect COM");
        Log.Error(e.Message);
      }
    }

    private static void startProcess(string program, string arguments, bool waitForExit)
    {
      try
      {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = program;
        startInfo.Arguments = arguments;
        Process proc = Process.Start(startInfo);
      }
      catch (Exception eStartProcess)
      {
        string error = string.Format("Error while starting process ( {0} ) : {1}", program, eStartProcess);
        Log.Error("AtmoWinHandler - [AtmoWakeHelper] [startProcess] " + error);
      }
    }

    #endregion

    #region OS 64-bit detection

    [DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr GetModuleHandle(string moduleName);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr GetProcAddress(IntPtr hModule,
        [MarshalAs(UnmanagedType.LPStr)]string procName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]

    static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

    /// <summary>
    /// The function determines whether the current operating system is a 
    /// 64-bit operating system.
    /// </summary>
    /// <returns>
    /// The function returns true if the operating system is 64-bit; 
    /// otherwise, it returns false.
    /// </returns>
    public static bool Is64BitOperatingSystem()
    {
      if (IntPtr.Size == 8)  // 64-bit programs run only on Win64
      {
        return true;
      }
      else  // 32-bit programs run on both 32-bit and 64-bit Windows
      {
        // Detect whether the current process is a 32-bit process 
        // running on a 64-bit system.
        bool flag;
        return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
            IsWow64Process(GetCurrentProcess(), out flag)) && flag);
      }
    }

    /// <summary>
    /// The function determins whether a method exists in the export 
    /// table of a certain module.
    /// </summary>
    /// <param name="moduleName">The name of the module</param>
    /// <param name="methodName">The name of the method</param>
    /// <returns>
    /// The function returns true if the method specified by methodName 
    /// exists in the export table of the module specified by moduleName.
    /// </returns>
    static bool DoesWin32MethodExist(string moduleName, string methodName)
    {
      IntPtr moduleHandle = GetModuleHandle(moduleName);
      if (moduleHandle == IntPtr.Zero)
      {
        return false;
      }
      return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
    }

    #endregion
  }
}
