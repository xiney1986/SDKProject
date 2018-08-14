using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**公会技能模板管理器
  *负责公会技能模板信息的初始化 
  *@author 汤琦
  **/
public class GuildSkillSampleManager : SampleConfigManager
{
	//单例
	private static GuildSkillSampleManager _Instance;
	private static bool _singleton = true;
	
	public GuildSkillSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_GUILDSKILL);
	}
	
	public static GuildSkillSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new GuildSkillSampleManager ();
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
		GuildSkillSample sample = new GuildSkillSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

	//获得公会技能模板对象
	public GuildSkillSample getGuildSkillSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as GuildSkillSample;
	} 
	
	//获得指定限制的所有公会技能sid
	public List<string> getAllGuildSkill()
	{
		List<string> list = new List<string>();
		foreach (int key in data.Keys) { 
			list.Add (key.ToString());
		}
		return list;
	}
}
