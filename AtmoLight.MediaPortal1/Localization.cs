using System;
using System.Collections.Generic;
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
      if (xmlFile == null || xmlFileFallback == null)
      {
        Init();
      }

      XmlNode xmlNode = fallback ? xmlFileFallback.DocumentElement.SelectSingleNode("/resources/" + node) : xmlFile.DocumentElement.SelectSingleNode("/resources/" + node);
      if (xmlNode == null)
      {
        Log.Warn("Could not find node {0} in {1}", node, xmlFile.BaseURI);

        // Try using english translation if node was not found
        if (xmlFile.BaseURI != xmlFileFallback.BaseURI)
        {
          return Translate(node, name, true);
        }
        return null;
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

      Log.Warn("Could not find translation for {0} in {1}", name, xmlFile.BaseURI);

      // Try using english translation if this translation was not found
      if (xmlFile.BaseURI != xmlFileFallback.BaseURI)
      {
        return Translate(node, name, true);
      }
      return null;
    }

    public static string ReverseTranslate(string node, string translation, bool fallback = false)
    {
      if (xmlFile == null || xmlFileFallback == null)
      {
        Init();
      }

      XmlNode xmlNode = fallback ? xmlFileFallback.DocumentElement.SelectSingleNode("/resources/" + node) : xmlFile.DocumentElement.SelectSingleNode("/resources/" + node);
      if (xmlNode == null)
      {
        Log.Warn("Could not find node {0} in {1}", node, xmlFile.BaseURI);

        // Try using english reverse translation if node was not found
        if (xmlFile.BaseURI != xmlFileFallback.BaseURI)
        {
          return ReverseTranslate(node, translation, true);
        }
        return null;
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

      Log.Warn("Could not reverse translate {0} in {1}", translation, xmlFile.BaseURI);

      // Try using english reverse translation if this translation was not found
      if (xmlFile.BaseURI != xmlFileFallback.BaseURI)
      {
        return ReverseTranslate(node, translation, true);
      }
      return null;
    }

    public static void Load(string languageFile)
    {
      if (xmlFile == null || xmlFileFallback == null)
      {
        Init();
      }

      xmlFile.Load(languageFile);
    }

    private static void Init()
    {
      if (string.IsNullOrEmpty(Settings.currentLanguageFile))
      {
        Settings.LoadSettings();
      }

      xmlFile = new XmlDocument();
      xmlFileFallback = new XmlDocument();

      xmlFile.Load(Settings.currentLanguageFile);
      xmlFileFallback.Load(Settings.currentLanguageFile.Substring(0, Settings.currentLanguageFile.LastIndexOf("\\") + 1) + "en.xml");
    }
  }
}
