using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 许愿实体
/// </summary>
public class FestivalWish {
    /** 当前许愿人数 */
    public int currentWishNum;
    /** 许愿状态 */
    public int state;
	/** 结束时间 */
	public int endTime;
    /** 许愿模版sid */
    public int sid;
	/** 许愿模版 */
	public FestivalWishSample sample;

    
	public FestivalWish(int sid)
	{
		this.sid = sid;
		this.sample = FestivalWishSampleManager.Instance.getFestivalWishSampleBySid(sid);
	}
	public FestivalWish(){
	}
}

