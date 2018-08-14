using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**PVP奖励模板   
 * 详见配置说明文件
 *@author 汤琦
 **/
public class PvpPrizeSample : Sample
{
	public PvpPrizeSample (string str)
	{
		parse (str);
	}

	public string des;//描述
	public int tapCount;//分页
	public List<PrizeSample> item = new List<PrizeSample> ();//奖励
	
	
	private void parse (string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 3);
		//strArr[1] tapCount
		this.tapCount = StringKit.toInt (strArr [1]);
		//strArr[2] item
		parseContent (strArr [2]);
		//strArr[3] tapCount
		this.des = strArr [3];
	}
	
	private void parseContent (string str)
	{
		if (str.Contains ("#")) {
			string[] strs = str.Split ('#');
			for (int i = 0; i < strs.Length; i++) {
				PrizeSample sample = new PrizeSample (strs [i], ',');
				item.Add (sample);
			}
		} else {
			item.Add (new PrizeSample (str, ','));
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		PvpPrizeSample dest = destObj as PvpPrizeSample;
		if (this.item != null) {
			dest.item = new List<PrizeSample> ();
			for (int i = 0; i < this.item.Count; i++)
				dest.item.Add (this.item [i].Clone () as PrizeSample);
		}
	}
}
