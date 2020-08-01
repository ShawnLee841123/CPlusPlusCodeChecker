using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TemplateFileDefines
{
	
}

public class TemplateFunctionParamDefine
{
#region Variable

	//	参数类型
	public string m_strParamType = "";
	//	参数名
	public string m_strParamName = "";
	//	指针符号
	public string m_strPointer = "";
	//	引用符号
	public string m_strRef = "";
	//	默认值
	public string m_strDefaultValue = "";
	//	默认值标记
	public string m_strDefaultFlag = "";
	//	开头常量标记
	public string m_strBeginConstFlag = "";
	//	接引用、指针常量标记
	public string m_strParamTypeEndConstFlag = "";

#endregion
	public TemplateFunctionParamDefine()
	{
	}

#region Parse Process
	//	param parse enter
	public bool OnParamParse(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return false;

		//	default value first
		string strParamFront = OnDefaultValueParse(strLine);
		//	param name after that
		string strParamTypeField = OnParamNameParse(strParamFront);
		//	param const after name
		string strNoConstField = OnConstFlagParse(strParamTypeField);

		//	replace double space
		while (strNoConstField.Contains("  "))
			strNoConstField.Replace("  ", " ");

		m_strParamType = strNoConstField;

		return true;
	}
#region Param Element Parse

	//	default value parse
	public string OnDefaultValueParse(string strParam)
	{
		if (!FileReader.Ins().CheckStringValid(strParam))
			return "";

		string[] arrDefaultProcess = strParam.Split(new char[] { '=' });
		if (2 == arrDefaultProcess.Length)
		{
			m_strDefaultFlag = "=";
			m_strDefaultValue = arrDefaultProcess[1].Replace(" ", "");
		}

		string strParamFront = arrDefaultProcess[0];
		return strParamFront;
	}
	//	"&" parse
	public string OnParamRefParse(string strParam)
	{
		if (!FileReader.Ins().CheckStringValid(strParam))
			return "";

		int nRefCharacterCount = GetCharacterCountInString(strParam, "&");
		if (nRefCharacterCount <= 0)
			return strParam;

		for (int i = 0; i < nRefCharacterCount; i++)
		{
			m_strRef += "&";
		}

		string[] arrRef = strParam.Split(new char[] { '&' });
		m_strParamName = arrRef[arrRef.Length - 1].Replace(" ", "");
		return arrRef[0];
	}
	//	"*" parse
	public string OnParamPointerParse(string strParam)
	{
		if (!FileReader.Ins().CheckStringValid(strParam))
			return "";

		int nPointerCharacterCount = GetCharacterCountInString(strParam, "*");
		if (nPointerCharacterCount <= 0)
			return strParam;

		for (int i = 0; i < nPointerCharacterCount; i++)
		{
			m_strPointer += "*";
		}

		string[] arrPointer = strParam.Split(new char[] { '*' });
		m_strParamName = arrPointer[arrPointer.Length - 1].Replace(" ", "");
		return arrPointer[0];
	}
	//	name parse
	public string OnParamNameParse(string strParam)
	{
		if (!FileReader.Ins().CheckStringValid(strParam))
			return "";

		//	"&"
		string strAfterRef = OnParamRefParse(strParam);
		if ("" != m_strParamName)
			return strAfterRef;

		//	"*"
		string strAfterPointer = OnParamPointerParse(strParam);
		if ("" != m_strParamName)
			return strAfterPointer;

		string[] arrSpace = strAfterPointer.Split(new char[] { ' ' });
		m_strParamName = arrSpace[arrSpace.Length - 1];

		string strParamTypeField = strAfterPointer.Replace(m_strParamName, "");
		return strParamTypeField;
	}
	//	const parse
	public string OnConstFlagParse(string strParam)
	{
		if (!FileReader.Ins().CheckStringValid(strParam))
			return "";

		string strReturn = strParam;
		if (strParam.Contains("const"))
		{
			int nFirstPos = strParam.IndexOf("const");
			int nLastPos = strParam.LastIndexOf("const");

			if (nFirstPos == 0)
				m_strBeginConstFlag = "const";

			if (nLastPos > 0)
				m_strParamTypeEndConstFlag = "const";

			if (m_strBeginConstFlag != "")
				strReturn = strReturn.Replace("const ", "");

			if (m_strParamTypeEndConstFlag != "")
				strReturn = strReturn.Replace(" const", "");
		}

		return strReturn;
	}
#endregion
	//	get spectial character count
	public int GetCharacterCountInString(string strParam, string strKey)
	{
		if (!FileReader.Ins().CheckStringValid(strParam))
			return -1;

		if (!FileReader.Ins().CheckStringValid(strKey))
			return -2;

		int nCharacterCount = 0;
		if (strParam.Contains(strKey))
		{
			int nFirstKeyPos = strParam.IndexOf(strKey);
			int nLastKeyPos = strParam.LastIndexOf(strKey);
			string strSubKeyString = strParam.Substring(nFirstKeyPos, nLastKeyPos - nFirstKeyPos + 1);
			string strNoSpace = strSubKeyString.Replace(" ", "");
			nCharacterCount = strNoSpace.Length;
		}

		return nCharacterCount;
	}
#endregion
}

public class TemplateFunctionDefine
{
	//	返回类型
	public string m_strReturnType = "";
	//	虚函数标记
	public string m_strVirFlag = "";
	//	函数名
	public string m_strFuncName = "";
	//	常量函数标记
	public string m_strConstFunctionFlag = "";
	//	覆盖标记
	public string m_strOverFlag = "";
	//	纯虚标记
	public string m_strAbsVirFlag = "";

	public List<TemplateFunctionParamDefine> m_listParamList;

	public TemplateFunctionDefine()
	{
		m_listParamList = null;
	}

	public bool OnParseFunction(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return false;

		//	virtual and abs virtual
		string strFunctionDeclear = OnFunctionVirParse(strLine);
		if ("" == strFunctionDeclear)
			return false;

		strFunctionDeclear = OnFunctionOverride(strFunctionDeclear);
		if ("" == strFunctionDeclear)
			return false;

		strFunctionDeclear = OnFunctionConstParse(strFunctionDeclear);
		if ("" == strFunctionDeclear)
			return false;
	}

	//	parse function virtual
	public string OnFunctionVirParse(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return "";

		string strFunctionDeclear = strLine;
		if (strLine.Contains("virtual"))
		{
			m_strVirFlag = "virtual";
			strFunctionDeclear = strFunctionDeclear.Replace("virtual ", "");
		}

		if (strLine.Contains("="))
		{
			if ("" == m_strVirFlag)
				return "";

			m_strAbsVirFlag = "0";
			string[] arrFunctionBody = strFunctionDeclear.Split(new char[] { '=' });
			strFunctionDeclear = arrFunctionBody[0];
		}

		return strFunctionDeclear;
	}
	//	parse function override
	public string OnFunctionOverride(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return "";

		string strFuncDelear = strLine;
		if (strLine.Contains("override"))
		{
			m_strOverFlag = "override";
			strFuncDelear = strLine.Replace(" override", "");
		}
	
		return strFuncDelear;
	}

	//	parse const variable calling func
	public string OnFunctionConstParse(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return "";

		string strFuncDelear = strLine;
		if (strLine.Contains("const")) 
		{ 
			int nConstPos = strLine.LastIndexOf("const");
			int nParamEndPos = strLine.IndexOf(")");
			bool bConstFlag = nConstPos > nParamEndPos;

			if (strLine.Contains(") const"))
			{
				strFuncDelear = strFuncDelear.Replace(") const", ")");
					
			}
			else if (strLine.Contains(")const"))
			{
				strFuncDelear = strFuncDelear.Replace(")const", ")");
			}

			m_strConstFunctionFlag = bConstFlag ? "const" : "";
		}

		return strFuncDelear;
	}
	//	parse return type
	public string OnFunctionReturnTypeParse(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return "";

		string strFunctionDelear = strLine;
		string[] arrDevide = strLine.Split(new char[] { ' ' });
		if (arrDevide.Length < 2)
			return "";

		m_strReturnType = arrDevide[0];
		string strTemp = m_strReturnType;
		strTemp += " ";
		strFunctionDelear = strFunctionDelear.Replace(strTemp, "");
		return strFunctionDelear;
	}

	public string OnFunctionNameParse(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return "";

		string strFuncDeclear = strLine;
		string[] arrNameDevide = strLine.Split(new char[] { '(' });
		if (arrNameDevide.Length < 2)
			return "";

		m_strFuncName = arrNameDevide[0];
		strFuncDeclear = arrNameDevide[arrNameDevide.Length - 1];

		return strFuncDeclear;
	}

	public int AddParamList(string strLine)
	{
		int nParamCount = 0;
		if (!FileReader.Ins().CheckStringValid(strLine))
			return nParamCount;

		string[] arrParam = strLine.Split(new char[] { ',' });
		if (arrParam.Length <= 0)
			return nParamCount;

		for (int i = 0; i < arrParam.Length; i++)
		{
			if (!ParseFunctionParam(arrParam[i]))
			{
				break;
			}
		}

		return nParamCount;
	}

	public bool ParseFunctionParam(string strParam)
	{
		if (!FileReader.Ins().CheckStringValid(strParam))
			return false;

		string[] arrParam = strParam.Split(new char[] { ',' });
		if (arrParam.Length <= 0)
			return false;

		for (int i = 0; i < arrParam.Length; i++)
		{
			TemplateFunctionParamDefine param = new TemplateFunctionParamDefine();
			if (param.OnParamParse(arrParam[i]))
			{
				m_listParamList.Add(param);
			}
		}

		return true;
	}

}
