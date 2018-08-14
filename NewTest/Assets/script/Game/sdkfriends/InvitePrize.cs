using System;
using System.Collections;

public class InvitePrize  {
	
	//奖品的sid
	public string prizeSid;
	//奖品所处等级
	public int friendsLevel;
	//好友等级最大领取次数
	public int GetMax;
	
	//可领取的次数
	public int currentGetNum;
	//可领取的份数
	public int getNums;
	
	//返利的金钱数
	public int backMoney;
	
	//返利是否可以领取
	public bool isBackGet;
	
	//返利说明
	public string backDesc;

	public PrizeSample[] prizes;
	//是否可以领取等级奖品
	public bool isGetLevelPrize;


	public InvitePrize (string str)
	{
		parse(str);
	}

	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		
		this.prizeSid = (strArr [0]);
		if (prizeSid != "100001")
		{
		  this.GetMax = StringKit.toInt(strArr [1]);
		  this.friendsLevel = StringKit.toInt(strArr [2]);
		  parsePrizes(strArr[3]);
		} else {
	      parsePrizes(strArr[1]);
		}
		
		
	}
	
	private void parsePrizes (string str)
	{
		string[] strArr = str.Split ('#');
		prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length && !string.IsNullOrEmpty(strArr[i]); i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
	}

}

public class LadderHegeMoney  {
	
	public string ladderSid;
	//等级范围
	public string rangeLevel;
	//等级积分
	public int levelPoint;
	//鉆石獎勵
	public int rmbPrize;

	// 对应天梯争霸活动的sid
	public int ladernoticeSid;

	public	 int startLevel = 0;
	public int endLevel = 0;
	

	
	
	public LadderHegeMoney (string str)
	{
		parse(str);
	}
	
	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		
		this.ladderSid = (strArr [0]);
		this.rangeLevel = (strArr [1]);
		lparse (strArr [1]);
		this.levelPoint = StringKit.toInt(strArr [2]);
		this.rmbPrize = StringKit.toInt (strArr [3]);
		this.ladernoticeSid = StringKit.toInt (strArr [4]);

	}

	private void lparse (string str)
	{
		string[] strArr = str.Split ('-');
		if (strArr.Length > 1) {
			startLevel =  StringKit.toInt(strArr[0]);
			endLevel =  StringKit.toInt( strArr[1]);
		}
	}
	
	
}

