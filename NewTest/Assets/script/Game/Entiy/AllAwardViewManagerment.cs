using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/**
 * 查看奖励管理器
 * @authro 陈世惟  
 * */
public class AllAwardViewManagerment
{

	private PrizeSample[] prizes;
	private const string TYPE_RMB = "rmb";//奖励类型 人民币
	private const string TYPE_CARD = "card";//奖励类型 卡片 角色+召唤兽
	private const string TYPE_PROP = "prop";//一般道具
	private const string TYPE_EQUIP = "equip";//装备
	private const string TYPE_CARD_EXP = "card_exp";//卡片经验值
	private const string TYPE_EXP = "exp";//玩家经验值
	private const string TYPE_MONEY = "money";//金币
	private const string TYPE_STARSOUL = "star_soul";//星魂
	private const string TYPE_STARSOUL_DEBRIS = "debris";//星魂碎片
	private const string TYPE_PRESTIGE = "prestige";//声望
    private const string TYPE_MAGICWEAPON = "artifact";//神器

	public static AllAwardViewManagerment Instance {
		get{ return SingleManager.Instance.getObj ("AllAwardViewManagerment") as AllAwardViewManagerment;}
	}
	


	public void setNull ()
	{
		prizes = null;
	}
	
	//返回前台所有奖励信息
	public PrizeSample[] getAwardsFPort (List<PrizeSample> _prizes)
	{
		prizes = new PrizeSample[_prizes.Count];
		PrizeSample ps = null;
		
		for (int i = 0; i < _prizes.Count; i++) {
			ps = new PrizeSample ();
			ps.type = _prizes [i].type;
			
			if (typeChange (ps.type) == TYPE_RMB) {
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_CARD) {
				ps.pSid = _prizes [i].pSid;
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_PROP) {
				ps.pSid = _prizes [i].pSid;
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_EQUIP) {
				ps.pSid = _prizes [i].pSid;
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_STARSOUL) {
				ps.pSid = _prizes [i].pSid;
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_EXP) {
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_MONEY) {
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_STARSOUL) {
				ps.num = _prizes [i].getPrizeNumByString ();
			} else if (typeChange (ps.type) == TYPE_STARSOUL_DEBRIS) {
				ps.num = _prizes [i].getPrizeNumByString ();
			}
			prizes [i] = ps;
		}
		return prizes;
	}
	
	//注入奖励
	//添加新奖励类型时需要修改此方法
	public PrizeSample[] addAwards (ErlType _type)
	{
		ErlArray arr = _type as ErlArray;
		PrizeSample ps = null;
		PrizeSample[] prizes = new PrizeSample[arr.Value.Length];
			
		for (int i = 0; i < arr.Value.Length; i++) {
			ps = new PrizeSample ();
			ErlArray array1 = arr.Value [i] as ErlArray;
			string type = (array1.Value [0] as ErlType).getValueString ();
			ps.type = typeChange (type);
			if (type == TYPE_RMB) {
				ps.num = (array1.Value [1] as ErlType).getValueString (); 
			} else if (type == TYPE_CARD) {
				ps.pSid = StringKit.toInt ((array1.Value [1] as ErlType).getValueString ()); 
				ps.num = (array1.Value [2] as ErlType).getValueString ();
			} else if (type == TYPE_PROP) {
				ps.pSid = StringKit.toInt ((array1.Value [1] as ErlType).getValueString ());
				ps.num = (array1.Value [2] as ErlType).getValueString ();
			} else if (type == TYPE_EQUIP) {
				ps.pSid = StringKit.toInt ((array1.Value [1] as ErlType).getValueString ()); 
				ps.num = (array1.Value [2] as ErlType).getValueString ();
			} else if (type == TYPE_STARSOUL) {
				ps.pSid = StringKit.toInt ((array1.Value [1] as ErlType).getValueString ()); 
				ps.num = (array1.Value [2] as ErlType).getValueString ();
			} else if (type == TYPE_EXP) {
				ps.num = (array1.Value [1] as ErlType).getValueString (); 
			} else if (type == TYPE_MONEY) {
				ps.num = (array1.Value [1] as ErlType).getValueString (); 
			} else if (type == TYPE_STARSOUL_DEBRIS) {
				ps.num = (array1.Value [1] as ErlType).getValueString ();
            } else if (type == TYPE_MAGICWEAPON) {
                ps.num = (array1.Value[1] as ErlType).getValueString();
            }
			prizes [i] = ps;
		}
		return prizes;
	}

	public PrizeSample[] exchangeAwards (Award[] award)
	{
		List<PrizeSample> list = new List<PrizeSample> ();
		for (int i = 0; i < award.Length; i++) {
			for (int j = 0; j < award[i].props.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_PROP, award [i].props [j].sid, award [i].props [j].num);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].equips.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_EQUIPMENT, award [i].equips [j].sid, 1);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].cards.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_CARD, award [i].cards [j].sid, 1);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].starsouls.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL, award [i].starsouls [j].sid, 1);
				list.Add (sample);
			}
			if (award [i].moneyGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_MONEY, 0, award [i].moneyGap);
				list.Add (sample);
			}
			if (award [i].rmbGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_RMB, 0, award [i].rmbGap);
				list.Add (sample);
			}
			if (award [i].starsoulDebrisGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL_DEBRIS, 0, award [i].starsoulDebrisGap);
				list.Add (sample);
			}
            for (int j = 0; j < award[i].magicWeapons.Count; j++) {
                PrizeSample sample = new PrizeSample(PrizeType.PRIZE_MAGIC_WEAPON, award[i].magicWeapons[j].sid, 1);
                list.Add(sample);
            }
		}

		return list.ToArray ();;
	}


	/// <summary>
	/// 奖励归集
	/// </summary>
	public List<PrizeSample> contrastList (List<PrizeSample> oldList)
	{
		
		List<PrizeSample> newList = new List<PrizeSample> ();
		
		if (oldList == null)
			return null;
		if (oldList.Count == 1)
			return oldList;
		
		foreach (PrizeSample oldInfo in oldList) {
			if (newList != null) {
				int b = 0;
				for (int a = 0; a < newList.Count; a++) {
					//卡片就不用归集了
					if (oldInfo.type != PrizeType.PRIZE_CARD && oldInfo.type != PrizeType.PRIZE_BEAST) {
						if (newList [a].pSid == oldInfo.pSid && newList [a].type == oldInfo.type) {
							b = 1;
							newList[a].addNum(oldInfo.getPrizeNumByInt());
							break;
						}
					}
				}
				if (b != 1) {
					newList.Add (oldInfo);
				}
			} else {
				newList.Add (oldInfo);
			}
		}
		newList = Sort(newList);
		return newList;	
	}

	public List<PrizeSample> exchagneAwardsByAnnex (Annex[] annex)
	{
		if (annex == null) {
			return null;
		}
		List<PrizeSample> list = new List<PrizeSample> ();
		for (int i = 0; i < annex.Length; i++) {
			list.Add (getPrize (annex[i]));
		}
		return list;
	}

	private PrizeSample getPrize(Annex annex)
	{
        if (annex.exp != null)
            return annex.exp;
        else if (annex.money != null)
            return annex.money;
        else if (annex.prop != null)
            return annex.prop;
        else if (annex.pve != null)
            return annex.pve;
        else if (annex.pvp != null)
            return annex.pvp;
        else if (annex.rmb != null)
            return annex.rmb;
        else if (annex.starsoulDebris != null)
            return annex.starsoulDebris;
        else if (annex.ladder != null)
            return annex.ladder;
        else if (annex.contribution != null)
            return annex.contribution;
        else if (annex.magicWeapon != null)
            return annex.magicWeapon;
        else
            return null;
	}

	public List<PrizeSample> exchangeAwardsToPrize (Award[] award)
	{
		List<PrizeSample> list = new List<PrizeSample> ();
		for (int i = 0; i < award.Length; i++) {
			for (int j = 0; j < award[i].props.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_PROP, award [i].props [j].sid, award [i].props [j].num);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].equips.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_EQUIPMENT, award [i].equips [j].sid, 1);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].cards.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_CARD, award [i].cards [j].sid, 1);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].starsouls.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL, award [i].starsouls [j].sid, 1);
				list.Add (sample);
			}
			if (award [i].moneyGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_MONEY, 0, award [i].moneyGap);
				list.Add (sample);
			}
			if (award [i].rmbGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_RMB, 0, award [i].rmbGap);
				list.Add (sample);
			}
			if (award [i].starsoulDebrisGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL_DEBRIS, 0, award [i].starsoulDebrisGap);
				list.Add (sample);
			}
			if (award [i].meritGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_MERIT, 0, award [i].meritGap);
				list.Add (sample);
			}
            for (int j = 0; j < award[i].magicWeapons.Count; j++) {
                PrizeSample sample = new PrizeSample(PrizeType.PRIZE_MAGIC_WEAPON, award[i].magicWeapons[j].sid, 1);
                list.Add(sample);
            }
		}
		return list;
	}
	public List<PrizeSample> exchangeArrayToList(PrizeSample[] prizes){
		List<PrizeSample> list = new List<PrizeSample> ();
		if (prizes != null) {
			for(int i=0; i<prizes.Length;i++)
			{
				list.Add(prizes[i]);
			}
		}
		return list;
	}
	public List<PrizeSample> exchangeAwardToPrize (Award award)
	{
		List<PrizeSample> list = new List<PrizeSample> ();
		for (int j = 0; j < award.props.Count; j++) {
			PrizeSample sample = new PrizeSample (PrizeType.PRIZE_PROP, award.props [j].sid, award.props [j].num);
			list.Add (sample);
		}
		for (int j = 0; j < award.equips.Count; j++) {
			PrizeSample sample = new PrizeSample (PrizeType.PRIZE_EQUIPMENT, award.equips [j].sid, 1);
			list.Add (sample);
		}
		for (int j = 0; j < award.cards.Count; j++) {
			PrizeSample sample = new PrizeSample (PrizeType.PRIZE_CARD, award.cards [j].sid, 1);
			list.Add (sample);
		}
		for (int j = 0; j < award.starsouls.Count; j++) {
			PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL, award.starsouls [j].sid, 1);
			list.Add (sample);
		}
		if (award.moneyGap > 0) {
			PrizeSample sample = new PrizeSample (PrizeType.PRIZE_MONEY, 0, award.moneyGap);
			list.Add (sample);
		}
		if (award.rmbGap > 0) {
			PrizeSample sample = new PrizeSample (PrizeType.PRIZE_RMB, 0, award.rmbGap);
			list.Add (sample);
		}
		if (award.starsoulDebrisGap > 0) {
			PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL_DEBRIS, 0, award.starsoulDebrisGap);
			list.Add (sample);
		}
		if(award.prestigeGap>0){
			PrizeSample sample =new PrizeSample(PrizeType.PRIZE_PRESTIGE,0,award.prestigeGap);
			list.Add (sample);
		}
        for (int j = 0; j < award.magicWeapons.Count; j++) {
            PrizeSample sample = new PrizeSample(PrizeType.PRIZE_MAGIC_WEAPON, award.magicWeapons[j].sid, 1);
            list.Add(sample);
        }
		return list;
	}
	public PrizeSample[] addAwards (Award[] award)
	{
		List<PrizeSample> list = new List<PrizeSample> ();
		for (int i = 0; i < award.Length; i++) {
			for (int j = 0; j < award[i].props.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_PROP, award [i].props [j].sid, award [i].props [j].num);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].equips.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_EQUIPMENT, award [i].equips [j].sid, 1);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].cards.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_CARD, award [i].cards [j].sid, 1);
				list.Add (sample);
			}
			for (int j = 0; j < award[i].starsouls.Count; j++) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL, award [i].starsouls [j].sid, 1);
				list.Add (sample);
			}
			if (award [i].moneyGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_MONEY, 0, award [i].moneyGap);
				list.Add (sample);
			}
			if (award [i].rmbGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_RMB, 0, award [i].rmbGap);
				list.Add (sample);
			}
			if (award [i].starsoulDebrisGap > 0) {
				PrizeSample sample = new PrizeSample (PrizeType.PRIZE_STARSOUL_DEBRIS, 0, award [i].starsoulDebrisGap);
				list.Add (sample);
			}
            for (int j = 0; j < award[i].magicWeapons.Count; j++) {
                PrizeSample sample = new PrizeSample(PrizeType.PRIZE_MAGIC_WEAPON, award[i].magicWeapons[j].sid, 1);
                list.Add(sample);
            }
		}
		
		return list.ToArray ();
	}

	/// <summary>
	/// 奖励归集
	/// </summary>
	public Award contrastAward (Award _award)
	{
		//归集物品
		List<PropAward> newProps = new List<PropAward> ();

		foreach (PropAward item in _award.props) {
			if (newProps != null) {
				bool haveBool = false;
				for (int a = 0; a < newProps.Count; a++) {
					if (newProps [a].sid == item.sid) {
						haveBool = true;
						newProps [a].num = newProps [a].num + item.num;
						break;
					}
				}
				if (!haveBool) {
					newProps.Add (item);
				}
			} else {
				newProps.Add (item);
			}
		}

		//归集装备
		List<EquipAward> newEquips = new List<EquipAward> ();

		foreach (EquipAward item in _award.equips) {
			if (newEquips != null) {
				bool haveBool = false;
				for (int a = 0; a < newEquips.Count; a++) {
					if (newEquips [a].sid == item.sid) {
						haveBool = true;
						newEquips [a].num = newEquips [a].num + item.num;
						break;
					}
				}
				if (!haveBool) {
					newEquips.Add (item);
				}
			} else {
				newEquips.Add (item);
			}
		}
		_award.props = newProps;
		_award.equips = newEquips;

		return _award;
	}

	/// <summary>
	/// 奖励归集
	/// </summary>
	public PrizeSample[] contrastToArray (PrizeSample[] oldList)
	{
		return contrastToList (oldList).ToArray ();
	}

	/// <summary>
	/// 奖励归集
	/// </summary>
	public List<PrizeSample> contrastToList (PrizeSample[] oldList)
	{
		return contrastToList (oldList.ToList ());
	}

	/// <summary>
	/// 奖励归集
	/// </summary>
	public List<PrizeSample> contrastToList (List<PrizeSample> oldList)
	{
		
		List<PrizeSample> newList = new List<PrizeSample> ();
		
		if (oldList == null)
			return null;
		if (oldList.Count == 1)
			return oldList;
		
		foreach (PrizeSample oldInfo in oldList) {
			if (newList != null) {
				int b = 0;
				for (int a = 0; a < newList.Count; a++) {
					//卡片就不用归集了
					if (oldInfo.type != PrizeType.PRIZE_CARD && oldInfo.type != PrizeType.PRIZE_BEAST) {
						if (newList [a].pSid == oldInfo.pSid && newList [a].type == oldInfo.type) {
							b = 1;
							newList [a].addNum(oldInfo.getPrizeNumByInt());
							break;
						}
					}
				}
				if (b != 1) {
					newList.Add (oldInfo);
				}
			} else {
				newList.Add (oldInfo);
			}
		}
		newList = Sort(newList);
		return newList;	
	}

    /// <summary>
    /// 奖励排序
    /// </summary>
    public List<PrizeSample> Sort(List<PrizeSample> oldList)
    {
		/* 
		 * 显示物品从左到右按品质顺序显示
		 * 相同品质,按装备→卡片→道具→碎片的顺序显示
		 * 装备类按照武器→衣服→鞋子→头盔→戒指部位排序
		 */
        oldList.Sort((PrizeSample a, PrizeSample b) => {
            if (a == null) return 1;
            if (b == null) return -1;
            int quality1 = a.getQuality();
            int quality2 = b.getQuality();
            if (quality1 == quality2) {
				return getSortIndex (a) < getSortIndex (b) ? -1 : 1;
			}
            else
                return quality1 > quality2 ? -1 : 1;
        });
        return oldList;
    }

	/// <summary>
	/// 奖励排序,从低到高
	/// </summary>
	public int getSortIndex (PrizeSample tmpPs)
	{
		//1-5
		if (tmpPs.type == PrizeType.PRIZE_EQUIPMENT) {
			EquipSample tmpEq = EquipmentSampleManager.Instance.getEquipSampleBySid (tmpPs.pSid);
			return tmpEq.partId;
		}
		else if (tmpPs.type == PrizeType.PRIZE_CARD) {
			return 6;
		}
		else if (tmpPs.type == PrizeType.PRIZE_PROP) {
			Prop ps = PropManagerment.Instance.createProp (tmpPs.pSid);
			if (!ps.isScrap ()) {
				return 7;
			} else {
				return 8;
			}
		}
		else if (tmpPs.type == PrizeType.PRIZE_STARSOUL) {
			return 9;
		}
		else if (tmpPs.type == PrizeType.PRIZE_STARSOUL_DEBRIS) {
			return 10;
		}
		else {
			return 11;
		}
	}


	/// <summary>
	/// 查看领取这个奖励会不会导致爆仓
	/// </summary>
	public bool isFull (List<PrizeSample> _prizes)
	{
		return isFull(_prizes.ToArray ());
	}

	/// <summary>
	/// 查看领取这个奖励会不会导致爆仓
	/// </summary>
	public bool isFull (PrizeSample[] _prizes)
	{
		for (int i = 0; i < _prizes.Length; i++) {
			switch (_prizes[i].type) {
			case PrizeType.PRIZE_BEAST:
				if ((StorageManagerment.Instance.getAllBeast ().Count + _prizes[i].getPrizeNumByInt ()) > StorageManagerment.Instance.getBeastStorageMaxSpace ()) {
					return true;
				} else {
					break;
				}
			case PrizeType.PRIZE_CARD:
				if ((StorageManagerment.Instance.getAllRole ().Count + _prizes[i].getPrizeNumByInt ()) > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
					return true;
				} else {
					break;
				}
			case PrizeType.PRIZE_EQUIPMENT:
				if ((StorageManagerment.Instance.getAllEquip ().Count + _prizes[i].getPrizeNumByInt ()) > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
					return true;
				} else {
					break;
				}
            case PrizeType.PRIZE_MAGIC_WEAPON:
                if (StorageManagerment.Instance.getAllMagicWeapon().Count + _prizes[i].getPrizeNumByInt() > StorageManagerment.Instance.getMagicWeaponStorageMaxSpace()) {
                    return true;
                } else
                    break;          
			case PrizeType.PRIZE_PROP:
				if ((StorageManagerment.Instance.getAllProp ().Count + _prizes[i].getPrizeNumByInt ()) > StorageManagerment.Instance.getPropStorageMaxSpace ()) {
					return true;
				} else {
					break;
				}
			case PrizeType.PRIZE_STARSOUL:
				if ((StorageManagerment.Instance.getAllStarSoul ().Count + _prizes[i].getPrizeNumByInt ()) > StorageManagerment.Instance.getStarSoulStorageMaxSpace ()) {
					return true;
				} else {
					break;
				}
			default:
				return false;
			}
		}
		return false;
	}

	/// <summary>
	/// 查看领取这个奖励会不会导致爆仓
	/// </summary>
	public bool isFull (PrizeSample _prizes)
	{
		PrizeSample[] psArray = new PrizeSample[1];
		psArray[0] = _prizes;
		return isFull(psArray);
	}

	/// <summary>
	/// 查看领取这个奖励会不会导致爆仓
	/// </summary>
	public bool isFull (Award[] _award)
	{
		return isFull(exchangeAwards(_award));
	}

	/// <summary>
	/// 查看领取这个奖励会不会导致爆仓
	/// </summary>
	public bool isFull (Award _award)
	{
		Award[] awardArray = new Award[1];
		awardArray[0] = _award;
		return isFull(exchangeAwards(awardArray));
	}
	
	//根据奖励信息返回奖励名称
	public string getNameByType (PrizeSample _ps)
	{
		switch (_ps.type) {
		case 5:
			return CardSampleManager.Instance.getRoleSampleBySid (_ps.pSid).name;
		case 6:
			return CardSampleManager.Instance.getRoleSampleBySid (_ps.pSid).name;
		case 4:
			return EquipmentSampleManager.Instance.getEquipSampleBySid (_ps.pSid).name;
		case 1:
			return LanguageConfigManager.Instance.getLanguage ("s0049");
		case 2:
			return LanguageConfigManager.Instance.getLanguage ("s0048");
		case 3:
			return PropSampleManager.Instance.getPropSampleBySid (_ps.pSid).name;
		case PrizeType.PRIZE_STARSOUL_DEBRIS:
			return LanguageConfigManager.Instance.getLanguage ("s0466");
		case PrizeType.PRIZE_STARSOUL:
			return StarSoulSampleManager.Instance.getStarSoulSampleBySid (_ps.pSid).name;
            case PrizeType.PRIZE_MAGIC_WEAPON:
            return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(_ps.pSid).name;
		default:
			return "";
		}
	}
	
	private int typeChange (string str)
	{
		switch (str) {
		case "card":
			return 5;
		case "beast":
			return 6;
		case "equip":
			return 4;
		case "prop":
			return 3;
		case "money":
			return 1;
		case "rmb":
			return 2;
		case "goods":
			return 3;
		case TYPE_STARSOUL:
			return PrizeType.PRIZE_STARSOUL;
		case TYPE_STARSOUL_DEBRIS:
			return PrizeType.PRIZE_STARSOUL_DEBRIS;
		case "prestige":
			return 20;
		default:
			return StringKit.toInt (str);
				
		}
	}
	
	private string typeChange (int type)
	{
		switch (type) {
		case 5:
			return "card";
		case 6:
			return "beast";
		case 4:
			return "equip";
//		case 3:
//			return "goods";
		case 1:
			return "money";
		case 2:
			return "rmb";
		case 3:
			return "prop";
		case PrizeType.PRIZE_STARSOUL:
			return TYPE_STARSOUL;
		case PrizeType.PRIZE_STARSOUL_DEBRIS:
			return TYPE_STARSOUL_DEBRIS;
		case 20:
			return "prestige";
		default:
			return "";
				
		}
	}
}
