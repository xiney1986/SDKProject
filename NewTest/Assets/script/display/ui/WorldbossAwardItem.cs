using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Worldboss奖励Item
/// </summary>
public class WorldbossAwardItem : MonoBase {
	/**奖励Item*/
	public GoodsView[] awardItems;
	/**奖励名称*/
	public UILabel     titleLabel;
	/**奖励icon*/
	public UISprite    titleSprite;
	/**奖励数据*/
	private ArenaAwardSample data;
	public UIScrollView parentScrollView;

	/// <summary>
	/// 初始奖励Item
	/// </summary>
	public void initItem (PrizeSample[] list) {
		for (int i = 0; i < awardItems.Length; i++) {
			if (i < list.Length) {
				awardItems [i].gameObject.SetActive (true);
				awardItems [i].init (list [i]);
			}
			else {
				awardItems [i].gameObject.SetActive (false);
			}
		}
	}
	/// <summary>
	/// 设置奖励名称 title
	/// </summary>
	public void setTitle (int i) {
		titleLabel.text = i.ToString ();
		//titleSprite.spriteName = 
	}
}