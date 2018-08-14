using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleDataErlang
{ 

	public object empty;
	public List<BattleClipErlang> battleClip; // 战斗回合数据列表
	public TeamInfo playerTeamInfo; // 己方队伍信息
	public TeamInfo enemyTeamInfo; // 敌方队伍信息
	public bool isStart = false;//是否有开场技
	public int winnerID = 0; // 胜利标识:OWN_CAMP=1[己方],ENEMY_CAMP=2[敌方]
	public int battleType = 0; // 战斗类型:BATTLE_TEN=1[十人战],BATTLE_FIVE=2[五人战],BATTLE_SUBSTITUTE=3[替补战]
	public bool isPvP = false; // 是否pvp战斗
    public bool isPve = false;
	public bool isArenaMass = false; //是否海选
	public bool isArenaFinal = false; //是否决赛
	public bool isGuide; //是否新手
	public bool isPractice = false;//是否是修炼 PRACTICE
	public bool isGuildFight = false; //是否为公会战
	public bool isMineralFight = false;//宝藏战
	public bool isMineralFightRecord = false;
	public bool isGodsWarGroupFight = false;//神战小组战斗
	public bool isGodsWarFinal=false;//神战淘汰赛
	public bool isGodsWarGroupAward = false;
	public int enemyFormationID = 0; // 敌方阵型id
	public int playerFormationID = 0; // 本方阵型id
	public int pvpType = 0;//pvp战斗类型:1=普通战斗,2=全力一击
	public int enemyBeastEffect = 0;//上方召唤兽共鸣加成
	public int playerBeastEffect = 0;//下方召唤兽共鸣加成
	public string replayAttackerName = ""; //录像战斗攻击者
	public string replayEnemyName = ""; //录像战斗迎战着
	public Dictionary<int,BattleHpInfo> hpMap; // hp信息[卡片uid,hp信息对象]
	public bool isGuildBoss = false;//是否是工会boss
	public Dictionary<string,int> evo;//卡片进化等级<卡片Uid,进化等级>,用的时候看情况判断null
	public int pveTeamNum = 0;//pve阵形人数,通过这个确定采用哪个阵形prefab;
	public bool isLadders = false;//是否天梯
	public bool isLaddersRecord = false;//是否是天梯重播
	public bool isGMFight;//是否是GM工具战斗
    public bool isOneOnOneBossFight = false;//是否是恶魔挑战
	public bool isLastBattle = false;// 是否是末日决战//
	public bool isLastBattleBossBattle = false;// 末日决战boss战//
	public bool isHeroRoad = false;

	public BattleDataErlang ()
	{
		battleClip = new List<BattleClipErlang> (); 
	}
	//根据Uid获得卡片进化等级
	public int getCardEvoLevel (string cardUid)
	{
		if (evo == null || cardUid == "" || cardUid == "0" || !evo.ContainsKey (cardUid)) 
			return 0;
		return evo [cardUid];
	}
}
//每回合
public class BattleClipErlang
{ 
	public	List<BattleInfoErlang> battleInfo;
	public bool isWinnerClip = false; // 是否为战斗结束回合数据
	public int frame = 0; //默认为0 不计入回合
	public	 BattleClipErlang ()
	{ 
		battleInfo = new List<BattleInfoErlang> (); 
	} 
}
 
// 战斗片段信息数据
public class BattleInfoErlang
{ 
	public 	List<BattleSkillErlang> battleSkill;

	public	 BattleInfoErlang ()
	{ 
		battleSkill = new List<BattleSkillErlang> (); 
	} 
}

// 战斗片段技能数据
public class  BattleSkillErlang
{
	public BattleSkillMsg skillMsg;

	public BattleSkillErlang ()
	{ 
		skillMsg = new BattleSkillMsg (); 
	}  
}

public class  BattleSkillMsg
{  
	public int[] targets;//目标对象 
	public string operationType;//操作类型 
	public int userID;//使用者 
	public int trigger;//触发者 
	public int skillSID;//技能sid
	public int skillID;//技能编号 

	public int damage;//伤害 
	public int valueType;//数字类型  "1";//属性 hp "2";//怒气 anger

	public int oldSkillSID;//旧技能sid
	public int oldSkillID;//旧技能
	public BuffEffectType[] effects;//buff影响效果类型
	
	public BuffAttrChange[] changes;//属性改变集合
	public TeamInfoPlayer card;//替补上阵的卡片信息

	public bool isLastAttack = false;//最后一击，默认false不是最后一击

	public int plotSID; //剧情对话sid
	public int exitEffectId; //退出战斗的特效id
	 
	public BattleSkillMsg ()
	{
		
	}
	
	public int  getValueByEffectType (string str)
	{
		if (effects == null)
			return 0;
		foreach (BuffEffectType each in effects) {
			if (each.type == str) {
				return each.effect;
			}
		}
		
		return 0; 
	}
	
}

//buff属性改变信息
public class BuffAttrChange
{ 
	public string operationType;//操作类型 
	public BattleAttrChange[] changes;
	public int skillSID;//技能sid
	public int skillID;//技能编号  
}

//属性改变
public class BattleAttrChange
{ 
	public int damageType;//伤害类型 
	public int damage;//伤害 
}

public class BuffEffectType
{
	public const string ATTACK = "physics_attack";//物理攻击
	public const string DEFENSE = "physics_defense";//物理防御
	public const string MAGIC = "magic";//魔力
	public const string AGILE = "agile";//敏捷
	public const string SILENCE = "silence";//沉默
	public const string DEDUCK = "deduck";//毒
	
	
	public string type = "";
	public int effect = 0;
}

