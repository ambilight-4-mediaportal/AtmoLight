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
    public static void PowerModeChanged(PowerModes powerMode, string atmoWinLocation, string comPort)
    {
      string appUSBDeview = @"C:\Program Files (x86)\Team MediaPortal\MediaPortal\USBDeview.exe";

      if(powerMode == PowerModes.Resume)
      {
          // Sleep 2.5s to allow for disk startup
          Thread.Sleep(2500);

          if (File.Exists(appUSBDeview))
          {
            // Close Atmowin
            try
            {
              Process[] proc = Process.GetProcessesByName("AtmoWinA");
              proc[0].Kill();
            }
            catch (Exception eProcAtmoKill)
            {
              Log.Error("AtmoWinHandler - [RESUME] Error while closing Atmowin:" + eProcAtmoKill.ToString());
            }

            // Disconnect COM port
            startProcess(appUSBDeview, "/disable_by_drive " + comPort);

            // Connect COM port
            startProcess(appUSBDeview, "/enable_by_drive " + comPort);

            // Start Atmowin
            if (File.Exists(atmoWinLocation))
            {
              startProcess(atmoWinLocation, "");
            }
            else
            {
              Log.Error(string.Format("AtmoWinHandler -[RESUME] Couldn't find the AtmoWinA.exe file: {0}", atmoWinLocation));
            }
          }
          else
          {
            Log.Error("AtmoWinHandler -[RESUME] Missing required program:  " + appUSBDeview);
          }
      }
      else if (powerMode == PowerModes.Suspend)
      {
          // Disconnect COM port
          Log.Debug("[SUSPEND] Disconnect " + comPort);
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

      // Sleep timer to avoid Windows being to quick upon COM port unlocking
      Thread.Sleep(2500);
    }
  }
}
