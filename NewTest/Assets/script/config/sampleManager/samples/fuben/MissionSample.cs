using System;
 
/**
 * 关卡模板
 * @author longlingquan
 * */
public class MissionSample:Sample
{
	public MissionSample ()
	{ 
	}

	public string name = "";//关卡名称 
	public string describe = "";//关卡描述
	public int chapterSid = 0;//章节sid
	public int level = 0;//最低等级限制  
	public int[] num;//挑战次数(普通#困难#噩梦，0==永久)
	public int timeSid = 0;//时间限制sid
	public string[] points;//事件点信息
	public PrizeSample[] firstPrizes;//首通奖励
	public PrizeSample[] prizes;//通关奖励
	public int bossSid = 0;//boss sid
	public int bossLv = 0;//boss 等级
	public int bloodType = 0;//1 不计血 2 要计血
	public int[] recommendedCombat;//推荐战斗力,难度不同 推荐战斗力不同
	public int[] weather;//随机天气的ID组
	public int teamType;//队伍类型要求 1表示主力战 ，2标识全队

	//其他 
	//英雄之章 是否直接战斗(0进副本1进战斗)#是否觉醒(0不觉醒1觉醒)#怪物等级#觉醒描述
	//剧情副本 关卡类型(0普通1特殊)
	public string[] other;
//	public string awakeStr; //觉醒文字
	public int maxStar = 0;//最大难度数
	public int pveCost = 0;//消耗行动力
	
	public override void parse (int sid, string str)
	{ 
		this.sid = sid;
		string[] strArr = str.Split ('|');
//		checkLength (strArr.Length, 17);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] describe
		this.describe = strArr [2];
		//strArr[3] chapterType
		this.chapterSid = StringKit.toInt (strArr [3]);
		//strArr[4] level
		this.level = StringKit.toInt (strArr [4]);
		//strArr[5] num
		parseNum (strArr [5]);
		//strArr[6] timeSid
		this.timeSid = StringKit.toInt (strArr [6]);
		//strArr[7]  points
		parsePoints (strArr [7]);
		//strArr[8] firstPrizes
		parseFirstPrizes (strArr [8]);
		//strArr[9] prizes
		parsePrizes (strArr [9]);
		//strArr[10] boss
		parseBoss (strArr [10]);
		//strArr[11] bloodType
		this.bloodType = StringKit.toInt (strArr [11]);
		////strArr[12] teamType
		this.teamType = StringKit.toInt(strArr[12]);
		//strArr[12] combat
		this.recommendedCombat = StringKit.toArrayInt (strArr [13], ',');

		parseOther (strArr [14]);
		parseWeather (strArr [15]);
		this.maxStar = StringKit.toInt (strArr [16]);//最大难度数
		this.pveCost = StringKit.toInt (strArr [17]);//消耗行动力
	}

	public int getRecommendCombat (int difficulty)
	{
		return this.recommendedCombat [difficulty - 1];
	}

	public int getRecommendCombat ()
	{
		return this.recommendedCombat [0];
	}

	void parseWeather (string str)
	{
		string[] strID = str.Split (',');
		if (strID == null || strID.Length < 1)
			return;
		weather = new int[strID.Length];

		for (int i=0; i<strID.Length; i++) {
			weather [i] = StringKit.toInt (strID [i]);
		}
	}

	private void parseNum (string str)
	{
		string[] strID = str.Split ('#');
		if (strID == null || strID.Length < 1)
			return;
		num = new int[strID.Length];
		
		for (int i=0; i<strID.Length; i++) {
			num [i] = StringKit.toInt (strID [i]);
		}
	}

	private void parsePoints (string str)
	{
		points = str.Split ('#');
	}
	
	//解析首通奖励
	private void parseFirstPrizes (string str)
	{
		string[] strArr = str.Split ('#');
		int max = strArr.Length;
		if (max == 0)
			return;
		firstPrizes = new PrizeSample[max];
		for (int i = 0; i < max; i++) {
			firstPrizes [i] = new PrizeSample (strArr [i], ',');
		}

	}

	//解析其他数据
	private void parseOther (string str)
	{
		other = str.Split ('#');
	}


	//解析通关奖励
	private void parsePrizes (string str)
	{
		string[] strArr = str.Split ('#');
		int max = strArr.Length;
		if (max == 0)
			return;
		prizes = new PrizeSample[max];
		for (int i = 0; i < max; i++) {
			prizes [i] = new PrizeSample (strArr [i], ',');
		}
	}
	
	//解析boss数据
	private void parseBoss (string str)
	{
		if (str == "0")
			return;
		string[] strArr = str.Split (',');
		bossSid = StringKit.toInt (strArr [0]);
		bossLv = StringKit.toInt (strArr [1]);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 
//队伍类型 全部,还是主力
public class TeamType
{
	/// <summary>
	/// 主力
	/// </summary>
	public const int Main=1;
	/// <summary>
	/// 全队
	/// </summary>
	public const int All=2;

}

