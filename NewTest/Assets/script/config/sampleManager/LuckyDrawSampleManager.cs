using System;
using System.Collections.Generic;
 
/**
 * 抽奖模板管理器
 * @author longlingquan
 * */
public class LuckyDrawSampleManager:SampleConfigManager
{
	private static LuckyDrawSampleManager _Instance;
	private static bool _singleton = true;

	public static LuckyDrawSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new LuckyDrawSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public LuckyDrawSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_LUCKY); 
	}
	
	//获得抽奖模板对象
	public LuckyDrawSample getLuckyDrawSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as LuckyDrawSample;
	}  
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		LuckyDrawSample sample = new LuckyDrawSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//得到指定商店所有商品sid
	public int[] getAllLuckyDarwIds ()
	{ 
		List<int> list = new List<int> (); 
		foreach (int key in data.Keys) {
			 
			list.Add ((int)key); 
		}
		return list.ToArray ();
	}
}  