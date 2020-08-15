using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Singleton<T> where T : new()
{
	private static T _instance;
	private static object _lockObj = new object();

	public static T Ins()
	{
		if (null == _instance)
		{
			lock(_lockObj)
			{
				_instance = new T();
			}		
		}

		return _instance;
	}

	public bool CheckStringValid(string strValue)
	{
		if (null == strValue)
			return false;

		if (strValue.Length <= 0)
			return false;

		return true;
	}

	public bool CheckListValid<T1>(List<T1> listValue)
	{
		if (null == listValue)
			return false;

		if (listValue.Count <= 0)
			return false;

		return true;
	}

	public bool CheckArrayValid<T1>(T1[] arrValue)
	{
		if (null == arrValue)
			return false;

		if (arrValue.Length <= 0)
			return false;

		return true;
	}

	public void LogMsg(string strMsg)
	{
		string strOutput = string.Format("[MSG] {0}", strMsg);
		LogOut(strOutput);
	}

	public void LogError(string strMsg)
	{
		string strOutput = string.Format("[ERROR] {0}", strMsg);
		LogOut(strOutput);
	}

	public void LogWarnning(string strMsg)
	{
		string strOutput = string.Format("[WARNNING] {0}", strMsg);
		LogOut(strOutput);
	}

	protected void LogOut(string strMsg)
	{
		string strTime = DateTime.UtcNow.ToString("yyyy:MM:dd-HH:mm:ss:fff");
		string strOutput = string.Format("[{0}] {1}", strTime, strMsg);
		Console.WriteLine(strOutput);
	}
};
