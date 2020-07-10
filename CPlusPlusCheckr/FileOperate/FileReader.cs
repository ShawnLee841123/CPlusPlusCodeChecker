using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class FileReader: Singleton<FileReader>
{
	public enum ReadFileResultType
	{
		RFRT_SUCCESS = 0,				//	Read Success
		RFRT_ERROR_FILE_NAME,			//	File name is error(null or empty)
		RFRT_ERROR_FILE_OPEN_FAILED,    //	Can not open file
		RFRT_ERROR_FILE_STREAM_FAILED,	//	error file stream
		RFRT_NOFILE,					//	No this File
	};

	public ReadFileResultType ReadFileByLine(string strFile, Action<string> pFunc)
	{
		if (!CheckStringValid(strFile))
			return ReadFileResultType.RFRT_ERROR_FILE_NAME;

		using (FileStream pFileStream = new FileStream(strFile, FileMode.Open))
		{
			if (null == pFileStream)
				return ReadFileResultType.RFRT_ERROR_FILE_OPEN_FAILED;

			StreamReader pReader = new StreamReader(pFileStream);
			if (null == pReader)
				return ReadFileResultType.RFRT_ERROR_FILE_STREAM_FAILED;

			while (!pReader.EndOfStream)
			{
				pFunc(pReader.ReadLine());
			}

			pReader.Dispose();
			pReader.Close();
			pReader = null;

			pFileStream.Dispose();
			pFileStream.Close();
		}

		return ReadFileResultType.RFRT_SUCCESS;
	}

	public ReadFileResultType ReadFileSizeBySize(string strFile, int nBufferSize, Action<char[]> pFunc)
	{
		if (!CheckStringValid(strFile))
			return ReadFileResultType.RFRT_ERROR_FILE_NAME;

		using (FileStream pFileStream = new FileStream(strFile, FileMode.Open))
		{
			if (null == pFileStream)
				return ReadFileResultType.RFRT_ERROR_FILE_OPEN_FAILED;

			StreamReader pReader = new StreamReader(pFileStream);

			if (null == pReader)
				return ReadFileResultType.RFRT_ERROR_FILE_STREAM_FAILED;

			int nCurPos = 0;
			char[] arrBuffer = new char[nBufferSize];
			while(!pReader.EndOfStream)
			{
				pReader.ReadBlock(arrBuffer, nCurPos, nBufferSize);
				pFunc(arrBuffer);
				nCurPos += nBufferSize;
			}

			pReader.Dispose();
			pReader.Close();
			pReader = null;

			pFileStream.Dispose();
			pFileStream.Close();
		}

		return ReadFileResultType.RFRT_SUCCESS;
	}
}

