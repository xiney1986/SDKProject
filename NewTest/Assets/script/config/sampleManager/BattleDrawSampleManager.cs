using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/**星星抽奖配置模板管理器
  *负责星星抽奖信息的初始化 
  *@author 汤琦
  **/
public class BattleDrawSampleManager : SampleConfigManager
{
	//单例
	private static BattleDrawSampleManager instance;

	public BattleDrawSampleManager ()
	{

		base.readConfig (ConfigGlobal.CONFIG_BATTLEDRAW);
	}
	
	public static BattleDrawSampleManager Instance {
		get{
			if(instance==null)
				instance=new BattleDrawSampleManager();
			return instance;
		}
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		BattleDrawSample sample = new BattleDrawSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得星星抽奖模板对象
	public BattleDrawSample getBattleDrawSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as BattleDrawSample;
	} 

	//获得所有星星抽奖sid
	public int[] getAllDraw()
	{
		List<int> list = new List<int>();
		foreach (int key in data.Keys) { 
			list.Add (key);
		}
		return list.ToArray();
	}
}
