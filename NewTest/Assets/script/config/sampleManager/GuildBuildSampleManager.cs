using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**公会建筑模板管理器
  *负责公会建筑模板信息的初始化 
  *@author 汤琦
  **/
public class GuildBuildSampleManager : SampleConfigManager
{
	//单例
	private static GuildBuildSampleManager _Instance;
	private static bool _singleton = true;
	
	public GuildBuildSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_GUILDBUILD);
	}
	
	public static GuildBuildSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new GuildBuildSampleManager ();
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
		GuildBuildSample sample = new GuildBuildSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得公会建筑模板对象
	public GuildBuildSample getGuildBuildSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as GuildBuildSample;
	} 
	
	//获得指定限制的所有公会建筑sid
	public List<int> getAllGuildBuild()
	{
		List<int> list = new List<int>();
		foreach (int key in data.Keys) { 
			list.Add (key);
		}
		return list;
	}
}
