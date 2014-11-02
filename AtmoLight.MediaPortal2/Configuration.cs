using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MediaPortal.Common.Configuration.ConfigurationClasses;
using MediaPortal.Common.Localization;
using MediaPortal.UI.Presentation.UiNotifications;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.Common;

namespace AtmoLight.Configuration
{
  public class AtmoWinExe : PathEntry
  {
    public override void Load()
    {
      _path = SettingsManager.Load<Settings>().AtmoWinExe;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AtmoWinExe = _path + "AtmoWinA.exe";
      SettingsManager.Save(settings);

      Core.GetInstance().atmoWinPath = settings.AtmoWinExe;
      if (settings.AtmoWinTarget)
      {
        Core.GetInstance().RemoveTarget(Target.AtmoWin);
        Core.GetInstance().AddTarget(Target.AtmoWin);
        Core.GetInstance().Initialise();
      }
    }
  }

  public class GIFFile : PathEntry
  {
    public override void Load()
    {
      _pathSelectionType = PathSelectionType.File;
      _path = SettingsManager.Load<Settings>().GIFFile;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.GIFFile = _path;
      SettingsManager.Save(settings);

      Core.GetInstance().SetGIFPath(settings.GIFFile);
    }
  }

  public class VideoEffect : SingleSelectionList
  {
    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined && !AtmoLight.Plugin.unsupportedEffects.Contains(effect))
        {
          effectList.Add("[AtmoLight." + effect.ToString() + "]");
        }
      }

      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().VideoEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.VideoEffect = (ContentEffect)Enum.Parse(typeof(ContentEffect), effectList[Selected].Remove(effectList[Selected].Length - 1).Remove(0, 11));
      SettingsManager.Save(settings);

      if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
      {
        Core.GetInstance().ChangeEffect(settings.VideoEffect);
      }
    }
  }

  public class AudioEffect : SingleSelectionList
  {
    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined && !AtmoLight.Plugin.unsupportedEffects.Contains(effect))
        {
          effectList.Add("[AtmoLight." + effect.ToString() + "]");
        }
      }

      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().AudioEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AudioEffect = (ContentEffect)Enum.Parse(typeof(ContentEffect), effectList[Selected].Remove(effectList[Selected].Length - 1).Remove(0, 11));
      SettingsManager.Save(settings);

      if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
      {
        Core.GetInstance().ChangeEffect(settings.AudioEffect);
      }
    }
  }

  public class MenuEffect : SingleSelectionList
  {
    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined && !AtmoLight.Plugin.unsupportedEffects.Contains(effect))
        {
          effectList.Add("[AtmoLight." + effect.ToString() + "]");
        }
      }

      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().MenuEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.MenuEffect = (ContentEffect)Enum.Parse(typeof(ContentEffect), effectList[Selected].Remove(effectList[Selected].Length - 1).Remove(0, 11));
      SettingsManager.Save(settings);

      if (!ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive && !ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
      {
        Core.GetInstance().ChangeEffect(settings.MenuEffect);
      }
    }
  }

  public class MPExitEffect : SingleSelectionList
  {
    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined && !AtmoLight.Plugin.unsupportedEffects.Contains(effect))
        {
          if (effect != ContentEffect.MediaPortalLiveMode && effect != ContentEffect.GIFReader)
          {
            effectList.Add("[AtmoLight." + effect.ToString() + "]");
          }
        }
      }

      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().MPExitEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.MPExitEffect = (ContentEffect)Enum.Parse(typeof(ContentEffect), effectList[Selected].Remove(effectList[Selected].Length - 1).Remove(0, 11));
      SettingsManager.Save(settings);
    }
  }

  public class OnOffButton : SingleSelectionList
  {
    public delegate void OnOffButtonHander();
    public static event OnOffButtonHander NewOnOffButton;

    private IList<string> buttonList = new List<string>();
    public override void Load()
    {
      if (buttonList.Count == 0)
      {
        buttonList.Add("[AtmoLight.None]");
        buttonList.Add("[AtmoLight.Red]");
        buttonList.Add("[AtmoLight.Green]");
        buttonList.Add("[AtmoLight.Yellow]");
        buttonList.Add("[AtmoLight.Blue]");
      }

      Selected = SettingsManager.Load<Settings>().OnOffButton;

      _items = buttonList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      if ((buttonList.IndexOf(buttonList[Selected]) != settings.ProfileButton) || buttonList.IndexOf(buttonList[Selected]) == 0)
      {
        settings.OnOffButton = buttonList.IndexOf(buttonList[Selected]);
        SettingsManager.Save(settings);
        NewOnOffButton();
      }
      else
      {
        ServiceRegistration.Get<INotificationService>().EnqueueNotification(NotificationType.Error, "[AtmoLight.Name]", "[AtmoLight.ButtonError]", true);
      }
    }
  }

  public class ProfileButton : SingleSelectionList
  {
    public delegate void ProfileButtonHander();
    public static event ProfileButtonHander NewProfileButton;

    private IList<string> buttonList = new List<string>();
    public override void Load()
    {
      if (buttonList.Count == 0)
      {
        buttonList.Add("[AtmoLight.None]");
        buttonList.Add("[AtmoLight.Red]");
        buttonList.Add("[AtmoLight.Green]");
        buttonList.Add("[AtmoLight.Yellow]");
        buttonList.Add("[AtmoLight.Blue]");
      }

      Selected = SettingsManager.Load<Settings>().ProfileButton;

      _items = buttonList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      if ((buttonList.IndexOf(buttonList[Selected]) != settings.OnOffButton) || buttonList.IndexOf(buttonList[Selected]) == 0)
      {
        settings.ProfileButton = buttonList.IndexOf(buttonList[Selected]);
        SettingsManager.Save(settings);
        NewProfileButton();
      }
      else
      {
        ServiceRegistration.Get<INotificationService>().EnqueueNotification(NotificationType.Error, "[AtmoLight.Name]", "[AtmoLight.ButtonError]", true);
      }
    }
  }

  public class ManualMode : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().ManualMode;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ManualMode = _yes;
      SettingsManager.Save(settings);
    }
  }

  public class SBS3D : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().SBS3D;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.SBS3D = _yes;
      SettingsManager.Save(settings);
    }
  }

  public class StopAtmoWinOnExit : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().StopAtmoWinOnExit;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.StopAtmoWinOnExit = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().atmoWinAutoStop = _yes;
    }
  }

  public class StartAtmoWinOnStart : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().StartAtmoWinOnStart;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.StartAtmoWinOnStart = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().atmoWinAutoStart = _yes;
    }
  }

  public class RestartAtmoWinOnError : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().RestartAtmoWinOnError;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.RestartAtmoWinOnError = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().SetReInitOnError(_yes);
    }
  }

  public class LowCPU : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().LowCPU;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.LowCPU = _yes;
      SettingsManager.Save(settings);
    }
  }

  public class LowCPUTime : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 1000;
      _value = SettingsManager.Load<Settings>().LowCPUTime;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.LowCPUTime = (int)_value;
      SettingsManager.Save(settings);
    }
  }

  public class Delay : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().Delay;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();

      if (settings.Delay != _yes && Core.GetInstance().GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        if (_yes)
        {
          Core.GetInstance().EnableDelay();
        }
        else
        {
          Core.GetInstance().DisableDelay();
        }
      }

      settings.Delay = _yes;
      SettingsManager.Save(settings);
    }
  }

  public class DelayTime : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 1000;
      _value = SettingsManager.Load<Settings>().DelayTime;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.DelayTime = (int)_value;
      SettingsManager.Save(settings);

      if (Core.GetInstance().IsDelayEnabled() && Core.GetInstance().GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        Core.GetInstance().SetDelay((int)_value);
      }
    }
  }

  public class DelayRefreshRate : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 120;
      _value = SettingsManager.Load<Settings>().DelayRefreshRate;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.DelayRefreshRate = (int)_value;
      SettingsManager.Save(settings);
    }
  }

  public class StaticColorRed : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 255;
      _value = SettingsManager.Load<Settings>().StaticColorRed;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.StaticColorRed = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().SetStaticColor((int)_value, settings.StaticColorGreen, settings.StaticColorBlue);
      if (Core.GetInstance().GetCurrentEffect() == ContentEffect.StaticColor)
      {
        Core.GetInstance().ChangeEffect(ContentEffect.StaticColor, true);
      }
    }
  }

  public class StaticColorGreen : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 255;
      _value = SettingsManager.Load<Settings>().StaticColorGreen;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.StaticColorGreen = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().SetStaticColor(settings.StaticColorRed, (int)_value, settings.StaticColorBlue);
      if (Core.GetInstance().GetCurrentEffect() == ContentEffect.StaticColor)
      {
        Core.GetInstance().ChangeEffect(ContentEffect.StaticColor, true);
      }
    }
  }

  public class StaticColorBlue : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 255;
      _value = SettingsManager.Load<Settings>().StaticColorBlue;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.StaticColorBlue = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().SetStaticColor(settings.StaticColorRed, settings.StaticColorGreen, (int)_value);
      if (Core.GetInstance().GetCurrentEffect() == ContentEffect.StaticColor)
      {
        Core.GetInstance().ChangeEffect(ContentEffect.StaticColor, true);
      }
    }
  }

  public class ExcludeTimeStartHour : LimitedNumberSelect
  {
    int pos;
    string excludeTimeStart;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 23;
      excludeTimeStart = SettingsManager.Load<Settings>().ExcludeTimeStart;
      pos = excludeTimeStart.IndexOf(":");
      double.TryParse(excludeTimeStart.Substring(0, pos), out _value);
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeStart = ((_value < 10) ? "0" : "") + _value + ":" + excludeTimeStart.Substring(pos + 1, 2);
      SettingsManager.Save(settings);
    }
  }

  public class ExcludeTimeEndHour : LimitedNumberSelect
  {
    int pos;
    string excludeTimeEnd;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 23;
      excludeTimeEnd = SettingsManager.Load<Settings>().ExcludeTimeEnd;
      pos = excludeTimeEnd.IndexOf(":");
      double.TryParse(excludeTimeEnd.Substring(0, pos), out _value);
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeEnd = ((_value < 10) ? "0" : "") + _value + ":" + excludeTimeEnd.Substring(pos + 1, 2);
      SettingsManager.Save(settings);
    }
  }

  public class ExcludeTimeStartMinutes : LimitedNumberSelect
  {
    int pos;
    string excludeTimeStart;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 59;
      excludeTimeStart = SettingsManager.Load<Settings>().ExcludeTimeStart;
      pos = excludeTimeStart.IndexOf(":");
      double.TryParse(excludeTimeStart.Substring(pos + 1, 2), out _value);
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeStart = excludeTimeStart.Substring(0, pos) + ":" + ((_value < 10) ? "0" : "") + _value;
      SettingsManager.Save(settings);
    }
  }

  public class ExcludeTimeEndMinutes : LimitedNumberSelect
  {
    int pos;
    string excludeTimeEnd;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 59;
      excludeTimeEnd = SettingsManager.Load<Settings>().ExcludeTimeEnd;
      pos = excludeTimeEnd.IndexOf(":");
      double.TryParse(excludeTimeEnd.Substring(pos + 1, 2), out _value);
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeEnd = excludeTimeEnd.Substring(0, pos) + ":" + ((_value < 10) ? "0" : "") + _value;
      SettingsManager.Save(settings);
    }
  }

  public class LEDsEnabled : YesNo
  {
    public override void Load()
    {
      _yes = Core.GetInstance().IsAtmoLightOn();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      if (_yes)
      {
        ContentEffect temp;
        if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
        {
          temp = settings.VideoEffect;
        }
        else if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
        {
          temp = settings.AudioEffect;
        }
        else
        {
          temp = settings.MenuEffect;
        }
        Core.GetInstance().ChangeEffect(temp);
      }
      else
      {
        Core.GetInstance().ChangeEffect(ContentEffect.LEDsDisabled);
      }
    }
  }

  public class AtmoWinTarget : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().AtmoWinTarget;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AtmoWinTarget = _yes;
      SettingsManager.Save(settings);

      if (_yes)
      {
        Core.GetInstance().AddTarget(Target.AtmoWin);
        Core.GetInstance().Initialise();
      }
      else
      {
        Core.GetInstance().RemoveTarget(Target.AtmoWin);
      }
    }
  }

  public class HyperionTarget : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().HyperionTarget;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HyperionTarget = _yes;
      SettingsManager.Save(settings);

      if (_yes)
      {
        Core.GetInstance().AddTarget(Target.Hyperion);
        Core.GetInstance().Initialise();
      }
      else
      {
        Core.GetInstance().RemoveTarget(Target.Hyperion);
      }
    }
  }

  public class HyperionIP : Entry
  {
    public override void Load()
    {
      _value = SettingsManager.Load<Settings>().HyperionIP;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      IPAddress ip;
      if (IPAddress.TryParse(_value, out ip))
      {
        settings.HyperionIP = _value;
        SettingsManager.Save(settings);

        Core.GetInstance().hyperionIP = _value;
      }
    }

    public override int DisplayLength
    {
      get { return 15; }
    }
  }

  public class HyperionPort : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 65535;
      _value = SettingsManager.Load<Settings>().HyperionPort;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HyperionPort = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hyperionPort = (int)_value;
    }
  }

  public class HyperionPriority : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 9999;
      _value = SettingsManager.Load<Settings>().HyperionPriority;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HyperionPriority = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hyperionPriority = (int)_value;
    }
  }

  public class HyperionPriorityStaticColor : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 9999;
      _value = SettingsManager.Load<Settings>().HyperionPriorityStaticColor;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HyperionPriorityStaticColor = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hyperionPriorityStaticColor = (int)_value;
    }
  }

  public class HyperionReconnectAttempts : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 9999;
      _value = SettingsManager.Load<Settings>().HyperionReconnectAttempts;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HyperionReconnectAttempts = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hyperionReconnectAttempts = (int)_value;
    }
  }

  public class HyperionReconnectDelay : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 99999;
      _value = SettingsManager.Load<Settings>().HyperionReconnectDelay;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HyperionReconnectDelay = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hyperionReconnectDelay = (int)_value;
    }
  }

  public class HyperionLiveReconnect : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().HyperionLiveReconnect;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HyperionLiveReconnect = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().hyperionLiveReconnect = _yes;
    }
  }
  public class HueTarget : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().HueTarget;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HueTarget = _yes;
      SettingsManager.Save(settings);

      if (_yes)
      {
        Core.GetInstance().AddTarget(Target.Hue);
        Core.GetInstance().Initialise();
      }
      else
      {
        Core.GetInstance().RemoveTarget(Target.Hue);
      }
    }
  }
  public class HueIP : Entry
  {
    public override void Load()
    {
      _value = SettingsManager.Load<Settings>().HueIP;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      IPAddress ip;
      if (IPAddress.TryParse(_value, out ip))
      {
        settings.HueIP = _value;
        SettingsManager.Save(settings);

        Core.GetInstance().hueIP = _value;
      }
    }

    public override int DisplayLength
    {
      get { return 15; }
    }
  }

  public class HuePort : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 65535;
      _value = SettingsManager.Load<Settings>().HuePort;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HuePort = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().huePort = (int)_value;
    }
  }
  public class HueMinimalColorDifference : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 255;
      _value = SettingsManager.Load<Settings>().HueMinimalColorDifference;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HueMinimalColorDifference = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hueMinimalColorDifference = (int)_value;
    }
  }

  public class CaptureWidth : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 1920;
      _value = SettingsManager.Load<Settings>().CaptureWidth;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.CaptureWidth = (int)_value;
      SettingsManager.Save(settings);

      // AtmoWin has hardcoded dimensions, we cant use any other if AtmoWin is a target.
      if (!settings.AtmoWinTarget)
      {
        Core.GetInstance().SetCaptureDimensions(settings.CaptureWidth, settings.CaptureHeight);
      }
    }
  }

  public class CaptureHeight : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 1080;
      _value = SettingsManager.Load<Settings>().CaptureHeight;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.CaptureHeight = (int)_value;
      SettingsManager.Save(settings);

      // AtmoWin has hardcoded dimensions, we cant use any other if AtmoWin is a target.
      if (!settings.AtmoWinTarget)
      {
        Core.GetInstance().SetCaptureDimensions(settings.CaptureWidth, settings.CaptureHeight);
      }
    }
  }
}