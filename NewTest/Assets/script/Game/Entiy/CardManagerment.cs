using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 角色管理器
 * @author longlingquan
 * */
public class CardManagerment
{ 
	public const int EQUIP_NUM = 5;//角色装备数量 
	public const string PREFAB = "character";
	public List<Equip> showChatEquips = new List<Equip>();
	public CardManagerment ()
	{ 
	}
	
	public static CardManagerment Instance {
		get{ return SingleManager.Instance.getObj ("CardManagerment") as CardManagerment;}
	}

	#region 创建卡片

	public Card createCard ()
	{
		return new Card ();
	}

	/** 创建一个角色 */
	public Card createCard (string id, int sid, int index, long exp, int hplevel, int attlevel, int deflevel, int magiclevel, int agilelevel,
	                        Skill[] skills, Skill[] buffskills, Skill[] attrskills, string[] equips, int state, int evoLevel, int sumLevel, int mainSkillSlot, int attrSkillSlot, int buffSkillSlot,string magicWeapUID,int bloodLv)
	{
        return new Card(id, sid, index, exp, hplevel, attlevel, deflevel, magiclevel, agilelevel, skills, buffskills, attrskills, equips, state, evoLevel, sumLevel, mainSkillSlot, attrSkillSlot, buffSkillSlot, magicWeapUID,bloodLv);
	}
	
	/** 创建一个角色 */
	public Card createCard (ErlArray array)
	{
		int j = 0;
		string uid = array.Value [j++].getValueString ();
		int sid = StringKit.toInt (array.Value [j++].getValueString ());
		long exp = StringKit.toLong (array.Value [j++].getValueString ());
		int hplevel = StringKit.toInt (array.Value [j++].getValueString ());
		int attlevel = StringKit.toInt (array.Value [j++].getValueString ());
		int magiclevel = StringKit.toInt (array.Value [j++].getValueString ());
		int deflevel = StringKit.toInt (array.Value [j++].getValueString ());
		int agilelevel = StringKit.toInt (array.Value [j++].getValueString ());
		Skill[] attrskills = SkillManagerment.Instance.createSkills (array.Value [j++] as ErlArray, SkillStateType.ATTR);
		Skill[] buffskills = SkillManagerment.Instance.createSkills (array.Value [j++] as ErlArray, SkillStateType.BUFF);
		Skill[] skills = SkillManagerment.Instance.createSkills (array.Value [j++] as ErlArray, SkillStateType.ACTIVE);
		string[] equips = EquipManagerment.Instance.getEquipId (array.Value [j++] as ErlArray);
		int state = StringKit.toInt (array.Value [j++].getValueString ());
		int evoLevel = StringKit.toInt (array.Value [j++].getValueString ());
		int sumLevel = StringKit.toInt (array.Value [j++].getValueString ());
		int mainSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int attrSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int buffSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
        string magicWeaponUID = array.Value[j++].getValueString();
        int bloodLv = StringKit.toInt(array.Value[j++].getValueString());
        return new Card(uid, sid, 0, exp, hplevel, attlevel, deflevel, magiclevel, agilelevel, skills, buffskills, attrskills, equips, state, evoLevel, sumLevel, mainSkillSlot, attrSkillSlot, buffSkillSlot, magicWeaponUID,bloodLv);
	}
	//通过后台的聊天数据建立新卡
	public ServerCardMsg createCardByChatServer (ErlArray array)
	{
		int j = 0;
		string uid = array.Value [j++].getValueString ();
		int sid = StringKit.toInt (array.Value [j++].getValueString ());
		long exp = StringKit.toLong (array.Value [j++].getValueString ());
		int combat = StringKit.toInt (array.Value [j++].getValueString ());
		int hplevel = StringKit.toInt (array.Value [j++].getValueString ());
		int attlevel = StringKit.toInt (array.Value [j++].getValueString ());
		int magiclevel = StringKit.toInt (array.Value [j++].getValueString ());
		int deflevel = StringKit.toInt (array.Value [j++].getValueString ());
		int agilelevel = StringKit.toInt (array.Value [j++].getValueString ());
		Skill[] attrskills = SkillManagerment.Instance.createSkillsLV (array.Value [j++] as ErlArray, SkillStateType.ATTR);
		Skill[] buffskills = SkillManagerment.Instance.createSkillsLV (array.Value [j++] as ErlArray, SkillStateType.BUFF);
		Skill[] skills = SkillManagerment.Instance.createSkillsLV (array.Value [j++] as ErlArray, SkillStateType.ACTIVE);
		string[] equips = null;
		List<Equip> showEquips = null;
		;
		object[] objs = EquipManagerment.Instance.createEquipByChat (array.Value [j++] as ErlArray);
		if (objs != null) {
			equips = objs [0] as String[];
			showEquips = objs [1] as List<Equip>;
		}
		int state = StringKit.toInt (array.Value [j++].getValueString ());
		int evoLevel = StringKit.toInt (array.Value [j++].getValueString ());
		int sumLevel = StringKit.toInt (array.Value [j++].getValueString ());
		int mainSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int attrSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int buffSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int cardAwaken = StringKit.toInt (array.Value [j++].getValueString ());
        ErlArray starSoulArray=null;
        if (array.Value.Length-3 == j) {
            starSoulArray = array.Value[j++] as ErlArray;
        }
        ArrayList bores = new ArrayList();
        if (starSoulArray!=null&&starSoulArray.Value.Length > 0) {
            StarSoulBore starSoulBore;
            for (int m = 0, count = starSoulArray.Value.Length; m < count; m++) {
                starSoulBore = new StarSoulBore();
                if ((starSoulArray.Value[m] as ErlArray).Value.Length < 1) continue;
                starSoulBore.bytesOtherRead(0, starSoulArray.Value[m] as ErlArray);
                bores.Add(starSoulBore);
            }
        }
        ErlArray magicWeaponArray = array.Value[j++] as ErlArray;
        MagicWeapon mweapon = null;
        if (magicWeaponArray != null && magicWeaponArray.Value.Length>0) {
            int mwSid =StringKit.toInt(magicWeaponArray.Value[0].getValueString());
            int stringLv = StringKit.toInt(magicWeaponArray.Value[1].getValueString());
            int phLv = StringKit.toInt(magicWeaponArray.Value[2].getValueString());
            mweapon = new MagicWeapon("", mwSid, stringLv, phLv, 0);

        }
        int bloodLv = 0;
        if (array.Value.Length > j) {
            bloodLv = StringKit.toInt(array.Value[j++].getValueString());
        }
        Card ca = new Card(uid, sid, 0, exp, hplevel, attlevel, deflevel, magiclevel, agilelevel, skills, buffskills, attrskills, equips, state, evoLevel, sumLevel, mainSkillSlot, attrSkillSlot, buffSkillSlot, true,mweapon,bloodLv);
		ca.CardAwake = cardAwaken;
		ca.CardCombat = combat;
        ca.setStarSoul(bores);
		return new ServerCardMsg (ca, showEquips, 1);
	}

	public ServerCardMsg createCardByServer (ErlArray array)
	{
		int j = 0;
		string uid = array.Value [j++].getValueString ();
		int sid = StringKit.toInt (array.Value [j++].getValueString ());
		long exp = StringKit.toLong (array.Value [j++].getValueString ());
		int hplevel = StringKit.toInt (array.Value [j++].getValueString ());
		int attlevel = StringKit.toInt (array.Value [j++].getValueString ());
		int magiclevel = StringKit.toInt (array.Value [j++].getValueString ());
		int deflevel = StringKit.toInt (array.Value [j++].getValueString ());
		int agilelevel = StringKit.toInt (array.Value [j++].getValueString ());
		Skill[] attrskills = SkillManagerment.Instance.createSkillsLV (array.Value [j++] as ErlArray, SkillStateType.ATTR);
		Skill[] buffskills = SkillManagerment.Instance.createSkillsLV (array.Value [j++] as ErlArray, SkillStateType.BUFF);
		Skill[] skills = SkillManagerment.Instance.createSkillsLV (array.Value [j++] as ErlArray, SkillStateType.ACTIVE);
		string[] equips = null;
		List<Equip> showEquips = null;
		;
		object[] objs = EquipManagerment.Instance.createEquipByChat (array.Value [j++] as ErlArray);
		if (objs != null) {
			equips = objs [0] as String[];
			showEquips = objs [1] as List<Equip>;
		}
		int state = StringKit.toInt (array.Value [j++].getValueString ());
		int evoLevel = StringKit.toInt (array.Value [j++].getValueString ());
		int sumLevel = StringKit.toInt (array.Value [j++].getValueString ());
		int mainSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int attrSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int buffSkillSlot = StringKit.toInt (array.Value [j++].getValueString ());
		int cardAwaken = StringKit.toInt (array.Value [j++].getValueString ());
        ErlArray starSoulArray = array.Value[j++] as ErlArray;
        ArrayList bores = new ArrayList();
        if (starSoulArray.Value.Length > 0) {
            StarSoulBore starSoulBore;
            for (int m = 0, count = starSoulArray.Value.Length; m < count; m++) {
                starSoulBore = new StarSoulBore();
                if ((starSoulArray.Value[m] as ErlArray).Value.Length<1) continue;
                starSoulBore.bytesOtherRead(0, starSoulArray.Value[m] as ErlArray);
                bores.Add(starSoulBore);
            }
        }
        string magicWeaponUID = array.Value[j++].getValueString();
	    int bloodLv = 0;
	    if (array.Value.Length > j)
	    {
            bloodLv = StringKit.toInt(array.Value[j++].getValueString());
	    }
        Card ca = new Card(uid, sid, 0, exp, hplevel, attlevel, deflevel, magiclevel, agilelevel, skills, buffskills, attrskills, equips, state, evoLevel, sumLevel, mainSkillSlot, attrSkillSlot, buffSkillSlot, magicWeaponUID,bloodLv);
		ca.CardAwake = cardAwaken;
        ca.setStarSoul(bores);

		return new ServerCardMsg (ca, showEquips, 1);
	}

	//创建一个角色  不包含属性附加等级 技能 装备
	public Card createCard (string id, int sid, int index, long exp)
	{ 
		return new Card (id, sid, index, exp, 0, 0, 0, 0, 0, null, null, null, null, 0, 0, 0, 0, 0, 0,"",0);
	}  

	public Card createCard(int sid,int surLevel)
	{
		return new Card ("", sid, 0, 0, 0, 0, 0, 0, 0, null, null, null, null, 0, 0, surLevel, 0, 0, 0,"",0);
	}
	
	//创建一个角色 只取模板数据（包含创建默认技能 默认附加等级)
	public Card createCard (int sid)
	{
		Card card = new Card ("", sid, 0, 0, 0, 0, 0, 0, 0, null, null, null, null, 0, 0, 0, 0, 0, 0,"",0);
		card.copySkillsBySample ();
		card.copyAppendAttrBySample ();
		return card;
	}
	
	//创建一个角色 只取模板数据（包含创建默认技能 默认附加等级)
	public Card createCard (int sid, int evoLv, int surLv)
	{
		Card card = new Card ("", sid, 0, 0, 0, 0, 0, 0, 0, null, null, null, null, 0, evoLv, surLv, 0, 0, 0,"",0);
		card.copySkillsBySample ();
		card.copyAppendAttrBySample ();
		return card;
	}
	
	/** 创建一个任意进化等级的相同角色（加成效果，以及装备相同） */
	public Card createCardByEvoLevel (Card _Card, int _lv)
	{
		if (_lv <= 0)
			_lv = 0;
		Card card = new Card (_Card.uid, _Card.sid, _Card.index, _Card.getEXP (), _Card.getHPExp (), _Card.getATTExp (), _Card.getDEFExp (), _Card.getMAGICExp (),
                              _Card.getAGILEExp (), _Card.getSkills (), _Card.getBuffSkills (), _Card.getAttrSkills (), _Card.getEquips (), _Card.state, _lv, _Card.getSurLevel (),
                              _Card.getMainSkillSlot(), _Card.getAttrSkillSlot(), _Card.getBuffSkillSlot(), _Card.magicWeaponUID,_Card.cardBloodLevel);
		card.setStarSoul (_Card.getStarSoulBores ());
		return card;
	}
	
	/** 创建一个任意突破等级的相同角色（加成效果，以及装备相同） */
	public Card createCardBySurLevel (Card _Card, int _lv)
	{
		if (_lv <= 0)
			_lv = 0;
		Card card = new Card (_Card.uid, _Card.sid, _Card.index, _Card.getEXP (), _Card.getHPExp (), _Card.getATTExp (), _Card.getDEFExp (), _Card.getMAGICExp (),
		                      _Card.getAGILEExp (), _Card.getSkills (), _Card.getBuffSkills (), _Card.getAttrSkills (), _Card.getEquips (), _Card.state, _Card.getEvoTimes (), _lv,
		                      _Card.getMainSkillSlot (), _Card.getAttrSkillSlot (), _Card.getBuffSkillSlot (),_Card.magicWeaponUID,_Card.cardBloodLevel);
		card.setStarSoul (_Card.getStarSoulBores ());
		return card;
	}

	#endregion

	#region 属性统计
	
	/** 获得队伍卡片全部基础属性 */
	public CardBaseAttribute getCardAllWholeAttrByTeam (Card card)
	{
		//所有属性
		CardBaseAttribute whole = new CardBaseAttribute (); 
		//卡片本身属性
		CardBaseAttribute attrCard = getCardAttribute (card);
		whole.mergeCardBaseAttr (attrCard);
		//卡片被动技能属性影响值
		CardBaseAttribute attrSkill = getCardSkillEffectNum (card);
		whole.mergeCardBaseAttr (attrSkill);
		//装备属性
		CardBaseAttribute attrEquips = getCardEquipsEffect (card);
		whole.mergeCardBaseAttr (attrEquips);
        //装备精炼
        CardBaseAttribute attrEquipsRefine = getRefineCombat(card);
        whole.mergeCardBaseAttr(attrEquipsRefine);
        //装备共鸣的所有属性
        CardBaseAttribute attrResonanceshuxing = getResonanceEffect(card);
        whole.mergeCardBaseAttr(attrResonanceshuxing);
        //神器属性
        CardBaseAttribute attrMagicWeapons = getCardMagicWeaponEffect(card);
        whole.mergeCardBaseAttr(attrMagicWeapons);
        //血脉的基础属性
        CardBaseAttribute attrBloodAttribute = getCardBloodAttributePerAdd(card);
        whole.mergeCardBaseAttr(attrBloodAttribute);
		//星魂属性
		CardBaseAttribute arrtStarSouls = getCardStarSoulEffect (card);
		whole.mergeCardBaseAttr (arrtStarSouls);
		//套装基础属性
		CardBaseAttribute attrSuit = getCardSuitEffect (card);
		whole.mergeCardBaseAttr (attrSuit);
		//附加等级属性
		CardBaseAttribute attrBase = getCardAppendEffectNoPer (card);
		whole.mergeCardBaseAttr (attrBase);
		//进化属性加成
		CardBaseAttribute attrEvo = getEvolutionAttr (card);
        whole.mergeCardBaseAttr(attrEvo);
        //神格基础属性
        CardBaseAttribute shengeBaseAttribute = getShenGeBaseAttribute(card);
        whole.mergeCardBaseAttr(shengeBaseAttribute);
		//如果是主角就有突破属性比例加成
		if (card.uid == UserManager.Instance.self.mainCardUid) {
			CardBaseAttribute attrSur = getSurmountAttr (card);
			whole.mergeCardBaseAttr (attrSur);
		}
		if (card.getCardType () == 1) {
			if (isMyCard (card)) {
				//公会技能
				if (GuildManagerment.Instance.getGuildSkill () != null) {
					CardBaseAttribute guildSur = GuildManagerment.Instance.getSkillEffect ();
					whole.mergeCardBaseAttr (guildSur);
				}
				//星盘属性加成
				CardBaseAttribute attrStar = GoddessAstrolabeManagerment.Instance.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrStar);
				//天梯称号属性加成
				CardBaseAttribute attrLadder_Title = LaddersManagement.Instance.TitleAttrEffect.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrLadder_Title);
				//天梯奖章属性加成
				CardBaseAttribute attrLadder_Medal = LaddersManagement.Instance.MedalAttrEffect.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrLadder_Medal);
				//队伍中的人才加坐骑属性
				if (card.isInTeam ()) {
					// 坐骑属性加成
					CardBaseAttribute mountsAttribute = MountsManagerment.Instance.getUseMountsAttribute();
					whole.mergeCardBaseAttr(mountsAttribute);
					// 坐骑技能基础属性加成
					CardBaseAttribute mountsSkillAttribute = MountsManagerment.Instance.getUseMountsSkillEffectNum();
					whole.mergeCardBaseAttr (mountsSkillAttribute);
				}

			}
		}
		return whole;
	}

	/** 获得队伍卡片全部百分比加成 */
	public CardBaseAttribute getCardAllWholeAttrPerByTeam (Card card)
	{
		//所有属性
		CardBaseAttribute whole = new CardBaseAttribute ();
		//套装比例加成
		CardBaseAttribute attrSuitByPer = getCardSuitEffectByPer (card);
		whole.mergeCardBaseAttr (attrSuitByPer);
		//卡片被动技能属性影响值
		CardBaseAttribute attrSkill = getCardSkillEffectPer (card);
		whole.mergeCardBaseAttr (attrSkill);
		//星魂百分比
		CardBaseAttribute attrStarSoul = getCardStarSoulEffectPer (card);
		whole.mergeCardBaseAttr (attrStarSoul);
        //装备精炼百分比
        CardBaseAttribute attrEquipsRefine = getRefineCombatPer(card);
        whole.mergeCardBaseAttr(attrEquipsRefine);
        //装备共鸣的所有属性
        CardBaseAttribute attrResonanceshuxing = getResonanceEffectPer(card);
        whole.mergeCardBaseAttr(attrResonanceshuxing);
        //神器被动技能百分比加成
        CardBaseAttribute attrMagicWeap = getCardMagicWeaponAdd(card);
        whole.mergeCardBaseAttr(attrMagicWeap);
        //血脉基础百分比
        CardBaseAttribute attrBloodPerAttr = getCardBloodPerAttribute(card);
        whole.mergeCardBaseAttr(attrBloodPerAttr);
        //血脉附加技能的加成百分比
        CardBaseAttribute attrBloodSkillPreAttr = getCardBloodSkillAdd(card);
        whole.mergeCardBaseAttr(attrBloodSkillPreAttr);
		if (card.getCardType () == 1) {
			if (isMyCard (card)) {
				//共鸣等级
				CardBaseAttribute attrBeast = getCardBeastEffectByPer ();
				whole.mergeCardBaseAttr (attrBeast);
				//星盘属性比例加成
				CardBaseAttribute attrStarPer = GoddessAstrolabeManagerment.Instance.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrStarPer);
				//天梯称号比例加成
				CardBaseAttribute attrLadder_Title = LaddersManagement.Instance.TitleAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_Title);
				//天梯奖章比例加成
				CardBaseAttribute attrLadder_Medal = LaddersManagement.Instance.MedalAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_Medal);
				//队伍中的人才加坐骑属性
				if (card.isInTeam ()) {
					// 坐骑技能属性百分比加成
					CardBaseAttribute mountsSkillAttribute = MountsManagerment.Instance.getUseMountsSkillEffectPer();
					whole.mergeCardBaseAttr (mountsSkillAttribute);
				}
			}
		}
		//进化属性加成
		CardBaseAttribute attrEvo = getEvolutionAttrPer (card);
		whole.mergeCardBaseAttr (attrEvo);
		//如果是主角就有突破属性比例加成
		if (card.uid == UserManager.Instance.self.mainCardUid) {
			CardBaseAttribute attrSur = getSurmountAttrPer (card);
			whole.mergeCardBaseAttr (attrSur);
		}
		return whole;
	}

	/** 获得卡片全部属性 */
	public CardBaseAttribute getCardAllWholeAttr (Card card)
	{
		//所有属性
		CardBaseAttribute whole = new CardBaseAttribute (); 
		//卡片本身属性
		CardBaseAttribute attrCard = getCardAttribute (card);
		whole.mergeCardBaseAttr (attrCard);
		//卡片被动技能属性影响值
		CardBaseAttribute attrSkill = getCardSkillEffect (card);
		whole.mergeCardBaseAttr (attrSkill);
		//装备属性
		CardBaseAttribute attrEquips = getCardEquipsEffect (card);
		whole.mergeCardBaseAttr (attrEquips);
        //装备精炼的所有属性
        CardBaseAttribute attrEquipsRefine = getRefineCombat(card);
        whole.mergeCardBaseAttr(attrEquipsRefine);
        //装备共鸣的所有属性
        CardBaseAttribute attrResonanceshuxing = getResonanceEffect(card);
        whole.mergeCardBaseAttr(attrResonanceshuxing);
        //装备精炼百分比
        CardBaseAttribute attrEquipsRefinePer = getRefineCombatPer(card);
        whole.mergeCardBaseAttr(attrEquipsRefinePer);
        //装备共鸣的所有属性
        CardBaseAttribute attrResonanceshuxingPer = getResonanceEffectPer(card);
        whole.mergeCardBaseAttr(attrResonanceshuxingPer);
        //装备神器的所有属性
        CardBaseAttribute attrMagicWeapons = getCardMagicWeaponAllAttr(card);
        whole.mergeCardBaseAttr(attrMagicWeapons);
        //血脉的所有属性
	    CardBaseAttribute attrBloodAttribute = getCardBloodAttribute(card);
        whole.mergeCardBaseAttr(attrBloodAttribute);
        //血脉附加技能的加成百分比
        CardBaseAttribute attrBloodSkillPreAttr = getCardBloodSkillAdd(card);
        whole.mergeCardBaseAttr(attrBloodSkillPreAttr);
		//星魂所有属性
		CardBaseAttribute attrStarSoul = getCardStarSoulEffectAll (card);
		whole.mergeCardBaseAttr (attrStarSoul);
		//套装基础属性
		CardBaseAttribute attrSuit = getCardSuitEffect (card);
		whole.mergeCardBaseAttr (attrSuit);
		//套装比例加成
		CardBaseAttribute attrSuitByPer = getCardSuitEffectByPer (card);
		whole.mergeCardBaseAttr (attrSuitByPer);
		//附加等级属性
		CardBaseAttribute attrBase = getCardAppendEffectNoPer (card);
		whole.mergeCardBaseAttr (attrBase);
		//进化属性加成
		CardBaseAttribute attrEvo = getEvolutionAttr (card);
		whole.mergeCardBaseAttr (attrEvo);
		//进化属性比例加成
		CardBaseAttribute attrEvoPer = getEvolutionAttrPer (card);
        whole.mergeCardBaseAttr(attrEvoPer);
		//如果是主角就有突破属性加成
		if (card.uid == UserManager.Instance.self.mainCardUid) {
			CardBaseAttribute attrSur = getSurmountAttr (card);
			whole.mergeCardBaseAttr (attrSur);
		}
		//如果是主角就有突破属性比例加成
		if (card.uid == UserManager.Instance.self.mainCardUid) {
			CardBaseAttribute attrSur = getSurmountAttrPer (card);
			whole.mergeCardBaseAttr (attrSur);
		}
	    if (card.getCardType () == 1) {
			if (isMyCard (card)) {
				//共鸣等级
				CardBaseAttribute attrBeast = getCardBeastEffectByPer ();
				whole.mergeCardBaseAttr (attrBeast);
				//公会技能
				if (GuildManagerment.Instance.getGuildSkill () != null) {
					CardBaseAttribute guildSur = GuildManagerment.Instance.getSkillEffect ();
					whole.mergeCardBaseAttr (guildSur);
				}
				//星盘属性加成
				CardBaseAttribute attrStar = GoddessAstrolabeManagerment.Instance.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrStar);
				//星盘属性比例加成
				CardBaseAttribute attrStarPer = GoddessAstrolabeManagerment.Instance.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrStarPer);
				//天梯称号属性加成
				CardBaseAttribute attrLadder_Title = LaddersManagement.Instance.TitleAttrEffect.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrLadder_Title);
				//天梯称号比例加成
				CardBaseAttribute attrLadder_TitlePer = LaddersManagement.Instance.TitleAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_TitlePer);
				//天梯奖章属性加成
				CardBaseAttribute attrLadder_Medal = LaddersManagement.Instance.MedalAttrEffect.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrLadder_Medal);
				//天梯奖章比例加成
				CardBaseAttribute attrLadder_MedalPer = LaddersManagement.Instance.MedalAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_MedalPer);
				//队伍中的人才加坐骑属性
				if (card.isInTeam ()) {
					// 坐骑属性加成
					CardBaseAttribute mountsAttribute = MountsManagerment.Instance.getUseMountsAttribute();
					whole.mergeCardBaseAttr(mountsAttribute);
					// 坐骑技能属性加成
					CardBaseAttribute mountsSkillAttribute = MountsManagerment.Instance.getUseMountsSkillEffect();
					whole.mergeCardBaseAttr (mountsSkillAttribute);
				}
			}
        }
		return whole;
	}

	/** 获得卡片全部属性(不包含卡片附加等级属性效果) */
	public CardBaseAttribute getCardWholeAttr (Card card)
	{
		//所有属性
		CardBaseAttribute whole = new CardBaseAttribute (); 
		//卡片本身属性
		CardBaseAttribute attrCard = getCardAttribute (card);
		whole.mergeCardBaseAttr (attrCard);
		//卡片被动技能属性影响值
		CardBaseAttribute attrSkill = getCardSkillEffect (card);
		whole.mergeCardBaseAttr (attrSkill);
		//装备属性
		CardBaseAttribute attrEquips = getCardEquipsEffect (card);
		whole.mergeCardBaseAttr (attrEquips);
        //装备精炼的所有属性
        CardBaseAttribute attrEquipsRefine = getRefineCombat(card);
        whole.mergeCardBaseAttr(attrEquipsRefine);
        //装备共鸣的所有属性
        CardBaseAttribute attrResonanceshuxing = getResonanceEffect(card);
        whole.mergeCardBaseAttr(attrResonanceshuxing);
        //装备精炼百分比
        CardBaseAttribute attrEquipsRefinePer = getRefineCombatPer(card);
        whole.mergeCardBaseAttr(attrEquipsRefinePer);
        //装备共鸣的所有属性
        CardBaseAttribute attrResonanceshuxingPer = getResonanceEffectPer(card);
        whole.mergeCardBaseAttr(attrResonanceshuxingPer);
        //装备神器的所有属性
        CardBaseAttribute attrMagicWeapons = getCardMagicWeaponAllAttr(card);
        whole.mergeCardBaseAttr(attrMagicWeapons);
        //血脉的所有属性
        CardBaseAttribute attrBloodAttribute = getCardBloodAttribute(card);
        whole.mergeCardBaseAttr(attrBloodAttribute);
        //血脉附加技能的加成百分比
        CardBaseAttribute attrBloodSkillPreAttr = getCardBloodSkillAdd(card);
        whole.mergeCardBaseAttr(attrBloodSkillPreAttr);
		//星魂所有属性
		CardBaseAttribute attrStarSoul = getCardStarSoulEffectAll (card);
		whole.mergeCardBaseAttr (attrStarSoul);
		//套装基础属性
		CardBaseAttribute attrSuit = getCardSuitEffect (card);
		whole.mergeCardBaseAttr (attrSuit);
		//套装比例加成
		CardBaseAttribute attrSuitByPer = getCardSuitEffectByPer (card);
		whole.mergeCardBaseAttr (attrSuitByPer);
		//进化属性加成
		CardBaseAttribute attrEvo = getEvolutionAttr (card);
		whole.mergeCardBaseAttr (attrEvo);
		//进化属性比例加成
		CardBaseAttribute attrEvoPer = getEvolutionAttrPer (card);
		whole.mergeCardBaseAttr (attrEvoPer);
		//如果是主角就有突破属性加成
		if (card.uid == UserManager.Instance.self.mainCardUid) {
			CardBaseAttribute attrSur = getSurmountAttr (card);
			whole.mergeCardBaseAttr (attrSur);
		}
		//如果是主角就有突破属性比例加成
		if (card.uid == UserManager.Instance.self.mainCardUid) {
			CardBaseAttribute attrSur = getSurmountAttrPer (card);
			whole.mergeCardBaseAttr (attrSur);
		}
		if (card.getCardType () == 1) {
			if (isMyCard (card)) {
				//共鸣等级
				CardBaseAttribute attrBeast = getCardBeastEffectByPer ();
				whole.mergeCardBaseAttr (attrBeast);
				//公会技能
				if (GuildManagerment.Instance.getGuildSkill () != null) {
					CardBaseAttribute guildSur = GuildManagerment.Instance.getSkillEffect ();
					whole.mergeCardBaseAttr (guildSur);
				}
				//星盘属性加成
				CardBaseAttribute attrStar = GoddessAstrolabeManagerment.Instance.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrStar);
				//星盘属性比例加成
				CardBaseAttribute attrStarPer = GoddessAstrolabeManagerment.Instance.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrStarPer);
				//天梯称号属性加成
				CardBaseAttribute attrLadder_Title = LaddersManagement.Instance.TitleAttrEffect.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrLadder_Title);	
				//天梯称号比例加成
				CardBaseAttribute attrLadder_TitlePer = LaddersManagement.Instance.TitleAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_TitlePer);	
				//天梯奖章属性加成
				CardBaseAttribute attrLadder_Medal = LaddersManagement.Instance.MedalAttrEffect.getAttrByAllInteger ();
				whole.mergeCardBaseAttr (attrLadder_Medal);
				//天梯奖章比例加成
				CardBaseAttribute attrLadder_MedalPer = LaddersManagement.Instance.MedalAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_MedalPer);
				//队伍中的人才加坐骑属性
				if (card.isInTeam ()) {
					// 坐骑属性加成
					CardBaseAttribute mountsAttribute = MountsManagerment.Instance.getUseMountsAttribute();
					whole.mergeCardBaseAttr(mountsAttribute);
					// 坐骑技能属性加成
					CardBaseAttribute mountsSkillAttribute = MountsManagerment.Instance.getUseMountsSkillEffect();
					whole.mergeCardBaseAttr (mountsSkillAttribute);
				}
			}
		}
		return whole;
	}
	/**获得血脉被动技能所附加的属性百分比*/
    public CardBaseAttribute getCardBloodSkillAdd(Card card) {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (!CardSampleManager.Instance.checkBlood(card.sid, card.uid)) return attr;
        List<Skill> bloodSkills = BloodConfigManager.Instance.isActiveSkillSid(card.sid, card.cardBloodLevel);
        if (bloodSkills == null || bloodSkills.Count < 1) return attr;
        for (int i = 0; i < bloodSkills.Count; i++) {
            SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(bloodSkills[i].sid);
            if (sample == null) return attr;
            if (sample.showType == 3 || sample.showType == 2) {//被动技能
                AttrChangeSample[] attrEffects = sample.effects;
                if (attrEffects == null) return attr;
                for (int j = 0; j < attrEffects.Length; j++) {
                    if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
                        attr.perHp += attrEffects[j].getAttrValue(0);
                    } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
                        attr.perAttack += attrEffects[j].getAttrValue(0);
                    } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
                        attr.perMagic += attrEffects[j].getAttrValue(0);
                    } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
                        attr.perAgile += attrEffects[j].getAttrValue(0);
                    } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
                        attr.perDefecse += attrEffects[j].getAttrValue(0);
                    }
                }
            }
        }
        return attr;
    }
    /** 获取神格附加的基础属性 */
    public CardBaseAttribute getShenGeBaseAttribute(Card card)
    {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (card.isBeast())
        {
            return attr;
        } // || !ArmyManager.Instance.getFightBeasts().Contains(card.uid)
        List<ShenGeCaoInfo> infos = ShenGeManager.Instance.getAllEquipedShenGeSid();
        for (int k = 0; k < infos.Count; k++)//计算各类型神格所附加的影响值总和
        {
            Prop tmpProp = PropManagerment.Instance.createProp(infos[k].sid);
            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(infos[k].sid);
            if (sample != null) {
                switch (tmpProp.getType()) {
                    case PropType.PROP_SHENGE_HP:
                        attr.hp += tmpProp.getEffectValue();
                        break;
                    case PropType.PROP_SHENGE_DEF:
                        attr.defecse += tmpProp.getEffectValue();
                        break;
                    case PropType.PROP_SHENGE_AGI:
                        attr.agile += tmpProp.getEffectValue();
                        break;
                    case PropType.PROP_SHENGE_ATT:
                        attr.attack += tmpProp.getEffectValue();
                        break;
                    case PropType.PROP_SHENGE_MAG:
                        attr.magic += tmpProp.getEffectValue();
                        break;
                }
            }
        }
        return attr;
    }

    /** 获得卡片装备属性值影响(不包含套装效果) */
	public CardBaseAttribute getCardEquipsEffect (Card card)
	{ 
		CardBaseAttribute attr = new CardBaseAttribute ();
		if (card.getEquips () == null || card.getEquips ().Length < 1)
			return attr;
		for (int i = 0; i < card.getEquips().Length; i++) {
			string id = card.getEquips () [i];
			attr.attack += getEquipById (id).getAttack ();
			attr.hp += getEquipById (id).getHP ();  
			attr.defecse += getEquipById (id).getDefecse ();
			attr.magic += getEquipById (id).getMagic ();
			attr.agile += getEquipById (id).getAgile ();
		}
		return attr;
	}
    /// <summary>
    /// 获得卡片装备的精炼属性
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    public CardBaseAttribute getRefineCombat(Card _card)
    {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (_card.getEquips() == null || _card.getEquips().Length < 1)
            return attr;
        string[] equips = _card.getEquips();
        for (int i = 0; i < equips.Length; i++)
        {
            Equip equip = StorageManagerment.Instance.getEquip(equips[i]);
            if (RefineSampleManager.Instance.getRefineSampleBySid(equip.sid) == null)
            {
                continue;
            }
            int equipRefineLevel = equip.getrefineLevel();
            int[] a = new int[3];
            string[] b = new string[3];
            RefinelvInfo newrfinfo = RefineSampleManager.Instance.getRefineSampleBySid(equip.sid).refinelvAttr[equipRefineLevel];
            for (int l = 0; l < newrfinfo.items.Count; l++) {
                AttrRefineChangeSample acs = newrfinfo.items[l];
                for (int k = 0; k < 3; k++)
                {
                    if (b[k] == null) {
                        b[k] = acs.getAttrType();
                        a[k] += acs.getAttrRefineValue(0);
                        break;
                    }
                    if (b[k] != acs.getAttrType()) continue;
                    a[k] += acs.getAttrRefineValue(0);
                    break;
                }
            } 
            for (int m = 0; m < 3; m++) {
                if (b[m] != null) {
                    switch (b[m]) {
                        case AttrChangeType.HP:
                            attr.hp += a[m];
                            break;
                        case AttrChangeType.ATTACK:
                            attr.attack += a[m];
                            break;
                        case AttrChangeType.DEFENSE:
                            attr.defecse += a[m];
                            break;
                        case AttrChangeType.AGILE:
                            attr.agile += a[m];
                            break;
                        case AttrChangeType.MAGIC:
                            attr.magic += a[m];
                            break;
                        //case AttrChangeType.PER_AGILE:
                        //    attr.perAgile += a[m];
                        //    break;
                        //case AttrChangeType.PER_ATTACK:
                        //    attr.perAttack += a[m];
                        //    break;
                        //case AttrChangeType.PER_DEFENSE:
                        //    attr.perDefecse += a[m];
                        //    break;
                        //case AttrChangeType.PER_HP:
                        //    attr.perHp += a[m];
                        //    break;
                        //case AttrChangeType.PER_MAGIC:
                        //    attr.perMagic += a[m];
                        //    break;
                    }
                }
            }
           }
            return attr;
        }
    
    /// <summary>
    /// 获得卡片装备共鸣的属性
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    public CardBaseAttribute getResonanceEffect(Card _card)
    {
        CardBaseAttribute attr = new CardBaseAttribute();
        ResonanceSampleManager.Instance.showNum(false);
        for (int i = 1; i < 4;i++)
        {
            List<ResonanceSample> rsss = ResonanceSampleManager.Instance.getrssList(i, _card, 0, 0);
            for (int k = 0; k < rsss.Count;k++)
            {
                ResonanceSample newRsS = rsss[k];
                ResonanceInfo newRiI = newRsS.resonanceAttr;
                for (int j = 0; j < newRiI.items.Count;j++)
                {
                    AttrChangeSample acs = newRiI.items[j];
                    switch(acs.getAttrType())
                     {
                         case AttrChangeType.HP:
                             attr.hp += acs.getAttrValue(0);
                             break;
                         case AttrChangeType.ATTACK:
                             attr.attack += acs.getAttrValue(0);
                             break;
                         case AttrChangeType.DEFENSE:
                             attr.defecse += acs.getAttrValue(0);
                             break;
                         case AttrChangeType.AGILE:
                             attr.agile += acs.getAttrValue(0);
                             break;
                         case AttrChangeType.MAGIC:
                             attr.magic += acs.getAttrValue(0);
                             break;
                         //case AttrChangeType.PER_AGILE:
                         //    attr.perAgile += acs.getAttrValue(0);
                         //    break;
                         //case AttrChangeType.PER_ATTACK:
                         //    attr.perAttack += acs.getAttrValue(0);
                         //    break;
                         //case AttrChangeType.PER_DEFENSE:
                         //    attr.perDefecse += acs.getAttrValue(0);
                         //    break;
                         //case AttrChangeType.PER_HP:
                         //    attr.perHp += acs.getAttrValue(0);
                         //    break;
                         //case AttrChangeType.PER_MAGIC:
                         //    attr.perMagic += acs.getAttrValue(0);
                         //    break;
                     }
                }
            }            
        }

        return attr;
    }
    //神器百分比加成
    public CardBaseAttribute getCardMagicWeaponAdd(Card card) {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (card.magicWeaponUID == null || card.magicWeaponUID == "") return attr;
        MagicWeapon mw = StorageManagerment.Instance.getMagicWeapon(card.magicWeaponUID);
        if (mw == null) return attr;
        //神器激活的被动技能所附加的属性加成
        List<int> activationSkill = mw.activationSkill;//技能对战斗力的影响
        if (activationSkill == null || activationSkill.Count < 1) return attr;
		if(mw.getPhaseLv() <= mw.getLvType()-1)
		{
			for (int i = 1; i <= mw.getPhaseLv(); i++) {
				SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(activationSkill[i - 1]);
				if (sample == null) {
					//return attr;
				} else {
					if (sample.showType != 1) {//被动技能
						AttrChangeSample[] attrEffects = sample.effects;
						if(attrEffects != null)
						{
							for (int j = 0; j < attrEffects.Length; j++) {
								if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
									attr.perHp += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
									attr.perAttack += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
									attr.perMagic += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
									attr.perAgile += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
									attr.perDefecse += attrEffects[j].getAttrValue(0);
								}
							}
						}
					}
				}
			}
		}
		else 
		{
			for (int i = mw.getPhaseLv()-(mw.getLvType()-1)+1; i <= mw.getPhaseLv(); i++) {
				SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(activationSkill[i - 1]);
				if (sample == null) {
					//return attr;
				} else {
					if (sample.showType != 1) {//被动技能
						AttrChangeSample[] attrEffects = sample.effects;
						if(attrEffects != null)
						{
							for (int j = 0; j < attrEffects.Length; j++) {
								if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
									attr.perHp += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
									attr.perAttack += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
									attr.perMagic += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
									attr.perAgile += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
									attr.perDefecse += attrEffects[j].getAttrValue(0);
								}
							}
						}
					}
				}
			}
		}
//        for (int i = 1; i <= mw.getPhaseLv(); i++) {
//            SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(activationSkill[i - 1]);
//            if (sample == null) {
//                //return attr;
//            } else {
//                if (sample.showType != 1) {//被动技能
//                    AttrChangeSample[] attrEffects = sample.effects;
//					if(attrEffects != null)
//					{
//						for (int j = 0; j < attrEffects.Length; j++) {
//							if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
//								attr.perHp += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
//								attr.perAttack += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
//								attr.perMagic += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
//								attr.perAgile += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
//								attr.perDefecse += attrEffects[j].getAttrValue(0);
//							}
//						}
//					}
////                    if (attrEffects == null) return attr;
////                    for (int j = 0; j < attrEffects.Length; j++) {
////                        if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
////                            attr.perHp += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
////                            attr.perAttack += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
////                            attr.perMagic += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
////                            attr.perAgile += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
////                            attr.perDefecse += attrEffects[j].getAttrValue(0);
////                        }
////                    }
//                }
//            }
//        }
        return attr;
    }
    //神器属性（不包含被动技能附加的属性）
    public CardBaseAttribute getCardMagicWeaponEffect(Card card) {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (card.magicWeaponUID == null || card.magicWeaponUID == "")return attr;
        MagicWeapon mw = StorageManagerment.Instance.getMagicWeapon(card.magicWeaponUID);
        if (mw == null) return attr;
        attr.attack+=mw.magicWeaponAttrbutes.getMagicWeaponAttack();
        attr.hp += mw.magicWeaponAttrbutes.getMagicWeaponHp();
        attr.defecse += mw.magicWeaponAttrbutes.getMagicWeaponDefecse();
        attr.magic += mw.magicWeaponAttrbutes.getMagicWeaponMagic(); 
        attr.agile += mw.magicWeaponAttrbutes.getMagicWeaponAgile();
        return attr;
    }
    /// <summary>
    /// 获得卡片激活的血脉所附加的属性
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public CardBaseAttribute getCardBloodAttribute(Card card)
    {
         CardBaseAttribute attr=new CardBaseAttribute();
         if(card.cardBloodLevel==0)return attr;
         BloodPointSample bps =
         BloodConfigManager.Instance.getBloodPointSampleBySid(CardSampleManager.Instance.getBloodSid(card.sid));
         if (bps == null) return attr;
        int[] itemSids = BloodConfigManager.Instance.getReadyItemSids(bps, card.cardBloodLevel);
        for (int i=0;i<itemSids.Length;i++)
        {
            BloodItemSample bis = BloodItemConfigManager.Instance.getBolldItemSampleBySid(itemSids[i]);
            if (bis == null) continue;
            bloodEffect[] blEffects = bis.effects;
            for (int j = 0; j < blEffects.Length; j++) {
                attr.hp += blEffects[j].hp;
                attr.attack += blEffects[j].attack;
                attr.defecse += blEffects[j].defec;
                attr.magic += blEffects[j].magic;
                attr.agile += blEffects[j].agile;
                attr.perHp += blEffects[j].perhp;
                attr.perAttack += blEffects[j].perattack;
                attr.perDefecse += blEffects[j].perdefec;
                attr.perMagic += blEffects[j].permagic;
                attr.perAgile += blEffects[j].peragile;
            }
        }
        return attr;

    }
    /// <summary>
    /// 获得卡片激活的血脉所附加的属性(不包含基础百分比加成)
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public CardBaseAttribute getCardBloodAttributePerAdd(Card card) {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (card.cardBloodLevel == 0) return attr;
        BloodPointSample bps =
        BloodConfigManager.Instance.getBloodPointSampleBySid(CardSampleManager.Instance.getBloodSid(card.sid));
        if (bps == null) return attr;
        int[] itemSids = BloodConfigManager.Instance.getReadyItemSids(bps, card.cardBloodLevel);
        for (int i = 0; i < itemSids.Length; i++) {
            BloodItemSample bis = BloodItemConfigManager.Instance.getBolldItemSampleBySid(itemSids[i]);
            if (bis == null) continue;
            bloodEffect[] blEffects = bis.effects;
            for (int j = 0; j < blEffects.Length; j++) {
                attr.hp += blEffects[j].hp;
                attr.attack += blEffects[j].attack;
                attr.defecse += blEffects[j].defec;
                attr.magic += blEffects[j].magic;
                attr.agile += blEffects[j].agile;
            }
        }
        return attr;

    }
    /**血脉基础属性百分比*/
    public CardBaseAttribute getCardBloodPerAttribute(Card card) {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (card.cardBloodLevel == 0) return attr;
        BloodPointSample bps =
        BloodConfigManager.Instance.getBloodPointSampleBySid(CardSampleManager.Instance.getBloodSid(card.sid));
        if (bps == null) return attr;
        int[] itemSids = BloodConfigManager.Instance.getReadyItemSids(bps, card.cardBloodLevel);
        for (int i = 0; i < itemSids.Length; i++) {
            BloodItemSample bis = BloodItemConfigManager.Instance.getBolldItemSampleBySid(itemSids[i]);
            if (bis == null) continue;
            bloodEffect[] blEffects = bis.effects;
            for (int j = 0; j < blEffects.Length; j++) {
                attr.perHp += blEffects[j].perhp;
                attr.perAttack += blEffects[j].perattack;
                attr.perDefecse += blEffects[j].perdefec;
                attr.perMagic += blEffects[j].permagic;
                attr.perAgile += blEffects[j].peragile;
            }
        }
        return attr;
    }
    //神器所有属性
    public CardBaseAttribute getCardMagicWeaponAllAttr(Card card) {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (card.magicWeaponUID == null || card.magicWeaponUID == "") return attr;
        MagicWeapon mw = StorageManagerment.Instance.getMagicWeapon(card.magicWeaponUID);
        if (mw == null) return attr;
        attr.attack += mw.magicWeaponAttrbutes.getMagicWeaponAttack();
        attr.hp += mw.magicWeaponAttrbutes.getMagicWeaponHp();
        attr.defecse += mw.magicWeaponAttrbutes.getMagicWeaponDefecse();
        attr.magic += mw.magicWeaponAttrbutes.getMagicWeaponMagic();
        attr.agile += mw.magicWeaponAttrbutes.getMagicWeaponAgile();
        //神器激活的被动技能所附加的属性加成
        List<int> activationSkill = mw.activationSkill;//技能对战斗力的影响
        if (activationSkill == null || activationSkill.Count < 1) return attr;

		if(mw.getPhaseLv() <= mw.getLvType()-1)
		{
			for (int i = 1; i <= mw.getPhaseLv(); i++) {
				SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(activationSkill[i - 1]);
				if (sample == null) {
					//return attr;
				} else {
					if (sample.showType != 1) {
						AttrChangeSample[] attrEffects = sample.effects;
						if(attrEffects != null)
						{
							for (int j = 0; j < attrEffects.Length; j++) {
								if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
									attr.perHp += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
									attr.perAttack += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
									attr.perMagic += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
									attr.perAgile += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
									attr.perDefecse += attrEffects[j].getAttrValue(0);
								}
							}
						}
					}
				}
			}
		}
		else 
		{
			for(int i= mw.getPhaseLv()-(mw.getLvType()-1)+1;i<=mw.getPhaseLv();i++)
			{
				SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(activationSkill[i - 1]);
				if (sample == null) {
					//return attr;
				} else {
					if (sample.showType != 1) {
						AttrChangeSample[] attrEffects = sample.effects;
						if(attrEffects != null)
						{
							for (int j = 0; j < attrEffects.Length; j++) {
								if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
									attr.perHp += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
									attr.perAttack += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
									attr.perMagic += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
									attr.perAgile += attrEffects[j].getAttrValue(0);
								} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
									attr.perDefecse += attrEffects[j].getAttrValue(0);
								}
							}
						}
					}
				}
			}
		}
//        for (int i = 1; i <= mw.getPhaseLv(); i++) {
//            SkillSample sample = SkillSampleManager.Instance.getSkillSampleBySid(activationSkill[i - 1]);
//            if (sample == null) {
//                //return attr;
//            } else {
//                if (sample.showType != 1) {
//                    AttrChangeSample[] attrEffects = sample.effects;
//					if(attrEffects != null)
//					{
//						for (int j = 0; j < attrEffects.Length; j++) {
//							if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
//								attr.perHp += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
//								attr.perAttack += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
//								attr.perMagic += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
//								attr.perAgile += attrEffects[j].getAttrValue(0);
//							} else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
//								attr.perDefecse += attrEffects[j].getAttrValue(0);
//							}
//						}
//					}
////                    if (attrEffects == null) return attr;
////                    for (int j = 0; j < attrEffects.Length; j++) {
////                        if (attrEffects[j].getAttrType() == AttrChangeType.PER_HP) {
////                            attr.perHp += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_ATTACK) {
////                            attr.perAttack += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_MAGIC) {
////                            attr.perMagic += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_AGILE) {
////                            attr.perAgile += attrEffects[j].getAttrValue(0);
////                        } else if (attrEffects[j].getAttrType() == AttrChangeType.PER_DEFENSE) {
////                            attr.perDefecse += attrEffects[j].getAttrValue(0);
////                        }
////                    }
//                }
//            }
//        }
        return attr;
    }
	/** 获得卡片星魂属性值影响(所有属性) */
	public CardBaseAttribute getCardStarSoulEffectAll (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		StarSoul[] starsouls = card.getStarSoulByAll ();
		if (starsouls == null)
			return attr;
		for (int i=0; i<starsouls.Length; i++) {
			StarSoul ss = starsouls [i];
			attr.hp += ss.getAttrChangesByType (AttrChangeType.HP, ss.getLevel ());
			attr.attack += ss.getAttrChangesByType (AttrChangeType.ATTACK, ss.getLevel ());
			attr.defecse += ss.getAttrChangesByType (AttrChangeType.DEFENSE, ss.getLevel ());
			attr.magic += ss.getAttrChangesByType (AttrChangeType.MAGIC, ss.getLevel ());
			attr.agile += ss.getAttrChangesByType (AttrChangeType.AGILE, ss.getLevel ());
			attr.perHp += ss.getAttrChangesByType (AttrChangeType.PER_HP, ss.getLevel ());
			attr.perAttack += ss.getAttrChangesByType (AttrChangeType.PER_ATTACK, ss.getLevel ());
			attr.perDefecse += ss.getAttrChangesByType (AttrChangeType.PER_DEFENSE, ss.getLevel ());
			attr.perMagic += ss.getAttrChangesByType (AttrChangeType.PER_MAGIC, ss.getLevel ());
			attr.perAgile += ss.getAttrChangesByType (AttrChangeType.PER_AGILE, ss.getLevel ());
		}
		return attr;
	}
	/** 获得卡片星魂属性值影响(不包含特殊属性) */
	public CardBaseAttribute getCardStarSoulEffect (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		StarSoul[] starsouls = card.getStarSoulByAll ();
		if (starsouls == null)
			return attr;
		for (int i=0; i<starsouls.Length; i++) {
			StarSoul ss = starsouls [i];
			attr.hp += ss.getAttrChangesByType (AttrChangeType.HP, ss.getLevel ());
			attr.attack += ss.getAttrChangesByType (AttrChangeType.ATTACK, ss.getLevel ());
			attr.defecse += ss.getAttrChangesByType (AttrChangeType.DEFENSE, ss.getLevel ());
			attr.magic += ss.getAttrChangesByType (AttrChangeType.MAGIC, ss.getLevel ());
			attr.agile += ss.getAttrChangesByType (AttrChangeType.AGILE, ss.getLevel ());
		}
		return attr;
	}
	/** 获得卡片星魂属性值影响(百分比属性) */
	public CardBaseAttribute getCardStarSoulEffectPer (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		StarSoul[] starsouls = card.getStarSoulByAll ();
		if (starsouls == null)
			return attr;
		for (int i=0; i<starsouls.Length; i++) {
			StarSoul ss = starsouls [i];
			attr.perHp += ss.getAttrChangesByType (AttrChangeType.PER_HP, ss.getLevel ());
			attr.perAttack += ss.getAttrChangesByType (AttrChangeType.PER_ATTACK, ss.getLevel ());
			attr.perDefecse += ss.getAttrChangesByType (AttrChangeType.PER_DEFENSE, ss.getLevel ());
			attr.perMagic += ss.getAttrChangesByType (AttrChangeType.PER_MAGIC, ss.getLevel ());
			attr.perAgile += ss.getAttrChangesByType (AttrChangeType.PER_AGILE, ss.getLevel ());
		}
		return attr;
	}
    /// <summary>
    /// 获得装备精炼属性百分比影响
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    public CardBaseAttribute getRefineCombatPer(Card _card)
    {
        CardBaseAttribute attr = new CardBaseAttribute();
        if (_card.getEquips() == null || _card.getEquips().Length < 1)
            return attr;
        string[] equips = _card.getEquips();
        for (int i = 0; i < equips.Length; i++)
        {
            Equip equip = StorageManagerment.Instance.getEquip(equips[i]);
            if (RefineSampleManager.Instance.getRefineSampleBySid(equip.sid) == null)
                continue;
            RefinelvInfo rfinfo = RefineSampleManager.Instance.getRefineSampleBySid(equip.sid).refinelvAttr[equip.getrefineLevel()];

            int equipRefineLevel = equip.getrefineLevel();
            int[] a = new int[3];
            string[] b = new string[3];
            for (int j = 0; j <= equipRefineLevel; j++)
            {
                RefinelvInfo newrfinfo = RefineSampleManager.Instance.getRefineSampleBySid(equip.sid).refinelvAttr[j];
                for (int l = 0; l < newrfinfo.items.Count; l++)
                {
                    AttrRefineChangeSample acs = newrfinfo.items[l];
                    for (int k = 0; k < 3; k++)
                    {
                        if (b[k] == null)
                        {
                            b[k] = acs.getAttrType();
                            a[k] += acs.getAttrRefineValue(0);
                            break;
                        }
                        else
                        {
                            if (b[k] == acs.getAttrType())
                            {
                                a[k] += acs.getAttrRefineValue(0);
                                break;
                            }
                        }
                    }
                    for (int m = 0; m < 3; m++)
                    {
                        if (b[m] != null)
                        {
                            switch (b[m])
                            {
                                case AttrChangeType.PER_AGILE:
                                    attr.perAgile += a[m];
                                    break;
                                case AttrChangeType.PER_ATTACK:
                                    attr.perAttack += a[m];
                                    break;
                                case AttrChangeType.PER_DEFENSE:
                                    attr.perDefecse += a[m];
                                    break;
                                case AttrChangeType.PER_HP:
                                    attr.perHp += a[m];
                                    break;
                                case AttrChangeType.PER_MAGIC:
                                    attr.perMagic += a[m];
                                    break;
                            }
                        }
                    }

                }
            }
        }
        return attr;
    }
    /// <summary>
    /// 获得装备共鸣百分比影响
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    public CardBaseAttribute getResonanceEffectPer(Card _card)
    {
        CardBaseAttribute attr = new CardBaseAttribute();
        for (int i = 1; i < 4; i++)
        {
            List<ResonanceSample> rsss = ResonanceSampleManager.Instance.getrssList(i, _card, 0, 0);
            for (int k = 0; k < rsss.Count; k++)
            {
                ResonanceSample newRsS = rsss[k];
                ResonanceInfo newRiI = newRsS.resonanceAttr;
                for (int j = 0; j < newRiI.items.Count; j++)
                {
                    AttrChangeSample acs = newRiI.items[j];
                    switch (acs.getAttrType())
                    {
                        case AttrChangeType.PER_AGILE:
                            attr.perAgile += acs.getAttrValue(0);
                            break;
                        case AttrChangeType.PER_ATTACK:
                            attr.perAttack += acs.getAttrValue(0);
                            break;
                        case AttrChangeType.PER_DEFENSE:
                            attr.perDefecse += acs.getAttrValue(0);
                            break;
                        case AttrChangeType.PER_HP:
                            attr.perHp += acs.getAttrValue(0);
                            break;
                        case AttrChangeType.PER_MAGIC:
                            attr.perMagic += acs.getAttrValue(0);
                            break;
                    }

                }
            }
        }

        return attr;
    }
	
	/** 获得卡片被动技能属性影响值 */
	public CardBaseAttribute getCardSkillEffect (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		Skill[] skills = card.getAttrSkills ();
		if (skills == null || skills.Length < 1)
			return attr;
		for (int i = 0; i < skills.Length; i++) {
			attr.mergeCardBaseAttr (skills [i].getSkillEffect ());
		}
		return attr;
	}

	/** 获得卡片被动技能属性影响值 */
	public CardBaseAttribute getCardSkillEffectNum (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		Skill[] skills = card.getAttrSkills ();
		if (skills == null || skills.Length < 1)
			return attr;
		for (int i = 0; i < skills.Length; i++) {
			attr.mergeCardBaseNum (skills [i].getSkillEffect ());
		}
		return attr;
	}

	/** 获得卡片被动技能属性影响值 */
	public CardBaseAttribute getCardSkillEffectPer (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		Skill[] skills = card.getAttrSkills ();
		if (skills == null || skills.Length < 1)
			return attr;
		for (int i = 0; i < skills.Length; i++) {
			attr.mergeCardBasePer (skills [i].getSkillEffect ());
		}
		return attr;
	}
	
	/** 获得卡片套装基础属性加成 */
	public CardBaseAttribute getCardSuitEffect (Card card)
	{
		return SuitManagerment.Instance.getSuitBaseAttrChange (card.getEquips (),getEquipStarLevel(card));
	}
	
	/** 获得卡片套装属性比例加成 */
	public CardBaseAttribute getCardSuitEffectByPer (Card card)
	{
		return SuitManagerment.Instance.getSuitBaseAttrByPer (card.getEquips (),getEquipStarLevel(card));
	}
	
	/** 获得共鸣比例加成 */
	public CardBaseAttribute getCardBeastEffectByPer ()
	{
		return BeastEvolveManagerment.Instance.getBeastResonanceEffectByPer ();
	}
	
	/** 获得装备套装属性属性变化信息 */
	public SuitAttrChange[] getCardSuitAttrChanges (Card card,int starLevel)
	{
		return SuitManagerment.Instance.getEquipsSuitAttrChanges (card.getEquips (),starLevel);
	}
	
	/** 获得当前进化属性加成 */
	public CardBaseAttribute getEvolutionAttr (Card card)
	{
		return EvolutionManagerment.Instance.getEvolutionAttr (card);
	}

	/** 获得当前进化属性比例加成 */
	public CardBaseAttribute getEvolutionAttrPer (Card card)
	{
		return EvolutionManagerment.Instance.getEvolutionAttrPer (card);
	}
	
	/** 获得当前突破属性加成 */
	public CardBaseAttribute getSurmountAttr (Card card)
	{
		return SurmountManagerment.Instance.getSurmountAttr (card);
	}

	/** 获得当前突破属性比例加成 */
	public CardBaseAttribute getSurmountAttrPer (Card card)
	{
		return SurmountManagerment.Instance.getSurmountAttrPer (card);
	}
	
	/** 获得卡片附加等级属性影响值 */
	public CardBaseAttribute getCardAppendEffectNoPer (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		attr.attack = getCardAppendAttr (card, AttributeType.attack);
		attr.hp = getCardAppendAttr (card, AttributeType.hp);
		attr.defecse = getCardAppendAttr (card, AttributeType.defecse);
		attr.magic = getCardAppendAttr (card, AttributeType.magic);
		attr.agile = getCardAppendAttr (card, AttributeType.agile);
		return attr;
	} 
	
	/** 获得卡片附加等级属性影响值(含套装比例加成效果) */
	public CardBaseAttribute getCardAppendEffectNoSuit (Card card)
	{
		CardBaseAttribute whole = new CardBaseAttribute ();
		whole.attack = getCardAppendAttr (card, AttributeType.attack);
		whole.hp = getCardAppendAttr (card, AttributeType.hp);
		whole.defecse = getCardAppendAttr (card, AttributeType.defecse);
		whole.magic = getCardAppendAttr (card, AttributeType.magic);
		whole.agile = getCardAppendAttr (card, AttributeType.agile);
		//套装比例加成
		CardBaseAttribute attrSuitByPer = getCardSuitEffectByPer (card);
		whole.mergeCardBaseAttr (attrSuitByPer);
		//卡片被动技能属性影响值
		CardBaseAttribute attrSkill = getCardSkillEffectPer (card);
		whole.mergeCardBaseAttr (attrSkill);
        //装备精炼百分比
        CardBaseAttribute attrEquipsRefine = getRefineCombatPer(card);
        whole.mergeCardBaseAttr(attrEquipsRefine);
        //装备共鸣的百分比
        CardBaseAttribute attrResonanceshuxing = getResonanceEffectPer(card);
        whole.mergeCardBaseAttr(attrResonanceshuxing);
		//星魂百分比
		CardBaseAttribute attrStarSoul = getCardStarSoulEffectPer (card);
		whole.mergeCardBaseAttr (attrStarSoul);
        //神器百分比
        CardBaseAttribute attrMagicWeapon = getCardMagicWeaponAdd(card);
        whole.mergeCardBaseAttr(attrMagicWeapon);
        //血脉基础百分比
        CardBaseAttribute attrBloodPerAttr = getCardBloodPerAttribute(card);
        whole.mergeCardBaseAttr(attrBloodPerAttr);
        //血脉附加技能的加成百分比
        CardBaseAttribute attrBloodSkillPreAttr = getCardBloodSkillAdd(card);
        whole.mergeCardBaseAttr(attrBloodSkillPreAttr);
		if (card.getCardType () == 1) {
			if (isMyCard (card)) {
				//共鸣等级
				CardBaseAttribute attrBeast = getCardBeastEffectByPer ();
				whole.mergeCardBaseAttr (attrBeast);
				//星盘属性比例加成
				CardBaseAttribute attrStarPer = GoddessAstrolabeManagerment.Instance.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrStarPer);
				
				//天梯称号比例加成
				CardBaseAttribute attrLadder_Title = LaddersManagement.Instance.TitleAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_Title);
				
				//天梯奖章比例加成
				CardBaseAttribute attrLadder_Medal = LaddersManagement.Instance.MedalAttrEffect.getAttrByAllNumber ();
				whole.mergeCardBaseAttr (attrLadder_Medal);
                //队伍中的人才加坐骑属性
                if (card.isInTeam()) {
                    // 坐骑技能属性加成
                    CardBaseAttribute mountsSkillAttribute = MountsManagerment.Instance.getUseMountsSkillEffect();
                    whole.mergeCardBaseAttr(mountsSkillAttribute);
                }
			}
		}
		//进化属性比例加成
		CardBaseAttribute attrEvoPer = getEvolutionAttrPer (card);
		whole.mergeCardBaseAttr (attrEvoPer);
		//如果是主角就有突破属性比例加成
		if (card.uid == UserManager.Instance.self.mainCardUid) {
			CardBaseAttribute attrSur = getSurmountAttrPer (card);
			whole.mergeCardBaseAttr (attrSur);
		}
		return whole;
	}
	
	/** 获得指定装备 */
	private Equip getEquipById (string id)
	{
		return StorageManagerment.Instance.getEquip (id);
	} 
	
	/** 获得卡片基础属性值 */
	public CardBaseAttribute getCardAttribute (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		if (card.getCardType () == 1) {
			attr.attack = getCardBaseSingleAttribute (card, AttributeType.attack) + KnighthoodConfigManager.Instance.getKnighthoodByGrade (UserManager.Instance.self.honorLevel).values [1].currentValue;
			attr.hp = getCardBaseSingleAttribute (card, AttributeType.hp) + KnighthoodConfigManager.Instance.getKnighthoodByGrade (UserManager.Instance.self.honorLevel).values [0].currentValue;
			attr.defecse = getCardBaseSingleAttribute (card, AttributeType.defecse) + KnighthoodConfigManager.Instance.getKnighthoodByGrade (UserManager.Instance.self.honorLevel).values [2].currentValue;
			attr.magic = getCardBaseSingleAttribute (card, AttributeType.magic) + KnighthoodConfigManager.Instance.getKnighthoodByGrade (UserManager.Instance.self.honorLevel).values [3].currentValue;
			attr.agile = getCardBaseSingleAttribute (card, AttributeType.agile) + KnighthoodConfigManager.Instance.getKnighthoodByGrade (UserManager.Instance.self.honorLevel).values [4].currentValue;
		} else {
			attr.attack = getCardBaseSingleAttribute (card, AttributeType.attack);
			attr.hp = getCardBaseSingleAttribute (card, AttributeType.hp);
			attr.defecse = getCardBaseSingleAttribute (card, AttributeType.defecse);
			attr.magic = getCardBaseSingleAttribute (card, AttributeType.magic);
			attr.agile = getCardBaseSingleAttribute (card, AttributeType.agile);
			
		}
		return attr;
	} 
	
	/** 获得角色基础属性(初始值+（等级-1）*等级成长值） */
	public int getCardBaseSingleAttribute (Card card, AttributeType attr)
	{ 
		int baseAttr = CardSampleManager.Instance.getBaseAttribute (card.sid, attr);
		int developAttr = CardSampleManager.Instance.getLevelUpAttribute (card.sid, attr); 
		return baseAttr + developAttr * (card.getLevel () - 1);
	}
	
	/** 获得卡片附加属性值 */
	public int getCardAppendAttr (Card card, AttributeType attr)
	{ 
		int appendAttr = CardSampleManager.Instance.getAppendUpAttribute (attr);  
		return  appendAttr * getCardAttrAppendLevel (card, attr);
	}
	
	/** 获得对应属性值 */
	public int getCardAppendAttr (int lv, AttributeType attr)
	{
		int appendAttr = CardSampleManager.Instance.getAppendUpAttribute (attr);  
		return appendAttr * lv;
	}
	
	/** 获得卡片属性附加等级 */
	public int getCardAttrAppendLevel (Card card, AttributeType attr)
	{
		if (attr == AttributeType.attack) {
			return card.getATTGrade ();
		} else if (attr == AttributeType.hp) {
			return card.getHPGrade ();
		} else if (attr == AttributeType.defecse) {
			return card.getDEFGrade ();
		} else if (attr == AttributeType.magic) {
			return card.getMAGICGrade ();
		} else if (attr == AttributeType.agile) {
			return card.getAGILEGrade ();
		} else {
			throw new Exception ("getAppendUpAttribute role attribute error! attr = " + attr);
		}
	}

	#endregion

	public string jobIDToString (int id)
	{ 
		switch (id) {
		case JobType.POWER:
			return LanguageConfigManager.Instance.getLanguage ("s0234");
		case JobType.AGILE:
			return LanguageConfigManager.Instance.getLanguage ("s0236");
		case JobType.MAGIC:
			return LanguageConfigManager.Instance.getLanguage ("s0235");
		case JobType.ASSIST:
			return LanguageConfigManager.Instance.getLanguage ("s0239");
		case JobType.COUNTER_ATTACK:
			return LanguageConfigManager.Instance.getLanguage ("s0238");
		case JobType.POISON:
			return LanguageConfigManager.Instance.getLanguage ("s0237");
		}
		return "?";
	}
	/** 获取卡片品质文字图标 */
	public string qualityIconTextToBackGround (int id)
	{ 
		switch (id) {
		case JobType.POWER:
			return "roleType_2";
		case JobType.AGILE:
			return "roleType_3";
		case JobType.MAGIC:
			return "roleType_5";
		case JobType.ASSIST:
			return "roleType_6";
		case JobType.COUNTER_ATTACK:
			return "roleType_1";
		case JobType.POISON:
			return "roleType_4";
		}
		return "?";
	}
	
	public int qualityStringToID (string str)
	{ 
		switch (str) {
		case "quality_1":
			return  QualityType.COMMON;
		case "quality_2":
			return  QualityType.EXCELLENT;
		case "quality_3":
			return  QualityType.GOOD;
		case "quality_4":
			return  QualityType.EPIC;
		case "quality_5":
			return  QualityType.LEGEND;
		} 
		return 0;  
	}

	public string qualityToCardName (int quality)
	{ 
		string colorStr = QualityManagerment.getQualityColor (quality);
		string qualityName = "";
		switch (quality) {
		case QualityType.COMMON:
			qualityName = LanguageConfigManager.Instance.getLanguage ("s0073");
			break;
		case QualityType.EXCELLENT:
			qualityName = LanguageConfigManager.Instance.getLanguage ("s0074");
			break;
		case QualityType.GOOD:
			qualityName = LanguageConfigManager.Instance.getLanguage ("s0075");
			break;
		case QualityType.EPIC:
			qualityName = LanguageConfigManager.Instance.getLanguage ("s0076");
			break;
		case QualityType.LEGEND:
			qualityName = LanguageConfigManager.Instance.getLanguage ("s0077");
			break;
		} 
		return colorStr + qualityName + LanguageConfigManager.Instance.getLanguage ("luckdraw13") + "[-]";
	}
	
	//判断能否自动穿装
	public bool isAutoPutonEquip (Card card)
	{
		Condition[] cons = new Condition[1];
		cons [0] = new Condition (SortType.EQUIP_STATE);
		cons [0].addCondition (EquipStateType.LOCKED);//添加筛选条件 锁定中
		
		Condition con = new Condition (SortType.SORT);
		con.addCondition (SortType.SORT_POWERDOWN);		
		SortCondition sc = new SortCondition ();
		sc.sortCondition = con;
		sc.siftConditionArr = cons;
		
		//获得空闲装备 包含锁定装备
		ArrayList list = SortManagerment.Instance.equipSort (StorageManagerment.Instance.getAllEquip (), sc);
		if (list.Count < 1)
			return false; 
		
		int[] temp = new int[]{1,2,3,4,5};//临时数组  存储未判断装备部位
		string[] cardEquips = card.getEquips ();
		if (cardEquips == null || cardEquips.Length < 1) {
			return true;
		} else { 
			for (int i = 0; i < cardEquips.Length; i++) {
				Equip eq = StorageManagerment.Instance.getEquip (cardEquips [i]);
				if (EquipManagerment.Instance.isStrongestEquip (list, eq))//如果找到更强的装备
					return true;
			}
		}
		
		return false;
	}

	/// <summary>
	/// 这张卡是不是我的
	/// </summary>
	public bool isMyCard (Card card)
	{
		if (card.uid == "") {
			return false;
		} else if (card.uid == "-1") {
			return true;
		}
		ArrayList cards = StorageManagerment.Instance.getAllRole ();

		for (int i = 0; i < cards.Count; i++) {
			if ((cards [i] as Card).uid == card.uid) {
				return true;
			}
		}

		return false;
	}

	/** 是否为全套 */
	public bool isFullSuit(string[] equipsUid) {
		if (equipsUid == null || equipsUid.Length==0)
			return false;
		Equip equip = StorageManagerment.Instance.getEquip (equipsUid [0]);
		if (equip == null)
			return false;
		SuitSample suitSample = SuitSampleManager.Instance.getSuitSampleBySid (equip.getSuitSid ());
		if(suitSample==null)
			return false;
		int totalSuitCount=suitSample.parts.Count;
		int currentSuitCount=0;
		for (int i=0; i<totalSuitCount; i++) {
			int j = 0;
			for (;j < equipsUid.Length; j++) {
				equip = StorageManagerment.Instance.getEquip (equipsUid[j]);
				if(equip==null)
					continue;
                if (suitSample.parts[i].rSid == equip.sid || suitSample.parts[i].ySid == equip.sid)
					break;
			}
			if(j!=equipsUid.Length)
				currentSuitCount++;
		}
		if (currentSuitCount == totalSuitCount)
			return true;
		return false;
	}

	/** 是否为全套 */
	public bool isFullSuit(List<Equip> equips) {
		if (equips == null || equips.Count==0)
			return false;
		SuitSample suitSample = SuitSampleManager.Instance.getSuitSampleBySid (equips[0].getSuitSid ());
		if(suitSample==null)
			return false;
		Equip equip;
		int totalSuitCount=suitSample.parts.Count;
		int currentSuitCount=0;
		for (int i=0; i<totalSuitCount; i++) {
			int j = 0;
			for (;j < equips.Count; j++) {
				equip=equips[j];
				if(equip==null)
					continue;
                if (suitSample.parts[i].rSid == equip.sid || suitSample.parts[i].ySid == equip.sid)
					break;
			}
			if(j!=equips.Count)
				currentSuitCount++;
		}
		if (currentSuitCount == totalSuitCount)
			return true;
		return false;
	}

	///<summary>
	/// 获得星级套装等级
	/// </summary>
	public int getEquipStarLevel(Card card){
		bool isSuit = false;
		int level = int.MaxValue;
		if (showChatEquips != null && showChatEquips.Count>0 && card != null && StorageManagerment.Instance.getRole(card.uid) == null) {
			isSuit = isFullSuit (showChatEquips);
			if (isSuit) {
				Equip equip;
				for (int i = 0; i < showChatEquips.Count; i++) {
					equip = showChatEquips[i];
					if(equip==null){
						level=0;
						continue;
					}
					if (equip.equpStarState < level) {
						level=equip.equpStarState;
					}
				}
			} else {
				return 0;
			}
		}	
		else {
			if (card == null)
				return 0;
			string[] equips = card.getEquips ();
			isSuit = isFullSuit (equips);
			if (isSuit) {
				Equip equip;
				for (int i = 0; i < equips.Length; i++) {
					equip = StorageManagerment.Instance.getEquip(equips[i]);
					if(equip==null){
						level=0;
						continue;
					}
					if (equip.equpStarState < level) {
						level=equip.equpStarState;
					}
				}
				
			} else {
				return 0;
			}
		}
		return level;
	}


}  
