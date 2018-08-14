using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastBattleProcessPrizeSample
{
	public int id;
	public string name;
	public PrizeSample[] prizes;//奖品
	public int state = LastBattleProcessPrizeState.CANNOT_RECEVIE;// 领取状态//
	public string processDesc;// 进度伤害描述//
	public int processType;// 进度伤害类型//
	public string harmValue;// 伤害值//
	public Dictionary<int,string> harmValueDic;
	public Dictionary<int,string> harmDesDic;

	public LastBattleProcessPrizeSample(string str)
	{
		parseStr(str);
	}

	public LastBattleProcessPrizeSample(string desc,string harmValue)
	{
		this.processDesc = desc;
		this.harmValue = harmValue;
	}

	void parseStr(string str)
	{
		string[] strArr = str.Split ('|');
		this.id = StringKit.toInt(strArr [0]);
		this.name = strArr [1];
		parsePrizes(strArr[2]);
		this.processType = StringKit.toInt(strArr [3]);

		if(processType == LastBattleProcessType.MIX)
		{
			this.harmDesDic = parseDescDic(strArr [4]);
			this.harmValueDic = parseDic(strArr [5]);
		}
		else 
		{
			this.processDesc = strArr [4];
			this.harmValue = strArr [5];
		}
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
	Dictionary<int,string> parseDic(string str)
	{
		Dictionary<int,string> dic = new Dictionary<int, string>();
		string[] str1 = str.Split (',');
		string[] str2;
		for(int i=0;i<str1.Length;i++)
		{
			str2 = str1[i].Split('#');
			dic.Add(StringKit.toInt(str2[0]),str2[1]);
		}
		return dic;
	}
	Dictionary<int,string> parseDescDic(string str)
	{
		Dictionary<int,string> dic = new Dictionary<int, string>();
		string[] str1 = str.Split (',');
		string[] str2;
		for(int i=0;i<str1.Length;i++)
		{
			str2 = str1[i].Split('#');
			dic.Add(StringKit.toInt(str2[0]),str2[1]);
			if(i < str1.Length - 1)
			{
				this.processDesc += str2[1] + ",";
			}
			else
			{
				this.processDesc += str2[1];
			}
		}
		return dic;
	}
}

public class LastBattleProcessPrizeState
{
	public const int CANNOT_RECEVIE = -1;// 不可领取//
	public const int CAN_RECEVIE = 0;// 可领取//
	public const int RECEVIED = 1;// 已领取//
}

public class LastBattleProcessType
{
	public const int STATE_WEAK = 1;//状态弱点类型//
	public const int PHYSICAL_HARM = 2;//物理伤害类型//
	public const int MAGIC_HARM = 3;//魔法伤害类型//
	public const int MIX = 4;// 混合//
}
