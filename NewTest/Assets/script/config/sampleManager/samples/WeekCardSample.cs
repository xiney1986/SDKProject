using UnityEngine;
using System.Collections;

public class WeekCardSample
{
	public int id;
	public string des;
	public int costDiamond;
	public int days;
	public int limitLv;
	public PrizeSample[] prizes;

	public WeekCardSample(string weekCardInfo,string prizeInfo,string limitLv)
	{
		parseWeekCardInfo(weekCardInfo);
		parsePrizeInfo(prizeInfo);
		this.limitLv = StringKit.toInt(limitLv);
	}

	public void parseWeekCardInfo(string weekCardInfo)
	{
		string[] strArr = weekCardInfo.Split(',');
		this.id = StringKit.toInt(strArr[0]);
		this.des = strArr[1];
		this.costDiamond = StringKit.toInt(strArr[2]);
		this.days = StringKit.toInt(strArr[3]);
	}
	public void parsePrizeInfo(string prizeInfo)
	{
		string[] prizesArr = prizeInfo.Split('#');
		string[] prize;
		PrizeSample sample;
		int prizeID;
		int prizeType;
		int prizeNum;
		this.prizes = new PrizeSample[prizesArr.Length];
		for(int i=0;i<prizesArr.Length;i++)
		{
			prize = prizesArr[i].Split(',');
			prizeType = StringKit.toInt(prize[0]);
			prizeID = StringKit.toInt(prize[1]);
			prizeNum = StringKit.toInt(prize[2]);
			sample = new PrizeSample(prizeType,prizeID,prizeNum);
			this.prizes[i] = sample;
		}
	}
}
