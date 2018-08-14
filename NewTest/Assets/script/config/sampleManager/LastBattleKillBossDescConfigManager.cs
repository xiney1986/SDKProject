using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastBattleKillBossDescConfigManager : SampleConfigManager
{
	public List<LastBattleKillBossDesc> descList;

	public LastBattleKillBossDescConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LASTBATTLEKILLBOSSPRIZEDESC);
	}

	public static LastBattleKillBossDescConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("LastBattleKillBossDescConfigManager") as LastBattleKillBossDescConfigManager;}
	}

	public override void parseConfig (string str)
	{
		LastBattleKillBossDesc desc = new LastBattleKillBossDesc();
		string[] strArr = str.Split ('|');
		desc.killBossCount = StringKit.toInt(strArr [1]);
		desc.tittle = strArr[2];
		desc.prizeDesc = strArr[3].Split ('#');

		if(descList == null)
			descList = new List<LastBattleKillBossDesc>();
		descList.Add(desc);
	}
	// 通过击杀boss个数得到获得奖励的个数//
	public int getPrizeCountByKillBossCount(int bossCount)
	{
		int count = 0;
		if(bossCount != 0)
		{
			if(descList != null && descList.Count > 0)
			{
				for(int i=0;i<descList.Count;i++)
				{
					if(bossCount == descList[i].killBossCount)
					{
						return count++;
					}
					if(bossCount > descList[i].killBossCount)
					{
						count++;
					}
				}
			}
		}

		return count;
	}

}

public class LastBattleKillBossDesc
{
	public string tittle;
	public int killBossCount;// 刺杀boss个数//
	public string[] prizeDesc;// 奖励描述// 
}
