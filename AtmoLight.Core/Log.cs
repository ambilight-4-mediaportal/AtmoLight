using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtmoLight
{
  public static class Log
  {
    public enum LogLevel
    {
      Debug,
      Warn,
      Info,
      Error
    }

    public delegate void NewLogHandler(LogLevel logLevel, string format, params object[] args);
    public static event NewLogHandler OnNewLog;

    public static void Debug(string format, params object[] args)
    {
      if (OnNewLog != null)
      {
        OnNewLog(LogLevel.Debug, "AtmoLight: " + format, args);
      }
    }

    public static void Warn(string format, params object[] args)
    {
      if (OnNewLog != null)
      {
        OnNewLog(LogLevel.Warn, "AtmoLight: " + format, args);
      }
    }

    public static void Info(string format, params object[] args)
    {
      if (OnNewLog != null)
      {
        OnNewLog(LogLevel.Info, "AtmoLight: " + format, args);
      }
    }

    public static void Error(string format, params object[] args)
    {
      if (OnNewLog != null)
      {
        OnNewLog(LogLevel.Error, "AtmoLight: " + format, args);
      }
    }
  }
}
