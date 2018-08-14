using UnityEngine;
using System.Collections;

/// <summary>
/// 限时积分活动容器
/// </summary>
public class SourceActiveContent : dynamicContent {

	public GameObject ItemPrefab;
	public RankAward[] totals;
	private int mySource;
	public void reLoad(RankAward[] total,int source){
		mySource=source;
		totals=total;
		base.reLoad(totals.Length);
	}
	/** 更新条目 */
	public override void updateItem (GameObject item, int index) {
		LuckActivityAwardItem awardItem = item.GetComponent<LuckActivityAwardItem> ();
		awardItem.updateAwardItem(totals [index],fatherWindow,mySource);
	}
	/** 初始化button */
	public override void initButton (int  i) {
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, ItemPrefab);
		}
	}
}
