using UnityEngine;
using System.Collections;

public class MineralEnemyContent : dynamicContent {

	public GameObject ItemPrefab;
	public PillageEnemyInfo[] totals; 
	
	public void reLoad (PillageEnemyInfo[] total) {
		totals = total;
		base.reLoad (totals.Length);
	}
	public override void updateItem (GameObject item, int index) {
		MineralEnemyItem enemyItem = item.GetComponent<MineralEnemyItem> ();
		enemyItem.updateItem (totals [index],index, fatherWindow);
	}
	public override void initButton (int  i) {
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, ItemPrefab);
		}
	}
}
