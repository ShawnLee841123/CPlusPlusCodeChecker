using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TemplateFiles
{
#region Variable
	public string m_strReplaceWord { get; protected set; }
	
#endregion

#region Construct Func
	public TemplateFiles()
	{
		m_strReplaceWord = "";
	}

	public bool OnDestory()
	{
		return true;
	}

	public bool OnReadFileLine(string strLine)
	{
		if (!FileReader.Ins().CheckStringValid(strLine))
			return false;



		return true;
	}


#endregion

/*	File Content must form be
 * 
 *	class LXTLName( : public AAAA, )
 *	{
 *	public:
 *		LXTLName();
 *		(virtual) ~LXTLName();
 *		
 *	#param region xxxxx
 *		virtual bool Func1() override;
 *		virtual bool Func2() override;
 *		virtual bool Func3() override;
 *	#param endregion
 *	
 *	};
 */

}



