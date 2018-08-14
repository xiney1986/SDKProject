using UnityEngine;
using System.Collections;

public class GuildDonateSample : Sample
{
	public string description; //描述
	public int maxTimesOneDay; //每天捐献次数限制
	public int[] consume; //消耗
	public int[] activity; //活跃度
	public int[] dedication;//贡献值


	public override void parse (int sid, string str)
	{
		this.sid = sid;  
		int pos = 1;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 5);
		description = strArr [pos++];
		maxTimesOneDay = StringKit.toInt (strArr [pos++]);
		consume = parseIntArray (strArr [pos++]);
		activity = parseIntArray (strArr [pos++]);
		dedication = parseIntArray (strArr [pos++]);
	}

	private int[] parseIntArray (string str)
	{
		string[] strs = str.Split (',');
		int[] result = new int[strs.Length];
		for (int i = 0; i < strs.Length; i++) {
			result [i] = StringKit.toInt (strs [i]);
		}
		return result;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		GuildDonateSample dest = destObj as GuildDonateSample;
		if (this.consume != null) {
			dest.consume = new int[this.consume.Length];
			for (int i = 0; i < this.consume.Length; i++)
				dest.consume [i] = this.consume [i];
		}
		if (this.activity != null) {
			dest.activity = new int[this.activity.Length];
			for (int i = 0; i < this.activity.Length; i++)
				dest.activity [i] = this.activity [i];
		}
		if (this.dedication != null) {
			dest.dedication = new int[this.dedication.Length];
			for (int i = 0; i < this.dedication.Length; i++)
				dest.dedication [i] = this.dedication [i];
		}
	}
}
