using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

#if DESKTOP_VERSION
using System.IO.IsolatedStorage;
#endif

namespace INIAccess
{
  class INIEntry 
  {
    public String strSection;
    public String strKey;
    public String strValue;

  }

  class INIWriterForCE
  {
    public Boolean bINIAvailable;
    String strINIFile;
    INIEntry[] entryArray;
		Boolean bFileIsClosed = false;

		/// <summary>
		/// constructor: opens the file and sets the bINIAvailable status
		/// </summary>
		/// <param name="strINIFileToWrite"></param>
    public INIWriterForCE (String strINIFileToWrite)
    {
      entryArray = new INIEntry[0];

      // test to see if filename is valid
      bINIAvailable = true;
      strINIFile = "";
      try
      {
        StreamWriter sw = new StreamWriter(strINIFileToWrite);
        sw.Close();
        strINIFile = strINIFileToWrite;
      }
      catch (SystemException ex)
      {
        MessageBox.Show(ex.ToString());
        bINIAvailable = false;
      }
    } // INIWriterForCE constructor

		/// <summary>
		/// destructor: writes the accumulated information to the file
		/// </summary>
    ~INIWriterForCE()
    {
			if (!bFileIsClosed)
				CloseTheFile();
    }

		public void CloseTheFile()
		{
			if (!bINIAvailable)
				return;

			StreamWriter sw = new StreamWriter(strINIFile);
			String strLastSection = "";

			for (int i = 0; i < entryArray.Length; i++)
			{
				if (entryArray[i].strSection != strLastSection)
				{
					sw.WriteLine("[" + entryArray[i].strSection + "]");
					strLastSection = entryArray[i].strSection;
				}
				sw.WriteLine(entryArray[i].strKey + "=" + entryArray[i].strValue);
			}

			sw.Flush();
			sw.Close();

			bFileIsClosed = true;
		}

		/// <summary>
		/// Write a string to the list of entries.  This is used by all of the other "Write" functions
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="strValue"></param>
    public void WriteString(String strSection, String strKey, String strValue)
    {
      INIEntry thisEntry = new INIEntry();

      thisEntry.strSection = strSection;
      thisEntry.strKey = strKey;
      thisEntry.strValue = strValue;
      entryArray = AppendWriteEntry(entryArray, thisEntry);
    }

		/// <summary>
		/// Write an integer to the list of entries
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="nValue"></param>
    public void WriteInt (String strSection, String strKey, int nValue)
    {
      WriteString(strSection, strKey, nValue.ToString());
    }

		/// <summary>
		/// Write a double to the list of entries
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="dValue"></param>
    public void WriteDouble(String strSection, String strKey, double dValue)
    {
      WriteString(strSection, strKey, dValue.ToString("f9"));
    }

		/// <summary>
		/// Write a Boolean to the list of entries
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="bValue"></param>
    public void WriteBoolean(String strSection, String strKey, Boolean bValue)
    {
      WriteString(strSection, strKey, bValue.ToString());
    }

		/// <summary>
		/// Append a new entry to the current list of entries
		/// </summary>
		/// <param name="theArray"></param>
		/// <param name="newItem"></param>
		/// <returns></returns>
    private INIEntry[] AppendWriteEntry(INIEntry[] theArray, INIEntry newItem)
    {
      INIEntry[] newArray = new INIEntry[theArray.Length + 1];
      newArray[newArray.Length - 1] = new INIEntry();
      Array.Copy(theArray, newArray, theArray.Length);
      newArray[newArray.Length - 1].strValue = newItem.strValue;
      newArray[newArray.Length - 1].strKey = newItem.strKey;
      newArray[newArray.Length - 1].strSection = newItem.strSection;

      return newArray;

    } // AppendReadEntry

	} // end class INIWriterForCE


	///////////////////////////////////////////////////////////////////////////
	class INIReaderForCE
  {
    public Boolean bINIAvailable;
    INIEntry[] entryArray;

		/// <summary>
		/// Constructor: opens file and reads all of the entries
		/// </summary>
		/// <param name="strINIFile"></param>
    public INIReaderForCE(String strINIFile)
    {
      bINIAvailable = ReadALLIni(strINIFile);
    }

		/// <summary>
		/// read all of the ini entries from a given file
		/// </summary>
		/// <param name="strINIFile"></param>
		/// <returns></returns>
    private Boolean ReadALLIni(String strINIFile)
    {
      INIEntry thisEntry = new INIEntry();
      String strCurrentLine;
      String strCurrentSection = "[]";
      char[] chEquals = { '=' };
      String[] subStrings;
      FileInfo fileInfo = new FileInfo(strINIFile);
      if (!fileInfo.Exists)
        return false;

      entryArray = new INIEntry[0];
      StreamReader sr = new StreamReader(strINIFile);

      do
      {
        strCurrentLine = sr.ReadLine();
        if ((strCurrentLine != null) && (strCurrentLine != ""))
        {
          if ((strCurrentLine[0] == '[') && (strCurrentLine[strCurrentLine.Length - 1] == ']'))
            strCurrentSection = strCurrentLine.Substring(1, strCurrentLine.Length - 2);
          else
          {
            subStrings = strCurrentLine.Split(chEquals);
            thisEntry.strSection = strCurrentSection;
            thisEntry.strKey = subStrings[0];
            thisEntry.strValue = subStrings[1];
            entryArray = AppendReadEntry(entryArray, thisEntry);
          }
        }
      } while (strCurrentLine != null) ;

      sr.Close();

			return (entryArray.Length > 0);

    } // ReadALLIni

		/// <summary>
		/// appends an entry to the list of entries
		/// </summary>
		/// <param name="theArray"></param>
		/// <param name="newItem"></param>
		/// <returns></returns>
		private INIEntry[] AppendReadEntry(INIEntry[] theArray, INIEntry newItem)
		{
			INIEntry[] newArray = new INIEntry[theArray.Length + 1];
			newArray[newArray.Length - 1] = new INIEntry();
			Array.Copy(theArray, newArray, theArray.Length);
			newArray[newArray.Length - 1].strValue = newItem.strValue;
			newArray[newArray.Length - 1].strKey = newItem.strKey;
			newArray[newArray.Length - 1].strSection = newItem.strSection;

			return newArray;

		} // AppendReadEntry


		/// <summary>
		/// extract a string from the list of entries
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="strDefaultValue"></param>
		/// <returns></returns>
    public String ReadString (String strSection, String strKey, String strDefaultValue)
    {
      Boolean bFound = false;
      int nRef = 0;
      do 
      {
        if ((entryArray[nRef].strSection == strSection) && (entryArray[nRef].strKey == strKey))
          bFound = true;
        else
          nRef++;
      } while ((nRef < entryArray.Length) && (!bFound));

      if (bFound)
        return entryArray[nRef].strValue;

      return strDefaultValue;

    } // ReadString

		/// <summary>
		/// extract an int from the list of entries
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="nDefaultValue"></param>
		/// <returns></returns>
    public int ReadInt (String strSection, String strKey, int nDefaultValue)
    {
      String strDefault = "Not Found";
      String strResult = ReadString(strSection, strKey, strDefault);

      if (strResult == strDefault)
        return nDefaultValue;

      return Convert.ToInt32(strResult);

    } // ReadInt

		/// <summary>
		/// extract a double from the list of entries
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="dDefaultValue"></param>
		/// <returns></returns>
    public double ReadDouble(String strSection, String strKey, double dDefaultValue)
    {
      // use this approach to avoid roundoff with default value switching to strings
      String strDefault = "Not Found";
      String strResult = ReadString(strSection, strKey, strDefault);

      if (strResult == strDefault)
        return dDefaultValue;

      return Convert.ToDouble(strResult);

		} // ReadDouble

		/// <summary>
		/// extract a Boolean from the list of entries
		/// </summary>
		/// <param name="strSection"></param>
		/// <param name="strKey"></param>
		/// <param name="bDefaultValue"></param>
		/// <returns></returns>
    public Boolean ReadBoolean(String strSection, String strKey, Boolean bDefaultValue)
    {
      return Convert.ToBoolean(ReadString(strSection, strKey, bDefaultValue.ToString()));

		} // ReadBoolean

	} // INIReaderForCE
}
