using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * PVP信息实体类管理器
 * @author 汤琦
 * */
public class PvpInfoManagerment
{
	private PvpInfo oppInfos;
	private PvpRankInfo pvpRankInfo;
	private int currentCompleteRound = 0;//当前完成的场次
	public bool  isCurrentRoundBattlePlayed = false;//是否当前回合的战斗已经看过
	private bool isGetPrize = false;
	private List<BattleFormationCard[]> bfLists;//PVP对手封装战斗队伍
	private int oppIndex = 0;
	private PvpOppInfo cupOpp;//杯赛对手
	private Timer timer;//刷新PVP倒计时
//	private int pvpTime;
	private CallBack<int> callback;
	private bool isStartTime = false;
	private PvpOppInfo currentOpp;//当前对手
	private int pvpType;
    public bool isMs = false;

	public static PvpInfoManagerment Instance {
		get{ return SingleManager.Instance.getObj ("PvpInfoManagerment") as PvpInfoManagerment;}		
	}


	//清除数据
	public void clear ()
	{
		if (oppInfos != null)
			oppInfos = null;
		if (pvpRankInfo != null)
			pvpRankInfo = null;
		if (bfLists != null)
			bfLists = null;
		if (cupOpp != null)
			cupOpp = null;
		if (callback != null)
			callback = null;
		if (currentOpp != null)
			currentOpp = null;
		this.currentCompleteRound = 0;
		this.isGetPrize = false;
		this.oppIndex = 0;
		this.callback = null;
		this.isStartTime = false;
	}

	public int getPvpType ()
	{
		return pvpType;
	}

	public void setPvpType (int _pvpType)
	{
		pvpType = _pvpType;
	}
	
	public void setCurrentOpp ()
	{
		currentOpp = getOpp ();
	}
	
	public PvpOppInfo getCurrentOpp ()
	{
		return currentOpp;
	}
	
	public int getPvpWinNum ()
	{
		int num = 0;
		if (oppInfos != null && getLeftPvpTime () > 0) { 
			num = pvpRankInfo.win + oppInfos.round - 1;
		} else
			num = pvpRankInfo.win;
		return num;
	}
	
	public int getPvpTime (CallBack<int> callback)
	{
		this.callback = callback;
		return getLeftPvpTime ();
	}

	private int getLeftPvpTime ()
	{
		if (oppInfos != null)
			return oppInfos.overTime - ServerTimeKit.getSecondTime ();
		return 0;
	}
	
	public bool getIsStartTime ()
	{
		return isStartTime;
	}

	
	//初始化PVP事件
	public void initPvpEvent ()
	{
		PvpGetInfoFPort fport = FPortManager.Instance.getFPort ("PvpGetInfoFPort") as PvpGetInfoFPort;
		fport.access (showPvpEffect);
	} 
	
	//发送PVP通信
	public void sendPvpFprot (CallBack callback)
	{
		if (getLeftPvpTime () <= 0) {
			PvpGetInfoFPort fport = FPortManager.Instance.getFPort ("PvpGetInfoFPort") as PvpGetInfoFPort;
			fport.access ((hasPvp) => {
				checkPvp (hasPvp);
				callback ();
			});
		} else
			callback ();
	}
	
	public void checkPvp (bool hasPvP)
	{
		if (!hasPvP)
			return;

		setCurrentRound ();
		startTime ();
	}
	
	void showPvpEffect (bool hasPvP)
	{
		if (!hasPvP)
			return;

		if (UiManager.Instance.missionMainWindow != null && UiManager.Instance.missionMainWindow.gameObject.activeSelf) {
			UiManager.Instance.missionMainWindow.showPvpActiveBarEffect ();
		}
		startTime ();
	}
	
	public void startTime ()
	{
		if (timer == null) {
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
			timer.addOnTimer (updatePvpTime);
			timer.start ();
		}
	}
	
	private void updatePvpTime ()
	{
		if (getLeftPvpTime () <= 0) {
			this.callback = null;
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
		} else if (callback != null) {
			callback (getLeftPvpTime ());
		}
	}
	
	//打开PVP相应窗口
	public void openPvpWindow ()
	{
		FPortManager.Instance.getFPort<PvpRankInfoFPort> ().access (() => {
			if (oppInfos.rule == "cup") {
				UiManager.Instance.openWindow <PvpCupWindow> ();
			} else if (oppInfos.rule == "match") {
				UiManager.Instance.openWindow<PvpMainWindow> ();
			}
		});
	}
	
	public PvpOppInfo getOpp ()
	{
		if (oppInfos != null)
			return oppInfos.oppInfo [oppIndex];
		else 
			return null;
	}
	
	public void setOppIndex (int index)
	{
		oppIndex = index;
	}

	/// <summary>
	/// 特殊对于天梯可直接设置对手信息
	/// </summary>
	/// <param name="_oppInfo">_opp info.</param>
	public void setLadderPvpOppInfo (PvpOppInfo _oppInfo)
	{
		currentOpp = _oppInfo;
	}
	
	public int getOppIndex ()
	{
		return oppIndex;
	}
	
	//设置回合数
	public void setCurrentRound ()
	{
		if (oppInfos == null) {
			return;
		}
		currentCompleteRound = oppInfos.round;
	}
	
	//获得回合数
	public int getCurrentRound ()
	{
		return currentCompleteRound;
	}
	//获得杯赛对手
	public void setOppIndex ()
	{
		if (currentCompleteRound == 1) {
			oppIndex = 1;
		} else if (currentCompleteRound == 2) {
			if (oppInfos.oppInfo [2].state == 1) {
				oppIndex = 2;
			} else if (oppInfos.oppInfo [3].state == 1) {
				oppIndex = 3;
			}
		} else if (currentCompleteRound == 3) {
			for (int i = 4; i < oppInfos.oppInfo.Length; i++) {
				if (oppInfos.oppInfo [i].state == 1) {
					oppIndex = i;
				}
			}
		}
	}
	//在杯赛中是否是玩家对手
	public bool isCupOpp (PvpOppInfo opp)
	{

		if (currentCompleteRound == 1) {
			if (opp.uid == oppInfos.oppInfo [1].uid) {
				return true;
			}
		} else if (currentCompleteRound == 2) {
			if (opp.state == 1 && opp.uid == oppInfos.oppInfo [2].uid) {
				return true;
			} else if (opp.state == 1 && opp.uid == oppInfos.oppInfo [3].uid) {
				return true;
			}
		} else if (currentCompleteRound == 3) {
			for (int i = 4; i < oppInfos.oppInfo.Length; i++) {
				if (oppInfos.oppInfo [i].state == 1 && oppInfos.oppInfo [i].uid == opp.uid) {
					return true;
				}
			}
		}
		return false;
	}
	
	public void clearDate ()
	{
		oppInfos = null;
		oppIndex = 0;
		currentCompleteRound = 0;
		isGetPrize = false;
	}
	
	public void createPvpInfo (int overTime, string rule, int round, PvpOppInfo[] oppInfo)
	{
		oppInfos = new PvpInfo (overTime, rule, round, oppInfo); 
		changeBattleFormationCards ();
	}
	
	public void createPvpRankInfo (int win, int rank)
	{
		pvpRankInfo = new PvpRankInfo (win, rank);
	}
	
	public PvpRankInfo getPvpRankInfo ()
	{
		return pvpRankInfo;
	}
	  
	public PvpInfo getPvpInfo ()
	{
		return oppInfos;
	}

	public void clearPvpInfo ()
	{
		oppInfos = null;
	}
	
	public void setGetPrize ()
	{
		isGetPrize = true;
	}
	
	public void result (bool isWin)
	{
		if (isGetPrize) {
			clearDate ();
            if (!GameManager.Instance.isCanBeSecondSkill ||
            PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) != 1) BattleManager.Instance.awardFinfish();
			return;
		}
		if (isWin) {
			setCurrentRound ();
			UiManager.Instance.openWindow<PvpResultWindow> ((win) => {
				win.initInfo (isWin);
			});
		} else {
			PvpInfoManagerment.Instance.clearDate ();
			UiManager.Instance.openWindow<PvpResultWindow> ((win) => {
				win.initInfo (isWin);
			});
		}
	}
	//设置杯赛对手 
	public void setCupOpp (PvpOppInfo cupOpp)
	{
		this.cupOpp = cupOpp;
		bfLists = new List<BattleFormationCard[]> ();
		BattleFormationCard[] bfs = new BattleFormationCard[10];
		for (int k = 0; k < cupOpp.opps.Length; k++) {
			bfs [k] = new BattleFormationCard ();
			bfs [k].loc = FormationManagerment.Instance.getLoctionByIndex (cupOpp.formation, cupOpp.opps [k].index);
			CardSample cs = CardSampleManager.Instance.getRoleSampleBySid (cupOpp.opps [k].sid);
			bfs [k].card = CardManagerment.Instance.createCard (cs.sid, cupOpp.opps [k].evoLevel, cupOpp.opps [k].surLevel);
			bfs [k].setLevel (EXPSampleManager.Instance.getLevel (cs.levelId, cupOpp.opps [k].exp, 0));
			bfs [k].setHp (-1);
			bfs [k].setHpMax (-1);
		}
		bfLists.Add (bfs);
	}
	
	private void changeBattleFormationCards ()
	{
		if (oppInfos.rule == "match") {
			bfLists = new List<BattleFormationCard[]> ();
			for (int i = 0; i < oppInfos.oppInfo.Length; i++) {
				BattleFormationCard[] bfs = new BattleFormationCard[10];
				for (int k = 0; k < oppInfos.oppInfo[i].opps.Length; k++) {
					bfs [k] = new BattleFormationCard ();
					bfs [k].loc = FormationManagerment.Instance.getLoctionByIndex (oppInfos.oppInfo [i].formation, oppInfos.oppInfo [i].opps [k].index);
					CardSample cs = CardSampleManager.Instance.getRoleSampleBySid (oppInfos.oppInfo [i].opps [k].sid);
					bfs [k].card = CardManagerment.Instance.createCard (cs.sid, oppInfos.oppInfo [i].opps [k].evoLevel, oppInfos.oppInfo [i].opps [k].surLevel);
					bfs [k].setLevel (EXPSampleManager.Instance.getLevel (cs.levelId, oppInfos.oppInfo [i].opps [k].exp, 0));
					bfs [k].setHp (-1);
					bfs [k].setHpMax (-1);
				}
				bfLists.Add (bfs);
			}
		}
	}
	
	public BattleFormationCard[] getBattleFormationCards ()
	{
		if (oppInfos.oppInfo.Length == 3) {
			return bfLists [oppIndex];
		} else {
			return bfLists [0];
		}
	}
	
	public BattleFormationCard[] getUserBFCards ()
	{
		BattleFormationCard[] bfs = new BattleFormationCard[10];
		for (int i = 0; i < 5; i++) {
			Card card1 = StorageManagerment.Instance.getRole (ArmyManager.Instance.getArmy (3).players [i]);
			if (card1 != null) {
				bfs [i] = new BattleFormationCard ();
				bfs [i].card = card1;
				bfs [i].loc = FormationManagerment.Instance.getLoctionByIndex (ArmyManager.Instance.getActiveArmy ().formationID, i); 
			}
			Card card2 = StorageManagerment.Instance.getRole (ArmyManager.Instance.getArmy (3).alternate [i]);
			if (card2 != null) {
				bfs [i + 5] = new BattleFormationCard ();
				bfs [i + 5].card = card2; 
				bfs [i + 5].loc = FormationManagerment.Instance.getLoctionByIndex (ArmyManager.Instance.getActiveArmy ().formationID, i);
			}
		}  
		return bfs; 
	}
	//消耗全部pvp
	private void usePvpAll ()
	{
		UserManager.Instance.self.costPoint (3, MissionEventCostType.COST_PVP);
	}

	//消耗一点pvp
	private void usePvpOne ()
	{
		UserManager.Instance.self.costPoint (1, MissionEventCostType.COST_PVP);
	}
	
	public void sendFight (int atract)
	{
		setCurrentOpp ();
		PvpFightFPort fport = FPortManager.Instance.getFPort ("PvpFightFPort") as PvpFightFPort;
		if (atract == 1)
			fport.access (atract, currentOpp.uid, PvpInfoManagerment.Instance.getPvpType (), usePvpOne);
		else if (atract == 2)
			fport.access (atract, currentOpp.uid, PvpInfoManagerment.Instance.getPvpType (), usePvpAll);
	}

    
	public void sendLaddersFight (string uid)
	{
		sendLaddersFight (uid, null);
	}

	public void sendLaddersFight (string uid, CallBack<string> callBack)
	{
		LaddersPlayerInfo oppInfo = LaddersManagement.Instance.CurrentOppPlayer;
		LaddersFightFPort fport = FPortManager.Instance.getFPort ("LaddersFightFPort") as LaddersFightFPort;
		fport.startFigth (callBack, uid, oppInfo.index, oppInfo.belongChestIndex);
	}

}
	

