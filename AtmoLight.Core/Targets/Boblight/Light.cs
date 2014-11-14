using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtmoLight
{
  class Light
  {
    public string name;
    public int[] rgb = new int[3];
    public int[] rgbPrev = new int[3];
    public int vScanStart;
    public int vScanEnd;
    public int hScanStart;
    public int hScanEnd;
    public double singleChange;
    public int rgbCount;

    public Light(string name, int vScanStart, int vScanEnd, int hScanStart, int hScanEnd)
    {
      this.name = name;
      this.vScanStart = vScanStart;
      this.vScanEnd = vScanEnd;
      this.hScanStart = hScanStart;
      this.hScanEnd = hScanEnd;
    }
  }
}