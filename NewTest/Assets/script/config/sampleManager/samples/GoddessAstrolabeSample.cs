using UnityEngine;
using System.Collections;

/**
 * 女神星盘
 * @author 陈世惟
 * */
public class GoddessAstrolabeSample {

	public GoddessAstrolabeSample (string str)
	{
		parse (str);
	}

	//本地配置
	public int id;//星星序号
	public int sizeType;//星星体积大小(1大2小）
	public int mainType;//星星主副类型(1主2副)
	public GoddessAstrolabeConditions[] conditions;//条件(消耗)
	public int father;//上一级星星
	public int[] next;//下一级星星
	public float[] position;//坐标
	public int colorId;//星星ID(1绿星星,2红星星,3橙星星,4蓝星星,5黄星星,6四角星,7五角星)
	public int awardType;//奖励类型(1属性,2功能,3奖励）
	public string awardDesc;//奖励描述
	public GoddessAstrolabeAward award;//奖励

	//后台信息
	public bool isOpen = false;

	public void parse (string str)
	{
		int num = 0;
		string[] strArr = str.Split ('|');
		this.id = StringKit.toInt (strArr [num++]);//星星序号
		this.sizeType = StringKit.toInt (strArr [num++]);//星星体积大小(1大2小）
		this.mainType = StringKit.toInt (strArr [num++]);//星星主副类型(1主2副)
		parseConditions (strArr [num++]);//条件(消耗)
		this.father = StringKit.toInt (strArr [num++]);//上一级星星
		this.next = parseInt (strArr [num++]);//下一级星星
		this.position = parseIntArray (strArr [num++]);//坐标
		this.colorId = StringKit.toInt (strArr [num++]);//星星ID
		this.awardType = StringKit.toInt (strArr [num++]);//奖励类型
		this.awardDesc = strArr [num++];//奖励描述
		parseAward (strArr [num++]);//奖励
	}

	//解析消耗条件
	private void parseConditions (string str)
	{
		string[] strByLv = str.Split ('#');
		int maxLv = strByLv.Length;
		conditions = new GoddessAstrolabeConditions[maxLv];
		
		for (int i = 0; i < maxLv; i++) {
			GoddessAstrolabeConditions cs = new GoddessAstrolabeConditions(strByLv[i]);
			conditions[i] = cs;
		}
	}

	//解析奖励消耗条件
	private void parseAward (string str)
	{
		if(str == "0") {
			award = null;
			return;
		}
		award  = new GoddessAstrolabeAward(str);
	}

	private float[] parseIntArray (string str)
	{
		string[] strArr = str.Split (',');
		float[] array = new float[strArr.Length];
		
		for (int i = 0; i < strArr.Length; i++) {
			array [i] = float.Parse (strArr [i]);
		}
		return array;
	}

	private int[] parseInt (string str)
	{
		string[] strArr = str.Split (',');
		int[] array = new int[strArr.Length];
		
		for (int i = 0; i < strArr.Length; i++) {
			array [i] = StringKit.toInt (strArr [i]);
		}
		return array;
	}
}

public class FindGoddessAstrolabeSample
{
	private int idValue;

	public int id
	{
		get {return idValue;}
		set {this.idValue = value;}
	}

	public FindGoddessAstrolabeSample ()
	{

	}

	public bool FindGoddessAstrolabeSampleByID (GoddessAstrolabeSample ga)
	{
		return idValue == ga.id;
	}
}

public class GoddessAstrolabeStarArray
{
	public GoddessAstrolabeStarArray ()
	{

	}

	public int id;//星云序号
	public int[] stars;//星星集合
	public int lastId;//最后一颗星星ID

	public void parse (int id, string str)
	{
		this.id = id;//星云序号
		int num = 1;
		string[] strArr = str.Split ('|');
//		this.id = StringKit.toInt (strArr [num++]);//星云序号
		this.stars = parseIntArray (strArr [num++]);//星星集合
		this.lastId = StringKit.toInt (strArr [num++]);//最后一颗星星ID

	}

	private int[] parseIntArray (string str)
	{
		string[] strArr = str.Split (',');
		int[] array = new int[strArr.Length];
		
		for (int i = 0; i < strArr.Length; i++) {
			array [i] = StringKit.toInt (strArr [i]);
		}
		return array;
	}
}

public class GoddessAstrolabeInfo
{
	public int[] openStars = null;//激活的星星
	public int star_score = 0;//星尘碎片
	public CardBaseAttribute frontAddEffectInteger = null;//前排属性加成
	public CardBaseAttribute frontAddEffectNumber = null;//前排属性百分比加成
	public CardBaseAttribute middleAddEffectInteger = null;//中排属性加成
	public CardBaseAttribute middleAddEffectNumber = null;//中排属性百分比加成
	public CardBaseAttribute behindAddEffectInteger = null;//后排属性加成
	public CardBaseAttribute behindAddEffectNumber = null;//后排属性百分比加成
	public CardBaseAttribute allAddEffectInteger = null;//所有卡片属性加成
	public CardBaseAttribute allAddEffectNumber = null;//所有卡片属性百分比加成
	public string addPveAttr = "0";//PVE伤害加成
	public int addFriend = 0;//增加好友上限
	public int addEquipStorage = 0;//增加装备仓库上限
	public int addCardStorage = 0;//增加卡片仓库上限
	public int addPveUse = 0;//增加PVE行动力上限
}

//星盘需求
public class GoddessAstrolabeConditions
{
	public string type = "";//类型
	public string num = "";//数量
	public string desc = "";//描述

	public GoddessAstrolabeConditions (string str)
	{
		parse (str);
	}
	
	private void parse (string str)
	{
		string[] strArr = str.Split ('*');
		type = strArr [0];
		num = strArr [1];
		desc = strArr [2];
	}
}

//星盘奖励
public class GoddessAstrolabeAward
{
	public int awardType = 0;//奖励类型
	public int awardSid = 0;//奖励物品SID
	public int awardNum = 0;//奖励数量

	public GoddessAstrolabeAward (string str)
	{
		parse (str);
	}
	
	private void parse (string str)
	{
		string[] strArr = str.Split (',');
		awardType = StringKit.toInt(strArr [0]);
		awardSid = StringKit.toInt(strArr [1]);
		awardNum = StringKit.toInt(strArr [2]);
	}
}
