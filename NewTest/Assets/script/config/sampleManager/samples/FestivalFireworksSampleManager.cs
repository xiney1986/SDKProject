using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 节日礼花模版管理
/// </summary>
public class FestivalFireworksSampleManager: SampleConfigManager {

	//单例
	private static FestivalFireworksSampleManager instance;
	
	public FestivalFireworksSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_FESTIVALFIREWORKS);
	}
	
	public static FestivalFireworksSampleManager Instance {
		get{
			if(instance==null)
				instance=new FestivalFireworksSampleManager();
			return instance;
		}
	} 	
	
	public FestivalFireworksSample getFestivalFireworksSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as FestivalFireworksSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		FestivalFireworksSample sample = new FestivalFireworksSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	public List<FestivalFireworksSample> getAllSamples()
	{
		List<FestivalFireworksSample> list = new List<FestivalFireworksSample>();
		List<int> _list = new List<int>();
		_list = getAllSampleSid();
//		foreach(int i in _list)
//			list.Add (getFestivalFireworksSampleBySid(i));
//		return list;
		for(int i=0;i<_list.Count;i++)
		{
			list.Add(getFestivalFireworksSampleBySid(_list[i]));
		}
		return list;
	}
	public List<int> getAllSampleSid()
	{
		List<int> list= new List<int>();
		foreach(int sid in data.Keys)
		{
			list.Add(sid);
		}
		return list;

	}
}

