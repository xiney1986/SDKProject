using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**诸神之战奖励模板
 **/
public class GodsWarPrizeSample : Sample
{
	public GodsWarPrizeSample (string str)
	{
		parse (str);
	}

	public string des;//描述
	public int type;//奖励类型(1:终极奖励 2:连胜奖励 3:最大连胜奖励 4:积分奖励)
	public List<PrizeSample> item = new List<PrizeSample> ();//奖励
	public int integral = 0;//领取奖励需要达到的积分
	public int costRMB;//双倍领取积分奖励消耗的钻石
	
	private void parse (string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		this.type = StringKit.toInt (strArr [1]);
		parsePrize (strArr [2]);
		this.des = strArr [3];
		if(strArr.Length>4)
			this.integral = StringKit.toInt(strArr[4]);
		if(strArr.Length>5)
			this.costRMB  = StringKit.toInt(strArr[5]);
	}
	
	private void parsePrize (string str)
	{
		if (str.Contains ("#")) {
			string[] strs = str.Split ('#');
			for (int i = 0; i < strs.Length; i++)
			{
			    string[] sts = strs[i].Split(',');
                PrizeSample sample = new PrizeSample(StringKit.toInt(sts[0]), StringKit.toInt(sts[1]),sts[2]);
				item.Add (sample);
			}
		} else {
			item.Add (new PrizeSample (str, ','));
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		GodsWarPrizeSample dest = destObj as GodsWarPrizeSample;
		if (this.item != null) {
			dest.item = new List<PrizeSample> ();
			for (int i = 0; i < this.item.Count; i++)
				dest.item.Add (this.item [i].Clone () as PrizeSample);
		}
	}
}
