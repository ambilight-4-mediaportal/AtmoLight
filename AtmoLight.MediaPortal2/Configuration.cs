using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MediaPortal.Common.Configuration.ConfigurationClasses;
using MediaPortal.Common.Localization;

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
      AtmoLight.Plugin.AtmoLightObject.SetGIFPath(settings.GIFFile);
    }
  }

  public class VideoEffect : SingleSelectionList
  {
    public override void Load()
    {
      IList<string> effectList = new List<string>();
      for (int x = 0; x < 7; x++)
      {
        effectList.Add("[AtmoLight." + ((ContentEffect)x).ToString() + "]");
      }
      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().VideoEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.VideoEffect = (ContentEffect)Selected;
      SettingsManager.Save(settings);

      if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
      {
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect((ContentEffect)Selected);
      }
    }
  }

  public class AudioEffect : SingleSelectionList
  {
    public override void Load()
    {
      IList<string> effectList = new List<string>();
      for (int x = 0; x < 7; x++)
      {
        effectList.Add("[AtmoLight." + ((ContentEffect)x).ToString() + "]");
      }

      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().AudioEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.AudioEffect = (ContentEffect)Selected;
      SettingsManager.Save(settings);

      if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
      {
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect((ContentEffect)Selected);
      }
    }
  }

  public class MenuEffect : SingleSelectionList
  {
    public override void Load()
    {
      IList<string> effectList = new List<string>();
      for (int x = 0; x < 7; x++)
      {
        effectList.Add("[AtmoLight." + ((ContentEffect)x).ToString() + "]");
      }

      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().MenuEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.MenuEffect = (ContentEffect)Selected;
      SettingsManager.Save(settings);

      if (!ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive && !ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
      {
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect((ContentEffect)Selected);
      }
    }
  }

  public class MPExitEffect : SingleSelectionList
  {
    public override void Load()
    {
      IList<string> effectList = new List<string>();
      for (int x = 0; x < 7; x++)
      {
        effectList.Add("[AtmoLight." + ((ContentEffect)x).ToString() + "]");
      }

      Selected = effectList.IndexOf("[AtmoLight." + SettingsManager.Load<Settings>().MPExitEffect.ToString() + "]");

      _items = effectList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.MPExitEffect = (ContentEffect)Selected;
      SettingsManager.Save(settings);
    }
  }

  public class MenuButton : SingleSelectionList
  {
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

      Selected = buttonList.IndexOf(SettingsManager.Load<Settings>().MenuButton.ToString());

      _items = buttonList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.MenuButton = buttonList[Selected];
      SettingsManager.Save(settings);
    }
  }

  public class OnOffButton : SingleSelectionList
  {
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

      Selected = buttonList.IndexOf(SettingsManager.Load<Settings>().OnOffButton.ToString());

      _items = buttonList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.OnOffButton = buttonList[Selected];
      SettingsManager.Save(settings);
    }
  }

  public class ProfileButton : SingleSelectionList
  {
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

      Selected = buttonList.IndexOf(SettingsManager.Load<Settings>().ProfileButton.ToString());

      _items = buttonList.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ProfileButton = buttonList[Selected];
      SettingsManager.Save(settings);
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

      AtmoLight.Plugin.AtmoLightObject.SetReInitOnError(_yes);
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

      if (settings.Delay != _yes && AtmoLight.Plugin.AtmoLightObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        if (_yes)
        {
          AtmoLight.Plugin.AtmoLightObject.EnableDelay();
        }
        else
        {
          AtmoLight.Plugin.AtmoLightObject.DisableDelay();
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

      if (AtmoLight.Plugin.AtmoLightObject.IsDelayEnabled() && AtmoLight.Plugin.AtmoLightObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        AtmoLight.Plugin.AtmoLightObject.SetDelay((int)_value);
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

      if (AtmoLight.Plugin.AtmoLightObject.GetCurrentEffect() == ContentEffect.StaticColor)
      {
        AtmoLight.Plugin.AtmoLightObject.SetStaticColor((int)_value, settings.StaticColorGreen, settings.StaticColorBlue);
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect(ContentEffect.StaticColor, true);
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

      if (AtmoLight.Plugin.AtmoLightObject.GetCurrentEffect() == ContentEffect.StaticColor)
      {
        AtmoLight.Plugin.AtmoLightObject.SetStaticColor(settings.StaticColorRed, (int)_value, settings.StaticColorBlue);
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect(ContentEffect.StaticColor, true);
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

      if (AtmoLight.Plugin.AtmoLightObject.GetCurrentEffect() == ContentEffect.StaticColor)
      {
        AtmoLight.Plugin.AtmoLightObject.SetStaticColor(settings.StaticColorRed, settings.StaticColorGreen, (int)_value);
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect(ContentEffect.StaticColor, true);
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
      _yes = AtmoLight.Plugin.AtmoLightObject.IsAtmoLightOn();
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
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect(temp);
      }
      else
      {
        AtmoLight.Plugin.AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
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
    }
  }
}