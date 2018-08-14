
using System;

public class RankAward  {
	public PrizeSample[] prizes;//奖品
	public int rinkNum;//第几名
	public int awardrinkORsource;//奖励类型 只针对抽奖奖励
	public string dec;//描述
	public int type;//类型卡片 装备
	public int awardSid;//奖励的Sid;
	public int needSource;//需要的积分
	public bool isAward=false;//是否领取过
	public int noticeSid;//活动积分Sid;

	public override string ToString()
	{
		string str = "";
		if (!string.IsNullOrEmpty(dec))
		{
			str = dec;
		}
		else
		{
			str += "积分达到" + needSource;
		}
		str += " 奖励 ";
		for (int i = 0; i < prizes.Length; i++)
		{
			str += prizes[i];
			if (i < prizes.Length - 1)
			{
				str += "，";
			}
		}
		return base.ToString()+"\t"+str;
	}
}
