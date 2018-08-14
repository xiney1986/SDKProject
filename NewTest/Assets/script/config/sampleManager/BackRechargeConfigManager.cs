using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BackRechargeConfigManager : SampleConfigManager
{
	private static BackRechargeConfigManager instance;

	public BackRechargeConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_BACKRECHARGE);
	}

	public static BackRechargeConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("BackRechargeConfigManager") as BackRechargeConfigManager;}
	}

	//解析模板数据
	public override void parseSample (int sid)
	{
		RechargeSample sample = new RechargeSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

	public List<RechargeSample> getRechargeList()
	{
		RechargeSample rs;
		List<RechargeSample> mList = new List<RechargeSample>();
		List<int> list = new List<int> ();
		foreach (int key in data.Keys) { 
			list.Add (key);
		}

		for(int i=0;i<list.Count;i++)
		{
			createSample(list[i]);
			rs = samples[list[i]] as RechargeSample;
			mList.Add(rs);
		}

		return mList;
	}
	public RechargeSample getRechargeSampleByID(int id)
	{
		for(int i=0;i<getRechargeList().Count;i++)
		{
			if(getRechargeList()[i].sid == id)
			{
				return getRechargeList()[i];
			}
		}
		return null;
	}
}
