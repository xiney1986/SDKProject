using UnityEngine;
using System.Collections;

/// <summary>
/// 成长奖励容器
/// </summary>
public class GrowupAwardContent : dynamicContent {

	public GameObject ItemPrefab;
	public GrowupAwardSample[] totals; 

	public void reLoad (GrowupAwardSample[] total,int jumpIndex) {
		totals = total;
		if(jumpIndex == totals.Length){
			jumpIndex --;
		}
		base.reLoad (totals.Length,jumpIndex);
	}
	public override void updateItem (GameObject item, int index) {
		GrowupAwardItem awardItem = item.GetComponent<GrowupAwardItem> ();
		awardItem.updateAwardItem (totals [index],index, fatherWindow);
	}
	public override void initButton (int  i) {
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, ItemPrefab);
		}
	}
}
