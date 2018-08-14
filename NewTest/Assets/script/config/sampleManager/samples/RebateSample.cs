using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RebateSample : Sample 
{
	public static int DIAMOND_TYPE = 0;
	public static int GOLD_TYPE = 1;

	public RebateSample()
	{

	}

	public int id;
	public int type;// 消耗类型0钻石1金币//
	public int compareVal;// 对比的数值//
	public int lessRate;// 小于等于的反利率//
	public int moreRate;// 大于的反利率//

	public override void parse (int sid, string str)
	{
		this.id = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 4);
		this.type = StringKit.toInt (strArr[1]);
		this.compareVal = StringKit.toInt (strArr[2]);
		this.lessRate = StringKit.toInt (strArr[3]);
		this.moreRate = StringKit.toInt (strArr[4]);
	}


}
