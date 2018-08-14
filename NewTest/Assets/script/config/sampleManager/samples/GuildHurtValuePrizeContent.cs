using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GuildHurtValuePrizeContent : MonoBase
{
	//===================
	public GameObject content;
	public GameObject itemprefab;
	//公会当前总伤害
	long currentHurtSum;
	long bossSum;
	//标记是否伤害小于最小挡
	long _bossSum;
	
	List<GuildBossPrizeSample> prizes;

	public void initContent()
	{
		if(prizes==null)
			prizes = new List<GuildBossPrizeSample>();
		prizes = GuildPrizeSampleManager.Instance.getPrizes();
		init ();
		Utils.RemoveAllChild (content.transform);
		for( int i=0;i< prizes.Count;i++)
		{
			GuildHurtValuePrizeItem item = NGUITools.AddChild(content.gameObject,itemprefab).GetComponent<GuildHurtValuePrizeItem>();
			item.transform.localPosition = new Vector3(0,-60*i,0);
			if(_bossSum==1)
				item.init(prizes[i],_bossSum);
			else
				item.init(prizes[i],bossSum);
			
		}
	}
	public void init(){

		currentHurtSum = GuildManagerment.Instance.getGuildAltar ().hurtSum;
		bossSum = getBossPrize (currentHurtSum).hurt;
	}

	public GuildBossPrizeSample getBossPrize (long currentHurtSum) {
		List<GuildBossPrizeSample> bossPrizes = GuildPrizeSampleManager.Instance.getPrizes ();
		for(int i=0;i<bossPrizes.Count;i++)
		{
			if(currentHurtSum < bossPrizes[i].hurt)
			{
				if(i==0)
					_bossSum = 1;
				return bossPrizes[i-1<0?0:i-1];
			}
		}
		return bossPrizes[bossPrizes.Count - 1];
	}
}