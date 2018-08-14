using System;

/// <summary>
/// 天梯数据管理
/// </summary>
using UnityEngine;


public class LaddersManagement
{
	

	public static LaddersManagement Instance {
		get {
			return SingleManager.Instance.getObj ("LaddersManagement") as LaddersManagement;
		}
	}

	public LaddersManagement ()
	{
		Chests = new LaddersChests (ChestCount);
		Players = new LaddersPlayers (PlayerCount);
		Records = new LaddersRecords ();
		Award = new LaddersAwardInfo ();
		TitleAttrEffect = new LaddersAttributeEffect ();
		MedalAttrEffect = new LaddersAttributeEffect ();
	}

	public void M_clear ()
	{
		if (Chests != null) {
			Chests.M_clear ();
		}
		if (Players != null) {
			Players.M_clear ();
		}
		if (Records != null) {
			Records.M_clear ();
		}
		if (Award != null) {
			Award.M_clear ();
		}
	}

	public const int ChestCount = 3;//天梯中宝箱的数量
	public const int PlayerCount = 9;	//天梯中可挑战的玩家数量

	/// <summary>
	/// 天梯排名发奖时间点
	/// </summary>
	public  int nextTime;
	/// <summary>
	/// 每天可申请好友求助的总次数
	/// </summary>
	public  int TotalApplyFriendHelpTimes;
	/// <summary>
	/// 天梯刷新冷却时间
	/// </summary>
	public int CDTime = 2 * 60 * 60;
	/// <summary>
	/// 天梯好友助战每个好友被最多邀请次数
	/// </summary>
	public int BeInviteMaxNum;


	/// <summary>
	/// 当前已经使用的挑战次数
	/// </summary>
	private int mCurrentChallengeTimes = 0;

	public int currentChallengeTimes {
		get {
			if (ServerTimeKit.getDayOfYear () != lastFightTime) {
				mCurrentChallengeTimes = 0;
				lastFightTime = ServerTimeKit.getDayOfYear ();
			}
			return mCurrentChallengeTimes;
		}set {
			mCurrentChallengeTimes = value;
		}
	}
	/// <summary>
	/// 上次使用时间
	/// </summary>
	public int lastFightTime;
	/// <summary>
	/// 最大挑战次数
	/// </summary>
	public int maxFightTime;
	/// <summary>
	/// 上次购买挑战次数时间
	/// </summary>
	public int lastBuyFightTime;
	/// <summary>
	/// 已购买挑战次数
	/// </summary>
	public int mBuyFightCount;

	/// <summary>
	/// 已购买好友出战次数
	/// </summary>
	public int mBuyFriendFightCount = 0;
	/// <summary>
	/// 当前天梯战是否是好友助战 true表示是好友助战 false表示是玩家自己挑战
	/// </summary>
	public bool currentBattleIsFriendHelp = false;

	public int buyFightCount {
		get {
			if (ServerTimeKit.getDayOfYear () != lastBuyFightTime) {
				mBuyFightCount = 0;
				lastBuyFightTime = ServerTimeKit.getDayOfYear ();
			}
			return mBuyFightCount;
		}set {
			mBuyFightCount = value;
		}
	}

	/// <summary>
	/// 当前已经申请好友帮助的使用次数
	/// </summary>
	public int lastFriendHelpTime;
	/// <summary>
	/// 当前已经申请好友帮助的使用次数
	/// </summary>
	private int mCurrentFriendHelpTimes;

	public int currentFriendHelpTimes {
		get {
			if (ServerTimeKit.getDayOfYear () != lastFriendHelpTime) {
				mCurrentFriendHelpTimes = 0;
				lastFriendHelpTime = ServerTimeKit.getDayOfYear ();
			}
			return mCurrentFriendHelpTimes;
		}set {
			mCurrentFriendHelpTimes = value;
		}
	}

	/// <summary>
	/// 当前玩家天梯排名
	/// </summary>
	public int currentPlayerRank = 0;
	/// <summary>
	/// 当前玩家天梯奖章Sid
	/// </summary>
	public int currentPlayerMedalSid = 0;
	/// <summary>
	/// 当前刷新对手的结束时间
	/// </summary>
	public int currentRefreshEndTime = 0;
	public LaddersChests Chests;
	public LaddersPlayers Players;
	public LaddersRecords Records;
	public LaddersAwardInfo Award;
	public LaddersAttributeEffect TitleAttrEffect;
	public LaddersAttributeEffect MedalAttrEffect;
	public LaddersPlayerInfo CurrentOppPlayer;
	public const int v1 = 200;
	public const float v2 = -1330f;
	public const float v3 = 10000f;
	public const int v4 = 20000;
	public const float percent = 100f;
	public int chest = 1;
	public int title = 1;
	public int multiple = 1;

	/// <summary>
	/// 好友助战是否可用
	/// </summary>
	/// <value><c>true</c> if friend can help; otherwise, <c>false</c>.</value>
	public bool FriendCanHelp {
		get {
			return currentFriendHelpTimes < TotalApplyFriendHelpTimes;
		}
	}

	/// <summary>
	/// 计算宝箱中的游戏币
	/// 获得的游戏币=INT(20000*排名^(-1330/10000)*宝箱品质系数(75,100,125)/100*称号加成(config)/100*暴击系数(service give)/100）
	/// </summary> trunc(((Value1*math:pow(Rank,Value2/10000)*Quality/100)*ResAdd/100)*Hit/100),
	/// <returns>The money.</returns>
	/// <param name="chest">Chest.</param>
	public int M_chestMoney (LaddersChestInfo chestInfo)
	{
		int prestige = UserManager.Instance.self.prestige;
		LaddersTitleSample titleSample = LaddersConfigManager.Instance.config_Title.M_getTitle (prestige);
		title = titleSample.factorNum;
		multiple = chestInfo.multiple;
		chest = LaddersConfigManager.Instance.config_Const.chestFactor.factors [chestInfo.index - 1];
		int rank = chestInfo.receiveEnble ? chestInfo.canReceiveRank : currentPlayerRank;
		int money = (int)(v4 * Mathf.Pow (rank, (v2 / v3)) * chest / percent * (title + 10000) / v3 * multiple / percent);
		return money;
	}


	/// <summary>
	/// 计算宝箱中的声望值
	/// 获得的声望=INT(200*排名^(-1330/10000)*宝箱品质系数/100*称号加成/100*暴击系数/100)
	/// </summary>
	/// <returns>The prestige.</returns>
	/// <param name="chest">Chest.</param>
	public int M_chestPrestige (LaddersChestInfo chestInfo)
	{
		int prestigeSelf = UserManager.Instance.self.prestige;
		LaddersTitleSample titleSample = LaddersConfigManager.Instance.config_Title.M_getTitle (prestigeSelf);
		title = titleSample.factorNum;
		multiple = chestInfo.multiple;
		chest = LaddersConfigManager.Instance.config_Const.chestFactor.factors [chestInfo.index - 1];
		int rank = chestInfo.receiveEnble ? chestInfo.canReceiveRank : currentPlayerRank;
		int prestige = (int)(v1 * Mathf.Pow (rank, (v2 / v3)) * chest / percent * (title + 10000) / v3 * multiple / percent);
		return prestige;
	}


	/// <summary>
	/// 返回玩家当前的奖章
	/// </summary>
	/// <returns>The current player medal.</returns>
	public LaddersMedalSample M_getCurrentPlayerMedal ()
	{
		LaddersMedalSample medal = LaddersConfigManager.Instance.config_Medal.M_getMedalBySid (currentPlayerMedalSid);
		return medal;
	}

	/// <summary>
	/// 返回玩家当前的称号
	/// </summary>
	/// <returns>The current player title.</returns>
	public LaddersTitleSample M_getCurrentPlayerTitle ()
	{
		int prestigeSelf = UserManager.Instance.self.prestige;
		LaddersTitleSample title = LaddersConfigManager.Instance.config_Title.M_getTitle (prestigeSelf);
		return title;
	}

	/// <summary>
	/// 返回刷新倒计时
	/// </summary>
	/// <returns>The refresh cutdown.</returns>
	public int M_getRefreshCutdown ()
	{
		int currentTime = ServerTimeKit.getSecondTime ();
		if (currentTime >= currentRefreshEndTime) {
			return 0;
		} else {
			return currentRefreshEndTime - currentTime;
		}
	}
	/// <summary>
	/// 更新宝箱状态
	/// </summary>
	/// <returns>The chest status.</returns>
	public void M_updateChestStatus ()
	{
		LaddersChestInfo[] chests = Chests.M_getChests ();
		LaddersPlayerInfo[] players = Players.M_getPlayers ();

		foreach (LaddersChestInfo chest in chests) {
			bool ok = true;
			foreach (LaddersPlayerInfo player in players) {
				if (player.belongChestIndex == chest.index) {
					if(LaddersManagement.Instance.CurrentOppPlayer != null)
					{
						if(player.rank == LaddersManagement.Instance.CurrentOppPlayer.rank)
						{
							player.isDefeated = LaddersManagement.Instance.CurrentOppPlayer.isDefeated;
						}
					}
					ok = player.isDefeated;
					if (!ok) {
						break;
					}
				}
			}
			chest.receiveEnble = ok;
		}
	}
}


