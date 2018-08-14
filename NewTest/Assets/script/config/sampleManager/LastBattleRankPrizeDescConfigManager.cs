using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastBattleRankPrizeDescConfigManager : SampleConfigManager
{
	public List<LastBattleRankPrizeDesc> descList;

	public LastBattleRankPrizeDescConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LASTBATTLERANKPRIZEDESC);
	}

	public static LastBattleRankPrizeDescConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("LastBattleRankPrizeDescConfigManager") as LastBattleRankPrizeDescConfigManager;}
	}

	public override void parseConfig (string str)
	{
		LastBattleRankPrizeDesc desc = new LastBattleRankPrizeDesc();
		string[] strArr = str.Split ('|');
		desc.rankName = strArr [1];
		desc.prizeDesc = strArr[2];

		if(descList == null)
			descList = new List<LastBattleRankPrizeDesc>();
		descList.Add(desc);
	}

}

public class LastBattleRankPrizeDesc
{
	public string rankName;// 排民//
	public string prizeDesc;// 排名奖励//
}
