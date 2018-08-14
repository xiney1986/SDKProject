
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 签到奖励配置模板
 * @author gc
 * */
public class SignInSample : Sample
{
	public List<PrizeSample> list;
    public PrizeSample allSignPrize;
    public int allSignSid = 0;
    public List<int> daySids;
    public List<int> types;

    public SignInSample(string str)
	{
        parse(str);
	}

	public void parse (string str)
	{
        string[] strArr = str.Split('|');
        this.sid = StringKit.toInt(strArr[0]);
		parseContent (strArr [1]);
        parseAllSignAward(strArr[2]);
	}

	private void parseContent (string str)
	{
		string[] strs = str.Split ('#');
        for (int i = 0; i < strs.Length; i++) {
            string[] strss = strs[i].Split(',');
            int type = StringKit.toInt(strss[0]);
            int sid = StringKit.toInt(strss[1]);
            int num = StringKit.toInt(strss[2]);
			PrizeSample prize = new PrizeSample (type, sid, num);
			if (list == null)
				list = new List<PrizeSample> ();
			list.Add (prize);
            if (daySids == null)
                daySids = new List<int>();
            daySids.Add(StringKit.toInt(strss[3]));
            if (types == null)
                types = new List<int>();
            types.Add(StringKit.toInt(strss[4]));
		}
	}
    private void parseAllSignAward(string str) {
        string[] strs = str.Split(',');
        int type = StringKit.toInt(strs[0]);
        int sid = StringKit.toInt(strs[1]);
        int num = StringKit.toInt(strs[2]);
        allSignPrize = new PrizeSample(type, sid, num);
        allSignSid = StringKit.toInt(strs[3]);
    }

	public override void copy (object destObj)
	{
		base.copy (destObj);
        SignInSample dest = destObj as SignInSample;
		if (this.list != null) {
			dest.list = new List<PrizeSample> ();
			for (int i = 0; i < this.list.Count; i++)
				dest.list.Add (this.list [i].Clone () as PrizeSample);
		}
	}
}
