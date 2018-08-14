using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TeamPrepareWindow : WindowBase
{

	public const int WIN_OLD_ITEM_TYPE = 0, //修改界面前的
		WIN_PRACTICE_ITEM_TYPE = 1, //女神试炼界面
		WIN_BOSS_ITEM_TYPE = 2, // 讨伐
		WIN_MISSION_ITEM_TYPE = 3; // 剧情
	
	public const int PRIZES_GENERAL_TYPE = 0, // 普通奖品模板
		PRIZES_FIRST_TYPE = 1; // 首通奖品模板

	private int m_sid;//关卡sid
	private int startTeamId = 1;//初始队伍编号
	private Mission mission;
	public List<Mission>  missionList;
	bool isDis;
	/** 女神试炼 */
	public GameObject practiceWinItem;
	/** 讨伐 */
	public GameObject bossViewWinItem;
	/** 剧情 */
	public GameObject missionWinItem;
	/** 无效的奖励点--解决多次加载BUG用 */
	public GameObject invalidAwardPoint;

	/** 卡片条目预制件 */
	public GameObject roleViewPrefab;
	/** 普通道具条目预制件 */
	public GameObject goodsViewPrefab;
	/** 奖励列表 */
	List<GameObject>[] awardItems;
	/** 窗口中的子窗口显示类型 */
	int showType;
    public int beforeResetMonney = 0;
    public bool isReast = false;
	public void Initialize (int sid)
	{
		Initialize (sid, 0);
	}

	public void Initialize (int sid, int showType)
	{
		this.m_sid = sid;
		this.showType = showType;
        closeHelpButton();
		PrizeSample[][] prizes = loadPrizes ();
		if (prizes != null)
			initAwards (prizes);
		doSwitchBackGround ();
	}

	public void Initialize (Mission mission)
	{
		Initialize (mission, 0);
	}

	public void Initialize (Mission mission, int showType)
	{
		Initialize (mission, showType, null);
	}

	public void Initialize (Mission mission, int showType, List<Mission> missionList)
	{
		this.mission = mission;
		this.missionList = missionList;
		this.m_sid = mission.sid;
		this.showType = showType;
        closeHelpButton();
		if (mission.getRequirLevel () > UserManager.Instance.self.getUserLevel () || !FuBenManagerment.Instance.isCompleteLastMission (m_sid)) {		
			isDis = true;
		} else {
			isDis = false;
		}
		PrizeSample[][] prizes = loadPrizes ();
		if (prizes != null)
			initAwards (prizes);
		doSwitchBackGround ();
		if(GuideManager.Instance.isEqualStep(134006000)){
			GuideManager.Instance.guideEvent();
			GuideManager.Instance.doGuide();
		}
	}
	
	public void Initialize (int sid, int teamId, bool isDis)
	{
		Initialize (sid, teamId, isDis, 0);
	}

	public void Initialize (int sid, int teamId, bool isDis, int showType)
	{
		this.m_sid = sid;
		this.startTeamId = teamId;
		this.isDis = isDis;
		this.showType = showType;
        closeHelpButton();
		PrizeSample[][] prizes = loadPrizes ();
		if (prizes != null)
			initAwards (prizes);
		doSwitchBackGround ();
	}

	public void Initialize (bool isBoss)
	{
		Initialize (isBoss, 0);
	}

	public void Initialize (bool isBoss, int showType)
	{
		this.startTeamId = ArmyManager.PVE_TEAMID;
		this.showType = showType;
        closeHelpButton();
		PrizeSample[][] prizes = loadPrizes ();
		if (prizes != null)
			initAwards (prizes);
		doSwitchBackGround ();
	}

	private void doSwitchBackGround() {
		if (showType == WIN_PRACTICE_ITEM_TYPE) {
			UiManager.Instance.backGround.switchBackGround("practiceBg");
		} else if (showType == WIN_BOSS_ITEM_TYPE) {
			UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		} else if (showType == WIN_MISSION_ITEM_TYPE) {
			UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		}
	}

	protected override void DoEnable () {
		base.DoEnable ();
		if (showType != WIN_OLD_ITEM_TYPE)
			doSwitchBackGround();
	}
    public override void OnNetResume() {
        base.OnNetResume();
        MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid(m_sid);
        MissionWinItem missionWin = missionWinItem.GetComponent<MissionWinItem>();
        FubenBuyChallengeTimesFport fport = FPortManager.Instance.getFPort<FubenBuyChallengeTimesFport>();
        fport.access(sample.sid, (isOk) => {
            if (isReast && beforeResetMonney > UserManager.Instance.self.getRMB()) {
                TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("MISSION_SUCCESS_02"));//重置挑战次数成功
                FuBenManagerment.Instance.resetTimesByMissionSid(sample.sid);
                missionWin.joinMissionButtons[2].textLabel.text = LanguageConfigManager.Instance.getLanguage("missionWinItem01");
                missionWin.joinMissionButtons[2].disableButton(false);
                missionWin.timesValue.text = FuBenManagerment.Instance.getTimesByMissionSid(sample.sid) + "/" + sample.num[2];
                if (FuBenManagerment.Instance.getTimesByMissionSid(sample.sid) == 0) {
                    missionWin.sweepMissionButtons[2].disableButton(true);
                } else {
                    missionWin.sweepMissionButtons[2].disableButton(false);
                }
            }
        });
    }
    void update() {
        missionWinItem.GetComponent<MissionWinItem>().updateButton(mission);
    }
	public PrizeSample[][] loadPrizes ()
	{
		PrizeSample[][] prizes = null;
		if (showType == WIN_PRACTICE_ITEM_TYPE) {
			prizes = loadPracticePrizes ();
		} else if (showType == WIN_BOSS_ITEM_TYPE) {
			prizes = loadBossPrizes ();
		} else if (showType == WIN_MISSION_ITEM_TYPE) {
			prizes = loadMissionPrizes ();
		}
		return prizes;
	}

	private PrizeSample[][] loadPracticePrizes ()
	{
		Chapter chapter = FuBenManagerment.Instance.getPracticeChapter ();
		//		chapter.getNum () + "/" + chapter.getMaxNum ();
		int chapterId = FuBenManagerment.Instance.selectedChapterSid;
		int[] missions = FuBenManagerment.Instance.getAllShowMissions (chapterId);
		List<Mission> missons = new List<Mission> ();
		foreach (int each in missions) {
			missons.Add (MissionInfoManager.Instance.getMissionBySid (each));
		}
		PrizeSample[][] prizes = new PrizeSample[1][];
		prizes [PRIZES_GENERAL_TYPE] = missons [0].getPrizes ();
		setMission (missons [0]);
		return prizes;
	}

	private PrizeSample[][] loadBossPrizes ()
	{
		PrizeSample[][] prizes = new PrizeSample[2][];
		prizes [PRIZES_GENERAL_TYPE] = mission.getPrizes ();
		prizes [PRIZES_FIRST_TYPE] = mission.getFirstPrizes ();
		return prizes;
	}

	private PrizeSample[][] loadMissionPrizes ()
	{
		PrizeSample[][] prizes = new PrizeSample[2][];
		prizes [PRIZES_GENERAL_TYPE] = mission.getPrizes ();
		prizes [PRIZES_FIRST_TYPE] = mission.getFirstPrizes ();
		return prizes;
	}

	protected override void begin ()
	{
		base.begin ();
		ArmyManager.Instance.checkFormation ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
		loadShow ();
        beforeResetMonney = UserManager.Instance.self.getRMB();
	}

	public override void OnStart () {
		base.OnStart ();
		/*如果预制体一开始隐藏的，就无法获取描边颜色，从而导致这个窗口中的按钮失去描边
		 * 所以保持里面组件的激活状态，在窗口打开的一开始，再进行隐藏
		 */
		setOff ();
	}

	public void updateMissionInfo ()
	{
		mission = MissionInfoManager.Instance.getMissionBySid (mission.sid);
	}

	private void loadShow ()
	{
		setShowTitle ();
		activeWinItem (); 
		showAwardItem ();
	}

	private void setShowTitle ()
	{
		if (showType == WIN_OLD_ITEM_TYPE) {
			setTitle (LanguageConfigManager.Instance.getLanguage ("s0004"));
		} else if (showType == WIN_PRACTICE_ITEM_TYPE) {
			setTitle (LanguageConfigManager.Instance.getLanguage ("practiceWindow01"));
		} else if (showType == WIN_MISSION_ITEM_TYPE) {
			setTitle (LanguageConfigManager.Instance.getLanguage ("s0443"));
		}
	}
	/// <summary>
	/// 激活子窗口的可用状态
	/// </summary>
	private void activeWinItem ()
	{
		setOff ();
		GameObject currentWinItem = getCurrentWinItem ();
		currentWinItem.SetActive (true);
	}

	public void setOff ()
	{
		practiceWinItem.SetActive (false);
		bossViewWinItem.SetActive (false);
		missionWinItem.SetActive (false);
	}

	private GameObject getCurrentWinItem ()
	{
		GameObject currentWinItem = missionWinItem;
		if (showType == WIN_PRACTICE_ITEM_TYPE) {
			currentWinItem = practiceWinItem;
		} else if (showType == WIN_BOSS_ITEM_TYPE) {
			currentWinItem = bossViewWinItem;
		} else if (showType == WIN_MISSION_ITEM_TYPE) {
			currentWinItem = missionWinItem;
		}
		return currentWinItem;
	}

	public void msgBack (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			intoFubenBack (msg.msgNum);
		}else{
			MaskWindow.UnlockUI();
		}
	}

	public void intoFubenBack ()
	{
		intoFubenBack (1);
	}

	public void intoFubenBack (int missionLevel)
	{
		//判断玩家是否有足够的存储空间
		if (FuBenManagerment.Instance.isStoreFull ()) {
			return;
		}

		if (m_sid == 0)
			throw new Exception (this.GetType () + " m_sid is 0!");
		MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (m_sid);
		int type = ChapterSampleManager.Instance.getChapterSampleBySid (sample.chapterSid).type;
		if (type == ChapterType.HERO_ROAD) {
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
				EventDelegate.Add (win.OnStartAnimFinish, () => {
					//如果是英雄之章
					HeroRoadIntoFPort port = FPortManager.Instance.getFPort ("HeroRoadIntoFPort") as HeroRoadIntoFPort;
					//发阵形
					port.intoRoad (HeroRoadManagerment.Instance.currentHeroRoad.sample.sid, ArmyManager.Instance.activeID, (isFight) => {
						UserManager.Instance.self.costPoint (1, MissionEventCostType.COST_CHV);
						if (!isFight) {
							//进副本保存队伍
							FuBenIntoFPort.intoMission (m_sid, missionLevel);
						} else {
							MaskWindow.instance.setServerReportWait (true);
							GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattle;
							//直接战斗等后台推战报
						}
						//	UiManager.Instance.backGroundWindow.hideAllBackGround ();
					});
				});
			});
		}
		else if (type == ChapterType.WAR) {
			FuBenManagerment.Instance.tmpStorageVersion = StorageManagerment.Instance.tmpStorageVersion;
			(FPortManager.Instance.getFPort ("FuBenWarAttackFPort") as FuBenWarAttackFPort).attackBoss (m_sid,()=>{
				//直接战斗等后台推战报
				MaskWindow.instance.setServerReportWait(true);
				GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
				MissionInfoManager.Instance.saveMission (m_sid, 1);
			});
		}
		else {
			FuBenIntoFPort port = FPortManager.Instance.getFPort ("FuBenIntoFPort") as FuBenIntoFPort;
			//特殊情况，在新手指引中，进入指定副本
			if (GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID1) {
				port.intoFuben (GuideGlobal.SECOND_MISSION_SID, missionLevel, ArmyManager.Instance.getActiveArmy ().armyid, FuBenIntoFPort.intoMission);
				return;
			}
			Mission mission=MissionInfoManager.Instance.getMissionBySid(m_sid);
			if(mission.getChapterType()==ChapterType.PRACTICE)
			{
				MissionInfoManager.Instance.saveMission(m_sid,missionLevel);
				port.intoPracticeFuben (m_sid, missionLevel, ArmyManager.Instance.getActiveArmy ().armyid, continueIntoMission);
			}else
			{
				port.intoFuben (m_sid, missionLevel, ArmyManager.Instance.getActiveArmy ().armyid, FuBenIntoFPort.intoMission);
			}
		}
	}


	//继续关卡
	private void continueIntoMission ()
	{ 	
		int practiceId=MissionInfoManager.Instance.mission.sid;;
		int practiceLevel=MissionInfoManager.Instance.mission.starLevel;

		UiManager.Instance.openWindow<EmptyWindow> ((win) => {
			ScreenManager.Instance.loadScreen (4, OnLoadingShow, () => {
				UiManager.Instance.switchWindow<MissionMainWindow> ();});
		});
	}	
	void OnLoadingShow ()
	{ 
	//	UiManager.Instance.backGround.hideAllBackGround (true);
	}
	/// <summary>
	/// 初始化奖励
	/// </summary>
	/// <param name="prizes">奖励模板下标0=普通奖励,1=首通奖励</param>
	private void initAwards (PrizeSample[][] prizes)
	{
		if (prizes == null)
			return;
		initAwardItems ();
		PrizeSample sample;
		bool isFirstAward = false;
		for (int i = 0; i < prizes.Length; i++) {
			isFirstAward = false;
			if (i == PRIZES_FIRST_TYPE)
				isFirstAward = true;
			for (int j = 0; j < prizes[i].Length&&j<10; j++) {
				sample = prizes [i] [j];
				if (sample == null)
					continue;
				pushAwardItems (sample, isFirstAward);
			}
		}
	}

	private void initAwardItems ()
	{
		if (awardItems == null) {
			awardItems = new List<GameObject>[2];
			for (int i=0; i<awardItems.Length; i++) {
				awardItems [i] = new List<GameObject> ();
			}
		} else {
			for (int i=0; i<awardItems.Length; i++) {
				awardItems [i].Clear ();
			}
		}
	}
	/// <summary>
	/// 更新奖励条目集
	/// </summary>
	/// <param name="prizeSample">奖励模板</param>
	/// <param name="isFirstAward">是否为第一次奖励</param>
	private void pushAwardItems (PrizeSample prizeSample, bool isFirstAward)
	{
		GameObject gameObject = CreateAwardItem (prizeSample);
		gameObject.transform.parent = invalidAwardPoint.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.zero;
		if (isFirstAward) {
			awardItems [PRIZES_FIRST_TYPE].Add (gameObject);
		} else {
			awardItems [PRIZES_GENERAL_TYPE].Add (gameObject);
		}
	}
	/// <summary>
	/// 创建奖励条目
	/// </summary>
	/// <param name="prizeSample">模板装备</param>
	private GameObject CreateAwardItem (PrizeSample prizeSample)
	{
		GameObject gameObject;
		if (prizeSample.type == PrizeType.PRIZE_CARD) {
			GameObject obj = Instantiate (goodsViewPrefab) as GameObject;
			GoodsView role = obj.GetComponent<GoodsView> ();
			role.fatherWindow = this;
			role.init (CardManagerment.Instance.createCard (prizeSample.pSid));
			role.rightBottomText.gameObject.SetActive (false);
			gameObject = role.gameObject;
			gameObject.SetActive (false);
		} else if (prizeSample.type == PrizeType.PRIZE_EQUIPMENT) {
			GameObject obj = Instantiate (goodsViewPrefab) as GameObject;
			GoodsView goods = obj.GetComponent<GoodsView> ();
			goods.fatherWindow = this;
			goods.init (EquipManagerment.Instance.createEquip (prizeSample.pSid));
			gameObject = goods.gameObject;
			gameObject.SetActive (false);
        }else if (prizeSample.type == PrizeType.PRIZE_MERIT)
        {
            GameObject obj = Instantiate(goodsViewPrefab) as GameObject;
            GoodsView goods = obj.GetComponent<GoodsView>();
            goods.fatherWindow = this;
            goods.init(prizeSample);
            gameObject = goods.gameObject;
            gameObject.SetActive(false);
        }else {
			GameObject obj = Instantiate (goodsViewPrefab) as GameObject;
			GoodsView goods = obj.GetComponent<GoodsView> ();
			goods.fatherWindow = this;
			goods.init (PropManagerment.Instance.createProp (prizeSample.pSid));
			gameObject = goods.gameObject;
			gameObject.SetActive (false);
		}
		return gameObject;
	}

	private void callGuideEvent ()
	{
		GuideManager.Instance.guideEvent ();
	}

	public void showAwardItem ()
	{
		GameObject gameObject = getCurrentWinItem ();
		if (showType == WIN_PRACTICE_ITEM_TYPE) {
			PracticeWinItem practice = practiceWinItem.GetComponent<PracticeWinItem> ();
			practice.Initialize (awardItems, m_sid);
		} else if (showType == WIN_BOSS_ITEM_TYPE) {
			BossViewWinItem bossView = bossViewWinItem.GetComponent<BossViewWinItem> ();
			bossView.Initialize (awardItems, missionList, mission);
		} else if (showType == WIN_MISSION_ITEM_TYPE) {
			MissionWinItem missionWin = missionWinItem.GetComponent<MissionWinItem> ();
			missionWin.Initialize (awardItems);
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		if (gameObj.name == "close") {
			//如果是讨伐，尝试清理掉倒计时监控
			if (showType == WIN_BOSS_ITEM_TYPE) {
				bossViewWinItem.GetComponent<BossViewWinItem> ().clearTimer ();
			}
			finishWindow ();
		} else if (gameObj.name == "formationButton") {
			UiManager.Instance.openWindow <TeamEditWindow> ((win) => {
				win.setComeFrom (TeamEditWindow.FROM_PVE);
			});
		} else if (gameObj.name == "intoFuBen") {
			GuideManager.Instance.doGuide (); 
			GuideManager.Instance.closeGuideMask ();	


			//除讨伐副本和英雄之章 其他副本需要进行行动力判断
			MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (m_sid);
			int cSid = sample.chapterSid;
			int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;
			if (type != ChapterType.WAR && type != ChapterType.PRACTICE && type != ChapterType.HERO_ROAD) {
				if (UserManager.Instance.self.getPvEPoint () < 1) {
					UiManager.Instance.openDialogWindow<PveUseWindow> ();
					return; 
				} 
			}

			int teamId=ArmyManager.PVE_TEAMID;;
			int currentCombat=0;
			if(sample.teamType==TeamType.All)
			{
				currentCombat=ArmyManager.Instance.getTeamAllCombat(teamId);
			}else if(sample.teamType==TeamType.Main)
			{
				currentCombat=ArmyManager.Instance.getTeamCombat(teamId);
			}
			int requestCombat=sample.getRecommendCombat();
			if (currentCombat < requestCombat) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.dialogCloseUnlockUI=false;
					string tip=(sample.teamType==TeamType.Main)?Language("combatTip_01",requestCombat.ToString()):Language("combatTip_02",requestCombat.ToString());
					win.initWindow (2, Language("s0094"), Language ("s0093"), tip, msgBack);
				});
				return;
			}
			intoFubenBack ();
		}
		if (showType == WIN_PRACTICE_ITEM_TYPE) {
			practiceWinItem.GetComponent<PracticeWinItem> ().doClieckEvent (this, gameObj, mission, m_sid);
		} else if (showType == WIN_BOSS_ITEM_TYPE) {
			bossViewWinItem.GetComponent<BossViewWinItem> ().doClieckEvent (this, gameObj, m_sid);
		} else if (showType == WIN_MISSION_ITEM_TYPE) {
			missionWinItem.GetComponent<MissionWinItem> ().doClieckEvent (this, gameObj, m_sid);
		}
	}
	public void setMission (Mission mission)
	{
		this.mission = mission;
		this.m_sid = mission.sid;
	}
	//讨伐更新页面信息
	public void setMissionByBossView (Mission mission)
	{
		this.mission = mission;
		this.m_sid = mission.sid;
		PrizeSample[][] prizes = loadPrizes ();
		if (prizes != null)
			initAwards (prizes);
		BossViewWinItem bossView = bossViewWinItem.GetComponent<BossViewWinItem> ();
		bossView.updateTapShow (mission, awardItems);
	}

	public Mission getMission ()
	{
		return mission;
	}

	public int getShowType ()
	{
		return this.showType;
	}
    ///<summary>
    ///根据需要关闭帮助按钮
    /// </summary>
    private void closeHelpButton() {
        GameObject helpObj = this.transform.FindChild("titleTweenGroupNew(Clone)/top/buttonHelp").gameObject;
        if (showType != WIN_PRACTICE_ITEM_TYPE)
            helpObj.SetActive(false);
    }
}
