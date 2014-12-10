using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtmoLight.Targets
{
  interface ILamp
  {
    string ID { get; }
    string Type { get; }
    void Connect(string ip, int port);
    void Disconnect();
    bool IsConnected();
    void ChangeColor(string color);
  }
}
