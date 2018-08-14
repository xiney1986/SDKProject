using System;
 
/**
 * 副本章节模板
 * @author longlingquan
 * */
public class ChapterSample:Sample
{
	public ChapterSample ()
	{
	}
	 
	public string name = "";//副本章节名称
	public string describe = "";//副本章节描述

	public int type = 0;//副本类型 missionType
	public int num = 0;//挑战次数
	public int timeSid = 0;//时间限制sid
	public int[] missions;//所有关卡序列
	public ChapterAwardSample[] prizes = null;//星星奖励
	public int thumbIcon = 0;//关卡选择的缩略图id

	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 8);
		
		//strArr[0] is sid   
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] describe
		this.describe = strArr [2]; 
		//strArr[3] type
		this.type = StringKit.toInt (strArr [3]);
		//strArr[3] num
		this.num = StringKit.toInt (strArr [4]);
		//strArr[5] timeSid
		this.timeSid = StringKit.toInt (strArr [5]);
		//strArr[6] missions
		parseMissions (strArr [6]);
		//strArr[7] prizes
		parsePrizes (strArr [7]);//星星奖励
		this.thumbIcon = StringKit.toInt (strArr [8]);//关卡选择的缩略图id

	}
	
	private void parseMissions (string str)
	{
		string[] strarr = str.Split (',');
		int max = strarr.Length;
		missions = new int[max];
		for (int i = 0; i < max; i++) {
			missions [i] = StringKit.toInt (strarr [i]);	
		}
	}

	private void parsePrizes (string str)
	{
		if (str == "0") {
			return;
		}
        //131203*4*3,71054,5#131204*10*2,0,50$4,35007,1
        //131203*4*3,71054,5
        //131204*10*2,0,50$4,35007,1
		string[] strArr = str.Split ('#');
		int max = strArr.Length;
		if (max == 0)
			return;
		prizes = new ChapterAwardSample[max];
		for (int i = 0; i < max; i++) {
			prizes [i] = new ChapterAwardSample (strArr [i]);
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		ChapterSample dest = destObj as ChapterSample;
		if (this.missions != null) {
			dest.missions = new int[this.missions.Length];
			for (int i = 0; i < this.missions.Length; i++)
				dest.missions [i] = this.missions [i];
		}
		if (this.prizes != null) {
			dest.prizes = new ChapterAwardSample[this.prizes.Length];
			for (int i = 0; i < this.prizes.Length; i++)
				dest.prizes [i] = this.prizes [i].Clone () as ChapterAwardSample;
		}
	}
}

//星星奖励
public class ChapterAwardSample:CloneObject
{
	public int awardSid = 0;//奖励SID
	public int needStarNum = 0;//奖励需求
	public PrizeSample[] prizes = null;//星星奖励

	public override void copy (object destObj)
	{
		base.copy (destObj);
		ChapterAwardSample dest = destObj as ChapterAwardSample;
		if (this.prizes != null) {
			dest.prizes = new PrizeSample[this.prizes.Length];
			for (int i = 0; i < this.prizes.Length; i++)
				dest.prizes [i] = this.prizes [i].Clone () as PrizeSample;
		}
	}

	public ChapterAwardSample (string str)
	{
		parse (str);
	}
    //131203*4*3,71054,5#131204*10*2,0,50$4,35007,1
    //131203*4*3,71054,5
    //131204*10*2,0,50$4,35007,1
	
	private void parse (string str)
	{
		string[] strArr = str.Split ('*');
		awardSid = StringKit.toInt (strArr [0]);
		needStarNum = StringKit.toInt (strArr [1]);
		parsePrizes (strArr [2]);
	}

	//解析通关奖励
	private void parsePrizes (string str)
	{
		string[] strArr = str.Split ('$');
		int max = strArr.Length;
		if (max == 0)
			return;
		prizes = new PrizeSample[max];
		for (int i = 0; i < max; i++) {
			prizes [i] = new PrizeSample (strArr [i], ',');
		}
	}

}

//星星后台奖励领取信息
public class ChapterAwardServerSample
{
	public int chapterSid = 0;//章节sid
	public int[] awardSids = null;//已领取奖励集合
}
