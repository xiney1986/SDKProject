using UnityEngine;
using System.Collections;

public class RankingActiveContent : dynamicContent {

	public GameObject ItemPrefab;
	public RankAward[] totals;
	public void reLoad(RankAward[] total){
		totals=total;
		base.reLoad(totals.Length);

	}
	public override void updateItem (GameObject item, int index)
	{
		LuckActivityAwardItem awardItem = item.GetComponent<LuckActivityAwardItem> ();
		awardItem.updateAwardItem(totals [index],fatherWindow);
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, ItemPrefab);
		}
	}

}
