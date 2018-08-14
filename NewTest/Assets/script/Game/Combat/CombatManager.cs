using System;
using System.Collections.Generic;
/**
 * 战斗力管理器
 * @authro 陈世惟  
 * */
using UnityEngine;

public class CombatManager : SampleConfigManager
{
	public const int COE_HP = 1, COE_ATT = 2, COE_DEF = 3, COE_MAG = 4, COE_AGI = 5;//生命，攻击，防御，魔法，敏捷


//	private static CombatManager instance;
	private const int MOUNTS_TYPE = 7;
	private int oldCombat = 0;
	private int newCombat = 0;
	private int step = 0;
	private bool isRefreshCombat = false;
	
	public CombatManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_COMBAT);
	}
	
	public static CombatManager Instance {
		get{
			return SingleManager.Instance.getObj("CombatManager") as CombatManager;
		}
	}


	#region 队伍战斗力
	/// <summary>
	/// 队伍主力和替补总的战斗力
	/// </summary>
	/// <returns>The team_ all combat.</returns>
	/// <param name="army">Army.</param>
	public int getTeam_AllCombat (Army army)
	{
		return getTeam_MainCombat(army)+getTeam_SubstituteCombat(army);
	}
	/// <summary>
	/// 主力战斗力
	/// </summary>
	/// <returns>The team_ main combat.</returns>
	/// <param name="army">Army.</param>
	public int getTeam_MainCombat(Army army)
	{
		int cardsCombat = 0;
		double beastCombat = army.getBeast() == null ? 0 : getBeastEvolveSkillCombat (army.getBeast());
		List<Card> mainPlayers=army.getPlayersByCard();
		if (mainPlayers != null) {
			for (int i=0; i<mainPlayers.Count; i++) {
				cardsCombat += getTeamCardCombat (mainPlayers [i], army.getBeast(),army.getFormationLoctionByCardUid(mainPlayers[i].uid));
               }
		}
		return (int)(cardsCombat+beastCombat);
	}
	/// <summary>
	/// 替补战斗力
	/// </summary>
	/// <returns>The team_ substitute combat.</returns>
	public int getTeam_SubstituteCombat(Army army)
	{
		int cardsCombat = 0;
		List<Card> subPlayers=army.getAlternateByCard();
		if (subPlayers != null) {
			for (int i=0; i<subPlayers.Count; i++) {
				cardsCombat += getTeamCardCombat (subPlayers [i], army.getBeast(),army.getFormationLoctionByCardUid(subPlayers[i].uid));
			}
		}
		return cardsCombat;
	}

	/// <summary>
	/// 获取队伍单个卡片战斗力
	/// </summary>
	private int getTeamCardCombat (Card _card, Card _beastCard , int _loction)
	{
//		MonoBase.print ("--------------------teamAttrCombat>>>>>>>hp===" + getTeamAttr (_card, _beastCard, COE_HP) + ",att=" + getTeamAttr (_card, _beastCard, COE_ATT) + ",def=" + getTeamAttr (_card, _beastCard, COE_DEF)
//		                + ",mag=" + getTeamAttr (_card, _beastCard, COE_MAG) + ",agi=" + getTeamAttr (_card, _beastCard, COE_AGI) + ",skill=" + getSkillCoe (_card));
        //Debug.LogError("teamAttrCombat>>>>>>>hp===" + getTeamAttr (_card, _beastCard, COE_HP,_loction) + ",att=" + getTeamAttr (_card, _beastCard, COE_ATT,_loction) + ",def=" + getTeamAttr (_card, _beastCard, COE_DEF,_loction)
        //                + ",mag=" + getTeamAttr(_card, _beastCard, COE_MAG, _loction) + ",agi=" + getTeamAttr(_card, _beastCard, COE_AGI, _loction) + ",skill=" + getSkillCoe(_card));
		return getTeamAttr (_card, _beastCard, COE_HP, _loction) + getTeamAttr (_card, _beastCard, COE_DEF, _loction) + getTeamAttr (_card, _beastCard, COE_ATT, _loction)
			+ getTeamAttr (_card, _beastCard, COE_MAG, _loction) + getTeamAttr (_card, _beastCard, COE_AGI, _loction) + getSkillCoe (_card)+getCardStarSoulCombat(_card)
            + getCardMagicWeaponCombat(_card)+getCardEquipRefineCombat(_card);
	}

	/// <summary>
	/// 队伍卡片单属性战斗力公式
	/// </summary>
	private int getTeamAttr (Card _card, Card _beastCard, int _coeType,int _loction)
	{
        CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttrByTeam(_card);
		CardBaseAttribute attrPer = CardManagerment.Instance.getCardAllWholeAttrPerByTeam (_card);
        CardBaseAttribute attrBeast = _beastCard == null ? null : CardManagerment.Instance.getCardAllWholeAttrByTeam(_beastCard);

		//这里计算星盘属性加成
		if(_loction == 1) {
			attr.mergeCardBaseAttr(GoddessAstrolabeManagerment.Instance.getAttrByFrontInteger());
			attrPer.mergeCardBaseAttr(GoddessAstrolabeManagerment.Instance.getAttrByFrontNumber());
		} else if(_loction == 2) {
			attr.mergeCardBaseAttr(GoddessAstrolabeManagerment.Instance.getAttrByMiddleInteger());
			attrPer.mergeCardBaseAttr(GoddessAstrolabeManagerment.Instance.getAttrByMiddleNumber());
		} else {
			attr.mergeCardBaseAttr(GoddessAstrolabeManagerment.Instance.getAttrByBehindInteger());
			attrPer.mergeCardBaseAttr(GoddessAstrolabeManagerment.Instance.getAttrByBehindNumber());
		}

//		MonoBase.print ("--------------------teamAttrPer>>>>>>>hp===" + attrPer.perHp + ",att=" + attrPer.perAttack + ",def=" + attrPer.perDefecse
//		                + ",mag=" + attrPer.perMagic + ",agi=" + attrPer.perAgile);

		switch (_coeType) {
		case COE_HP:
			double hp = attr.getWholeHp ();
			double beastHp = _beastCard == null ? 0 : attrBeast.getWholeHp ();
			double coeHpPer = attrPer.perHp;
			double coeHp = getCombatSampleBySid (_card.getJob ()).hpCoe;
			return getTeamAttrsCombat (_card, hp, beastHp, coeHpPer, coeHp);

		case COE_ATT:
			double att = attr.getWholeAtt ();
			double beastAtt = _beastCard == null ? 0 : attrBeast.getWholeAtt ();
			double coeAttPer = attrPer.perAttack;
			double coeAtt = getCombatSampleBySid (_card.getJob ()).attCoe;
			return getTeamAttrsCombat (_card, att, beastAtt, coeAttPer, coeAtt);

		case COE_DEF:
			double def = attr.getWholeDEF ();
			double beastDef = _beastCard == null ? 0 : attrBeast.getWholeDEF ();
			double coeDefPer = attrPer.perDefecse;
			double coeDef = getCombatSampleBySid (_card.getJob ()).defCoe;
			return getTeamAttrsCombat (_card, def, beastDef, coeDefPer, coeDef);

		case COE_MAG:
			double mag = attr.getWholeMAG ();
			double beastMag = _beastCard == null ? 0 : attrBeast.getWholeMAG ();
			double coeMagPer = attrPer.perMagic;
			double coeMag = getCombatSampleBySid (_card.getJob ()).magCoe;
			return getTeamAttrsCombat (_card, mag, beastMag, coeMagPer, coeMag);

		case COE_AGI:
			double agi = attr.getWholeAGI ();
			double beastAgi = _beastCard == null ? 0 : attrBeast.getWholeAGI ();
			double coeAgiPer = attrPer.perAgile;
			double coeAgi = getCombatSampleBySid (_card.getJob ()).agiCoe;
			return getTeamAttrsCombat (_card, agi, beastAgi, coeAgiPer, coeAgi);

		default:
			return 0;
		}
	}

	/// <summary>
	/// 队伍卡片单属性战斗力公式
	/// </summary>
	/// <returns>The team attrs combat.</returns>
	/// <param name="attr">属性基础值</param>
	/// <param name="beastAttr">幻兽相应属性值</param>
	/// <param name="attrPer">属性加成值</param>
	/// <param name="JobCoe">职业对应属性系数</param>
	private int getTeamAttrsCombat (Card _card, double attr, double beastAttr, double attrPer, double JobCoe)
	{
		double cardsSkills = getCardBuffSkillsCombat (_card.getSkills ());
		double buffskill = getCardBuffSkillsCombat (_card.getBuffSkills ());
		double skillCombat = Math.Pow ((cardsSkills + buffskill), 0.43);
		return (int)(((int)((attr + beastAttr) * (100 + attrPer) / 100)) * JobCoe * skillCombat / 100);
	}

	#endregion

	#region 女神战斗力

	/// <summary>
	/// 获取召唤兽总战斗力
	/// </summary>
	public int getBeastEvolveCombat (Card _card)
	{
        
        return (int)(getBeastEvolveAttrCombat(_card) * 5 + getBeastEvolveSkillCombat(_card));
	}

	/// <summary>
	/// 获取召唤兽属性战斗力
	/// </summary>
	public double getBeastEvolveAttrCombat (Card _card)
	{
//		return (getCardHpCombat (_card, false) + getCardAttCombat (_card, false) + getCardDefCombat (_card, false) + getCardMagCombat (_card, false) + getCardAgiCombat (_card, false)) / 100;
		int a = 0;
		for (int i = 1; i < 6; i++) {
			a += (int)getCardAttrCombat (_card,i, false);
		}
		return a/100;
	}

	/// <summary>
	/// 获取召唤兽技能战斗力
	/// </summary>
	public double getBeastEvolveSkillCombat (Card _card)
	{
		if (_card .getSkills () != null) {
			Skill mSkill = _card .getSkills () [0];
			int skillLv = BeastEvolveManagerment.Instance.getSkillLv ();
            ShenGePower tmpPower = ShenGeManager.Instance.CalculateShenGePower();
            int value = 0;
            if (tmpPower != null) {
                for (int i = 0; i < tmpPower.AttrInfos.Count; i++) {
                    value = tmpPower.AttrInfos[i].value;
                }
            }
		    double skillCombat = getBeastSkillCoeByLevel(skillLv)*(0.2*mSkill.getSkillQuality() + 0.2) * (100 + value)/100;
			return skillCombat;
		} else
			return 0;
	}

	#endregion
	
	#region 卡片战斗力

	/// <summary>
	/// 获取卡片总战斗力
	/// </summary>
	public int getCardCombat (Card _card)
	{
		int a = 0;
        for (int i = 1; i < 6; i++) {
            a += (int)getCardAttrCombat (_card,i, true);
        }
		a += getSkillCoe(_card);
		a += getCardStarSoulCombat(_card);
        a += getCardMagicWeaponCombat(_card);
        a += getCardEquipRefineCombat(_card);
		return a;
	}
	/// <summary>
	/// 卡片上星魂战斗力计算(只计算了非属性战斗力)
	/// </summary>
	public int getCardStarSoulCombat(Card _card)
	{
		StarSoul[] stars=_card.getStarSoulByAll();
		int attr=0;
		if(stars!=null){
			for(int i=0;i<stars.Length;i++){
				attr+=stars[i].getUnpropertyAttr(stars[i].getLevel());
			}
		}
		return attr;
	}
    /// <summary>
    /// 计算卡片装备精炼的非属性战斗力
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    public int getCardEquipRefineCombat(Card _card)
    {
        int attr = 0;
        if (_card.getEquips() == null || _card.getEquips().Length < 1)
            return 0;
        string[] equips = _card.getEquips();
        for (int i = 0; i < equips.Length;i++ )
        {
            Equip equip = StorageManagerment.Instance.getEquip(equips[i]);
            if (RefineSampleManager.Instance.getRefineSampleBySid(equip.sid) == null)
            {
                continue;
            }
            RefinelvInfo rfinfo = RefineSampleManager.Instance.getRefineSampleBySid(equip.sid).refinelvAttr[equip.getrefineLevel()];
            for (int j = 0; j < rfinfo.items.Count;j++ )
            {
                AttrRefineChangeSample acs = rfinfo.items[j];
                attr += acs.getAttrRefineCombatValue();
            }
        }
        return attr;
    }
    /// <summary>
    /// 计算神器所带的已经激活的主动技能附加的战斗力
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public int getCardMagicWeaponCombat(Card card) {
        if (card.magicWeaponUID == null || card.magicWeaponUID == "") return 0;
        MagicWeapon mw = StorageManagerment.Instance.getMagicWeapon(card.magicWeaponUID);
        if (mw == null) return 0;
        List<int> activationSkill = mw.activationSkill;//技能对战斗力的影响
        if (activationSkill == null || activationSkill.Count < 1) return 0;
        int combatNum=0;
        for (int i = 0; i < activationSkill.Count; i++) { //遍历所有的激活的神器技能
            SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(activationSkill[i]);
            if (sample == null) {
                return combatNum;
            } else {
                if (sample.showType == 1 || sample.showType == 0) {
                    combatNum += (int)(CommandConfigManager.Instance.combatMagicWeapon()[mw.getMaxPhaseLv()] * Math.Pow(mw.getPhaseLv(), CommandConfigManager.Instance.combatMagicNum()));
                    return combatNum;
                }
            }
        }            
        return combatNum;
    }
	/// <summary>
	/// 卡片属性战斗力
	/// </summary>
	private double getCardAttrCombat (Card _card,int Type, bool isCard)
	{
		if (isCard) {
			double buffskill = getCardBuffSkillsCombat (_card.getBuffSkills ());
			double skill = getCardBuffSkillsCombat (_card.getSkills ());
			return (int)(getCardAttrCombat (_card, Type) * Math.Pow ((buffskill + skill), 0.43) / 100);
		} else 
			return (int)getCardAttrCombat (_card, Type);
	}

	/// <summary>
	/// 获得全部技能战斗力总和
	/// </summary>
	private double getCardBuffSkillsCombat (Skill[] skills)
	{
		if (skills == null || skills.Length < 1)
			return 0;
		double combat = 0;
		for (int i = 0; i < skills.Length; i++) {
			combat += getCardBuffSkillCombat (skills [i]);
		}
		return combat;
		
	}
	
	/// <summary>
	/// 获得单个技能战斗力
	/// </summary>
	private double getCardBuffSkillCombat (Skill _skill)
	{
		return 0.036 * _skill.getSkillQuality () + 0.0017 * _skill.getLevel () + 0.5;
	}

	/// <summary>
	/// 卡片或者召唤兽单个属性战斗力计算
	/// </summary>
	private double getCardAttrCombat (Card _card, int _coeType)
	{
		CardBaseAttribute attr = CardManagerment.Instance.getCardAllWholeAttr (_card);

		switch (_coeType) {
		case COE_HP:
			double hp = attr.getWholeHp ();
			double coehp = getCombatSampleBySid (_card.getJob ()).hpCoe;
			return (int)(hp * coehp);
		case COE_ATT:
			double att = attr.getWholeAtt ();
			double coeAtt = getCombatSampleBySid (_card.getJob ()).attCoe;
			return (int)(att * coeAtt);
		case COE_DEF:
			double def = attr.getWholeDEF ();
			double coeDef = getCombatSampleBySid (_card.getJob ()).defCoe;
			return (int)(def * coeDef);
		case COE_MAG:
			double mag = attr.getWholeMAG ();
			double coeMag = getCombatSampleBySid (_card.getJob ()).magCoe;
			return (int)(mag * coeMag);
		case COE_AGI:
			double agi = attr.getWholeAGI ();
			double coeAgi = getCombatSampleBySid (_card.getJob ()).agiCoe;
			return (int)(agi * coeAgi);
		default:
			return 0;
		}

	}

	#endregion
	
	#region 坐骑战斗力

	/// <summary>
	/// 获取坐骑总战斗力
	/// </summary>
	public int getMountsCombat (Mounts mounts)
	{
		return (int)(getMountsAttrCombat (mounts) * (100 + 2 * getMountsSkillCombat (mounts)) / 100);
	}
	
	/// <summary>
	/// 获取召唤兽属性战斗力
	/// </summary>
	private int getMountsAttrCombat (Mounts mounts)
	{
		return (int)((getMountsAttrCombat (mounts, COE_HP) + getMountsAttrCombat (mounts, COE_ATT) + getMountsAttrCombat (mounts, COE_DEF)
		        + getMountsAttrCombat (mounts, COE_MAG) + getMountsAttrCombat (mounts, COE_AGI)) / 20);
	}

	/// <summary>
	/// 坐骑属性战斗力
	/// </summary>
	private double getMountsAttrCombat (Mounts mounts, int Type)
	{
		CardBaseAttribute attr = mounts.getMountsAddEffect ();
		switch (Type) {
		case COE_HP:
			double hp = attr.getWholeHp ();
			double coehp = getCombatSampleBySid (MOUNTS_TYPE).hpCoe;
			return (int)(hp * coehp);
		case COE_ATT:
			double att = attr.getWholeAtt ();
			double coeAtt = getCombatSampleBySid (MOUNTS_TYPE).attCoe;
			return (int)(att * coeAtt);
		case COE_DEF:
			double def = attr.getWholeDEF ();
			double coeDef = getCombatSampleBySid (MOUNTS_TYPE).defCoe;
			return (int)(def * coeDef);
		case COE_MAG:
			double mag = attr.getWholeMAG ();
			double coeMag = getCombatSampleBySid (MOUNTS_TYPE).magCoe;
			return (int)(mag * coeMag);
		case COE_AGI:
			double agi = attr.getWholeAGI ();
			double coeAgi = getCombatSampleBySid (MOUNTS_TYPE).agiCoe;
			return (int)(agi * coeAgi);
		default:
			return 0;
		}
	}
	
	/// <summary>
	/// 获取召唤兽技能品质之和
	/// </summary>
	public int getMountsSkillCombat (Mounts mounts)
	{
		int skillCombat = 0; 
		if (mounts .getSkills () != null) {
			Skill[] skills = mounts.getSkills ();
			for (int i = 0; i < skills.Length; i++) {
				skillCombat += skills[0].getSkillQuality ();
			}
		}
		return skillCombat;
	}

	#endregion

	/// <summary>
	/// 技能等级修正值
	/// </summary>
	private int getSkillCoe (Card _card)
	{
		int lvs = 0;
		if (_card.getSkills () != null && _card.getSkills ().Length > 0) {
			for (int i = 0; i < _card.getSkills ().Length; i++) {
				lvs += _card.getSkills () [i].getLevel ();
			}
		}
		if (_card.getBuffSkills () != null && _card.getBuffSkills ().Length > 0) {
			for (int i = 0; i < _card.getBuffSkills ().Length; i++) {
				lvs += _card.getBuffSkills () [i].getLevel ();
			}
		}
		return 31 * lvs;
	}

	//返回圣器技能等级系数
	public int getBeastSkillCoeByLevel (int lv)
	{
		return getBeastSkillCoe () [lv];
	}

	private int[] getBeastSkillCoe ()
	{
		return getCombatSampleBySid (7).beastSkillCoe;
	}
	
	//获得系数模板对象,sid=职业,参考JobType
	public CombatSample getCombatSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as CombatSample;
	}   
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		CombatSample sample = new CombatSample ();
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr);
		samples.Add (sid, sample);
	}


	/// <summary>
	/// 战斗力动态加载方法
	/// </summary>
	/// <param name="newCombat">新战力</param>
	/// <param name="oldCombat">旧战力</param>
	public void setCombatStep (int newCombat, int oldCombat)
	{
		this.newCombat = newCombat;
		this.oldCombat = oldCombat;
		isRefreshCombat = true;
		if (newCombat >= oldCombat)
			step = (int)((float)(newCombat - oldCombat) / 20);
		else
			step = (int)((float)(oldCombat - newCombat) / 20);
		if (step < 1)
			step = 1;
	}

	/// <summary>
	/// 刷新战斗力开关
	/// </summary>
	public bool getIsRefreshCombat ()
	{
		return isRefreshCombat;
	}

	/// <summary>
	/// 返回战斗力值
	/// </summary>
	public string getRefreshCombat ()
	{ 
		string str = "";
		if (oldCombat != newCombat) {
			if (oldCombat < newCombat) {
				oldCombat += step;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			} else if (oldCombat > newCombat) {
				oldCombat -= step;
				if (oldCombat <= newCombat)
					oldCombat = newCombat;
			}
			str = "" + oldCombat;
		} else {
			str = "" + newCombat;
			oldCombat = 0;
			newCombat = 0;
			step = 0;
			isRefreshCombat = false;
		}
		return str;
	}
}
