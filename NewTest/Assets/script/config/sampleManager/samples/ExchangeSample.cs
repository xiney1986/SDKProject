using System;
 
/**
 * 兑换模板
 * @author longlingquan
 * */
public class ExchangeSample:Sample
{
	public ExchangeSample ()
	{
	}
	 
	public string describe = "";//描述信息
	public int exType = 0;//兑换条目类型
	public int type = 0;//类型CostType.cs
	public int exchangeSid = 0;//兑换物品sid
	public int num = 0;//每次兑换数量
	public int times = 0;//兑换次数 0表示无限制
	public int start = 0;//开始时间(秒)
	public int end = 0;//结束时间(秒)
	public ExchangePremise[][] premises;//兑换前提条件
	public ExchangeCondition[][] conditions;//兑换条件(消耗)
	
	public string  getExhangeItemName ()
	{
		if (type == PrizeType.PRIZE_CARD) {		
			return CardSampleManager.Instance.getRoleSampleBySid (exchangeSid).name;	
		} else if (type == PrizeType.PRIZE_EQUIPMENT) {
			return EquipmentSampleManager.Instance.getEquipSampleBySid (exchangeSid).name;
		} else if (type == PrizeType.PRIZE_PROP) {
			return PropSampleManager .Instance.getPropSampleBySid (exchangeSid).name;
		} else if(type==PrizeType.PRIZE_MOUNT){
			return MountsSampleManager.Instance.getMountsSampleBySid(exchangeSid).name;
		} else if(type==PrizeType.PRIZE_STARSOUL){
			return StarSoulManager.Instance.createStarSoul (exchangeSid).getName ();
		} else if(type==PrizeType.PRIZE_RMB){
			return LanguageConfigManager.Instance.getLanguage ("s0131");
		} else if(type==PrizeType.PRIZE_MONEY){
			return LanguageConfigManager.Instance.getLanguage ("s0049");
		}
		return "none";
	}

	public int getQuality()
	{
		if (type == PrizeType.PRIZE_CARD || type == PrizeType.PRIZE_BEAST)
		{
			return CardSampleManager.Instance.getRoleSampleBySid(exchangeSid).qualityId;
		}
		else if (type == PrizeType.PRIZE_EQUIPMENT)
		{
			return EquipmentSampleManager.Instance.getEquipSampleBySid(exchangeSid).qualityId;
		}
		else if (type == PrizeType.PRIZE_PROP)
		{
			return PropSampleManager.Instance.getPropSampleBySid(exchangeSid).qualityId;
		}
		else if (type == PrizeType.PRIZE_MOUNT)
		{
			return MountsSampleManager.Instance.getMountsSampleBySid(exchangeSid).qualityId;
		}
		return 1;
	}

	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|'); 
		checkLength (strArr.Length, 10);
		
		//strArr[0] is sid  
		//strArr[1] describe
		this.describe = strArr [1];
		//strArr[2] exType
		this.exType = StringKit.toInt (strArr [2]);
		//strArr[2] type
		this.type = StringKit.toInt (strArr [3]); 
		//strArr[3] exchangeSid
		this.exchangeSid = StringKit.toInt (strArr [4]); 
		//strArr[4] num
		this.num = StringKit.toInt (strArr [5]); 
		//strArr[5] times
		this.times = StringKit.toInt (strArr [6]);
		//strArr[6] start
		this.start = StringKit.toInt (strArr [7]);
		//strArr[7] end
		this.end = StringKit.toInt (strArr [8]); 
		parsePremises (strArr [9]);
		parseConditions (strArr [10]);
	}
	
	//解析前置条件
	private void parsePremises (string str)
	{
		string[] strAr=str.Split('^');
		premises = new ExchangePremise[strAr.Length][];
		for(int j=0;j<strAr.Length;j++){
			string[] strArr = strAr[j].Split ('#');
			premises[j]=new ExchangePremise[strArr.Length];
			for(int k=0;k<strArr.Length;k++){
				premises[j][k]=new ExchangePremise(strArr[k]);
			}
		}


	}
	
	//解析兑换消耗条件
	private void parseConditions (string str)
	{
		string[] strArr = str.Split ('^');
		conditions = new ExchangeCondition[strArr.Length][];
		for(int j=0;j<strArr.Length;j++){
			string[] atrar=strArr[j].Split('#');
			conditions[j]=new ExchangeCondition[atrar.Length];
			for (int i = 0; i < atrar.Length; i++) {
				conditions [j][i] = new ExchangeCondition (atrar [i]); 
			}
		}

	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		ExchangeSample dest = destObj as ExchangeSample;
		if (this.premises != null) {
			dest.premises = new ExchangePremise[this.premises.Length][];
			for (int i = 0; i < this.premises.Length; i++){
				dest.premises[i]=new ExchangePremise[this.premises[i].Length];
			for(int j=0;j<this.premises[i].Length;j++){
				dest.premises[i][j]=this.premises[i][j].Clone()as ExchangePremise;
				}
			}//dest.premises [i] = this.premises [i].Clone () as ExchangePremise;
		}
		if (this.conditions != null) {
			dest.conditions = new ExchangeCondition[this.conditions.Length][];
			for (int i = 0; i < this.conditions.Length; i++){
				dest.conditions[i]=new ExchangeCondition[this.premises[i].Length];
			for(int j=0;j<this.conditions[i].Length;j++){
				dest.conditions[i][j]=this.conditions[i][j].Clone()as ExchangeCondition;
			}
		}
	}
}

	public override string ToString()
	{
		string conditionInfo = "";
		for (int i = 0; i < conditions[0].Length; i++)
		{
			conditionInfo += conditions[0][i];
			if (i < conditions[0].Length - 1)
			{
				conditionInfo += "，";
			}
		}
		string premiseInfo = "";
		for (int i = 0; i < premises[0].Length; i++)
		{
			premiseInfo += premises[0][i].describe;
			if (i < premises[0].Length - 1)
			{
				premiseInfo += "，";
			}
		}

		return "条件："+ premiseInfo + " 消耗："+conditionInfo + " 次数：" + times + " 兑换："+ PrizeSample.GetQualityName(getQuality()) + getExhangeItemName() + "x" + num;
	}
}

//兑换条件(消耗)
public class ExchangeCondition:CloneObject
{
	public int costType = 0;//消耗类型 costType
	public int costSid = 0;//消耗品sid
	public int num = 0;//消耗数量
	public ExchangeCondition (string str)
	{
		parse (str);
	}
	
	public string  getName ()
	{
		if (costType == PrizeType.PRIZE_CARD) {
			
			return  CardSampleManager.Instance.getRoleSampleBySid (costSid).name;
			
		} else if (costType == PrizeType.PRIZE_EQUIPMENT) {
 
			return   EquipmentSampleManager.Instance.getEquipSampleBySid (costSid).name;
 
					
		} else if (costType == PrizeType.PRIZE_PROP) {
			return   PropSampleManager .Instance.getPropSampleBySid (costSid).name;
		} else if (costType == PrizeType.PRIZE_MONEY) {
			return   LanguageConfigManager.Instance.getLanguage ("s0049");
		} else if (costType == PrizeType.PRIZE_RMB) {
			return   LanguageConfigManager.Instance.getLanguage ("s0131");
				
		}
		return "none";
	}

	public int getQuality()
	{
		if (costType == PrizeType.PRIZE_CARD || costType == PrizeType.PRIZE_BEAST)
		{
			return CardSampleManager.Instance.getRoleSampleBySid(costSid).qualityId;
		}
		else if (costType == PrizeType.PRIZE_EQUIPMENT)
		{
			return EquipmentSampleManager.Instance.getEquipSampleBySid(costSid).qualityId;
		}
		else if (costType == PrizeType.PRIZE_PROP)
		{
			return PropSampleManager.Instance.getPropSampleBySid(costSid).qualityId;
		}
		return 1;
	}

	private void parse (string str)
	{
		string[] strArr = str.Split ('*');
		if(strArr.Length<3){
			costType=-1;
			costSid=0;
			num=0;
		}else{
			costType = StringKit.toInt (strArr [0]);
			costSid = StringKit.toInt (strArr [1]);
			num = StringKit.toInt (strArr [2]);
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}

	public override string ToString()
	{
		return PrizeSample.GetQualityName(getQuality()) + getName() + "x" + num;
	}
}

//兑换前提条件
public class ExchangePremise:CloneObject
{
	public string type = "";//条件类型
	public int _value = 0;//条件值
	public string describe = "";//前提条件描述
	
	public ExchangePremise (string str)
	{
		parse (str);
	}
	
	private void parse (string str)
	{
		string[] strArr = str.Split ('*');
		type = strArr [0];
		_value = StringKit.toInt (strArr [1]);
		describe = strArr [2];
		
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}


