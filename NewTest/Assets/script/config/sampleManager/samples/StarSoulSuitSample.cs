using System;
using System.Collections;

/// <summary>
/// 星魂刻印模板
/// </summary>
public class StarSoulSuitSample : Sample {


	/* fields */
	/** 刻印名 */
	public string name = "";
	/** 部件sids(可配置相同sid) */
	public int[] parts;
	/** 效果 */
	AttrChangeSample[] effects;
	/** 描述信息 */
	public string desc;

	/* methods */
	public StarSoulSuitSample () {
	}

	/** 解析 */
	public override void parse (int sid, string str) {
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 4); 
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] parts
		parseParts (strArr [2]);
		//strArr[3] attrChange
		parseEffects (strArr [3]);
		//strArr[4] desc
		this.desc = strArr [4]; 
	}

	/** 解析部件 */
	private void parseParts (string str)
	{
		string[] strArr = str.Split (',');
		parts = new int[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			parts [i] = StringKit.toInt (strArr [i]);
		}
	}
	
	/** 解析属性效果 */
	private void parseEffects (string str) {
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

	/** 克隆 */
	public override void copy (object destObj)
	{
		base.copy (destObj);
		StarSoulSuitSample dest = destObj as StarSoulSuitSample;
		if (this.parts != null) {
			dest.parts = new int[this.parts.Length];
			for (int i = 0; i < this.parts.Length; i++)
				dest.parts [i] = this.parts [i];
		}
		if (this.effects != null) {
			dest.effects = new AttrChangeSample[this.effects.Length];
			for (int i = 0; i < this.effects.Length; i++)
				dest.effects [i] = this.effects [i].Clone () as AttrChangeSample;
		}
	}
} 
