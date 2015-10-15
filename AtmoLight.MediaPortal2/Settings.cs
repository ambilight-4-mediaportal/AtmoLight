using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Common;
using MediaPortal.Common.Settings;

namespace AtmoLight
{
  public class Settings
  {
    [Setting(SettingScope.User, "C:\\ProgramData\\Team MediaPortal\\MediaPortal\\AtmoWin\\AtmoWinA.exe")]
    public string AtmoWinExe { get; set; }

    [Setting(SettingScope.User, ContentEffect.MediaPortalLiveMode)]
    public ContentEffect VideoEffect { get; set; }

    [Setting(SettingScope.User, ContentEffect.LEDsDisabled)]
    public ContentEffect AudioEffect { get; set; }

    [Setting(SettingScope.User, ContentEffect.LEDsDisabled)]
    public ContentEffect MenuEffect { get; set; }

    [Setting(SettingScope.User, ContentEffect.LEDsDisabled)]
    public ContentEffect MPExitEffect { get; set; }

    [Setting(SettingScope.User, 1)]
    public int OnOffButton { get; set; }

    [Setting(SettingScope.User, 2)]
    public int ProfileButton { get; set; }

    [Setting(SettingScope.User, "00:00")]
    public string ExcludeTimeStart { get; set; }

    [Setting(SettingScope.User, "00:00")]
    public string ExcludeTimeEnd { get; set; }

    [Setting(SettingScope.User, false)]
    public bool ManualMode { get; set; }

    [Setting(SettingScope.User, false)]
    public bool SBS3D { get; set; }

    [Setting(SettingScope.User, false)]
    public bool LowCPU { get; set; }

    [Setting(SettingScope.User, 0)]
    public int LowCPUTime { get; set; }
    
    [Setting(SettingScope.User, false)]
    public bool Delay { get; set; }
    
    [Setting(SettingScope.User, 0)]
    public int DelayTime { get; set; }
    
    [Setting(SettingScope.User, 50)]
    public int DelayRefreshRate { get; set; }
    
    [Setting(SettingScope.User, true)]
    public bool StopAtmoWinOnExit { get; set; }
    
    [Setting(SettingScope.User, true)]
    public bool StartAtmoWinOnStart { get; set; }
    
    [Setting(SettingScope.User, false)]
    public bool AtmoWakeHelperEnabled { get; set; }

    [Setting(SettingScope.User, "COM1")]
    public string AtmoWakeHelperComPort { get; set; }

    [Setting(SettingScope.User, 100)]
    public int AtmoWakeHelperResumeDelay { get; set; }

    [Setting(SettingScope.User, 100)]
    public int AtmoWakeHelperDisconnectDelay { get; set; }

    [Setting(SettingScope.User, 100)]
    public int AtmoWakeHelperConnectDelay { get; set; }

    [Setting(SettingScope.User, 0)]
    public int AtmoWakeHelperReinitializationDelay { get; set; }

    [Setting(SettingScope.User, true)]
    public bool RestartAtmoWinOnError { get; set; }
    
    [Setting(SettingScope.User, 0)]
    public int StaticColorRed { get; set; }
        
    [Setting(SettingScope.User, 0)]
    public int StaticColorGreen { get; set; }
        
    [Setting(SettingScope.User, 0)]
    public int StaticColorBlue { get; set; }

    [Setting(SettingScope.User, "")]
    public string GIFFile { get; set; }

    [Setting(SettingScope.User, true)]
    public bool AtmoWinTarget { get; set; }

    [Setting(SettingScope.User, false)]
    public bool HyperionTarget { get; set; }

    [Setting(SettingScope.User, false)]
    public bool HueTarget { get; set; }

    [Setting(SettingScope.User, 64)]
    public int CaptureWidth { get; set; }

    [Setting(SettingScope.User, 64)]
    public int CaptureHeight { get; set; }

    [Setting(SettingScope.User, true)]
    public bool MonitorScreensaverState { get; set; }

    [Setting(SettingScope.User, "127.0.0.1")]
    public string HyperionIP { get; set; }

    [Setting(SettingScope.User, 19445)]
    public int HyperionPort { get; set; }

    [Setting(SettingScope.User, 1)]
    public int HyperionPriority { get; set; }

    [Setting(SettingScope.User, 1)]
    public int HyperionPriorityStaticColor { get; set; }

    [Setting(SettingScope.User, 10000)]
    public int HyperionReconnectDelay { get; set; }

    [Setting(SettingScope.User, 5)]
    public int HyperionReconnectAttempts { get; set; }

    [Setting(SettingScope.User, false)]
    public bool HyperionLiveReconnect { get; set; }

    [Setting(SettingScope.User, "")]
    public string hueExe { get; set; }

    [Setting(SettingScope.User, true)]
    public bool hueStart { get; set; }

    [Setting(SettingScope.User, false)]
    public bool hueIsRemoteMachine { get; set; }

    [Setting(SettingScope.User, "127.0.0.1")]
    public string HueIP { get; set; }

    [Setting(SettingScope.User, 20123)]
    public int HuePort { get; set; }

    [Setting(SettingScope.User, 10000)]
    public int HueReconnectDelay { get; set; }

    [Setting(SettingScope.User, 5)]
    public int HueReconnectAttempts { get; set; }

    [Setting(SettingScope.User, false)]
    public bool HueBridgeEnableOnResume { get; set; }

    [Setting(SettingScope.User, false)]
    public bool HueBridgeDisableOnSuspend { get; set; }

    [Setting(SettingScope.User, 16)]
    public int HueThreshold { get; set; }

    [Setting(SettingScope.User, 16)]
    public int HueBlackThreshold { get; set; }

    [Setting(SettingScope.User, 16)]
    public int HueMinDiversion { get; set; }

    [Setting(SettingScope.User, 0.2)]
    public double HueSaturation { get; set; }

    [Setting(SettingScope.User, true)]
    public bool HueUseOverallLightness { get; set; }

    [Setting(SettingScope.User, false)]
    public bool HueTheaterEnabled { get; set; }

    [Setting(SettingScope.User, false)]
    public bool HueTheaterRestoreLights { get; set; }
    
    [Setting(SettingScope.User, false)]
    public bool BoblightTarget { get; set; }

    [Setting(SettingScope.User, "127.0.0.1")]
    public string BoblightIP { get; set; }

    [Setting(SettingScope.User, 19333)]
    public int BoblightPort { get; set; }

    [Setting(SettingScope.User, 10)]
    public int BoblightMaxFPS { get; set; }

    [Setting(SettingScope.User, 5)]
    public int BoblightMaxReconnectAttempts { get; set; }

    [Setting(SettingScope.User, 5000)]
    public int BoblightReconnectDelay { get; set; }

    [Setting(SettingScope.User, 100)]
    public int BoblightSpeed { get; set; }

    [Setting(SettingScope.User, 0)]
    public int BoblightAutospeed { get; set; }

    [Setting(SettingScope.User, true)]
    public bool BoblightInterpolation { get; set; }

    [Setting(SettingScope.User, 1)]
    public int BoblightSaturation { get; set; }

    [Setting(SettingScope.User, 1)]
    public int BoblightValue { get; set; }

    [Setting(SettingScope.User, 20)]
    public int BoblightThreshold { get; set; }

    [Setting(SettingScope.User, 2.2)]
    public double BoblightGamma { get; set; }

    [Setting(SettingScope.User, false)]
    public bool BlackbarDetection { get; set; }

    [Setting(SettingScope.User, 1000)]
    public int BlackbarDetectionTime { get; set; }

    [Setting(SettingScope.User, 20)]
    public int BlackbarDetectionThreshold { get; set; }

    [Setting(SettingScope.User, 5000)]
    public int PowerModeChangedDelay { get; set; }

    [Setting(SettingScope.User, "127.0.0.1")]
    public string AmbiBoxIP { get; set; }

    [Setting(SettingScope.User, 3636)]
    public int AmbiBoxPort { get; set; }

    [Setting(SettingScope.User, 5)]
    public int AmbiBoxMaxReconnectAttempts { get; set; }

    [Setting(SettingScope.User, 5000)]
    public int AmbiBoxReconnectDelay { get; set; }

    [Setting(SettingScope.User, "")]
    public string AmbiBoxMediaPortalProfile { get; set; }

    [Setting(SettingScope.User, "")]
    public string AmbiBoxExternalProfile { get; set; }

    [Setting(SettingScope.User, "C:\\Program Files (x86)\\AmbiBox\\AmbiBox.exe")]
    public string AmbiBoxPath { get; set; }

    [Setting(SettingScope.User, false)]
    public bool AmbiBoxAutoStart { get; set; }

    [Setting(SettingScope.User, false)]
    public bool AmbiBoxAutoStop { get; set; }

    [Setting(SettingScope.User, false)]
    public bool AmbiBoxTarget { get; set; }

    [Setting(SettingScope.User, true)]
    public bool BlackbarDetectionLinkAreas { get; set; }

    [Setting(SettingScope.User, true)]
    public bool BlackbarDetectionHorizontal { get; set; }

    [Setting(SettingScope.User, true)]
    public bool BlackbarDetectionVertical { get; set; }

    [Setting(SettingScope.User, false)]
    public bool RemoteApiServer { get; set; }


    ISettingsManager settingsManager = ServiceRegistration.Get<ISettingsManager>();
    Settings settings;

    public bool LoadAll()
    {
      settings = settingsManager.Load<Settings>();
      AtmoWinExe = settings.AtmoWinExe;
      VideoEffect = settings.VideoEffect;
      AudioEffect = settings.AudioEffect;
      MenuEffect = settings.MenuEffect;
      OnOffButton = settings.OnOffButton;
      ProfileButton = settings.ProfileButton;
      ExcludeTimeStart = settings.ExcludeTimeStart;
      ExcludeTimeEnd = settings.ExcludeTimeEnd;
      ManualMode = settings.ManualMode;
      SBS3D = settings.SBS3D;
      LowCPU = settings.LowCPU;
      LowCPUTime = settings.LowCPUTime;
      Delay = settings.Delay;
      DelayTime = settings.DelayTime;
      DelayRefreshRate = settings.DelayRefreshRate;
      StopAtmoWinOnExit = settings.StopAtmoWinOnExit;
      StartAtmoWinOnStart = settings.StartAtmoWinOnStart;
      AtmoWakeHelperEnabled = settings.AtmoWakeHelperEnabled;
      AtmoWakeHelperComPort = settings.AtmoWakeHelperComPort;
      AtmoWakeHelperResumeDelay = settings.AtmoWakeHelperResumeDelay;
      AtmoWakeHelperDisconnectDelay = settings.AtmoWakeHelperDisconnectDelay;
      AtmoWakeHelperConnectDelay = settings.AtmoWakeHelperConnectDelay;
      AtmoWakeHelperReinitializationDelay = settings.AtmoWakeHelperReinitializationDelay;
      RestartAtmoWinOnError = settings.RestartAtmoWinOnError;
      MonitorScreensaverState = settings.MonitorScreensaverState;
      StaticColorBlue = settings.StaticColorBlue;
      StaticColorGreen = settings.StaticColorGreen;
      StaticColorRed = settings.StaticColorRed;
      GIFFile = settings.GIFFile;
      MPExitEffect = settings.MPExitEffect;
      AtmoWinTarget = settings.AtmoWinTarget;
      HyperionTarget = settings.HyperionTarget;
      HueTarget = settings.HueTarget;
      HyperionIP = settings.HyperionIP;
      HyperionLiveReconnect = settings.HyperionLiveReconnect;
      HyperionPort = settings.HyperionPort;
      HyperionPriority = settings.HyperionPriority;
      HyperionPriorityStaticColor = settings.HyperionPriorityStaticColor;
      HyperionReconnectAttempts = settings.HyperionReconnectAttempts;
      HyperionReconnectDelay = settings.HyperionReconnectDelay;
      hueExe = settings.hueExe;
      hueStart = settings.hueStart;
      hueIsRemoteMachine = settings.hueIsRemoteMachine;
      HueIP = settings.HueIP;
      HuePort = settings.HuePort;
      HueReconnectDelay = settings.HueReconnectDelay;
      HueReconnectAttempts = settings.HueReconnectAttempts;
      HueBridgeEnableOnResume = settings.HueBridgeEnableOnResume;
      CaptureHeight = settings.CaptureHeight;
      CaptureWidth = settings.CaptureWidth;
      BoblightTarget = settings.BoblightTarget;
      BoblightIP = settings.BoblightIP;
      BoblightPort = settings.BoblightPort;
      BoblightMaxFPS = settings.BoblightMaxFPS;
      BoblightMaxReconnectAttempts = settings.BoblightMaxReconnectAttempts;
      BoblightReconnectDelay = settings.BoblightReconnectDelay;
      BoblightSpeed = settings.BoblightSpeed;
      BoblightAutospeed = settings.BoblightAutospeed;
      BoblightInterpolation = settings.BoblightInterpolation;
      BoblightSaturation = settings.BoblightSaturation;
      BoblightValue = settings.BoblightValue;
      BoblightThreshold = settings.BoblightThreshold;
      BoblightGamma = settings.BoblightGamma;
      BlackbarDetection = settings.BlackbarDetection;
      BlackbarDetectionTime = settings.BlackbarDetectionTime;
      BlackbarDetectionThreshold = settings.BlackbarDetectionThreshold;
      PowerModeChangedDelay = settings.PowerModeChangedDelay;
      AmbiBoxAutoStart = settings.AmbiBoxAutoStart;
      AmbiBoxAutoStop = settings.AmbiBoxAutoStop;
      AmbiBoxExternalProfile = settings.AmbiBoxExternalProfile;
      AmbiBoxIP = settings.AmbiBoxIP;
      AmbiBoxMaxReconnectAttempts = settings.AmbiBoxMaxReconnectAttempts;
      AmbiBoxMediaPortalProfile = settings.AmbiBoxMediaPortalProfile;
      AmbiBoxPath = settings.AmbiBoxPath;
      AmbiBoxPort = settings.AmbiBoxPort;
      AmbiBoxReconnectDelay = settings.AmbiBoxReconnectDelay;
      AmbiBoxTarget = settings.AmbiBoxTarget;
      HueThreshold = settings.HueThreshold;
      HueBlackThreshold = settings.HueBlackThreshold;
      HueMinDiversion = settings.HueMinDiversion;
      HueSaturation = settings.HueSaturation;
      HueUseOverallLightness = settings.HueUseOverallLightness;
      HueTheaterEnabled = settings.HueTheaterEnabled;
      HueTheaterRestoreLights = settings.HueTheaterRestoreLights;
      BlackbarDetectionLinkAreas = settings.BlackbarDetectionLinkAreas;
      BlackbarDetectionHorizontal = settings.BlackbarDetectionHorizontal;
      BlackbarDetectionVertical = settings.BlackbarDetectionVertical;
      RemoteApiServer = settings.RemoteApiServer;
      return true;
    }

    public bool SaveAll()
    {
      settings.AtmoWinExe = AtmoWinExe;
      settings.VideoEffect = VideoEffect;
      settings.AudioEffect = AudioEffect;
      settings.MenuEffect = MenuEffect;
      settings.OnOffButton = OnOffButton;
      settings.ProfileButton = ProfileButton;
      settings.ExcludeTimeStart = ExcludeTimeStart;
      settings.ExcludeTimeEnd = ExcludeTimeEnd;
      settings.ManualMode = ManualMode;
      settings.SBS3D = SBS3D;
      settings.LowCPU = LowCPU;
      settings.LowCPUTime = LowCPUTime;
      settings.Delay = Delay;
      settings.DelayTime = DelayTime;
      settings.DelayRefreshRate = DelayRefreshRate;
      settings.StopAtmoWinOnExit = StopAtmoWinOnExit;
      settings.StartAtmoWinOnStart = StartAtmoWinOnStart;
      settings.RestartAtmoWinOnError = RestartAtmoWinOnError;
      settings.AtmoWakeHelperEnabled =AtmoWakeHelperEnabled;
      settings.AtmoWakeHelperComPort = AtmoWakeHelperComPort;
      settings.AtmoWakeHelperResumeDelay = AtmoWakeHelperResumeDelay;
      settings.AtmoWakeHelperDisconnectDelay = AtmoWakeHelperDisconnectDelay;
      settings.AtmoWakeHelperConnectDelay = AtmoWakeHelperConnectDelay;
      settings.AtmoWakeHelperReinitializationDelay = AtmoWakeHelperReinitializationDelay;
      settings.MonitorScreensaverState = MonitorScreensaverState;
      settings.StaticColorBlue = StaticColorBlue;
      settings.StaticColorGreen = StaticColorGreen;
      settings.StaticColorRed = StaticColorRed;
      settings.GIFFile = GIFFile;
      settings.MPExitEffect = MPExitEffect;
      settings.AtmoWinTarget = AtmoWinTarget;
      settings.HyperionTarget = HyperionTarget;
      settings.HueTarget = HueTarget;
      settings.HyperionIP = HyperionIP;
      settings.HyperionLiveReconnect = HyperionLiveReconnect;
      settings.HyperionPort = HyperionPort;
      settings.HyperionPriority = HyperionPriority;
      settings.HyperionPriorityStaticColor = HyperionPriorityStaticColor;
      settings.HyperionReconnectAttempts = HyperionReconnectAttempts;
      settings.HyperionReconnectDelay = HyperionReconnectDelay;
      settings.hueExe = hueExe;
      settings.hueStart = hueStart;
      settings.hueIsRemoteMachine = hueIsRemoteMachine;
      settings.HueIP = HueIP;
      settings.HuePort = HuePort;
      settings.HueReconnectDelay = HueReconnectDelay;
      settings.HueReconnectAttempts = HueReconnectAttempts;
      settings.HueBridgeEnableOnResume = HueBridgeEnableOnResume;
      settings.CaptureWidth = CaptureWidth;
      settings.CaptureHeight = CaptureHeight;
      settings.BoblightTarget = BoblightTarget;
      settings.BoblightIP = BoblightIP;
      settings.BoblightPort = BoblightPort;
      settings.BoblightMaxFPS = BoblightMaxFPS;
      settings.BoblightMaxReconnectAttempts = BoblightMaxReconnectAttempts;
      settings.BoblightReconnectDelay = BoblightReconnectDelay;
      settings.BoblightSpeed = BoblightSpeed;
      settings.BoblightAutospeed = BoblightAutospeed;
      settings.BoblightInterpolation = BoblightInterpolation;
      settings.BoblightSaturation = BoblightSaturation;
      settings.BoblightValue = BoblightValue;
      settings.BoblightThreshold = BoblightThreshold;
      settings.BoblightGamma = BoblightGamma;
      settings.BlackbarDetection = BlackbarDetection;
      settings.BlackbarDetectionTime = BlackbarDetectionTime;
      settings.BlackbarDetectionThreshold = BlackbarDetectionThreshold;
      settings.PowerModeChangedDelay = PowerModeChangedDelay;
      settings.AmbiBoxAutoStart = AmbiBoxAutoStart;
      settings.AmbiBoxAutoStop = AmbiBoxAutoStop;
      settings.AmbiBoxExternalProfile = AmbiBoxExternalProfile;
      settings.AmbiBoxIP = AmbiBoxIP;
      settings.AmbiBoxMaxReconnectAttempts = AmbiBoxMaxReconnectAttempts;
      settings.AmbiBoxMediaPortalProfile = AmbiBoxMediaPortalProfile;
      settings.AmbiBoxPath = AmbiBoxPath;
      settings.AmbiBoxPort = AmbiBoxPort;
      settings.AmbiBoxReconnectDelay = AmbiBoxReconnectDelay;
      settings.AmbiBoxTarget = AmbiBoxTarget;
      settings.HueThreshold = HueThreshold;
      settings.HueBlackThreshold = HueBlackThreshold;
      settings.HueMinDiversion = HueMinDiversion;
      settings.HueSaturation = HueSaturation;
      settings.HueUseOverallLightness = HueUseOverallLightness;
      settings.HueTheaterEnabled = HueTheaterEnabled;
      settings.HueTheaterRestoreLights = HueTheaterRestoreLights;
      settings.BlackbarDetectionLinkAreas = BlackbarDetectionLinkAreas;
      settings.BlackbarDetectionHorizontal = BlackbarDetectionHorizontal;
      settings.BlackbarDetectionVertical = BlackbarDetectionVertical;
      settings.RemoteApiServer = RemoteApiServer;
      settingsManager.Save(settings);
      return true;
    }
  }
}
