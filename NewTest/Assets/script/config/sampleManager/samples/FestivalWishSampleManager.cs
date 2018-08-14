using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 节日许愿模版管理
/// </summary>
public class FestivalWishSampleManager: SampleConfigManager {

	//单例
	private static FestivalWishSampleManager instance;
	
	public FestivalWishSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_FESTIVALWISH);
	}
	
	public static FestivalWishSampleManager Instance {
		get{
			if(instance==null)
				instance=new FestivalWishSampleManager();
			return instance;
		}
	} 	
	
	public FestivalWishSample getFestivalWishSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as FestivalWishSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		FestivalWishSample sample = new FestivalWishSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	//得到所有的节日许愿模版信息
	public List<FestivalWish> getAllFestivalWishSample()
	{
		List<FestivalWish> list = new List<FestivalWish>();
		foreach(int sid in data.Keys)
		{
			list.Add(new FestivalWish(sid));
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

