using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LotteryAwardContent : dynamicContent 
{
	public GameObject awardTmp;// 奖励tmp//
	WindowBase farWin;
	public void initContent(WindowBase farWin)
	{
		this.farWin = farWin;
		base.reLoad(LotterySelectPrizeConfigManager.Instance.prizes.Count,getAwardCanReciveIndex());
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, awardTmp);
		}
	}

	public override void updateItem (GameObject item, int index)
	{
		LotteryAwardItem mItem = item.GetComponent<LotteryAwardItem>();
		mItem.updateItem(LotterySelectPrizeConfigManager.Instance.prizes[index],farWin);
	}
	public void destroyAward()
	{
		for(int i=0;i<gameObject.transform.childCount;i++)
		{
			Destroy(gameObject.transform.GetChild(i).gameObject);
		}
	}
	int getAwardCanReciveIndex()
	{
		int index = 0;
		for(int i=0;i<LotterySelectPrizeConfigManager.Instance.prizes.Count;i++)
		{
			if(LotterySelectPrizeConfigManager.Instance.prizes[i].state == LotterySelectPrizeState.CanRecive)
				return i;
		}
		return index;
	}
}
