using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtmoLight.Targets
{
  interface ILamp
  {
    string ID { get; }
    LampType Type { get; }
    int[] OverallAverageColor { get; set; }
    int[] AverageColor { get; set; }
    int[] PreviousColor { get; set; } 
    int PixelCount { get; set; }
    int HScanStart { get; }
    int HScanEnd { get; }
    int VScanStart { get; }
    int VScanEnd { get; }
    bool ZoneInverted { get; }
    string IP { get; }
    int Port { get; }
    void Connect(string ip, int port);
    void Disconnect();
    bool IsConnected();
    void ChangeColor(string color, bool forceLightsOff);
  }
}
