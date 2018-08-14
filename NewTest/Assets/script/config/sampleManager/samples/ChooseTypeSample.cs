using UnityEngine;
using System.Collections;

public class ChooseTypeSample : Sample
{

	public ChooseTypeSample (string str)
	{
		parse (str);
	}

	private int type;//类型(1技能经验，2装备经验，3游戏币，4附加属性)
	private int[] sids;//sid组合
	private string desc;//注释(无用)

	public void parse (string str)
	{
		int num = 0;
		string[] strArr = str.Split ('|');
		this.type = StringKit.toInt (strArr [num++]);	//类型
		this.sids = parseIntArray (strArr [num++]);	//sid组合
		this.desc = strArr [num++];	//注释(无用)
	}

	private int[] parseIntArray (string str)
	{
		string[] strArr = str.Split (',');
		int[] array = new int[strArr.Length];
		
		for (int i = 0; i < strArr.Length; i++) {
			array [i] = StringKit.toInt (strArr [i]);
		}
		return array;
	}

	/** 获得类型 */
	public int getType ()
	{
		return type;
	}

	/** 获得sid组合 */
	public int[] getSids ()
	{
		return sids;
	}

	/** 获得注释 */
	public string getDesc ()
	{
		return desc;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		ChooseTypeSample dest = destObj as ChooseTypeSample;
		if (this.sids != null) {
			dest.sids = new int[this.sids.Length];
			for (int i = 0; i < this.sids.Length; i++)
				dest.sids [i] = this.sids [i];
		}
	}
}
