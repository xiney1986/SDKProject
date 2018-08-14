using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 分享抽奖配置模板
 * @author 
 * */
public class ShareDrawSample : Sample
{
	public List<PrizeSample> list;
    public List<int> prizeSidList;

    public ShareDrawSample()
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
			int type = StringKit.toInt (strss [1]);
			int sid = StringKit.toInt (strss [2]);
			int num = StringKit.toInt (strss [3]);
			PrizeSample prize = new PrizeSample (type, sid, num);
            int pSid = StringKit.toInt(strss[0]);
			if (list == null)
				list = new List<PrizeSample> ();
            if (prizeSidList == null)
                prizeSidList = new List<int>();
            prizeSidList.Add(pSid);
			list.Add (prize);
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		SuperDrawSample dest = destObj as SuperDrawSample;
		if (this.list != null) {
			dest.list = new List<PrizeSample> ();
			for (int i = 0; i < this.list.Count; i++)
				dest.list.Add (this.list [i].Clone () as PrizeSample);
		}
	}
}
