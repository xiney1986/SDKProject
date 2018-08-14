using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**充值模板管理器
  *负责充值模板信息的初始化 
  *@author 汤琦
  **/
public class RechargeSampleManager : SampleConfigManager
{
	//单例
	private static RechargeSampleManager _Instance;
	private static bool _singleton = true;
	
	public RechargeSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_RECHARGE);
	}
	
	public static RechargeSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new RechargeSampleManager ();
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
		RechargeSample sample = new RechargeSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得充值模板对象
	public RechargeSample getRechargeSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as RechargeSample;
	}
	
	//获得指定限制的所有充值sid
	public int[] getAllRecharge ()
	{
		List<int> list = new List<int> ();
		foreach (int key in data.Keys) { 
			list.Add (key);
		}
		return list.ToArray ();
	}

	//需要修改 data samples 如果存在就覆盖，不存在添加
	public void updataSample (int sid, string dataStr)
	{
		if (data.ContainsKey (sid)) { // 存在
			data [sid] = dataStr;
			if (samples [sid] != null)//若样本已经创建，需要修改
				(samples [sid] as RechargeSample).parse (sid, dataStr);
		} else {
			data.Add (sid, dataStr);
		}
	}
}
