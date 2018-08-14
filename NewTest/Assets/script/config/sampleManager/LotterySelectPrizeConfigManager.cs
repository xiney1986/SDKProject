using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LotterySelectPrizeConfigManager : SampleConfigManager
{
	public List<LotterySelectPrizeSample> prizes;
	public LotterySelectPrizeConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LOTTERYSELECTPRIZE);
	}

	public static LotterySelectPrizeConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("LotterySelectPrizeConfigManager") as LotterySelectPrizeConfigManager;}
	}

	public override void parseConfig (string str)
	{
		LotterySelectPrizeSample prize = new LotterySelectPrizeSample (str);
		if (prizes == null)
			prizes = new List<LotterySelectPrizeSample> ();
		prizes.Add (prize);
	}
	public LotterySelectPrizeSample getPrize(int id)
	{
		for(int i=0;i<prizes.Count;i++)
		{
			if(id == prizes[i].id)
			{
				return prizes[i];
			}
		}
		return null;
	}
}
