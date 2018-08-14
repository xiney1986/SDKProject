using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 星魂显示节点容器
/// </summary>
public class StarSoulItemContent : dynamicContent {

	/* fields */
	/**所用每一条显示的Perfab */
	public GameObject startSoulButtonPerfab;
	/** 状态类型 */
	private ButtonStoreStarSoul.ButtonStateType intoType;
	/** 星魂列表 */
	ArrayList starSouls;
	bool isAutoSelect=false;

	/* methods */
	/// <summary>
	/// 重新读取玩家身上的星魂
	/// </summary>
	public void reLoad (ArrayList _starSouls, ButtonStoreStarSoul.ButtonStateType intoType) {
		starSouls = _starSouls;
		this.intoType = intoType;
		isAutoSelect=false;
		base.reLoad (starSouls.Count);
	}
	/// <summary>
	/// 重新读取玩家身上的星魂
	/// </summary>
	public void reLoad (ArrayList _starSouls) {
		starSouls = _starSouls;
		isAutoSelect=true;
		base.reLoad (starSouls.Count);
	}

	/// <summary>
	/// 开始更新星魂仓库的每一条数据
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="index">Index.</param>
	public override void updateItem (GameObject item, int index) {
		ButtonStoreStarSoul button = item.GetComponent<ButtonStoreStarSoul> ();
		StarSoul startsol=starSouls[index] as StarSoul;
        button.UpdateSoul(startsol, intoType, isAutoSelect);
	}
	/***/
	public override void initButton(int i) {
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject,startSoulButtonPerfab);
		}
		ButtonStoreStarSoul button = nodeList [i].GetComponent<ButtonStoreStarSoul> ();
		button.fatherWindow = fatherWindow;
	}

	/* properties */
	/** 设置初始类型 */
	public void setIntoType(ButtonStoreStarSoul.ButtonStateType intoType) {
		this.intoType = intoType;
	}
}
