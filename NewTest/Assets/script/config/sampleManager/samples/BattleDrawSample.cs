using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 星星抽奖配置模板
 * @author 汤琦
 * */
public class BattleDrawSample : Sample
{
	public List<PrizeSample> list;

	public BattleDrawSample ()
	{
	}

	public override void parse (int sid, string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		parseContent (strArr [1]);
	}

	private void parseContent (string str)
	{
		string[] strs = str.Split ('#');
		for (int i = 0; i < strs.Length; i++) {

			string[] strss = strs [i].Split (',');
			int type = StringKit.toInt (strss [0]);
			int sid = StringKit.toInt (strss [1]);
			int num = StringKit.toInt (strss [2]);
			PrizeSample prize = new PrizeSample (type, sid, num);
			if (list == null)
				list = new List<PrizeSample> ();
			list.Add (prize);
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		BattleDrawSample dest = destObj as BattleDrawSample;
		if (this.list != null) {
			dest.list = new List<PrizeSample> ();
			for (int i = 0; i < this.list.Count; i++)
				dest.list.Add (this.list [i].Clone () as PrizeSample);
		}
	}
}
