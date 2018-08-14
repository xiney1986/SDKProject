using UnityEngine;
using System;
using System.Collections;

/// <summary>
///  装备升星模板   
/// </summary>
public class EquipStarAttrSample:Sample {
	/** 附加属性 */
	public AttrChangeSample[] attributes = null;
	/*  附加属性信息*/
	public string[] equipStarAtr;
	/// <summary>
	/// 获取指定类型属性效果
	/// </summary>
	/// <param name="type">类型</param>
	public AttrChangeSample getAttrChangeSampleByType(string type) {
		if (attributes == null)
			return null;
		foreach (AttrChangeSample item in attributes) {
			if(!item.getAttrType().Equals(type)) continue;
			return item;
		}
		return null;
	}
	/// <summary>
	/// 获取所有的属性效果
	/// </summary>
	/// <param name="type">类型</param>
	public AttrChangeSample[] getAttrChangeSample(int starState)  {
		parseEffects (equipStarAtr [starState - 1]);
		return attributes;
	}
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 1);
		//strArr[1] effects
		parseEffectsInfo (strArr [1]);

	}
	/** 解析属性效果 */
	private void parseEffectsInfo (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		equipStarAtr = str.Split ('$');  
	}
	/** 解析属性效果 */
	private void parseEffects (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		attributes = new AttrChangeSample[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			AttrChangeSample attr = new AttrChangeSample ();
			attr.parse (strArr [i]);
			attributes [i] = attr;
		}
	}

	/** 拷贝 */
	public override void copy (object destObj)
	{
		base.copy (destObj);
		EquipStarAttrSample dest = destObj as EquipStarAttrSample;
		if (this.attributes != null) {
			dest.attributes = new AttrChangeSample[this.attributes.Length];
			for (int i = 0; i < this.attributes.Length; i++)
				dest.attributes [i] = this.attributes [i].Clone () as AttrChangeSample;
		}
	}
}
