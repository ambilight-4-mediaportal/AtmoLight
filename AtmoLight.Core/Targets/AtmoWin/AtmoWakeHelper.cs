using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AtmoLight.Targets.AtmoWin
{
  class AtmoWakeHelper
  {
    public static void PowerModeChanged(PowerModes powerMode, string atmoWinLocation, string comPort, int resumeDelay)
    {
      string appUSBDeview = @"C:\Program Files (x86)\Team MediaPortal\MediaPortal\USBDeview.exe";

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
            Process[] proc = Process.GetProcessesByName("AtmoWinA");

            if (proc.Count() >= 0)
            {
              proc[0].Kill();
            }
          }
          catch (Exception eProcAtmoKill)
          {
            Log.Error("AtmoWinHandler - [RESUME] Error while closing Atmowin:" + eProcAtmoKill.ToString());
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
  }
}
