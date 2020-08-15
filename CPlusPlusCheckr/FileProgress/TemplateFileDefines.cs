using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TemplateFileDefines
{

}

#region Function Param define

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

#endregion

#region Function Declear define

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

		strFunctionDeclear = OnFunctionReturnTypeParse(strFunctionDeclear);
		if ("" == strFunctionDeclear)
			return false;

		strFunctionDeclear = OnFunctionNameParse(strFunctionDeclear);
		if ("" == strFunctionDeclear)
			return false;

		string[] arrNameList = strFunctionDeclear.Split(new char[] { ')' });
		if (!FileReader.Ins().CheckStringValid(arrNameList[0]))
			return false;

		int nParam = AddParamList(arrNameList[0]);

		return true;
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
	//	parse function name
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

#endregion

#region Field Base define

public class TemplateFieldBase
{
#region Field Parse Result
	public enum EFieldParseResultType
	{
		EFPRT_PARSE_FAILED = 0,				//	parse failed
		EFPRT_PARSE_SUCCESS,				//	parse success
		EFPRT_CHILD_PARSE_SUCCESS,			//	child field parse success
		EFPRT_NEW_CHILD,					//	child field begin
		EFPRT_END_FIELD,					//	child field end

		EFPRT_MAX
	}
#endregion

	#region Variable

	protected Dictionary<string, TemplateFieldBase> dicChild = null;
	public string Name { get; protected set; }

	protected TemplateFieldBase curChild = null;

	protected bool Parsed = true;
	protected bool Finished = false;

#endregion

	public TemplateFieldBase()
	{
		Name = "";
		if (null == dicChild)
			dicChild = new Dictionary<string, TemplateFieldBase>();
	}

	public TemplateFieldBase(string strName)
	{
		Name = strName;
		if (null == dicChild)
			dicChild = new Dictionary<string, TemplateFieldBase>();
	}

	public virtual bool AddChild(string strName, TemplateFieldBase oChild)
	{
		if (null == dicChild)
			dicChild = new Dictionary<string, TemplateFieldBase>();

		if (!FileReader.Ins().CheckStringValid(strName))
		{
			return false;
		}

		if (null == oChild)
		{
			return false;
		}

		if (dicChild.ContainsKey(strName))
		{
			return false;
		}

		dicChild.Add(strName, oChild);
		return true;
	}

	public TemplateFieldBase GetChild(string strChildName)
	{
		if (!FileReader.Ins().CheckStringValid(strChildName))
		{
			return null;
		}

		if (dicChild.Count <= 0)
		{
			return null;
		}

		if (dicChild.ContainsKey(strChildName))
		{
			return dicChild[strChildName];
		}

		return null;
	}

	public bool SetName(string strName)
	{
		if (FileReader.Ins().CheckStringValid(strName))
		{
			Name = strName;
			return true;
		}

		return false;
	}

	public virtual EFieldParseResultType OnReadFileLine(string strLine)
	{
		if (null != curChild)
		{
			EFieldParseResultType eRet = curChild.OnFieldParse(strLine);
			if (EFieldParseResultType.EFPRT_PARSE_SUCCESS == eRet)
			{
				//	child parse success
				return EFieldParseResultType.EFPRT_CHILD_PARSE_SUCCESS;
			}
			else if (EFieldParseResultType.EFPRT_NEW_CHILD == eRet)
			{
				//	child create child
				return EFieldParseResultType.EFPRT_NEW_CHILD;
			}
			else if (EFieldParseResultType.EFPRT_END_FIELD == eRet)
			{
				
				curChild = null;
				return EFieldParseResultType.EFPRT_END_FIELD;
			}

			//	parse failed
			return EFieldParseResultType.EFPRT_PARSE_FAILED;
		}

		return OnFieldParse(strLine);
	}

	public virtual EFieldParseResultType OnFieldParse(string strLine)
	{
		//	结束标记
		if (Finished)
			return EFieldParseResultType.EFPRT_PARSE_FAILED;

		TemplateFieldBase child = ParseNewField(strLine);
		if (null != child)
		{
			curChild = child;
			return EFieldParseResultType.EFPRT_NEW_CHILD;
		}

		return EFieldParseResultType.EFPRT_PARSE_FAILED;
	}

	public virtual TemplateFieldBase ParseNewField(string strLine)
	{
		if (strLine.Contains("#param region"))
		{
			TemplateRegionDefine tempRegion = new TemplateRegionDefine();
			if (tempRegion.ParseRegionName(strLine))
				return tempRegion;
		}
		else if (strLine.Contains("class "))
		{
			TemplateClassDefine tempClass = new TemplateClassDefine();
			if (tempClass.ParseClassName(strLine))
				return tempClass;
		}

		return null;
	}

	public virtual bool ParseRegionName(string strLine)
	{
		return false;
	}
}
#endregion

public class TemplateRegionDefine : TemplateFieldBase
{
	public TemplateRegionDefine()
	{ }

	public new EFieldParseResultType OnFieldParse(string strLine)
	{
		EFieldParseResultType eBaseRet = base.OnFieldParse(strLine);

		if (EFieldParseResultType.EFPRT_PARSE_FAILED != eBaseRet)
			return eBaseRet;
		
		//	field end
		if (strLine.Contains("#param endregion"))
		{
			Finished = true;
			return EFieldParseResultType.EFPRT_END_FIELD;
		}
		
		//	todo

		return EFieldParseResultType.EFPRT_PARSE_FAILED;
	}

	public new TemplateFieldBase ParseNewField(string strLine)
	{
		return base.ParseNewField(strLine);
	}

	public new bool ParseRegionName(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return false;

		if (!strLine.Contains("#param region"))
			return false;

		string strName = strLine.Replace("#param region ", "");
		if (!FileReader.Ins().CheckStringValid(strName))
			return false;

		return SetName(strName);
	}
}

#region Class Declear define

public class TemplateClassDefine : TemplateFieldBase
{
	public TemplateClassDefine()
	{ }

	public new EFieldParseResultType OnFieldParse(string strLine)
	{
		EFieldParseResultType eBaseRet = base.OnFieldParse(strLine);

		if (EFieldParseResultType.EFPRT_PARSE_FAILED != eBaseRet)
			return eBaseRet;

		//	field end
		if (strLine.Contains("};"))
		{
			Finished = true;
			return EFieldParseResultType.EFPRT_END_FIELD;
		}

		//	todo

		return EFieldParseResultType.EFPRT_PARSE_FAILED;
	}

	public new TemplateFieldBase ParseNewField(string strLine)
	{
		return base.ParseNewField(strLine);
	}

	public bool ParseClassName(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return false;

		if (!strLine.Contains("class"))
			return false;

		string strKeyNames = "";
		if (strLine.Contains(":"))
		{
			string[] arrKeyNames = strLine.Split(new char[] { ':' });
			strKeyNames = arrKeyNames[0];
			//	Parent or Interface
		}
		else
		{
			strKeyNames = strLine;
		}

		string strTempName = strKeyNames.Replace("class ", "");

		bool bNamed = SetName(strTempName.Replace(" ", ""));
		//	todo Parent or Interface

		return bNamed;
	}
}

#endregion