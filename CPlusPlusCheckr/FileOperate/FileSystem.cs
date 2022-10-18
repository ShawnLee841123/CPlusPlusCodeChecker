using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FileSystem: Singleton<FileSystem>
{
	public string m_strFileName { get; protected set; }
	public string strFileContent { get; protected set; }

	public FileSystem()
	{
		Reset();
	}

	public void Reset()
	{
		m_strFileName = "";
		strFileContent = "";
	}

	public void SetFileName(string strValue)
	{
		if (!CheckStringValid(strValue))
			return;

		m_strFileName = strValue;
	}

	public void ReadFileLine(string strLine)
	{
		strFileContent += strLine;
	}

	public bool ReadFile(string strFileName)
	{
		SetFileName(strFileName);
		FileReader.Ins().ReadFileByLine(m_strFileName, ReadFileLine);
		if (!CheckStringValid(strFileContent))
		{
			return false;
		}

		return true;
	}

}

