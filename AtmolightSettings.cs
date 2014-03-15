using System;
using MediaPortal.Profile;
using Language;

namespace MediaPortal.ProcessPlugins.Atmolight
{
  public enum ContentEffect
  {
    LEDs_disabled = 0,
    AtmoWin_GDI_Live_view,
    Colorchanger,
    Colorchanger_LR,
    MP_Live_view,
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
    public static int killbutton = 0;
    public static int cmbutton = 0;
    public static int menubutton = 0;
    public static bool disableOnShutdown = true;
    public static bool enableInternalLiveView = false;
    public static bool HateTheStopThing = false;
    public static bool SBS_3D_ON = false;
    public static bool OffOnStart = false;
    public static bool lowCPU = false;
    public static int lowCPUTime = 0;
    public static bool startAtmoWin = true;
    public static bool exitAtmoWin = true;
    public static DateTime excludeTimeStart;
    public static DateTime excludeTimeEnd;
    public static int StaticColorRed = 0, StaticColorGreen = 0, StaticColorBlue = 0;
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
        killbutton = reader.GetValueAsInt("atmolight", "killbutton", 4);
        cmbutton = reader.GetValueAsInt("atmolight", "cmbutton", 4);
        menubutton = reader.GetValueAsInt("atmolight", "menubutton", 4);
        disableOnShutdown = reader.GetValueAsBool("atmolight", "disableOnShutdown", true);
        enableInternalLiveView = reader.GetValueAsBool("atmolight", "enableInternalLiveView", false);
        excludeTimeStart = LoadTimeSetting(reader, "excludeTimeStart", "08:00");
        excludeTimeEnd = LoadTimeSetting(reader, "excludeTimeEnd", "21:00");
        HateTheStopThing = reader.GetValueAsBool("atmolight", "dontstop", false);
        OffOnStart = reader.GetValueAsBool("atmolight", "OffOnStart", false);
        SBS_3D_ON = reader.GetValueAsBool("atmolight", "SBS_3D_ON", false);
        lowCPU = reader.GetValueAsBool("atmolight", "lowCPU", false);
        lowCPUTime = reader.GetValueAsInt("atmolight", "lowCPUTime", 0);
        exitAtmoWin = reader.GetValueAsBool("atmolight", "ExitAtmoWin", true);
        startAtmoWin = reader.GetValueAsBool("atmolight", "StartAtmoWin", true);
        StaticColorRed = reader.GetValueAsInt("atmolight", "StaticColorRed", 0);
        StaticColorGreen = reader.GetValueAsInt("atmolight", "StaticColorGreen", 0);
        StaticColorBlue = reader.GetValueAsInt("atmolight", "StaticColorBlue", 0);
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
        reader.SetValue("atmolight", "killbutton", (int)killbutton);
        reader.SetValue("atmolight", "cmbutton", (int)cmbutton);
        reader.SetValue("atmolight", "menubutton", (int)menubutton);
        reader.SetValueAsBool("atmolight", "disableOnShutdown", disableOnShutdown);
        reader.SetValueAsBool("atmolight", "dontstop", HateTheStopThing);
        reader.SetValueAsBool("atmolight", "OffOnStart", OffOnStart);
        reader.SetValueAsBool("atmolight", "SBS_3D_ON", SBS_3D_ON);
        reader.SetValueAsBool("atmolight", "lowCPU", lowCPU);
        reader.SetValue("atmolight", "lowCPUTime", lowCPUTime);
        reader.SetValueAsBool("atmolight", "ExitAtmoWin", exitAtmoWin);
        reader.SetValueAsBool("atmolight", "StartAtmoWin", startAtmoWin);
        reader.SetValueAsBool("atmolight", "enableInternalLiveView", enableInternalLiveView);
        reader.SetValue("atmolight", "excludeTimeStart", excludeTimeStart.ToString("HH:mm"));
        reader.SetValue("atmolight", "excludeTimeEnd", excludeTimeEnd.ToString("HH:mm"));
        reader.SetValue("atmolight", "CurrentLanguageFile", LanguageLoader.strCurrentLanguageFile);
        reader.SetValue("atmolight", "StaticColorRed", StaticColorRed);
        reader.SetValue("atmolight", "StaticColorGreen", StaticColorGreen);
        reader.SetValue("atmolight", "StaticColorBlue", StaticColorBlue);
      }
    }
  }
}
