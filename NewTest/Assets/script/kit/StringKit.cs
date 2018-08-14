using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class StringKit
{

	/** 符号标记常量 */
	public const char USD_SIGN='$',POUND_SIGN='#';

	public static byte[] DefaultToUTF8Byte (string p_str)
	{
		try {
			byte[] b = System.Text.Encoding.Default.GetBytes (p_str);
			byte[] c = System.Text.Encoding.Convert (System.Text.Encoding.Default, System.Text.Encoding.UTF8, b);		
			return c;
		} catch {
			return null;
		} 
	}

	public static string  intToFixString (int data)
	{
		return data.ToString("D3");
	}
	/// <summary>
	/// 字符串构建字符串数组
	/// </summary>
	/// <returns>The to string list.</returns>
	/// <param name="changeText">Change text.</param>
	/// <param name="tmpchar">Tmpchar.</param>
	public static string[] stringToStringList (string changeText, char[] tmpchar)
	{
		if (changeText == null)
			return null;
		
		string[] StringList;
		Char[] sp;
		if (tmpchar == null) {
	
			sp = new char[]{',','|'};
		} else
			sp = tmpchar;
			
			
		StringList = changeText.Split (sp);
		
		return StringList;
		
	}
	/// <summary>
	/// 字符串数值拼接字符串
	/// </summary>
	/// <returns>The list tostring.</returns>
	/// <param name="changeText">Change text.</param>
	/// <param name="tmpchar">Tmpchar.</param>
	public static string stringListTostring (string[] changeText, char tmpchar)
	{
		if (changeText == null)
			return null;
		StringBuilder sb = new StringBuilder ();
		for (int i=0; i<changeText.Length; i++) {
			sb.Append(changeText[i]+tmpchar);
		}
		string str = sb.ToString ();
		if (sb.Length > 0)
			str = str.Substring (0,sb.Length-1);
		return str;
	}

	public static int[] toArrayInt(string strValue,char sepChar)
	{
		string[] tempStr=strValue.Split(new char[]{sepChar},StringSplitOptions.RemoveEmptyEntries);
		int length=tempStr.Length;
		int[] tempInt=new int[length];
		for(int i=0;i<length;i++)
		{
			tempInt[i]=toInt(tempStr[i]);
		}
		return tempInt;
	}
	
	public static byte[] UTF8ToDefaultByte (byte[] p_bt)
	{
		try {
	
			return  System.Text.Encoding.Convert (System.Text.Encoding.UTF8, System.Text.Encoding.Default, p_bt);
		} catch {
			return null;
		} 
	}

	public static int   toInt (string strValue)
	{
		int tmpV;
		if (strValue == null)
			return 0;
		try {
			int.TryParse (strValue, out tmpV);
		} catch {
			return 0;
			 
		}
		return tmpV;
	}

	public static float toFloat(string strValue)
	{
		float tmpV;
		if (strValue == null)
			return 0;
		try {
			float.TryParse (strValue, out tmpV);
		} catch {
			return 0;
			
		}
		return tmpV;
	}

	public static long   toLong (string strValue)
	{
		long tmpV;
		if (strValue == null)
			return 0;
		try {
			long.TryParse (strValue, out tmpV);
		} catch {
			return 0;
			
		}
		return tmpV;
	}

	public static string UTF8ToDefaultString (byte[] p_bt)
	{
		try {
			byte[] d = System.Text.Encoding.Convert (System.Text.Encoding.UTF8, System.Text.Encoding.Default, p_bt);
			return System.Text.Encoding.Default.GetString (d);
		} catch {
			return null;
		} 
	}

//	//平台ID
//	private static long pid = 0;
//	//服务器ID
//	private static long sid = 0;

	public static string serverIdToFrontId(string str)
	{

//		pid = pid == 0 ? (toLong(UserManager.Instance.self.uid) >> 48) : pid;
//		sid = sid == 0 ? ((toLong(UserManager.Instance.self.uid) << 16) >> 48) : sid;
		long pid = toLong(UserManager.Instance.self.uid) >> 48;
		long sid = (toLong(UserManager.Instance.self.uid) << 16) >> 48;
		long serverUid = toLong(str);
		long id = (serverUid << 32) >> 32;
		return sid.ToString() + id.ToString();
	}

	public static string frontIdToServerId(string fId)
	{
		if(StringKit.toInt(fId)<0)
			return "error";

//		pid = pid == 0 ? (toLong(UserManager.Instance.self.uid) >> 48) : pid;
//		sid = sid == 0 ? ((toLong(UserManager.Instance.self.uid) << 16) >> 48) : sid;
		long pid = toLong(UserManager.Instance.self.uid) >> 48;
		long sid = (toLong(UserManager.Instance.self.uid) << 16) >> 48;

		if(fId.Length<= sid.ToString().Length){
			return "error";
		}
		long id = toLong(fId.Substring(sid.ToString().Length));
		return ((pid << 48)|(sid << 32)|id).ToString();
	}
	
	//判断输入内容是否纯数字
	public static bool isNum(string s)
    {
        string pattern = "^[0-9]*$";
        Regex rx = new Regex(pattern);
        return rx.IsMatch(s);
    }
	
	//用Math.Round()函数进行四舍五入，f1，1代表1个小数，另有nx写法
	public static string intToThousand (int number)
	{
		if (number >= 1000000000)
		{
			return (Math.Round((number / 1000000000f),2)).ToString("f1") + "b";
		}
		else if (number >= 1000000f && number < 1000000000)
		{
			return (Math.Round((number / 1000000f),2)).ToString("f1")+"m";
		}
		else if (number >= 1000 && number < 1000000)
		{
			return (Math.Round((number / 1000f),2)).ToString ("f1")+"k";
		}
		return number.ToString ();
	}
}
