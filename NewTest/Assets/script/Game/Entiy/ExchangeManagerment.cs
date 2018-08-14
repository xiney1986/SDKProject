using System;
using System.Collections.Generic;
 
/**
 * 兑换管理器
 * 所有兑换信息
 * @author longlingquan
 * */
public class ExchangeManagerment
{
	public ExchangeManagerment ()
	{
		initList ();
	}
	
	private List<int> completeSids;
	private List<Exchange> list;
	private int completeNum = 0;//能够兑换的条目数量
	private string showLabel = "";
	
	//缓存下可兑换列表, 目前主要作用在活动主界面每次都要重新取,当更新活动条目后重新缓存
	//private List<Exchange> mCacheCanUseList; 
	private Dictionary<int, List<Exchange>> mCacheListDic;
 
	//单例
	public static ExchangeManagerment Instance {

		get{ return SingleManager.Instance.getObj ("ExchangeManagerment") as ExchangeManagerment;}
	}
	
	//初始化兑换条目集合
	private void initList ()
	{
		mCacheListDic = new Dictionary<int, List<Exchange>> ();
		list = new List<Exchange> ();
		int[] sids = ExchangeSampleManager.Instance.getAllExchange ();
		int max = sids.Length;
		for (int i = 0; i < max; i++) {
			ExchangeSample sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (sids [i]);
			Exchange ex = new Exchange (sample.sid, 0, sample.exType);
            if (sample.exType == ExchangeType.WNCARD)//红色万能卡的兑换不显示在兑换页面
                continue;
			list.Add (ex);
			if (sample.exType == 2) {
				if (!mCacheListDic.ContainsKey (sample.exType))
					mCacheListDic[sample.exType] = new List<Exchange> ();
				mCacheListDic[sample.exType].Add (ex);
			}
		}
	}
	
	//计算能够兑换的条目数量
	public void countCompleteNum ()
	{
		completeNum = 0;
		int max = list.Count;
		for (int i = 0; i < max; i++) {
			if (isCheckPremises (list [i].getExchangeSample ()) && isCheckConditions (list [i].getExchangeSample ()))
				completeNum++;
		}
	}
	
	//获得能够兑换的条目数量
	public int getCompleteNum ()
	{
		return completeNum;
	}
	 
	
	//更新兑换条目
	public void updateExchange (int sid, int num)
	{
		int max = list.Count;
		for (int i = 0; i < max; i++) {
			if (list [i].sid == sid) {
				list [i].setNum (num); 
				setCompleteSid (sid);
			}
		}
	}
	
	public void addExchange (int sid, int num)
	{
		int max = list.Count;
		for (int i = 0; i < max; i++) {
			if (list [i].sid == sid) {
				list [i].addNum (num); 
				setCompleteSid (sid);
			}
		}
	}
	
	//设置兑换过的集合(兑换过至少一次)
	public void setCompleteSid (int sid)
	{
		if (completeSids == null)
			completeSids = new List<int> ();
		if (completeSids.IndexOf (sid) == -1)
			completeSids.Add (sid);
	}
	
	//获得所有兑换条目
	public List<Exchange> getAllExchanges ()
	{
		return list;
	}
	
	//获得 指定兑换条目
	public Exchange getExchangesByID (int sid)
	{
		foreach (Exchange each in list) {
			if (each.sid == sid) {
				return each;
			}
		}
		return null;
	}
	
	//获得所有可用的兑换条目
	public List<Exchange> getCanUseExchanges (int type)
	{
		List<Exchange> tmps = new List<Exchange> ();

		foreach (Exchange each in list) {
			if (each.exType == type && each.isShow ()) {
				tmps.Add (each);
			}
			
		}
		return getSortExchanges (tmps);
	}

	/** 获得所有可用的兑换条目(卡片碎片) */
	public List<Exchange> getCanUseExchangesCardScrap ()
	{
		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		foreach (Exchange each in list) {
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (each.exType == ExchangeType.CARDSCRAP && each.isShow () && sample.type == PrizeType.PRIZE_CARD) {
				tmps.Add (each);
			}
		}
		return tmps;
	}
    /**获得可以兑换的红色万能卡*/
    public List<Exchange> getCanExchangeWNCard() {
        List<Exchange> tmps = new List<Exchange>();
        int[] sids = ExchangeSampleManager.Instance.getAllExchange();
        int max = sids.Length;
        for (int i = 0; i < max; i++) {
            ExchangeSample sample = ExchangeSampleManager.Instance.getExchangeSampleBySid(sids[i]);
            Exchange ex = new Exchange(sample.sid, 0, sample.exType);
            if (sample.exType == ExchangeType.WNCARD)
                tmps.Add(ex);
        }
        return tmps;
    }
	/** 获得所有可用的兑换条目(卡装备碎片) */
	public List<Exchange> getCanUseExchangesEquipScrap ()
	{
		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		foreach (Exchange each in list) {
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (each.exType == ExchangeType.EQUIPSCRAP && each.isShow () && sample.type == PrizeType.PRIZE_EQUIPMENT) {
				tmps.Add (each);
			}
		}
		return tmps;
	}
    /** 获得所有可用的兑换条目(秘宝碎片) */
    public List<Exchange> getCanUseExchangesWeaponScrap() {
        List<Exchange> tmps = new List<Exchange>();
        ExchangeSample sample;
        foreach (Exchange each in list) {
            sample = ExchangeSampleManager.Instance.getExchangeSampleBySid(each.sid);
            if (each.exType == ExchangeType.MAGICWEAPON && each.isShow() && sample.type == PrizeType.PRIZE_MAGIC_WEAPON) {
                tmps.Add(each);
            }
        }
        return tmps;
    }

	//获得所有可用的兑换条目(卡片)
	public List<Exchange> getCanUseExchangesCard (int type)
	{
		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		foreach (Exchange each in list) {
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (each.exType == type && each.isShow () && sample.type == PrizeType.PRIZE_CARD) {
				tmps.Add (each);
			}
		}

		//增加卡片碎片
		PropSample propSample;
		foreach (Exchange each in list) {
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (each.exType != type && each.isShow () || sample.type != PrizeType.PRIZE_PROP)
				continue;
			propSample = PropSampleManager.Instance.getPropSampleBySid (sample.exchangeSid);
			if (propSample.type == PropType.PROP_TYPE_CARDSCRAP && each.isShow())
				tmps.Add (each);
		}
//		for (int i=0; i<list.Count; i++) {
//			tmp = list [i];
//			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (tmp.sid);
//			propSample = PropSampleManager.Instance.getPropSampleBySid (sample.exchangeSid);
//			if (propSample.type == PropType.PROP_TYPE_CARDSCRAP)
//				tmps.Add (tmp);
//		}

		return getSortExchanges (tmps);
	}
	//获得所有兑换条目(坐骑)
	public List<Exchange> getAllExchangesMount (int type) {
		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		foreach (Exchange each in list) {
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (each.exType == type && sample.type == PrizeType.PRIZE_MOUNT) {
				tmps.Add (each);
			}
		}
		//getSortMountList(tmps);
		return getSortExchanges (tmps);
	}

	//获得所有可用的兑换条目(道具)
	public List<Exchange> getCanUseExchangesProp (int type)
	{
		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		PropSample propSample;
		foreach (Exchange each in list) {
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (each.exType == type && each.isShow () && sample.type == PrizeType.PRIZE_PROP) {
				propSample = PropSampleManager.Instance.getPropSampleBySid(sample.exchangeSid);
				if(propSample.type != PropType.PROP_TYPE_CARDSCRAP)
					tmps.Add (each);
			}
		}
		return getSortExchanges (tmps);
	}
	//获得所有可用的兑换条目(装备)
	public List<Exchange> getCanUseExchangesEquip (int type)
	{
		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		foreach (Exchange each in list) {
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (each.exType == type && each.isShow () && sample.type == PrizeType.PRIZE_EQUIPMENT) {
				tmps.Add (each);
			}
		}
		return getSortExchanges (tmps);
	}
	//获得所有可用的兑换条目
	public List<Exchange> getCanUseExchanges (int[] sids, int type)
	{
		return getCanUseExchanges (sids,type,true);
	}
	//获得所有可用的兑换条目
	public List<Exchange> getCanUseExchanges (int[] sids, int type,bool isSort)
	{
		List<Exchange> list = this.list;
		if (mCacheListDic.ContainsKey(type))
			list = mCacheListDic[type];

		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		int now = ServerTimeKit.getSecondTime ();
		for (int i = 0; i < sids.Length; i++) {
			foreach (Exchange each in list) {
				sample = each.getExchangeSample();
				if (each.exType == type && each.sid == sids [i]) {
					if(each.isTimeLimit()) //是限时低缓
					{
						if(!each.isTimeout(now)){
							tmps.Add (each);
						}
					}else{
						//不是限时兑换，领取要隐藏
						if(each.isShow())
							tmps.Add (each);
					}
				}
			}
		}
		if(isSort) return getSortExchanges (tmps);
		return tmps;
	}
	/// <summary>
	/// 获得指定时间有效的Exchange
	/// </summary>
	/// <param name="sids">Sids</param>
	/// <param name="type">类型</param>
	/// <param name="now">时间</param>
	public List<Exchange> getValidExchangesByTime (int[] sids, int type,int now)
	{
		List<Exchange> list = getCanUseExchanges (sids, type, false);
		List<Exchange> tmps = new List<Exchange> ();
		ExchangeSample sample;
		Exchange each;
		for (int i=0; i < list.Count; i++) {
			each = list[i];
			if(each==null) continue;
			sample = ExchangeSampleManager.Instance.getExchangeSampleBySid (each.sid);
			if (!(!isCheckPremises (sample) || !isCheckConditions (sample) || each.checkTimeOut (now) || (sample.times != 0 && each.getNum () >= sample.times)))
				tmps.Add (list[i]);
		}
		return tmps;
	}

	//排序
	public List<Exchange> getSortExchanges (List<Exchange> tmps)
	{
		List<Exchange> sample1 = new List<Exchange> ();//可兑换
		List<Exchange> sample2 = new List<Exchange> ();//前置达成
		List<Exchange> sample3 = new List<Exchange> ();//前置限制
		ExchangeSample sample;
		for (int i = 0; i < tmps.Count; i++) {
			if (isCheckPremises (tmps [i].getExchangeSample ())) {
				if (checkCondition (tmps [i]))
					sample1.Add (tmps [i]);
				else
					sample2.Add (tmps [i]);
			} else {
				sample3.Add (tmps [i]);
			}
		}
		ListKit.AddRange (sample2, sample3);
		ListKit.AddRange (sample1, sample2);
		return sample1;
	}

	/** 根据是否可以兑换排序 */
	public List<Exchange> getSortByCanExchange (List<Exchange> tmps)
	{
		List<Exchange> sample1 = new List<Exchange> ();//可兑换
		List<Exchange> sample2 = new List<Exchange> ();//前置达成
		ExchangeSample sample;
		for (int i = 0; i < tmps.Count; i++) {
			if (checkCondition (tmps [i]))
				sample1.Add (tmps [i]);
			else
				sample2.Add (tmps [i]);
		}
		ListKit.AddRange (sample1, sample2);
		return sample1;
	}


	//获得可兑换数量
	public int getExchangeCount (int index, int type)
	{
		List<Exchange> exchangeList = null;
		int time = ServerTimeKit.getSecondTime ();
		switch (index) {
		case 0:
			exchangeList = getCanUseExchangesCard (type);
			break;
		case 1:
			exchangeList = getCanUseExchangesEquip (type);
			break;
		case 2:
			exchangeList = getCanUseExchangesProp (type);
			break; 
		}
		if (exchangeList == null)
			return 0;
		int num = 0;
		//ExchangeSample sample;
		for (int i = 0; i < exchangeList.Count; ++i) {
			if (checkCondition (exchangeList [i]))
				num ++;
		}
		return num;
	}
	/// <summary>
	/// 校验第一个兑换条件(包含--前置条件和兑换条件)
	/// </summary>
	/// <param name="exchange">兑换对象</param>
	public bool checkCondition (Exchange exchange)
	{
		int time = ServerTimeKit.getSecondTime ();
		if (exchange.getStartTime () > time && exchange.getStartTime () > 0)
			return false;
		if (exchange.getEndTime () < time && exchange.getEndTime () > 0)
			return false;
		if (isCheckPremises (exchange.getExchangeSample ()) && isCheckConditions (exchange.getExchangeSample ()))
			return true;
		return false;
	}
	/// <summary>
	/// 校验所有兑换条件(包含--前置条件和兑换条件)
	/// </summary>
	/// <param name="exchange">兑换对象</param>
	public bool checkConditionByAll (Exchange exchange)
	{
		int time = ServerTimeKit.getSecondTime ();
		if (exchange.getStartTime () > time && exchange.getStartTime () > 0)
			return false;
		if (exchange.getEndTime () < time && exchange.getEndTime () > 0)
			return false;
		if (isCheckPremisesByAll (exchange.getExchangeSample ()) && isCheckConditionsByAll (exchange.getExchangeSample ()))
			return true;
		return false;
	}
	//检查前提条件 返回 如果都完成 则返回 全部达成 否则返回第一个没达成的条件
	public string checkPremises (ExchangeSample exchange) { 
		int max = exchange.premises.Length;
		string nopremiss="";
		int flag=0;
		for (int i = 0; i < max; i++) {
			for(int j=0;j<exchange.premises[i].Length;j++){
				if (checkPremise (exchange.premises [i][j]) != ""){
					if(nopremiss=="")nopremiss=checkPremise (exchange.premises [i][j]);
					flag+=1;
					break;
				}
			}
		}
		if(flag==max)return nopremiss;
		return LanguageConfigManager.Instance.getLanguage ("s0106");;
	}
	/// <summary>
	/// 校验第1个前置条件
	/// </summary>
	/// <param name="exchange">兑换模板</param>
	public bool isCheckPremises(ExchangeSample exchange) {
		return isCheckPremises(exchange,0);
	}
	/// <summary>
	/// 校验指定下标的前置条件
	/// </summary>
	/// <param name="exchange">兑换模板</param>
	/// <param name="index">前置条件下标</param>
	public bool isCheckPremises (ExchangeSample exchange,int index) {
		if(exchange.premises==null||index>=exchange.premises.Length)
			return true;
		int max = exchange.premises[index].Length;
		for (int i = 0; i < max; i++) {
			if (checkPremise (exchange.premises[index][i]) != "")
				return false;
		}
		return true;
	}
	/// <summary>
	/// 校验单组(or的关系)兑换前提条件是否有一个满足---true=有一组满足,false=都不满足
	/// <param name="exchange">兑换模板</param>
	/// </summary>
	public bool isCheckPremisesByAll(ExchangeSample exchange) {
		if(exchange.premises==null) return true;
		int len=exchange.premises.Length;
		bool isCheck=false;
		int i=0;
		for(;i<len;i++) {
			isCheck=isCheckPremises(exchange,i);
			if(isCheck) {
				break;
			}
		}
		if(i!=len)
			return true;
		return false;
	}
	//得到满足的是哪个条件
	public int isCheckPremisesForMount(ExchangeSample exchange) { 
		int max = exchange.premises.Length;
		int flag=0;
		int iindex=0;
		for (int i = 0; i < max; i++) {
			for(int j=0;j<exchange.premises[i].Length;j++){
				if (checkPremise (exchange.premises [i][j]) != ""){
					flag+=1;
					break;
				}
			}
		}
		return flag;
	}
	/// <summary>
	/// 校验第一个兑换消耗条件是否满足
	/// </summary>
	/// <param name="exchange">兑换对象</param>
	public bool isCheckConditions(ExchangeSample exchange){
		return isCheckConditions(exchange,0);
	}
	/// <summary>
	/// 校验指定下标的兑换消耗条件是否满足
	/// </summary>
	/// <param name="exchange">兑换对象</param>
	/// <param name="index">兑换下标</param>
	public bool isCheckConditions (ExchangeSample exchange ,int index) { 
		if(exchange.conditions==null||index>=exchange.conditions.Length)
			return true;
		int max = exchange.conditions[index].Length;
		for (int i = 0; i < max; i++) {
			if (exchange.conditions [index][i].costType == PrizeType.PRIZE_RMB) {
				if (UserManager.Instance.self.getRMB () < exchange.conditions [index][i].num)
					return false;
			} else if (exchange.conditions [index][i].costType == PrizeType.PRIZE_MONEY) {
				if (UserManager.Instance.self.getMoney () < exchange.conditions[index] [i].num)
					return false;
			} else if (exchange.conditions[index] [i].costType == PrizeType.PRIZE_EQUIPMENT) {
				if (StorageManagerment.Instance.getEquipsBySid (exchange.conditions [index][i].costSid).Count < exchange.conditions[index] [i].num)
					return false;
			} else if (exchange.conditions[index] [i].costType == PrizeType.PRIZE_CARD) {
				if (StorageManagerment.Instance.getNoUseRolesBySid (exchange.conditions [index][i].costSid).Count < exchange.conditions [index][i].num)
					return false;
			} else if (exchange.conditions[index] [i].costType == PrizeType.PRIZE_PROP) {
				Prop p = StorageManagerment.Instance.getProp (exchange.conditions [index][i].costSid);
				if (p == null || p.getNum () < exchange.conditions[index] [i].num)
					return false;
			} else if (exchange.conditions[index] [i].costType == PrizeType.PRIZE_MERIT) {
				if (UserManager.Instance.self.merit < exchange.conditions [index][i].num)
					return false;
			}
		}
		return true;
	}
	/// <summary>
	/// 校验指定下标的兑换消耗条件是否满足
	/// </summary>
	/// <param name="exchange">兑换对象</param>
	/// <param name="index">兑换下标</param>
	public bool isCheckConditions (ExchangeSample exchange ,int index,int i) { 
		if(exchange.conditions==null||index>=exchange.conditions.Length)
			return true;
		int max = exchange.conditions[index].Length;
		if (exchange.conditions [index] [i].costType == PrizeType.PRIZE_RMB) {
			if (UserManager.Instance.self.getRMB () < exchange.conditions [index] [i].num)
				return false;
		}
		else if (exchange.conditions [index] [i].costType == PrizeType.PRIZE_MONEY) {
			if (UserManager.Instance.self.getMoney () < exchange.conditions [index] [i].num)
				return false;
		}
		else if (exchange.conditions [index] [i].costType == PrizeType.PRIZE_EQUIPMENT) {
			if (StorageManagerment.Instance.getEquipsBySid (exchange.conditions [index] [i].costSid).Count < exchange.conditions [index] [i].num)
				return false;
		}
		else if (exchange.conditions [index] [i].costType == PrizeType.PRIZE_CARD) {
			if (StorageManagerment.Instance.getNoUseRolesBySid (exchange.conditions [index] [i].costSid).Count < exchange.conditions [index] [i].num)
				return false;
		}
		else if (exchange.conditions [index] [i].costType == PrizeType.PRIZE_PROP) {
			Prop p = StorageManagerment.Instance.getProp (exchange.conditions [index] [i].costSid);
			if (p == null || p.getNum () < exchange.conditions [index] [i].num)
				return false;
		}
		else if (exchange.conditions [index] [i].costType == PrizeType.PRIZE_MERIT) {
			if (UserManager.Instance.self.merit < exchange.conditions [index] [i].num)
				return false;
		}
		return true;
	}
	/// <summary>
	/// 校验单组(or的关系)兑换消耗条件是否有一个满足---true=有一组满足,false=都不满足
	/// </summary>
	/// <param name="exchange">兑换模板</param>
	public bool isCheckConditionsByAll (ExchangeSample exchange) {
		if(exchange.conditions==null) return true;
		int len=exchange.conditions.Length;
		bool isCheck=false;
		int i=0;
		for(;i<len;i++) {
			isCheck=isCheckConditions(exchange,i);
			if(isCheck)
				break;
		}
		if(i!=len)
			return true;
		return false;
	} 
	/// <summary>
	/// 拿到兑换条件的文字描述
	/// </summary>
	/// <returns>The conditions desc.</returns>
	public string[] getConditionsDesc(ExchangeSample exchange,int index){
		if(index>0&&exchange.conditions.Length<2)return new string[0];
		int max = exchange.conditions[index].Length;
		//string[] strArr = new string[max];
		List<string> listtt=new List<string>();
		bool isGreen=true,isProp=false;
		string str="";
		for (int i = 0; i < max; i++) {
			isGreen=true;
			isProp=false;
			str="";
			if (exchange.conditions [index][i].costType == PrizeType.PRIZE_RMB) {
				if (UserManager.Instance.self.getRMB () < exchange.conditions [index][i].num)isGreen=false;
				str=LanguageConfigManager.Instance.getLanguage("s0048")+"X"+exchange.conditions [index][i].num;
			} else if (exchange.conditions [index][i].costType == PrizeType.PRIZE_MONEY) {
				if (UserManager.Instance.self.getMoney () < exchange.conditions[index] [i].num)isGreen=false;
				str=LanguageConfigManager.Instance.getLanguage("s0049")+"X"+exchange.conditions [index][i].num;
			} else if (exchange.conditions[index] [i].costType == PrizeType.PRIZE_EQUIPMENT) {
				if (StorageManagerment.Instance.getEquipsBySid (exchange.conditions [index][i].costSid).Count < exchange.conditions[index] [i].num)isGreen=false;
				str=EquipManagerment.Instance.createEquip(exchange.conditions [index][i].costSid).getName()+"X"+exchange.conditions [index][i].num;
			} else if (exchange.conditions[index] [i].costType == PrizeType.PRIZE_CARD) {
				if (StorageManagerment.Instance.getNoUseRolesBySid (exchange.conditions [index][i].costSid).Count < exchange.conditions [index][i].num)isGreen=false;
				str=CardManagerment.Instance.createCard(exchange.conditions [index][i].costSid).getName()+"X"+exchange.conditions [index][i].num;
			} else if (exchange.conditions[index] [i].costType == PrizeType.PRIZE_PROP) {
				Prop p = StorageManagerment.Instance.getProp (exchange.conditions [index][i].costSid);
				if (p == null || p.getNum () < exchange.conditions[index] [i].num)isGreen=false;
				str=PropManagerment.Instance.createProp(exchange.conditions [index][i].costSid).getName()+"X"+exchange.conditions[index] [i].num;
				isProp=true;
			}
			if(str!=""){
				if(isGreen){
					str=Colors.GRENN+str+LanguageConfigManager.Instance.getLanguage ("s0098");
				}else{
					str=Colors.REDD+str+LanguageConfigManager.Instance.getLanguage ("s0099");
				}
				if(isProp){
					listtt.Add("[u]"+str);
				}else if(str!=""){
					listtt.Add(str);
				}
			}
		} 
		return listtt.ToArray();
	}
	/// <summary>
	/// 除了坐骑之外的其他兑换数量走这里 默认为0
	/// </summary>
	public int getCanExchangeNum(Exchange exchange){
		return getCanExchangeNum(exchange,0);
	}
	
	//最大可兑换数量
	public int getCanExchangeNum (Exchange exchange, int index)
	{
		ExchangeSample sample = exchange.getExchangeSample ();
		int max = sample.conditions[index].Length;
		int[] count = new int[max];
		for (int i = 0; i < max; i++) {
			if (sample.conditions[index] [i].costType == PrizeType.PRIZE_RMB) {
				count [i] = UserManager.Instance.self.getRMB () / sample.conditions[index] [i].num;
			} else if (sample.conditions[index] [i].costType == PrizeType.PRIZE_MONEY) {
				count [i] = UserManager.Instance.self.getMoney () / sample.conditions[index] [i].num;
			} else if (sample.conditions[index] [i].costType == PrizeType.PRIZE_EQUIPMENT) {
				count [i] = StorageManagerment.Instance.getEquipsBySid (sample.conditions[index] [i].costSid).Count / sample.conditions[index] [i].num;
			} else if (sample.conditions [index][i].costType == PrizeType.PRIZE_CARD) {
				count [i] = StorageManagerment.Instance.getNoUseRolesBySid (sample.conditions [index][i].costSid).Count / sample.conditions [index][i].num;
			} else if (sample.conditions[index] [i].costType == PrizeType.PRIZE_PROP) {
				Prop p = StorageManagerment.Instance.getProp (sample.conditions[index] [i].costSid);
				if (p != null) {
					count [i] = p.getNum () / sample.conditions [index][i].num;
				} else {
					count [i] = 0;
				}
			}
		}
		
		//木桶原理~
		int last = count [0];
		foreach (int each in count) {
			if (each < last)
				last = each;
		}
		
		//和最大的可兑换数比一下
		if (last > exchange.getLastNum ()) {
			return exchange.getLastNum ();
		} else {
			return last;
		}
		
	}
	/// <summary>
	/// 返回第一个前提条件状态文字描述
	/// </summary>
	/// <param name="exchange">Exchange.</param>
	public string[] getAllPremises (ExchangeSample exchange){
		return getAllPremises(exchange,0);
	}
	/// <summary>
	/// 返回指定下标前提条件状态文字描述
	/// </summary>
	/// <param name="exchange">Exchange.</param>
	/// <param name="index">Index.</param>
	public string[] getAllPremises (ExchangeSample exchange,int index)
	{
		int max = exchange.premises[index].Length;
		string[] strArr = new string[max];
		for (int i = 0; i < max; i++) {
			if (checkPremise (exchange.premises [index][i]) == "")
				strArr [i] = Colors.GREEN + exchange.premises [index][i].describe + LanguageConfigManager.Instance.getLanguage ("s0098");
			else
				strArr [i] = checkPremise (exchange.premises[index] [i]);
		} 
		return strArr;
	}
	//返回单个前提条件结果 如果为""表示已经完成 否则返回未达成说明文字
	public string checkPremise (ExchangePremise premise) {
		if (premise.type == PremiseType.BSID_MAX) {
			Card card = StorageManagerment.Instance.getBeastBySid (premise._value);
			if (card == null)
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
			else if (card.isMaxLevel () == false) {
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
			}
		} else if (premise.type == PremiseType.LEVEL) {
			Card card = StorageManagerment.Instance.getBeastBySid (premise._value);
			if (UserManager.Instance.self.getUserLevel () < premise ._value)
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
		} else if (premise.type == PremiseType.PARENT) {
			if (!isParentComplete (premise._value))
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
		} else if (premise.type == PremiseType.PVP) {
			if (UserManager.Instance.self.getActiveScore () < premise._value)
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
		} else if (premise.type == PremiseType.VIP_LEVEL) {
			if (UserManager.Instance.self.getVipLevel () < premise._value) {
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
			}
		}else if(premise.type==PremiseType.RIDE){
			if(MountsManagerment.Instance.getMountsLevel()<premise._value){
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
			}
		}else if (premise.type == PremiseType.FRIENDS_NUM) {
			return "friends num";
		}else if (premise.type==PremiseType.HAVEMOUNT){
			if (!isMountComplete (premise._value))
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
		}else if(premise.type==PremiseType.HAVE_GUIDE_SORCE){//公会战胜利次数
			if(GuildManagerment.Instance.getWunNum()<premise._value)
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
		}else if(premise.type==PremiseType.GUIDE_INDEX){//公会贡献榜
			if(GuildManagerment.Instance.getGongxuanIndex()<premise._value){
				return premise.describe + LanguageConfigManager.Instance.getLanguage ("s0099");
			}
		}
		return "";

	}
	public bool isMountComplete(int sid){
		if(MountsManagerment.Instance.getMountsBySid(sid)!=null)return true;
		return false;
		
	}
	//前置兑换任务是否完成
	public bool isParentComplete (int sid)
	{
		if (completeSids == null || completeSids.Count < 1)
			return false;
		int max = completeSids.Count;
		for (int i = 0; i < max; i++) {
			if (completeSids [i] == sid)
				return true;
		}
		return false;
	}

	public bool testTime (Exchange ex, int currentTime)
	{
		bool isOk = false;
		if (ex.getStartTime () == 0 && ex.getEndTime () == 0) {
			isOk = true;
		}
		//差多久开始
		else if (ex.getStartTime () > currentTime && ex.getStartTime () > 0) {	
			isOk = false;
		}
		//过期移除
		else if (ex.getEndTime () < currentTime && ex.getEndTime () > 0) {
			isOk = false;
		} else if (ex.getStartTime () == 0 && ex.getEndTime () > 0 && currentTime < ex.getEndTime ()) {
			isOk = true;
		} else if (ex.getStartTime () > 0 && ex.getEndTime () == 0 && currentTime > ex.getStartTime ()) {
			isOk = true;
		} else if (ex.getStartTime () > 0 && ex.getEndTime () > 0 && currentTime > ex.getStartTime () && currentTime < ex.getEndTime ()) {
			isOk = true;
		}
		return isOk;
	}
	
} 

