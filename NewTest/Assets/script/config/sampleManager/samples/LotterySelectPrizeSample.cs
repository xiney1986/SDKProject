using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LotterySelectPrizeSample
{
	public int id;
	public string name;
	public PrizeSample[] prizes;
	public int state;
	public int condition;// 领取条件//

	public LotterySelectPrizeSample(string str)
	{
		parseStr(str);
	}
	
	void parseStr(string str)
	{
		string[] strs = str.Split('|');
		this.id = StringKit.toInt(strs[0]);
		this.name = strs[1];
		this.prizes = parsePrizes(strs[2]);
		this.condition = StringKit.toInt(strs[3]);
	}

	public PrizeSample[] parsePrizes(string str)
	{
		string[] strArr = str.Split ('#');
		PrizeSample[] prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
		return prizes;
	}

}
public class LotterySelectPrizeState
{
	public const int CantRecive = 0;// 不能领取//
	public const int CanRecive = 1;// 可领取//
	public const int Recived = 2; // 已领取//
}

