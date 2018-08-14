using UnityEngine;
using System.Collections;

/// <summary>
/// 竞技场基础信息模版管理器
/// </summary>
public class ArenaInfoConfigManager : SampleConfigManager
{
	public static ArenaInfoConfigManager instance;
	public static ArenaInfoConfigManager Instance ()
	{
		if (instance == null)
			instance = new ArenaInfoConfigManager ();
		return instance;
	}

	public ArenaInfoConfigManager(){
		base.readConfig (ConfigGlobal.CONFIG_ARENA_INFO);
	}

	public override void parseSample (int sid)
	{
		AreaInfoSample sample = new AreaInfoSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

	public AreaInfoSample getSampleBySid(int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as AreaInfoSample;
	}


}

