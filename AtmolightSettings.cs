using System;
using MediaPortal.Profile;
using Language;

namespace MediaPortal.ProcessPlugins.Atmolight
{
  public enum ContentEffect
  {
    LEDsDisabled = 0,
    AtmoWinLiveMode,
    Colorchanger,
    ColorchangerLR,
    MediaPortalLiveMode,
    StaticColor
  }
  public class AtmolightSettings
  {
    #region Config variables
    public static string atmowinExe = "";
    public static ContentEffect effectVideo;
    public static ContentEffect effectMusic;
    public static ContentEffect effectRadio;
    public static ContentEffect effectMenu;
    public static int killButton = 0;
    public static int profileButton = 0;
    public static int menuButton = 0;
    public static bool disableOnShutdown = true;
    public static bool enableInternalLiveView = false;
    public static bool sbs3dOn = false;
    public static bool manualMode = false;
    public static bool lowCPU = false;
    public static int lowCPUTime = 0;
    public static bool delay = false;
    public static int delayTime = 0;
    public static bool startAtmoWin = true;
    public static bool exitAtmoWin = true;
    public static DateTime excludeTimeStart;
    public static DateTime excludeTimeEnd;
    public static int staticColorRed = 0;
    public static int staticColorGreen = 0;
    public static int staticColorBlue = 0;
    public static bool restartOnError = true;
    #endregion

    public static DateTime LoadTimeSetting(Settings reader, string name, string defaultTime)
    {
      string s = reader.GetValueAsString("atmolight", name, defaultTime);
      DateTime dt;
      if (!DateTime.TryParse(s, out dt))
        dt = DateTime.Parse(defaultTime);
      return dt;
    }

    public static void LoadSettings()
    {
      using (Settings reader = new Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
      {
        atmowinExe = reader.GetValueAsString("atmolight", "atmowinexe", "");
        effectVideo = (ContentEffect)reader.GetValueAsInt("atmolight", "effectVideo", 4);
        effectMusic = (ContentEffect)reader.GetValueAsInt("atmolight", "effectMusic", 1);
        effectRadio = (ContentEffect)reader.GetValueAsInt("atmolight", "effectRadio", 0);
        effectMenu = (ContentEffect)reader.GetValueAsInt("atmolight", "effectMenu", 0);
        killButton = reader.GetValueAsInt("atmolight", "killbutton", 4);
        profileButton = reader.GetValueAsInt("atmolight", "cmbutton", 4);
        menuButton = reader.GetValueAsInt("atmolight", "menubutton", 4);
        disableOnShutdown = reader.GetValueAsBool("atmolight", "disableOnShutdown", true);
        enableInternalLiveView = reader.GetValueAsBool("atmolight", "enableInternalLiveView", false);
        excludeTimeStart = LoadTimeSetting(reader, "excludeTimeStart", "08:00");
        excludeTimeEnd = LoadTimeSetting(reader, "excludeTimeEnd", "21:00");
        manualMode = reader.GetValueAsBool("atmolight", "OffOnStart", false);
        sbs3dOn = reader.GetValueAsBool("atmolight", "SBS_3D_ON", false);
        lowCPU = reader.GetValueAsBool("atmolight", "lowCPU", false);
        lowCPUTime = reader.GetValueAsInt("atmolight", "lowCPUTime", 0);
        delay = reader.GetValueAsBool("atmolight", "Delay", false);
        delayTime = reader.GetValueAsInt("atmolight", "DelayTime", 0);
        exitAtmoWin = reader.GetValueAsBool("atmolight", "ExitAtmoWin", true);
        startAtmoWin = reader.GetValueAsBool("atmolight", "StartAtmoWin", true);
        staticColorRed = reader.GetValueAsInt("atmolight", "StaticColorRed", 0);
        staticColorGreen = reader.GetValueAsInt("atmolight", "StaticColorGreen", 0);
        staticColorBlue = reader.GetValueAsInt("atmolight", "StaticColorBlue", 0);
        restartOnError = reader.GetValueAsBool("atmolight", "RestartOnError", true);
      }
    }
    public static void SaveSettings()
    {
      using (Settings reader = new Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
      {
        reader.SetValue("atmolight", "atmowinexe", atmowinExe);
        reader.SetValue("atmolight", "effectVideo", (int)effectVideo);
        reader.SetValue("atmolight", "effectMusic", (int)effectMusic);
        reader.SetValue("atmolight", "effectRadio", (int)effectRadio);
        reader.SetValue("atmolight", "effectMenu", (int)effectMenu);
        reader.SetValue("atmolight", "killbutton", (int)killButton);
        reader.SetValue("atmolight", "cmbutton", (int)profileButton);
        reader.SetValue("atmolight", "menubutton", (int)menuButton);
        reader.SetValueAsBool("atmolight", "disableOnShutdown", disableOnShutdown);
        reader.SetValueAsBool("atmolight", "OffOnStart", manualMode);
        reader.SetValueAsBool("atmolight", "SBS_3D_ON", sbs3dOn);
        reader.SetValueAsBool("atmolight", "lowCPU", lowCPU);
        reader.SetValue("atmolight", "lowCPUTime", lowCPUTime);
        reader.SetValueAsBool("atmolight", "Delay", delay);
        reader.SetValue("atmolight", "DelayTime", delayTime);
        reader.SetValueAsBool("atmolight", "ExitAtmoWin", exitAtmoWin);
        reader.SetValueAsBool("atmolight", "StartAtmoWin", startAtmoWin);
        reader.SetValueAsBool("atmolight", "enableInternalLiveView", enableInternalLiveView);
        reader.SetValue("atmolight", "excludeTimeStart", excludeTimeStart.ToString("HH:mm"));
        reader.SetValue("atmolight", "excludeTimeEnd", excludeTimeEnd.ToString("HH:mm"));
        reader.SetValue("atmolight", "CurrentLanguageFile", LanguageLoader.strCurrentLanguageFile);
        reader.SetValue("atmolight", "StaticColorRed", staticColorRed);
        reader.SetValue("atmolight", "StaticColorGreen", staticColorGreen);
        reader.SetValue("atmolight", "StaticColorBlue", staticColorBlue);
        reader.SetValueAsBool("atmolight", "RestartOnError", restartOnError);
      }
    }

    public static void SaveSpecificSetting(string Setting, String Value)
    {
      using (Settings reader = new Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
      {
        reader.SetValue("atmolight", Setting, Value);
      }
    }
  }
}
