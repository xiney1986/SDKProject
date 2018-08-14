using System;
 
/**
 * 召唤兽进化实体对象
 * @author longlingquan
 * */
public class BeastEvolve
{
	public BeastEvolve (string str)
	{
		parse (str);
	}
	
	public int[] sids;//召唤兽sid序列
	public int[] exchangeSids;//召唤条件序列
	
	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		sids = new int[strArr.Length];
		exchangeSids = new int[strArr.Length];
		for (int i = 0; i <strArr.Length; i++) {
			string[] strArr2 = strArr [i].Split (',');
			sids [i] = StringKit.toInt (strArr2 [0]);
			exchangeSids [i] = StringKit.toInt (strArr2 [1]);
		}
	}
	
	//获得指定索引召唤兽
	public Card getBeast (int index)
	{
		Card card = StorageManagerment.Instance.getBeastBySid (sids [index]);
		if (card == null)
			return CardManagerment.Instance.createCard (sids [index]);
		return card;
	}

	//获得指定索引召唤兽兑换条件
	public ExchangeSample getExchangeBySids (int sid)
	{
		int index = getIndexBySid (sid);
		return ExchangeSampleManager.Instance.getExchangeSampleBySid (exchangeSids [index]);
	}

	//返回所有前提条件状态文字描述
	public string[][] getAllPremises (BeastEvolve beast)
	{
		ExchangeSample exchange = getExchangeBySids(beast.getNextBeast().sid);
		int max = exchange.premises.Length;
		string[][] strArr = new string[max][];
		for(int m=0;m<exchange.premises.Length;m++){
			strArr[m]=new string[exchange.premises[m].Length];
			for(int n=0;n<exchange.premises[m].Length;n++){
				if (checkPremise (exchange.premises [m][n],beast) == "")
					strArr [m][n] = Colors.GREEN + exchange.premises [m][n].describe + LanguageConfigManager.Instance.getLanguage ("s0098");
				else
					strArr [m][n] = checkPremise (exchange.premises [m][n],beast);
			}
		}
		return strArr;

	} 

	/** 进化列表中的所有召唤兽是否所有前提条件达成 */
	public bool isCheckAllPremises (BeastEvolve beast)
	{
		ExchangeSample exchange = getExchangeBySids(beast.getNextBeast().sid);
		int max = exchange.premises.Length;
		int flag=0;
		for (int i = 0; i < max; i++) {
			for(int j=0;j<exchange.premises[i].Length;j++){
				if (checkPremise (exchange.premises [i][j],beast) != ""){
					flag+=1;
					break;
				}
			}

		}
		if(flag==max)return false;
		return true;
	}

	/** 指定索引召唤兽是否所有前提条件达成 */
	public bool isCheckPremises (BeastEvolve beast)
	{
		ExchangeSample exchange = getExchangeBySids(beast.getNextBeast().sid);
		int max = exchange.premises.Length;
		int flag=0;
		for (int i = 0; i < max; i++) {
			for(int j=0;j<exchange.premises[i].Length;j++){
				if (checkPremise (exchange.premises [i][j],beast) != ""){
					flag+=1;
					break;
				}
			}
			
		}
		if(flag==max)return false;
		return true;
	}

	//检查前提条件 返回 如果都完成 则返回 全部达成 否则返回第一个没达成的条件
	public string checkPremises (BeastEvolve beast)
	{
		ExchangeSample exchange = getExchangeBySids(beast.getNextBeast().sid);
		int max = exchange.premises.Length;
		string nopremiss="";
		int flag=0;
		for (int i = 0; i < max; i++) {
			for(int j=0;j<exchange.premises[i].Length;j++){
				if (checkPremise (exchange.premises [i][j],beast) != ""){
					if(nopremiss=="")nopremiss=checkPremise (exchange.premises [i][j],beast);
					flag+=1;
					break;
				}
			}
			
		}
		if(flag==max)return nopremiss;
		//return LanguageConfigManager.Instance.getLanguage ("s0106");
		return LanguageConfigManager.Instance.getLanguage ("nvShengJingHuaTips");


	}

	//返回单个前提条件结果 如果为""表示已经完成 否则返回未达成说明文字

	public string checkPremise (ExchangePremise premise,BeastEvolve beast)
	{
		if (premise.type == PremiseType.LEVEL) {
			if (UserManager.Instance.self.getUserLevel () < premise._value)
				return  premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
		} 
		else if (premise.type == PremiseType.BSID_MAX) {
			if (beast == null)
				return  premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
			else if (beast.getBeast().getLevel() < premise._value) {
				return  premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
			}
		} else if (premise.type == PremiseType.PARENT) {
			if (!ExchangeManagerment.Instance.isParentComplete (premise._value))
				return  premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
		}
		return "";
	}
	
	//获得当前需要的召唤兽
	public Card getBeast ()
	{
		int max = sids.Length;
		for (int i = 0; i < max; i++) {
			Card card = StorageManagerment.Instance.getBeastBySid (sids [i]);
			if (card != null)
				return card;
		}
		//如果所有召唤兽都没有拥有 则返回第一个召唤兽
		return getBeast (0);
	}

    //获得上一个需要的召唤兽
    public Card getLastBeast()
    {
        //当前一个都没，返回第一个
        Card tmp = getBeast();
        if (tmp.uid == "")
        {
            return tmp;
        }
        else
        {
            int index = getIndexBySid(tmp.sid);
            index -= 1;

            if (index >= sids.Length)
                return getBeast(sids.Length - 1);
            else
                return getBeast(index);
        }
    }
	//获得下一个需要的召唤兽
	public Card getNextBeast ()
	{

		//当前一个都没，返回第一个
		Card tmp = getBeast ();
		if (tmp.uid == "") {
			return tmp;
		} else {	
			int index = getIndexBySid (tmp.sid);	
			index += 1;
			
			if (index >= sids.Length)
				return getBeast (sids.Length - 1);
			else
				return getBeast (index);	
		}
	}

	/// <summary>
	/// 获得女神初始形态
	/// </summary>
	public Card getFristBeast ()
	{
		return getBeast (0);
	}

	public  bool isEndBeast ()
	{	
		if (getIndexBySid (getBeast ().sid) >= sids.Length - 1) {
			return true;
		} else {
			return false;
		}
		
	}
	
	//获得当前指定sid召唤兽索引
	public int getIndexBySid (int sid)
	{
		int max = sids.Length;
		for (int i = 0; i < max; i++) {
			if (sid == sids [i])
				return i;
		}
		return -1;
	}
	
	//判断是否存在指定sid
	public bool isExist (int sid)
	{
		for (int i = 0; i < sids.Length; i++) {
			if (sids [i] == sid)
				return true;
		}
		return false;
	} 
	
	/** 判断当前进化实体中 是否已经有召唤兽被召唤 */
	public bool isExist ()
	{
		int max = sids.Length;
		for (int i = 0; i < max; i++) {
			return StorageManagerment.Instance.getBeastBySid (sids [i]) != null;
		}
		return false;
	}

	/** 判断当前进化实体中 是否已经有召唤兽被召唤 */
	public bool isAllExist ()
	{
		int max = sids.Length;
		for (int i = 0; i < max; i++) {
			bool b = StorageManagerment.Instance.getBeastBySid (sids [i]) != null;
			if (b) {
				return true;
			}
		}
		return false;
	}
} 

