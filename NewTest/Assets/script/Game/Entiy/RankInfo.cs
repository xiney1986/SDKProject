using UnityEngine;
using System.Collections;

/**
 * 排名信息实体类
 * @author 汤琦
 * */
public class RankInfo 
{
	public RankInfo(int lastWinNum,int rank,int maxNum)
	{
		this.lastWinNum = lastWinNum;
		this.rank = rank;
		this.maxNum = maxNum;
	}
	public int lastWinNum = 0;
	public int rank = 0;
	public int maxNum = 0;
}
public class RankItem
{
	public RankItem(string uid,string name,int headIcon,long exp,int winNum,int formation,int beastSid,long beastExp,RankItemTeam[] opps,int _combat)
	{

		this.uid = uid;
		this.name = name;
		this.headIcon = headIcon;
		this.exp = exp;
		this.winNum = winNum;
		this.formation = formation;
		this.beastSid = beastSid;
		this.beastExp = beastExp;
		this.opps = opps;
		this.combat = _combat;
	}
	public string uid;//uid
	//public int integral = 0;//积分
	public string name;//名字
	public int headIcon = 0;//头像图标sid
	public long exp = 0;//经验值
	public int winNum = 0;//连胜次数
	//public int nameIcon = 0;//称号图标sid
	//public int vipLv = 0;//VIP等级
	public int formation = 0;//阵型
	public int beastSid =0;
	public long beastExp =0;
	public RankItemTeam[] opps;//主力sid和经验
	public int combat;//战力
}

public class RankItemTeam
{
	public RankItemTeam(int sid,long exp,int index)
	{
		this.sid = sid;
		this.exp = exp;
		this.index = index;
	}
	public int sid = 0;
	public long exp = 0;
	public int index = 0;
}

public class RankItemCombat
{
	public string uid;
	public string name;
    public int combat;
    public int vipLevel;
}

public class RankItemGuild
{
	public string gid;
	public string name;
    public int score;
    public int vipLevel;
}

public class RankItemGuildFight{
	/** 公会uid */
	public string uid;
	/** 名称 */
	public string name;
	/** 评分 */
	public int judgeScore;

	public string getJudgeString(){
		return GuildFightSampleManager.Instance ().getJudgeString (judgeScore);
	}
}

public class RankItemRoleLv
{
	public string uid;
	public string name;
	public int lv;
	public int vipLevel;
}

public class RankItemTotalDamage {
    public string uid;
    public string name;
    public string damage;
    public int vipLevel;
}

public class RankItemPVP
{
	public string uid;
	public string name;
    public int win;
    public int vipLevel;
}

public class RankItemLuckyCard
{
	public string uid;
	public string name;
	public int integral;
	public int vipLevel;
	public int rank;

	/** 序列化读取 */
	public void bytesRead (int rankIndex,int j, ErlArray ea) {
        this.rank = rankIndex;
        string serverName = ea.Value[j++].getValueString();
		this.uid = ea.Value [j++].getValueString ();
        //this.rank = StringKit.toInt(ea.Value[j++].getValueString());
        string name1 = ea.Value[j++].getValueString();
        this.name = "(" + serverName + ")" + name1;
		this.vipLevel = StringKit.toInt (ea.Value [j++].getValueString ());
		this.integral = StringKit.toInt (ea.Value [j++].getValueString ());
	}
	/** 序列化读取 */
	public void bytesReadLuo (int rankIndex,int j, ErlArray ea) {
		this.rank = rankIndex;
		//string serverName = ea.Value[j++].getValueString();
		this.uid = ea.Value [j++].getValueString ();
		//this.rank = StringKit.toInt(ea.Value[j++].getValueString());
		this.name = ea.Value[j++].getValueString();
		this.integral = StringKit.toInt (ea.Value [j++].getValueString ());
		this.vipLevel = StringKit.toInt (ea.Value [j++].getValueString ());
	}
}
public class RankItemLuckyLiehun
{
	public string uid;
	public string name;
	public int integral;
	public int vipLevel;
	public int rank;
	
	/** 序列化读取 */
	public void bytesRead (int j, ErlArray ea,int _rank) {
		if(ea.Value.Length==4){
			this.uid = ea.Value [j++].getValueString ();
			this.rank = _rank;
			this.name = ea.Value[j++].getValueString();
			this.vipLevel = StringKit.toInt (ea.Value [j++].getValueString ());
			this.integral = StringKit.toInt (ea.Value [j++].getValueString ());
		}else{
			string serverName = ea.Value[j++].getValueString();
			this.uid = ea.Value [j++].getValueString ();
			this.rank = _rank;
			string name1 = ea.Value[j++].getValueString();
			this.name = "(" + serverName + ")" + name1;  
			this.vipLevel = StringKit.toInt (ea.Value [j++].getValueString ());
			this.integral = StringKit.toInt (ea.Value [j++].getValueString ());
		}
        
	}
}
public class RankItemLuckyLianjin
{
	public string uid;
	public string name;
	public int integral;
	public int vipLevel;
	public int rank;
	
	/** 序列化读取 */
	public void bytesRead (int j, ErlArray ea,int _rank) {
        if (ea.Value.Length == 4)
        {
            this.uid = ea.Value[j++].getValueString();
            this.rank = _rank;
            this.name = ea.Value[j++].getValueString();
            this.vipLevel = StringKit.toInt(ea.Value[j++].getValueString());
            this.integral = StringKit.toInt(ea.Value[j++].getValueString());
        }
        else {
            string serverName = ea.Value[j++].getValueString();
            this.uid = ea.Value[j++].getValueString();
            this.rank = _rank;
            string name1 = ea.Value[j++].getValueString();
            this.name = "(" + serverName + ")" + name1;
            this.vipLevel = StringKit.toInt(ea.Value[j++].getValueString());
            this.integral = StringKit.toInt(ea.Value[j++].getValueString());
        }
        
	}
}
public class RankItemLuckyEquip
{
	public string uid;
	public string name;
	public int integral;
	public int vipLevel;
	public int rank;

	/** 序列化读取 */
	public void bytesRead (int j, ErlArray ea) {
		this.uid = ea.Value [j++].getValueString ();
		this.rank = StringKit.toInt (ea.Value [j++].getValueString ());
		this.name = ea.Value [j++].getValueString ();
		this.integral = StringKit.toInt (ea.Value [j++].getValueString ());
		this.vipLevel = StringKit.toInt (ea.Value [j++].getValueString ());
	}
}

public class RankItemMoney
{
	public string uid;
	public string name;
	public int money;
    public int vipLevel;
}

public class RankItemRole
{
	
	public string uid;
	public string name;
	public string cardUid;
    public string cardName;
    public int vipLevel;
}

public class RankItemGoddess
{
	public string uid;
	public string name;
	public int addPer;//女神加成值
	public int vipLevel;
}

/// <summary>
/// 世界首领排行榜实体
/// </summary>
public class WorldBossRankItem
{
	public WorldBossRankItem(string uid,string name,string damage,int vipLevel)
	{
		this.uid = uid;
		this.name = name;
		this.damage = damage;
		this.vipLevel = vipLevel;
	}
	/** 玩家Uid */
	public string uid;
	/** 名字 */
	public string name;
	/** 伤害值 */
	public string damage;
	/** VIP等级 */
	public int vipLevel;
}


//公会幸运女神积分排名信息
public class GuildShakeRankItem
{
	public GuildShakeRankItem(string uid,string name,int integral )
	{
		this.uid = uid;
		this.name = name;
		this.integral = integral;
	}
	/** 玩家Uid */
	public string  uid; 
	/** 姓名 */
	public string name; 
	/** 积分 */
	public int integral;
	/** 贡献 */
	public string contribution; 	
}

/// <summary>
/// 公会战贡献排行榜
/// </summary>
public class GuildAreaHurtRankItem{
	public GuildAreaHurtRankItem(string uid,string name,string warNum,string hurtNum){
		this.uid = uid;
		this.name = name;
		this.warNum = warNum;
		this.hurtNum = hurtNum;
	}
	/** 玩家Uid */
	public string uid;
	/** 玩家姓名 */
	public string name;
	/** 战争点 */
	public string warNum;
	/** 伤害 */
	public string hurtNum;
	/** vip等级 */
	public string vipLevel;
}


