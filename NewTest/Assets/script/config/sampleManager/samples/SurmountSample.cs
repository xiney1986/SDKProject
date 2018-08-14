using UnityEngine;
using System.Collections;

public class SurmountSample : Sample
{

	public SurmountSample (string str)
	{
		parse (str);
	}
	
	private int type;//卡片类型
	private int maxSurNum;//最大突破次数
	private int[] icoSid;//形象设置，为0则不改变
	private int[] evoLevel;//需要修身等级
	private int[] quitlyLevel;//突破后品质等级
	private int[] needMoney;//消耗金钱条件
	private EvolutionCondition[][] conditions;//条件(突破次数)(消耗)
	private int[] maxSurLevel;//突破后增加等级上限
	private AttrChangeSample[][] addEffects = null;//产生属性影响效果(突破次数)(效果)
	private string[][] addEffectString;//突破完成后增加效果描述(突破次数)(效果)

	public void parse (string str)
	{
		int num = 0;
		string[] strArr = str.Split ('|');
		this.type = StringKit.toInt (strArr [num++]);	//卡片类型
		this.maxSurNum = StringKit.toInt (strArr [num++]);	//突破后增加等级上限
		this.icoSid = parseIntArray (strArr [num++]);	//形象设置
		this.evoLevel = parseIntArray (strArr [num++]);	//需要修身等级
		this.quitlyLevel = parseIntArray (strArr [num++]);	//突破后品质等级
		this.needMoney = parseIntArray (strArr [num++]);	//消耗金钱条件
		parseConditions (strArr [num++]);	//条件(突破次数)(消耗)
		this.maxSurLevel = parseIntArray (strArr [num++]);	//突破后增加等级上限
		parseSuitAttr (strArr [num++]);	//产生属性影响效果(突破次数)(效果)
		parseStringArray (strArr [num++]);	//突破完成后增加效果(突破次数)(效果)
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

	//解析突破完成后增加效果
	private void parseStringArray (string str)
	{
		string[] strByLv = str.Split ('#');
		int maxLv = strByLv.Length;
		addEffectString = new string[maxLv][];
		
		for (int i = 0; i < maxLv; i++) {
			addEffectString [i] = strByLv [i].Split ('~');
		}
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

	/** 最大突破次数 */
	public int getMaxSurNum ()
	{
		return maxSurNum;
	}
	
	/** 突破后增加等级上限 */
	public int[] getMaxSurLevel ()
	{
		return maxSurLevel;
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
	
	/** 需要修身等级(突破等级) */
	public int[] getSurLevel ()
	{
		return evoLevel;
	}
	
	/** 突破后增加品质等级(突破等级) */
	public int[] getQuitlyLevel ()
	{
		return quitlyLevel;
	}
	
	/** 消耗金钱条件 */
	public int[] getNeedMoney ()
	{
		return needMoney;
	}
	
	/** 条件(突破次数)(消耗) */
	public EvolutionCondition[][] getConditions ()
	{
		if (conditions.Length > 1)
			return conditions;
		else 
			return null;
	}
	
	/** 产生属性影响效果(突破次数)(效果) */
	public AttrChangeSample[][] getAddEffects ()
	{
		if (addEffects.Length > 1)
			return addEffects;
		else
			return null;
	}
	
	/** 突破完成后增加效果(突破次数)(效果) */
	public string[][] getAddEffectByString ()
	{
		return addEffectString;
	}
	
	public override void copy (object destObj)
	{
		base.copy (destObj);
		SurmountSample dest = destObj as SurmountSample;
		if (this.icoSid != null) {
			dest.icoSid = new int[this.icoSid.Length];
			for (int i = 0; i < this.icoSid.Length; i++)
				dest.icoSid [i] = this.icoSid [i];
		}
		if (this.evoLevel != null) {
			dest.evoLevel = new int[this.evoLevel.Length];
			for (int i = 0; i < this.evoLevel.Length; i++)
				dest.evoLevel [i] = this.evoLevel [i];
		}
		if (this.quitlyLevel != null) {
			dest.quitlyLevel = new int[this.quitlyLevel.Length];
			for (int i = 0; i < this.quitlyLevel.Length; i++)
				dest.quitlyLevel [i] = this.quitlyLevel [i];
		}
		if (this.needMoney != null) {
			dest.needMoney = new int[this.needMoney.Length];
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
		if (this.maxSurLevel != null) {
			dest.maxSurLevel = new int[this.maxSurLevel.Length];
			for (int i = 0; i < this.maxSurLevel.Length; i++)
				dest.maxSurLevel [i] = this.maxSurLevel [i];
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
		if (this.addEffectString != null) {
			dest.addEffectString = new string[this.addEffectString.Length][];
			string[] temp;
			for (int i = 0; i < this.addEffectString.Length; i++) {
				temp = addEffectString [i];
				if (temp != null) {
					dest.addEffectString [i] = new string[temp.Length];
					for (int j = 0; j < temp.Length; j++)
						dest.addEffectString [i] [j] = temp [j];
				}
			}
		}
	}
}
