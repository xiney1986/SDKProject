using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 节日礼花模版
/// </summary>
public class FestivalFireworksSample: Sample {
    /** 礼花条目sid */
    public int noitceItemSid;
    /** 奖励道具 */
    public PrizeSample prizes;
	/** 兑换模版 */
	public ExchangeSample exchangeSample;

    public override void parse(int sid, string str)
    {
        string[] strs = str.Split('|');
        this.noitceItemSid = sid;
		string[] prize= parse(strs[1]);
		this.prizes = new PrizeSample(StringKit.toInt(prize[1]),StringKit.toInt(prize[0]),StringKit.toInt(prize[2]));
		this.exchangeSample = ExchangeSampleManager.Instance.getExchangeSampleBySid(sid);
    }
	private string[] parse(string str)
	{
		return str.Split(',');
	}
}

