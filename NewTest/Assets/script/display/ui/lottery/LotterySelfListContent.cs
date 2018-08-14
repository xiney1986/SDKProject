using UnityEngine;
using System.Collections.Generic;

public class LotterySelfListContent  : dynamicContent
{

	public GameObject tmp;

	public void reLoad()
	{
		base.reLoad (LotteryManagement.Instance.playerLotteryList.Count);
	}

	public override void updateItem (GameObject item, int index)
	{
		LotterySelfListItem selfItem = item.GetComponent<LotterySelfListItem> ();
		selfItem.updateSelfItem(LotteryManagement.Instance.playerLotteryList[index]);
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, tmp);
		}
	}
}
