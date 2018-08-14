using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 进化管理器
 * @author 陈世惟
 * */
public class EvolutionManagerment : SampleConfigManager
{
	private static EvolutionManagerment instance;

	public EvolutionManagerment ()
	{
		base.readConfig (ConfigGlobal.CONFIG_EVOLUTION);
	}

	public static EvolutionManagerment Instance {
		get {
			if (instance == null)
				instance = new EvolutionManagerment ();
			return instance;
		}
	}

	//解析模板数据
	public override void parseSample (int sid)
	{
		EvolutionSample sample = new EvolutionSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

	/** 获取指定(类型)进化信息 */
	public EvolutionSample getEvoInfoByType (int type)
	{
		if (!isSampleExist (type))
			createSample (type); 
		return samples [type] as EvolutionSample;
	}

	/** 获取指定(类型)进化信息 */
	public EvolutionSample getEvoInfoByType (Card card)
	{
		return getEvoInfoByType (card.getEvolveNextSid ());
	}

	/** 是否已进化到极限次数 */
	public bool isMaxEvoLevel (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);
        if (info == null)
            return true;
	    if (card.isMainCard())
	    {
	        return (card.getEvoTimes() >= info.getMaxEvoLevel());

	    }
        long[] exps = EXPSampleManager.Instance.getEXPSampleBySid(79).getExps();
        int index = card.getEvoTimes();
        for (int i = 0; i < exps.Length-1; i++) {
            if (exps[i] <= card.getEvoTimes()&&exps[i+1]>card.getEvoTimes()) index = i + 1;
        }
        return (index >= info.getMaxEvoLevel());
	}

	/** 获取进化极限次数 */
	public int getMaxLevel (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);
		if (info == null)
			return 0;
        if (card.isMainCard()) return info.getMaxEvoLevel();
        long[] exp = EXPSampleManager.Instance.getEXPSampleBySid(79).getExps();
        return (int)exp[info.getMaxEvoLevel() - 1 >= exp.Length ? exp.Length - 1 : info.getMaxEvoLevel() - 1];
		//else
		//	
	}

    public int getMaxLevell(Card card)
    {
        EvolutionSample info = getEvoInfoByType(card);
        if (info == null)
            return 0;
        return info.getMaxEvoLevel();
    }

	/** 是否已满足进化的条件(不带提示) */
	public bool isCanEvo (Card card)
	{	
		int cardLevel = getCardLevel (card);
		int needPlayerLv = getPlayerLevel (card);
		int plaverLv = UserManager.Instance.self.getUserLevel ();
		
		if (card == null)
			return false;
		if (isMaxEvoLevel (card))
			return false;
		if (cardLevel > card.getLevel ())
			return false;
		if (needPlayerLv > plaverLv)
			return false;
		if (getNeedMoney (card) > UserManager.Instance.self.getMoney ())
			return false;
		
		EvolutionCondition[] evoCon = getEvoCon (card);
		if (evoCon != null) {
			Prop showProp;
			for (int i = 0; i < evoCon.Length; i++) {
				showProp = StorageManagerment.Instance.getProp (evoCon [i].costSid);
				return showProp == null ? false : evoCon [i].num <= showProp.getNum ();
			}
		}
		
		return true;
	}

	/** 是否已满足进化的条件(带提示) */
	public bool isCanEvoByString (Card card)
	{	
		int cardLevel = getCardLevel (card);
		int needPlayerLv = getPlayerLevel (card);
		int plaverLv = UserManager.Instance.self.getUserLevel ();
		
		if (card == null)
			return false;
		if (isMaxEvoLevel (card)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Evo04", cardLevel.ToString (), card.getLevel ().ToString ()));
			return false;
		}
		if (cardLevel > card.getLevel ()) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Evo01", cardLevel.ToString (), card.getLevel ().ToString ()));
			return false;
		}
		if (needPlayerLv > plaverLv) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Evo02", needPlayerLv.ToString (), plaverLv.ToString ()));
			return false;
		}
		if (getNeedMoney (card) > UserManager.Instance.self.getMoney ()) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Evo06"));
			return false;
		}

		EvolutionCondition[] evoCon = getEvoCon (card);

		if (evoCon != null) {

			Prop showProp;
			Prop prop;

			for (int i = 0; i < evoCon.Length; i++) {
				showProp = StorageManagerment.Instance.getProp (evoCon [i].costSid);
				bool num = showProp == null ? false : (evoCon [i].num > showProp.getNum () ? false : true);
				if (!num) {
					prop = PropManagerment.Instance.createProp (evoCon [i].costSid);
					UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
						string goTo01 = LanguageConfigManager.Instance.getLanguage ("shop_mystical");
						string goTo02 = LanguageConfigManager.Instance.getLanguage ("s0258");
						string des = LanguageConfigManager.Instance.getLanguage ("Evo07", prop.getName ());
						win.initWindow(2,goTo01,goTo02,des,true,msgCallBack,MessageAlignType.center);
					});
					return false;
				}
			}
		}
		
		return true;
	}

	private void msgCallBack (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			UiManager.Instance.openWindow<ShopWindow> ((win) => {
				win.setTitle (LanguageConfigManager.Instance.getLanguage ("shop_mystical"));
				win.init (ShopWindow.TAP_MYSTICAL_CONTENT);
			});
		} else if(msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			UiManager.Instance.openWindow<ExChangeWindow> ();
		}
	}

	/** 获取所有卡片  */
	private List<Card> getAllCardList ()
	{
		ArrayList list = StorageManagerment.Instance.getAllRole ();
		List<Card> cardList = new List<Card> ();
		
		for (int i = 0; i < list.Count; i++) {
			cardList.Add ((Card)list [i]);
		}
		return cardList;
	}

	/** 普通卡是否有副卡或万能卡支持进化 */
	public bool isGeneralCardsCanEvolution (Card card)
	{
		if (isCanEvo (card)) {
			if (getCardByQuilty (card) != null) {
				return true;
			} else {
				List<Card> allCards = getAllCardList ();

				for (int i = 0; i < allCards.Count; i++) {
					if ((!ArmyManager.Instance.getAllArmyPlayersIds ().Contains (allCards [i].uid) && !ArmyManager.Instance.getAllArmyAlternateIds ().Contains (allCards [i].uid))) {
						if (allCards [i].getEvolveNextSid () == card.getEvolveNextSid () && allCards [i].uid != card.uid && !EvolutionManagerment.Instance.isMaxEvoLevel (allCards [i]))
							return true;
					}
				}
				return false;
			}
		} else {
			return false;
		}
	}

    /// <summary>
    /// 获取第一张可用于进化的副卡 
    /// </summary>
	public Card getFoodCardForEvo (Card card)
	{
		if (isCanEvo (card)) {
			List<Card> allCards = getAllCardList ();
			bool has = false;
			string uid;
			for (int i = 0; i < allCards.Count; i++) {
				uid = allCards [i].uid;
				if (!ArmyManager.Instance.getAllArmyPlayersIds ().Contains (uid) && !ArmyManager.Instance.getAllArmyAlternateIds ().Contains (uid) && !ArmyManager.Instance.isExistByActiveEditArmy (uid)) {
                    /** 剔除有进化等级的卡,剔除类型不符的卡,剔除自身 */
                    if (allCards[i].getEvolveNextSid() == card.getEvolveNextSid() && allCards[i].uid != card.uid && allCards[i].getEvoLevel() == 0)
						return allCards [i];
				}
			}
			return null;
		} else {
			return null;
		}
	}

 
    /// <summary>
    /// 获取可用于超进化的所有副卡
    /// </summary>
    public List<Card> getFoodCardsForSuperEvo(Card card)
    {
        if (isCanEvo(card))
        {
            List<Card> allCards = getAllCardList();
            List<Card> matchCards = new List<Card>();
            bool has = false;
            string uid;
            for (int i = 0; i < allCards.Count; i++)
            {
                uid = allCards[i].uid;
                if (!ArmyManager.Instance.getAllArmyPlayersIds().Contains(uid) && !ArmyManager.Instance.getAllArmyAlternateIds().Contains(uid) && !ArmyManager.Instance.isExistByActiveEditArmy(uid))
                {
                    /** 剔除有进化等级的卡,剔除类型不符的卡,剔除自身 */
                    if (allCards[i].getEvolveNextSid() == card.getEvolveNextSid() && allCards[i].uid != card.uid && allCards[i].getEvoLevel() ==0 )
                        matchCards.Add(allCards[i]);
                }
            }
            return matchCards;
        } else {
            return new List<Card>();
        }
    }

	/** 普通卡是否有万能卡支持进化 */
	public bool isGeneralCardsCanEvolutionByGoods (Card card)
	{
		if (isCanEvo (card)) {
			return getCardByQuilty (card) != null;
		} else {
			return false;
		}
	}

	/** 获取当前卡片进化到下一级的卡牌等级需求 */
	public int getCardLevel (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);
		return info == null ? 0 : (isMaxEvoLevel (card) ? info.getCardLevel () [card.getEvoLevel () - 1] : info.getCardLevel () [card.getEvoLevel ()]);
	}

	/** 获取当前卡片进化到下一级的玩家等级需求 */
	public int getPlayerLevel (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);

		return info == null ? 0 : (isMaxEvoLevel (card) ? info.getPlayerLevel () [card.getEvoLevel () - 1] : info.getPlayerLevel () [card.getEvoLevel ()]);
	}

	/// <summary>
    /// 普通进化中，获取卡片进化到下一级的金钱消耗需求
	/// </summary>  
	public long getNeedMoney (Card card)
	{
		return getNeedMoneyByLevel (card, 1, 0);
	}

	/** 获取当前卡片的金钱消耗需求 */
	public long getCostMoney (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);
		if (info == null)
			return 0;
		if (card.getEvoLevel () == 0) {
			return 0;
		}
		int index = Mathf.Clamp (card.getEvoLevel () - 1, 0, info.getNeedMoney ().Length - 1);
		return info.getNeedMoney () [index];
	}

	/** 获取指定sid和进化等级的金钱消耗需求 */
	public long getCostMoney (int sid, int lv)
	{
		EvolutionSample info = getEvoInfoByType (sid);
		if (info == null)
			return 0;
		if (lv == 0) {
			return 0;
		}
		int index = Mathf.Clamp (lv - 1, 0, info.getNeedMoney ().Length - 1);
//		Debug.LogError ("--->>>sid=" + sid + ",lv=" + lv + ",index=" + index);
		return info.getNeedMoney () [index];
	}

	/// <summary>
    /// 获取当前卡片提升指定进化等级后的金钱消耗需求
	/// </summary>
	/// <param name="_mainCard">主卡 </param>
	/// <param name="evoTimes">副卡的进化次数</param>
	/// <param name="evoLevel">副卡的进化等级</param>
	public long getNeedMoneyByLevel (Card _mainCard, int evoTimes, int evoLevel)
	{
//		Debug.LogError ("--->>>_mainCard=" + _mainCard.getName () + ",evoTimes=" + evoTimes + ",evoLevel=" + evoLevel);
		//主卡吃卡后对应进化等级的消耗 - max(主卡吃卡前进化等级,副卡进化等级)对应进化等级消耗
		if (isMaxEvoLevel (_mainCard))
			return 0;
		EvolutionSample info = getEvoInfoByType (_mainCard);
		if (info == null)
			return 0;
		int newLv = _mainCard.getTempUpdateEvoLevel (evoTimes);
		int evoSid = _mainCard.getEvolveNextSid ();
		int maxLv = Mathf.Max (_mainCard.getEvoLevel (), evoLevel);
//		Debug.LogError ("--->>newLv=" + newLv + ",maxLv=" + maxLv + ",minLv=" + minLv
//		                + ",newCost=" + getCostMoney (evoSid,newLv)
//		                + ",maxCost=" + getCostMoney (evoSid,maxLv)
//		                + ",minCost=" + getCostMoney (evoSid,minLv));
		return getCostMoney (evoSid,newLv) - getCostMoney (evoSid,maxLv);
		/*
		//进化后的等级
		int upLv = Mathf.Min (card.getTem      pUpdateEvoLevel (evoLv), card.getMaxEvoLevel ());
		//进化后等级的消耗
		int upMoney = info.getNeedMoney () [upLv - 1];
		int downLv = Mathf.Max (card.getEvoLevel (), 0);
		int downMoney = downLv > 0 ? info.getNeedMoney () [downLv - 1] : 0;
//		Debug.LogError("==========card=" + card.getName() + ",evoLv=" + (card.getEvoLevel() + evoLv) + "EvoCost,upMoney=" + upMoney + ",downMoney=" + downMoney);
		return upMoney - downMoney;
		*/
	}

    /// <summary>
    /// 获取进化到下一等级需要的钱
    /// </summary>
    public long getCostToNextEvoLevel(Card _mainCard)
    {
        return getNeedMoneyByLevel(_mainCard, _mainCard.getEvotimesToNextLevel(), 1);
    }
	/** 获取当前卡片进化到下一级的消耗需求 */
	public EvolutionCondition[] getEvoCon (Card card)
	{
		if (isMaxEvoLevel (card))
			return null;
		EvolutionSample info = getEvoInfoByType (card);
		if (info != null && info.getConditions () != null)
			return info.getConditions () [card.getEvoLevel ()];
		else 
			return null;
	}

	/** 获取当前卡片的附加效果 */
	public AttrChangeSample[] getAddEffect (Card card)
	{
		if (card.getEvolveNextSid () == 0)
			return null;
		if ((card.getEvoLevel () - 1) < 0)
			return null;
		EvolutionSample info = getEvoInfoByType (card);
		return info == null ? null : info.getAddEffects () [card.getEvoLevel () - 1];
	}

	/** 获取当前卡片增加的等级 */
	public int getAddLevel (Card card)
	{
		if ((card.getEvoLevel () - 1) < 0)
			return 0;
		EvolutionSample info = getEvoInfoByType (card);
		return info == null ? 0 : info.getAddLevel () [card.getEvoLevel () - 1];
	}

	/** 获取当前卡片的形象 */
	public int getImageSid (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);
		if (info == null || info.getIcoSid () == null || (card.getEvoLevel () - 1) < 0)
			return CardSampleManager.Instance.getRoleSampleBySid (card.sid).imageID;
		else
			return info.getIcoSid () [card.getEvoLevel () - 1];
	}

	/** 获取当前卡片所有天赋描述 */
	public string[] getAddTalentString (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);
		return info == null ? null : info.getAddTalentString ();
	}

	/** 获取当前卡片已(可)激活天赋描述（如果有） */
	public string getOpenTalentString (Card card,int oldEvo)
	{
//		string[] str = getAddTalentString (card);
//		int time = getCanOpenTalentNum (card);
//		if (str != null && time != 0) {
//			return str [time - 1];
//		}
//		return "";
		string[] str = getAddTalentString (card);
		int evo = card.getEvoLevel ();
		int[] times = getTalentNeedTimes (card);
		for (int i=times.Length-1; i>=0; i--) {
			if(oldEvo>=times[i])
				return "";
			if(evo>=times[i])
				return str[i];
		}
		return "";
	}

	/** 获取当前卡片天赋开启需要进化次数 */
	public int getTalentNeedTime (Card card)
	{
		int[] times = getTalentNeedTimes (card);
		int time = getCanOpenTalentNum (card);
		if (times != null && time != 0) {
			return times [time - 1];
		}
		return -1;
	}

	/** 获取当前卡片天赋开启需要进化次数需求列表 */
	public int[] getTalentNeedTimes (Card card)
	{
		EvolutionSample info = getEvoInfoByType (card);
		return info == null ? null : info.getTalentNeedTimes ();
	}

	/** 获取当前卡片可开启天赋数 */
	public int getCanOpenTalentNum (Card card)
	{
		int num = 0;
		int[] times = getTalentNeedTimes (card);
		if (times != null) {
			for (int i = 0; i < times.Length; ++i) {
				if (card.getEvoLevel () >= times [i]) {
					num++;
				}
			}
			return num;
		}
		return num;
	}

	/** 获取当前卡片已激活天赋前置次数 */
	public int getOpenTalentNum (Card card)
	{
		int num = 0;
		if (HeroRoadManagerment.Instance.isHaveBySid (card.getEvolveNextSid ())) {
			HeroRoad heroRoad = HeroRoadManagerment.Instance.map [card.getEvolveNextSid ()];
			int[] awakeInfo = heroRoad.getAwakeInfo ();
			for (int i = 0; i < awakeInfo.Length; i++) {
				if (awakeInfo [i] == 1) {
					num++;
				}
			}
			return num;
		} else {
			return num;
		}
	}

	/** 获取当前卡片进化次数是否可以激活天赋 */
	public bool isOpenTalentByThisEvoLv (Card card,int oldEvo)
	{
//		int[] times = getTalentNeedTimes (card);
//		int i = getOpenTalentNum (card);
//		int j = getCanOpenTalentNum (card);
//
//		if (times == null) {
//			return false;
//		}
//
//		if (j > 0) {
//			if (i > 0) {
//				for (int k = 0; k < times.Length; ++k) {
//					if (card.getEvoLevel () == times [k]) {
//						return true;
//					}
//				}
//			}
//		}
//		return false;
		//新的觉醒规则只有进化次数相关
		int evo = card.getEvoLevel ();
		int[] times = getTalentNeedTimes (card);
		for (int i=times.Length-1; i>=0; i--) {
			if(oldEvo>=times[i])
				return false;
			if(evo>=times[i])
				return true;
		}
		return false;
	}

	/** 获得当前进化属性加成 */
	public CardBaseAttribute getEvolutionAttr (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();  
		AttrChangeSample[] effects = getAddEffect (card);
		if (effects == null || effects.Length < 1)
			return attr;
		for (int j = 0; j < effects.Length; j++) {
			if (effects [j].getAttrType () == AttrChangeType.HP) {
				attr.hp += effects [j].getAttrValue (card.getLevel ()); 
			} else if (effects [j].getAttrType () == AttrChangeType.ATTACK) {
				attr.attack += effects [j].getAttrValue (card.getLevel ());
			} else if (effects [j].getAttrType () == AttrChangeType.DEFENSE) {
				attr.defecse += effects [j].getAttrValue (card.getLevel ());
			} else if (effects [j].getAttrType () == AttrChangeType.MAGIC) {
				attr.magic += effects [j].getAttrValue (card.getLevel ());
			} else if (effects [j].getAttrType () == AttrChangeType.AGILE) {
				attr.agile += effects [j].getAttrValue (card.getLevel ());
			}
		} 

		return attr;
	}

	/** 获得当前进化属性比例加成 */
	public CardBaseAttribute getEvolutionAttrPer (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();  
		AttrChangeSample[] effects = getAddEffect (card);
		if (effects == null || effects.Length < 1)
			return attr;
		for (int j = 0; j < effects.Length; j++) {
			
			if (effects [j].getAttrType () == AttrChangeType.PER_HP) {
				attr.perHp += effects [j].getAttrValue (card.getLevel ());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_ATTACK) {
				attr.perAttack += effects [j].getAttrValue (card.getLevel ());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_DEFENSE) {
				attr.perDefecse += effects [j].getAttrValue (card.getLevel ());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_MAGIC) {
				attr.perMagic += effects [j].getAttrValue (card.getLevel ());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_AGILE) {
				attr.perAgile += effects [j].getAttrValue (card.getLevel ());
			}
		} 
		
		return attr;
	}

	/** 获得卡片进化价值 */
	public int getEvoValue (Card _card)
	{
		EvolutionSample info = getEvoInfoByType (_card);
		return info.getEvoValue ();
	}

	/** 返回按卡片品质区分的万能卡 */
	public Prop getCardByQuilty (Card card)
	{
		return getPropBySid (getUniversalCardSid (card)) == null ? null : getPropBySid (getUniversalCardSid (card));
	}
    /** 返回按卡片品质区分的万能卡(不会返回空) */
    public Prop getCardByQualityNotNull(Card card)
    {
        return getPropBySid(getUniversalCardSid(card)) == null
            ? PropManagerment.Instance.createProp(getUniversalCardSid(card))
            : getPropBySid(getUniversalCardSid(card));
    }

    /// <summary>
	/// 获得对应万能卡SID(红卡还需根据进化等级分星级万能卡)
	/// </summary>
	public int getUniversalCardSid (Card card)
	{
		int qlv = card.getQualityId ();
		switch (qlv) {
		case 1:
			return 71059;
			
		case 2:
			return 71060;
			
		case 3:
			return 71061;
			
		case 4:
			return 71062;
			
		case 5:
			return 71063;
        case 6:
            int sid = 0;
            if (card.getEvoLevel() == 0) {
				sid = 71196;
			} else if (card.getEvoLevel() == 1) {
				sid = 71197;
			} else if (card.getEvoLevel() == 2) {
				sid = 71198;
			} else if (card.getEvoLevel() == 3) {
				sid = 71199;
			} else if (card.getEvoLevel() >= 4) {
                sid = 71200;
            }
            return sid;
		default:
			return -1;
		}
	}

	private Prop getPropBySid (int sid)
	{
		return StorageManagerment.Instance.getProp (sid);
	}
}
