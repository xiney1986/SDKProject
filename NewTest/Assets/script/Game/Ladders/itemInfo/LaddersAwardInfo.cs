using System;
/// <summary>
/// 天梯排名奖励发放 时数据封装
/// </summary>
public class LaddersAwardInfo
{
	public bool hasReceive=true;//是否已经领取
	public int limitTime;//领取限制时间
	public int rank;//奖励发放时玩家的排名

	public bool canReceive
	{
		get
		{
			return (!hasReceive)&&(limitTime>=ServerTimeKit.getSecondTime());
		}
	}
	public LaddersAwardInfo ()
	{
	}
	public void M_update(bool _hasReceive,int _limitTime,int _rank)
	{
		hasReceive=_hasReceive;
		limitTime=_limitTime;
		rank=_rank;
	}
	public void M_clear()
	{
		hasReceive=true;
		limitTime=0;
		rank=0;
	}
}


