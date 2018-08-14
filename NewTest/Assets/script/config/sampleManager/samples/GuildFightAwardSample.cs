using UnityEngine;
using System.Collections;

public class GuildFightAwardSample : Sample {
	/** 名字 */
	public string name;
	/** 宝箱类型 */
	public string type;
	/** 奖励 */
	public PrizeSample[] prizes;
	override public void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		this.name = strArr [1];
		this.type = strArr [2];
		string[] strs = strArr [3].Split ('#');
		prizes = new PrizeSample[strs.Length];
		for (int i = 0; i < prizes.Length; i++) {
			prizes [i] = parsePrize (strs [i]);
		}
	}
	
	PrizeSample parsePrize (string str)
	{
		string[] strs = str.Split (',');
		PrizeSample sample = new PrizeSample ();
		sample.type = StringKit.toInt (strs [0]);
		sample.pSid = StringKit.toInt (strs [1]);
		sample.num = strs [2];
		return sample;
	}
	
	public override void copy (object destObj)
	{
		base.copy (destObj);
		ArenaAwardSample dest = destObj as ArenaAwardSample;
		if (this.prizes != null) {
			dest.prizes = new PrizeSample[this.prizes.Length];
			for (int i = 0; i < dest.prizes.Length; i++)
				dest.prizes [i] = this.prizes [i].Clone () as PrizeSample;
		}
	}
}
