using System;

/**
 *成长计划奖励
 *@author 黄兴财
 **/
public class GrowupAwardSample:Sample
{
	
	public GrowupAwardSample ()
	{
	}


	public PrizeSample prize;//奖品
	public string needLevel;//需要的等级
	public int backPercent;//返点比例
	public int totalinvest;
	public bool unclaimed=false;//是否领取过
	
	public override  void parse (int sid,string str)
	{
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 1);
		this.needLevel = strArr[0];
		this.backPercent = StringKit.toInt(strArr[1]);
		prize = parsePrize();
	}
	
	PrizeSample parsePrize ()
	{
		PrizeSample sample = new PrizeSample ();
		sample.type = PrizeType.PRIZE_RMB;
		sample.pSid = 0;
		sample.num = "20";
		return sample;
	}

	 
	public override void copy (object destObj)
	{
		base.copy (destObj);
		GrowupAwardSample dest = destObj as GrowupAwardSample;

	}
} 

