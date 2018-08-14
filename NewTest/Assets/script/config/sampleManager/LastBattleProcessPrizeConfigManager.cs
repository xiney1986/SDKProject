using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastBattleProcessPrizeConfigManager : SampleConfigManager
{
	public List<LastBattleProcessPrizeSample> processPrize;
	public LastBattleProcessPrizeConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LASTBATTLEPROCESSPRIZE);
	}

	public static LastBattleProcessPrizeConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("LastBattleProcessPrizeConfigManager") as LastBattleProcessPrizeConfigManager;}
	}

	public override void parseConfig (string str)
	{
		LastBattleProcessPrizeSample prize = new LastBattleProcessPrizeSample (str);
		if (processPrize == null)
			processPrize = new List<LastBattleProcessPrizeSample> ();
		processPrize.Add (prize);
	}
	public LastBattleProcessPrizeSample getPrize(int id)
	{
		for(int i=0;i<processPrize.Count;i++)
		{
			if(id == processPrize[i].id)
			{
				return processPrize[i];
			}
		}
		return null;
	}
}
