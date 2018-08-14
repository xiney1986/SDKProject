using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/**超级抽奖配置模板管理器
  *负责超级抽奖信息的初始化 
  *@author gc
  **/
public class SuperDrawSampleManager : SampleConfigManager
{
	//单例
	private static SuperDrawSampleManager instance;

	public SuperDrawSampleManager ()
	{

		base.readConfig (ConfigGlobal.CONFIG_SUPERDRAW);
	}
	
	public static SuperDrawSampleManager Instance {
		get{
			if(instance==null)
				instance=new SuperDrawSampleManager();
			return instance;
		}
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		SuperDrawSample sample = new SuperDrawSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得星星抽奖模板对象
	public SuperDrawSample getSuperDrawSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as SuperDrawSample;
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
