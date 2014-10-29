using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtmoLight
{
  public interface ITargets
  {
    Target Name { get; }
    void Initialise(bool force);
    void ReInitialise(bool force);
    void Dispose();
    bool IsConnected();
    bool ChangeEffect(ContentEffect effect);
    void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader);
    void ChangeProfile();
  }
}
