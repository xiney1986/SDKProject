using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LotteryBuyCountSample
{
	public int vipLv;// vip等级//
	public int goldCount;// 金币购买次数//
	public int rmbCount;// 钻笔购买次数//
	public int totalCount;// 购买总次数//

	public LotteryBuyCountSample(string str)
	{
		parseStr(str);
	}
	
	void parseStr(string str)
	{
		string[] strs = str.Split('|');
		this.vipLv = StringKit.toInt(strs[1]);
		this.goldCount = StringKit.toInt(strs[2]);
		this.rmbCount = StringKit.toInt(strs[3]);
		this.totalCount = StringKit.toInt(strs[4]);
	}

}

