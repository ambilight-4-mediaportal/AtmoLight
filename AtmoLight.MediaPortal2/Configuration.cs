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
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;

    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined)
        {
          if (effect != ContentEffect.VUMeter && effect != ContentEffect.VUMeterRainbow)
          {
            effectList.Add("[AtmoLight." + effect.ToString() + "]");
          }
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
      SettingsChanged();

      if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
      {
        Core.GetInstance().ChangeEffect(settings.VideoEffect);
      }
    }
  }

  public class AudioEffect : SingleSelectionList
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;

    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined)
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
      SettingsChanged();

      if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
      {
        Core.GetInstance().ChangeEffect(settings.AudioEffect);
      }
    }
  }

  public class MenuEffect : SingleSelectionList
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;

    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined)
        {
          if (effect != ContentEffect.VUMeter && effect != ContentEffect.VUMeterRainbow)
          {
            effectList.Add("[AtmoLight." + effect.ToString() + "]");
          }
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
      SettingsChanged();

      if (!ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive && !ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
      {
        Core.GetInstance().ChangeEffect(settings.MenuEffect);
      }
    }
  }

  public class MPExitEffect : SingleSelectionList
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;

    private IList<string> effectList = new List<string>();
    public override void Load()
    {
      List<ContentEffect> supportedEffects = Core.GetInstance().GetSupportedEffects();

      effectList.Clear();
      foreach (ContentEffect effect in Enum.GetValues(typeof(ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined)
        {
          if (effect != ContentEffect.MediaPortalLiveMode && effect != ContentEffect.GIFReader && effect != ContentEffect.VUMeter && effect != ContentEffect.VUMeterRainbow)
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
      SettingsChanged();
    }
  }

  public class OnOffButton : SingleSelectionList
  {
    public delegate void ButtonsChangedHandler();
    public static event ButtonsChangedHandler ButtonsChanged;

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
        ButtonsChanged();
      }
      else
      {
        ServiceRegistration.Get<INotificationService>().EnqueueNotification(NotificationType.Error, "[AtmoLight.Name]", "[AtmoLight.ButtonError]", true);
      }
    }
  }

  public class ProfileButton : SingleSelectionList
  {
    public delegate void ButtonsChangedHandler();
    public static event ButtonsChangedHandler ButtonsChanged;

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
        ButtonsChanged();
      }
      else
      {
        ServiceRegistration.Get<INotificationService>().EnqueueNotification(NotificationType.Error, "[AtmoLight.Name]", "[AtmoLight.ButtonError]", true);
      }
    }
  }

  public class ManualMode : YesNo
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
    }
  }

  public class SBS3D : YesNo
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
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
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
    }
  }

  public class LowCPUTime : LimitedNumberSelect
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
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
      settings.Delay = _yes;
      SettingsManager.Save(settings);

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
    }
  }

  public class DelayTime : LimitedNumberSelect
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();

      if (Core.GetInstance().IsDelayEnabled() && Core.GetInstance().GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        Core.GetInstance().SetDelay((int)_value);
      }
    }
  }

  public class DelayRefreshRate : LimitedNumberSelect
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
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
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
    }
  }

  public class ExcludeTimeEndHour : LimitedNumberSelect
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
    }
  }

  public class ExcludeTimeStartMinutes : LimitedNumberSelect
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
    }
  }

  public class ExcludeTimeEndMinutes : LimitedNumberSelect
  {
    public delegate void SettingsChangedHandler();
    public static event SettingsChangedHandler SettingsChanged;
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
      SettingsChanged();
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

  public class HueExe : PathEntry
  {
    public override void Load()
    {
      _pathSelectionType = PathSelectionType.File;
      _path = SettingsManager.Load<Settings>().hueExe;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.hueExe = _path + "AtmoHue.exe";
      SettingsManager.Save(settings);

      Core.GetInstance().huePath = settings.hueExe;
      if (settings.HueTarget)
      {
        Core.GetInstance().RemoveTarget(Target.Hue);
        Core.GetInstance().AddTarget(Target.Hue);
        Core.GetInstance().Initialise();
      }
    }
  }

  public class HueStart : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().hueStart;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.hueStart = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().hueStart = _yes;
    }
  }

  public class HueIsRemoteMachine : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().hueIsRemoteMachine;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.hueIsRemoteMachine = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().hueIsRemoteMachine = _yes;
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

  public class HueReconnectAttempts : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 9999;
      _value = SettingsManager.Load<Settings>().HueReconnectAttempts;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HueReconnectAttempts = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hueReconnectAttempts = (int)_value;
    }
  }

  public class HueReconnectDelay : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 99999;
      _value = SettingsManager.Load<Settings>().HueReconnectDelay;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HueReconnectDelay = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().hueReconnectDelay = (int)_value;
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

  public class HueBridgeEnableOnResume : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().HueBridgeEnableOnResume;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HueBridgeEnableOnResume = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().hueBridgeEnableOnResume = _yes;
    }
  }

  public class HueBridgeDisableOnSuspend : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().HueBridgeDisableOnSuspend;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.HueBridgeDisableOnSuspend = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().hueBridgeDisableOnSuspend = _yes;
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

  public class BoblightTarget : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().BoblightTarget;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightTarget = _yes;
      SettingsManager.Save(settings);

      if (_yes)
      {
        Core.GetInstance().AddTarget(Target.Boblight);
        Core.GetInstance().Initialise();
      }
      else
      {
        Core.GetInstance().RemoveTarget(Target.Boblight);
      }
    }
  }

  public class BoblightIP : Entry
  {
    public override void Load()
    {
      _value = SettingsManager.Load<Settings>().BoblightIP;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      IPAddress ip;
      if (IPAddress.TryParse(_value, out ip))
      {
        settings.BoblightIP = _value;
        SettingsManager.Save(settings);

        Core.GetInstance().boblightIP = _value;
      }
    }

    public override int DisplayLength
    {
      get { return 15; }
    }
  }

  public class BoblightPort : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 65535;
      _value = SettingsManager.Load<Settings>().BoblightPort;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightPort = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightPort = (int)_value;
    }
  }

  public class BoblightMaxFPS : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 144;
      _value = SettingsManager.Load<Settings>().BoblightMaxFPS;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightMaxFPS = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightMaxFPS = (int)_value;
    }
  }

  public class BoblightMaxReconnectAttempts : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 9999;
      _value = SettingsManager.Load<Settings>().BoblightMaxReconnectAttempts;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightMaxReconnectAttempts = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightMaxReconnectAttempts = (int)_value;
    }
  }

  public class BoblightReconnectDelay : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 999999;
      _value = SettingsManager.Load<Settings>().BoblightReconnectDelay;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightReconnectDelay = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightReconnectDelay = (int)_value;
    }
  }

  public class BoblightSpeed : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 100;
      _value = SettingsManager.Load<Settings>().BoblightSpeed;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightSpeed = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightSpeed = (int)_value;
    }
  }

  public class BoblightAutospeed : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 100;
      _value = SettingsManager.Load<Settings>().BoblightAutospeed;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightAutospeed = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightAutospeed = (int)_value;
    }
  }

  public class BoblightSaturation : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 20;
      _value = SettingsManager.Load<Settings>().BoblightSaturation;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightSaturation = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightSaturation = (int)_value;
    }
  }

  public class BoblightValue : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 20;
      _value = SettingsManager.Load<Settings>().BoblightValue;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightValue = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightValue = (int)_value;
    }
  }

  public class BoblightThreshold : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 255;
      _value = SettingsManager.Load<Settings>().BoblightThreshold;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightThreshold = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightThreshold = (int)_value;
    }
  }

  public class BoblightInterpolation : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().BoblightInterpolation;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightInterpolation = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightInterpolation = _yes;
    }
  }

  public class BoblightGamma : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 0.1;
      _lowerLimit = 0;
      _upperLimit = 10;
      _value = SettingsManager.Load<Settings>().BoblightGamma;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BoblightGamma = _value;
      SettingsManager.Save(settings);

      Core.GetInstance().boblightGamma = (int)_value;
    }
  }

  public class BlackbarDetection : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().BlackbarDetection;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BlackbarDetection = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().blackbarDetection = _yes;
    }
  }

  public class BlackbarDetectionTime : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 99999;
      _value = SettingsManager.Load<Settings>().BlackbarDetectionTime;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BlackbarDetectionTime = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().blackbarDetectionTime = (int)_value;
    }
  }

  public class BlackbarDetectionThreshold : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 255;
      _value = SettingsManager.Load<Settings>().BlackbarDetectionThreshold;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.BlackbarDetectionThreshold = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().blackbarDetectionThreshold = (int)_value;
    }
  }

  public class PowerModeChangedDelay : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 99999;
      _value = SettingsManager.Load<Settings>().PowerModeChangedDelay;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.PowerModeChangedDelay = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().powerModeChangedDelay = (int)_value;
    }
  }

  public class AmbiBoxTarget : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().AmbiBoxTarget;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxTarget = _yes;
      SettingsManager.Save(settings);

      if (_yes)
      {
        Core.GetInstance().AddTarget(Target.AmbiBox);
        Core.GetInstance().Initialise();
      }
      else
      {
        Core.GetInstance().RemoveTarget(Target.AmbiBox);
      }
    }
  }

  public class AmbiBoxIP : Entry
  {
    public override void Load()
    {
      _value = SettingsManager.Load<Settings>().AmbiBoxIP;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      IPAddress ip;
      if (IPAddress.TryParse(_value, out ip))
      {
        settings.AmbiBoxIP = _value;
        SettingsManager.Save(settings);

        Core.GetInstance().ambiBoxIP = _value;
      }
    }

    public override int DisplayLength
    {
      get { return 15; }
    }
  }

  public class AmbiBoxPort : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 65535;
      _value = SettingsManager.Load<Settings>().AmbiBoxPort;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxPort = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxPort = (int)_value;
    }
  }

  public class AmbiBoxMaxReconnectAttempts : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 9999;
      _value = SettingsManager.Load<Settings>().AmbiBoxMaxReconnectAttempts;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxMaxReconnectAttempts = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxMaxReconnectAttempts = (int)_value;
    }
  }

  public class AmbiBoxReconnectDelay : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 99999;
      _value = SettingsManager.Load<Settings>().AmbiBoxReconnectDelay;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxReconnectDelay = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxReconnectDelay = (int)_value;
    }
  }

  public class AmbiBoxLEDCount : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = 300;
      _value = SettingsManager.Load<Settings>().AmbiBoxLEDCount;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxLEDCount = (int)_value;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxLEDCount = (int)_value;
    }
  }

  public class AmbiBoxMediaPortalProfile : Entry
  {
    public override void Load()
    {
      _value = SettingsManager.Load<Settings>().AmbiBoxMediaPortalProfile;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxMediaPortalProfile = _value;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxMediaPortalProfile = _value;
    }

    public override int DisplayLength
    {
      get { return 15; }
    }
  }

  public class AmbiBoxExternalProfile : Entry
  {
    public override void Load()
    {
      _value = SettingsManager.Load<Settings>().AmbiBoxExternalProfile;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxExternalProfile = _value;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxExternalProfile = _value;
    }

    public override int DisplayLength
    {
      get { return 15; }
    }
  }

  public class AmbiBoxPath : PathEntry
  {
    public override void Load()
    {
      _path = SettingsManager.Load<Settings>().AmbiBoxPath;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxPath = _path + "AmbiBox.exe";
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxPath = settings.AmbiBoxPath;
      if (settings.AmbiBoxTarget)
      {
        Core.GetInstance().RemoveTarget(Target.AmbiBox);
        Core.GetInstance().AddTarget(Target.AmbiBox);
        Core.GetInstance().Initialise();
      }
    }
  }

  public class AmbiBoxAutoStart : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().AmbiBoxAutoStart;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxAutoStart = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxAutoStart = _yes;
    }
  }

  public class AmbiBoxAutoStop : YesNo
  {
    public override void Load()
    {
      _yes = SettingsManager.Load<Settings>().AmbiBoxAutoStop;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AmbiBoxAutoStop = _yes;
      SettingsManager.Save(settings);

      Core.GetInstance().ambiBoxAutoStop = _yes;
    }
  }
}