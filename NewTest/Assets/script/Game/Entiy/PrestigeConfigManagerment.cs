using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrestigeConfigManagerment:SampleConfigManager
{
	//单例
	private static PrestigeConfigManagerment instance;
	private List<PrestigeSample> list;
	
	public PrestigeConfigManagerment ()
	{
		base.readConfig (ConfigGlobal.CONFIG_PRESTIGE);
	}
	
	public static PrestigeConfigManagerment Instance {
		get {
			if (instance == null)
				instance = new PrestigeConfigManagerment ();
			return instance;
		}
	}

	public PrestigeSample[] getAllPrestige ()
	{
		return list.ToArray ();
	}

	public PrestigeSample getPrestigeSampleByLevel (int level)
	{
		if (level >= list.Count)
			return null;

		return list [level];
	}



	//解析配置
	public override void parseConfig (string str)
	{  
		PrestigeSample be = new PrestigeSample (str);
		if (list == null)
			list = new List<PrestigeSample> ();
		list.Add (be);
	}




}
