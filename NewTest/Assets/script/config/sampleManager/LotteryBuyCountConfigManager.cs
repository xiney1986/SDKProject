using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LotteryBuyCountConfigManager : SampleConfigManager
{
	List<LotteryBuyCountSample> countSample;
	public LotteryBuyCountConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LOTTERYBUYCOUNT);
	}

	public static LotteryBuyCountConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("LotteryBuyCountConfigManager") as LotteryBuyCountConfigManager;}
	}

	public override void parseConfig (string str)
	{
		LotteryBuyCountSample sample = new LotteryBuyCountSample (str);
		if (countSample == null)
			countSample = new List<LotteryBuyCountSample> ();
		countSample.Add (sample);
	}
	public LotteryBuyCountSample getCountSample(int vipLv)
	{
		for(int i=0;i<countSample.Count;i++)
		{
			if(vipLv == countSample[i].vipLv)
			{
				return countSample[i];
			}
		}
		return null;
	}
	public List<LotteryBuyCountSample> getSample()
	{
		return countSample;
	}
}
