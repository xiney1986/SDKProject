using UnityEngine;
using System.Collections;

/// <summary>
/// 成长奖励返现信息容器
/// </summary>
public class GrowupRebateContent : dynamicContent {

	public GameObject ItemPrefab;
	public GrowupAwardSample[] totals;

	public void reLoad (GrowupAwardSample[] total) {
		totals = total;
		MonoBase.print ("totals length:" + totals.Length);
		base.reLoad (totals.Length);
	}
	public override void updateItem (GameObject item, int index) {
		GrowupRebateItem awardItem = item.GetComponent<GrowupRebateItem> ();
		awardItem.updateAwardItem (totals [index], fatherWindow);
	}
	public override void initButton (int  i) {
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, ItemPrefab);
		}
	}
}
