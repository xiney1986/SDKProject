using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntensifyCardManager
{
	/** 献祭 */
	public const int INTENSIFY_CARD_SACRIFICE = 1;
	/** 学习技能 */
	public const int INTENSIFY_CARD_LEARNSKILL = 2;
	/** 进化 */
	public const int INTENSIFY_CARD_EVOLUTION = 3;
	/** 附加 */
	public const int INTENSIFY_CARD_ADDON = 4;
	/** 传承 */
	public const int INTENSIFY_CARD_INHERIT = 5;
    /** 超进化 */
    public const int INTENSIFY_CARD_SUPRE_EVO = 6;
	/** 主卡选择 */
	public const int MAINCARDSELECT = 1;
	/** 副卡选择 */
	public const int FOODCARDSELECT = 2;
	public static bool IsOpenOneKeyWnd = true; //是否打开一键选择窗口
	public static int Choose = QualityType.EXCELLENT; //一键选择的品质
    /** 超进化已经选择的数量 */
    private int superEvoChoosedNum = 0;

	/** 主卡 or 传承原始卡(被吃数据的） */
	private Card mainCard;//主卡
	/** 食物卡 or 传承新卡(继承数据）*/
	private List<Card> foodCard = new List<Card>();
	/** 进化万能卡 */
	private Prop foodProp;
    /** 选择的万能卡数量 */
    private int foodPropNum;
	public bool isFromIncrease = false;//是否从强化提示打开
	/** 献祭后回收食物卡返还的奖品-显示用 */
	public List<PrizeSample> sacrificeRestorePrize;
	/** 附加属性后回收食物卡返还的奖品-显示用 */
	public List<PrizeSample> addOnRestorePrize;

	public static IntensifyCardManager Instance {
		get{ return SingleManager.Instance.getObj ("IntensifyCardManager") as IntensifyCardManager;}
	}

	//强化卡片入口
	public void intoIntensify (int intoType)
	{
		intoIntensify (intoType, null);
	}

	//强化卡片入口
	public void intoIntensify (int intoType, Card _mainCard, CallBack callback)
	{
		mainCard = _mainCard;
		intoIntensify (intoType, callback);
	}
	/// <summary>
	/// 动画结束跳到进化主界面
	/// </summary>

	public void updateEvolution(int intoType, Card _mainCard, CallBack callback){
		mainCard=_mainCard;
		updateEvolution(intoType,callback);
	}
	/// <summary>
	/// 动画结束跳到进化主界面
	/// </summary>
	public void updateEvolution(int intoType, CallBack callback){
		if (UiManager.Instance.intensifyCardWindow == null) {
			UiManager.Instance.openWindow<IntensifyCardWindow> ((win) => {
				win.setCallBack (callback);
				win.initWindow (intoType);
			});
		} else {
			UiManager.Instance.intensifyCardWindow.updateEvolution (intoType);
		}
	}
	//强化卡片入口
	public void intoIntensify (int intoType, CallBack callback)
	{
		if (UiManager.Instance.intensifyCardWindow == null) {
			UiManager.Instance.openWindow<IntensifyCardWindow> ((win) => {
				win.setCallBack (callback);
				win.initWindow (intoType);
			});
		} else {
			UiManager.Instance.intensifyCardWindow.initWindow (intoType);
		}
	}

	/** 设置主卡 */
	public void setMainCard (Card card)
	{
		mainCard = card;
	}

	/** 没有主卡，TRUE=没有 */
	public bool isHaveMainCard ()
	{
		return mainCard == null;
	}

	/** 获得主卡 */
	public Card getMainCard ()
	{
		return mainCard;
	}

	/** 匹配主卡 */
	public bool compareMainCard (Card card)
	{
		return card != null && mainCard != null && card.uid == mainCard.uid;
	}


	/** 主卡是否为空（true=不为空） */
	public bool isMainCard ()
	{
		return mainCard != null;// && mainCard.id != UserManager.Instance.self.mainCardUid;
	}

	/** 获得主卡卡片类型 */
	public int getMainCardType ()
	{
		return mainCard == null ? 0 : mainCard.getEvolveNextSid ();
	}

	/** 移除主卡 */
	public void removeMainCard ()
	{
		mainCard = null;
	}

	/** 设置食物卡 */
	public void setFoodCard (Card card)
	{
		foodCard.Add (card);
	}

	/** 获得食物卡 */
	public List<Card> getFoodCard ()
	{
		return foodCard;
	}



	/** 设置万能卡 */
	public void setFoodProp (Prop _prop)
	{
        if(foodProp == null || foodProp.sid != _prop.sid){
            foodProp = _prop;
            foodPropNum = 1;
        }
        else
        {
            foodPropNum++;
        }
	}
    /// <summary>
    /// 检查万能卡数量是否还可供选择
    /// </summary>
    public bool checkPropCanChoose(Prop prop ) {
        if (foodProp == null)
            return true;
        if (foodProp.sid != prop.sid)
            return true;
        if (foodProp.getNum() == foodPropNum)
            return false;
        return true;
            
    }

	/** 获得万能卡 */
	public Prop getFoodProp ()
	{
		return foodProp;
	}

    public int getFoodPropNum() {
        return foodPropNum;
    }

	/** 获得所有可以附加的卡 */
	public List<Card> getOneKeyAddon ()
	{
		List<Card> tempList = new List<Card> ();
	
		ArrayList allCard = StorageManagerment.Instance.getAllRole ();
		Card card;
		for (int i = 0; i <allCard.Count; i++) {
			card = allCard [i] as Card;
			if (isSpriteCard (card))
				tempList.Add (card);
		}
		return tempList;
	}

	/** 是否在队伍中 */
	private bool isInTeam (Card card)
	{
		List<Card> tempList = ArmyManager.Instance.getAllEditArmyCards ();
		if (tempList != null && tempList.Count != 0) {
			for (int i = 0; i < tempList.Count; i++) {
				if (tempList [i].uid == card.uid)
					return true;
			}
		}

		ArrayList teams = ArmyManager.Instance.getTeamCardUidList ();
		if (teams != null) {
			for (int i = 0; i<teams.Count; i++) {
				if (((string)teams [i]) == card.uid) {
					return true;
				}
			}
		}

		return false;
	}

	/** 获得一键献祭的卡片集合 */
	public List<Card> getOneKeySacrifice ()
	{
		List<Card> tempList = new List<Card> ();
		int num = 0;
		if (foodCard == null) {
			num = 8;
		} else {
			num = 8 - foodCard.Count;
		}
		//获取普通献祭卡集
		ArrayList commonSacrifice = StorageManagerment.Instance.getAllSacrificeRoleOfCommon ();
		if (commonSacrifice != null) {
			Card tempCard;
			for (int i = 0; i < commonSacrifice.Count; i++) {
				tempCard = new Card ();
				tempCard = commonSacrifice [i] as Card;
				if (compareMainCard (tempCard) || isInFood (tempCard) || isInTeam (tempCard) || isEatCard (tempCard) || isHaveExp (tempCard) )
					continue;

				tempList.Add (tempCard);
				num --;
				if (num == 0) {
					return tempList;
				}
			}
		}
		//获取优秀献祭卡集
		ArrayList excellentSacrifice = StorageManagerment.Instance.getAllSacrificeRoleOfExcellent ();
		if (excellentSacrifice != null) {
			Card tempCard;
			for (int i = 0; i < excellentSacrifice.Count; i++) {
				tempCard = new Card ();
				tempCard = excellentSacrifice [i] as Card;
				if (compareMainCard (tempCard) || isInFood (tempCard) || isInTeam (tempCard) || isEatCard (tempCard) || isHaveExp (tempCard))
					continue;
				tempList.Add (tempCard);
				num --;
				if (num == 0) {
					return tempList;
				}
			}
		}
		//获取精良献祭卡集
		ArrayList goodSacrifice = StorageManagerment.Instance.getAllSacrificeRoleOfGood ();
		if (goodSacrifice != null) {
			Card tempCard;
			for (int i = 0; i < goodSacrifice.Count; i++) {
				tempCard = new Card ();
				tempCard = goodSacrifice [i] as Card;
				if (compareMainCard (tempCard) || isInFood (tempCard) || isInTeam (tempCard) || isEatCard (tempCard) || isHaveExp (tempCard))
					continue;
				tempList.Add (tempCard);
				num --;
				if (num == 0) {
					return tempList;
				}
			}
		}

		//获取普通卡集
		ArrayList common = StorageManagerment.Instance.getAllRoleOfCommon ();
		if (common != null) {
			Card tempCard;
			for (int i = 0; i < common.Count; i++) {
				tempCard = new Card ();
				tempCard = common [i] as Card;
				if (compareMainCard (tempCard) || isInFood (tempCard) || isInTeam (tempCard) || isEatCard (tempCard) || isHaveExp (tempCard) )
					continue;
				
				tempList.Add (tempCard);
				num --;
				if (num == 0) {
					return tempList;
				}
			}
		}
		//获取优秀卡集
		ArrayList excellent = StorageManagerment.Instance.getAllRoleOfExcellent ();
		if (excellent != null) {
			Card tempCard;
			for (int i = 0; i < excellent.Count; i++) {
				tempCard = new Card ();
				tempCard = excellent [i] as Card;
				if (compareMainCard (tempCard) || isInFood (tempCard) || isInTeam (tempCard) || isEatCard (tempCard) || isHaveExp (tempCard))
					continue;
				tempList.Add (tempCard);
				num --;
				if (num == 0) {
					return tempList;
				}
			}
		}
		return tempList;
	}

	/** 移除指定的食物卡 */
	public void removeFoodCard (Card card)
	{
        foodCard.Remove (card);
	}

    public void removeFoodProp(Prop prop)
    {
        if (prop == null)
            return;
        if (foodProp == null)
        {
            foodPropNum = 0;
            return;
        }        
        if (prop.sid == foodProp.sid)
            foodPropNum--;
        if (foodPropNum <= 0)
        {
            foodProp = null;
        }
    }

	/** 清空食物卡 */
	public void clearFoodCard ()
	{
        foodCard.Clear ();
	}

	/** 是否有食物卡,TRUE=空 */
	public bool isHaveFood ()
	{
		return  foodCard.Count == 0;
	}

	/** 食物卡是否满了 */
	public bool isFoodFull ()
	{
		if (isHaveFood ())
			return false;
		return foodCard.Count >= 8;
	}

	/** 进化的食物卡是否满了 */
	public bool isEvoFoodFull ()
	{
		if (isHaveFood ())
			return false;
		return foodCard.Count >= 1;
	}


	/** 匹配食物卡 */
	public bool isInFood (Card card)
	{
		if ( card == null)
			return false;
		for (int i = 0; i < foodCard.Count; i++) {
			if (foodCard [i].uid == card.uid) {
				return true;
			}
		}
		return false;
	}

    public bool isInFood(Prop prop)
    {
        if (prop == null)
            return false;
        if (foodProp == null)
            return false;
        if (foodProp.sid == prop.sid)
            return true;
        return false;
    }

	/** 获得献祭消耗 */
	public string getCost ()
	{
		int cost = 0;
		for (int i = 0; i < foodCard.Count; i++) {
			cost += foodCard [i].getCardSkillLevelUpCast ();
		}
		return cost.ToString ();
	}

	/** 获得附加消耗 */
	public string getAddonCost ()
	{
		int cost = 0;
		for (int i = 0; i < foodCard.Count; i++) {
			cost += foodCard [i].getCardAddonCast ();
		}
		return cost.ToString ();
	}

	/** 是否可以点击献祭强化按钮 */
	public bool isShowIntesifyBtnBySkillLevelUp ()
	{
		int cost = 0;
		for (int i = 0; i < foodCard.Count; i++) {
			cost += foodCard [i].getCardSkillLevelUpCast ();
		}
		return cost > 0 && UserManager.Instance.self.getMoney () >= cost && mainCard != null && !isHaveFood ();
	}

	/** 是否可以点击附加强化按钮 */
	public bool isShowIntesifyBtnByAddon ()
	{
		int cost = 0;
		for (int i = 0; i < foodCard.Count; i++) {
			cost += foodCard [i].getCardAddonCast ();
		}
		return cost > 0 && UserManager.Instance.self.getMoney () >= cost && mainCard != null && !isHaveFood ();
	}


    /// <summary>
    /// 是否可以点击进化按钮
    /// </summary>
	public bool isShowIntesifyBtnByEvo ()
	{
		if (mainCard == null)
			return false;
		long cost = 0;
		if (foodCard != null && foodCard.Count > 0) {
			cost = EvolutionManagerment.Instance.getNeedMoneyByLevel (mainCard, foodCard [0].getEvoTimes () + 1, foodCard [0].getEvoLevel ());
		} else if (foodProp == null) {
			return false;
		} else {
			cost = EvolutionManagerment.Instance.getNeedMoney (mainCard);
		}

		return cost >= 0 && UserManager.Instance.self.getMoney () >= cost;
	}

    /// <summary>
    /// 是否可以点击超进化按钮
    /// </summary>
    /// <returns></returns>
    public bool isShowIntesifyBtnSuperEvo() {
        if (mainCard == null)
            return false;
        long cost = 0;
        if (foodCard.Count + foodPropNum == mainCard.getEvotimesToNextLevel())
        {
            cost = EvolutionManagerment.Instance.getCostToNextEvoLevel(mainCard);
        }
        else 
        {
            return false;
        }
        return cost >= 0 && UserManager.Instance.self.getMoney() >= cost;
    }

	public void topIntensifyCard (ArrayList list)
	{
		if (list == null)
			return;
		if (siftMainOrFood (list, mainCard)) {
			Card temp = list [0] as Card;
			list [0] = list [list.Count - 1] as Card;
			list [list.Count - 1] = temp;
		}
	}

	private bool siftMainOrFood (ArrayList list, Card card)
	{
		if (card != null) {
			for (int i = 0; i < list.Count; i++) {
				if ((list [i] as Card).uid == card.uid) {
					list.Remove (list [i]);
					break;
				}
			}
			list.Add (card);
			return true;
		}
		return false;
	}


	/** 只要精灵卡 */
	public ArrayList RemoveWasteCard (ArrayList list)
	{ 
		ArrayList newlist = new ArrayList ();
		Card card;
		for (int i = 0; i <list.Count; i++) {
			card = list [i] as Card;
			if (isSpriteCard (card))
				newlist.Add (card);
		}
//		foreach (Card each in list) { 
//			if (each == null)
//				continue; 
//			int count = 0;
//			//计算附加经验
//			count += each.getHPExp ();
//			count += each.getATTExp ();
//			count += each.getDEFExp ();
//			count += each.getMAGICExp ();
//			count += each.getAGILEExp ();
//			//如果附加经验大于0才会添加到显示队列
//			if (count > 0)
//				newlist.Add (each); 
//		}
		return newlist;
		
	}

	/** 是否精灵卡 */
	public bool isSpriteCard (Card _card)
	{
		if (ChooseTypeSampleManager.Instance.isToEat (_card, ChooseTypeSampleManager.TYPE_ADDON_NUM)) {
			return true;
		}
		return false;
	}

	/** 是否是特殊祭品卡 */
	public bool isEatCard (Card _card)
	{
		if (ChooseTypeSampleManager.Instance.isToEat (_card, ChooseTypeSampleManager.TYPE_ADDON_NUM)
			|| ChooseTypeSampleManager.Instance.isToEat (_card, ChooseTypeSampleManager.TYPE_MONEY_NUM)) {
			return true;
		} else
			return false;
	}

	/** 是否有经验 */
	public bool isHaveExp (Card _card)
	{
		if (_card.getEXP () > 0 || _card.getHPExp () > 0 || _card.getATTExp () > 0 || _card.getDEFExp () > 0 ||
		    _card.getMAGICExp () > 0 || _card.getAGILEExp () > 0 || _card.getEvoTimes () > 0) {
			return true;
		} else 
			return false;
	}

	public void filterLearn (ArrayList list)
	{
		for (int i = 0; i < list.Count; i++) {
			if ((list [i] as Card).getLevel () < 30) {
				list.Remove (list [i]);
				i--;
			}
		}
	}

    /// <summary>
    /// 清空副卡
    /// </summary>
	public void clearFood ()
	{
        foodCard.Clear();
        foodProp = null;
        foodPropNum = 0;
        superEvoChoosedNum = 0;

	}

    /// <summary>
    /// 清空副卡,主卡
    /// </summary>
	public void clearData ()
	{
        foodCard.Clear();
        foodProp = null;
        foodPropNum = 0;
        superEvoChoosedNum = 0;
		mainCard = null;

	}

	//页面切换
	public void TapChange (int type)
	{
		if (type == INTENSIFY_CARD_SACRIFICE) {
			if (mainCard != null && !mainCard.isCanSacrific ())
				clearFood ();
			else
				clearData ();
		} else if (type == INTENSIFY_CARD_EVOLUTION) {
			if (mainCard != null && (mainCard.uid != UserManager.Instance.self.mainCardUid))
				clearFood ();
			else
				clearData ();
		} else if (type == INTENSIFY_CARD_ADDON) {
			clearFood ();
		} else if (type == INTENSIFY_CARD_INHERIT) {
			if (mainCard != null && mainCard.uid == UserManager.Instance.self.mainCardUid)
			    clearData ();
			else
				clearFood();
		} else if (type == INTENSIFY_CARD_SUPRE_EVO) {
            if (mainCard != null && (mainCard.uid == UserManager.Instance.self.mainCardUid || mainCard.isMaxEvoLevel()))
                clearData();
            else
                clearFood();
        }
	}

    /** 重置已经选择的卡的数量 */
    public void resetSuperExoChoosedNum() {
        superEvoChoosedNum  = foodCard.Count;
    }    

    /// <summary>
    /// 是否能够选取副卡,保证每次只能选一张
    /// </summary>
    public bool getIsCanGetSuperExoFood() {
        return foodCard.Count - superEvoChoosedNum == 0;
    }

    /// <summary>
    /// 是否已经选取了新副卡,确认是否选取了副卡
    /// </summary>
    public bool getHaveGetSuperExoFood() {
        return foodCard.Count - superEvoChoosedNum != 0;
    }


    /** 获取最新的同名卡 */
    public Card getNewFoodCard()
    {
        return foodCard.Count == 0 ? null : foodCard[foodCard.Count - 1]; 
    }

    /// <summary>
    /// 是否限时超进化按钮
    /// </summary>
    /// <returns></returns>
    public bool isShowSuperEvoButton() {
        ArrayList allList = StorageManagerment.Instance.getAllRoleByNotToEat();
       foreach (Card each in allList)
       {
          
           if (each.uid == UserManager.Instance.self.mainCardUid)
               continue;
           /** 选择能够超进化的卡片 */
           if (each.isInSuperEvo() )
               return true;
       }
       return false;
    }

    /// <summary>
    /// 获取超进化指引的那张卡
    /// </summary>
    /// <returns></returns>
    public Card getSuperEvoGuideCard()
    {
        ArrayList allList = StorageManagerment.Instance.getAllRoleByNotToEat();
        foreach (Card each in allList)
        {
            if (each.uid == UserManager.Instance.self.mainCardUid)
                continue;
            /** 选择能够超进化的卡片 */
            if (each.isInSuperEvo() && each.getEvoLevel() == 10)
                return each;
        }
        return null;
    }

    /// <summary>
    /// 是否显示超进化指引
    /// </summary>
    /// <returns></returns>
    public bool isShowSuperEvoGuide() {
        ArrayList allList = StorageManagerment.Instance.getAllRoleByNotToEat();
        foreach (Card each in allList) {
            if (each.uid == UserManager.Instance.self.mainCardUid)
                continue;
            /** 选择能够超进化的卡片 */
            if (each.isInSuperEvo() && each.getEvoLevel() == 10)
                return true;
        }
        return false;
    }

	public int getFoodCardNum(int cardSid){
		int num =0;
		foreach(Card card in foodCard){
			if(card.sid == cardSid){
				num ++;
			}
		}
		return num;

	}

}
