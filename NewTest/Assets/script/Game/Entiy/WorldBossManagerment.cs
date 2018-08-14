using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 世界首领管理器
/// </summary>
public class WorldBossManagerment {

	/* methods */
	public WorldBossManagerment () { 
	}

	/* static methods */
	public static WorldBossManagerment Instance {
		get{ return SingleManager.Instance.getObj ("WorldBossManagerment") as WorldBossManagerment;}
	}

	/** 挑战冷却周期(秒) */
	public const int ATTACK_CD = 10;
	/** 刷新首领血量冷却周期(秒) */
	public const int RESFRESH_HPCD = 3;
	/** 复活消耗(钻石) */
	public const int RESFRESH_COST = 10;
	/** 复活战斗力提升比例 */
	public const float ADD_ATTRIBUTE_PER = 1.1f;

	/** 是否跳战世界首领 */
	public bool isAttackBoss = false;
	/** 世界首领激活时间 */
	private int startTime = 0;
	/** 世界首领结束时间 */
	private int overTime = 0;
	/** 克星卡片Sid组 */
	private int[] nemesisCard = null;
	/** 世界首领总血量 */
	private long boosHp = 0;
	/** 伤害排行信息 */
	private List<WorldBossRankItem> rankList = null;

	/** 获得排名奖励 */
	public List<ArenaAwardSample> getAward () {
		return ArenaAwardSampleManager.Instance.getSamplesByType (ArenaAwardSample.TYPE_WORLDBOSS);
	}

	/** 获得克星卡片Sid组 */
	public int[] getNemesidCard () {
		return nemesisCard;
	}

	/** 设置克星卡片Sid组 */
	public void setNemesidCard (int[] sids) {
		nemesisCard = sids;
	}

	/** 获得世界首领结束时间 */
	public int getOverTime () {
		return overTime;
	}

	/** 设置世界首领结束时间 */
	public void getOverTime (int time) {
		overTime = time;
	}

	/** 获得世界首领激活时间 */
	public int getStartTime () {
		return startTime;
	}

	/** 设置世界首领激活时间 */
	public void setStartTime (int time) {
		startTime = time;
	}

	/** 获得世界首领总血量 */
	public long getBossHp () {
		return boosHp;
	}

	/** 设置世界首领总血量 */
	public void setBossHp (long hp) {
		this.boosHp = hp;
	}

	/** 是否正在挑战期间 */
	public bool isInTime () {
		int nowTime = ServerTimeKit.getSecondTime ();
		return startTime < nowTime && overTime > nowTime;
	}

	/** 获得伤害排名信息 */
	public List<WorldBossRankItem> getRank () {
		rankList = new List<WorldBossRankItem> ();
		rankList.Add (new WorldBossRankItem("0","11","9999",9));
		rankList.Add (new WorldBossRankItem("1","22","8888",8));
		rankList.Add (new WorldBossRankItem("2","33","7777",0));
		return rankList;
	}

	/** 设置伤害排名信息 */
	public void setRank (List<WorldBossRankItem> rank) {
		rankList = rank;
	}

	/// <summary>
	/// 挑战世界首领端口
	/// </summary>
	public void attackWorldBoss () {
		FPortManager.Instance.getFPort<WorldBossFPort> ().attackWorldBoss (()=>{
			MaskWindow.instance.setServerReportWait(true);
			GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
		});
	}
}
