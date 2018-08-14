using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionMainWindow :WindowBase
{ 
	public int goldBoxCount;
	public int sliverBoxCount;
	public ButtonChat UI_ChatBtn;
	public UILabel goldBoxLabel;
	public UILabel sliverBoxLabel;
	public UILabel moneyCount;
	public GameObject newLineup;
	public UITexture formationSprite;//阵型
	public UISprite pvpButton;//PVP提示按钮
	private Timer timer;//刷新用户信息
	public Transform UIEffectRoot;
	private int pvpTime;
	private int apvpTime;
	private bool isPvp = false;
	public GameObject[] pvpSprits;
	public barCtrl pveBar;
	public barCtrl storeBar;
	public UILabel pveValue;
	public UILabel storeValue;
	public ButtonBase moveButton;
	public GameObject missionCompletePoint;
	public GameObject pvpActiveBar;
	private CallBack callback;
	public ButtonBase ouitButton;
	public ButtonBase closeButton;
	private const int QUITSHOWLEVEL = 11;
	public UILabel fubenName;
	public UILabel practiceName;
	public ButtonBase starButton;//星星查看按钮
	public GameObject bossEffect;
	public GameObject guajiPoint;
	public float awardBoxTime = -1.0f;
	public Award aw = null;
	public Award boxAward = null; //箱子奖励
	public GameObject propPrefab;
	public missionNvShenItem nvshenItem;
	/**英雄解锁进度类 */
	public MissionHeroPropessItem heroPropess;
	public UISprite lucyBg;
	public UILabel lucyMethod;
	public UITexture topBgTexture;//顶部背景
	public GameObject moveEffect;//前进特效
	public UILabel luckyTimeLabel;//限时图标
	const int STAR_NUMBER = 100;
	public ButtonLevelupReward levelupRewardButton;
	public PracticeAwardContent practiceAwardDisplay;
	public PracticeFubenTipContent practicePointSaveTip;
	public bool isShowEffectRoot = true;
	public ButtonBase stopButton;
	public GameObject guideGuaji;
	public GameObject redion;
    //===========爬塔
    public GameObject guajihide;
    public GameObject pkhide;
    public Transform teamTransform;
    public Transform ouitTransform;
    public GameObject towerBarPoint;
    public barCtrl towerBar;
    public UILabel towerValue;
    public Transform baoxiangBeginPoint;
    public GameObject textureDemo;
    public GameObject textureDemo1;
    public GameObject pvePoint;
    public GameObject pvpPoint;
    public GameObject goldPoint;
    public GameObject moneyPoint;
    public GameObject boxPoint;
    public GameObject luckPoint;
    public GameObject towerTitlePoint;
    public UILabel towerTitleLabel;
    public GameObject miaoshaPoint;

    public void playMiaoShaEffect(bool bo)
    {
        if (bo) EffectManager.Instance.CreateEffectCtrlByCache(miaoshaPoint.transform, "Effect/UiEffect/Miaosha", null);
    }

	public void starMethod ()
	{
		if(lucyBg != null && lucyMethod != null)
		{
			lucyBg.transform.parent.gameObject.SetActive (UserManager.Instance.self.getlastStarSum () / STAR_NUMBER > 0);
			lucyMethod.text = (UserManager.Instance.self.getlastStarSum () / STAR_NUMBER).ToString ();
		}
	}

	public void updateLevelupRewardButton ()
	{
		int lastLevelupSid = LevelupRewardManagerment.Instance.lastRewardSid;
		if (lastLevelupSid > 0) {
			lastLevelupSid++;//to show next levelupReward
		}
		LevelupSample rewardSample = LevelupRewardSampleManager.Instance.getSampleBySid (lastLevelupSid);
		if (rewardSample == null||HeroGuideManager.Instance.checkHaveGuid() || MissionInfoManager.Instance.mission.getChapterType () != ChapterType.STORY ||HeroGuideManager.Instance.checkHaveExistGuid()||!HeroGuideManager.Instance.isShowLevelAward()) {//the top level,no reward,disvisible
			levelupRewardButton.gameObject.SetActive (false);
		} else {
			if(rewardSample.showMinLevel<=UserManager.Instance.self.getUserLevel()&&rewardSample.showManLevel>UserManager.Instance.self.getUserLevel()){
				levelupRewardButton.gameObject.SetActive (false);
			}else if(MissionInfoManager.Instance.mission!=null){
				int sid=MissionInfoManager.Instance.mission.sid;
				levelupRewardButton.gameObject.SetActive (true);
				levelupRewardButton.init (rewardSample.level, rewardSample.samples [0]);
			}
		}
	}
	public void showMissionBossWarring (int sid)
	{
		MaskWindow.LockUI ();
		AudioManager.Instance.PlayAudio (145);
		bossEffect.SetActive (true);
		StartCoroutine (Utils.DelayRun (() => {
			bossEffect.SetActive (false);
			//MaskWindow.UnlockUI ();
			MissionManager.instance.showBossWarringCompleteEffectOver ();
		}, 1.667f));
	}

	public override void OnBeginCloseWindow ()
	{
		base.OnBeginCloseWindow ();
		UIEffectRoot.gameObject.SetActive (false);
	}

	public void showPvpActiveBarEffect ()
	{
		//pvpActiveBar.gameObject.SetActive (true);
		//pvpActiveBar.init ();
	    if (pvpActiveBar == null)
            EffectManager.Instance.CreateEffectCtrlByCache(gameObject.transform.FindChild("root"), "Effect/Other/pvpActiveBar_effect", null);
	    else 
            pvpActiveBar.SetActive (true);
		Invoke ("onActiveBarAniCmp", 2);
	}

	private void onActiveBarAniCmp ()
	{
		if(MissionInfoManager.Instance.autoGuaji&&MissionInfoManager.Instance.mSettings[0]){
			MissionInfoManager.Instance.autoGuaji=false;
			PvpInfoManagerment.Instance.openPvpWindow ();
			UiManager.Instance.cancelMask ();
			MissionInfoManager.Instance.mission.restPointNoPvP = true;
		}else if(MissionInfoManager.Instance.autoGuaji&&!MissionInfoManager.Instance.mSettings[0]){
			StartCoroutine(MissionManager.instance.autoMove(0f));
			UiManager.Instance.cancelMask ();
			MissionInfoManager.Instance.mission.restPointNoPvP = true;
		}else{
			PvpInfoManagerment.Instance.openPvpWindow ();
			UiManager.Instance.cancelMask ();
			MissionInfoManager.Instance.mission.restPointNoPvP = true;
		}
		
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		if(guideGuaji.activeInHierarchy)guideGuaji.SetActive(false);
		base.buttonEventBase (gameObj); 
		if(gameObj.name=="stop"){
			MissionInfoManager.Instance.autoGuaji=false;
			stopButton.gameObject.SetActive(false);
			return;
		}
		if (BattleManager.isWaitForBattleData) {
			MaskWindow.UnlockUI ();
			return;
		}
		if (gameObj.name == "move") {
			if(!MissionManager.instance.isLoadFinish) {
				MaskWindow.UnlockUI ();
				return;
			}
			if(MissionInfoManager.Instance.mission!=null&&MissionInfoManager.Instance.mission.sid==41005&&FuBenManagerment.Instance.isNewMission (ChapterType.STORY,41005)&&GuideManager.Instance.loadTimes(51007985)<1){
				GuideManager.Instance.saveTimes(51007985);
			}
			if (MissionManager.instance.AutoRunIndex != -1) {
				MissionManager.instance.AutoRunStart ();
				return;
			}
			nvshen ();
			HeroGuideManager.Instance.doBegin=true;
			MissionManager.instance.moveForward (); 
		}  
		// 放弃副本
		else if (gameObj.name == "ouit") {
			UiManager.Instance.openDialogWindow<MessageWindow> (
				(win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("s0270"), outFuBen);
			}
			);
			
		}		
		
		//离开副本(save)
		else if (gameObj.name == "close") {
			//主窗口有引导，不给直接返回，最好从退出副本返回
			//			if(!GuideManager.Instance.isGuideComplete()) {
			//				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("missionMain03"));
			//				MaskWindow.UnlockUI();
			//				return;
			//			}
			//提前预判是否完成了一次性指引，没有就不给返回，先引导了再说
			if (!GuideManager.Instance.isOnceGuideComplete (GuideGlobal.ONCEGUIDE_BACK)) {
				GuideManager.Instance.onceGuideEvent (GuideGlobal.ONCEGUIDE_BACK);
				MaskWindow.UnlockUI ();
				return;
			}
			LoadingWindow.isShowProgress = false;
            ClmbTowerManagerment.Instance.turnSpriteData = null;
			UiManager.Instance.switchWindow<EmptyWindow> (
				(win) => {
				MissionManager.instance.cleanCache ();
				ScreenManager.Instance.loadScreen (1, MissionManager.instance.missionClean, GameManager.Instance.saveMission);
			}
			);
		} else if (gameObj.name == "team") {
			FuBenGetSelfHpFPort port = FPortManager.Instance.getFPort ("FuBenGetSelfHpFPort") as FuBenGetSelfHpFPort;
			port.getInfo (showTeamViewInMissionWindow);
			
		} else if (gameObj.name == "PK") {
			PvpRankInfoFPort fport = FPortManager.Instance.getFPort ("PvpRankInfoFPort") as PvpRankInfoFPort;
			fport.access (getRankInfo);
		} else if (gameObj.name == "luckyButton") {
			UiManager.Instance.openWindow<BattleDrawWindow> ();
			//hideWindow ();
		} else if (gameObj.name == "levelupAwardButton") {
			UiManager.Instance.openDialogWindow<LevelupRewardWindow> ((win) => {
				win.init (updateLevelupRewardButton);
			});
		} else if (gameObj.name == UI_ChatBtn.name) {
			UiManager.Instance.openWindow<ChatWindow> ((win) => {
				win.initChatWindow (ChatManagerment.Instance.sendType - 1);
			});
		} else if (gameObj.name == "party") {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1,LanguageConfigManager.Instance.getLanguage("s0040"),null,LanguageConfigManager.Instance.getLanguage("total_actionisnotopen"),null);
			});
		}else if(gameObj.name=="guaji"){
			if(!MissionManager.instance.isLoadFinish) {
				MaskWindow.UnlockUI ();
				return;
			}
			UiManager.Instance.openDialogWindow<AutoMoveSetWindow>((win)=>{
				win.dialogCloseUnlockUI=false;
			});
		}
		else if(gameObj.name=="heroShowPoint"){
			HeroGuideSample heroGuidee=null;
			if(HeroGuideManager.Instance.checkHaveGuid()){
				heroGuidee=HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ());
			}else if(HeroGuideManager.Instance.checkHaveExistGuid()){
				heroGuidee=HeroGuideManager.Instance.getOldSample();
			}
			if(heroGuidee!=null){
				if(heroGuidee.prizeSample[0].type==5){
					CardBookWindow.Show (CardManagerment.Instance.createCard(heroGuidee.prizeSample[0].pSid), CardBookWindow.SHOW, null);
				}else if(heroGuidee.prizeSample[0].type==6){
					UiManager.Instance.openWindow<BeastAttrWindow>((win)=>{
						win.Initialize(getFistGoddess(heroGuidee.prizeSample[0].pSid),4);
					});
			   }else if(heroGuidee.prizeSample[0].type==7){
					UiManager.Instance.openDialogWindow<MainCardSurmountShowWindow>((win)=>{
						win.init(StringKit.toInt(heroGuidee.prizeSample[0].num)+3);
					});
					
				}
			}else{
				MaskWindow.UnlockUI();
			}
		}
        else if (gameObj.name == "addPvePoint")
        { 
            UiManager.Instance.openDialogWindow<PveUseWindow>();
        }
        else
        {
            MaskWindow.UnlockUI();
        }
	}
	
	public override void OnAwake ()
	{
		base.OnAwake ();
		//		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "topBG",topBgTexture);
		UiManager.Instance.missionMainWindow = this;
		//	UiManager.Instance.backGround.hideAllBackGround (true);
		MaskWindow.UnlockUI ();
	}
	
	private void outFuBen (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		ArmyManager.Instance.unActiveArmy ();

		FuBenOperateFPort port = FPortManager.Instance.getFPort ("FuBenOperateFPort") as FuBenOperateFPort;
		port.giveUp ();


		Mission currentMission = MissionInfoManager.Instance.mission;
        //如果是主动放弃的爬塔副本 要把奖励这些玩意都清空
        if (currentMission != null && currentMission.getChapterType() == ChapterType.TOWER_FUBEN) {
            ClmbTowerManagerment.Instance.turnSpriteData = null;
            ClmbTowerManagerment.Instance.getAwardSuccessCallBack = null;
            ClmbTowerManagerment.Instance.getGiveUpCallBack = null;
            ClmbTowerManagerment.Instance.isCanGetAward = false;
        }
		//修炼副本在主动放弃时 要给结算奖励
		if (currentMission != null && currentMission.getChapterType () == ChapterType.PRACTICE) {
			int playCurrentStep = MissionInfoManager.Instance.mission.getPlayerPointIndex ();
			if (playCurrentStep == 0) {
				toOutFuBen ();
			} else {
				int historyPracticeHightPoint = UserManager.Instance.self.practiceHightPoint;
				MissionInfoManager.Instance.mission.updatePracticeRecode (playCurrentStep, historyPracticeHightPoint);

				AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_FUBEN_OVER, (awards) => {
					//MissionAward.Instance.parcticeAwards=awards;
					UiManager.Instance.openDialogWindow<PracticeAwardWindow> ((win) => {
						win.init (currentMission);
						win.updateAward (awards);
					});
				}); 
			}
		} else {
			toOutFuBen ();
		}
	}
    public void outTowerFuBen() {
        ArmyManager.Instance.unActiveArmy();
        FuBenOperateFPort port = FPortManager.Instance.getFPort("FuBenOperateFPort") as FuBenOperateFPort;
        port.giveUp();
        Mission currentMission = MissionInfoManager.Instance.mission;
        //如果是主动放弃的爬塔副本 要把奖励这些玩意都清空
        if (currentMission != null && currentMission.getChapterType() == ChapterType.TOWER_FUBEN) {
            ClmbTowerManagerment.Instance.turnSpriteData = null;
            ClmbTowerManagerment.Instance.getAwardSuccessCallBack = null;
            ClmbTowerManagerment.Instance.getGiveUpCallBack = null;
            ClmbTowerManagerment.Instance.isCanGetAward = false;
        }
        toOutFuBen();
    }

	private void toOutFuBen ()
	{
		LoadingWindow.isShowProgress = false;

		//切换到空窗口后回调
		UiManager.Instance.switchWindow<EmptyWindow> (
			(win) => {
			MissionManager.instance.cleanCache ();
			ScreenManager.Instance.loadScreen (1, MissionManager.instance.missionClean, GameManager.Instance.outMission);
		});
	}
	
	private void getRankInfo ()
	{
		if (ServerTimeKit.getSecondTime () >= RankManagerment.Instance.getNextUpdateTime (RankManagerment.TYPE_COMBAT)) {
			RankManagerment.Instance.loadData (RankManagerment.TYPE_COMBAT, pvpRankInfoBack);
		} else
			pvpRankInfoBack ();
	}
	
	private void pvpRankInfoBack ()
	{
		PvpInfoManagerment.Instance.setPvpType (PvpInfo.TYPE_PVP_FB);
		UiManager.Instance.openDialogWindow<PvpInfoWindow> ();
	}
	
	private void updataBar ()
	{
		int numm=UserManager.Instance.self.getPvPPoint ();
		for(int i=0;i<pvpSprits.Length;i++){
			if(i<numm){
				pvpSprits[i].SetActive(true);
			}else pvpSprits[i].SetActive(false);
		}
		pveBar.gameObject.SetActive(true);
			pveBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
			pveValue.text = UserManager.Instance.self.getTotalPvEPoint() + "/" + UserManager.Instance.self.getPvEPointMax ();
	}
	
	private void showTeamViewInMissionWindow ()
	{
		//UiManager.Instance.openWindow<TeamViewInMissionWindow> ();
		//UiManager.Instance.openWindow<TeamViewInMissionWindow1> ();
		UiManager.Instance.openWindow<TeamViewInMissionWindow1>((win) =>
		{
			//win.setComeFrom(TeamViewInMissionWindow1.FROM_PVE);
		});
	}
    //更新爬塔UI界面
    public void updateTowerUI() {
        if (MissionInfoManager.Instance.isTowerFuben()) {
            pvePoint.SetActive(false);
            pvpPoint.SetActive(false);
            goldPoint.SetActive(false);
            moneyPoint.SetActive(false);
            boxPoint.SetActive(false);
            luckPoint.SetActive(false);
            guajihide.SetActive(false);
            pkhide.SetActive(false);
            towerTitlePoint.SetActive(true);
            towerBarPoint.SetActive(true);
            teamTransform.localPosition = new Vector3(160f, -8f, 0f);
            ouitTransform.localPosition = new Vector3(-160f, -8f, 0f);
            string missionName = MissionInfoManager.Instance.mission.getMissionName();
            int numm = 0;
            if (MissionInfoManager.Instance.mission.sid < 151010) {
                numm = StringKit.toInt(missionName.Substring(2, 1));
            } else {
                numm = StringKit.toInt(missionName.Substring(2, 2));
            }
            towerTitleLabel.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow18",numm+"");
            //MissionPointInfo[] mp = MissionInfoManager.Instance.mission.getAllPoint();
            //int conctentPoint = MissionInfoManager.Instance.mission.getComplatePont()-1<=0?0:MissionInfoManager.Instance.mission.getComplatePont();
            //int maxPoint = MissionInfoManager.Instance.mission.getMaxPoint()-2;
            //for (int i = 0; i < mp.Length-1;i++) {
            //    MissionPointInfo mpi = mp[i];
            //    string[] e_sids = mpi.getEventArr();
            //    if (e_sids != null && e_sids.Length > 0) {
            //        for (int j = 0; j < e_sids.Length;j++ ) {
            //            MissionEventSample e = MissionEventSampleManager.Instance.getMissionEventSampleBySid(StringKit.toInt(e_sids[j]));
            //            if (e != null) {
            //                if (e.eventType == MissionEventType.TOW_TREASURE) {
            //                    GameObject obj = NGUITools.AddChild(baoxiangBeginPoint.gameObject, textureDemo.gameObject);
            //                    obj.transform.localPosition = new Vector3((float)(514 / maxPoint) * i - 20, 0f, 0f);
            //                    obj.SetActive(true);
            //                }
            //            }
            //        }
            //    }
                
            //}
            //towerValue.text = MissionInfoManager.Instance.mission.getComplatePont().ToString() + "/" + maxPoint.ToString();
            //towerBar.updateValue((float)MissionInfoManager.Instance.mission.getComplatePont(), (float)maxPoint);

        } else {
            towerBarPoint.SetActive(false);
            guajihide.SetActive(true);
        }
    }
    public void updateTowerLottey() {
        //if (!MissionInfoManager.Instance.mission.getPlayerPoint().isComplete()) return;
        //int conctentPoint = MissionInfoManager.Instance.mission.getComplatePont() - 1 <= 0 ? 0 : MissionInfoManager.Instance.mission.getComplatePont();
        //int maxPoint = MissionInfoManager.Instance.mission.getMaxPoint() - 2;
        //towerValue.text = MissionInfoManager.Instance.mission.getComplatePont().ToString() + "/" + maxPoint.ToString();
        //towerBar.updateValue((float)MissionInfoManager.Instance.mission.getComplatePont(), (float)maxPoint);
    }
    /// <summary>
    /// 开始方法
    /// </summary>
	protected override void begin ()
	{
		base.begin ();
        updateTowerUI();
		if(MissionInfoManager.Instance.autoGuaji){
			stopButton.gameObject.SetActive(true);
		}else{
			stopButton.gameObject.SetActive(false);
		}
		if(bossEffect.activeInHierarchy)bossEffect.SetActive(false);
		if (isShowEffectRoot) {
			UIEffectRoot.gameObject.SetActive (true);
		} else {
			UIEffectRoot.gameObject.SetActive(false);
			isShowEffectRoot = true;
		}
		isPvp = false;
		updataBar ();

		if (GuideManager.Instance.isOverStep (GuideGlobal.NEWOVERSID)) {
			updateLevelupRewardButton ();
			UI_ChatBtn.gameObject.SetActive (true);
		} else {
			levelupRewardButton.gameObject.SetActive (false);
			UI_ChatBtn.gameObject.SetActive (false);
		}

		if (MissionInfoManager.Instance.mission != null && MissionInfoManager.Instance.mission.getChapterType () == ChapterType.PRACTICE) {
			if (GuideManager.Instance.isOverStep (GuideGlobal.NEWOVERSID)) {
				practiceAwardDisplay.gameObject.SetActive (true);
			} else {
				practiceAwardDisplay.gameObject.SetActive (false);
			}
		} else {
			practiceAwardDisplay.gameObject.SetActive (false);
		}


		
		updateUserInfo ();
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
		timer.addOnTimer (updateUserInfo);
		timer.start ();		
		StartCoroutine (Utils.DelayRun (() => {
			if(MissionManager.instance.isLoadFinish) {
				practiceName.transform.parent.gameObject.SetActive (MissionInfoManager.Instance.mission.getChapterType () == ChapterType.PRACTICE);
				UpdatePracticeLabel ();
			}
		}, 0.2f));
		fubenName.text = MissionInfoManager.Instance.getMission ().getMissionName ();
		if (MissionInfoManager.Instance.mission.getChapterType () == ChapterType.STORY) {
			starButton.gameObject.SetActive (true);
		}
		if (callback != null) {
			callback ();
			callback = null;
		}
		
		//副本第一场战斗后需要前进指引
		if (GuideManager.Instance.isEqualStep (4004000)) {
			GuideManager.Instance.guideEvent ();
		}
		
		if (FuBenManagerment.Instance.isNewMission (ChapterType.STORY, MissionInfoManager.Instance.getMission ().sid) && GameManager.Instance.playAnimationType != 2) {
			StartCoroutine (Utils.DelayRun (() => {
				nvshen ();
			}, 0.1f));
		}
		if(MissionInfoManager.Instance.mission.getChapterType () == ChapterType.STORY){
			if(HeroGuideManager.Instance.checkHaveGuid()){
				heroShow();
			}else if(HeroGuideManager.Instance.checkHaveExistGuid()){
				heroOldShow();
			}
		}
		if (GuideManager.Instance.isEqualStep (10001000)) {
			StartCoroutine (Utils.DelayRun (() => {
				showMoveEffect ();
			}, 0.2f));
		}
		if(MissionInfoManager.Instance.mission!=null&&MissionInfoManager.Instance.mission.sid==41005&&FuBenManagerment.Instance.isNewMission (ChapterType.STORY,41005)&&GuideManager.Instance.loadTimes(51007985)<1){
			guideGuaji.SetActive(true);
		}else{
			guideGuaji.SetActive(false);
		}
		MaskWindow.UnlockUI ();
	}
	public void AwardBoxMove ()
	{
		if (sliverBoxCount > 0) {
			EffectCtrl silverCtrl = EffectManager.Instance.CreateEffect (GameObject.Find ("3Dscreen/root/character/role").transform, "Effect/Other/Treasure_Silver");
			GameObject hero = GameObject.Find ("3Dscreen/root/character");
			silverCtrl.transform.localPosition = new Vector3 (0.33f, 0, -0.35f);
			silverCtrl.transform.localScale = Vector3.one;
		}
		if (goldBoxCount > 0) {
			EffectCtrl goldCtrl = EffectManager.Instance.CreateEffect (GameObject.Find ("3Dscreen/root/character/role").transform, "Effect/Other/Treasure_Golden");
			GameObject hero = GameObject.Find ("3Dscreen/root/character");
			goldCtrl.transform.localPosition = new Vector3 (-0.12f, 0, -0.35f);
			goldCtrl.transform.localScale = Vector3.one;
		}
		if (aw == null)
			return;
		if (aw.playerLevelUpInfo != null) {
			List<int> list = FormationManagerment.Instance.getPlayerNewFormationAfterLevelup (aw.playerLevelUpInfo.oldLevel, aw.playerLevelUpInfo.newLevel);
			if (list != null) {
				newLineup.SetActive (true);
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.TEXTURE_TEAM_FORMATION_PATH + list [0].ToString (), formationSprite);
			}
		} 

	}
	
	public void OpenBoxEffect ()
	{
		if (sliverBoxCount > 0) {
			EffectCtrl ctrl = EffectManager.Instance.CreateEffect (this.transform.FindChild ("root").transform, "Effect/UiEffect/OpenTheTreasureChest2");
			ctrl.transform.localPosition = new Vector3 (107, -80, 0);
			
		}
		if (goldBoxCount > 0) {
			EffectCtrl ctrl = EffectManager.Instance.CreateEffect (this.transform, "Effect/UiEffect/OpenTheTreasureChest1");
			ctrl.transform.localPosition = new Vector3 (-107, -80, 0);
		}
		if (boxAward != null) {
			if (boxAward.props != null) {
				foreach (PropAward prop in boxAward.props) {
					GameObject obj = Instantiate (propPrefab) as GameObject;
					UITexture icon = obj.transform.FindChild ("icon").GetComponent<UITexture> ();
					Prop p = PropManagerment.Instance.createProp (prop.sid);
					ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + p.getIconId (), icon);
					obj.transform.parent = this.transform.FindChild ("root").transform;
					obj.transform.localScale = Vector3.one;
					obj.transform.localPosition = Vector3.zero;
					icon.gameObject.SetActive (false);
					obj.SetActive (true);
				}
			}
		}
	}
	
	public void updateUserInfo ()
	{
		if(UiManager.Instance.getWindow<MissionMainWindow>() == null)
			return;
		starMethod ();
		showLuckyTimeLabel ();
		moneyCount.text = UserManager.Instance.self.getMoney ().ToString ();
		starButton.textLabel.text = "X " + UserManager.Instance.self.getlastStarSum ();
		if (MissionInfoManager.Instance.mission != null) {
			goldBoxCount = MissionInfoManager.Instance.mission.getTreasureNum (TreasureType.TREASURE_GOLD);
			goldBoxLabel.text = goldBoxCount.ToString ();
			sliverBoxCount = MissionInfoManager.Instance.mission.getTreasureNum (TreasureType.TREASURE_SILVER);
			sliverBoxLabel.text = sliverBoxCount.ToString ();
		}
		moneyCount.text = UserManager.Instance.self.getMoney ().ToString ();
		updataBar ();
		if (PvpInfoManagerment.Instance.getPvpTime (null) > 0) {
			isPvp = true;
		} else {
			isPvp = false;
		}
		//有PVP时按钮的动画
		if (isPvp) {
			pvpTime = PvpInfoManagerment.Instance.getPvpTime (null);
			if (pvpTime <= 0) {
				isPvp = false;
				pvpButton.alpha = 1;
			}
		} else {
			pvpButton.alpha = 1;
		}
	}

	private void showLuckyTimeLabel ()
	{
		int now = ServerTimeKit.getSecondTime ();
		long[] starInfo = FuBenManagerment.Instance.getStarMultipleTimes ();
		if (starInfo == null)
			return;
		// 在有效时间内
		if (starInfo [0] <= now && now <= starInfo [1]) {
			int time = (int)(starInfo [1] - now);
			string hs, ms, ss;
			int h, m, s;
			h = time / 3600;
			m = (time % 3600) / 60;
			s = (time % 3600) % 60;
			if (h >= 10)
				hs = h.ToString ();
			else
				hs = 0 + h.ToString ();
			if (m >= 10)
				ms = m.ToString ();
			else
				ms = 0 + m.ToString ();
			if (s >= 10)
				ss = s.ToString ();
			else
				ss = 0 + s.ToString ();
			luckyTimeLabel.text = LanguageConfigManager.Instance.getLanguage ("missionMain04", hs, ms, ss);
			if (!luckyTimeLabel.gameObject.activeSelf)
				luckyTimeLabel.gameObject.SetActive (true);
		} else if (luckyTimeLabel.gameObject.activeSelf)
			luckyTimeLabel.gameObject.SetActive (false);
	}
	
	void showMoveEffect ()
	{
		//		iTween.ShakePosition (moveButton.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.2f));
		EffectCtrl a = EffectManager.Instance.CreateEffect (moveButton.transform, "Effect/UiEffect/feature_open");
		Object audioObj = a.gameObject.GetComponent ("AudioPlayer");
		if (audioObj != null) {
			Destroy (audioObj);
		}
		StartCoroutine (Utils.DelayRun (() => {
			Destroy (a.gameObject, 2f);
		}, 1.5f));
	}
	/// <summary>
	/// 展示过期的激活任务
	/// </summary>
	public void heroOldShow(){
		HeroGuideSample hero=HeroGuideManager.Instance.getOldSample();
		if(hero==null)return;
		Card cardd=HeroGuideManager.Instance.getSuitCard(hero);
		heroPropess.gameObject.SetActive(true);
		heroPropess.updatePropess(false,cardd,hero.prizeSample[0].type,hero);
	}
	/// <summary>
	/// 展示英雄进度（英雄冰封效果）
	/// </summary>
	public void heroShow(){
		HeroGuideSample heroGuide=HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ());
		if(heroGuide==null)return;
		Card cardd=HeroGuideManager.Instance.getSuitCard(heroGuide);
		//这里就开始显示需要的动画和面板
		if(heroGuide.showFlag==0&&MissionInfoManager.Instance.mission.getPlayerPoint().isComplete()){//不显示动画 就一直显示头像 进度条在指定点开始更新
			heroPropess.gameObject.SetActive(true);
			MaskWindow.LockUI ();
			if(MissionInfoManager.Instance.mission.getPlayerPointIndex()<heroGuide.pointNum){
				heroPropess.updatePropess(false,cardd,heroGuide.prizeSample[0].type);
			}else{
				heroPropess.updatePropess(true,cardd,heroGuide.prizeSample[0].type);
			}
		}else if(heroGuide.showFlag==1){//展示解封动画，在指定点开始显示头像
			if(!HeroGuideManager.Instance.doBegin&&MissionInfoManager.Instance.mission.getPlayerPointIndex()==heroGuide.pointNum){
				heroPropess.gameObject.SetActive(true);
				MaskWindow.LockUI ();
				heroPropess.updatePropess(false,cardd,heroGuide.prizeSample[0].type);
			}else if(MissionInfoManager.Instance.mission.getPlayerPointIndex()>heroGuide.pointNum){
				heroPropess.gameObject.SetActive(true);
				MaskWindow.LockUI ();
				heroPropess.updatePropess(false,cardd,heroGuide.prizeSample[0].type);
			}
			else if(HeroGuideManager.Instance.doBegin&&MissionInfoManager.Instance.mission.getPlayerPointIndex()==heroGuide.pointNum&&heroGuide.haveTalk==0&&
			        (MissionInfoManager.Instance.mission.getPlayerPoint().isComplete()||(MissionInfoManager.Instance.mission.getPlayerPoint().getPointEvent()!=null&&MissionInfoManager.Instance.mission.getPlayerPoint().getPointEvent().battleType==0))){
				HeroGuideManager.Instance.doBegin=false;
				HeroGuideManager.Instance.openNvShenWindow(heroGuide,true,1);
			}
		}else if(heroGuide.showFlag==2){//开始显示头像，到了指定点则真是解封
			heroPropess.gameObject.SetActive(true);
			MaskWindow.LockUI ();
			if(MissionInfoManager.Instance.mission.getPlayerPointIndex()<heroGuide.pointNum){
				heroPropess.updatePropess(false,cardd,heroGuide.prizeSample[0].type);
			}else if(!MissionInfoManager.Instance.mission.getPlayerPoint().isComplete()){
				heroPropess.updatePropess(false,cardd,heroGuide.prizeSample[0].type);
			}else if(heroGuide.haveTalk==0&&HeroGuideManager.Instance.doBegin){
				HeroGuideManager.Instance.doBegin=false;
				HeroGuideManager.Instance.openNvShenWindow(heroGuide,false,3);
			}
			else{
				heroPropess.updatePropess(true,cardd,heroGuide.prizeSample[0].type);
			}
		}
	}
	/// <summary>
	/// 更新英雄进度
	/// </summary>
	public void showEffectForHero(bool bo){
		HeroGuideSample heroGuide=HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ());
		if(heroGuide==null)return;
		if(heroGuide.showFlag==1||heroGuide.showFlag==2){
			heroPropess.gameObject.SetActive(true);
			MaskWindow.LockUI ();
			Card ca=HeroGuideManager.Instance.getSuitCard(heroGuide);
			StartCoroutine (Utils.DelayRun (() => {
				heroPropess.showHeroEffect (ca,bo,heroGuide.prizeSample[0].type);
			}, 0.1f));
		}
	}
	private Card getFistGoddess(int sid){
		List<BeastEvolve> list =BeastEvolveManagerment.Instance.getAllBest();//女神样本
		ArrayList beastList=StorageManagerment.Instance.getAllBeast();//已经有的女神
		bool flag=false;
		int flagNUm=0;
		if(beastList==null){
			return  CardManagerment.Instance.createCard (sid);
		}else{
			for(int i=0;i<list.Count;i++){
				flag=false;
				for(int j=0;j<beastList.Count;j++){
					flagNUm=i;
					if((list[i] as BeastEvolve).getBeast(0).getName()==(beastList[j] as Card).getName()){
						flag=true;
						break;
					}
				}
				if(!flag)return(list[flagNUm] as BeastEvolve).getBeast(0);
			}
			return  CardManagerment.Instance.createCard (sid);
		}
	}
	//新手指引-女神碎片收集展示
	public void nvshen ()
	{
		//都解救完了就不用播放了
		if (GuideManager.Instance.isOverStep (GuideGlobal.SPECIALSID7)) {
			nvshenItem.gameObject.SetActive (false);
			return;
		}
		nvshenItem.gameObject.SetActive (true);
		if (GameManager.Instance.playAnimationType == 0) {
			nvshenItem.initWindow ();
		}
		
		//第一个副本内特殊运作
		if (MissionInfoManager.Instance.mission.sid == GuideGlobal.FIRST_MISSION_SID && FuBenManagerment.Instance.isNewMission (ChapterType.STORY, GuideGlobal.FIRST_MISSION_SID)) {
			//救出女神,第三个点,对话完,关闭女神被锁窗口后开始播放卡片移动动画
			if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3 && GameManager.Instance.playAnimationType == 1) {
				GameManager.Instance.playAnimationType = 2;
				MaskWindow.LockUI ();
				StartCoroutine (Utils.DelayRun (() => {
					nvshenItem.showNvShenEffect (false);
				}, 0.1f));
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3 && GameManager.Instance.playAnimationType == 2) {
				nvshenItem.initNvShen ();
				nvshenItem.initCheck (0);
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3 && GameManager.Instance.playAnimationType == 0) {
				GameManager.Instance.playAnimationType = 2;
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () > 3) {
				nvshenItem.initNvShen ();
				nvshenItem.initCheck (0);
			}
		}
		//第二个副本内特殊运作
		if (MissionInfoManager.Instance.mission.sid == GuideGlobal.SECOND_MISSION_SID && FuBenManagerment.Instance.isNewMission (ChapterType.STORY, GuideGlobal.SECOND_MISSION_SID)) {
			nvshenItem.initNvShen ();
			if (MissionInfoManager.Instance.mission.getPlayerPointIndex () < 2) {
				nvshenItem.initCheck (0);
			}
			//打到第一块碎片
			else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 2 && GameManager.Instance.playAnimationType == 1) {
				GameManager.Instance.playAnimationType = 2;
				MaskWindow.LockUI ();
				nvshenItem.initCheck (0);
				StartCoroutine (Utils.DelayRun (() => {
					nvshenItem.showNvShenEffect (true);
				}, 0.1f));
				StartCoroutine (Utils.DelayRun (() => {
					NGUITools.AddChild (nvshenItem.checkOnBg [0].gameObject, nvshenItem.effectLight);
					NGUITools.AddChild (nvshenItem.checkOnBg [0].gameObject, nvshenItem.effectFire);
				}, 1f));
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 2 && GameManager.Instance.playAnimationType == 2) {
				nvshenItem.initCheck (1);
				if (nvshenItem.checkOnBg [0].transform.childCount == 0) {
					NGUITools.AddChild (nvshenItem.checkOnBg [0].gameObject, nvshenItem.effectLight);
					NGUITools.AddChild (nvshenItem.checkOnBg [0].gameObject, nvshenItem.effectFire);
				}
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 2 && GameManager.Instance.playAnimationType == 0) {
				GameManager.Instance.playAnimationType = 2;
				nvshenItem.initCheck (0);
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () > 2) {
				nvshenItem.initCheck (1);
				if (nvshenItem.checkOnBg [0].transform.childCount == 0) {
					NGUITools.AddChild (nvshenItem.checkOnBg [0].gameObject, nvshenItem.effectFire);
				}
			}
		}
		//第三个副本内特殊运作
		if (MissionInfoManager.Instance.mission.sid == GuideGlobal.THREE_MISSION_SID && FuBenManagerment.Instance.isNewMission (ChapterType.STORY, GuideGlobal.THREE_MISSION_SID)) {
			nvshenItem.initNvShen ();
			if (nvshenItem.checkOnBg [0].transform.childCount == 0) {
				NGUITools.AddChild (nvshenItem.checkOnBg [0].gameObject, nvshenItem.effectFire);
			}
			if (MissionInfoManager.Instance.mission.getPlayerPointIndex () < 3) {
				nvshenItem.initCheck (1);
			}
			//打到第二块碎片
			else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3 && GameManager.Instance.playAnimationType == 1) {
				GameManager.Instance.playAnimationType = 2;
				MaskWindow.LockUI ();
				nvshenItem.initCheck (1);
				StartCoroutine (Utils.DelayRun (() => {
					nvshenItem.showSuiPianEffect ();
				}, 0.1f));
				StartCoroutine (Utils.DelayRun (() => {
					NGUITools.AddChild (nvshenItem.checkOnBg [1].gameObject, nvshenItem.effectLight);
					NGUITools.AddChild (nvshenItem.checkOnBg [1].gameObject, nvshenItem.effectFire);
				}, 1f));
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3 && GameManager.Instance.playAnimationType == 2) {
				nvshenItem.initCheck (2);
				if (nvshenItem.checkOnBg [1].transform.childCount == 0) {
					NGUITools.AddChild (nvshenItem.checkOnBg [1].gameObject, nvshenItem.effectLight);
					NGUITools.AddChild (nvshenItem.checkOnBg [1].gameObject, nvshenItem.effectFire);
				}
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () == 3 && GameManager.Instance.playAnimationType == 0) {
				GameManager.Instance.playAnimationType = 2;
				nvshenItem.initCheck (1);
			} else if (MissionInfoManager.Instance.mission.getPlayerPointIndex () > 3) {
				nvshenItem.initCheck (2);
				if (nvshenItem.checkOnBg [1].transform.childCount == 0) {
					NGUITools.AddChild (nvshenItem.checkOnBg [1].gameObject, nvshenItem.effectFire);
				}
			}
		}
	}
	
	void Update ()
	{
		if (isPvp) {
			pvpButton.alpha = sin ();
		}
		if(MissionInfoManager.Instance.autoGuaji){
			//if(stopButton!=null&&stopButton.isDisable())stopButton.disableButton(false);
			if(!guajiPoint.activeInHierarchy)guajiPoint.SetActive(true);
		}else{
			//if(stopButton!=null&&!stopButton.isDisable())stopButton.disableButton(true);
			if(guajiPoint.activeInHierarchy)guajiPoint.SetActive(false);
		}
		//		showBoxEffect ();
	}

	protected override void DoEnable ()
	{
		//这里不需要背景变黑,调基类默认背景变黑
	}

	public override void DoDisable ()
	{ 
		if (timer != null)
		{
			timer.stop ();
			timer = null; 
		}
		base.DoDisable ();

		UiManager.Instance.removeAllEffect ();
	}


	public void UpdatePracticeLabel () {
		practiceName.text = Language ("missionPracticeRunTips_2", MissionManager.instance.AutoRunIndex != -1 ? 0 : MissionManager.instance.mapInfo.getPlayerPointInfo ().pointIndex);
	}

	/// <summary>
	/// 更新聊天按钮好友聊天显示状态
	/// </summary>
	public void UpdateChatMsgTips ()
	{
		UI_ChatBtn.setShowTips (ChatManagerment.Instance.IsHaveNewHaveMsg ());
	}

	/// <summary>
	/// 以下的代码是为了处理副本内短线重连
	/// </summary>

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		if(MissionInfoManager.Instance.autoGuaji){
			MissionInfoManager.Instance.autoGuaji=false;
			if(guajiPoint.activeInHierarchy)guajiPoint.SetActive(false);
			stopButton.gameObject.SetActive(false);
		}
		BattleManager.isWaitForBattleData = false;
		FPortManager.Instance.getFPort<FuBenIntoFPort> ().toContinue ((hasFB) => {
			if (!hasFB) {
				MessageWindow.ShowAlert (Language ("reconnection_02"), (msg) => {
					FPortManager.Instance.getFPort<FuBenInfoFPort> ().info (MissionManager.instance.missionEnd, MissionInfoManager.Instance.mission.getChapterType ());
				});
			}
		});
	}
}
