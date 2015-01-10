﻿using System;
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
          xmlFile.Load(Settings.currentLanguageFile.Substring(0, Settings.currentLanguageFile.LastIndexOf("\\") + 1) + "en.xml");
        }
        else
        {
          xmlFile.Load(Settings.currentLanguageFile);
        }
      }

      XmlNode xmlNode = xmlFile.DocumentElement.SelectSingleNode("/resources/" + node);
      if (xmlNode == null)
      {
        Log.Warn("Could not find node {0} in {1}", node, xmlFile.BaseURI);

        // Try using english translation if node was not found
        if (Settings.currentLanguageFile != Settings.currentLanguageFile.Substring(0, Settings.currentLanguageFile.LastIndexOf("\\") + 1) + "en.xml")
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
      if (Settings.currentLanguageFile != Settings.currentLanguageFile.Substring(0, Settings.currentLanguageFile.LastIndexOf("\\") + 1) + "en.xml")
      {
        return Translate(node, name, true);
      }
      return null;
    }

    public static string ReverseTranslate(string node, string translation)
    {
      if (xmlFile == null)
      {
        Settings.LoadSettings();
        xmlFile = new XmlDocument();
        xmlFile.Load(Settings.currentLanguageFile);
      }

      XmlNode xmlNode = xmlFile.DocumentElement.SelectSingleNode("/resources/" + node);
      if (xmlNode == null)
      {
        Log.Warn("Could not find node {0} in {1}", node, xmlFile.BaseURI);
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
      return null;
    }
  }
}
