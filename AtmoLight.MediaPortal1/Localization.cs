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


    public static string Translate(string node, string name, bool fallback = false)
    {
      if (xmlFile == null || fallback)
      {
        Settings.LoadSettings();
        xmlFile = new XmlDocument();
        if (fallback)
        {
          xmlFile.Load(Settings.currentLanguageFile.Substring(0, Settings.currentLanguageFile.LastIndexOf("\\")) + "en.xml");
        }
        else
        {
          xmlFile.Load(Settings.currentLanguageFile);
        }
      }

      XmlNode xmlNode = xmlFile.DocumentElement.SelectSingleNode("/ressources/" + node);
      if (xmlNode == null)
      {
        // Try using english translation if node was not found
        if (Settings.currentLanguageFile != Settings.currentLanguageFile.Substring(0, Settings.currentLanguageFile.LastIndexOf("\\")) + "en.xml")
        {
          return Translate(node, name, true);
        }
        return null;
      }

      foreach (XmlNode childNodes in xmlNode.ChildNodes)
      {
        if (childNodes.Attributes[0].Value == name)
        {
          return childNodes.InnerText;
        }
      }

      // Try using english translation if this translation was not found
      if (Settings.currentLanguageFile != Settings.currentLanguageFile.Substring(0, Settings.currentLanguageFile.LastIndexOf("\\")) + "en.xml")
      {
        return Translate(node, name, true);
      }
      return null;
    }
  }
}
