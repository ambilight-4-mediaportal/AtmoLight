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
    public static void PowerModeChanged(PowerModes powerMode, string comPort, int resumeDelay)
    {
      string appUSBDeviewPath = @"C:\Program Files (x86)\Team MediaPortal\MediaPortal\";
      string appUSBDeviewExe = "USBDeview.exe";

      // Detect x86 systems and use other executable if required.
      if(!Is64BitOperatingSystem())
      {
        appUSBDeviewPath = @"C:\Program Files\Team MediaPortal\MediaPortal\";
        appUSBDeviewExe = "USBDeview-x86.exe";
      }

      string appUSBDeview = Path.Combine(appUSBDeviewPath, appUSBDeviewExe);

      if (powerMode == PowerModes.Resume)
      {
        // Optional resume delay
        Thread.Sleep(resumeDelay);

        Log.Debug("AtmoWinHandler - [RESUME] AtmoWakeHelper INIT");
        if (File.Exists(appUSBDeview))
        {
          // Close Atmowin
          try
          {
            Log.Debug("AtmoWinHandler - [RESUME] AtmoWakeHelper killing AtmoWin process");
            string atmoWinProcessName = "AtmoWinA";

            foreach (var process in Process.GetProcessesByName(atmoWinProcessName))
            {
              process.Kill();
            }
          }
          catch (Exception eProcAtmoKill)
          {
            Log.Error("AtmoWinHandler - [RESUME] AtmoWakeHelper Error while closing Atmowin:" + eProcAtmoKill.ToString());
          }

          // Disconnect COM port
          Log.Debug("[SUSPEND] AtmoWakeHelper disconnecting " + comPort);
          startProcess(appUSBDeview, "/disable_by_drive " + comPort);

          // Sleep timer to avoid Windows being to quick upon COM port unlocking
          Thread.Sleep(1500);

          // Connect COM port
          Log.Debug("[SUSPEND] AtmoWakeHelper connecting " + comPort);
          startProcess(appUSBDeview, "/enable_by_drive " + comPort);

          // Sleep timer to avoid Windows being to quick upon COM port unlocking
          Thread.Sleep(1500);
        }
        else
        {
          Log.Error("AtmoWinHandler -[RESUME] Missing required program:  " + appUSBDeview);
        }

        Log.Debug("AtmoWinHandler - [RESUME] AtmoWakeHelper COMPLETED");
      }
      else if (powerMode == PowerModes.Suspend)
      {
        // Disconnect COM port
        Log.Debug("[SUSPEND] AtmoWakeHelper disconnecting " + comPort);
        startProcess(appUSBDeview, "/disable_by_drive " + comPort);
      }
    }

    private static void startProcess(string program, string arguments)
    {
      try
      {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = program;
        startInfo.Arguments = arguments;
        Process proc = Process.Start(startInfo);
        proc.WaitForExit(10000);
      }
      catch (Exception eStartProcess)
      {
        Log.Error(string.Format("AtmoWinHandler - Error while starting process ( {0} ) : {1}", program, eStartProcess));
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
