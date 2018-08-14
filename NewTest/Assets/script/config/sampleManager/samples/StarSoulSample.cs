using UnityEngine;
using System;
using System.Collections;

/// <summary>
///  星魂模板
/// </summary>
public class StarSoulSample:Sample {

//	/* enum */
//	/** 星魂类型 */
//	public enum StarSoulType {
//
//	}

	/** 星魂名 */
	public string name = "";
	/** 图标编号 */
	public int iconId = 0;
	/** 品质编号 */
	public int qualityId;
	/** 最高等级 */
	public int maxLevel = 0;
	/** 等级映射编号 */
	public int levelSid = 0;
	/** 被吃经验 */
	public int eatenExp = 0;
	/** 激活属性映射编号(刻印) */
	public int suitSid = 0;
	/** 卖出价格 */
	public int sell;
	/** 属性效果 */
	AttrChangeSample[] effects;
	/** 属性描述信息 */
	public string desc;
	/** 非属性基础战力 */
	public int nAttrComat;
	/** 非属性成长战力 */
	public int nDevelopComat;
	/** 星魂类型(通过此字段决定在装备列表中不能装备同一类型) */
	public int starSoulType;

	/// <summary>
	/// 获取指定类型属性效果
	/// </summary>
	/// <param name="type">类型</param>
	public AttrChangeSample getAttrChangeSampleByType(string type) {
		if (effects == null)
			return null;
		foreach (AttrChangeSample item in effects) {
			if(!item.getAttrType().Equals(type)) continue;
			return item;
		}
		return null;
	}
	/// <summary>
	/// 获取所有的属性效果
	/// </summary>
	/// <param name="type">类型</param>
	public AttrChangeSample[] getAttrChangeSample() {
		return effects;
	}

	public override void parse (int sid, string str)
	{
		//strArr[0] is sid  
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 13); 
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]);
		//strArr[3] qualityId
		this.qualityId = StringKit.toInt (strArr [3]);
		//strArr[4] maxLevel
		this.maxLevel = StringKit.toInt (strArr [4]);
		//strArr[5] levelSid
		this.levelSid = StringKit.toInt (strArr [5]);
		//strArr[6] eatenExp
		this.eatenExp = StringKit.toInt (strArr [6]);
		//strArr[7] suitSid
		this.suitSid = StringKit.toInt (strArr [7]);
		//strArr[8] sell
		this.sell = StringKit.toInt (strArr [8]);
		//strArr[9] effects
		parseEffects (strArr [9]);
		//strArr[10] desc
		this.desc = strArr [10]; 
		//strArr[11] sell
		this.nAttrComat = StringKit.toInt (strArr [11]);
		//strArr[12] sell
		this.nDevelopComat = StringKit.toInt (strArr [12]);
		//strArr[13] starSoulType
		this.starSoulType = StringKit.toInt (strArr [13]);
	}
	/** 解析属性效果 */
	private void parseEffects (string str)
	{
		//表示空
		if (str == Convert.ToString(0))
			return;
		string[] strArr = str.Split ('#');  
		effects = new AttrChangeSample[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			AttrChangeSample attr = new AttrChangeSample ();
			attr.parse (strArr [i]);
			effects [i] = attr;
		}
	}
	/** 拷贝 */
	public override void copy (object destObj)
	{
		base.copy (destObj);
		StarSoulSample dest = destObj as StarSoulSample;
		if (this.effects != null) {
			dest.effects = new AttrChangeSample[this.effects.Length];
			for (int i = 0; i < this.effects.Length; i++)
				dest.effects [i] = this.effects [i].Clone () as AttrChangeSample;
		}
	}
}
