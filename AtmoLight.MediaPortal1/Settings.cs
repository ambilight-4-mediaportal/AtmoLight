using System;
using MediaPortal.Profile;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AtmoLight
{
  public class Settings
  {
    #region Fields
    // Generic
    public static ContentEffect effectVideo;
    public static ContentEffect effectMusic;
    public static ContentEffect effectRadio;
    public static ContentEffect effectMenu;
    public static ContentEffect effectMPExit;
    public static int killButton;
    public static int profileButton;
    public static int menuButton;
    public static bool sbs3dOn;
    public static bool manualMode;
    public static bool lowCPU;
    public static int lowCPUTime;
    public static bool delay;
    public static int delayReferenceTime;
    public static int delayReferenceRefreshRate;
    public static DateTime excludeTimeStart;
    public static DateTime excludeTimeEnd;
    public static int staticColorRed;
    public static int staticColorGreen;
    public static int staticColorBlue;
    public static bool restartOnError;
    public static bool trueGrabbing;
    public static bool blackbarDetection;
    public static int blackbarDetectionTime;
    public static int blackbarDetectionThreshold;
    public static string gifFile;
    public static int captureWidth;
    public static int captureHeight;
    public static int powerModeChangedDelay;
    public static bool monitorScreensaverState;
    public static bool monitorWindowState;
    public static int vuMeterMindB;
    public static double vuMeterMinHue;
    public static double vuMeterMaxHue;
    public static string currentLanguageFile;
    public static bool blackbarDetectionHorizontal;
    public static bool blackbarDetectionVertical;
    public static bool blackbarDetectionLinkAreas;
    public static bool remoteApiServer;

    // Atmowin
    public static bool atmoWinTarget;
    public static string atmowinExe;
    public static bool startAtmoWin;
    public static bool exitAtmoWin;
    public static bool atmoWakeHelperEnabled;
    public static string atmoWakeHelperComPort;
    public static int atmoWakeHelperResumeDelay;
    public static int atmoWakeHelperDisconnectDelay;
    public static int atmoWakeHelperConnectDelay;
    public static int atmoWakeHelperReinitializationDelay;

    // Boblight
    public static bool boblightTarget;
    public static string boblightIP;
    public static int boblightPort;
    public static int boblightMaxFPS;
    public static int boblightMaxReconnectAttempts;
    public static int boblightReconnectDelay;
    public static int boblightSpeed;
    public static int boblightAutospeed;
    public static bool boblightInterpolation;
    public static int boblightSaturation;
    public static int boblightValue;
    public static int boblightThreshold;
    public static double boblightGamma;

    // Hyperion
    public static bool hyperionTarget;
    public static string hyperionIP;
    public static int hyperionPort;
    public static int hyperionPriority;
    public static int hyperionReconnectDelay;
    public static int hyperionReconnectAttempts;
    public static int hyperionPriorityStaticColor;
    public static bool hyperionLiveReconnect;

    // Hue
    public static bool hueTarget;
    public static string hueExe;
    public static bool hueStart;
    public static bool hueIsRemoteMachine;
    public static string hueIP;
    public static int huePort;
    public static int hueReconnectDelay;
    public static int hueReconnectAttempts;
    public static bool hueBridgeEnableOnResume;
    public static bool hueBridgeDisableOnSuspend;
    public static int hueMinDiversion;
    public static int hueThreshold;
    public static int hueBlackThreshold;
    public static double hueSaturation;
    public static bool hueUseOverallLightness;
    public static bool hueTheaterEnabled;
    public static bool hueTheaterRestoreLights;

    // AmbiBox
    public static bool ambiBoxTarget;
    public static string ambiBoxIP;
    public static int ambiBoxPort;
    public static int ambiBoxMaxReconnectAttempts;
    public static int ambiBoxReconnectDelay;
    public static int ambiBoxChangeImageDelay;
    public static string ambiBoxMediaPortalProfile;
    public static string ambiBoxExternalProfile;
    public static string ambiBoxPath;
    public static bool ambiBoxAutoStart;
    public static bool ambiBoxAutoStop;

    // AtmoOrb
    public static bool atmoOrbTarget;
    public static int atmoOrbBroadcastPort;
    public static int atmoOrbThreshold;
    public static int atmoOrbMinDiversion;
    public static double atmoOrbSaturation;
    public static double atmoOrbGamma;
    public static int atmoOrbBlackThreshold;
    public static bool atmoOrbUseOverallLightness;
    public static bool atmoOrbUseSmoothing;
    public static int atmoOrbSmoothingThreshold;


    public static List<string> atmoOrbLamps = new List<string>();
    #endregion

    #region Methods
    public static DateTime LoadTimeSetting(MediaPortal.Profile.Settings reader, string name, string defaultTime)
    {
      string s = reader.GetValueAsString("atmolight", name, defaultTime);
      DateTime dt;
      if (!DateTime.TryParse(s, out dt))
      {
        dt = DateTime.Parse(defaultTime);
      }
      return dt;
    }


    public static void LoadSettings()
    {
      using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
      {
        // Legacy support
        // The effect settings were integers in the past, but now are strings.
        // In order to avoid a lot of people loosing their effect settings during an update,
        // we convert the old settings to the new ones.
        int effectVideoInt;
        if (int.TryParse(reader.GetValueAsString("atmolight", "effectVideo", "MediaPortalLiveMode"), out effectVideoInt))
        {
          effectVideo = OldIntToNewContentEffect(effectVideoInt);
          SaveSpecificSetting("effectVideo", effectVideo.ToString());
        }
        else
        {
          effectVideo = (ContentEffect)Enum.Parse(typeof(ContentEffect), reader.GetValueAsString("atmolight", "effectVideo", "MediaPortalLiveMode"));
        }

        int effectMusicInt;
        if (int.TryParse(reader.GetValueAsString("atmolight", "effectMusic", "LEDsDisabled"), out effectMusicInt))
        {
          effectMusic = OldIntToNewContentEffect(effectMusicInt);
          SaveSpecificSetting("effectMusic", effectMusic.ToString());
        }
        else
        {
          effectMusic = (ContentEffect)Enum.Parse(typeof(ContentEffect), reader.GetValueAsString("atmolight", "effectMusic", "LEDsDisabled"));
        }

        int effecRadioInt;
        if (int.TryParse(reader.GetValueAsString("atmolight", "effectRadio", "LEDsDisabled"), out effecRadioInt))
        {
          effectRadio = OldIntToNewContentEffect(effecRadioInt);
          SaveSpecificSetting("effectRadio", effectRadio.ToString());
        }
        else
        {
          effectRadio = (ContentEffect)Enum.Parse(typeof(ContentEffect), reader.GetValueAsString("atmolight", "effectRadio", "LEDsDisabled"));
        }

        int effectMenuInt;
        if (int.TryParse(reader.GetValueAsString("atmolight", "effectMenu", "LEDsDisabled"), out effectMenuInt))
        {
          effectMenu = OldIntToNewContentEffect(effectMenuInt);
          SaveSpecificSetting("effectMenu", effectMenu.ToString());
        }
        else
        {
          effectMenu = (ContentEffect)Enum.Parse(typeof(ContentEffect), reader.GetValueAsString("atmolight", "effectMenu", "LEDsDisabled"));
        }

        int effectMPExitInt;
        if (int.TryParse(reader.GetValueAsString("atmolight", "effectMPExit", "LEDsDisabled"), out effectMPExitInt))
        {
          effectMPExit = OldIntToNewContentEffect(effectMPExitInt == 4 ? 5 : effectMPExitInt);
          SaveSpecificSetting("effectMPExit", effectMPExit.ToString());
        }
        else
        {
          effectMPExit = (ContentEffect)Enum.Parse(typeof(ContentEffect), reader.GetValueAsString("atmolight", "effectMPExit", "LEDsDisabled"));
        }

        currentLanguageFile = reader.GetValueAsString("atmolight", "CurrentLanguageFile", currentLanguageFile = Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language), @"\AtmoLight\en.xml"));
        if (currentLanguageFile.Substring(currentLanguageFile.Length - 3, 3).ToLower() == "lng")
        {
          int lastBackslash = currentLanguageFile.LastIndexOf("\\") + 1;
          int lastDot = currentLanguageFile.LastIndexOf(".");

          switch (currentLanguageFile.Substring(lastBackslash, lastDot - lastBackslash))
          {
            case "GermanDE":
              currentLanguageFile = Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language), @"\AtmoLight\de.xml");
              break;
            case "DutchNL":
              currentLanguageFile = Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language), @"\AtmoLight\nl.xml");
              break;
            case "FrenchFR":
              currentLanguageFile = Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language), @"\AtmoLight\fr.xml");
              break;
            default:
            case "EnglishUS":
              currentLanguageFile = Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language), @"\AtmoLight\en.xml");
              break;
          }
        }

        // Normal settings loading
        atmowinExe = reader.GetValueAsString("atmolight", "atmowinexe", "");
        killButton = reader.GetValueAsInt("atmolight", "killbutton", 4);
        profileButton = reader.GetValueAsInt("atmolight", "cmbutton", 4);
        menuButton = reader.GetValueAsInt("atmolight", "menubutton", 4);
        excludeTimeStart = LoadTimeSetting(reader, "excludeTimeStart", "00:00");
        excludeTimeEnd = LoadTimeSetting(reader, "excludeTimeEnd", "00:00");
        manualMode = reader.GetValueAsBool("atmolight", "OffOnStart", false);
        sbs3dOn = reader.GetValueAsBool("atmolight", "SBS_3D_ON", false);
        lowCPU = reader.GetValueAsBool("atmolight", "lowCPU", false);
        lowCPUTime = reader.GetValueAsInt("atmolight", "lowCPUTime", 0);
        delay = reader.GetValueAsBool("atmolight", "Delay", false);
        delayReferenceTime = reader.GetValueAsInt("atmolight", "DelayTime", 0);
        exitAtmoWin = reader.GetValueAsBool("atmolight", "ExitAtmoWin", true);
        startAtmoWin = reader.GetValueAsBool("atmolight", "StartAtmoWin", true);
        atmoWakeHelperEnabled = reader.GetValueAsBool("atmolight", "atmoWakeHelperEnabled", false);
        atmoWakeHelperComPort = reader.GetValueAsString("atmolight", "atmoWakeHelperComPort", "");
        atmoWakeHelperResumeDelay = reader.GetValueAsInt("atmolight", "atmoWakeHelperResumeDelay", 2500);
        atmoWakeHelperDisconnectDelay = reader.GetValueAsInt("atmolight", "atmoWakeHelperDisconnectDelay", 1500);
        atmoWakeHelperConnectDelay = reader.GetValueAsInt("atmolight", "atmoWakeHelperConnectDelay", 1500);
        atmoWakeHelperReinitializationDelay = reader.GetValueAsInt("atmolight", "atmoWakeHelperReinitializationDelay", 0);
        staticColorRed = reader.GetValueAsInt("atmolight", "StaticColorRed", 0);
        staticColorGreen = reader.GetValueAsInt("atmolight", "StaticColorGreen", 0);
        staticColorBlue = reader.GetValueAsInt("atmolight", "StaticColorBlue", 0);
        restartOnError = reader.GetValueAsBool("atmolight", "RestartOnError", true);
        trueGrabbing = reader.GetValueAsBool("atmolight", "TrueGrabbing", true);
        delayReferenceRefreshRate = reader.GetValueAsInt("atmolight", "DelayRefreshRate", 50);
        blackbarDetection = reader.GetValueAsBool("atmolight", "BlackbarDetection", false);
        blackbarDetectionTime = reader.GetValueAsInt("atmolight", "BlackbarDetectionTime", 1000);
        gifFile = reader.GetValueAsString("atmolight", "GIFFile", "");
        captureWidth = reader.GetValueAsInt("atmolight", "captureWidth", 64);
        captureHeight = reader.GetValueAsInt("atmolight", "captureHeight", 64);
        monitorScreensaverState = reader.GetValueAsBool("atmolight", "monitorScreensaverState", true);
        monitorWindowState = reader.GetValueAsBool("atmolight", "monitorWindowState", true);
        hyperionIP = reader.GetValueAsString("atmolight", "hyperionIP", "127.0.0.1");
        hyperionPort = reader.GetValueAsInt("atmolight", "hyperionPort", 19445);
        hyperionReconnectDelay = reader.GetValueAsInt("atmolight", "hyperionReconnectDelay", 10000);
        hyperionReconnectAttempts = reader.GetValueAsInt("atmolight", "hyperionReconnectAttempts", 5);
        hyperionPriority = reader.GetValueAsInt("atmolight", "hyperionPriority", 1);
        hyperionPriorityStaticColor = reader.GetValueAsInt("atmolight", "hyperionStaticColorPriority", 1);
        hyperionLiveReconnect = reader.GetValueAsBool("atmolight", "hyperionLiveReconnect", false);
        hueExe = reader.GetValueAsString("atmolight", "hueExe", "");
        hueStart = reader.GetValueAsBool("atmolight", "hueStart", true);
        hueIsRemoteMachine = reader.GetValueAsBool("atmolight", "hueIsRemoteMachine", false);
        hueIP = reader.GetValueAsString("atmolight", "hueIP", "127.0.0.1");
        huePort = reader.GetValueAsInt("atmolight", "huePort", 20123);
        hueReconnectDelay = reader.GetValueAsInt("atmolight", "hueReconnectDelay", 10000);
        hueReconnectAttempts = reader.GetValueAsInt("atmolight", "hueReconnectAttempts", 5);
        hueBridgeEnableOnResume = reader.GetValueAsBool("atmolight", "hueBridgeEnableOnResume", false);
        hueBridgeDisableOnSuspend = reader.GetValueAsBool("atmolight", "hueBridgeDisableOnSuspend", false);
        hueTheaterEnabled = reader.GetValueAsBool("atmolight", "hueTheaterEnabled", false);
        hueTheaterRestoreLights = reader.GetValueAsBool("atmolight", "hueTheaterRestoreLights", false);
        boblightIP = reader.GetValueAsString("atmolight", "boblightIP", "127.0.0.1");
        boblightPort = reader.GetValueAsInt("atmolight", "boblightPort", 19333);
        boblightMaxFPS = reader.GetValueAsInt("atmolight", "boblightMaxFPS", 10);
        boblightMaxReconnectAttempts = reader.GetValueAsInt("atmolight", "boblightMaxReconnectAttempts", 5);
        boblightReconnectDelay = reader.GetValueAsInt("atmolight", "boblightReconnectDelay", 5000);
        boblightSpeed = reader.GetValueAsInt("atmolight", "boblightSpeed", 100);
        boblightAutospeed = reader.GetValueAsInt("atmolight", "boblightAutospeed", 0);
        boblightInterpolation = reader.GetValueAsBool("atmolight", "boblightInterpolation", true);
        boblightSaturation = reader.GetValueAsInt("atmolight", "boblightSaturation", 1);
        boblightValue = reader.GetValueAsInt("atmolight", "boblightValue", 1);
        boblightThreshold = reader.GetValueAsInt("atmolight", "boblightThreshold", 20);
        boblightGamma = Double.Parse(reader.GetValueAsString("atmolight", "boblightGamma", "2.2").Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
        atmoWinTarget = reader.GetValueAsBool("atmolight", "atmoWinTarget", true);
        boblightTarget = reader.GetValueAsBool("atmolight", "boblightTarget", false);
        hueTarget = reader.GetValueAsBool("atmolight", "hueTarget", false);
        hyperionTarget = reader.GetValueAsBool("atmolight", "hyperionTarget", false);
        blackbarDetectionThreshold = reader.GetValueAsInt("atmolight", "blackbarDetectionThreshold", 20);
        powerModeChangedDelay = reader.GetValueAsInt("atmolight", "powerModeChangedDelay", 5000);
        ambiBoxTarget = reader.GetValueAsBool("atmolight", "ambiBoxTarget", false);
        ambiBoxIP = reader.GetValueAsString("atmolight", "ambiBoxIP", "127.0.0.1");
        ambiBoxPort = reader.GetValueAsInt("atmolight", "ambiBoxPort", 3636);
        ambiBoxMaxReconnectAttempts = reader.GetValueAsInt("atmolight", "ambiBoxMaxReconnectAttempts", 5);
        ambiBoxReconnectDelay = reader.GetValueAsInt("atmolight", "ambiBoxReconnectDelay", 5000);
        ambiBoxChangeImageDelay = reader.GetValueAsInt("atmolight", "ambiBoxChangeImageDelay", 10);
        ambiBoxMediaPortalProfile = reader.GetValueAsString("atmolight", "ambiBoxMediaPortalProfile", "MediaPortal");
        ambiBoxExternalProfile = reader.GetValueAsString("atmolight", "ambiBoxExternalProfile", "External");
        ambiBoxPath = reader.GetValueAsString("atmolight", "ambiBoxPath", "C:\\Program Files (x86)\\AmbiBox\\AmbiBox.exe");
        ambiBoxAutoStart = reader.GetValueAsBool("atmolight", "ambiBoxAutoStart", false);
        ambiBoxAutoStop = reader.GetValueAsBool("atmolight", "ambiBoxAutoStop", false);
        atmoOrbTarget = reader.GetValueAsBool("atmolight", "atmoOrbTarget", false);
        atmoOrbBlackThreshold = reader.GetValueAsInt("atmolight", "atmoOrbBlackThreshold", 16);
        atmoOrbBroadcastPort = reader.GetValueAsInt("atmolight", "atmoOrbBroadcastPort", 49692);
        atmoOrbGamma = Double.Parse(reader.GetValueAsString("atmolight", "atmoOrbGamma", "1").Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
        atmoOrbMinDiversion = reader.GetValueAsInt("atmolight", "atmoOrbMinDiversion", 16);
        atmoOrbSaturation = Double.Parse(reader.GetValueAsString("atmolight", "atmoOrbSaturation", "0.2").Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
        atmoOrbThreshold = reader.GetValueAsInt("atmolight", "atmoOrbThreshold", 0);
        atmoOrbUseOverallLightness = reader.GetValueAsBool("atmolight", "atmoOrbUseOverallLightness", true);
        atmoOrbUseSmoothing = reader.GetValueAsBool("atmolight", "atmoOrbUseSmoothing", true);
        atmoOrbSmoothingThreshold = reader.GetValueAsInt("atmolight", "atmoOrbSmoothingThreshold", 200);

        string atmoOrbLampTemp = reader.GetValueAsString("atmolight", "atmoOrbLamps", "");
        string[] atmoOrbLampTempSplit = atmoOrbLampTemp.Split('|');
        for (int i = 0; i < atmoOrbLampTempSplit.Length; i++)
        {
          if (!string.IsNullOrEmpty(atmoOrbLampTempSplit[i]))
          {
            atmoOrbLamps.Add(atmoOrbLampTempSplit[i]);
          }
        }
        vuMeterMindB = reader.GetValueAsInt("atmolight", "vuMeterMindB", -24);
        vuMeterMinHue = Double.Parse(reader.GetValueAsString("atmolight", "vuMeterMinHue", "0,74999").Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
        vuMeterMaxHue = Double.Parse(reader.GetValueAsString("atmolight", "vuMeterMaxHue", "0,95833").Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
        hueThreshold = reader.GetValueAsInt("atmolight", "hueThreshold", 16);
        hueBlackThreshold = reader.GetValueAsInt("atmolight", "hueBlackThreshold", 16);
        hueMinDiversion = reader.GetValueAsInt("atmolight", "hueMinDiversion", 16);
        hueSaturation = Double.Parse(reader.GetValueAsString("atmolight", "hueSaturation", "0.2").Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
        hueUseOverallLightness = reader.GetValueAsBool("atmolight", "hueUseOverallLightness", true);
        blackbarDetectionHorizontal = reader.GetValueAsBool("atmolight", "blackbarDetectionHorizontal", true);
        blackbarDetectionVertical = reader.GetValueAsBool("atmolight", "blackbarDetectionVertical", true);
        blackbarDetectionLinkAreas = reader.GetValueAsBool("atmolight", "blackbarDetectionLinkAreas", true);
        remoteApiServer = reader.GetValueAsBool("atmolight", "remoteApiServer", false);
      }
    }
    public static void SaveSettings()
    {
      using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
      {
        reader.SetValue("atmolight", "atmowinexe", atmowinExe);
        reader.SetValue("atmolight", "effectVideo", effectVideo.ToString());
        reader.SetValue("atmolight", "effectMusic", effectMusic.ToString());
        reader.SetValue("atmolight", "effectRadio", effectRadio.ToString());
        reader.SetValue("atmolight", "effectMenu", effectMenu.ToString());
        reader.SetValue("atmolight", "effectMPExit", effectMPExit.ToString());
        reader.SetValue("atmolight", "killbutton", (int)killButton);
        reader.SetValue("atmolight", "cmbutton", (int)profileButton);
        reader.SetValue("atmolight", "menubutton", (int)menuButton);
        reader.SetValueAsBool("atmolight", "OffOnStart", manualMode);
        reader.SetValueAsBool("atmolight", "SBS_3D_ON", sbs3dOn);
        reader.SetValueAsBool("atmolight", "lowCPU", lowCPU);
        reader.SetValue("atmolight", "lowCPUTime", lowCPUTime);
        reader.SetValueAsBool("atmolight", "Delay", delay);
        reader.SetValue("atmolight", "DelayTime", delayReferenceTime);
        reader.SetValueAsBool("atmolight", "ExitAtmoWin", exitAtmoWin);
        reader.SetValueAsBool("atmolight", "StartAtmoWin", startAtmoWin);
        reader.SetValueAsBool("atmolight", "atmoWakeHelperEnabled", atmoWakeHelperEnabled);
        reader.SetValue("atmolight", "atmoWakeHelperComPort", atmoWakeHelperComPort);
        reader.SetValue("atmolight", "atmoWakeHelperResumeDelay", atmoWakeHelperResumeDelay);
        reader.SetValue("atmolight", "atmoWakeHelperDisconnectDelay", atmoWakeHelperDisconnectDelay);
        reader.SetValue("atmolight", "atmoWakeHelperConnectDelay", atmoWakeHelperConnectDelay);
        reader.SetValue("atmolight", "atmoWakeHelperReinitializationDelay", atmoWakeHelperReinitializationDelay);
        reader.SetValue("atmolight", "excludeTimeStart", excludeTimeStart.ToString("HH:mm"));
        reader.SetValue("atmolight", "excludeTimeEnd", excludeTimeEnd.ToString("HH:mm"));
        reader.SetValue("atmolight", "CurrentLanguageFile", currentLanguageFile);
        reader.SetValue("atmolight", "StaticColorRed", staticColorRed);
        reader.SetValue("atmolight", "StaticColorGreen", staticColorGreen);
        reader.SetValue("atmolight", "StaticColorBlue", staticColorBlue);
        reader.SetValueAsBool("atmolight", "RestartOnError", restartOnError);
        reader.SetValueAsBool("atmolight", "TrueGrabbing", trueGrabbing);
        reader.SetValue("atmolight", "DelayRefreshRate", delayReferenceRefreshRate);
        reader.SetValueAsBool("atmolight", "BlackbarDetection", blackbarDetection);
        reader.SetValue("atmolight", "BlackbarDetectionTime", blackbarDetectionTime);
        reader.SetValue("atmolight", "GIFFile", gifFile);
        reader.SetValue("atmolight", "captureWidth", (int)captureWidth);
        reader.SetValue("atmolight", "captureHeight", (int)captureHeight);
        reader.SetValueAsBool("atmolight", "monitorScreensaverState", monitorScreensaverState);
        reader.SetValueAsBool("atmolight", "monitorWindowState", monitorWindowState);
        reader.SetValue("atmolight", "hyperionIP", hyperionIP);
        reader.SetValue("atmolight", "hyperionPort", (int)hyperionPort);
        reader.SetValue("atmolight", "hyperionPriority", (int)hyperionPriority);
        reader.SetValue("atmolight", "hyperionReconnectDelay", (int)hyperionReconnectDelay);
        reader.SetValue("atmolight", "hyperionReconnectAttempts", (int)hyperionReconnectAttempts);
        reader.SetValue("atmolight", "hyperionStaticColorPriority", (int)hyperionPriorityStaticColor);
        reader.SetValueAsBool("atmolight", "hyperionLiveReconnect", hyperionLiveReconnect);
        reader.SetValue("atmolight", "hueExe", hueExe);
        reader.SetValueAsBool("atmolight", "hueStart", hueStart);
        reader.SetValueAsBool("atmolight", "hueIsRemoteMachine", hueIsRemoteMachine);
        reader.SetValue("atmolight", "hueIP", hueIP);
        reader.SetValue("atmolight", "huePort", (int)huePort);
        reader.SetValue("atmolight", "hueReconnectDelay", (int)hueReconnectDelay);
        reader.SetValue("atmolight", "hueReconnectAttempts", (int)hueReconnectAttempts);
        reader.SetValueAsBool("atmolight", "hueBridgeEnableOnResume", hueBridgeEnableOnResume);
        reader.SetValueAsBool("atmolight", "hueBridgeDisableOnSuspend", hueBridgeDisableOnSuspend);
        reader.SetValueAsBool("atmolight", "hueTheaterEnabled", hueTheaterEnabled);
        reader.SetValueAsBool("atmolight", "hueTheaterRestoreLights", hueTheaterRestoreLights);
        reader.SetValue("atmolight", "boblightIP", boblightIP);
        reader.SetValue("atmolight", "boblightPort", boblightPort);
        reader.SetValue("atmolight", "boblightMaxFPS", boblightMaxFPS);
        reader.SetValue("atmolight", "boblightMaxReconnectAttempts", boblightMaxReconnectAttempts);
        reader.SetValue("atmolight", "boblightReconnectDelay", boblightReconnectDelay);
        reader.SetValue("atmolight", "boblightSpeed", boblightSpeed);
        reader.SetValue("atmolight", "boblightAutospeed", boblightAutospeed);
        reader.SetValue("atmolight", "boblightSaturation", boblightSaturation);
        reader.SetValue("atmolight", "boblightValue", boblightValue);
        reader.SetValue("atmolight", "boblightThreshold", boblightThreshold);
        reader.SetValueAsBool("atmolight", "boblightInterpolation", boblightInterpolation);
        reader.SetValue("atmolight", "boblightGamma", boblightGamma.ToString());
        reader.SetValue("atmolight", "blackbarDetectionThreshold", blackbarDetectionThreshold.ToString());
        reader.SetValue("atmolight", "powerModeChangedDelay", powerModeChangedDelay.ToString());
        reader.SetValue("atmolight", "ambiBoxIP", ambiBoxIP.ToString());
        reader.SetValue("atmolight", "ambiBoxPort", ambiBoxPort.ToString());
        reader.SetValue("atmolight", "ambiBoxMaxReconnectAttempts", ambiBoxMaxReconnectAttempts.ToString());
        reader.SetValue("atmolight", "ambiBoxReconnectDelay", ambiBoxReconnectDelay.ToString());
        reader.SetValue("atmolight", "ambiBoxChangeImageDelay", ambiBoxChangeImageDelay.ToString());
        reader.SetValue("atmolight", "ambiBoxMediaPortalProfile", ambiBoxMediaPortalProfile.ToString());
        reader.SetValue("atmolight", "ambiBoxExternalProfile", ambiBoxExternalProfile.ToString());
        reader.SetValue("atmolight", "ambiBoxPath", ambiBoxPath.ToString());
        reader.SetValueAsBool("atmolight", "ambiBoxAutoStart", ambiBoxAutoStart);
        reader.SetValueAsBool("atmolight", "ambiBoxAutoStop", ambiBoxAutoStop);
        reader.SetValue("atmolight", "vuMeterMindB", vuMeterMindB.ToString());
        reader.SetValue("atmolight", "vuMeterMinHue", vuMeterMinHue.ToString());
        reader.SetValue("atmolight", "vuMeterMaxHue", vuMeterMaxHue.ToString());
        reader.SetValueAsBool("atmolight", "atmoWinTarget", atmoWinTarget);
        reader.SetValueAsBool("atmolight", "boblightTarget", boblightTarget);
        reader.SetValueAsBool("atmolight", "hueTarget", hueTarget);
        reader.SetValueAsBool("atmolight", "hyperionTarget", hyperionTarget);
        reader.SetValueAsBool("atmolight", "ambiBoxTarget", ambiBoxTarget);
        reader.SetValueAsBool("atmolight", "atmoOrbTarget", atmoOrbTarget);
        reader.SetValue("atmolight", "atmoOrbBlackThreshold", atmoOrbBlackThreshold.ToString());
        reader.SetValue("atmolight", "atmoOrbBroadcastPort", atmoOrbBroadcastPort.ToString());
        reader.SetValue("atmolight", "atmoOrbGamma", atmoOrbGamma.ToString());
        reader.SetValue("atmolight", "atmoOrbMinDiversion", atmoOrbMinDiversion.ToString());
        reader.SetValue("atmolight", "atmoOrbSaturation", atmoOrbSaturation.ToString());
        reader.SetValue("atmolight", "atmoOrbThreshold", atmoOrbThreshold.ToString());
        reader.SetValueAsBool("atmolight", "atmoOrbUseOverallLightness", atmoOrbUseOverallLightness);
        reader.SetValueAsBool("atmolight", "atmoOrbUseSmoothing", atmoOrbUseSmoothing);
        reader.SetValue("atmolight", "atmoOrbSmoothingThreshold", atmoOrbSmoothingThreshold.ToString());

        string atmoOrbLampsTemp = "";
        for (int i = 0; i < atmoOrbLamps.Count; i++)
        {
          if (i > 0)
          {
            atmoOrbLampsTemp += "|";
          }
          atmoOrbLampsTemp += atmoOrbLamps[i];
        }
        reader.SetValue("atmolight", "atmoOrbLamps", atmoOrbLampsTemp);
        reader.SetValue("atmolight", "hueMinDiversion", hueMinDiversion.ToString());
        reader.SetValue("atmolight", "hueThreshold", hueThreshold.ToString());
        reader.SetValue("atmolight", "hueBlackThreshold", hueBlackThreshold.ToString());
        reader.SetValue("atmolight", "hueSaturation", hueSaturation.ToString());
        reader.SetValueAsBool("atmolight", "hueUseOverallLightness", hueUseOverallLightness);
        reader.SetValueAsBool("atmolight", "blackbarDetectionHorizontal", blackbarDetectionHorizontal);
        reader.SetValueAsBool("atmolight", "blackbarDetectionVertical", blackbarDetectionVertical);
        reader.SetValueAsBool("atmolight", "blackbarDetectionLinkAreas", blackbarDetectionLinkAreas);
        reader.SetValueAsBool("atmolight", "remoteApiServer", remoteApiServer);
      }
    }

    public static void SaveSpecificSetting(string Setting, String Value)
    {
      using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
      {
        reader.SetValue("atmolight", Setting, Value);
      }
    }

    // Legacy Support
    /// <summary>
    /// Converts old ContentEffect integer (before rework) into new ContentEffect.
    /// This is done to not lose settings after updating AtmoLight.
    /// </summary>
    /// <param name="effectInt"></param>
    /// <returns></returns>
    private static ContentEffect OldIntToNewContentEffect(int effectInt)
    {
      switch (effectInt)
      {
        case 0:
          return ContentEffect.LEDsDisabled;
        case 1:
          return ContentEffect.ExternalLiveMode;
        case 2:
          return ContentEffect.AtmoWinColorchanger;
        case 3:
          return ContentEffect.AtmoWinColorchangerLR;
        case 4:
          return ContentEffect.MediaPortalLiveMode;
        case 5:
          return ContentEffect.StaticColor;
        case 6:
          return ContentEffect.GIFReader;
        case 7:
          return ContentEffect.VUMeter;
        case 8:
          return ContentEffect.VUMeterRainbow;
        default:
          return ContentEffect.LEDsDisabled;
      }
    }
    #endregion
  }
}