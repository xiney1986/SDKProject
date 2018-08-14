using UnityEngine;
using System.Collections;

public class MiningSample : Sample {
	public int  sid;//奖品
	public int  time;//耗时
	public int type;//活动类型
	public int size;//活动规模
	public float outputRate;//产出
	public string background;


	public override  void parse (int sid,string str)
	{
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 5);
		this.sid = StringKit.toInt(strArr[0]);
		time = StringKit.toInt(strArr[1]);
		type = StringKit.toInt(strArr[2]);
		size = StringKit.toInt(strArr[3]);
		outputRate =float.Parse(strArr[4]);

		background =strArr[5];
	}
	
	public override void copy (object destObj)
	{
		base.copy (destObj);
		GrowupAwardSample dest = destObj as GrowupAwardSample;
		
	}
}
