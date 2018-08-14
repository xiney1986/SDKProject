using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
 
/**
 * 角色实体类
 * @author longlingquan
 * */
public class Card:StorageProp
{
	public Card ()
	{
		this.isU = true;//写死了
	}

	public Card (string uid, int sid, int index, long exp, int hplevel, int attlevel, int deflevel, int magiclevel, int agilelevel, Skill[] skills, Skill[] buffskills
	             , Skill[] attrskills, string[] equips, int state, int evoLevel, int surLevel, int mainSkillSlot, int attrSkillSlot, int buffSkillSlot,string magicWeaponUID,int bloodLv)
	{
		this.uid = uid;
		this.sid = sid;
		this.index = index; 
		this.hpExp = hplevel;
		this.attExp = attlevel;
		this.defExp = deflevel;
		this.magicExp = magiclevel;
		this.agileExp = agilelevel;
		this.skills = skills;
		this.buffskills = buffskills;
		this.attrskills = attrskills;
		this.equips = equips;
		this.state = state;
		this.evoLevel = evoLevel;
        this.surLevel = surLevel;
        this.cardBloodLevel = bloodLv;
        updateExp(exp);
		this.mainSkillSlot = mainSkillSlot;
		this.attrSkillSlot = attrSkillSlot;
		this.buffSkillSlot = buffSkillSlot;
		this.isU = true;//写死了
        this.magicWeaponUID = magicWeaponUID;
	}

    public Card(string uid, int sid, int index, long exp, int hplevel, int attlevel, int deflevel, int magiclevel, int agilelevel, Skill[] skills, Skill[] buffskills
                 , Skill[] attrskills, string[] equips, int state, int evoLevel, int surLevel, int mainSkillSlot, int attrSkillSlot, int buffSkillSlot, bool isShowInfo,MagicWeapon magicweapon,int bloodLv)
    {
        this.IsShowInfo = isShowInfo;
        this.uid = uid;
        this.sid = sid;
        this.index = index;
        this.hpExp = hplevel;
        this.attExp = attlevel;
        this.defExp = deflevel;
        this.magicExp = magiclevel;
        this.agileExp = agilelevel;
        this.skills = skills;
        this.buffskills = buffskills;
        this.attrskills = attrskills;
        this.equips = equips;
        this.state = state;
        this.evoLevel = evoLevel;
        this.surLevel = surLevel;
        this.cardBloodLevel = bloodLv;
        updateExp(exp);
        this.mainSkillSlot = mainSkillSlot;
        this.attrSkillSlot = attrSkillSlot;
        this.buffSkillSlot = buffSkillSlot;
        this.isU = true;//写死了
        this.magicWeaponUID = "";
        this.magicWapon = magicweapon;

    }
    

	public int CardAwake {
		get {
			return cardAwaken;
		}
		set {
			cardAwaken = value;
		}
	}

    public bool IsShowInfo = false; //是否是点击相关的东西,查看这张卡牌详细信息,比如查看好友的卡牌
	private long exp = 0;//经验值
	private ArrayList bores;//星魂槽列表
	private string[] equips;//装备信息 ids 唯一id
	private Skill[] skills;//技能信息
	private Skill[] buffskills;//开场技能
	private Skill[] attrskills;//属性加成技能
	public int state;//卡片状态
	private int level = 0;//等级
	private int hpExp = 0;//生命附加经验
	private int attExp = 0;//攻击附加经验
	private int defExp = 0;//防御附加经验
	private int magicExp = 0;//魔力附加经验
	private int agileExp = 0;//敏捷附加经验
	private int evoLevel = 0;//进化等级
	private int surLevel = 0;//突破等级
	private int mainSkillSlot = 0;//主动槽位
	private int attrSkillSlot = 0;//被动槽位
	private int buffSkillSlot = 0;//开场槽位
	private int cardAwaken = 0;//觉醒次数
	private int cardcombat = 0;//卡片战力
	private bool expLimitByRole = false; //是否被主角等级限制
	public bool isInherit = false;//是否被传承了
    public string magicWeaponUID = "";//穿戴的秘宝UID
    public int strenglv = 0;
    public int phaseLv = 0;//
    public MagicWeapon magicWapon=null;//别个的卡片上的秘宝
    public int cardBloodLevel=0;//卡片血脉等级
	public int CardCombat {
		get {
			return cardcombat;
		}
		set {
			cardcombat = value;
		}
	}
	/** 获得已开主动技能槽 */
	public int getMainSkillSlot ()
	{
		return mainSkillSlot;
	}

	/** 获得已开被动技能槽 */
	public int getAttrSkillSlot ()
	{
		return attrSkillSlot;
	}

	/** 获得已开开场技能槽 */
	public int getBuffSkillSlot ()
	{
		return buffSkillSlot;
	}

	/** 是否是主卡 */
	public bool isMainCard ()
	{
//		return this.uid == UserManager.Instance.self.mainCardUid; //这里可能是别人的主卡，肯定不能匹配
		return this.sid <= 10;
	}

	/** 获得进化等级 */
	public int getEvoLevel ()
	{
		if (isMainCard ()) {
			return evoLevel;
		} else {
			return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_EVO_EXP, getEvoTimes ());
		}
	}
    public int getEvoLevelForBlood(int cost) {
        if (isMainCard()) {
            return evoLevel;
        } else {
            return EXPSampleManager.Instance.getLevel(EXPSampleManager.SID_EVO_EXP, (getEvoTimes() - cost));
        }
    }

	/** 获得进化最大等级 */
	public int getMaxEvoLevel ()
	{
        int a = Math.Min(EvolutionManagerment.Instance.getMaxLevell(this), EXPSampleManager.Instance.getMaxLevel(EXPSampleManager.SID_EVO_EXP));
		return a;
	}

	/// <summary>
    /// 是否已进化到极限 
	/// </summary>
	public bool isMaxEvoLevel ()
	{
		return EvolutionManagerment.Instance.isMaxEvoLevel (this);
	}

	/** 获得进化次数 */
	public int getEvoTimes ()
	{
		return evoLevel;
	}

	/** 传入进化次数，返回进化等级 */
	public int getTempUpdateEvoLevel (int times)
	{
		int newTimes = times + getEvoTimes ();
		if (isMainCard ()) {
			return newTimes;
		} else {
			return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_EVO_EXP, newTimes);
		}
	}
    /** 获取进化到下一等级需要进化的次数 */
    public int getEvotimesToNextLevel() {
        long max = EXPSampleManager.Instance.getMaxEXPShow(EXPSampleManager.SID_EVO_EXP,getEvoTimes());
        long now = EXPSampleManager.Instance.getNowEXPShow(EXPSampleManager.SID_EVO_EXP,getEvoTimes());
        int times = StringKit.toInt((max - now).ToString());
        return times;
    }

	/** 传入进化次数，查看是否可以提升进化等级 */
	public bool isAddEvoLevel (int times)
	{
		return getTempUpdateEvoLevel (times) > getEvoLevel ();
	}

	/** 获得超级进化进度 */
	public string getSuperEvoProgress ()
	{
		return EXPSampleManager.Instance.getExpBarShow (EXPSampleManager.SID_EVO_EXP, getEvoTimes ());
	}

    

	/// <summary>
    /// 获得当前是否处于超进化阶段
	/// </summary>    
	public bool isInSuperEvo ()
	{
        if (isMaxEvoLevel()) 
            return false;        
        if (getEvoTimes() > 0)
        {
            return EXPSampleManager.Instance.getMaxEXPShow(EXPSampleManager.SID_EVO_EXP, getEvoTimes()) > 1;
        }
	    return false;
	}

	/** 设置进化等级 */
	public void updateEvoLevel (int _evoLv)
	{
		evoLevel = _evoLv;
	    int tempLv = 0;
	    long[] exs = EXPSampleManager.Instance.getEXPSampleBySid(79).getExps();
	    for (int i=0;i<exs.Length-1;i++)
	    {
	        if (exs[i] <=_evoLv&&exs[i+1]>_evoLv) tempLv = i + 1;
	    }
	    int maxLevle = EvolutionManagerment.Instance.getMaxLevel(this);
		if (tempLv > maxLevle)
		{
		    if (maxLevle == 0) evoLevel = 0;
		    evoLevel = (int)exs[maxLevle - 1 >= exs.Length ? exs.Length - 1 : maxLevle - 1];
			//evoLevel = EvolutionManagerment.Instance.getMaxLevel(this);
		}
	}

	/** 进化成功 */
	public void evoOk (int _evoLv)
	{
		evoLevel += _evoLv;
		//手动进化更新仓库版本 返回仓库后更新形象
		StorageManagerment.Instance.RoleStorageVersion += 1;
	}

	/** 突破成功 */
	public void surOk ()
	{
		surLevel++;
	}

	/** 设置突破等级 */
	public void updateSurLevel (int _surLv)
	{
		surLevel = _surLv;
	}

	/** 获得突破等级 */
	public int getSurLevel ()
	{
		return surLevel;
	}
	/** 获得最高等级 */
	public int getMaxLevel ()
	{
		int oldLv = CardSampleManager.Instance.getRoleSampleBySid (sid).maxLevel;
		if (isBeast ())
			return oldLv;
		else {
			int addLv = 0;
			if (isMainCard ())
				addLv = SurmountManagerment.Instance.getAddMaxSurLevel (this);
			else
				addLv = EvolutionManagerment.Instance.getAddLevel (this);

			return oldLv + addLv;
		}
	}
	/// <summary>
	/// 检测准备加的经验是否会超等级
	/// </summary>
	public long checkExp(long _exp){
		if(_exp==0)return -1;
		int mc =0;
		if (!isMainCard() && UserManager.Instance.self != null && StorageManagerment.Instance.getAllRole() != null && 
		    StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid) != null && !IsShowInfo) {
			mc = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).getLevel();
		}
		mc = Math.Min (getMaxLevel (),mc);
		if (EXPSampleManager.Instance.getLevel (getEXPSid (), getEXP () + _exp) >= mc) {
			//大于=最大等级,返回等级exp上限-1
			return 	EXPSampleManager.Instance.getEXPUp(getEXPSid (),mc-1<=0?0:mc-1);
		}

		return -1;
	}
    /// <summary>
    /// 检测准备加的经验是否会超等级
    /// </summary>
    public long checkExpforTr(long _exp) {
        if (_exp == 0) return -1;
        int mc = 0;
        if (!isMainCard() && UserManager.Instance.self != null && StorageManagerment.Instance.getAllRole() != null &&
            StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid) != null && !IsShowInfo) {
            mc = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).getLevel();
        }
        mc = Math.Min(getMaxLevel(), mc);
       // Debug.LogError("getEXPSID===" + getEXPSid().ToString());
        if (EXPSampleManager.Instance.getLevel(getEXPSid(), getEXP() + _exp) >= mc) {
            //大于=最大等级,返回等级exp上限-1
           // Debug.LogError("getEXPUp" + (EXPSampleManager.Instance.getEXPUp(getEXPSid(), mc) - 1).ToString());
            return EXPSampleManager.Instance.getEXPUp(getEXPSid(), mc-1);
        }

        return -1;
    }
    /// <summary>
    /// 获得最高等级,不同于会获取真实等级不受主卡影响的最大等级
    /// </summary>
    public int getMyMaxLevel()
    {
        int oldLv = CardSampleManager.Instance.getRoleSampleBySid(sid).maxLevel;
        int addLv = 0;
        if (isMainCard())
            addLv = SurmountManagerment.Instance.getAddMaxSurLevel(this);
        else
            addLv = EvolutionManagerment.Instance.getAddLevel(this);
        return oldLv + addLv;
    }
    

	/** 获得技能学习消耗 */
	public int getCardSkillLearnCast ()
	{ 
		return IntensifyCostManager.Instance.getCostListBySid (1) [getQualityId () - 1];

	}

	/** 返回战斗力 */
	public int getCardCombat ()
	{
		if (!isBeast ())
			return CombatManager.Instance.getCardCombat (this);
		else
			return CombatManager.Instance.getBeastEvolveCombat (this);
	}

	/** 获得技能升级消耗 */
	public int getCardSkillLevelUpCast ()
	{
		return IntensifyCostManager.Instance.getCostListBySid (2) [getQualityId () - 1];
	}

	/** 获得附加属性消耗 */
	public int getCardAddonCast ()
	{
		return 1000;
	}

	/** 获得继承卡片经验的系数 */
	public int getCardExpLevelUpCast ()
	{
		return  IntensifyCostManager.Instance.getCostListBySid (4) [getQualityId () - 1];
	}
	
	public PrizeSample[] getResolveResults ()
	{
		PrizeSample[] sr = CardSampleManager.Instance.getRoleSampleBySid (sid).getClonePrizeSample ();
		if (sr == null)
			return null;
		PrizeSample prizeSample;
		for (int i = 0; i < sr.Length; i++) {
			prizeSample = sr [i];
			if (prizeSample.type == PrizeType.PRIZE_MONEY) {
				prizeSample.addNum (getEvoTimes () * StringKit.toInt (prizeSample.num) + (int)EvolutionManagerment.Instance.getCostMoney (this));
			} else {
				prizeSample.addNum (getEvoTimes () * StringKit.toInt (prizeSample.num));
			}
		}
		sr = AllAwardViewManagerment.Instance.contrastToArray (sr);
		return sr;
	}
	
	public string[] getEquips ()
	{
		return this.equips;
        
	
	}

    public List<Equip> getEquipList()
    {
        if (equips == null || equips.Length < 0) return null;
        List<Equip> tempList=new List<Equip>();
        for (int i = 0; i < equips.Length; i++)
        {
            Equip eq = StorageManagerment.Instance.getEquip(equips[i]);
            if(eq!=null)
            tempList.Add(eq);
        }
        return tempList;
    } 

	public void setEquips (string[] equips)
	{
		if (equips != null) {
			this.equips = new string[equips.Length];
			Array.Copy (equips, this.equips, equips.Length);
		} else {
			this.equips = null;
		}
	}

	/** 获取卡片星魂槽列表 */
	public ArrayList getStarSoulBores () {
		return this.bores;
	}

	/// <summary>
	/// 获取指定位置的星魂槽
	/// </summary>
	/// <param name="index">Index.</param>
	public StarSoulBore getStarSoulBoreByIndex(int index) {
		if (bores == null)
			return null;
		StarSoulBore starSoulBore;
		for (int i=0; i<bores.Count; i++) {
			starSoulBore=(StarSoulBore)bores[i];
			if(starSoulBore.checkStarSoulBoreByIndex(index))
				return starSoulBore;
		}
		return null;
	}

	/// <summary>
	/// 获取指定星魂UID在星魂槽的下标
	/// </summary>
	/// <param name="uid">uid</param>
	public StarSoulBore getStarSoulBoreIndexByUid(string uid) {
		if (bores == null)
			return null;
		StarSoulBore starSoulBore;
		for (int i=0; i<bores.Count; i++) {
			starSoulBore=(StarSoulBore)bores[i];
			if(starSoulBore.checkStarSoulBoreByUid(uid))
				return starSoulBore;
		}
		return null;
	}
	/// <summary>
	/// 移除卡片星魂槽的星魂
	/// <param name="index">星魂槽位置</param>
	/// </summary>
	public void delStarSoulBoreByAll () {
		if (bores == null || bores.Count < 1)
			return;
		StorageManagerment smanager=StorageManagerment.Instance;
		StarSoulBore starSoulBore;
		for (int i=0; i<bores.Count; i++) {
			starSoulBore=(StarSoulBore)bores[i];
			StarSoul starSoul=smanager.getStarSoul(starSoulBore.getUid());
			if(starSoul!=null) {
				starSoul.unState(EquipStateType.OCCUPY);
				starSoul.isNew=false;
			}
			bores.RemoveAt(i--);
		}
	}
	/// <summary>
	/// 移除卡片星魂槽的星魂
	/// <param name="index">星魂槽位置</param>
	/// </summary>
	public bool delStarSoulBoreByIndex(int index) {
		if (bores == null)
			return false;
		StorageManagerment smanager=StorageManagerment.Instance;
		StarSoulBore starSoulBore;
		for (int i=0; i<bores.Count; i++) {
			starSoulBore=(StarSoulBore)bores[i];
			if(starSoulBore.checkStarSoulBoreByIndex(index)) {
				StarSoul starSoul=smanager.getStarSoul(starSoulBore.getUid());
				if(starSoul!=null) {
					starSoul.unState(EquipStateType.OCCUPY);
					starSoul.isNew=false;
				}
				bores.RemoveAt(i);
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 添加卡片上的星魂槽
	/// </summary>
	public void addStarSoulBore(string uid,int hole) {
		if (bores == null)
			bores = new ArrayList ();
		StarSoulBore starSoulBore=getStarSoulBoreByIndex (hole);
		if (starSoulBore == null) {
			starSoulBore=new StarSoulBore(uid,hole);
			bores.Add (starSoulBore);
		}
		else{
			starSoulBore.setUid(uid);
		}
	}
	/// <summary>
	/// 获取星魂槽上所有的星魂
	/// </summary>
	public StarSoul[] getStarSoulByAll () {
		if (bores == null||bores.Count==0)
			return null;
		List<StarSoul> temList = new List<StarSoul>();
		StarSoul starSoul;
		StorageManagerment manager = StorageManagerment.Instance;
		StarSoulBore starSoulBore;
		for (int i=0; i<bores.Count; i++) {
			starSoulBore=(StarSoulBore)bores[i];
            if (starSoulBore.getSid() != 0) {
                starSoul = new StarSoul("0",starSoulBore.getSid(),starSoulBore.getExp(),0);
            } else {
                starSoul = manager.getStarSoul(starSoulBore.getUid());
            }
			
			if(starSoul==null) continue;
			temList.Add(starSoul);
		}

		return temList.ToArray();
	}

	/// <summary>
	/// 获取星魂槽上所有的星魂
	/// </summary>
	public ArrayList getStarSoulArrayList ()
	{
		return bores;
	}

	/// <summary>
	/// 替换星魂
	/// </summary>
	public void setStarSoul (ArrayList _bores)
	{
		this.bores = _bores;
	}

	/// <summary>
	/// 获得卡片上指定刻印的数量
	/// </summary>
	public int getStarSoulsPartNum (int partId) {
		int count = 0;
		StarSoul[] starSous=getStarSoulByAll ();
		if (starSous == null)
			return count;
		StarSoulSample starSoulSample;
		StarSoul temp;
		for (int i=0; i<starSous.Length; i++) {
			temp=starSous[i];
			if(temp.partId!=partId) continue;
			count++;
		}
		return count;
	}

	/// <summary>
	/// 根据套装sid获取卡片身上此sid的套装的部件数量
	/// </summary>
	public int getSuitPartNumBySid (int suitSid)
	{
		int partNum = 0;
		if (equips == null)
			return 0;
		Equip tmp;
		for (int m=0; m<equips.Length; m++) {
			if (CardManagerment.Instance.showChatEquips != null && CardManagerment.Instance.showChatEquips.Count > 0) {
				tmp = CardManagerment.Instance.showChatEquips[m];
			}
			else {
				tmp = StorageManagerment.Instance.getEquip (equips [m]);
			}
			//Equip tmp = StorageManagerment.Instance.getEquip (equips [m]);
			if (tmp == null)
				continue;
			if(tmp.getSuitSid() == suitSid || tmp.getSuitSid() == suitSid / 100){
				partNum++;
			}
//
//			if(tmp.equpStarState ==0){
//				if (tmp.getSuitSid () != suitSid)
//					continue;
//			}else{
//				if (tmp.getSuitSid () != suitSid/100 )
//					continue;
//			}
//			partNum++;
		}
		return partNum;
	}
	
	public int getSellPrice ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).sell;
	}
	
	//进化到下个形态需要的价格
	public int getEvolvePrice ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).evolvePrice;
	} 
	//（原进化到下个形态的sid，现进化类型）
	public int getEvolveNextSid ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).evolveSid;
	} 
	//获得卡片类型 1card 0 beast
	public int getCardType ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).cardType;
	}
	
	//获得初始敏捷值
	public int getDex ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).baseAgile;
	}
	//获得初始魔法值
	public int getMag ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).baseMagic;
	}
	//获得初始防御值
	public int getDef ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).baseDefecse;
	}
	//获得初始攻击值
	public int getAtt ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).baseAttack;
	}
	//获得初始生命值
	public int getLife ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).baseLife;
	}
	
	/** 获得当前等级 */
	public int getLevel ()
	{
		return level;
	}

	/** 设置等级 */
	public void setLevel (int level)
	{
		this.level = level;
	}

	/** 改变经验值 */
	public void updateExp (long exp)
	{
		this.exp = exp;
		if (!isInherit) {
			updateLevel ();
		} else {
			level = EXPSampleManager.Instance.getLevel (getEXPSid (), exp);
			isInherit = false;
		}

	}

	/** 更新等级 */
	private void updateLevel ()
	{
        if (CardSampleManager.Instance.checkBlood(sid, uid) && BloodConfigManager.Instance.isQualityChanged(sid, cardBloodLevel)) {
            level = EXPSampleManager.Instance.getLevel(getEXPSid(), exp, level);
            return;
        }
		level = EXPSampleManager.Instance.getLevel (getEXPSid (), exp, level);

		if (isMaxLevel() && !IsShowInfo) {
			level = getMaxLevel();
		}
	}

	/** 获得经验值 */
	public long getEXP ()
	{
		return exp;
	}
	
	/** 获得名字 */
	public string getName ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).name /**+ " uid:" + uid + " sid:" + sid*/;
	}

	/** 获得星座女神标题图标 */
	public string getTitleName (int sid)
	{
		return CardSampleManager.Instance.getGoddessNameSprite (sid);
	}
	 
	/** 获得已开放技能最大槽位 */
	public int[]  getSkillMaxSlot ()
	{
//		return CardSampleManager.Instance.getRoleSampleBySid (sid).skillsNum;
		return new int[3]{buffSkillSlot,mainSkillSlot,attrSkillSlot};
	}

	/** 获得图片编号 */
	public int getImageID ()
	{
		if (UserManager.Instance.self == null) {
			return CardSampleManager.Instance.getRoleSampleBySid (this.sid).imageID;
		}
		if (isMainCard ())
			return SurmountManagerment.Instance.getImageSid (this);
		else
			return EvolutionManagerment.Instance.getImageSid (this);
	}
	/// <summary>
	/// 通过sid取得主角的头像
	/// </summary>
	public int getMainCardImageIDBysid(int sid)
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).imageID;
	}

	/** 获得普通卡进化后图片编号 */
	public int getImageID (int _evoLv)
	{
		Card newCard = CardManagerment.Instance.createCardByEvoLevel (this, _evoLv);
		return EvolutionManagerment.Instance.getImageSid (newCard);
	}

	/** 获得图标编号 */
	public int getIconID ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (this.sid).iconID;
	}

	/** 获得品质 */
	public int getQualityId ()
	{
	    if (isMainCard()) return SurmountManagerment.Instance.getQuitlyLevel(this);
        return CardSampleManager.Instance.getRoleSampleBySid(sid).qualityId;
        //return BloodConfigManager.Instance.getCurrentBloodQuality(sid, cardBloodLevel);
			
	}

	/// <summary>
	/// 计算卡片回收后的奖品
	/// </summary>
	public List<PrizeSample> computeRestoreCardPrize ()
	{
		List<PrizeSample> prizeList = new List<PrizeSample> ();
		int cardEvoLevel = getEvoTimes ();
		if (cardEvoLevel == 0)
			return prizeList;
		PrizeSample prize;
		EvolutionSample info = EvolutionManagerment.Instance.getEvoInfoByType (this);
		if (info == null) {
			return prizeList;
		}
		long upMoney = info.getNeedMoney () [cardEvoLevel - 1];
		// 游戏币奖品
		if (upMoney > 0) {
			prize = new PrizeSample (PrizeType.PRIZE_MONEY, 0, upMoney);
			prizeList.Add (prize);
		}
		int debrisPrizeNum = 0;
		int debrisPrizeSid = 0;
		int cardQualityId = getQualityId ();
		if (cardQualityId == QualityType.GOOD) {
			debrisPrizeNum = cardEvoLevel * 1;
			debrisPrizeSid = PropManagerment.PROP_PRIPLE_DEBRIS_SID;
		} else if (cardQualityId == QualityType.EPIC) {
			debrisPrizeNum = cardEvoLevel * 10;
			debrisPrizeSid = PropManagerment.PROP_PRIPLE_DEBRIS_SID;
		} else if (cardQualityId == QualityType.LEGEND) {
			debrisPrizeNum = cardEvoLevel * 10;
			debrisPrizeSid = PropManagerment.PROP_PRIPLE_ORANGE_SID;
		}
		// 碎片奖励
		if (debrisPrizeNum > 0 && debrisPrizeSid != 0) {
			prize = new  PrizeSample (PrizeType.PRIZE_PROP, debrisPrizeSid, debrisPrizeNum);
			prizeList.Add (prize);
		}
		return prizeList;	
	}
	
	/** 是否达到满级 */
	public bool isMaxLevel ()
	{
		if (getLevel () >= getMaxLevel ())
			return true;
		else
			return false;
	}

	/** 是否Boss */
	public bool isBoss ()
	{ 
		if (CardSampleManager.Instance.getRoleSampleBySid (sid).cardType == 3)
			return true;
		
		return false;
		
	}

	/** 是否召唤兽 */
	public bool isBeast ()
	{
		if (CardSampleManager.Instance.getRoleSampleBySid (sid).cardType == 2)
			return true;
		return false;
	}
	
	/** 是否达到满级 返回int类型 */
	public int isMaxLevelToInt ()
	{
		if (isMaxLevel ())
			return 1;
		else
			return 0;
	}

	//获得星级
	public int getStarLevel ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).starLevel;
	}
	//获得已进化星级
	public int getEvoStarLevel ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).evoStarLevel;
	}

	//可进化总星级
	public int getAllStarLevel ()
	{
		return getEvoStarLevel () + getStarLevel ();
	}	

	//获得职业
	public int getJob ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).job;
	}

	public int getEatenExp ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).eatenExp;
	}
	//获得战力值
	public int getPower ()
	{
		return 1;
	}
	/// <summary>
	/// 是否在队伍中
	/// </summary>
	public bool isInTeam ()
	{
		if (this.uid == "-1" && (this.state & CardStateType.STATE_USING) == 1) {
			return true;
		} else {
			return ArmyManager.Instance.getAllArmyPlayersIds ().Contains (this.uid) || ArmyManager.Instance.getAllArmyAlternateIds ().Contains (this.uid);
		}
	}

	//脱下身上所有装备
	public void putOffEquip ()
	{
		if (equips == null || equips.Length < 1)
			return;
		for (int i = 0; i < equips.Length; i++)
			putOffEquip (equips [i]);
	}
	//脱下身上指定uid装备
	public void putOffEquip (string uid)
	{
		Equip equip = StorageManagerment.Instance.getEquip (uid);
		if (equip == null)
			return;
		if (!equip.checkState (EquipStateType.OCCUPY))
			return;
		equip.state -= EquipStateType.OCCUPY;
	}
	
	//穿装备
	public List<AttrChange> putOnEquip (string equipId)
	{
		CardBaseAttribute attrOld = CardManagerment.Instance.getCardWholeAttr (this);
		
		Equip equip = StorageManagerment.Instance.getEquip (equipId);
		equip.state += EquipStateType.OCCUPY;
		if (equips == null) {
			string[] temp = new string[1];
			temp [0] = equipId;
			equips = temp;
		} else {
			string[] temps = new string[equips.Length + 1];
			Array.Copy (getEquips (), temps, getEquips ().Length);
			temps [temps.Length - 1] = equipId;
			equips = temps;
		}
		CardBaseAttribute attrNew = CardManagerment.Instance.getCardWholeAttr (this);
		return getAttr (attrOld, attrNew);
	}
	
	private List<AttrChange> getAttr (CardBaseAttribute cardOld, CardBaseAttribute cardNew)
	{
		List<AttrChange> attrs = new List<AttrChange> ();
		if (cardNew.hp - cardOld.hp != 0) {
			attrs.Add (new AttrChange (AttrChangeType.HP, cardNew.hp - cardOld.hp));
			
		}
		if (cardNew.attack - cardOld.attack != 0) {
			attrs.Add (new AttrChange (AttrChangeType.ATTACK, cardNew.attack - cardOld.attack));
		}
		if (cardNew.defecse - cardOld.defecse != 0) {
			attrs.Add (new AttrChange (AttrChangeType.DEFENSE, cardNew.defecse - cardOld.defecse));
		}
		if (cardNew.magic - cardOld.magic != 0) {
			attrs.Add (new AttrChange (AttrChangeType.MAGIC, cardNew.magic - cardOld.magic));
		}
		if (cardNew.agile - cardOld.agile != 0) {
			attrs.Add (new AttrChange (AttrChangeType.AGILE, cardNew.agile - cardOld.agile));
		}
		return attrs;
	}
	//操作装备
	public List<AttrChange> operateEquip (string equipId, string lastequipId)
	{
		if (lastequipId == "" || lastequipId == "0")
			return putOnEquip (equipId);
		else
			return getOffEquip (equipId, lastequipId);
	}
	
	public List<AttrChange> oneKeyEquip (string[] array)
	{
		CardBaseAttribute attrOld = CardManagerment.Instance.getCardWholeAttr (this);
		string[] temps = null;
		if (equips != null) {
			temps = new string[equips.Length];
			Array.Copy (getEquips (), temps, getEquips ().Length);
		}
		equips = null;
		equips = new string[array.Length];
		Array.Copy (array, equips, array.Length);
		
		for (int i = 0; i < equips.Length; i++) { 
			Equip equip = StorageManagerment.Instance.getEquip (equips [i]);
			if ((equip.state & EquipStateType.OCCUPY) != 1)
				equip.state += EquipStateType.OCCUPY;
		}
		
		if (temps != null) {
			for (int i = 0; i < temps.Length; i++) {
				for (int j = 0; j < array.Length; j++) {
					if (temps [i] == array [j]) {
						break;
					}
					if (j == array.Length - 1) {
						Equip equip = StorageManagerment.Instance.getEquip (temps [i]);
						equip.state -= EquipStateType.OCCUPY;
					} 
				}
			}
		}
		CardBaseAttribute attrNew = CardManagerment.Instance.getCardWholeAttr (this);
		return getAttr (attrOld, attrNew);
	}
	
	//脱装备和替换装备
	public List<AttrChange> getOffEquip (string equipId, string lastequipId)
	{
		//脱装备
		if (equipId == "0") {
			CardBaseAttribute attrOld = CardManagerment.Instance.getCardWholeAttr (this);
			Equip equip = StorageManagerment.Instance.getEquip (lastequipId);
			equip.state -= EquipStateType.OCCUPY;
			string[] temp = new string[equips.Length - 1];
			int count = 0;
			for (int i = 0; i < equips.Length; i++) {
				if (equips [i] != lastequipId) {
					temp [count] = equips [i];
					count ++;
				}
			}
			equips = temp;
			CardBaseAttribute attrNew = CardManagerment.Instance.getCardWholeAttr (this);
			return getAttr (attrOld, attrNew);
		}
		//替换装备
		else {
			CardBaseAttribute attrOld = CardManagerment.Instance.getCardWholeAttr (this);
			for (int i = 0; i < equips.Length; i++) {
				if (equips [i] == lastequipId) {
					equips [i] = equipId;
					Equip equip = StorageManagerment.Instance.getEquip (lastequipId);
					equip.state -= EquipStateType.OCCUPY;
					Equip equipl = StorageManagerment.Instance.getEquip (equipId);
					equipl.state += EquipStateType.OCCUPY;
				}
			}
			CardBaseAttribute attrNew = CardManagerment.Instance.getCardWholeAttr (this);
			return getAttr (attrOld, attrNew);
		}
	}
	
	/** 获得死亡宣言 */
	public string getDeadWords ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).deadWords;
	}
	
	/** 获得角色特效id */
	public int getEffectId ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).effectId;
	}
	
	/** 获得当前等级经验下限 */
	public long getEXPDown ()
	{
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), level);
	}

	/** 获得对应等级经验下限 */
	public long getEXPDown (int lv)
	{
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), lv);
	}

	public int getEXPSid ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).levelId;
	}
	
	/** 获得当前等级经验值上限 */
	public long getEXPUp ()
	{
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPUp (getEXPSid (), level);
	} 
	
	/** 获得当前等级经验值上限 */
	public long getEXPUp (int lv)
	{
		return EXPSampleManager.Instance.getEXPUp (getEXPSid (), lv);
	}

    /** 获取当前等级到下一级还需要的经验*/
    public long getNeedExp()
    {
        return getEXPUp() - getEXPDown();
    }

	/** 特性 */
	public string[]  getFeatures ()
	{
		return CardSampleManager.Instance.getRoleSampleBySid (sid).features;
	}
	/** 获得经验对于的卡片附加等级 */
	public static int getAttrAddGrade (long exp)
	{
		if (exp >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
			return EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
		}
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP, exp) - 1;
	}
	/** 获得卡片生命附加等级 */
	public int getHPGrade ()
	{
		if (getHPExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
			return EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
		}
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP, getHPExp ()) - 1;
	}
	/** 获得卡片生命附加经验 */
	public int getHPExp ()
	{
		return this.hpExp;
	}
	/** 改变卡片生命附加经验 */
	public void updateHPExp (int _exp)
	{
		this.hpExp = _exp;
	}
	/** 获得卡片攻击附加等级 */
	public int getATTGrade ()
	{
		if (getATTExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
			return EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
		}
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP, getATTExp ()) - 1;
	}
	/** 获得卡片攻击附加经验 */
	public int getATTExp ()
	{
		return this.attExp;
	}
	/** 改变卡片攻击附加经验 */
	public void updateATTExp (int _exp)
	{
		this.attExp = _exp;
	}
	/** 获得卡片防御附加等级 */
	public int getDEFGrade ()
	{
		if (getDEFExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
			return EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
		}
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP, getDEFExp ()) - 1;
	}
	/** 获得卡片防御附加经验 */
	public int getDEFExp ()
	{
		return this.defExp;
	}
	/** 改变卡片防御附加经验 */
	public void updateDEFExp (int _exp)
	{
		this.defExp = _exp;
	}
	/** 获得卡片魔力附加等级 */
	public int getMAGICGrade ()
	{
		if (getMAGICExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
			return EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
		}
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP, getMAGICExp ()) - 1;
	}
	/** 获得卡片魔力附加经验 */
	public int getMAGICExp ()
	{
		return this.magicExp;
	}
	/** 改变卡片魔力附加经验 */
	public void updateMAGICExp (int _exp)
	{
		this.magicExp = _exp;
	}
	/** 获得卡片敏捷附加等级 */
	public int getAGILEGrade ()
	{
		if (getAGILEExp () >= EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
			return EXPSampleManager.Instance.getMaxLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
		}
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_ATTR_ADD_EXP, getAGILEExp ()) - 1;
	}
	/** 获得卡片敏捷附加经验 */
	public int getAGILEExp ()
	{
		return this.agileExp;
	}
	/** 改变卡片敏捷附加经验 */
	public void updateAGILEExp (int _exp)
	{
		this.agileExp = _exp;
	}
	/** 获得主动技能 */
	public Skill[] getSkills ()
	{
		return this.skills;
	}

	/** 是否已经无法献祭 */
	public bool isCanSacrific ()
	{
		if (isSkillLvUpFull ())
			return isMaxLevel ();
		else
			return false;
	}

	//判断技能是否升级满了
	public bool isSkillLvUpFull ()
	{
		if (skills != null) {
			for (int i = 0; i < skills.Length; i++) {
				if (skills [i].getLevel () < Math.Min (skills [i].getMaxLevel (), getLevel () + 5)) {
					return false;
				}
			}
		}
		
		if (buffskills != null) {
			for (int i = 0; i < buffskills.Length; i++) {
				if (buffskills [i].getLevel () < Math.Min (buffskills [i].getMaxLevel (), getLevel () + 5)) {
					return false;
				}
			}
		}
		if (attrskills != null) {
			for (int i = 0; i < attrskills.Length; i++) {
				if (attrskills [i].getLevel () < Math.Min (attrskills [i].getMaxLevel (), getLevel () + 5)) {
					return false;
				}
			}
		}
		return true;
	}

	//判断技能是否有加满了
	public bool isSkillExpUpFull (int addExp)
	{
		if (skills != null) {
			for (int i = 0; i < skills.Length; i++) {
				if (skills [i].getEXP () + addExp > skills [i].getEXPDown (Math.Min (skills [i].getMaxLevel (), getLevel () + 5))) {
					return true;
				}
			}
		}
		if (buffskills != null) {
			if (!this.isMainCard ()) {
				for (int i = 0; i < buffskills.Length; i++) {
					if (buffskills [i].getEXP () + addExp > buffskills [i].getEXPDown (Math.Min (buffskills [i].getMaxLevel (), getLevel () + 5))) {
						return true;
					}
				}
			}
		}
		if (attrskills != null) {
			for (int i = 0; i < attrskills.Length; i++) {
				if (attrskills [i].getEXP () + addExp < attrskills [i].getEXPDown (Math.Min (attrskills [i].getMaxLevel (), getLevel () + 5))) {
					return true;
				}
			}
		}
		return false;
	}
	
	/** 获得开场buff技能 */
	public Skill[] getBuffSkills ()
	{
		return this.buffskills;
	}
	
	/** 获得被动技能 */
	public Skill[] getAttrSkills ()
	{
		return this.attrskills;
	}
    /**获得血脉激活的技能*/

	/** 获得技能经验 */
	public long getSkillsExp()
	{
		long skillsExp = 0;
		if (skills != null) {
			for (int i = 0; i < skills.Length; i++) {
				skillsExp += skills[i].getEXP();
			}
		}
		if (buffskills != null) {
			for (int i = 0; i < buffskills.Length; i++) {
				skillsExp += buffskills[i].getEXP();
			}
		}
		if (attrskills != null) {
			for (int i = 0; i < attrskills.Length; i++) {
				skillsExp += attrskills[i].getEXP();
			}
		}
		return skillsExp;
	}
	
	//copy模板默认技能信息
	public void copySkillsBySample ()
	{
		int[] buffSkillInts = CardSampleManager.Instance.getRoleSampleBySid (sid).buffSkills;
		int[] mainSkillInts = CardSampleManager.Instance.getRoleSampleBySid (sid).mainSkills;
		int[] attrSkillInts = CardSampleManager.Instance.getRoleSampleBySid (sid).attrSkills;
		
		if (buffSkillInts != null) {
			buffskills = new Skill[buffSkillInts.Length];
			for (int i = 0; i < buffSkillInts.Length; i++) {
				buffskills [i] = SkillManagerment.Instance.createSkill (buffSkillInts [i]);
				buffskills [i].setSkillStateType (SkillStateType.BUFF);
			}
			this.buffSkillSlot = buffskills.Length;
		}
		
		if (mainSkillInts != null) {
			skills = new Skill[mainSkillInts.Length];
			for (int i = 0; i < mainSkillInts.Length; i++) {
				skills [i] = SkillManagerment.Instance.createSkill (mainSkillInts [i]);
				skills [i].setSkillStateType (SkillStateType.ACTIVE);
			}
			this.mainSkillSlot = skills.Length;
		}
		
		if (attrSkillInts != null) {
			attrskills = new Skill[attrSkillInts.Length];
			for (int i = 0; i < attrSkillInts.Length; i++) {
				attrskills [i] = SkillManagerment.Instance.createSkill (attrSkillInts [i]);
				attrskills [i].setSkillStateType (SkillStateType.ATTR);
			}
			this.attrSkillSlot = attrskills.Length;
		} 
	}
	
	//copy模板默认附加属性信息
	public void copyAppendAttrBySample ()
	{ 
		hpExp = CardSampleManager.Instance.getRoleSampleBySid (sid).attrLevel [0];//生命附加经验
		attExp = CardSampleManager.Instance.getRoleSampleBySid (sid).attrLevel [1];//攻击附加经验
		defExp = CardSampleManager.Instance.getRoleSampleBySid (sid).attrLevel [2];//防御附加经验
		magicExp = CardSampleManager.Instance.getRoleSampleBySid (sid).attrLevel [3];//魔力附加经验
		agileExp = CardSampleManager.Instance.getRoleSampleBySid (sid).attrLevel [4];//敏捷附加经验 
	}
	
	//获得卡片技能数量 主动被动开场
	public int getSkillNum ()
	{
		int num = 0;
		if (getSkills () != null)
			num += getSkills ().Length;
		if (getAttrSkills () != null)
			num += getAttrSkills ().Length;
		if (getBuffSkills () != null)
			num += getBuffSkills ().Length;
		return num;
	}

	//检测卡片状态 处于初始状态返回true
	public bool freeState ()
	{
		return state == 0;
	}
	//检测卡片状态 处于对应状态返回true
	public bool checkState (int _state)
	{
		if (state == _state)
			return true;
		else
			return (state & _state) > 0;
	}

	public void addState (int _state)
	{
		//若数据异常，可能出现这种状况
		if (checkState (_state))
			return;
		state += _state;
	}

	public void delState (int _state)
	{
		//若数据异常，可能出现这种状况
		if (!checkState (_state))
			return;
		state -= _state;
	}

	public override bool equal (StorageProp prop)
	{
		return this.uid == prop.uid;
	}

	long tempExp;
	public override void bytesRead (int j, ErlArray ea)
	{
		tempExp = 0;
		this.uid = ea.Value [j++].getValueString ();
		this.sid = StringKit.toInt (ea.Value [j++].getValueString ());
		tempExp = StringKit.toLong (ea.Value [j++].getValueString ()); 
		this.hpExp = StringKit.toInt (ea.Value [j++].getValueString ());
		this.attExp = StringKit.toInt (ea.Value [j++].getValueString ());
		this.magicExp = StringKit.toInt (ea.Value [j++].getValueString ());
		this.defExp = StringKit.toInt (ea.Value [j++].getValueString ());
		this.agileExp = StringKit.toInt (ea.Value [j++].getValueString ());
		this.attrskills = SkillManagerment.Instance.createSkills (ea.Value [j++] as ErlArray, SkillStateType.ATTR);
		this.buffskills = SkillManagerment.Instance.createSkills (ea.Value [j++] as ErlArray, SkillStateType.BUFF);
		this.skills = SkillManagerment.Instance.createSkills (ea.Value [j++] as ErlArray, SkillStateType.ACTIVE);
		this.equips = EquipManagerment.Instance.getEquipId (ea.Value [j++] as ErlArray);
		this.state = StringKit.toInt (ea.Value [j++].getValueString ());
		this.evoLevel = StringKit.toInt (ea.Value [j++].getValueString ());
		this.surLevel = StringKit.toInt (ea.Value [j++].getValueString ());
		this.mainSkillSlot = StringKit.toInt (ea.Value [j++].getValueString ());
		this.attrSkillSlot = StringKit.toInt (ea.Value [j++].getValueString ());
		this.buffSkillSlot = StringKit.toInt (ea.Value [j++].getValueString ());
		ErlArray erlArray=ea.Value [j++] as ErlArray;
		if (erlArray.Value.Length>0) {
			bores = new ArrayList ();
			StarSoulBore starSoulBore;
			for (int m = 0,count=erlArray.Value.Length; m < count; m++) {
				starSoulBore=new StarSoulBore();
				starSoulBore.bytesRead(0,erlArray.Value [m] as ErlArray);
				bores.Add(starSoulBore);
			}
		}
        this.magicWeaponUID = ea.Value[j++].getValueString();
        this.cardBloodLevel = StringKit.toInt(ea.Value[j++].getValueString());
        updateExp(tempExp); 
	}
	/***/
	public override void copy (object destObj)
	{
		base.copy (destObj);
		Card dest = destObj  as Card;
		if (this.equips != null) {
			dest.equips = new string[this.equips.Length];
			for (int i = 0; i < dest.equips.Length; i++)
				dest.equips [i] = this.equips [i];
		}
		if (this.skills != null) {
			dest.skills = new Skill[this.skills.Length];
			for (int i = 0; i < dest.skills.Length; i++)
				dest.skills [i] = this.skills [i].Clone () as Skill;
		}
		if (this.buffskills != null) {
			dest.buffskills = new Skill[this.buffskills.Length];
			for (int i = 0; i < dest.buffskills.Length; i++)
				dest.buffskills [i] = this.buffskills [i].Clone () as Skill;
		}
		if (this.attrskills != null) {
			dest.attrskills = new Skill[this.attrskills.Length];
			for (int i = 0; i < dest.attrskills.Length; i++)
				dest.attrskills [i] = this.attrskills [i].Clone () as Skill;
		}
	}
}  