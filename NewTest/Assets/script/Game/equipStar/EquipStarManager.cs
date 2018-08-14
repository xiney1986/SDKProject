using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 装备升星对象管理器
/// </summary>
public class EquipStarManager {
		/* static methods */
	public static EquipStarManager Instance {
		get{return SingleManager.Instance.getObj("EquipStarManager") as EquipStarManager;}
	}

	///<summary>
	/// 获得制定类型的加成值
	/// </summary>

}