using System;
using MediaPortal.Profile;
using Language;
using System.Globalization;


namespace AtmoLight
{
  public class Settings
  {
    #region Config variables

    // Generic
    public static ContentEffect effectVideo;
    public static ContentEffect effectMusic;
    public static ContentEffect effectRadio;
    public static ContentEffect effectMenu;
    public static ContentEffect effectMPExit;
    public static int killButton = 0;
    public static int profileButton = 0;
    public static int menuButton = 0;
    public static bool sbs3dOn = false;
    public static bool manualMode = false;
    public static bool lowCPU = false;
    public static int lowCPUTime = 0;
    public static bool delay = false;
    public static int delayReferenceTime = 0;
    public static int delayReferenceRefreshRate = 0;
    public static DateTime excludeTimeStart;
    public static DateTime excludeTimeEnd;
    public static int staticColorRed = 0;
    public static int staticColorGreen = 0;
    public static int staticColorBlue = 0;
    public static bool restartOnError = true;
    public static bool blackbarDetection = false;
    public static int blackbarDetectionTime = 0;
    public static string gifFile = "";
    public static int captureWidth = 0;
    public static int captureHeight = 0;

    // Atmowin
    public static bool atmoWinTarget;
    public static string atmowinExe = "";
    public static bool startAtmoWin = true;
    public static bool exitAtmoWin = true;

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
    public static string hyperionIP = "";
    public static int hyperionPort = 0;
    public static int hyperionPriority = 0;
    public static int hyperionReconnectDelay = 0;
    public static int hyperionReconnectAttempts = 0;
    public static int hyperionPriorityStaticColor = 0;
    public static bool hyperionLiveReconnect;

    // Hue
    public static bool hueTarget;
    public static string hueExe = "";
    public static bool hueStart;
    public static bool hueIsRemoteMachine;
    public static string hueIP = "";
    public static int huePort = 0;
    public static int hueReconnectDelay = 0;
    public static int hueReconnectAttempts = 0;
    public static int hueMinimalColorDifference;

    #endregion

    public static DateTime LoadTimeSetting(MediaPortal.Profile.Settings reader, string name, string defaultTime)
    {
      string s = reader.GetValueAsString("atmolight", name, defaultTime);
      DateTime dt;
      if (!DateTime.TryParse(s, out dt))
        dt = DateTime.Parse(defaultTime);
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
        staticColorRed = reader.GetValueAsInt("atmolight", "StaticColorRed", 0);
        staticColorGreen = reader.GetValueAsInt("atmolight", "StaticColorGreen", 0);
        staticColorBlue = reader.GetValueAsInt("atmolight", "StaticColorBlue", 0);
        restartOnError = reader.GetValueAsBool("atmolight", "RestartOnError", true);
        delayReferenceRefreshRate = reader.GetValueAsInt("atmolight", "DelayRefreshRate", 50);
        blackbarDetection = reader.GetValueAsBool("atmolight", "BlackbarDetection", false);
        blackbarDetectionTime = reader.GetValueAsInt("atmolight", "BlackbarDetectionTime", 0);
        gifFile = reader.GetValueAsString("atmolight", "GIFFile", "");
        captureWidth = reader.GetValueAsInt("atmolight", "captureWidth", 64);
        captureHeight = reader.GetValueAsInt("atmolight", "captureHeight", 64);
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
        hueMinimalColorDifference = reader.GetValueAsInt("atmolight", "hueMinimalColorDifference", 25);
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
        reader.SetValue("atmolight", "excludeTimeStart", excludeTimeStart.ToString("HH:mm"));
        reader.SetValue("atmolight", "excludeTimeEnd", excludeTimeEnd.ToString("HH:mm"));
        reader.SetValue("atmolight", "CurrentLanguageFile", LanguageLoader.strCurrentLanguageFile);
        reader.SetValue("atmolight", "StaticColorRed", staticColorRed);
        reader.SetValue("atmolight", "StaticColorGreen", staticColorGreen);
        reader.SetValue("atmolight", "StaticColorBlue", staticColorBlue);
        reader.SetValueAsBool("atmolight", "RestartOnError", restartOnError);
        reader.SetValue("atmolight", "DelayRefreshRate", delayReferenceRefreshRate);
        reader.SetValueAsBool("atmolight", "BlackbarDetection", blackbarDetection);
        reader.SetValue("atmolight", "BlackbarDetectionTime", blackbarDetectionTime);
        reader.SetValue("atmolight", "GIFFile", gifFile);
        reader.SetValue("atmolight", "captureWidth", (int)captureWidth);
        reader.SetValue("atmolight", "captureHeight", (int)captureHeight);
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
        reader.SetValue("atmolight", "hueMinimalColorDifference", (int)hueMinimalColorDifference);
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

        reader.SetValueAsBool("atmolight", "atmoWinTarget", atmoWinTarget);
        reader.SetValueAsBool("atmolight", "boblightTarget", boblightTarget);
        reader.SetValueAsBool("atmolight", "hueTarget", hueTarget);
        reader.SetValueAsBool("atmolight", "hyperionTarget", hyperionTarget);
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
  }
}
