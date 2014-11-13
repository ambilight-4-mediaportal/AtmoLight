using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace AtmoLight
{
  public interface ITargets
  {
    Target Name { get; }
    List<ContentEffect> SupportedEffects { get; }
    void Initialise(bool force);
    void ReInitialise(bool force);
    void Dispose();
    bool IsConnected();
    bool ChangeEffect(ContentEffect effect);
    void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader);
    void ChangeProfile();
    void PowerModeChanged(PowerModes powerMode);
  }
}
