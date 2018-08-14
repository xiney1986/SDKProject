using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 队伍数据
  * 包含有所队员的序列化信息
  * 用于传递给Team实例化characterData以及characterCtrl
  * @author 李程
  * */

public class TeamInfo
{ 
	public TeamInfoPlayer guardianForce;//召唤兽
	public int teamCamp = 0;//队伍阵营
	public List<TeamInfoPlayer> list = new List<TeamInfoPlayer> ();
	public List<TeamInfoPlayer> subList = new List<TeamInfoPlayer> ();
	public const int OWN_CAMP = 1;
	public const int ENEMY_CAMP = 2;

	public TeamInfo (int camp)
	{ 
		teamCamp = camp;
	} 
	
	//设置召唤兽编号
	public void setGuardianForce (TeamInfoPlayer gf)
	{ 
		if (gf.camp != teamCamp)
			return;
		this.guardianForce = gf;
	}
	
	//添加teamInfoPlayer 
	public void addTeamInfoPlayer (TeamInfoPlayer player)
	{
		if (!isSameCamp (player.camp))
			return;
		list.Add (player);
	}

	//添加替补的逻辑数据
	public void addTeamInfoSub (TeamInfoPlayer player)
	{
		if (!isSameCamp (player.camp))
			return;
		if (subList == null)
			subList = new List<TeamInfoPlayer> ();

		subList.Add (player);
	}

	//是否是相同阵营
	public bool isSameCamp (int camp)
	{
		if (teamCamp == 0) 
			return false; 
		if (camp == teamCamp)
			return true;
		else
			return false;
	} 

	//根据阵形站位获得对应的替补数据
	public TeamInfoPlayer getSubstitute (int embattle)
	{
		if (subList == null || subList.Count <= 0)
			return null;


		for (int i=0; i<=subList.Count; i++) {
			
			if (i >= subList.Count || subList [i] == null)
				continue;

			if (subList [i].embattle == embattle) {
				//根据index来对应substitute中的人
				return subList [i];
			}
		}

		return null;
	}

	
}

public class TeamInfoPlayer
{ 
	public int camp = 0;//阵营 1为自己 2为对手  TeamInfo.OWN_CAMP 
	public int sid = 0;//sid 模板编号 
	public int id = 0;//战斗中的唯一标示 
	public string uid = "";//卡片 唯一标示
	public int hp = 0;//当前血量 召唤兽标示怒气
	public int maxHp = 0;//最高血量  召唤兽标示最高怒气
	public string master = "";//所属人 
	public int embattle = 0;//阵型站位
	public bool isGuardianForce; // 是否为召唤兽
	public bool isAlternate = false;
	public int evoLevel = 0; //卡片进化等级
	public int surLevel = 0;// 卡片突破等级//
	
	public TeamInfoPlayer ()
	{
		 
	}	
	
}
