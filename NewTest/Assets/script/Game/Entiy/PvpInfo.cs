using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * PVP信息实体类
 * @author 汤琦
 * */
public class PvpInfo
{
	public PvpInfo (int overTime, string rule, int round, PvpOppInfo[] oppInfo)
	{ 
		this.overTime = overTime;
		this.rule = rule;
		this.round = round;
		this.oppInfo = oppInfo;
	}

	public const int TYPE_PVP_FB = 1;
	public const int TYPE_PVP_SWEEP = 2;

	public int overTime = 0;//结束时间时间戳
	public string rule;// cup 杯赛 match 三选一
	public int round = 0;//回合 1 第一回合.....
	public PvpOppInfo[] oppInfo;
	
	public void clear ()
	{
		if (oppInfo != null)
			oppInfo = null;
		this.overTime = 0;
		this.rule = "";
		this.round = 0;
	}
}

public class PvpRankInfo
{
	public PvpRankInfo (int win, int rank)
	{
		this.win = win;
		this.rank = rank;
	}

	public int win = 0;//连胜
	public int rank = 0;//排名
	//public int integral = 0;
	//public int integralRank = 0;

	public void clear ()
	{
		this.win = 0;
		this.rank = 0;
	}
}

public class PvpOppInfo
{

	/** 专为公会战而生 */
	public static PvpOppInfo paresInfo(ErlArray list){
		int index = 0;
		string name = list.Value [index++].getValueString ();
		int vip = StringKit.toInt(list.Value [index++].getValueString ());
		int headIcon = StringKit.toInt (list.Value [index++].getValueString ());
		long exp = StringKit.toLong (list.Value [index++].getValueString ());
		int star = StringKit.toInt (list.Value [index++].getValueString ());
		int beastSid = 0;
		long beastExp = 0;
		ErlType beastType = list.Value [index++];
		if (beastType.getValueString () != "none") {
			ErlArray beastArray = beastType as ErlArray;
			beastSid = StringKit.toInt(beastArray.Value[0].getValueString());
			beastExp = StringKit.toLong(beastArray.Value[1].getValueString());
		} 
		ErlArray cardArray = list.Value [index++] as ErlArray;
		List<PvpOppTeam> ptList = new List<PvpOppTeam> ();
		for (int i =0; i<cardArray.Value.Length; i++) {
			ErlArray ea = cardArray.Value [i] as ErlArray;
			if (ea.Value.Length != 0) {
				int teamOppSid = StringKit.toInt (ea.Value [0].getValueString ());
				long teamOppExp = StringKit.toLong (ea.Value [1].getValueString ());
				int evoLevel = StringKit.toInt (ea.Value [2].getValueString ());
				int surLevel = StringKit.toInt (ea.Value [3].getValueString ());
				int teamOppIndex = StringKit.toInt(ea.Value [4].getValueString ());
				/** 这个位置是后台算的位置,为了配合其他位置,需要调整数值 */
				teamOppIndex --;
				int hpNow = StringKit.toInt(ea.Value [5].getValueString ());
				int hpMax = StringKit.toInt(ea.Value [6].getValueString ());
				PvpOppTeam pt = new PvpOppTeam (teamOppSid,teamOppExp,teamOppIndex,evoLevel,surLevel,hpNow,hpMax);
				ptList.Add (pt);
			}
		}
		string guildName = list.Value [index++].getValueString ();
		PvpOppInfo opp = new PvpOppInfo (name, vip, headIcon, exp,star, beastSid, beastExp, ptList.ToArray(),guildName);
		return opp;
	}

	public static PvpOppInfo pares(ErlArray list)
	{
		
		int index = 0;
		string uid = list.Value [index++].getValueString ();
		string name = list.Value [index++].getValueString ();
		string guildName = list.Value [index++].getValueString ();
		int headIcon = StringKit.toInt (list.Value [index++].getValueString ());
		long exp = StringKit.toLong (list.Value [index++].getValueString ());
		int state = StringKit.toInt (list.Value [index++].getValueString ());
		ErlArray lists = list.Value [index++] as ErlArray;
		int combat = StringKit.toInt (list.Value [index++].getValueString ());
		int allCombat = StringKit.toInt (list.Value [index++].getValueString ());//队伍中所有卡片战斗力 对于10人阵而言
		int star = StringKit.toInt (list.Value [index++].getValueString ());
		int vipLv = StringKit.toInt (list.Value [index++].getValueString ());
		index = 0;
		int formation = StringKit.toInt (lists.Value [index++].getValueString ());
		ErlArray bArray = lists.Value [index++] as ErlArray;
		int beastSid = 0;
		long beastExp = 0;
		string beastUid = "";
		if (bArray.Value.Length != 0) {
			beastSid = StringKit.toInt (bArray.Value [0].getValueString ());
			beastExp = StringKit.toLong (bArray.Value [1].getValueString ());
			beastUid = bArray.Value [2].getValueString ();
		}
		ErlArray tArray = lists.Value [index++] as ErlArray;
		index = 0;
		List<PvpOppTeam> ptList = new List<PvpOppTeam> ();
		for (int j = 0; j < tArray.Value.Length; j++) {
			ErlArray ea = tArray.Value [j] as ErlArray;
			if (ea.Value.Length != 0) {
				int teamOppSid = StringKit.toInt (ea.Value [0].getValueString ());
				long teamOppExp = StringKit.toLong (ea.Value [1].getValueString ());
				string teamOppUid = ea.Value [2].getValueString ();
				int evoLevel = StringKit.toInt (ea.Value [3].getValueString ());
				int surLevel = StringKit.toInt (ea.Value [4].getValueString ());
				int teamOppIndex = j;
				PvpOppTeam pt = new PvpOppTeam (teamOppSid, teamOppExp, teamOppIndex, teamOppUid, evoLevel, surLevel);
				ptList.Add (pt);
			}
		}
		PvpOppInfo opp=new 	PvpOppInfo (uid, name, guildName, headIcon, exp, state, formation, beastSid, beastExp, beastUid, ptList.ToArray (), combat, star, vipLv);
		opp.allCombat=allCombat;
		return opp;
	}


	public PvpOppInfo (string uid, string name, string guildName, int headIcon, long exp, int state, int formation, int beastSid, long beastExp, string beastUid, PvpOppTeam[] opps, int _combat, int _star
	                  , int _vipLv)
	{

		this.uid = uid;
		this.name = name;
		if ("[]".Equals (guildName))
			guildName = "";
		this.guildName = guildName;
		this.headIcon = headIcon;
		this.exp = exp;
		this.state = state;
		this.formation = formation;
		this.beastSid = beastSid;
		this.beastExp = beastExp;
		this.beastUid = beastUid;
		this.opps = opps;
		this.combat = _combat;

		this.star = _star;
		this.vipLv = _vipLv;

	}

	public PvpOppInfo(string name,int vipLevel,int headIcon,long exp ,int star,int beastSid,long beastExp,PvpOppTeam[] opps,string guildName){
		this.name = name;
		this.vipLv = vipLevel;
		this.headIcon = headIcon;
		this.exp = exp;
		this.star = star;
		this.beastSid = beastSid;
		this.beastExp = beastExp;
		this.opps = opps;
		this.guildName = guildName;
	}

	public string uid;//uid
	//public int integral = 0;//积分
	public string name;//名字
	public string guildName;//公会
	public int headIcon = 0;//头像图标sid
	public long exp = 0;//经验值
	public int state = 0;//0 输 1 赢
	//public int nameIcon = 0;//称号图标sid
	public int vipLv = 0;//VIP等级
	public int formation = 0;//阵型
	public int beastSid = 0;
	public long beastExp = 0;
	public string beastUid = "";
	public PvpOppTeam[] opps;//主力sid和经验
	public PvpOppTeam[] subOpps;//替补sid和经验
	public int combat = 0;//战力
	public int allCombat = 0;//所有人战斗力
	public int star = 0;//星座
	public int arenaTeam;
	public int arenaIntegral ;
	public int arenaRank ;
	public PlatFormUserInfo sdkInfo;
	public int ladderRank;
	public int prestige;
	public int medalSid;

	public void clear ()
	{
		if (opps != null)
			opps = null;
		this.name = "";
		this.guildName = "";
		this.uid = "";
		this.headIcon = 0;
		this.exp = 0;
		this.state = 0;
		this.formation = 0;
		this.beastSid = 0;
		this.beastExp = 0;
		this.beastUid = "";
		this.combat = 0;
		this.vipLv = 0;
	}
}


public class PvpOppTeam
{


	public PvpOppTeam (int sid, long exp, int index, string uid, int evoLevel, int surLevel)
	{
		this.sid = sid;
		this.exp = exp;
		this.index = index;
		this.uid = uid;
		this.evoLevel = evoLevel;
		this.surLevel = surLevel;
	}

	/** 专为公会战而生 */
	public PvpOppTeam(int sid,long exp ,int index,int evoLevel,int surLevel,int hpNow,int hpMax){
		this.sid = sid;
		this.exp = exp;
		this.index = index;
		this.evoLevel = evoLevel;
		this.surLevel = surLevel;
		this.hpNow = hpNow;
		this.hpMax = hpMax;
	}

	public int sid = 0;
	public string uid = "";
	public long exp = 0;
	public int index = 0;
	public int evoLevel = 0;
	public int surLevel = 0;
	/** 当前血量 */
	public int hpNow = 0;
	/** 最大血量 */
	public int hpMax = 0;

	public void clear ()
	{
		this.index = 0;
		this.exp = 0;
		this.sid = 0;
		this.uid = "";
	}


}
