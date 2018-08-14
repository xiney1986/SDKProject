using UnityEngine;
using System.Collections;

/**
 * 进化模板
 * @author 陈世惟
 * */
public class EvolutionSample : Sample
{

	public EvolutionSample ()
	{

	}

	private int type;//卡片类型
	private int maxEvoLevel;//进化等级上限
	private int[] icoSid;//形象设置，为0则不改变
	private int[] cardLevel;//需要卡片等级
	private int[] playerLevel;//需要玩家等级
	private long[] needMoney;//消耗金钱条件
	private EvolutionCondition[][] conditions;//条件(进化次数)(消耗)
	private int[] addLevel;//修身完成后增加等级上限
	private AttrChangeSample[][] addEffects = null;//产生属性影响效果(进化次数)(效果)
	private string[] addTalentString;//天赋描述
	private int[] talentNeedTimes; //天赋开启需要进化次数
	private int evoValue;//进化价值,传承用

	public void parse (int sid, string str)
	{
		int num = 1;
		string[] strArr = str.Split ('|');
		this.type = sid;	//卡片类型
		this.maxEvoLevel = StringKit.toInt (strArr [num++]);	//进化等级上限
		this.icoSid = parseIntArray (strArr [num++]);	//形象设置
		this.cardLevel = parseIntArray (strArr [num++]);	//需要卡片等级
		this.playerLevel = parseIntArray (strArr [num++]);	//需要玩家等级
		this.needMoney = parseLongArray (strArr [num++]);	//消耗金钱条件
		parseConditions (strArr [num++]);	//条件(进化次数)(消耗)
		this.addLevel = parseIntArray (strArr [num++]);	//修身完成后增加等级上限
		parseSuitAttr (strArr [num++]);	//产生属性影响效果(进化次数)(效果)
		this.addTalentString = parseStringArray (strArr [num++]);//天赋描述
		this.talentNeedTimes = parseIntArray (strArr [num++]); //天赋开启需要进化次数
		this.evoValue = StringKit.toInt (strArr [num++]);//进化价值,传承用
	}

	private long[] parseLongArray (string str)
	{
		string[] strArr = str.Split (',');
		long[] array = new long[strArr.Length];
		
		for (int i = 0; i < strArr.Length; i++) {
			array [i] = StringKit.toLong (strArr [i]);
		}
		return array;
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

	private string[] parseStringArray (string str)
	{
		if (str == "0")
			return null;
		else
			return str.Split ('#');
	}

	//解析兑换消耗条件
	private void parseConditions (string str)
	{
		string[] strByLv = str.Split ('#');
		int maxLv = strByLv.Length;
		conditions = new EvolutionCondition[maxLv][];

		for (int i = 0; i < maxLv; i++) {
			conditions [i] = parseEvoCon (strByLv [i]);
		}

	}

	private EvolutionCondition[] parseEvoCon (string str)
	{
		if (str == "0")
			return null;
		string[] strArr = str.Split ('*');
		EvolutionCondition[] evoCon = new EvolutionCondition[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			EvolutionCondition attr = new EvolutionCondition (strArr [i]);
			evoCon [i] = attr;
		}
		return evoCon;
	}

	//解析奖励属性效果
	private void parseSuitAttr (string str)
	{
		string[] strByLv = str.Split ('#');
		int maxLv = strByLv.Length;
		addEffects = new AttrChangeSample[maxLv][];

		for (int i = 0; i < maxLv; i++) {
			if (strByLv [i] == "0")
				addEffects [i] = null;
			else
				addEffects [i] = parseAttrChange (strByLv [i]);
		}
	}

	private AttrChangeSample[] parseAttrChange (string str)
	{
		string[] strArr = str.Split ('*');
		AttrChangeSample[] effects = new AttrChangeSample[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			AttrChangeSample attr = new AttrChangeSample ();
			attr.parse (strArr [i]);
			effects [i] = attr;
		}

		return effects;
	}

	/** 卡片类型 */
	public int getCardType ()
	{
		return type;
	}
	
	/** 进化等级上限 */
	public int getMaxEvoLevel ()
	{
		return maxEvoLevel;
	}
	
	/** 形象设置，为0则不改变 */
	public int[] getIcoSid ()
	{
		if (icoSid == null)
			return null;
		else if (icoSid [0] == 0)
			return null;
		else
			return icoSid;
	}
	
	/** 需要卡片等级 */
	public int[] getCardLevel ()
	{
		return cardLevel;
	}
	
	/** 需要主角等级 */
	public int[] getPlayerLevel ()
	{
		return playerLevel;
	}
	
	/** 消耗金钱条件 */
	public long[] getNeedMoney ()
	{
		return needMoney;
	}
	
	/** 修身完成后增加等级 */
	public int[] getAddLevel ()
	{
		return addLevel;
	}
	
	/** 条件(进化次数)(消耗) */
	public EvolutionCondition[][] getConditions ()
	{
		if (conditions.Length > 1)
			return conditions;
		else 
			return null;
	}
	
	/** 产生属性影响效果(进化次数)(效果) */
	public AttrChangeSample[][] getAddEffects ()
	{
		return addEffects;
	}

	/** 天赋描述 */
	public string[] getAddTalentString ()
	{
		return addTalentString;
	}

	/** 天赋开启需要进化次数 */
	public int[] getTalentNeedTimes ()
	{
		return talentNeedTimes;
	}

	/** 获得卡片进化价值 */
	public int getEvoValue()
	{
		return evoValue;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		EvolutionSample dest = destObj as EvolutionSample;
		if (this.icoSid != null) {
			dest.icoSid = new int[this.icoSid.Length];
			for (int i = 0; i < this.icoSid.Length; i++)
				dest.icoSid [i] = this.icoSid [i];
		}
		if (this.cardLevel != null) {
			dest.cardLevel = new int[this.cardLevel.Length];
			for (int i = 0; i < this.cardLevel.Length; i++)
				dest.cardLevel [i] = this.cardLevel [i];
		}
		if (this.playerLevel != null) {
			dest.playerLevel = new int[this.playerLevel.Length];
			for (int i = 0; i < this.playerLevel.Length; i++)
				dest.playerLevel [i] = this.playerLevel [i];
		}
		if (this.needMoney != null) {
			dest.needMoney = new long[this.needMoney.Length];
			for (int i = 0; i < this.needMoney.Length; i++)
				dest.needMoney [i] = this.needMoney [i];
		}
		if (this.conditions != null) {
			dest.conditions = new EvolutionCondition[this.conditions.Length][];
			EvolutionCondition[] temp;
			for (int i = 0; i < this.conditions.Length; i++) {
				temp = conditions [i];
				if (temp != null) {
					dest.conditions [i] = new EvolutionCondition[temp.Length];
					for (int j = 0; j < temp.Length; j++)
						dest.conditions [i] [j] = temp [j].Clone () as EvolutionCondition;
				}
			}
		}
		if (this.addLevel != null) {
			dest.addLevel = new int[this.addLevel.Length];
			for (int i = 0; i < this.addLevel.Length; i++)
				dest.addLevel [i] = this.addLevel [i];
		}
		if (this.addEffects != null) {
			dest.addEffects = new AttrChangeSample[this.addEffects.Length][];
			AttrChangeSample[] temp;
			for (int i = 0; i < this.addEffects.Length; i++) {
				temp = addEffects [i];
				if (temp != null) {
					dest.addEffects [i] = new AttrChangeSample[temp.Length];
					for (int j = 0; j < temp.Length; j++)
						dest.addEffects [i] [j] = temp [j].Clone () as AttrChangeSample;
				}
			}
		}
		if (this.addTalentString != null) {
			dest.addTalentString = new string[this.addTalentString.Length];
			for (int i = 0; i < this.addTalentString.Length; i++)
				dest.addTalentString [i] = this.addTalentString [i];
		}
		if (this.talentNeedTimes != null) {
			dest.talentNeedTimes = new int[this.talentNeedTimes.Length];
			for (int i = 0; i < this.talentNeedTimes.Length; i++)
				dest.talentNeedTimes [i] = this.talentNeedTimes [i];
		}
	}
}



//兑换条件(消耗)
public class EvolutionCondition:CloneObject
{
	public int costType = 0;//消耗类型 costType
	public int costSid = 0;//消耗品sid
	public int num = 0;//消耗数量

	public EvolutionCondition (string str)
	{
		parse (str);
	}
	
	private void parse (string str)
	{
		string[] strArr = str.Split (',');
		costType = StringKit.toInt (strArr [0]);
		costSid = StringKit.toInt (strArr [1]);
		num = StringKit.toInt (strArr [2]);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
