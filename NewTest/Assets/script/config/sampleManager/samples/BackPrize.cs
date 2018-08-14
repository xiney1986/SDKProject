using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackPrize
{
	public int dayID;
	public int isRecevied = BackPrizeRecevieType.CANT_RECEVIE;// 是否领取或者不可领取//
	public PrizeSample[] prizes;//奖品

	public BackPrize(string str)
	{
		parse(str);
	}

	public void parse(string str)
	{
		string[] strArr = str.Split ('|');
		this.dayID = StringKit.toInt(strArr [0]);
		parsePrizes(strArr[1]);
	}

	public void parsePrizes(string str)
	{
		string[] strArr = str.Split ('#');
		prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
	}

}

public class BackPrizeRecevieType
{
	public static int CANT_RECEVIE = -1;// 不可领取//	
	public static int RECEVIE = 0;// 待领取//
	public static int RECEVIED = 1;// 已领取//
}
