using System;
using System.Collections;

/**语言包 
  *@author longlingquan
  **/
public class LanguageConfigManager:ConfigManager
{
	Hashtable languages;
	//单例
	private static LanguageConfigManager _Instance;
	private static bool _singleton = true;
	 
	public static LanguageConfigManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new LanguageConfigManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public string  coverText (string text)
	{
		if (text.Length > 1 && text.Substring (0, 1) == "@" && GameManager.Instance != null) {
			return getLanguage (text.Substring (1, text.Length - 1));

		}

		return text;
	}

	public LanguageConfigManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}    
		languages = new Hashtable ();
		base.readConfig (ConfigGlobal.CONFIG_LANGUAGE);
	}
	
	//解析配置
	public override void parseConfig (string str)
	{
		string[] arr = str.Split ('|');
		string key = arr [0];
		if(arr.Length<=1)
			NGUIDebug.Log(str);
	
		languages.Add (key, arr [1]);
	}
	
	public string getLanguage (string key)
	{
		return parseLanguage (key, null);
	}

	public string getLanguage (string key, string str)
	{
		string[] arr = {str};
		return parseLanguage (key, arr);
	}

	public string getLanguage (string key, string str1, string str2)
	{
		string[] arr = {str1, str2};
		return parseLanguage (key, arr);
	}

	public string getLanguage (string key, string str1, string str2, string str3)
	{
		string[] arr = {str1, str2, str3};
		return parseLanguage (key, arr);
	}
	
	public string getLanguage (string key, string str1, string str2, string str3, string str4)
	{
		string[] arr = {str1, str2, str3, str4};
		return parseLanguage (key, arr);
	}

	public string getLanguage (string key, string str1, string str2, string str3, string str4, string str5)
	{
		string[] arr = {str1, str2, str3, str4,str5};
		return parseLanguage (key, arr);
	}
	
	public string parseLanguage (string key, object[] param)
	{
		string str = languages [key] as string;
		if (str == null || str == "")
			return "error! " + key;
		if (param != null && param.Length > 0) {
			for (int i = 0; i < param.Length; i++) {
				string temp = "%" + (i + 1).ToString ();
				str = str.Replace (temp, param [i].ToString());
			}
		}
		
		return replace (str);
	}
	//替换换行符
	private static string replace (string desc)
	{
		return desc.Replace ('~', '\n');
	} 
	
} 

