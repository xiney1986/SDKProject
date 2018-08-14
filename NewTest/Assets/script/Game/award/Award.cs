using System;
using System.Collections.Generic;
 
/**
 * 奖励对象
 * @author longlingquan
 * */
public class Award
{
	/**  忽略奖励type合并奖励数据 */
	public static Award mergeAward (params Award[] awards)
	{
		Award award = null;
		Award aw;
		for (int i = 0; i < awards.Length; i++) {
			aw = awards [i];
			if (aw == null)
				continue;
			if (award == null) {
				award = new Award ();
				award.resetBaseData ();
			}
			award.copyAward (aw);
		}
		return award;
	}

	public Award ()
	{ 
		exps = new List<EXPAward> ();
		props = new List<PropAward> ();
		equips = new List<EquipAward> ();
		starsouls = new List<StarSoulAward> ();
		cards = new List<CardAward> ();
        magicWeapons = new List<MagicwWeaponAward>();
	}
	
	public List<EXPAward> exps;//卡片经验值奖励 
	public List<PropAward> props;//道具奖励
	public List<EquipAward> equips;//装备奖励
	public List<StarSoulAward> starsouls;//星魂奖励
	public List<CardAward> cards;//卡片奖励
    public List<MagicwWeaponAward> magicWeapons;//神器奖励

	public EXPAward getExpAwardByCardID (string id)
	{

		foreach (EXPAward each in exps) {
			if (each.id == id)
				return each;
		}
		return null;
	}

	public LevelupInfo playerLevelUpInfo;
	//玩家总经验值
	long exp = -1;

	public long awardExp {
		get {
			return exp;
		}
		set {
			if (value == -1) {
				exp = -1;
				expGap = 0;
			} else {

				exp = value;
				expGap = (int)(value - UserManager.Instance.self.getEXP ());
				playerLevelUpInfo = new LevelupInfo ();
				playerLevelUpInfo.oldExp = UserManager.Instance.self.getEXP () - expGap;
				playerLevelUpInfo.oldLevel = UserManager.Instance.self.getUserLevel ();
				playerLevelUpInfo.oldExpUp = UserManager.Instance.self.getEXPUp ();
				playerLevelUpInfo.oldExpDown = UserManager.Instance.self.getEXPDown ();
				playerLevelUpInfo.oldCardCombat = ArmyManager.Instance.getTeamCombat (ArmyManager.Instance.getActiveArmy ().armyid);
				UserManager.Instance.self.updateExp (value);
				playerLevelUpInfo.newLevel = UserManager.Instance.self.getUserLevel ();
				playerLevelUpInfo.newExp = UserManager.Instance.self.getEXP ();
				playerLevelUpInfo.newExpUp = UserManager.Instance.self.getEXPUp ();
				playerLevelUpInfo.newExpDown = UserManager.Instance.self.getEXPDown ();
                if (playerLevelUpInfo.newLevel > playerLevelUpInfo.oldLevel)
                {
                    SdkManager.INSTANCE.SetData("levelup", UserManager.Instance.self.ToDic());
                    SdkManager.INSTANCE.UpdateRole();
                }
            }
		}
	}

	public int expGap = -1;//奖励后和奖励前玩家经验的差值

	//星屑
	int star = -1;

	public int awardStar {
		get {
			return star;
		}
		set {
			if (value == -1) {
				star = -1;
				starGap = 0;
			} else {
				star = value;
				starGap = value - GoddessAstrolabeManagerment.Instance.getStarScore ();
				GoddessAstrolabeManagerment.Instance.setStarScore (value);
			}
		}
	}

	public int starGap = -1;//奖励后和奖励前玩家money的差值

	//金币总量
	int money = -1;

	public int awardMoney {
		get {
			return money;
		}
		set {
			if (value == -1) {
				money = -1;
				moneyGap = 0;
			} else {
				money = value;
				moneyGap = value - UserManager.Instance.self.getMoney ();
				UserManager.Instance.self.updateMoney (value);
			}
		}
	}

	public int moneyGap = -1;//奖励后和奖励前玩家money的差值

	//rmb总量
	int rmb = -1;

	public int awardRmb {
		get {
			return rmb;
		}
		set {
			if (value == -1) {
				rmb = -1;
				rmbGap = 0;
			} else {
				rmb = value;
				rmbGap = value - UserManager.Instance.self.getRMB ();
				UserManager.Instance.self.updateRMB (value);
			}
		}
	}

	public int rmbGap = -1;//奖励后和奖励前玩家rmb的差值

	//荣誉总量
	int honor = -1;

	public int awardHonor {
		get {
			return honor;
		}
		set {
			if (value == -1) {
				honor = -1;
				honorGap = 0;
			} else {
				honor = value;
				honorGap = value - UserManager.Instance.self.honor;
				UserManager.Instance.self.honor = value;
			}
		}
	}

	public int honorGap = -1;//奖励后和奖励前玩家荣誉的差值

	//积分总量
	int integral = -1;

	public int awardIntegral {
		get{ return integral;}
		set {
			if (value == -1) {
				integral = -1;
				integralGap = 0;
			} else {
				integral = value;
				integralGap = value - ArenaManager.instance.self.integral;
				ArenaManager.instance.self.integral = value;
			}
		}
	}

	//积分总量
	int godsWar_integral = -1;
	
	public int godsWar_awardIntegral {
		get{ return godsWar_integral;}
		set {
			if (value == -1) {
				godsWar_integral = -1;
				godsWar_integralGap = 0;
			} else {
				godsWar_integral = value;
				godsWar_integralGap = value - GodsWarManagerment.Instance.self.todayIntegral;
				GodsWarManagerment.Instance.self.totalIntegral = value;
				GodsWarManagerment.Instance.self.todayIntegral += godsWar_integralGap;
			}
		}
	}
	public int godsWar_integralGap = -1;
	public int integralGap = -1;

	//积分总量
	int merit = -1;

	public int awardMerit {
		get{ return merit;}
		set {
			if (value == -1) {
				merit = -1;
				meritGap = 0;
			} else {
				merit = value;
				meritGap = value - UserManager.Instance.self.merit;
				UserManager.Instance.self.merit = value;
			}
		}
	}

	public int meritGap = -1;

	//声望
	int prestige = -1;
	public int awardPrestige
	{
		get{return prestige;}
		set{
			if(value == -1)
			{
				prestige =-1;
				prestigeGap = 0;
			}else
			{
				prestige = value;
				prestigeGap = value -UserManager.Instance.self.prestige;
				UserManager.Instance.self.prestige = value;
			}
		}
	}
	public int prestigeGap = -1;

	//星魂碎片
	int starsoulDebris = -1;
	public int awardStarsoulDebris
	{
		get{return starsoulDebris;}
		set{
			if(value == -1) {
				starsoulDebris =-1;
				starsoulDebrisGap = 0;
			} else {
				StarSoulManager manager=StarSoulManager.Instance;
				starsoulDebris = value;
				starsoulDebrisGap = value -manager.getDebrisNumber();
				manager.setDebrisNumber(value);
			}
		}
	}
	public int starsoulDebrisGap = -1;

	//贡献总量
	int contribution = -1;

	public int awardCon {
		get {
			return contribution;
		}
		set {
			if (value == -1) {
				contribution = -1;
				contributionGap = 0;
			} else {
				contribution = value + GuildManagerment.Instance.getGuild ().contributioning;
				contributionGap = value;
				GuildManagerment.Instance.getGuild ().contributioning = contribution;
				GuildManagerment.Instance.getGuild ().contributioned += value; 
			}
		}
	}

	public int contributionGap = -1;//奖励后和奖励前玩家贡献的差值

	//活跃总量
	int active = -1;

	public int awardActive {
		get {
			return active;
		}
		set {
			if (value == -1) {
				active = -1;
				activeGap = 0;
			} else {
				active = value + GuildManagerment.Instance.getGuild ().livenessing;
				activeGap = value;
				GuildManagerment.Instance.getGuild ().livenessing = active;
				GuildManagerment.Instance.getGuild ().livenessed += value; 
			}
		}
	}

	public int activeGap = -1;//奖励后和奖励前玩家活跃的差值


	//活跃总量
	int luckyStar = -1;
	
	public int awardLuckyStar {
		get {
			return luckyStar;
		}
		set {
			if (value == -1) {
				luckyStar = -1;
				activeGap = 0;
			} else {
				luckyStar = value;
				luckyStarGap = value - UserManager.Instance.self.starSum;
				UserManager.Instance.self.lastStarSum = value;
			}
		}
	}
	
	public int luckyStarGap = -1;//奖励后和奖励前玩家活跃的差值

	public string type;//奖励类型
	
	public bool isNull ()
	{
		if (exp > 0 || money > 0 || rmb > 0 || exps.Count > 0 || props.Count > 0 || equips.Count > 0|| starsouls.Count > 0 || cards.Count > 0)
			return false;
		return true;
	}

	public void removeStarsouls() {

	}


	public void copyAward (Award award)
	{
		if (award.exps != null) {
			if (exps == null)
				exps = new List<EXPAward> ();
			ListKit.AddRange (exps, award.exps);
		}
		if (award.props != null) {
			if (props == null)
				props = new List<PropAward> ();
			ListKit.AddRange (props, award.props);
		}
		if (award.equips != null) {
			if (equips == null)
				equips = new List<EquipAward> ();
			ListKit.AddRange (equips, award.equips);
		}
		if (award.starsouls != null) {
			if (starsouls == null)
				starsouls = new List<StarSoulAward> ();
			ListKit.AddRange (starsouls, award.starsouls);
		}
		if (award.cards != null) {
			if (cards == null)
				cards = new List<CardAward> ();
			ListKit.AddRange (cards, award.cards);
		}
        if (award.magicWeapons != null) {
            if (magicWeapons == null) {
                magicWeapons = new List<MagicwWeaponAward>();
            }
            ListKit.AddRange(magicWeapons, award.magicWeapons);
        }
		if (award.rmbGap > 0)
			rmbGap += award.rmbGap;
		if (award.starsoulDebrisGap > 0)
			starsoulDebrisGap += award.starsoulDebrisGap;
		if (award.expGap > 0)
			expGap += award.expGap;
		if (award.moneyGap > 0)
			moneyGap += award.moneyGap;
		if (award.honorGap > 0)
			honorGap += award.honorGap;
		if (award.integralGap > 0)
			integralGap += award.integralGap;
		if (award.meritGap > 0)
			meritGap += award.meritGap;
		if (award.starGap > 0)
			starGap += award.starGap;
		if (award.luckyStarGap > 0)
			luckyStarGap += award.luckyStarGap;
		if(award.godsWar_integralGap>0)
			godsWar_integral+=award.godsWar_integralGap;

	}

	public void resetBaseData ()
	{
		rmbGap = 0;
		starsoulDebrisGap = 0;
		expGap = 0;
		moneyGap = 0;
		honorGap = 0;
		integralGap = 0;
		godsWar_integral=0;
		meritGap = 0;
		starGap = 0;
		luckyStarGap = 0;
	}
} 

/**
 * 经验值奖励
 * */
public class EXPAward
{
	public EXPAward ()
	{
		
	}
	
	public string id = "";//唯一id 
	long exp = -1;//战斗后总经验值 
	public CardLevelUpData cardLevelUpData;

	public long awardExp {
		get {
			return exp;
		}
		set {
			if (value == -1) {
				exp = -1;
				expGap = 0;
			} else {
				exp = value;
				Card card = StorageManagerment.Instance.getRole (id);
				if (card == null)
					card = StorageManagerment.Instance.getBeast (id);

				if (card == null)
					return;
	
				expGap =Math.Max(0, (int)(value - card.getEXP ()));

//				UnityEngine.Debug.LogError("uid:"+card.uid + ",name=" + card.getName ()+ ",server: "+value+",cardExp:" + card.getEXP () +",expGap:   "+expGap);
				cardLevelUpData = new CardLevelUpData ();
				cardLevelUpData.oldAttr = CardManagerment.Instance.getCardAllWholeAttr (card);
				cardLevelUpData.levelInfo = new LevelupInfo ();
				cardLevelUpData.levelInfo.orgData = card;
				cardLevelUpData.levelInfo.oldExp = card.getEXP () - expGap;
				cardLevelUpData.levelInfo.oldExpDown = card.getEXPDown ();
				cardLevelUpData.levelInfo.oldExpUp = card.getEXPUp ();
				cardLevelUpData.levelInfo.oldLevel = card.getLevel ();
				cardLevelUpData.levelInfo.oldCardCombat = card.getCardCombat ();

				card.updateExp (value);

				//升级后的新属性
				cardLevelUpData.newAttr = CardManagerment.Instance.getCardAllWholeAttr (card);
				cardLevelUpData.levelInfo.newExp = card.getEXP ();
				cardLevelUpData.levelInfo.newExpDown = card.getEXPDown ();
				cardLevelUpData.levelInfo.newExpUp = card.getEXPUp ();
				cardLevelUpData.levelInfo.newLevel = card.getLevel ();
				cardLevelUpData.levelInfo.newCardCombat = card.getCardCombat ();
			}
		}
	}

	public int expGap = -1;//奖励后和奖励前玩家经验的差值

	public void bytesWrite (EXPAward aw)
	{
		this.id = aw.id;
		this.exp = aw.exp;
	}
}

/**
 * 道具奖励
 * */
public class PropAward
{
	public PropAward ()
	{
	}

	public int sid = 0;//sid
	public int num = 0;//数量
	public void bytesWrite (PropAward aw)
	{
		this.sid = aw.sid;
		this.num = aw.num;
	}
}

/**
 * 装备奖励
 * */
public class EquipAward
{
	public EquipAward ()
	{
	}

	public string id = "";//唯一id
	public int sid = 0;//sid
	public int num = 0;
	
	public void bytesWrite (EquipAward aw)
	{
		this.id = aw.id;
		this.sid = aw.sid;
	}
}
/// <summary>
/// 神器奖励
/// </summary>
public class MagicwWeaponAward
{
	public MagicwWeaponAward ()
	{
	}

	public string id = "";//唯一id
	public int sid = 0;//sid
	public int num = 0;
	
	public void bytesWrite (MagicwWeaponAward aw)
	{
		this.id = aw.id;
		this.sid = aw.sid;
	}
}

/**
 * 星魂奖励
 * */
public class StarSoulAward
{
	public StarSoulAward ()
	{
	}

	/** 唯一id */
	public string uid = "";
	/** sid */
	public int sid = 0;
	
	public void bytesWrite (StarSoulAward aw)
	{
		this.uid = aw.uid;
		this.sid = aw.sid;
	}
}


/**
 * 卡片奖励
 * */
public class CardAward
{
	public CardAward ()
	{
		
	}

	public string id = "";//唯一id
	public int sid = 0;//sid
	public int num = 0;
	
	public void bytesWrite (CardAward aw)
	{
		this.id = aw.id;
		this.sid = aw.sid;
	}
}


