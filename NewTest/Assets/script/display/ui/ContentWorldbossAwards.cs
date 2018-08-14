using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentWorldbossAwards : dynamicContent {
	/**奖励链表*/
	protected List<ArenaAwardSample> awards;
	private UIScrollView scrollView;
	/// <summary>
	/// 获得奖励信息
	/// </summary>
	public void init () {
		scrollView = GetComponent<UIScrollView> ();
		awards = WorldBossManagerment.Instance.getAward ();
		//逆置奖励链表
		awards.Reverse ();
		if (awards != null)
			base.reLoad (awards.Count);
	}
	/// <summary>
	/// 初始化奖励内容
	/// </summary>
	public override void initButton (int i) {
		nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as WorldbossAwardWindow).prefabAwardItem);
		nodeList [i].SetActive (true);
		nodeList [i].name = StringKit.intToFixString (i + 1);
		WorldbossAwardItem button = nodeList [i].GetComponent<WorldbossAwardItem> ();
		button.parentScrollView = scrollView;
		button.initItem (awards [i].prizes);
		button.setTitle (i + 1);
	
	}
}
