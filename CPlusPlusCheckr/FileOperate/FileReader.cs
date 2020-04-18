using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class FileReader: Singleton<FileReader>
{
	public enum ReadErrorType
	{
		RET_SUCCESS = 0,			//	Read Success
		RET_ERROR_FILE_NAME,		//	File name is error(null or empty)
		RET_ERROR_FILE_OPEN_FAILED,	//	Can not open file
		RET_NOFILE,					//	No this File

	};

	public ReadErrorType ReadFile(string strFile)
	{
		if (!CheckStringValid(strFile))
			return ReadErrorType.RET_ERROR_FILE_NAME;

		using (FileStream pFileStream = new FileStream(strFile, FileMode.Open))
		{
			if (null == pFileStream)
				return ReadErrorType.RET_ERROR_FILE_OPEN_FAILED;

			StreamReader pReader = new StreamReader(pFileStream);
			if (null == pReader)
				return ReadErrorType.RET_ERROR_FILE_OPEN_FAILED;

		}

		return ReadErrorType.RET_SUCCESS;
	}
}

