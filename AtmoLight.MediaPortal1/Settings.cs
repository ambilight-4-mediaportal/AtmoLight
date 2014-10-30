﻿using System;
using MediaPortal.Profile;
using Language;

namespace AtmoLight
{
  public class Settings
  {
    #region Config variables

    //Generic
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

    //Atmowin
    public static bool atmoWinTarget;
    public static string atmowinExe = "";
    public static bool startAtmoWin = true;
    public static bool exitAtmoWin = true;

    //Hyperion
    public static bool hyperionTarget;
    public static string hyperionIP = "";
    public static int hyperionPort = 0;
    public static int hyperionPriority = 0;
    public static int hyperionReconnectDelay = 0;
    public static int hyperionReconnectAttempts = 0;
    public static int HyperionPriorityStaticColor = 0;
    public static bool HyperionLiveReconnect;

    //Hue
    public static bool hueTarget;
    public static string hueIP = "";
    public static int huePort = 0;

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
        atmowinExe = reader.GetValueAsString("atmolight", "atmowinexe", "");
        effectVideo = (ContentEffect)reader.GetValueAsInt("atmolight", "effectVideo", 4);
        effectMusic = (ContentEffect)reader.GetValueAsInt("atmolight", "effectMusic", 1);
        effectRadio = (ContentEffect)reader.GetValueAsInt("atmolight", "effectRadio", 0);
        effectMenu = (ContentEffect)reader.GetValueAsInt("atmolight", "effectMenu", 0);
        effectMPExit = (ContentEffect)reader.GetValueAsInt("atmolight", "effectMPExit", 0);
        killButton = reader.GetValueAsInt("atmolight", "killbutton", 4);
        profileButton = reader.GetValueAsInt("atmolight", "cmbutton", 4);
        menuButton = reader.GetValueAsInt("atmolight", "menubutton", 4);
        excludeTimeStart = LoadTimeSetting(reader, "excludeTimeStart", "08:00");
        excludeTimeEnd = LoadTimeSetting(reader, "excludeTimeEnd", "21:00");
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
        hyperionIP = reader.GetValueAsString("atmolight", "hyperionIP", "0.0.0.0");
        hyperionPort = reader.GetValueAsInt("atmolight", "hyperionPort", 19445);
        hyperionReconnectDelay = reader.GetValueAsInt("atmolight", "hyperionReconnectDelay", 10000);
        hyperionReconnectAttempts = reader.GetValueAsInt("atmolight", "hyperionReconnectAttempts", 5);
        hyperionPriority = reader.GetValueAsInt("atmolight", "hyperionPriority", 1);
        HyperionPriorityStaticColor = reader.GetValueAsInt("atmolight", "hyperionStaticColorPriority", 1);
        HyperionLiveReconnect = reader.GetValueAsBool("atmolight", "HyperionLiveReconnect", false);
        hueIP = reader.GetValueAsString("atmolight", "hueIP", "127.0.0.1");
        huePort = reader.GetValueAsInt("atmolight", "huePort", 20123);
        atmoWinTarget = reader.GetValueAsBool("atmolight", "atmoWinTarget", true);
        hueTarget = reader.GetValueAsBool("atmolight", "hueTarget", false);
        hyperionTarget = reader.GetValueAsBool("atmolight", "hyperionTarget", false);

      }
    }
    public static void SaveSettings()
    {
      using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
      {
        reader.SetValue("atmolight", "atmowinexe", atmowinExe);
        reader.SetValue("atmolight", "effectVideo", (int)effectVideo);
        reader.SetValue("atmolight", "effectMusic", (int)effectMusic);
        reader.SetValue("atmolight", "effectRadio", (int)effectRadio);
        reader.SetValue("atmolight", "effectMenu", (int)effectMenu);
        reader.SetValue("atmolight", "effectMPExit", (int)effectMPExit);
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
        reader.SetValue("atmolight", "hyperionStaticColorPriority", (int)HyperionPriorityStaticColor);
        reader.SetValueAsBool("atmolight", "HyperionLiveReconnect", HyperionLiveReconnect);     
        reader.SetValue("atmolight", "hueIP", hueIP);
        reader.SetValue("atmolight", "huePort", (int)huePort);
        reader.SetValueAsBool("atmolight", "atmoWinTarget", atmoWinTarget);
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
  }
}
