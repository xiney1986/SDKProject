using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 筛选排序管理器
 * @author huangzhenghan
 * */

public class SortManagerment
{ 
	public  bool isCardStoreModifyed = false;
	public  bool isCardChooseModifyed = false;
	public  bool isIntensifyTapModifyed = false;
	public  bool isIntensifyCardChooseModifyed = false;
	public  bool isEquipChooseModifyed = false;
	public  bool isStoreModifyed = false;
	public  bool isIntensifyEquipModifyed = false;
	
	public SortManagerment ()
	{  

	}
	
	public static SortManagerment Instance {
		get{ return SingleManager.Instance.getObj ("SortManagerment") as SortManagerment;}
	}
	
	public ArrayList shopSort (ArrayList goods, SortCondition sc)
	{
		ArrayList newGoods = new ArrayList ();
		foreach (object each in goods) {
			newGoods.Add (each);
		}
		if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0)
			goodsSift (newGoods, sc.siftConditionArr);
		if (sc.sortCondition != null)
			goodsOrder (newGoods, sc.sortCondition);
		return newGoods;
	}
	
	//卡片筛选排序(所有卡片) conditions筛选条件集合 condition排序条件(只有一个)
	public ArrayList cardSort (ArrayList cards, SortCondition sc)
	{
		return cardSort (cards, sc, CardStateType.STATE_INIT, CardStateType.NO_REMOVE);
	}
	//卡片筛选排序(所有卡片) conditions筛选条件集合 condition排序条件(只有一个) state根据状态区分
	public ArrayList cardSort (ArrayList cards, SortCondition sc, int state)
	{
		return cardSort (cards, sc, state, CardStateType.NO_REMOVE);
	}
	//卡片筛选排序 conditions筛选条件集合 condition排序条件(只有一个) state根据哪个状态区分排前面 removeState剔除相应状态卡片，不剔除传-1
	public ArrayList cardSort (ArrayList cards, SortCondition sc, int state, int removeState)
	{
		ArrayList newCards1 = new ArrayList ();//初始状态卡片
		ArrayList newCards2;//指定状态卡片
		//是否需要区分状态
		switch (state) {
		case CardStateType.STATE_INIT:
			foreach (Card each in cards) {
				if (removeState != CardStateType.NO_REMOVE && each.checkState (removeState))
					continue;
				newCards1.Add (each);
			}
			if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0)
				cardSift (newCards1, sc.siftConditionArr);
			if (sc.sortCondition != null)
				cardOrder (newCards1, sc.sortCondition);
			break;
		default:
			newCards2 = new ArrayList ();
			foreach (Card each in cards) {
				//处于剔除状态
				if (removeState != CardStateType.NO_REMOVE && each.checkState (removeState))
					continue;
				//处于指定的状态
				if (each.checkState (state))
					newCards2.Add (each);
				else
					newCards1.Add (each);
			}
			if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
				cardSift (newCards1, sc.siftConditionArr);
				cardSift (newCards2, sc.siftConditionArr);
			}
			if (sc.sortCondition != null) {
				cardOrder (newCards1, sc.sortCondition);
				cardOrder (newCards2, sc.sortCondition);
			}
			//合并两个列表
			ListKit.AddRange(newCards2,newCards1);
			newCards1 = newCards2;
			break;
		}
		return newCards1;
	}
	
	//装备筛选排序(所有装备) conditions筛选条件集合 condition排序条件(只有一个) 包括异常状态装备
	public ArrayList equipSort (ArrayList equips, SortCondition sc)
	{
		return equipSort (equips, sc, EquipStateType.INIT, EquipStateType.NO_REMOVE);
	}
    //神器筛选排序(所有神器) conditions筛选条件集合 condition排序条件(只有一个) 包括异常状态装备
    public ArrayList magicWeaponSort(ArrayList magicWeapons, SortCondition sc) {
        return magicWeaponSort(magicWeapons, sc, EquipStateType.INIT, EquipStateType.NO_REMOVE);
    }

	//装备筛选排序 conditions筛选条件集合 condition排序条件(只有一个) state区分指定状态装备
	public ArrayList equipSort (ArrayList equips, SortCondition sc, int state)
	{
		return equipSort (equips, sc, state, EquipStateType.NO_REMOVE);
	}
    //执行筛选
    private void magicWeaponSift(ArrayList magicWeapons, Condition[] conditions) {
        List<int> selects;
        //先执行筛选
        for (int i = 0; i < conditions.Length; i++) {
            selects = conditions[i].getConditions();
            switch (conditions[i].getType()) {
                case SortType.EQUIP_JOB:
                    for (int j = 0; j < magicWeapons.Count; j++) {
                        for (int k = 0; k < selects.Count; k++) {
                            if ((magicWeapons[j] as MagicWeapon).getMgType() == selects[k])
                                break;
                            else if (k >= selects.Count - 1) {
                                magicWeapons.RemoveAt(j);
                                j--;
                            }
                        }
                    }
                    break;
                case SortType.QUALITY:
                    for (int j = 0; j < magicWeapons.Count; j++) {
                        for (int k = 0; k < selects.Count; k++) {
                            if ((magicWeapons[j] as MagicWeapon).getLvType() == selects[k])
                                break;
                            else if (k >= selects.Count - 1) {
                                magicWeapons.RemoveAt(j);
                                j--;
                            }
                        }
                    }
                    break;
                case SortType.EQUIP_STATE:
                    for (int j = 0; j < magicWeapons.Count; j++) {
                        for (int k = 0; k < selects.Count; k++) {
                            if ((magicWeapons[j] as MagicWeapon).state == selects[k])
                                break;
                            else if (k >= selects.Count - 1) {
                                magicWeapons.RemoveAt(j);
                                j--;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    //执行排序
    private void magicWeaponOrder(ArrayList magicWeapons, Condition condition) {
        if (condition.getType() == SortType.SORT) {
            MagicWeapon equip1, equip2;
            for (int k = 0; k < condition.getConditions().Count; k++) {
                int mm = condition.getConditions()[k];
                switch (condition.getConditions()[k]) {
                    case SortType.SORT_STRENG_LEVEUP:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getStrengLv() > equip2.getStrengLv())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    case SortType.SORT_STENG_LEVEDOWN:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getStrengLv() < equip2.getStrengLv())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    case SortType.SORT_PHASE_LEVEUP:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getPhaseLv() > equip2.getPhaseLv())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    case SortType.SPRT_PHASE_LEVEDOWN:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getPhaseLv() < equip2.getPhaseLv())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    case SortType.SORT_QUALITYUP:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getMagicWeaponQuality() > equip2.getMagicWeaponQuality())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    case SortType.SORT_QUALITYDOWN:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getMagicWeaponQuality() < equip2.getMagicWeaponQuality())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    case SortType.SORT_POWERUP:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getPowerCombat() > equip2.getPowerCombat())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    case SortType.SORT_POWERDOWN:
                        for (int i = 0; i < magicWeapons.Count; i++) {
                            for (int j = 0; j < magicWeapons.Count - 1 - i; j++) {
                                equip1 = magicWeapons[j] as MagicWeapon;
                                equip2 = magicWeapons[j + 1] as MagicWeapon;
                                if (equip1.getPowerCombat() < equip2.getPowerCombat())
                                    swap(magicWeapons, j, j + 1);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public ArrayList magicWeaponSort(ArrayList magicWeapons, SortCondition sc, int state, int removeState) {
        ArrayList newEquip1 = new ArrayList();//初始状态装备
        ArrayList newEquip2;//指定状态装备
        //是否需要区分状态
        switch (state) {
            case EquipStateType.INIT:
                foreach (MagicWeapon each in magicWeapons) {
                    if (removeState != EquipStateType.NO_REMOVE && each.checkState(removeState))
                        continue;
                    newEquip1.Add(each);
                }
                if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0)
                    magicWeaponSift(newEquip1, sc.siftConditionArr);
                if (sc.sortCondition != null)
                    magicWeaponOrder(newEquip1, sc.sortCondition);
                break;
            default:
                newEquip2 = new ArrayList();
                foreach (MagicWeapon each in magicWeapons) {
                    //处于剔除状态
                    if (removeState != EquipStateType.NO_REMOVE && each.checkState(removeState))
                        continue;
                    //处于指定的状态
                    if (each.checkState(state))
                        newEquip2.Add(each);
                    else
                        newEquip1.Add(each);
                }
                if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
                    magicWeaponSift(newEquip1, sc.siftConditionArr);
                    magicWeaponSift(newEquip2, sc.siftConditionArr);
                }
                if (sc.sortCondition != null) {
                    magicWeaponOrder(newEquip1, sc.sortCondition);
                    magicWeaponOrder(newEquip2, sc.sortCondition);
                }
                //合并两个列表
                ListKit.AddRange(newEquip2, newEquip1);
                newEquip1 = newEquip2;
                break;
        }
        return newEquip1;
    }

	//装备筛选排序 conditions筛选条件集合 condition排序条件(只有一个) state根据哪个状态区分排前面 removeState剔除指定状态装备
	public ArrayList equipSort (ArrayList equips, SortCondition sc, int state, int removeState)
	{
		ArrayList newEquip1 = new ArrayList ();//初始状态装备
		ArrayList newEquip2;//指定状态装备
		//是否需要区分状态
		switch (state) {
		case EquipStateType.INIT:
			foreach (Equip each in equips) {
				if (removeState != EquipStateType.NO_REMOVE && each.checkState (removeState))
					continue;
				newEquip1.Add (each);
			}
			if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0)
				equipSift (newEquip1, sc.siftConditionArr);
			if (sc.sortCondition != null)
				equipOrder (newEquip1, sc.sortCondition);
			break;
		default:
			newEquip2 = new ArrayList ();
			foreach (Equip each in equips) {
				//处于剔除状态
				if (removeState != EquipStateType.NO_REMOVE && each.checkState (removeState))
					continue;
				//处于指定的状态
				if (each.checkState (state))
					newEquip2.Add (each);
				else
					newEquip1.Add (each);
			}
			if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
				equipSift (newEquip1, sc.siftConditionArr);
				equipSift (newEquip2, sc.siftConditionArr);
			}
			if (sc.sortCondition != null) {
				equipOrder (newEquip1, sc.sortCondition);
				equipOrder (newEquip2, sc.sortCondition);
			}
			//合并两个列表
			ListKit.AddRange(newEquip2,newEquip1);
			newEquip1 = newEquip2;
			break;
		}
		return newEquip1;
	}

	//装备筛选排序(所有装备) conditions筛选条件集合 condition排序条件(只有一个) 包括异常状态装备
	public ArrayList starSoulSort (ArrayList starSouls, SortCondition sc,int type) {
		return starSoulSort (starSouls, sc, EquipStateType.INIT, type);
	}
	//星魂筛选排序 conditions筛选条件集合 condition排序条件(只有一个) state根据哪个状态区分排前面 removeState剔除指定状态装备
	public ArrayList starSoulSort (ArrayList starSouls, SortCondition sc, int state, int removeState) {
		ArrayList newStarSoul1 = new ArrayList ();//初始状态星魂
		ArrayList newStarSoul2;//指定状态星魂
		//是否需要区分状态
		switch (state) {
		case EquipStateType.INIT:
			foreach (StarSoul each in starSouls) {
				if (removeState != EquipStateType.NO_REMOVE && each.checkState (removeState))
					continue;
				newStarSoul1.Add (each);
			}
			if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0)
				starSoulSift (newStarSoul1, sc.siftConditionArr);
			if (sc.sortCondition != null)
				starSoulOrder (newStarSoul1, sc.sortCondition);
			break;
		default:
			newStarSoul2 = new ArrayList ();
			foreach (StarSoul each in starSouls) {
				//处于剔除状态
				if (removeState != EquipStateType.NO_REMOVE && each.checkState (removeState))
					continue;
				//处于指定的状态
				if (each.checkState (state))
					newStarSoul2.Add (each);
				else
					newStarSoul2.Add (each);
			}
			if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
				starSoulSift (newStarSoul1, sc.siftConditionArr);
				starSoulSift (newStarSoul2, sc.siftConditionArr);
			}
			if (sc.sortCondition != null) {
				starSoulOrder (newStarSoul1, sc.sortCondition);
				starSoulOrder (newStarSoul2, sc.sortCondition);
			}
			//合并两个列表
			ListKit.AddRange(newStarSoul2,newStarSoul1);
			newStarSoul1 = newStarSoul2;
			break;
		}
		return newStarSoul1;
	}

	//splitTypes 按顺序分组 equips是已经排序筛选过的列表
	public ArrayList equipSplit (ArrayList equips, int[] splitTypes)
	{
		ArrayList[] arrays = new ArrayList[splitTypes.Length];
		for (int i = 0; i<splitTypes.Length; i++) {
			arrays[i] = new ArrayList ();
			for (int j = 0; j<equips.Count; j++) {
				if(equipSplitByType(arrays[i],equips[j] as Equip,splitTypes[i]))
					equips.RemoveAt(j--);
			}
		}
		//类型批分完成后合并
		ArrayList final = new ArrayList ();
		for (int i = 0; i<arrays.Length; i++) {
			ListKit.AddRange(final,arrays[i]);
		}
		ListKit.AddRange(final,equips);
		return final;
	}

	//splitTypes 按顺序分组 starSouls是已经排序筛选过的列表
	public ArrayList starSoulSplit (ArrayList starSouls, int[] splitTypes)
	{
		ArrayList[] arrays = new ArrayList[splitTypes.Length];
		for (int i = 0; i<splitTypes.Length; i++) {
			arrays[i] = new ArrayList ();
			for (int j = 0; j<starSouls.Count; j++) {
				if(starSoulSplitByType(arrays[i],starSouls[j] as StarSoul,splitTypes[i]))
					starSouls.RemoveAt(j--);
			}
		}
		//类型批分完成后合并
		ArrayList final = new ArrayList ();
		for (int i = 0; i<arrays.Length; i++) {
			ListKit.AddRange(final,arrays[i]);
		}
		ListKit.AddRange(final,starSouls);
		return final;
	}

	//返回是否符合区分条件
	private bool equipSplitByType (ArrayList newList, Equip equip, int type)
	{
		//有些类型需要排除祭品
		switch (type) {
		case SortType.SPLIT_EQUIP_NEW:
			if (equip.isNew) {
				newList.Add (equip);
				return true;
			}
			break;
		case SortType.SPLIT_EATEN:
			if (ChooseTypeSampleManager.Instance.isToEat (equip, ChooseTypeSampleManager.TYPE_EQUIP_EXP)) {
				newList.Add (equip);
				return true;
			}
			break;
		case SortType.SPLIT_FREE_STATE:
			if (equip.freeState () && !ChooseTypeSampleManager.Instance.isToEat (equip, ChooseTypeSampleManager.TYPE_EQUIP_EXP)) {
				newList.Add (equip);
				return true;
			}
			break;
		case SortType.SPLIT_USING_STATE:
			if (equip.checkState(EquipStateType.OCCUPY) && !ChooseTypeSampleManager.Instance.isToEat (equip, ChooseTypeSampleManager.TYPE_EQUIP_EXP)) {
				newList.Add (equip);
				return true;
			}
			break;
		}
		return false;
	}

	//返回是否符合区分条件
	private bool starSoulSplitByType (ArrayList newList, StarSoul starSoul, int type) {
		//有些类型需要排除祭品
		switch (type) {
		case SortType.SPLIT_EQUIP_NEW:
			if (starSoul.isNew) {
				newList.Add (starSoul);
				return true;
			}
			break;
		case SortType.SPLIT_EATEN:
			if (starSoul.getStarSoulType()==0) {
				newList.Add (starSoul);
				return true;
			}
			break;
		case SortType.SPLIT_FREE_STATE:
			if ((starSoul.freeState ()||starSoul.getState()==EquipStateType.LOCKED) && starSoul.getStarSoulType()!=0) {
				newList.Add (starSoul);
				return true;
			}
			break;
		case SortType.SPLIT_USING_STATE:
			if (starSoul.checkState(EquipStateType.OCCUPY) && starSoul.getStarSoulType()!=0) {
				newList.Add (starSoul);
				return true;
			}
			break;
		}
		return false;
	}
	
	public ArrayList propSort (ArrayList props, SortCondition sc)
	{
		ArrayList newList = new ArrayList ();
		foreach (object each in props) {
			newList.Add (each);
		}
		if (sc.siftConditionArr != null && sc.siftConditionArr.Length > 0)
			propSift (newList, sc.siftConditionArr);
		if (sc.sortCondition != null)
			propOrder (newList, sc.sortCondition);
		return newList;
	}
	
	//执行筛选
	private void cardSift (ArrayList cards, Condition[] conditions)
	{
		List<int> selects;
		for (int i = 0; i < conditions.Length; i++) {
			selects = conditions [i].getConditions ();
			switch (conditions [i].getType ()) {
			case SortType.JOB:
				for (int j = 0; j < cards.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((cards [j] as Card).getJob () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							cards.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			case SortType.QUALITY:
				for (int j = 0; j < cards.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((cards [j] as Card).getQualityId () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							cards.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			case SortType.CARD_LEVEL_MAX:
				for (int j = 0; j < cards.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((cards [j] as Card).isMaxLevelToInt () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							cards.RemoveAt (j);
							j--;
						}
					}
				}
				break;
				
			default:
				break;
			}
		}
	}
	
	//执行排序
	private void cardOrder (ArrayList cards, Condition condition)
	{
		if (condition.getType () == SortType.SORT) {
			//排序只有一个条件
			Card card1, card2;
			for (int k=0; k<condition.getConditions().Count; k++) {
				switch (condition.getConditions () [k]) {
				case SortType.SORT_LEVELUP:
					for (int i = 0; i < cards.Count; i++) {
						for (int j = 0; j < cards.Count - 1 - i; j++) {
							card1 = cards [j] as Card;
							card2 = cards [j + 1] as Card;
							if (card1.getLevel () > card2.getLevel ())
								swap (cards, j, j + 1);
							else if (card1.getLevel () == card2.getLevel () && card1.sid > card2.sid)
								swap (cards, j, j + 1);
						}
					}
					break;
				case SortType.SORT_LEVELDOWN:
					for (int i = 0; i < cards.Count; i++) {
						for (int j = 0; j < cards.Count - 1 - i; j++) {
							card1 = cards [j] as Card;
							card2 = cards [j + 1] as Card;
							if (card1.getLevel () < card2.getLevel ())
								swap (cards, j, j + 1);
							else if (card1.getLevel () == card2.getLevel () && card1.sid < card2.sid)
								swap (cards, j, j + 1);
						}
					}
					break;
				case SortType.SORT_QUALITYUP:
					for (int i = 0; i < cards.Count; i++) {
						for (int j = 0; j < cards.Count - 1 - i; j++) {
							card1 = cards [j] as Card;
							card2 = cards [j + 1] as Card;
							if (card1.getQualityId () > card2.getQualityId ())
								swap (cards, j, j + 1);
							else if (card1.getQualityId () == card2.getQualityId () && card1.sid > card2.sid) //修改第二排序为等级 - yxl - 140114 重新修改，符合进化相同sid挨着
								swap (cards, j, j + 1);
							else if (card1.getQualityId () == card2.getQualityId () && card1.sid == card2.sid && card1.getLevel() > card2.getLevel()) 
								swap (cards, j, j + 1);
						}
					}
					break;
				case SortType.SORT_QUALITYDOWN:
					for (int i = 0; i < cards.Count; i++) {
						for (int j = 0; j < cards.Count - 1 - i; j++) {
							card1 = cards [j] as Card;
							card2 = cards [j + 1] as Card;
							if (card1.getQualityId () < card2.getQualityId ())
								swap (cards, j, j + 1);
							else if (card1.getQualityId () == card2.getQualityId () && card1.sid < card2.sid)
								swap (cards, j, j + 1);
							else if (card1.getQualityId () == card2.getQualityId () && card1.sid == card2.sid && card1.getLevel () < card2.getLevel ())
								swap (cards, j, j + 1);
						}
					}
					break;
				case SortType.SORT_POWERUP:
					for (int i = 0; i < cards.Count; i++) {
						for (int j = 0; j < cards.Count - 1 - i; j++) {
							card1 = cards [j] as Card;
							card2 = cards [j + 1] as Card;
							if (card1.getPower () > card2.getPower ())
								swap (cards, j, j + 1);
							else if (card1.getPower () == card2.getPower () && card1.getLevel () > card2.getLevel ())
								swap (cards, j, j + 1);
						}
					}
					break;
				case SortType.SORT_POWERDOWN:
					for (int i = 0; i < cards.Count; i++) {
						for (int j = 0; j < cards.Count - 1 - i; j++) {
							card1 = cards [j] as Card;
							card2 = cards [j + 1] as Card;
							if (card1.getPower () < card2.getPower ())
								swap (cards, j, j + 1);
							else if (card1.getPower () == card2.getPower () && card1.getLevel () < card2.getLevel ())
								swap (cards, j, j + 1);
						}
					}
					break;
				default:
					break;
				}
			}
		}
	}
	
	//执行筛选
	private void equipSift (ArrayList equips, Condition[] conditions)
	{
		List<int> selects;
		//先执行筛选
		for (int i = 0; i < conditions.Length; i++) {
			selects = conditions [i].getConditions ();
			switch (conditions [i].getType ()) {
			case SortType.EQUIP_JOB:
				for (int j = 0; j < equips.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((equips [j] as Equip).getJob () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							equips.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			case SortType.EQUIP_SUIT:
				for (int j = 0; j < equips.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((equips [j] as Equip).getSuitSid () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							equips.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			case SortType.QUALITY:
				for (int j = 0; j < equips.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((equips [j] as Equip).getQualityId () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							equips.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			case SortType.EQUIP_PART:
				for (int j = 0; j < equips.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((equips [j] as Equip).getPartId () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							equips.RemoveAt (j);
							j--;
						}
					}
				}
				break; 
			case SortType.EQUIP_STATE:
				for (int j = 0; j < equips.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((equips [j] as Equip).getState () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							equips.RemoveAt (j);
							j--;
						}
					}
				}
				break; 
			default:
				break;
			}
		}
	}
	
	//执行排序
	private void equipOrder (ArrayList equips, Condition condition)
	{
		if (condition.getType () == SortType.SORT) {
			Equip equip1, equip2;
			for (int k=0; k<condition.getConditions().Count; k++) {
				switch (condition.getConditions () [k]) {
				case SortType.SORT_LEVELUP:
					for (int i = 0; i < equips.Count; i++) {
						for (int j = 0; j < equips.Count - 1 - i; j++) {
							equip1 = equips [j] as Equip;
							equip2 = equips [j + 1] as Equip;
                            if (equip1.getLevel() > equip2.getLevel())
								swap (equips, j, j + 1);
						}
					}
					break;
				case SortType.SORT_LEVELDOWN:
					for (int i = 0; i < equips.Count; i++) {
						for (int j = 0; j < equips.Count - 1 - i; j++) {
							equip1 = equips [j] as Equip;
							equip2 = equips [j + 1] as Equip;
                            if (equip1.getLevel() < equip2.getLevel())
								swap (equips, j, j + 1);
						}
					}
					break;
				case SortType.SORT_QUALITYUP:
					for (int i = 0; i < equips.Count; i++) {
						for (int j = 0; j < equips.Count - 1 - i; j++) {
							equip1 = equips [j] as Equip;
							equip2 = equips [j + 1] as Equip;
							if (equip1.getQualityId () > equip2.getQualityId ())
								swap (equips, j, j + 1);
						}
					}
					break;
				case SortType.SORT_QUALITYDOWN:
					for (int i = 0; i < equips.Count; i++) {
						for (int j = 0; j < equips.Count - 1 - i; j++) {
							equip1 = equips [j] as Equip;
							equip2 = equips [j + 1] as Equip;
							if (equip1.getQualityId () < equip2.getQualityId ())
								swap (equips, j, j + 1);
						}
					}
					break;
				case SortType.SORT_POWERUP:
					for (int i = 0; i < equips.Count; i++) {
						for (int j = 0; j < equips.Count - 1 - i; j++) {
							equip1 = equips [j] as Equip;
							equip2 = equips [j + 1] as Equip;
							if (equip1.getPower () > equip2.getPower ())
								swap (equips, j, j + 1);
						}
					}
					break;
				case SortType.SORT_POWERDOWN:
					for (int i = 0; i < equips.Count; i++) {
						for (int j = 0; j < equips.Count - 1 - i; j++) {
							equip1 = equips [j] as Equip;
							equip2 = equips [j + 1] as Equip;
							if (equip1.getPower () < equip2.getPower ())
								swap (equips, j, j + 1);
						}
					}
					break;
				default:
					break;
				}
			}
		}
	}
	
	//执行筛选
	private void propSift (ArrayList props, Condition[] conditions)
	{
		List<int> selects;
		for (int i = 0; i < conditions.Length; i++) {
			selects = conditions [i].getConditions ();
			switch (conditions [i].getType ()) {
			case SortType.PROP_TYPE:
				for (int j = 0; j < props.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((props [j] as Prop).getSiftType () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							props.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			case SortType.QUALITY:
				for (int j = 0; j < props.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((props [j] as Prop).getQualityId () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							props.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			default:
				break;
			}
		}
	}

	//执行星魂筛选
	private void starSoulSift (ArrayList starSouls, Condition[] conditions)
	{
		List<int> selects;
		//先执行筛选
		for (int i = 0; i < conditions.Length; i++) {
			selects = conditions [i].getConditions ();
			switch (conditions [i].getType ()) {
			case SortType.QUALITY:
				for (int j = 0; j < starSouls.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((starSouls [j] as StarSoul).getQualityId () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							starSouls.RemoveAt (j);
							j--;
						}
					}
				}
				break;
			case SortType.EQUIP_STATE:
				for (int j = 0; j < starSouls.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((starSouls [j] as StarSoul).checkState(selects [k]))
							break;
						else if (k >= selects.Count - 1) {
							starSouls.RemoveAt (j);
							j--;
						}
					}
				}
				break; 
			case SortType.STARSOUL_TYPE:
				for (int j = 0; j < starSouls.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((starSouls [j] as StarSoul).getStarSoulType() == selects [k]){
							starSouls.RemoveAt(j);
							j--;
							break;
						}
					}
				}
				break; 
			default:
				break;
			}
		}
	}
	//执行星魂排序
	private void starSoulOrder (ArrayList starSouls, Condition condition)
	{
		if (condition.getType () == SortType.SORT) {
			StarSoul starSoul1, starSoul2;
			for (int k=0; k<condition.getConditions().Count; k++) {
				switch (condition.getConditions () [k]) {
				case SortType.SORT_LEVELUP:
					for (int i = 0; i < starSouls.Count; i++) {
						for (int j = 0; j < starSouls.Count - 1 - i; j++) {
							starSoul1 = starSouls [j] as StarSoul;
							starSoul2 = starSouls [j + 1] as StarSoul;
							if (starSoul1.getEXP () > starSoul2.getEXP ())
								swap (starSouls, j, j + 1);
						}
					}
					break;
				case SortType.SORT_LEVELDOWN:
					for (int i = 0; i < starSouls.Count; i++) {
						for (int j = 0; j < starSouls.Count - 1 - i; j++) {
							starSoul1 = starSouls [j] as StarSoul;
							starSoul2 = starSouls [j + 1] as StarSoul;
							if (starSoul1.getEXP () < starSoul2.getEXP ())
								swap (starSouls, j, j + 1);
						}
					}
					break;
				case SortType.SORT_QUALITYUP:
					for (int i = 0; i < starSouls.Count; i++) {
						for (int j = 0; j < starSouls.Count - 1 - i; j++) {
							starSoul1 = starSouls [j] as StarSoul;
							starSoul2 = starSouls [j + 1] as StarSoul;
							if (starSoul1.getQualityId () > starSoul2.getQualityId ())
								swap (starSouls, j, j + 1);
						}
					}
					break;
				case SortType.SORT_QUALITYDOWN:
					for (int i = 0; i < starSouls.Count; i++) {
						for (int j = 0; j < starSouls.Count - 1 - i; j++) {
							starSoul1 = starSouls [j] as StarSoul;
							starSoul2 = starSouls [j + 1] as StarSoul;
							if (starSoul1.getQualityId () < starSoul2.getQualityId ())
								swap (starSouls, j, j + 1);
						}
					}
					break;
				case SortType.SORT_POWERUP:
					for (int i = 0; i < starSouls.Count; i++) {
						for (int j = 0; j < starSouls.Count - 1 - i; j++) {
							starSoul1 = starSouls [j] as StarSoul;
							starSoul2 = starSouls [j + 1] as StarSoul;
							if (starSoul1.getPower () > starSoul2.getPower ())
								swap (starSouls, j, j + 1);
						}
					}
					break;
				case SortType.SORT_POWERDOWN:
					for (int i = 0; i < starSouls.Count; i++) {
						for (int j = 0; j < starSouls.Count - 1 - i; j++) {
							starSoul1 = starSouls [j] as StarSoul;
							starSoul2 = starSouls [j + 1] as StarSoul;
							if (starSoul1.getPower () < starSoul2.getPower ())
								swap (starSouls, j, j + 1);
						}
					}
					break;
				default:
					break;
				}
			}
		}
	}
	
	//执行排序
	private void propOrder (ArrayList props, Condition condition)
	{
		if (condition.getType () == SortType.SORT) {
			Prop prop1, prop2;
			for (int k=0; k<condition.getConditions().Count; k++) {
				switch (condition.getConditions () [k]) {
				case SortType.SORT_QUALITYUP:
					for (int i = 0; i < props.Count; i++) {
						for (int j = 0; j < props.Count - 1 - i; j++) {
							prop1 = props [j] as Prop;
							prop2 = props [j + 1] as Prop;
							if (prop1.getQualityId () > prop2.getQualityId ())
								swap (props, j, j + 1);
							else if (prop1.getQualityId () == prop2.getQualityId () && prop1.sid > prop2.sid)
								swap (props, j, j + 1);
						}
					}
					break;
				case SortType.SORT_QUALITYDOWN:
					for (int i = 0; i < props.Count; i++) {
						for (int j = 0; j < props.Count - 1 - i; j++) {
							prop1 = props [j] as Prop;
							prop2 = props [j + 1] as Prop;
							if(prop1.GetOrderId() < prop2.GetOrderId())
								swap (props, j, j + 1);
							else if (prop1.GetOrderId() == prop2.GetOrderId() && prop1.getQualityId () < prop2.getQualityId ())
								swap (props, j, j + 1);
							else if (prop1.GetOrderId() == prop2.GetOrderId() && prop1.getQualityId () == prop2.getQualityId () && prop1.sid > prop2.sid)
								swap (props, j, j + 1);
						}
					}
					break;
				default:
					break;
				}
			}
		}
	}
	
	private void goodsSift (ArrayList goods, Condition[] conditions)
	{
		List<int> selects;
		for (int i = 0; i < conditions.Length; i++) {
			selects = conditions [i].getConditions ();
			switch (conditions [i].getType ()) {
			case SortType.GOODS_TYPE:
				for (int j = 0; j < goods.Count; j++) {
					for (int k = 0; k < selects.Count; k++) {
						if ((goods [j] as Goods).getGoodsType () == selects [k])
							break;
						else if (k >= selects.Count - 1) {
							goods.RemoveAt (j);
							j--;
						}
					}
				}
				break;
//			case SortType.EQUIP_PART:
//				for (int j = 0; j < goods.Count; j++) {
//					for (int k = 0; k < selects.Count; k++) {
//						if ((goods [j] as Goods).getGoodsType () == GoodsType.EQUIP && (goods [j] as Equip).getPartId () == selects [k])
//							break;
//						else if (k >= selects.Count - 1) {
//							goods.RemoveAt (j);
//							j--;
//						}
//					}
//				}
//				break;
			default:
				break;
			}
		}
	}
	
	private void goodsOrder (ArrayList goods, Condition condition)
	{
		if (condition.getType () == SortType.SORT) {
			//排序只有一个条件
			Goods goods1, goods2;
			for (int k=0; k<condition.getConditions().Count; k++) {
				switch (condition.getConditions () [k]) {
				case SortType.SORT_CONTRIBUTIONUP:
					for (int i = 0; i < goods.Count; i++) {
						for (int j = 0; j < goods.Count - 1 - i; j++) {
							goods1 = goods [j] as Goods;
							goods2 = goods [j + 1] as Goods;
							if (goods1.getCostPrice () > goods2.getCostPrice ())
								swap (goods, j, j + 1);
							else if (goods1.getCostPrice () == goods2.getCostPrice () && goods1.sid > goods2.sid)
								swap (goods, j, j + 1);
						}
					}
					break;
				case SortType.SORT_CONTRIBUTIONDOWN:
					for (int i = 0; i < goods.Count; i++) {
						for (int j = 0; j < goods.Count - 1 - i; j++) {
							goods1 = goods [j] as Goods;
							goods2 = goods [j + 1] as Goods;
							if (goods1.getCostPrice () < goods2.getCostPrice ())
								swap (goods, j, j + 1);
							else if (goods1.getCostPrice () == goods2.getCostPrice () && goods1.sid < goods2.sid)
								swap (goods, j, j + 1);
						}
					}
					break;
				case SortType.SORT_ORDER:
					for (int i = 0; i < goods.Count; i++) {
						for (int j = 0; j < goods.Count - 1 - i; j++) {
							goods1 = goods [j] as Goods;
							goods2 = goods [j + 1] as Goods;
							if (goods1.getOrder () > goods2.getOrder ())
								swap (goods, j, j + 1);
							else if (goods1.getOrder () == goods2.getOrder () && goods1.sid > goods2.sid)
								swap (goods, j, j + 1);
						}
					}
					break;
				default:
					break;
				}
			}
		}
	}

	// 交换
	private void swap (ArrayList list, int left, int right)
	{
		Object temp;  
		temp = list [left];
		list [left] = list [right];
		list [right] = temp;
	}
	
}