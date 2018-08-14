using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RebateSampleManager : SampleConfigManager {


	//单例
	private static RebateSampleManager _Instance ;
	
	public RebateSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_REBATE);
	}
	
	public static RebateSampleManager Instance {
		get {
			if (_Instance == null) {
				_Instance = new RebateSampleManager ();
				return _Instance;
			}
			return _Instance;
		}
		set {
			_Instance = value;
		}
	}

	public override void parseSample (int sid)
	{
		RebateSample rs = new RebateSample();
		string dataStr = getSampleDataBySid (sid); 
		rs.parse (sid, dataStr);
		samples.Add(sid,rs);
	}

	public List<int> getAllIDs()
	{
		lock (this) {
			List<int> list = new List<int> ();
			foreach (int key in data.Keys) { 
				list.Add (key);
			}
			return list;
		}
	}

	public List<RebateSample> getDiamondSampleByIDs(List<int> ids)
	{
		List<RebateSample> rs = new List<RebateSample>();
		RebateSample sample;
		for (int i = 0; i < ids.Count; i++)
		{
			createSample(ids[i]);
			sample = samples[ids[i]] as RebateSample;
			if(sample.type == RebateSample.DIAMOND_TYPE)
			{
				rs.Add(sample);
			}
		}
		return rs;
	}

	public List<RebateSample> getGoldSampleByIDs(List<int> ids)
	{
		List<RebateSample> rs = new List<RebateSample>();
		RebateSample sample;
		for (int i = 0; i < ids.Count; i++)
		{
			createSample(ids[i]);
			sample = samples[ids[i]] as RebateSample;
			if(sample.type == RebateSample.GOLD_TYPE)
			{
				rs.Add(sample);
			}
		}
		return rs;
	}
	
}
