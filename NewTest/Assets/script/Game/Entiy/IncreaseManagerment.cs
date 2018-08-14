using System.Collections;
using System.Collections.Generic;

/**
 * 可强化提示管理器
 * @authro 陈世惟  
 * */
public class IncreaseManagerment
{

	private List<BeastEvolve> allbeastArray;//召唤兽列表
	private List<BeastEvolve> beastCanSummonArray;//可以召唤的召唤兽列表
	private List<BeastEvolve> beastCanEvolutionArray;//可以进化的召唤兽列表
	private List<Card> allCardArray;//所有卡片
	private List<Card> generalCardEvolutionArray;//普通卡可进化列表
	private Prop[] propHalloews = new Prop[3];//三种圣石
	private List<Equip> allCanEquip;//所有可装备的装备列表
	private List<Card> allTeamCards;//队伍一的卡片

	public const int TYPE_BEAST = 1;
	public const int TYPE_CARD = 2;
	public IncreaseWayWindow increaseWayWindow;

	public static IncreaseManagerment Instance {
		get{ return SingleManager.Instance.getObj ("IncreaseManagerment") as IncreaseManagerment;}
	}

	public List<Equip> getAllCanEquip ()
	{
		return allCanEquip;
	}

	//显示数目
	public int showSum ()
	{
		int SpriteSum = 0;
        if (FriendsShareManagerment.Instance.getShareInfo() != null || (FriendsShareManagerment.Instance.getPraiseNum() > 0 && FriendsShareManagerment.Instance.getPraiseInfo() != null)) {
            return ++SpriteSum;
        }
		if (isBeastCanSummon ())
			return ++SpriteSum;
		if (isBeastCanEvolution ())
			return ++SpriteSum;
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW07)) {
			if (isMainCardCanSurmount ())
				return ++SpriteSum;
			else if (isMainCardCanEvoluion ()) 
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW29)) {
			if (isCardCanEvoluion ())
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW10)) {
			if (getNewKnighthood ())
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW08)) {
			if (getHeroRoad ())
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW23)) {
			if (isTeamCardCanPutOnEquip ())
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW04)) {
			if (isHallowsCanIntensify ())
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW01)) {
			if (isHaveStarCanOpen ())
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW02)) {
			if (isHaveNewGoodsCanOpen ())
				return ++SpriteSum;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW31)) {
			if (isCardCanTraining ())
				return ++SpriteSum;
		}
		if (GodsWarManagerment.Instance.GetIsOpen()&& GodsWarManagerment.Instance.isOnlineDay30()) {
			return ++SpriteSum;
		}
		return SpriteSum;
	}

	/// <summary>
	/// 是否存在可强化的项目
	/// </summary>
	public bool isHaveSomethingCanIncrease ()
	{
		if (isBeastCanSummon ())
			return true;
		if (isBeastCanEvolution ())
			return true;
		else if (isMainCardCanEvoluion ())
			return true;
		else if (isCardCanEvoluion ())
			return true;
		else if (getNewKnighthood ())
			return true;
		else if (getHeroRoad ())
			return true;
		else if (isTeamCardCanPutOnEquip ())
			return true;
		else if (isHallowsCanIntensify ())
			return true;
		else if (isHaveStarCanOpen ())
			return true;
		else if (isHaveNewGoodsCanOpen ())
			return true;
		else
			return false;
	}

	/// <summary>
	/// 根据类型清理缓存
	/// </summary>
	public void clearData (int type)
	{
		switch (type) {
		case TYPE_BEAST:
			allbeastArray = null;
			beastCanSummonArray = null;
			beastCanEvolutionArray = null;
			break;

		case TYPE_CARD:
			allCardArray = null;
			generalCardEvolutionArray = null;
			allTeamCards = null;
			break;
		}

		if (increaseWayWindow != null) {
			increaseWayWindow.updateUI ();
		}

	}

	#region 圣器强化

	public bool isHallowsCanIntensify ()
	{
		int maxLv = EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_HALLOW_EXP);
		int nowLv = EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_HALLOW_EXP, BeastEvolveManagerment.Instance.getHallowExp ());
		if (nowLv >= maxLv)
			return false;
		//有免费次数
		if (BeastEvolveManagerment.Instance.getLaveHallowConut () > 0)
			return true;

//		propHalloews [0] = StorageManagerment.Instance.getProp (71041);
//		propHalloews [1] = StorageManagerment.Instance.getProp (71042);
//		propHalloews [2] = StorageManagerment.Instance.getProp (71043);
		int hallowNum = 0;
		if (StorageManagerment.Instance.getProp (71041) != null) {
			hallowNum += StorageManagerment.Instance.getProp (71041).getNum ();
		}
		if (StorageManagerment.Instance.getProp (71042) != null) {
			hallowNum += StorageManagerment.Instance.getProp (71042).getNum ();
		}
		if (StorageManagerment.Instance.getProp (71043) != null) {
			hallowNum += StorageManagerment.Instance.getProp (71043).getNum ();
		}


		if (hallowNum >= 10)
			return true;
		return false;
	}

	#endregion

	#region 召唤兽

	/// <summary>
	/// 是否有可以召唤召唤兽
	/// </summary>
	/// <returns><c>true</c>, if beast can summon was ised, <c>false</c> otherwise.</returns>
	public bool isBeastCanSummon ()
	{
		return getBeastCanSummon () != null && getBeastCanSummon ().Count > 0&&(StorageManagerment.Instance.getAllBeast()==null||(StorageManagerment.Instance.getAllBeast()!=null&&StorageManagerment.Instance.getAllBeast().Count<=3));
	}

	/// <summary>
	/// 是否有可以进化的召唤兽
	/// </summary>
	/// <returns><c>true</c>, if beast can summon was ised, <c>false</c> otherwise.</returns>
	public bool isBeastCanEvolution ()
	{
		return getBeastEvolutionList () != null && getBeastEvolutionList ().Count > 0;
	}

	/// <summary>
	/// 获取可召唤召唤兽列表
	/// </summary>
	public BeastEvolve getBeastSummon ()
	{
		return (getBeastCanSummon () != null && getBeastCanSummon ().Count > 0) ? getBeastCanSummon () [0] : null;
	}

	/// <summary>
	/// 获取可进化召唤兽列表
	/// </summary>
	public List<BeastEvolve> getBeastEvolutionList ()
	{
		return (getBeastCanEvolution () != null && getBeastCanEvolution ().Count > 0) ? getBeastCanEvolution () : null;
	}

	/// <summary>
	/// 获取所有召唤兽
	/// </summary>
	private List<BeastEvolve> getAllBesastList ()
	{
		if ((allbeastArray != null && allbeastArray.Count <= 0)) {
			return allbeastArray;
		} else {
			allbeastArray = BeastEvolveManagerment.Instance.getAllBest ();
			return allbeastArray;
		}
	}

	/// <summary>
	/// 获取可召唤召唤兽列表
	/// </summary>
	private List<BeastEvolve> getBeastCanSummon ()
	{
		if (beastCanSummonArray != null && beastCanSummonArray.Count > 0)
			return beastCanSummonArray;
		beastCanSummonArray = new List<BeastEvolve> ();
		List<BeastEvolve> allBeasts = getAllBesastList ();
		for (int i = 0; i < allBeasts.Count; i++) {
			if (allBeasts [i].isCheckPremises (allBeasts [i]) && !allBeasts [i].isAllExist ()) {
				if (ExchangeManagerment.Instance.isCheckConditions (allBeasts [i].getExchangeBySids (allBeasts [i].getNextBeast ().sid))) {
					beastCanSummonArray.Add (allBeasts [i]);
				}
			}
		}
		return beastCanSummonArray;
	}

	/// <summary>
	/// 获取可进化召唤兽列表
	/// </summary>
	private List<BeastEvolve> getBeastCanEvolution ()
	{
		if (beastCanEvolutionArray != null && beastCanEvolutionArray.Count > 0)
			return beastCanEvolutionArray;
		beastCanEvolutionArray = new List<BeastEvolve> ();
		List<BeastEvolve> allBeasts = getAllBesastList ();
		
		for (int i = 0; i < allBeasts.Count; i++) {
			if (allBeasts [i].isAllExist () && !allBeasts [i].isEndBeast () && allBeasts [i].isCheckAllPremises (allBeasts [i])) {
				if (ExchangeManagerment.Instance.isCheckConditions (allBeasts [i].getExchangeBySids (allBeasts [i].getNextBeast ().sid)))
					beastCanEvolutionArray.Add (allBeasts [i]);
			}
		}
		return beastCanEvolutionArray;
	}

	#endregion

	#region 卡片

	/// <summary>
	/// 主卡是否可以进化
	/// </summary>
	public bool isMainCardCanEvoluion ()
	{
		return getMainCardCanEvolution ();
	}

	/// <summary>
	/// 主卡是否可以突破
	/// </summary>
	public bool isMainCardCanSurmount ()
	{
		return getMainCardCanSurmount ();
	}

	/// <summary>
	/// 是否有普通卡可以进化
	/// </summary>
	public bool isCardCanEvoluion ()
	{
		return getGeneralCardEvolutionList () != null && getGeneralCardEvolutionList ().Count > 0;
	}

	/// <summary>
	/// 是否有训练栏位可用
	/// </summary>
	/// <returns></returns>
	public bool isCardCanTraining ()
	{
		return CardTrainingManagerment.Instance.getCanUseLocation () > 0;
	}


	/// <summary>
	/// 获取满足进化条件的普通卡
	/// </summary>
	public List<Card> getCardEvolutionList ()
	{
		return getGeneralCardEvolutionList ();
	}

	/// <summary>
	/// 获取所有卡片
	/// </summary>
	private List<Card> getAllCardList ()
	{
		if (allCardArray != null && allCardArray.Count > 0)
			return allCardArray;
		ArrayList list = StorageManagerment.Instance.getAllRole ();
		allCardArray = new List<Card> ();

		for (int i = 0; i < list.Count; i++) {
			allCardArray.Add ((Card)list [i]);
		}
		return allCardArray;
	}

	/// <summary>
	/// 获取能进化的普通卡
	/// </summary>
	private List<Card> getGeneralCardEvolutionList ()
	{
		if (generalCardEvolutionArray != null && generalCardEvolutionArray.Count > 0)
			return generalCardEvolutionArray;

		generalCardEvolutionArray = new List<Card> ();
		List<Card> allCards = getAllCardList ();
		List<string> playersIds=ArmyManager.Instance.getAllArmyPlayersIds ();
		List<string> alternateIds=ArmyManager.Instance.getAllArmyAlternateIds ();
		//先从队伍里面找普通卡支持
		for (int i = 0; i < allCards.Count; i++) {
			if ((playersIds!=null&&playersIds.Contains (allCards [i].uid)) || (alternateIds!=null&&alternateIds.Contains (allCards [i].uid))) {
				if (!allCards [i].isMainCard () && EvolutionManagerment.Instance.isCanEvo (allCards [i])) {
					if (EvolutionManagerment.Instance.getFoodCardForEvo (allCards [i]) != null) {
						generalCardEvolutionArray.Add (allCards [i]);
						return generalCardEvolutionArray;
					}
				}
			}
		}
		//再从队伍里面找万能卡支持
		for (int i = 0; i < allCards.Count; i++) {
			if ((playersIds!=null&&playersIds.Contains (allCards [i].uid)) || (alternateIds!=null&&alternateIds.Contains (allCards [i].uid))) {
				if (!allCards [i].isMainCard () && EvolutionManagerment.Instance.isCanEvo (allCards [i])) {
					if (EvolutionManagerment.Instance.isGeneralCardsCanEvolutionByGoods (allCards [i])) {
						generalCardEvolutionArray.Add (allCards [i]);
						return generalCardEvolutionArray;
					}
				}
			}
		}		
		return generalCardEvolutionArray;
	}
    /// <summary>
    /// 获取能进化的普通卡
    /// </summary>
    public List<Card> geCommendCardEvolutionList() {
        List<Card> commendCard = new List<Card>();
        List<Card> allCards = getAllCardList();
        //先从队伍里面找普通卡支持
        for (int i = 0; i < allCards.Count; i++) {
            if (!allCards[i].isMainCard() && EvolutionManagerment.Instance.isCanEvo(allCards[i])) {
                if (EvolutionManagerment.Instance.getFoodCardForEvo(allCards[i]) != null) {
                    commendCard.Add(allCards[i]);
                    return commendCard;
                }
            }
        }
        //再从队伍里面找万能卡支持
        for (int i = 0; i < allCards.Count; i++) {
            if (!allCards[i].isMainCard() && EvolutionManagerment.Instance.isCanEvo(allCards[i])) {
                if (EvolutionManagerment.Instance.isGeneralCardsCanEvolutionByGoods(allCards[i])) {
                    commendCard.Add(allCards[i]);
                    return commendCard;
                }
            }
        }
        return commendCard;
    }

	/// <summary>
	/// 主角卡是否能进化
	/// </summary>
	private bool getMainCardCanEvolution ()
	{
		Card mainCard = StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid);
		return EvolutionManagerment.Instance.isCanEvo (mainCard);
	}

	/// <summary>
	/// 主角卡是否能突破
	/// </summary>
	private bool getMainCardCanSurmount ()
	{
		Card mainCard = StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid);
		return (SurmountManagerment.Instance.isCanSurByCon(mainCard));
	}

	#endregion
	
	#region 爵位

	/// <summary>
	/// 是否能升级爵位
	/// </summary>
	public bool getNewKnighthood ()
	{
		Knighthood knighthood = KnighthoodConfigManager.Instance.getKnighthoodByGrade (UserManager.Instance.self.honorLevel);

		//是否爵位达到顶值
		if (!KnighthoodConfigManager.Instance.isLastKnighthood (UserManager.Instance.self.honorLevel))
			return UserManager.Instance.self.honor - knighthood.needHonorValue >= 0;
		else 
			return false;
	}

	#endregion
	
	#region 英雄之章

	/// <summary>
	/// 是否有未挑战的英雄之章
	/// </summary>
	public bool getHeroRoad ()
	{
		int chvNum = UserManager.Instance.self.getChvPoint ();
		int chvMax = UserManager.Instance.self.getChvPointMax ();
		return HeroRoadManagerment.Instance.getCanBeChallengingTimes () > 0 && chvNum == chvMax;
	}

	#endregion
	
	#region 队伍中可穿装备

	/// <summary>
	/// 获得PVE队伍中卡片
	/// </summary>
	public List<Card> getAllArmyCards ()
	{
		if (allTeamCards == null) {
			List<string> stringList = ArmyManager.Instance.getCardsByTeam (ArmyManager.PVE_TEAMID);
			allTeamCards = new List<Card> ();
			for (int i=0; i<stringList.Count; i++) {
				if (stringList [i] != "0")
					allTeamCards.Add (StorageManagerment.Instance.getRole (stringList [i]));
			}
		}
		return allTeamCards;
	}

	/// <summary>
	/// 重新计算所有未穿的装备
	/// </summary>
	public void getAllEquips ()
	{
		ArrayList list = StorageManagerment.Instance.getAllEquip ();
		if (list == null) 
			return;

		if (allCanEquip == null)
			allCanEquip = new List<Equip> ();
		else
			allCanEquip.Clear ();
		Equip eq;
		for (int i = 0; i < list.Count; i++) {
			eq = (Equip)list [i];
			if (eq.getState () == EquipStateType.LOCKED || eq.getState () == EquipStateType.OCCUPY
				|| ChooseTypeSampleManager.Instance.isToEat (eq, ChooseTypeSampleManager.TYPE_EQUIP_EXP))
				continue;
			allCanEquip.Add (eq);
		}
	}

	/// <summary>
	/// 队伍中卡片是否可穿装备
	/// </summary>
	public bool isTeamCardCanPutOnEquip ()
	{
		getAllEquips ();
		allTeamCards = null;

		if (allCanEquip == null || allCanEquip.Count <= 0)
			return false;
		else if (isTeamCardAllPutOn ()) {
			return false;
		} else if (getTeamCardCanPutOnEquip () != null)
			return true;
//		else //感觉是和上面的分支相同多余的
//			return isCanBePutOnEquip(allCanEquip);
		return false;
	}

	public Card getTeamCardCanPutOnEquip ()
	{
		if (allCanEquip == null || allCanEquip.Count <= 0) {
			return null;
		}
		Card card;
		List<Card> sortCard = new List<Card>();
		foreach (Equip each in allCanEquip) {
			card = getCanPutOnEquipByTeamCards (each);
			if(card!=null)sortCard.Add(card);
		}
		if(sortCard!=null&&sortCard.Count>=1){
			for(int i=0;i<sortCard.Count;i++){
				if(sortCard[i].uid==UserManager.Instance.self.mainCardUid)return sortCard[i];
			}
			return sortCard[0];
		}
		return null;
	}
	/// <summary>
	/// 判断当前是否有适合卡片的,合适部位的 可以使用的装备 
	/// </summary>
	/// <returns><c>true</c>, if has free equip can use by car was checked, <c>false</c> otherwise.</returns>
	/// <param name="_card">_card.</param>
	public Equip checkHasFreeEquipCanUseByCar (Card _card, int _partId)
	{
		getAllEquips ();
		Equip equip = null;
		foreach (Equip item in allCanEquip) {
			if (_partId != item.getPartId ()) {
				continue;
			}
			if (isHaveEquipCanUse (_card, item)) {
				equip = item;
				break;
			}
		}
		return equip;
	}

	/// <summary>
	/// 对应卡片能否穿上对应装备 这里的装备肯定是可以穿的（剔除了祭品和使用中的）
	/// </summary>
	private bool isHaveEquipCanUse (Card _card, Equip _awards)
	{
		if (_card == null || _awards == null)
			return false;
		string[] equips = _card.getEquips ();
		//身上未穿任何装备
		if (equips == null || equips.Length <= 0) {
			return _awards.isPutOn (_card.sid);
		} else {
			for (int i=0; i<equips.Length; i++) {
				//表示相同部位已经有装备了
				if (StorageManagerment.Instance.getEquip (equips [i]).getPartId () == _awards.getPartId ()) 
					return false;
			}
			return _awards.isPutOn (_card.sid);
		}


//		if (_card.getEquips ()!= null) {
//
//			if(_card.getEquips ().Length < 5) {
//				int putOnNum = 0;
//				for (int i=0; i<_card.getEquips().Length; i++) {
//					if(StorageManagerment.Instance.getEquip (_card.getEquips()[i]).getPartId() == _awards.getPartId()) {
//						putOnNum++;
//					}
//				}
//				if(putOnNum == 0 && _awards.isPutOn(_card.sid)) {
//					return true;
//				}
//				else {
//					return false;
//				}
//			}
//			else {
//				return false;
//			}
//		}
//		else {
//			if(_awards.isPutOn(_card.sid))
//				return true;
//			else
//				return false;
//		}
	}

	public bool isCanBePutOnEquip (List<Equip> eqs)
	{
		Card card;
		foreach (Equip each in eqs) {
			card = getCanPutOnEquipByTeamCards (each);
			if (card != null)
				return true;
		}
		return false;
	}

	/// <summary>
	/// 如果全部都穿了，就别看装备了
	/// </summary>
	private bool isTeamCardAllPutOn ()
	{
		if (allTeamCards == null) {
			allTeamCards = getAllArmyCards ();
		}
		string[] equips;
		for (int i=0; i<allTeamCards.Count; i++) {
			equips = allTeamCards [i].getEquips ();
			if (equips == null || equips.Length < 5)
				return false;
		}
		return true;
	}

	/// <summary>
	/// 丢装备进来，看看队伍里的卡片谁可以穿
	/// </summary>
	public Card getCanPutOnEquipByTeamCards (Equip _awards)
	{
		if (allTeamCards == null) {
			allTeamCards = getAllArmyCards ();
		}
		List<Card> sortCard = new List<Card>();
		for(int j=0;j<allTeamCards.Count;j++){
			if(UserManager.Instance.self.mainCardUid==allTeamCards[j].uid){
				sortCard.Add(allTeamCards[j]);
				break;
			}
		}
		for(int jj=0;jj<allTeamCards.Count;jj++){
			if(UserManager.Instance.self.mainCardUid!=allTeamCards[jj].uid)sortCard.Add(allTeamCards[jj]);
		}
		string[] equips;
		for (int i=0; i<sortCard.Count; i++) {
			equips = sortCard [i].getEquips ();
			if (equips != null && equips.Length >= 5)
				continue;
			if (isHaveEquipCanUse (sortCard [i], _awards))
				return sortCard [i];
		}
		return null;
	}

	#endregion

	#region 女神星盘

	/// <summary>
	/// 是否存在可激活星星
	/// </summary>
	public bool isHaveStarCanOpen ()
	{
		return GoddessAstrolabeManagerment.Instance.isHaveStarCanOpen ();
	}

	#endregion

	#region 新手礼包

	private bool getNewGoodsBool (int _sid)
	{
		Prop prop = StorageManagerment.Instance.getProp (_sid);
		return prop != null && prop.getNum () > 0 && UserManager.Instance.self.getUserLevel () >= prop.getUseLv ();
	}

	public bool isHaveNewGoodsCanOpen ()
	{
		for (int i = 71031; i < 71041; i++) {
			if (getNewGoodsBool (i))
				return true;
		}
		return false;
	}

	#endregion

	#region 骑术修炼

	public bool isEnoughPropForMountsPractice ()
	{
		Prop tmpProp = null;
		StorageManagerment instance = StorageManagerment.Instance;
		MountsConfigManager config = MountsConfigManager.Instance;
		int[] itemSids = config.getItemSids ();
		int num = 0;
		for (int i = 0; i < itemSids.Length; i++) {
			tmpProp = instance.getProp (itemSids [i]);
			if (tmpProp != null) {
				num += tmpProp.getNum ();
			}
			if (num > 10) {
				return true;
			}
		}
		return false;
	}

	#endregion
}
