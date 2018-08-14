using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 节日许愿模版
/// </summary>
public class FestivalWishSample: Sample {
    /** 活动条目sid */
    public int noitceItemSid;
    /** 奖励道具 */
    public PrizeSample prizes;
    /** 最大许愿人数 */
    public int maxWishsNum;
	/** 原价 */
	public int oldCost;
	/** 许愿价 */
	public int wishCost;


    public override void parse(int sid, string str)
    {
		//1000|71063|1|1|100|50|30
        string[] strs = str.Split('|');
        this.noitceItemSid = sid;
		this.prizes = new PrizeSample(StringKit.toInt(strs[2]),StringKit.toInt(strs[1]),StringKit.toInt(strs[3]));
		this.maxWishsNum = StringKit.toInt(strs[4]);
		this.oldCost = StringKit.toInt(strs[5]);
		this.wishCost = StringKit.toInt(strs[6]);
    }
}

