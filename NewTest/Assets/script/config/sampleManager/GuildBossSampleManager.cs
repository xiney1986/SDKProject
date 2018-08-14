using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**公会BOSS信息模板管理器
  *负责公会BOSS信息模板信息的初始化 
  *@author 汤琦
  **/
public class GuildBossSampleManager : SampleConfigManager 
{
	//单例
	private static GuildBossSampleManager _Instance;
	private static bool _singleton = true;
	
	public GuildBossSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_GUILDBOSS);
	}
	
	public static GuildBossSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new GuildBossSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	//解析模板数据
	public override void parseSample (int sid)
	{
		GuildBossSample sample = new GuildBossSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得公会BOSS信息模板对象
	public GuildBossSample getGuildBossSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as GuildBossSample;
	} 
	
	//获得指定限制的所有公会BOSS信息sid
	public List<string> getAllGuildBoss()
	{
		List<string> list = new List<string>();
		foreach (int key in data.Keys) { 
			list.Add (key.ToString());
		}
		return list;
	}
}
