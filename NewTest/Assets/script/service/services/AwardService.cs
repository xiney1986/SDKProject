using System;
/**
 * 奖励服务
 * @author longlingquan
 * */
using System.Collections.Generic;

public class AwardService:BaseFPort
{
	
	private const string TYPE_RMB = "rmb";//奖励类型 人民币
	private const string TYPE_CARD = "card";//奖励类型 卡片 角色
	private const string TYPE_BEAST = "beast";//奖励类型 卡片 召唤兽
	private const string TYPE_PROP = "goods";//一般道具
	private const string TYPE_EQUIP = "equip";//装备
	private const string TYPE_CARD_EXP = "card_exp";//卡片经验值
	private const string TYPE_EXP = "exp";//玩家经验值
	private const string TYPE_MONEY = "money";//金币
	private const string TYPE_HONOR = "honor";//荣誉
	private const string TYPE_INTEGRAL = "arena_score";//积分
	private const string TYPE_MERIT = "merit";//功勋
	private const string TYPE_STAR = "star_score";//星屑
	private const string TYPE_CONTRIBUTION = "contribution";//公会贡献
	private const string TYPE_PK = "pvp_num";//挑战次数	
	private const string TYPE_LUCKSTAR = "lucky_star";//幸运星
	private const string TYPE_PRESTIGE = "prestige";//声望
	private const string KEY_ARGU = "argu";//奖励信息
	private const string KEY_TARGET = "target";//奖励类型
	private const string KEY_WARPOINT = "warPoint";//战争点
    private const string KEY_MAGICWEAPON = "artifact";//神器
	private const string TYPE_GODSWAR_INTEGRAL = "godsWar_score";//积分
	private const string TYPE_LASTBATTLE_SCORE = "armageddon_score";// 末日决战积分//
	
	public AwardService ()
	{
		 
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlArray arr = message.getValue (KEY_ARGU) as ErlArray;
		string type = (message.getValue (KEY_TARGET) as  ErlAtom).Value;
		
		Award[] awards = new Award[arr.Value.Length];
		ErlArray arr1;
		try {
			for (int i =0; i<awards.Length; i++) {
				arr1 = arr.Value [i] as ErlArray;
				awards [i] = new Award ();
				awards [i].type = (arr1.Value [0]).getValueString ();
				parse (arr1.Value [1] as ErlArray, awards [i]);
			}
		} catch (System.Exception ex) {
			UnityEngine.Debug.LogError ("ex:" + message.toJsonString ());
		}
//		MonoBase.print ("awardType-------------------------" + type);
		AwardManagerment.Instance.addAwards (type, awards); 


	}
	public void parseAward (ErlList date)
	{
		for (int i = 0; i < date.Value.Length; i++)
			parseAward (date.Value [i] as ErlArray);
	}
    //副本pve 副本pvp 秒杀奖励就放在这个位置
    public void parseMAward(ErlList date) {
        for (int i = 0; i < date.Value.Length; i++)
            parseMAward(date.Value[i] as ErlArray);
    }

    public void parseMAward(ErlArray date) {
        string type = (date.Value[0] as ErlType).getValueString();
        ErlArray arr = date.Value[1] as ErlArray;
        Award[] awards = new Award[arr.Value.Length];
        ErlArray arr1;
        for (int i = 0; i < awards.Length; i++) {
            arr1 = arr.Value[i] as ErlArray;
            awards[i] = new Award();
            awards[i].type = (arr1.Value[0]).getValueString();
            parse(arr1.Value[1] as ErlArray, awards[i]);
        }
        if (type == AwardManagerment.AWARDS_PVP_DOUBLE)//把连胜奖励分离出来
        {
            AwardManagerment.Instance.addAwards(type, awards);
        }
        else
        {
            List<Award> lsit = new List<Award>();
            if (AwardManagerment.Instance.miaoShaAward != null && AwardManagerment.Instance.miaoShaAward.Length > 0)
                lsit.AddRange(AwardManagerment.Instance.miaoShaAward);
            lsit.AddRange(awards);
            AwardManagerment.Instance.miaoShaAward = lsit.ToArray();
        }
    }
	public void parseAward (ErlArray date)
	{ 
		string type = (date.Value [0] as ErlType).getValueString ();
		ErlArray arr = date.Value [1] as ErlArray;
		Award[] awards = new Award[arr.Value.Length];
		ErlArray arr1;
		for (int i =0; i<awards.Length; i++) {
			arr1 = arr.Value [i] as ErlArray;
			awards [i] = new Award ();
			awards [i].type = (arr1.Value [0]).getValueString ();
			parse (arr1.Value [1] as ErlArray, awards [i]);
		}  
		AwardManagerment.Instance.addAwards (type, awards);
	}
	
	public void parse (ErlArray array, Award award)
	{ 
		ErlArray arr;
		for (int i = 0; i < array.Value.Length; i++) {
			arr = array.Value [i] as ErlArray;
			string type = (arr.Value [0]).getValueString ();
			if (type == TYPE_RMB) { 
				int rmb = StringKit.toInt (arr.Value [1].getValueString ()); 
				award.awardRmb = rmb;
			} else if (type == TYPE_CARD) {
				Card c = CardManagerment.Instance.createCard ();
				c.bytesRead (0, arr.Value [1] as ErlArray);
				StorageManagerment.Instance.addCardProp (c);
				CardAward card = new CardAward ();
				card.id = c.uid; 
				card.sid = c.sid;
				award.cards.Add (card);
			} else if (type == TYPE_BEAST) {
				Card c = CardManagerment.Instance.createCard ();
				c.bytesRead (0, arr.Value [1] as ErlArray);
				StorageManagerment.Instance.addBeastProp (c);
				CardAward card = new CardAward ();
				card.id = c.uid; 
				card.sid = c.sid;
				award.cards.Add (card);
			} else if (type == TYPE_PROP) {
				Prop p = PropManagerment.Instance.createProp ();
				p.bytesRead (1, arr);
				StorageManagerment.Instance.addGoodsProp (p);
				PropAward prop = new PropAward ();
				prop.sid = p.sid;
				prop.num = p.getNum ();
				award.props.Add (prop);
			} else if (type == TYPE_EQUIP) {
				Equip eq = EquipManagerment.Instance.createEquip ();
				eq.bytesRead (0, arr.Value [1] as ErlArray);
//				if(!ChooseTypeSampleManager.Instance.isToEat (eq, ChooseTypeSampleManager.TYPE_EQUIP_EXP))
				eq.isNew = true;
				StorageManagerment.Instance.addEquipProp (eq);
				EquipAward equip = new  EquipAward ();
				equip.id = eq.uid;
				equip.sid = eq.sid;
				award.equips.Add (equip);
            } else if (type == KEY_MAGICWEAPON) {
                MagicWeapon mc = MagicWeaponManagerment.Instance.createMagicWeapon();
                mc.bytesRead(0,arr.Value[1] as ErlArray);
                StorageManagerment.Instance.addMagicWeaponProp(mc);
                MagicwWeaponAward mcAward = new MagicwWeaponAward();
                mcAward.id = mc.uid;
                mcAward.sid = mc.sid;
                award.magicWeapons.Add(mcAward);

            }  
            else if (type == TempPropType.STARSOUL) {
				StarSoul starSoul = StarSoulManager.Instance.createStarSoul();
				starSoul.bytesRead (0, arr.Value [1] as ErlArray);
				StorageManagerment.Instance.addStarSoulStorage (starSoul,true);
				StarSoulAward starSoulAward = new  StarSoulAward ();
				starSoulAward.uid = starSoul.uid;
				starSoulAward.sid = starSoul.sid;
				award.starsouls.Add (starSoulAward);
			} else if (type == TYPE_EXP) {
				long exp = StringKit.toLong (arr.Value [1].getValueString ()); 
				award.awardExp = exp;
			} else if (type == TYPE_MONEY) {
				int money = StringKit.toInt (arr.Value [1].getValueString ()); 
				award.awardMoney = money; 
			} else if (type == TYPE_CARD_EXP) {
				EXPAward exp = new EXPAward ();
				exp.id = arr.Value [1].getValueString (); 
				exp.awardExp = StringKit.toLong (arr.Value [2].getValueString ());
				award.exps.Add (exp);
			} else if (type == TYPE_HONOR) {
				int honor = StringKit.toInt (arr.Value [1].getValueString ());
				award.awardHonor = honor;
			} else if (type == TYPE_INTEGRAL) {
				int integral = StringKit.toInt (arr.Value [1].getValueString ());
				award.awardIntegral = integral;
			} else if (type == TYPE_MERIT) {
				int merit = StringKit.toInt (arr.Value [1].getValueString ());
				award.awardMerit = merit;
			} else if (type == TYPE_STAR) {
				int starNum = StringKit.toInt (arr.Value [1].getValueString ());
				award.awardStar = starNum;
			}else if (type == TYPE_CONTRIBUTION) {
				int contribution = StringKit.toInt (arr.Value [1].getValueString ());
				award.awardCon = contribution;
			}else if (type == TYPE_PK) {
				int pkNum = StringKit.toInt (arr.Value [1].getValueString ());
				SweepManagement.Instance.initPvpNum (pkNum);
			}else if (type == TYPE_LUCKSTAR) {
				int luckStarNum = StringKit.toInt (arr.Value [1].getValueString ());
				award.awardLuckyStar = luckStarNum;
			}else if(type == TYPE_PRESTIGE) {
				int prestige = StringKit.toInt (arr.Value[1].getValueString());
				award.awardPrestige= prestige;
			}else if(type == TempPropType.STARSOUL_DEBRIS) {
				int debris = StringKit.toInt (arr.Value[1].getValueString());
                award.awardStarsoulDebris = debris;
			}
			else if(type == TYPE_GODSWAR_INTEGRAL) {
			int debris = StringKit.toInt (arr.Value[1].getValueString());
			award.godsWar_awardIntegral= debris;
			}
			else if(type == TYPE_LASTBATTLE_SCORE)
			{
				LastBattleManagement.Instance.battleScore = StringKit.toInt (arr.Value[1].getValueString());
			}
		}
	}
} 

