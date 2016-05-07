using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace AtmoLight
{
  public static class Localization
  {
    private static XmlDocument xmlFile;
    private static XmlDocument xmlFileFallback;


    public static string Translate(string node, string name, bool fallback = false)
    {
      try
      {

        if (xmlFile == null || xmlFileFallback == null)
        {
          Init();
        }

        XmlNode xmlNode = fallback
          ? xmlFileFallback.DocumentElement.SelectSingleNode("/resources/" + node)
          : xmlFile.DocumentElement.SelectSingleNode("/resources/" + node);
        if (xmlNode == null)
        {
          Log.Warn("Could not find node {0} in {1}", node, fallback ? xmlFileFallback.BaseURI : xmlFile.BaseURI);

          // Try using english translation if node was not found
          if (xmlFile.BaseURI != xmlFileFallback.BaseURI && !fallback)
          {
            return Translate(node, name, true);
          }
          return "";
        }

        foreach (XmlNode childNodes in xmlNode.ChildNodes)
        {
          if (childNodes.Attributes[0].Value == name)
          {
            if (!string.IsNullOrEmpty(childNodes.InnerText))
            {
              return childNodes.InnerText;
            }
          }
        }

        Log.Warn("Could not find translation for {0} in {1}", name, fallback ? xmlFileFallback.BaseURI : xmlFile.BaseURI);

        // Try using english translation if this translation was not found
        if (xmlFile.BaseURI != xmlFileFallback.BaseURI && !fallback)
        {
          return Translate(node, name, true);
        }
        return "";
      }
      catch (Exception e)
      {
        Log.Error("Error during Localization - Translate");
        Log.Error(e.Message);
        return "";
      }
    }

    public static string ReverseTranslate(string node, string translation, bool fallback = false)
    {
      try
      {

        if (xmlFile == null || xmlFileFallback == null)
        {
          Init();
        }

        XmlNode xmlNode = fallback
          ? xmlFileFallback.DocumentElement.SelectSingleNode("/resources/" + node)
          : xmlFile.DocumentElement.SelectSingleNode("/resources/" + node);
        if (xmlNode == null)
        {
          Log.Warn("Could not find node {0} in {1}", node, fallback ? xmlFileFallback.BaseURI : xmlFile.BaseURI);

          // Try using english reverse translation if node was not found
          if (xmlFile.BaseURI != xmlFileFallback.BaseURI && !fallback)
          {
            return ReverseTranslate(node, translation, true);
          }
          return "";
        }

        foreach (XmlNode childNodes in xmlNode.ChildNodes)
        {
          if (childNodes.InnerText == translation)
          {
            if (!string.IsNullOrEmpty(childNodes.Attributes[0].Value))
            {
              return childNodes.Attributes[0].Value;
            }
          }
        }

        Log.Warn("Could not reverse translate {0} in {1}", translation,
          fallback ? xmlFileFallback.BaseURI : xmlFile.BaseURI);

        // Try using english reverse translation if this translation was not found
        if (xmlFile.BaseURI != xmlFileFallback.BaseURI && !fallback)
        {
          return ReverseTranslate(node, translation, true);
        }
        return "";
      }
      catch (Exception e)
      {
        Log.Error("Error during Localization - ReverseTranslate");
        Log.Error(e.Message);
        return "";
      }
    }

    public static void Load(string languageFileLocation)
    {
      try
      {
        if (xmlFile == null || xmlFileFallback == null)
        {
          Init();
        }

        xmlFile.Load(languageFileLocation);
      }
      catch (Exception e)
      {
        Log.Error("Error during loading of XML language file.");
        Log.Error(e.Message);
      }

    }

    private static void Init()
    {
      try
      {

        if (string.IsNullOrEmpty(Settings.currentLanguageFileLocation))
        {
          Settings.LoadSettings();
        }

        string mediaportalLanguageDir =
          MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language);

        if (string.IsNullOrEmpty(mediaportalLanguageDir) || !Directory.Exists(mediaportalLanguageDir))
        {
          mediaportalLanguageDir = @"C:\ProgramData\Team MediaPortal\MediaPortal\Language";
        }

        string fallbackFilename = mediaportalLanguageDir + "\\AtmoLight\\en.xml";

        xmlFile = new XmlDocument();
        xmlFileFallback = new XmlDocument();

        if (File.Exists(Settings.currentLanguageFileLocation))
        {
          xmlFile.Load(Settings.currentLanguageFileLocation);
        }
        else
        {
          Log.Error(String.Format("Language file {0} doesn't exist: ", Settings.currentLanguageFileLocation));
          Log.Error("Mediaportal configuration language location: " + MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language));
        }

        try
        {
          if (File.Exists(fallbackFilename))
          {
            xmlFileFallback.Load(fallbackFilename);
          }
          else
          {
            Log.Error(String.Format("Fallback language file {0} doesn't exist: ", fallbackFilename));
            Log.Error("Mediaportal configuration language location: " +
                      MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Language));
          }
        }
        catch (Exception)
        {
        }
      }
      catch (Exception e)
      {
        Log.Error("Error during Localization - Init");
        Log.Error(e.Message);
      }
    }
  }
}
