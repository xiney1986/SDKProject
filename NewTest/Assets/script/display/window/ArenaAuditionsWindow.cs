using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 竞技场海选窗口
/// yxl
/// </summary>
public class ArenaAuditionsWindow : WindowBase
{

	public GameObject arenaRolePrefab;
	public UILabel lblLevel;
	public UILabel lblTimes; //挑战次数
	public UILabel lblCD; //挑战时间
	public UILabel lblRank; //我的排名

	public UILabel lblIntegral; //我的积分
	public UILabel lblMerit; //我的功勋
	public GameObject integralAddGroup;
	public GameObject meritAddGroup;
	public UILabel integralAddLabel;
	public UILabel meritAddLabel;
	public ButtonBase btnRefreshChallengeCD; //刷新挑战CD按钮
	public ButtonBase btnRefreshEnemy; //刷新对手按钮

	public GameObject[] roleLocations; //3d角色的2d位置
	public ArenaRoleTip[] roleTips; //角色信息
	public UILabel lblUpdateCD; //刷新cd
	public UILabel lblTimeCD;
	public GameObject buttonClose;
	public GameObject backgroundEffectRoot;
	public GameObject awardIconIntegral;
	public GameObject awardIconMerit;
	public ArenaAwardNum numFinal;
	public ArenaAwardNum numIntegral;
	public ButtonBase[] enemyButtons;//敌人点击按钮
	public UILabel freeTimes;
    public GameObject miaoshaoTranfrom;
	[HideInInspector]
	public bool
		showTeamDialog; //只有从首页进入页面才会提示

	ArenaRoles arenaRoles;
	ArenaManager arenaManager;
	//Timer updateCDTimer;
	//Timer updateEnemyCDTimer;
	List<ArenaUserInfo> enemyList;
	Timer checkStateTimer;
	[HideInInspector]
	public int
		isWin = -1;//打完出来是否胜负，在获取海选对手前赋值
	public override void OnAwake () {
		base.OnAwake ();
		isWin = -1;
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		if (arenaRoles != null) {
			Utils.SetLayer (arenaRoles.gameObject, LayerMask.NameToLayer ("Hide"));
		}
	}

	// Use this for initialization
	void initialization ()
	{
		//has initialization 
		if(arenaRoles!=null)
		{
			return;
		}
		arenaManager = ArenaManager.instance;
		User user = UserManager.Instance.self;
		lblLevel.text = user.getUserLevel ().ToString ();

		GameObject obj = Instantiate (arenaRolePrefab) as GameObject; 
		arenaRoles = obj.GetComponent<ArenaRoles> ();
		EffectManager.Instance.CreateEffectCtrlByCache (backgroundEffectRoot.transform, "Effect/UiEffect/MeteorShower", null);
		checkStateTimer = TimerManager.Instance.getTimer (1000);
		checkStateTimer.addOnTimer (CheckState);
		checkStateTimer.start ();

		initEnemyButtons ();
	}

	private void initEnemyButtons ()
	{
		for (int i=0; i<enemyButtons.Length; i++) 
			enemyButtons [i].onClickEvent = OnEnemyItemClick;
	}

	void cacheModel ()
	{
		string[] paths = new string[]{
			"mission/ez",
			"mission/girl",
			"mission/mage",
			"mission/maleMage",
			"mission/point",
			"mission/swordsman",
			"mission/archer",
		};
		ResourcesManager.Instance.cacheData (paths, (list) => {
			cacheFinish ();
		}, "other");
	}

	void cacheFinish ()
	{
	//	UiManager.Instance.gameCamera.clearFlags = CameraClearFlags.Depth;
	//	UiManager.Instance.backGround.hideAllBackGround (false);
		if (arenaRoles != null) {
			Utils.SetLayer (arenaRoles.gameObject, LayerMask.NameToLayer ("3D"));
		}
	
			if (showTeamDialog && UserManager.Instance.self.getUserLevel () < GameConfig.Instance.getInt (GameConfig.SID_ARENA_TEAM_DIALOG)) {
				showTeamDialog = false;
			}
			FPortManager.Instance.getFPort<ArenaMassFPort> ().access (OnInfoLoad);
	
		integralAddGroup.SetActive (false);
		meritAddGroup.SetActive (false);
		MaskWindow.UnlockUI ();
	}
	protected override void begin ()
	{
		initialization();

		if (ResourcesManager.Instance.allowLoadFromRes) {
			cacheFinish ();
		} else {
			cacheModel ();
		}
	}

    public void updateArenaAuditionWindow()
    {
        initialization();
        if (ResourcesManager.Instance.allowLoadFromRes) {
            cacheFinish();
        } else {
            cacheModel();
        }
    }

    public override void OnNetResume()
    {
        base.OnNetResume();
        updateArenaAuditionWindow();
        if (UiManager.Instance.getWindow<ArenaIntegralAwardWindow>() != null)
        {
            //List<ArenaAwardSample> awards;
            //awards = ArenaAwardSampleManager.Instance.getSamplesByType(ArenaAwardWindow.TYPE_INTEGRAL);
            //ArenaIntegralAwardItem[] trs = UiManager.Instance.getWindow<ArenaIntegralAwardWindow>().content.GetComponentsInChildren<ArenaIntegralAwardItem>();
            //if (awards != null)
            //{
            //    for (int i = 0; i < awards.Count; i++)
            //    {
            //        trs[i].initialize(awards[i], UiManager.Instance.getWindow<ArenaIntegralAwardWindow>());
            //    }
            //}
            UiManager.Instance.getWindow<ArenaIntegralAwardWindow>().finishWindow();
        }
    }

    public void init()
    {
        EffectManager.Instance.CreateEffectCtrlByCache(miaoshaoTranfrom.transform, "Effect/UiEffect/Miaosha", null);
        //miaoShaoPerfab.SetActive(true);
        StartCoroutine(Utils.DelayRun(() => {
            initialization();
            if (ResourcesManager.Instance.allowLoadFromRes) {
                cacheFinish();
            } else {
                cacheModel();
            }
        }, 2f));
        
    }
    public void initt() {
        EffectManager.Instance.CreateEffectCtrlByCache(miaoshaoTranfrom.transform, "Effect/UiEffect/Miaosha", null);

    }
	void CheckState ()
	{
		if (!ArenaManager.instance.isStateCorrect () && gameObject.activeInHierarchy) {
			checkStateTimer.stop ();
			checkStateTimer = null;

			MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena25"), (msg) => {
				UiManager.Instance.clearWindowsName ("MainWindow");
				UiManager.Instance.BackToWindow<MainWindow> ();
			});
		}
	}

	void OnDestroy ()
	{
//		if (updateCDTimer != null)
//			updateCDTimer.stop ();
//
//		if (updateEnemyCDTimer != null)
//			updateEnemyCDTimer.stop ();

		if (checkStateTimer != null)
			checkStateTimer.stop ();
	}
	public override void OnBeginCloseWindow () {
		base.OnBeginCloseWindow ();
		if(DestroyWhenClose){
			Destory3D();
		}else{
			Hide3D();
		}
		
	}
	void Destory3D(){
		if (arenaRoles != null && !arenaRoles.destroyed)
			Destroy (arenaRoles.gameObject);
	}
	
	void Hide3D(){
		if (arenaRoles != null) {
			Utils.SetLayer (arenaRoles.gameObject, LayerMask.NameToLayer ("Hide"));
		}
	}

	void OnInfoLoad ()
	{
		if (ArenaManager.instance.self == null) {
			MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena11"), (msg) => {
				buttonEventBase (buttonClose);
			});
			return;
		}

		updateInfo ();

		string currentMassEnemyUid = ArenaManager.instance.currentMassEnemyUid;
		ArenaUserInfo currentMassEnemy = null;
		GameObject currentEnemyLocation = null;

		List<ArenaUserInfo> list = arenaManager.getEnemyList ();
		int[] icons = new int[list.Count];
		for (int i = 0; i < icons.Length; i++) {
			icons [i] = list [i].headIcon;
			roleTips [i].init (list [i]);
			roleTips [i].gameObject.SetActive (true);

			if (list [i].challengedWin) {
				icons [i] *= 10000;
			}

			if (list [i].uid == currentMassEnemyUid) {
				currentMassEnemy = list [i];
				currentEnemyLocation = roleLocations [i];
			}
		}
		arenaRoles.init (icons, roleLocations);
		enemyList = list;

		//处理挑战胜利后奖励动画
		if (currentMassEnemyUid != null) {
			ArenaManager.instance.currentMassEnemyUid = null;
			if (currentMassEnemy != null && currentMassEnemy.challengedWin) {
				StartCoroutine (Utils.DelayRun (() => {
					awardIconIntegral.SetActive (true);
					awardIconIntegral.transform.localPosition = currentEnemyLocation.transform.localPosition;
					TweenPosition tp = TweenPosition.Begin (awardIconIntegral, 0.8f, new Vector3 (-254, 358, 0));
					tp.method = UITweener.Method.EaseIn;
					EventDelegate.Add (tp.onFinished, () => {
						awardIconIntegral.SetActive (false);
					}, true);
				}, 0f));
				StartCoroutine (Utils.DelayRun (() => {
					awardIconMerit.SetActive (true);
					awardIconMerit.transform.localPosition = currentEnemyLocation.transform.localPosition;
					TweenPosition tp2 = TweenPosition.Begin (awardIconMerit, 0.8f, new Vector3 (-254, 308, 0));
					tp2.method = UITweener.Method.EaseIn;
					EventDelegate.Add (tp2.onFinished, () => {
						awardIconMerit.SetActive (false);
					}, true);
				}, 0.4f));
			}
		}
		PlayerAwardDataEffect (isWin);
	}

	private void PlayerAwardDataEffect (int isWin)
	{
		int integralDesc = IncAttributeGlobal.Instance.getIntAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_INTEGRAL);
		if (integralDesc >= 0 && isWin != -1) {
			integralDesc = ArenaManager.instance.self.integral - integralDesc;
			if (integralDesc > 0) {
				integralAddGroup.transform.localPosition = new Vector3 (lblIntegral.transform.localPosition.x + lblIntegral.width + 10, integralAddGroup.transform.localPosition.y, integralAddGroup.transform.localPosition.z);
				integralAddGroup.SetActive (true);
				TweenLabelNumber tln2 = TweenLabelNumber.Begin (integralAddLabel.gameObject, 0.3f, integralDesc);
				EventDelegate.Add (tln2.onFinished, () => {
					StartCoroutine (Utils.DelayRun (() => {
						lblIntegral.text = LanguageConfigManager.Instance.getLanguage ("Arena05") + arenaManager.self.integral;
						integralAddGroup.SetActive (false);
					}, 1f));
				}, true);
			}
			IncAttributeGlobal.Instance.removeAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_INTEGRAL);
		}
		int meritDesc = IncAttributeGlobal.Instance.getIntAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_MERIT);
		if (meritDesc >= 0 && isWin != -1) {
            meritDesc = UserManager.Instance.self.merit - meritDesc;
            updateMetr(meritDesc);
		}
		string str = null;
		if (isWin == 0) {
			str = LanguageConfigManager.Instance.getLanguage ("ArenaAuditions01", integralDesc.ToString (), meritDesc.ToString ());
		} else if (isWin == 1) {
			str = LanguageConfigManager.Instance.getLanguage ("ArenaAuditions02", integralDesc.ToString (), meritDesc.ToString ());
		}
		this.isWin = -1;
		if (!string.IsNullOrEmpty (str)) {
			TextTipWindow.ShowNotUnlock (str);
		}
	}
    public void updateMetr(int dec){
        if (dec > 0) {
				meritAddGroup.transform.localPosition = new Vector3 (lblMerit.transform.localPosition.x + lblMerit.width + 10, meritAddGroup.transform.localPosition.y, meritAddGroup.transform.localPosition.z);
				meritAddGroup.SetActive (true);
                TweenLabelNumber tln2 = TweenLabelNumber.Begin(meritAddLabel.gameObject, 0.3f, dec);
				EventDelegate.Add (tln2.onFinished, () => {
					StartCoroutine (Utils.DelayRun (() => {
						lblMerit.text = LanguageConfigManager.Instance.getLanguage ("Arena06")  + UserManager.Instance.self.merit;
						meritAddGroup.SetActive (false);
					}, 1f));
				}, true);
			}
			IncAttributeGlobal.Instance.removeAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_MERIT);
    }
	public void updateInfo ()
	{
		numFinal.loadFinalAndGuess ();
		numIntegral.loadData (); 
		UpdateChallengeCount ();
		WindowBase curwin = UiManager.Instance.CurrentWindow;
		if (!(curwin is ArenaAuditionsWindow) || curwin == null && !curwin.gameObject.activeInHierarchy)
			return;
		btnRefreshChallengeCD.disableButton (false);
//		if (updateCDTimer == null) {
//			updateCDTimer = TimerManager.Instance.getTimer (1000);
//			updateCDTimer.addOnTimer (updateCD);
//			updateCDTimer.start ();
//		}
		//updateCD ();

//		if (updateEnemyCDTimer == null) {
//			updateEnemyCDTimer = TimerManager.Instance.getTimer (1000);
//			updateEnemyCDTimer.addOnTimer (updateEnemyCD);
//			updateEnemyCDTimer.start ();
//		}
//		updateEnemyCD ();
        if (CommonConfigSampleManager.Instance.getSampleBySid<ArenaNumsSample>(5).getTimesInt() - ArenaManager.instance.challengeLastUpdateTime > 0)
        {
			freeTimes.gameObject.SetActive (true);
			freeTimes.text = (CommonConfigSampleManager.Instance.getSampleBySid<ArenaNumsSample>(5).getTimesInt() - ArenaManager.instance.challengeLastUpdateTime).ToString ();
		}
		else {
			freeTimes.gameObject.SetActive(false);
		}
		if (arenaManager.self != null) {
			lblRank.text = LanguageConfigManager.Instance.getLanguage ("Arena04", arenaManager.getTeamNameById (arenaManager.self.team) + "", "[d8603e] " + arenaManager.self.rank + " [-]");
			if (arenaManager.self.rank < 17)
				lblRank.text = lblRank.text + LanguageConfigManager.Instance.getLanguage ("Arena03");
		}
		int integralValue = IncAttributeGlobal.Instance.getIntAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_INTEGRAL);
		if (integralValue == 0) {
			if (arenaManager.self != null) {
				integralValue = arenaManager.self.integral;
			}
		}
		int meritValue = IncAttributeGlobal.Instance.getIntAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_MERIT);
		if (meritValue == 0)
			meritValue = UserManager.Instance.self.merit;
		lblIntegral.text = LanguageConfigManager.Instance.getLanguage ("Arena05") + " : " + integralValue;
		lblMerit.text = LanguageConfigManager.Instance.getLanguage ("Arena06") + " : " + meritValue;
	}

	void UpdateChallengeCount ()
	{
		int count = arenaManager.getChallengeCount ();
		lblTimes.text = LanguageConfigManager.Instance.getLanguage ("Arena01") + ": " + count + "/" + arenaManager.maxChallengeCount;
        if (CommonConfigSampleManager.Instance.getSampleBySid<ArenaNumsSample>(5).getTimesInt() - ArenaManager.instance.challengeLastUpdateTime > 0)
        {
			freeTimes.gameObject.SetActive (true);
            freeTimes.text = (CommonConfigSampleManager.Instance.getSampleBySid<ArenaNumsSample>(5).getTimesInt() - ArenaManager.instance.challengeLastUpdateTime).ToString();
		}
		else {
			freeTimes.gameObject.SetActive(false);
		}
	}


	void addChalleageTimes ()
	{
		/** 挑战次数满 */
		if (arenaManager.getChallengeCount () >= arenaManager.maxChallengeCount) {
			MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena38"), null);
			return;
		}

		/*** 可购买挑战次数已用尽 */
		int currentCanBuyNum = arenaManager.getMaxCanBuyCount () - arenaManager.buyChallengeCount;
		if (currentCanBuyNum <= 0) {
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_031"));
			});
			return ;
		} 


		UiManager.Instance.openDialogWindow<BuyWindow> ((win) => {
			win.init (new ArenaChallengePrice (arenaManager.buyChallengeCount), Mathf.Min ((arenaManager.maxChallengeCount - arenaManager.getChallengeCount ()), currentCanBuyNum), 1, PrizeType.PRIZE_RMB, (msg) => {
				if (msg.msgEvent == msg_event.dialogOK) {
					ArenaChallengePrice price = msg.msgInfo as ArenaChallengePrice;
					if (price.getPrice (msg.msgNum) > UserManager.Instance.self.getRMB ())
						MessageWindow.ShowRecharge (LanguageConfigManager.Instance.getLanguage ("s0158"));
					else
						FPortManager.Instance.getFPort<ArenaBuyChallengeCountFport> ().access (msg.msgNum, (success) => {
							if (success) {
								arenaManager.buyChallengeCount += msg.msgNum;
								UpdateChallengeCount ();
							}
						});
				}
			});
		});
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "button_close") {
			IncAttributeGlobal.Instance.removeAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_INTEGRAL);
			IncAttributeGlobal.Instance.removeAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_MERIT);
			finishWindow ();
//			UiManager.Instance.openMainWindow ();
		} else if (gameObj.name == "button_rank") {
			UiManager.Instance.openWindow<ArenaRankWindow> ();
		} else if (gameObj.name == "button_addTimes") {
			addChalleageTimes ();
		} else if (gameObj.name == "buttonHelp") {
			UiManager.Instance.openDialogWindow<GeneralDesWindow>((win)=>{
				string massTimeStr = ArenaTimeSampleManager.Instance.getMassTimeString(ArenaManager.instance.state,ArenaManager.instance.stateEndTime);
				string finalTimeStr = ArenaTimeSampleManager.Instance.getFinalTimeString(ArenaManager.instance.state,ArenaManager.instance.stateEndTime);
				string rule = LanguageConfigManager.Instance.getLanguage("Arena68",massTimeStr,finalTimeStr);
				win.initialize(rule,LanguageConfigManager.Instance.getLanguage("Arena69"),"");
			});
		}
		//屏蔽刷新CD功能
//		else if (gameObj.name == "button_refreshCD") {
//			if (am.getChallengeCD () <= 0) {
//				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena49"));
//				return;
//			}
//
//			int cost = GameConfig.Instance.getInt (GameConfig.SID_ARENA_REFRESH_CHALLENGE_CD);
//			MessageWindowShowConfirm (LanguageConfigManager.Instance.getLanguage ("Arena36", cost.ToString ()), (msg) => {
//                
//				if (msg.msgEvent == msg_event.dialogOK) {
//					if (UserManager.Instance.self.getRMB () < cost) {
//						MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("s0158"), null);
//					} else {
//						ClearChallangeCD ();
//					}
//				}
//			});
//            
//		}
		else if (gameObj.name == "buttonUpdate") {
			OnButtonUpdateEnemyCick ();
		} else if (gameObj.name == "button_team") {
			UiManager.Instance.openWindow<TeamEditWindow> ((win)=>{
				win.comeFrom = TeamEditWindow.FROM_PVP;
			});
		} else if (gameObj.name == "buttonFinalAward") {
			UiManager.Instance.openWindow<ArenaAwardWindow> ((win) => {
				win.init (ArenaAwardWindow.TYPE_FINAL);
			});
		} else if (gameObj.name == "buttonIntegralAward") {
			UiManager.Instance.openDialogWindow<ArenaIntegralAwardWindow>((win)=>{
                win.callback = updateMetr;
				win.initUI();
			});
		} else if (gameObj.name == "buttonMeritShop") {
			UiManager.Instance.openWindow<MeritShopWindow> ();
		}
	}

	//刷新对手按钮事件调用
	void OnButtonUpdateEnemyCick ()
	{
		//if (ServerTimeKit.getSecondTime () - arenaManager.lastUpdateEnemyTime >= GameConfig.Instance.getInt (GameConfig.SID_ARENA_REFRESH_ENEMY_CD)) {
        if (CommonConfigSampleManager.Instance.getSampleBySid<ArenaNumsSample>(5).getTimesInt() - ArenaManager.instance.challengeLastUpdateTime > 0)
        {
			MessageWindowShowConfirm (LanguageConfigManager.Instance.getLanguage ("Arena28"), (msg) => {
				if (msg.msgEvent == msg_event.dialogOK) {
					FPortManager.Instance.getFPort<ArenaRefreshEnemyFPort> ().access (OnRefreshEnemyBack);
				}
			});
		} else {
			MessageWindowShowConfirm (LanguageConfigManager.Instance.getLanguage ("Arena29", "100"), (msg) => {
				if (msg.msgEvent == msg_event.dialogOK) {
					FPortManager.Instance.getFPort<ArenaRMBRefreshEnemyFPort> ().access (OnRefreshEnemyBack);
				}
			});
		}
	}

//	//清除挑战CD按钮事件调用
//	void ClearChallangeCD ()
//	{
//		FPortManager.Instance.getFPort<ArenaResetCDFPort> ().access ((result) => {
//			if (!result) {
//				MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("s0158"), null);
//			} else {
////				arenaManager.redChallengeCd = 0;
//				arenaManager.challengeUseTime = 0;
//				arenaManager.challengeLastUpdateTime = ServerTimeKit.getSecondTime ();
//			}
//		});
//	}
    
	void OnRefreshEnemyBack (string msg)
	{
		if (msg != null) {
			if (msg == "limit_rmb") {
				MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("s0158"), null);
			} else {
				MessageWindowShowAlert ("error", null);
			}
		} else {
			Utils.DestoryChilds (arenaRoles.root);
			OnInfoLoad ();
		}
	}

	void OnEnemyItemClick (GameObject obj)
	{

		int index = StringKit.toInt (obj.name);

		
		//oninfoload 没加载好也可能走这里!!!!!
		if( enemyList==null  || enemyList.Count<1 || index>=enemyList.Count)
			return;


		ArenaUserInfo user = enemyList [index];
		if (user.challengedWin) {
			MaskWindow.UnlockUI ();
			return;
		}
	    if (UserManager.Instance.self.getMerit() >= CommandConfigManager.Instance.getLimitOfMerit())
	    {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_029"));
            });
	        return;
	    }
	    //挑战次数用尽
		if (arenaManager.getChallengeCount () <= 0) {            
			addChalleageTimes();
			return;
		}
		//挑战时间冷却中，请稍后再来  去除CD限制
//		if (ArenaManager.instance.redChallengeCd != 0 && am.getChallengeCD () > 0) {
//			MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena10", GameConfig.Instance.getInt (GameConfig.SID_ARENA_FIGHT_CD_VIP).ToString ()), null);
//			return;
//		}
		if (!ArmyManager.Instance.checkArmyMemberCount ()) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI = false;
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0442"), LanguageConfigManager.Instance.getLanguage ("s0093"),
				                LanguageConfigManager.Instance.getLanguage ("MassPlayerWindow_MembersNotEnough"), (msg) => {
					if (msg.buttonID == MessageHandle.BUTTON_LEFT) 
						UiManager.Instance.openWindow<TeamEditWindow> ((wins)=>{
							wins.comeFrom = TeamEditWindow.FROM_PVP;
						});
					else if (msg.buttonID == MessageHandle.BUTTON_RIGHT)
						sureFight (user, index);
				});
			});
		} else
			sureFight (user, index);
	}

	private void sureFight (ArenaUserInfo user, int index)
	{		
	
		if (user.npc) {
			GameManager.Instance.battleReportCallback=	GameManager.Instance.intoBattleNoSwitchWindow;
			intoFight (user);

		} else {
			FPortManager.Instance.getFPort<MassGetPlayerInfoFPort> ().access (index + 1,PvpPlayerWindow.PVP_TEAM,PvpPlayerWindow.PVP_TEAM_TYPE, (info) => {
				UiManager.Instance.openWindow<MassPlayerWindow> ((win) => {
					win.teamType = 10;
					win.initInfo (info, () => {
						GameManager.Instance.battleReportCallback=	GameManager.Instance.intoBattle;
						intoFight (user);
					});
				});
			});
		}
	}

//	private void showTeamEditWindow (MessageHandle msg)
//	{
//		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
//			UiManager.Instance.openWindow<TeamEditWindow> ();
//		}
//	}

	void intoFight (ArenaUserInfo user)
	{

		FPortManager.Instance.getFPort<ArenaChallengeFport> ().access ((success) => {
			if (success) {
				MaskWindow.instance.setServerReportWait(true);
				IncAttributeGlobal.Instance.setAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_INTEGRAL, ArenaManager.instance.self == null ? 0 : ArenaManager.instance.self.integral);
				IncAttributeGlobal.Instance.setAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_ARENA_MERIT, UserManager.Instance.self.merit);
				ArenaManager.instance.currentMassEnemyUid = user.uid;

			} else {
				MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena40"), (msg) => {
					GameManager.Instance.battleReportCallback=null;
					MaskWindow.instance.setServerReportWait(false);
					finishWindow ();
				});
			}
		}, user.massPosition);
	}

	void MessageWindowShowAlert (string msg, CallBackMsg callback)
	{
		MessageWindow.ShowAlert (msg, (ev) => {
			if (callback != null)
				callback (ev);
		});
	}

	void MessageWindowShowConfirm (string msg, CallBackMsg callback)
	{
		MessageWindow.ShowConfirm (msg, (ev) => {
			if (callback != null)
				callback (ev);
		});
	}
}
