using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Common.Configuration.ConfigurationClasses;
using MediaPortal.Common.Localization;

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
      settings.AtmoWinExe = _path;
      SettingsManager.Save(settings);
    }
  }

  public class VideoEffect : SingleSelectionList
  {
    public override void Load()
    {
      IList<string> effectList = new List<string>();
      for (int x = 0; x < 6; x++)
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
    }
  }

  public class AudioEffect : SingleSelectionList
  {
    public override void Load()
    {
      IList<string> effectList = new List<string>();
      for (int x = 0; x < 6; x++)
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
    }
  }

  public class MenuEffect : SingleSelectionList
  {
    public override void Load()
    {
      IList<string> effectList = new List<string>();
      for (int x = 0; x < 6; x++)
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

  public class OnMediaPortalExit : SingleSelectionList
  {
    private IList<string> list = new List<string>();
    public override void Load()
    {
      if (list.Count == 0)
      {
        list.Add("[AtmoLight.DisableLEDsOnExit]");
        list.Add("[AtmoLight.EnableLiveviewOnExit]");
      }

      if (SettingsManager.Load<Settings>().DisableLEDsOnExit == true)
      {
        Selected = 0;
      }
      else
      {
        Selected = 1;
      }

      _items = list.Select(LocalizationHelper.CreateResourceString).ToList();
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      if (Selected == 0)
      {
        settings.DisableLEDsOnExit = true;
        settings.EnableLiveviewOnExit = false;
      }
      else
      {
        settings.DisableLEDsOnExit = false;
        settings.EnableLiveviewOnExit = true;
      }
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
      _lowerLimit = 0;
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
      _lowerLimit = 0;
      _upperLimit = 1000;
      _value = SettingsManager.Load<Settings>().DelayTime;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.DelayTime = (int)_value;
      SettingsManager.Save(settings);
    }
  }

  public class DelayRefreshRate : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
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
    }
  }

  public class ExcludeTimeStartHour : LimitedNumberSelect
  {
    int value;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 23;
      string excludeTimeStart = SettingsManager.Load<Settings>().ExcludeTimeStart;
      int pos = excludeTimeStart.IndexOf(":");
      int.TryParse(excludeTimeStart.Substring(0, pos), out value);
      _value = value;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeStart = settings.ExcludeTimeStart.Replace(value + ":", _value + ":");
      SettingsManager.Save(settings);
    }
  }

  public class ExcludeTimeStopHour : LimitedNumberSelect
  {
    int value;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 23;
      string excludeTimeStart = SettingsManager.Load<Settings>().ExcludeTimeStart;
      int pos = excludeTimeStart.IndexOf(":");
      int.TryParse(excludeTimeStart.Substring(0, pos), out value);
      _value = value;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeStart = settings.ExcludeTimeStart.Replace(value + ":", _value + ":");
      SettingsManager.Save(settings);
    }
  }

  public class ExcludeTimeStartMinutes : LimitedNumberSelect
  {
    int value;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 23;
      string excludeTimeStart = SettingsManager.Load<Settings>().ExcludeTimeStart;
      int pos = excludeTimeStart.IndexOf(":");
      int.TryParse(excludeTimeStart.Substring(pos + 1, 2), out value);
      _value = value;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeStart = settings.ExcludeTimeStart.Replace(":" + value, ":" + _value);
      SettingsManager.Save(settings);
    }
  }

  public class ExcludeTimeStopMinutes : LimitedNumberSelect
  {
    int value;
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 0;
      _upperLimit = 23;
      string excludeTimeStart = SettingsManager.Load<Settings>().ExcludeTimeStart;
      int pos = excludeTimeStart.IndexOf(":");
      int.TryParse(excludeTimeStart.Substring(pos + 1, 2), out value);
      _value = value;
    }

    public override void Save()
    {
      base.Save();
      Settings settings = SettingsManager.Load<Settings>();
      settings.ExcludeTimeStart = settings.ExcludeTimeStart.Replace(":" + value, ":" + _value);
      SettingsManager.Save(settings);
    }
  }
}