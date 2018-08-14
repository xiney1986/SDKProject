using System;
using System.IO;
using System.Collections.Generic;

public class Translator
{
	/* static fields */
	private readonly static char[] chars = {'='};
	private readonly static string variablePrefix = "%$";
	private readonly static string variableSuffix = "%";
	private static Dictionary<string,string> dic = null;
	
	/* fields */
	
	/* methods */
	public static void init (byte[] bytes)
	{
		dic = new Dictionary<string, string> ();
		ByteReader reader = new ByteReader (bytes);
		while (reader.canRead) {
			string str = reader.ReadLine ().Trim ();
			if (str.StartsWith ("#"))
				continue;
			string[] strs = str.Split (chars);
			if (strs.Length < 2)
				continue;
			dic [strs [0]] = strs [1];
		}
	}

	public static string trans (string key)
	{
		return trans (key, null, null, null);
	}

	public static string trans (string key, string replace1)
	{
		return trans (key, replace1, null, null);
	}

	public static string trans (string key, string replace1, string replace2)
	{
		return trans (key, replace1, replace2, null);
	}

	public static string trans (string key, string replace1, string replace2, string replace3)
	{
		if (dic == null) {
			throw new ArgumentNullException (typeof(Translator) + ", must init");
		}
		if(!dic.ContainsKey(key))
		{
			return key;
		}
		string str = dic [key];
		if (replace1 != null) {
			str = str.Replace (variablePrefix + 1 + variableSuffix, replace1);
			if (replace2 != null) {
				str = str.Replace (variablePrefix + 2 + variableSuffix, replace2);
				if (replace3 != null) {
					str = str.Replace (variablePrefix + 3 + variableSuffix, replace3);
				}
			}
		}
		return str;
	}
}