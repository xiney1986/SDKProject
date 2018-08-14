using System;
using System.Collections;
 
/**
 * 特效路径配置文件管理器
 * effect 目录下特效配置 键值对
 * @author longlingquan
 * */
public class EffectConfigManager:ConfigManager
{
	//单例
	private static EffectConfigManager instance;
	private Hashtable table;
	public const string PATH = "Effect/";

	public static EffectConfigManager Instance
	{ 
		get{
			if(instance==null)
				instance=new EffectConfigManager();
			return instance;
		}
	}

	public EffectConfigManager ()
	{ 
		table = new Hashtable ();
		base.readConfig (ConfigGlobal.CONFIG_EFFECT);
	}
	
	public override void parseConfig (string str)
	{
		string[] arr = str.Split ('|');
		int id = StringKit.toInt (arr [0]);
		table.Add (id, arr [1]);
	}
	
	//获得特效路径
	public string getEffectPerfab (int id)
	{ 
		if (table.Contains (id))
			return PATH + table [id] as string;
		return "";
	}
	 
} 
 

