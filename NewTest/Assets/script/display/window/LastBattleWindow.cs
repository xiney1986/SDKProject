using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LastBattleWindow : WindowBase
{
	public UILabel processValue;// 备战进度描述//
	public UISprite processFront;// 进度//
	public UISprite processFrontTmp;// 进度(底下)//
	public UISprite bossHpFront;// boss血量条//
	public UISprite bossHpTmp;// 扣血块模板//
	public GameObject bossHpTmpObj;
	private TweenPosition bossHpTmpTweenPos;
	private Vector3 bossHpTmpPos;// 血块初始位置//
	public UILabel bossHp;//  boss血量//
	public UILabel processAddValue;// 进度增量//
	public UILabel hpReduceValue;// boss血量减量//
	public UILabel countdown;// 决战倒计时//
	public UILabel bossBattleCountDown;// boss阶段倒计时//
	public UILabel nvShenBlessLV;// 女神赐福等级//
	public UILabel nvShenBlessDesc;// 女神赐福描述//
	public UILabel fightTimes;// 战斗次数//
	public UILabel fightTimesDesc;// 战斗次数增加描述//
	public UILabel fightProcessValue;//  挑战小怪关卡进度//
	public UILabel donateCountdown;// 捐献倒计时//
	public GameObject processPrizeBtnTip;// 查看进度奖励按钮提示//
	public GameObject donateUpdateTip;// 物资捐献已刷新提示//
	public GameObject timeInfo;// 决战倒计时详情//
	public UILabel currentProcessValue;// 当前备战进度//
	public UILabel[] processBossDesc;// 进度开启boss弱点描述//

	public Transform[] processPrizesPos;
	public GoodsView goodsTmp;
	GameObject[] goodsObj;

	public UILabel processPrizeTittle;
	public GameObject processPrizeBtn;// 查看进度奖励按钮//
	public GameObject processInfoPanel;// 进度奖励界面//
	public GameObject closeBtn;
	public GameObject helpBtn;
	public GameObject buttonCloseHelp;
	public ButtonBase detailBtn;// boss弱点查看详情按钮//
	public GameObject detailTips;

	bool isUpdateBossHpBar = false;
	float newBossHp;
	bool isUpdateProcessBarTmp = false;// 是否刷新备战进度条Tmp//
	float newProcess;
	float m_time = 2f;// 两个进度条间隔播放时间//  
	float m_startTime;// 第一个进度条开始时间//
	float m_addValueTime = 3.2f;// 增量文字描述停留时间//
	float m_hpStartTime;// boss血条开始时间//
	float m_reduceHpValueTime = 1.5f;// 血量减少停留时间//

	Color processLabelColor;//  进度描述文字颜色//
	public GameObject closeProcessPanelBtn; //  关闭进度奖励界面按钮//
	LastBattleProcessPrizeSample cureentProcessPrize;// 当前进度奖励//
	int cureentProcessPrizeIndex;// 当前奖励下标//
	public ButtonBase awardBtn;// 领取进度奖按钮//
	public UILabel awardBtnLabel;// 领取进度奖按钮//

	public BoxCollider lastBattleWinCloseBtn;
	public BoxCollider lastBattleWinHelpBtn;
	public BoxCollider lastBattleWinProcessPrizeBtn;
	public BoxCollider lastBattleWinRankBtn;
	public BoxCollider lastBattleWinDonateBtn;
	public BoxCollider lastBattleWinBattleBtn;
	public BoxCollider lastBattleWinShopBtn;
	public BoxCollider lastBattleWinFightInfoBtn;
	public BoxCollider lastBattleWinKillBossDescBtn;
	public BoxCollider lastBattleWinDetailBtn;

	public GameObject buttonShop;// 军功商店按钮//
	public GameObject buttonRank;// 排名按钮//
	public GameObject buttonDonate;// 物资捐献按钮//
	public GameObject buttonBattle;// 挑战按钮//
	public GameObject buttonFightInfo;// 战报按钮//
	public GameObject buttonCloseFightInfo;// 关闭战报按钮//

	public UILabel stateWeakLabel;
	public UILabel physicalHarmLabel;
	public UILabel magicHarmLabel;

	public GameObject bossBattlePanelInfo;// 挑战boss界面//
	public GameObject preparePanelInfo;// 备战阶段界面//

	public ButtonBase killBossPrizeDescBtn;// 击杀boss查看奖励btn//
	public GameObject killBossPrizeDescInfo;// 击杀boss查看奖励界面//
	public GameObject descTmp;// 描述模版//
	public Transform descParent;// 描述父节点//
	public ButtonBase buttonCloseInfo;// 关闭击杀boss查看奖励界面//
	public UIGrid descGrid;
	GameObject[] descObjArr;

	public GameObject fightReportInfo;// 查看战报界面//
	public GameObject reportTmp;// 战报描述模板//
	GameObject[] reportObjArr; 
	public Transform reportParent;// 战报父节点//
	public UIGrid reportGrid;
	public UIPanel reportDescLabelPanel;
	Vector3 reportDescLabelPanelPos;

	public UILabel lastBattleNotOpenLabel;
	public GameObject notOpenPanel;

	public GameObject passPanelCloseBtn;
	public GameObject passBtn;
	public GameObject nextBtn;
	public UILabel passBtnLabel;
	public GameObject passPanel;

	public UILabel nvShenLvBossPanel;
	public UILabel propAddValue;
	public UILabel bossBattleCount;
	public UILabel bossBattleUpdateTime;
	
	public UITexture bossTexture;// boss图片//
	public UISprite bossNameSprite;// boss名称//

	//计时器
	private Timer timer;
	DateTime dt;//获取服务器时间
	int currentDayOfWeek;// 当前星期几//
	int currentTime;
	int dayOfWeek;// 活动在星期几开启//
	int activityOpenTime;//活动开启时间//
	int prepareEndTime;//备战结束时间//
	int activityEndTime;//活动开启时间//

	// 挑战小怪关卡数据//
	string[] fightProcessInfoStrs = new string[2];

	int nextUpdateBattleCountTime;
	int nextUpdateBossBattleCountTime;

	int updateProcessTime = 0;
	int updateBossTime = 0;

	public UIPlayTween tweenHelpPanel;
	public GameObject effectObj;
	public GameObject fireEffectObj;
	public GameObject[] fires;
	int fireCountdown = 0;// 火焰显示计数//

	bool isUpdatePrepare = true;// 未开启到准备阶段过度刷新//
	bool isUpdateBoss = true;// 准备阶段到boss阶段过度刷新//
	bool isUpdateAward = true;// boss阶段到领奖阶段直到活动结束过度刷新//
	bool isUpdateNotOpen = true;// 领奖阶段到未开启阶段过渡//

	bool canSwichBg = true;

	private int oldProcess;
	protected override void DoEnable ()
	{
		if(bossHpTmpPos == Vector3.zero)
			bossHpTmpPos = bossHpTmp.transform.localPosition;
		reportDescLabelPanelPos = reportDescLabelPanel.transform.localPosition;
		base.DoEnable ();
		//UiManager.Instance.backGround.switchBackGround("lastBattle_beiJing");
		dayOfWeek = CommandConfigManager.Instance.lastBattleData.dayOfWeek;// 活动在星期几开启//
		activityOpenTime = CommandConfigManager.Instance.lastBattleData.startTime;//活动开启时间//
		prepareEndTime = CommandConfigManager.Instance.lastBattleData.battlePrepareEndTime;//备战结束时间//
		activityEndTime = CommandConfigManager.Instance.lastBattleData.endTime;//活动开启时间//

		lastBattleNotOpenLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_NotOpenLabel2"),dayOfWeek + ("(" + TimeKit.timeTransform(activityOpenTime * 1000) +")"));


//		LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
//		init.lastBattleInitAccess(setTimer);
		setTimer();

	}
	void setTimer()
	{
		// 备战阶段//
		if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE && TimeKit.getWeekCHA(TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime()).DayOfWeek) == dayOfWeek)
		{
			updatePreparePanel();
			StartCoroutine(showProcess());
			// 更新进度按钮提示//
			showProcessPrizeBtnTips();
		}
		// boss战阶段//
		else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.BOSS && TimeKit.getWeekCHA(TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime()).DayOfWeek) == dayOfWeek)
		{
			UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
			showDetialBtnTips();
			updateBossBattlePanel();
		}
		// 挑战boss结束到活动当天24点//
		else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.AWARD)
		{
			// 更新进度按钮提示//
			showProcessPrizeBtnTips();
		}

		timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
		timer.addOnTimer(updateTime);
		timer.start();
	}

	void updateTime()
	{
		if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE)
		{
			updateProcessTime++;
			if(updateProcessTime >= CommandConfigManager.Instance.lastBattleData.processUpdateTime)
			{
				updateProcessTime = 0;
				LastBattleUpdateFPort init = FPortManager.Instance.getFPort ("LastBattleUpdateFPort") as LastBattleUpdateFPort;
				init.updateAccess(()=>{
					updatePreparePanel();
					//initBattlePrepareProcess();
					updateBattlePrepareProcess();
					// 更新进度按钮提示//
					showProcessPrizeBtnTips();
				},LastBattleUpdateType.PREPARE);
			}
		}
		if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.BOSS)
		{
			updateBossTime++;
			if(updateBossTime >= CommandConfigManager.Instance.lastBattleData.bossHpUpdateTime)
			{
				updateBossTime = 0;
				LastBattleUpdateFPort init = FPortManager.Instance.getFPort ("LastBattleUpdateFPort") as LastBattleUpdateFPort;
				init.updateAccess(()=>{
					showDetialBtnTips();
					updateBossBattlePanel();
				},LastBattleUpdateType.BOSS);
			}
		}

		dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		currentDayOfWeek = TimeKit.getWeekCHA(dt.DayOfWeek);
		if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE)
		{
			currentTime = ServerTimeKit.getCurrentSecond() - 181;
		}
		else 
		{
			currentTime = ServerTimeKit.getCurrentSecond();
		}
		if(currentDayOfWeek == dayOfWeek)
		{
			// 活动开始到备战结束//
			if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE)
			{
				setPreparePanel();
			}
			// 挑战boss 阶段//
			else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.BOSS)
			{
				setBossPanel();
			}
			// 挑战boss结束到活动当天24点//
			else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.AWARD)
			{
				setNotOpenPanel();
			}
			// 活动未开启//
			else
			{
				setNotOpenPanel();
				if(currentTime == activityOpenTime)
				{
					if(isUpdatePrepare)
					{
						isUpdatePrepare = false;
						LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
						init.lastBattleInitAccess(()=>{
							updatePreparePanel();
							updateBattlePrepareProcess();
							// 更新进度按钮提示//
							showProcessPrizeBtnTips();
							isUpdatePrepare = true;
						});
					}
				}
			}
		}
		else
		{
			// 活动未开启//
			setNotOpenPanel();
			if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.AWARD)
			{
				if(currentTime == 0)
				{
					if(isUpdateNotOpen)
					{
						isUpdateNotOpen = false;
						LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
						init.lastBattleInitAccess(()=>{
							setNotOpenPanel();
							showProcessPrizeBtnTips();
							isUpdateNotOpen = true;
						});
					}
				}
			}
		}
		// 刷新小怪挑战次数增加倒计时,捐献倒计时//
		if(preparePanelInfo.activeSelf)
		{
			updateBattleCountTime();
			updateDonationTime();
		}
		// 刷新boss挑战次数倒计时//
		if(bossBattlePanelInfo.activeSelf)
		{
			updateBossBattleCountTime();
		}
		// 刷新火焰显示//
		if(effectObj.activeSelf && fireEffectObj.activeSelf)
		{
			showFire(++fireCountdown);
		}

	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		if(timer != null)
		{
			timer.stop();
		}
		fireCountdown = 0;
		for(int i=0;i<fires.Length;i++)
		{
			if(!fires[i].activeSelf)
			{
				fires[i].SetActive(true);
			}
		}
		isUpdateProcessBarTmp = false;
		setBattlePrepareProcess();
	}

	protected override void begin ()
	{
		processLabelColor = processBossDesc[0].color;

		base.begin ();
		initProcessBossDesc();

		MaskWindow.UnlockUI ();
		canSwichBg = true;
		if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.BOSS)
		{
			UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		}
		else 
		{
			UiManager.Instance.backGround.switchBackGround("lastBattle_beiJing");
		}

	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		// 点击进入进度奖励界面//
		if(gameObj == processPrizeBtn)
		{
			boxColliderCtrl(false);
			updateProcessInfoPanel();

			MaskWindow.UnlockUI();
		}
		else if(gameObj == closeBtn)
		{
			finishWindow();
		}
		else if(gameObj == closeProcessPanelBtn)
		{
			// 更新进度按钮提示//
			if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE)
			{
				showProcessPrizeBtnTips();
			}
			else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.BOSS)
			{
				showDetialBtnTips();
			}
			else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.AWARD || LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.NOT_OPEN)
			{
				showProcessPrizeBtnTips();
			}

			closeProcessPanel();
			boxColliderCtrl(true);
			updateBossWeak();
		}
		// 领取进度奖励//
		else if(gameObj == awardBtn.gameObject)
		{
			processAward();
		}
		else if(gameObj == buttonShop)
		{
			UiManager.Instance.openWindow<LastBattleShopWindow>();
		}
		else if(gameObj == buttonRank)
		{
			UiManager.Instance.openWindow<LastBattleRankWindow>();
		}
		else if(gameObj == buttonDonate)
		{
            //if (LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.AWARD || LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.NOT_OPEN)
            //{
            //    MaskWindow.UnlockUI();
            //    return;
            //}
            clickButtonDonate();
		}
		else if(gameObj == buttonBattle)
		{
			if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.AWARD || LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.NOT_OPEN)
			{
				MaskWindow.UnlockUI();
				return;
			}
			if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE)
			{
				clickBattleBtnInPrepare();
			}
			else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.BOSS)
			{
				clickBattleBtnInBoss();
			}

		}
		// 点击boss弱点详情查看按钮//
		else if(gameObj == detailBtn.gameObject)
		{
			boxColliderCtrl(false);
			updateProcessInfoPanel();
			MaskWindow.UnlockUI();
		}
		// 点击查看刺杀boss奖励描述//
		else if(gameObj == killBossPrizeDescBtn.gameObject)
		{
			boxColliderCtrl(false);
			initKillBossPrizeDescPanel();
			MaskWindow.UnlockUI();
		}
		else if(gameObj == buttonCloseInfo.gameObject)
		{
			boxColliderCtrl(true);
			cleanKillBossPrizeDescPanel();
		}
		// 点击查看战报//
		else if(gameObj == buttonFightInfo)
		{
			boxColliderCtrl(false);
			LastBattleKillLogFPort fport = FPortManager.Instance.getFPort ("LastBattleKillLogFPort") as LastBattleKillLogFPort;
			fport.killLogAccess(()=>{
				initFightReportPanel();
				MaskWindow.UnlockUI ();
			});
		}
		else if(gameObj == buttonCloseFightInfo)
		{
			boxColliderCtrl(true);
			cleanFightReprotPanel();
			MaskWindow.UnlockUI ();
		}
		// 关闭跳关界面//
		else if(gameObj == passPanelCloseBtn)
		{
			boxColliderCtrl(true);
			passPanel.SetActive(false);
			passBtnLabel.text = "";
		}
		// 点击跳关按钮//
		else if(gameObj == passBtn)
		{
			intoBattleFightSkip();
		}
		// 点击下一关按钮//
		else if(gameObj == nextBtn)
		{
			intoBattleFight();
		}
		else if(gameObj == helpBtn)
		{
			boxColliderCtrl(false);
			tweenerMessageGroupIn(tweenHelpPanel);
		}
		else if(gameObj == buttonCloseHelp)
		{
			tweenerMessageGroupOut(tweenHelpPanel);
			boxColliderCtrl(true);
		}
	}

	void Update()
	{
		//  备战阶段//
		// 刷新备战进度条//
		if(isUpdateProcessBarTmp && preparePanelInfo.activeSelf)
		{
			updateProcessBarTmp();
			if(Time.time - m_startTime >= m_time)
			{
				updateProcessBar();
			}
		}
		if(preparePanelInfo.activeSelf && Time.time - m_startTime >= m_addValueTime && processAddValue.gameObject.activeSelf)
		{
			processAddValue.gameObject.SetActive(false);
		}
		if(PlayerPrefs.HasKey(LastBattleManagement.lastbattleDonationKey) && LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE)
		{
//			if(LastBattleManagement.Instance.isUpdateDonationList && PlayerPrefs.GetInt(LastBattleManagement.lastbattleDonationKey) == 1)
//			{
			if(PlayerPrefs.GetInt(LastBattleManagement.lastbattleDonationKey) == 1)
			{
				//LastBattleManagement.Instance.isUpdateDonationList = false;
				donateUpdateTip.SetActive(true);
			}
		}
		//  boss阶段//
		if(bossBattlePanelInfo.activeSelf)
		{
//			if(isUpdateBossHpBar)
//			{
//				updateBossHpBar();
//			}
			if(Time.time - m_hpStartTime >= m_reduceHpValueTime && hpReduceValue.gameObject.activeSelf)
			{
				hpReduceValue.gameObject.SetActive(false);
				deletBossHpBar();
			}
		}
	}

	public void initProcessBossDesc()
	{
		for(int i=0;i<LastBattleProcessPrizeConfigManager.Instance.processPrize.Count;i++)
		{
			processBossDesc[i].text = LastBattleProcessPrizeConfigManager.Instance.processPrize[i].name + ": " + LastBattleProcessPrizeConfigManager.Instance.processPrize[i].processDesc;
		}
	}

	// 刷新进度奖励界面//
	public void updateProcessInfoPanel()
	{
		currentProcessValue.text = LastBattleManagement.Instance.newProcess + "/" + CommandConfigManager.Instance.lastBattleData.totalProcess + " (" + LastBattleManagement.Instance.caculateProcess() + ")";
		for(int i=0;i<LastBattleManagement.Instance.getCurrentPrecessCount();i++)
		{
			processBossDesc[i].color = new Color(0.3568628f,0.9254902f,0.3843137f);//0.3568628    0.9254902    0.3843137
			processBossDesc[i].effectStyle = UILabel.Effect.Outline;
		}
		// 刷新进度奖励界面奖品栏//
		updateProcessPrize();

		processInfoPanel.SetActive(true);
	}
	// 关闭进度奖励界面//
	public void closeProcessPanel()
	{
		for(int i=0;i<LastBattleManagement.Instance.getCurrentPrecessCount();i++)
		{
			processBossDesc[i].color = processLabelColor;
			processBossDesc[i].effectStyle = UILabel.Effect.None;
		}
		cleanProcessPrizeObj();
		processInfoPanel.SetActive(false);
	}
	// 刷新进度奖励界面奖品栏//
	public void updateProcessPrize()
	{
		cleanProcessPrizeObj();

		for(int i=0;i<LastBattleProcessPrizeConfigManager.Instance.processPrize.Count;i++)
		{
			if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].state != LastBattleProcessPrizeState.RECEVIED)
			{
				cureentProcessPrize = LastBattleProcessPrizeConfigManager.Instance.processPrize[i];
				cureentProcessPrizeIndex = i;
				if(cureentProcessPrize.state == LastBattleProcessPrizeState.CAN_RECEVIE)
				{
					awardBtn.disableButton(false);
				}
				else
				{
					awardBtn.disableButton(true);
				}
				// 领取//
				awardBtnLabel.text = LanguageConfigManager.Instance.getLanguage("s0309");

				initProcessPrizeObj(cureentProcessPrize);
				return;
			}
		}
		// 全部领完的情况//
		cureentProcessPrize = LastBattleProcessPrizeConfigManager.Instance.processPrize[LastBattleProcessPrizeConfigManager.Instance.processPrize.Count - 1];
		cureentProcessPrizeIndex = LastBattleProcessPrizeConfigManager.Instance.processPrize.Count - 1;
		
		initProcessPrizeObj(cureentProcessPrize);
		// 已领取//
		awardBtnLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
		awardBtn.disableButton(true);
	}
	// 当领取成功当前奖励后，刷新到下一个进度奖励//
	public void updateNextProcessPrize()
	{
		if(LastBattleProcessPrizeConfigManager.Instance.getPrize(cureentProcessPrize.id) != null)
		{
			// 展示获得奖品//
			showGetPrize(cureentProcessPrize.prizes);
			// 改变当前领过奖成功状态//
			LastBattleProcessPrizeConfigManager.Instance.getPrize(cureentProcessPrize.id).state = LastBattleProcessPrizeState.RECEVIED;
		}
		cleanProcessPrizeObj();
		// 当前领取不是最后一个奖励//
		if(cureentProcessPrizeIndex != LastBattleProcessPrizeConfigManager.Instance.processPrize.Count - 1)
		{
			cureentProcessPrize = LastBattleProcessPrizeConfigManager.Instance.processPrize[cureentProcessPrizeIndex + 1];
			cureentProcessPrizeIndex += 1;
			// 领取//
			awardBtnLabel.text = LanguageConfigManager.Instance.getLanguage("s0309");
		}
		else 
		{
			//已领取//
			awardBtnLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
		}

		if(cureentProcessPrize.state == LastBattleProcessPrizeState.CAN_RECEVIE)
		{
			awardBtn.disableButton(false);
		}
		else
		{
			awardBtn.disableButton(true);
		}
		initProcessPrizeObj(cureentProcessPrize);
	}


	// 刷新备战进度详情//
	public void updateBattlePrepareProcess()
	{
		oldProcess = 0;
		if(PlayerPrefs.HasKey(LastBattleManagement.lastBattleOldProcessKey + UserManager.Instance.self.uid))
		{
			oldProcess = PlayerPrefs.GetInt(LastBattleManagement.lastBattleOldProcessKey + UserManager.Instance.self.uid);
			processFront.fillAmount = (float)oldProcess / CommandConfigManager.Instance.lastBattleData.totalProcess;
			processFrontTmp.fillAmount = (float)oldProcess / CommandConfigManager.Instance.lastBattleData.totalProcess;
		}

		processValue.text = LastBattleManagement.Instance.caculateProcess();
//		if(LastBattleManagement.Instance.newProcess - LastBattleManagement.Instance.oldProcess > 0 && LastBattleManagement.Instance.newProcess < CommandConfigManager.Instance.lastBattleData.totalProcess)
//		{
		if(LastBattleManagement.Instance.newProcess - oldProcess > 0 && LastBattleManagement.Instance.newProcess < CommandConfigManager.Instance.lastBattleData.totalProcess)
		{
			//processAddValue.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_ProcessAddValue"),(LastBattleManagement.Instance.newProcess - LastBattleManagement.Instance.oldProcess).ToString());
			processAddValue.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_ProcessAddValue"),(LastBattleManagement.Instance.newProcess - oldProcess).ToString());
			processAddValue.gameObject.SetActive(true);

			//LastBattleManagement.Instance.oldProcess = LastBattleManagement.Instance.newProcess;
			PlayerPrefs.SetInt(LastBattleManagement.lastBattleOldProcessKey + UserManager.Instance.self.uid,LastBattleManagement.Instance.newProcess);
			newProcess = (float)LastBattleManagement.Instance.newProcess / CommandConfigManager.Instance.lastBattleData.totalProcess;
			isUpdateProcessBarTmp = true;
			m_startTime = Time.time;
		}
	}
	// 设置备战进度//
	void setBattlePrepareProcess()
	{
		newProcess = (float)LastBattleManagement.Instance.newProcess / CommandConfigManager.Instance.lastBattleData.totalProcess;
		processFront.fillAmount = newProcess;
		processFrontTmp.fillAmount = newProcess;
		//LastBattleManagement.Instance.oldProcess = LastBattleManagement.Instance.newProcess;
		PlayerPrefs.SetInt(LastBattleManagement.lastBattleOldProcessKey + UserManager.Instance.self.uid,LastBattleManagement.Instance.newProcess);
	}

	void updateProcessBarTmp()
	{
		if (newProcess != processFrontTmp.fillAmount)
		{ 
			processFrontTmp.fillAmount = Mathf.Lerp (processFrontTmp.fillAmount, newProcess, Time.deltaTime); 
		} 
	}
	void updateProcessBar()
	{
		if (newProcess != processFront.fillAmount)
		{ 
			processFront.fillAmount = Mathf.Lerp (processFront.fillAmount, newProcess, Time.deltaTime); 
		} 
		else
		{
			isUpdateProcessBarTmp = false;
		}
	}

	void boxColliderCtrl(bool b)
	{
		lastBattleWinCloseBtn.enabled = b;
		lastBattleWinHelpBtn.enabled = b;
		lastBattleWinProcessPrizeBtn.enabled = b;
		lastBattleWinRankBtn.enabled = b;
		lastBattleWinDonateBtn.enabled = b;
		lastBattleWinBattleBtn.enabled = b;
		lastBattleWinShopBtn.enabled = b;
		lastBattleWinFightInfoBtn.enabled = b;
		lastBattleWinKillBossDescBtn.enabled = b;
		lastBattleWinDetailBtn.enabled = b;

		effectObj.SetActive(b);
	}

	void clickButtonDonate()
	{
		if(PlayerPrefs.HasKey(LastBattleManagement.lastbattleDonationKey))
		{
			PlayerPrefs.SetInt(LastBattleManagement.lastbattleDonationKey,0);
		}
        donateUpdateTip.SetActive(false);
		UiManager.Instance.openWindow<LastBattleDonationWindow>();
	}

	// 更新boss根据进度的弱点//
	void updateBossWeak()
	{
		stateWeakLabel.text = "";
		physicalHarmLabel.text = "";
		magicHarmLabel.text = "";

		LastBattleManagement.Instance.stateWeakList.Clear();
		LastBattleManagement.Instance.physicalHarmList.Clear();
		LastBattleManagement.Instance.magicHarmList.Clear();

		for(int i=0;i<LastBattleManagement.Instance.getCurrentPrecessCount();i++)
		{
			if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].processType == LastBattleProcessType.STATE_WEAK)
			{
				LastBattleManagement.Instance.stateWeakList.Add(LastBattleProcessPrizeConfigManager.Instance.processPrize[i]);
			}
			else if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].processType == LastBattleProcessType.PHYSICAL_HARM)
			{
				LastBattleManagement.Instance.physicalHarmList.Add(LastBattleProcessPrizeConfigManager.Instance.processPrize[i]);
			}
			else if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].processType == LastBattleProcessType.MAGIC_HARM)
			{
				LastBattleManagement.Instance.magicHarmList.Add(LastBattleProcessPrizeConfigManager.Instance.processPrize[i]);
			}
			else if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].processType == LastBattleProcessType.MIX)
			{
				if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].harmDesDic != null && LastBattleProcessPrizeConfigManager.Instance.processPrize[i].harmValueDic != null)
				{
					foreach (KeyValuePair<int,string> item in LastBattleProcessPrizeConfigManager.Instance.processPrize[i].harmValueDic)
					{
						if(item.Key == LastBattleProcessType.STATE_WEAK)
						{
							LastBattleManagement.Instance.stateWeakList.Add(new LastBattleProcessPrizeSample(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].harmValueDic[item.Key],item.Value));
						}
						else if(item.Key == LastBattleProcessType.PHYSICAL_HARM)
						{
							LastBattleManagement.Instance.physicalHarmList.Add(new LastBattleProcessPrizeSample(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].harmValueDic[item.Key],item.Value));
						}
						else if(item.Key == LastBattleProcessType.MAGIC_HARM)
						{
							LastBattleManagement.Instance.magicHarmList.Add(new LastBattleProcessPrizeSample(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].harmValueDic[item.Key],item.Value));
						}
					}
				}
			}
		}

		updateStateWeakLabel(LastBattleManagement.Instance.stateWeakList);
		updatePhysicalHarmLabel(LastBattleManagement.Instance.physicalHarmList);
		updateMagicHarmLabel(LastBattleManagement.Instance.magicHarmList);
	}
	// 刷新boss状态弱点//
	void updateStateWeakLabel(List<LastBattleProcessPrizeSample> sample)
	{
		if(sample.Count <= 0)
		{
			stateWeakLabel.text = LanguageConfigManager.Instance.getLanguage("LastBattle_None");
		}
		else
		{
			for(int i=0;i<sample.Count;i++)
			{
				if(i == sample.Count - 1)
				{
					stateWeakLabel.text += sample[i].harmValue;
				}
				else
				{
					stateWeakLabel.text += sample[i].harmValue + ",";
				}
			}
		}
	}
	// 刷新boss物理易伤//
	void updatePhysicalHarmLabel(List<LastBattleProcessPrizeSample> sample)
	{
		if(sample.Count <= 0)
		{
			physicalHarmLabel.text = LanguageConfigManager.Instance.getLanguage("LastBattle_None");
		}
		else
		{
			int count = 0;
			for(int i=0;i<sample.Count;i++)
			{
				count += StringKit.toInt(sample[i].harmValue);
			}
			physicalHarmLabel.text = count + "%";
		}
	}
	// 刷新boss魔法易伤//
	void updateMagicHarmLabel(List<LastBattleProcessPrizeSample> sample)
	{
		if(sample.Count <= 0)
		{
			magicHarmLabel.text = LanguageConfigManager.Instance.getLanguage("LastBattle_None");
		}
		else
		{
			int count = 0;
			for(int i=0;i<sample.Count;i++)
			{
				count += StringKit.toInt(sample[i].harmValue);
			}
			magicHarmLabel.text = count + "%";
		}
	}
	// 初始化查看击杀boss奖励描述界面//
	void initKillBossPrizeDescPanel()
	{
		GameObject descObj;
		//int killBossCount = LastBattleManagement.Instance.killBossCount;
		//int prizeCount = LastBattleKillBossDescConfigManager.Instance.getPrizeCountByKillBossCount(killBossCount);
		List<LastBattleKillBossDesc> descList = LastBattleKillBossDescConfigManager.Instance.descList;
		if(descList != null)
		{
			if(descObjArr == null)
			{
				descObjArr = new GameObject[descList.Count];
			}

			for(int i =0;i<descList.Count;i++)
			{
				descObj = GameObject.Instantiate(descTmp) as GameObject;
				descObj.transform.parent = descParent;
				descObj.transform.localPosition = Vector3.zero;
				descObj.transform.localScale = Vector3.one;
				descObj.GetComponent<UILabel>().text = descList[i].tittle + "\n";
				if(descList[i].prizeDesc.Length > 0)
				{
					for(int j=0;j<descList[i].prizeDesc.Length;j++)
					{
						if(j != descList[i].prizeDesc.Length - 1)
						{
							descObj.GetComponent<UILabel>().text += descList[i].prizeDesc[j] + "  ";
						}
						else
						{
							descObj.GetComponent<UILabel>().text += descList[i].prizeDesc[j];
						}
					}
				}
				descObj.SetActive(true);
				descObjArr[i] = descObj;
			}
			descGrid.repositionNow = true;
		}
		killBossPrizeDescInfo.SetActive(true);
	}
	// 清理击杀boss奖励描述界面//
	void cleanKillBossPrizeDescPanel()
	{
		if(descObjArr != null)
		{
			for(int i=0;i<descObjArr.Length;i++)
			{
				GameObject.Destroy(descObjArr[i]);
			}
			descObjArr = null;
		}
		killBossPrizeDescInfo.SetActive(false);
	}
	// 初始化战报界面//
	void initFightReportPanel()
	{
		string[] strArr = new string[5];
		GameObject reportObj;
		List<LastBattleKillBossData> datas = LastBattleManagement.Instance.killBossDatas;
		if(datas != null && datas.Count > 0)
		{
			reportObjArr = new GameObject[datas.Count];
			for(int i=0;i<datas.Count;i++)
			{
				reportObj = GameObject.Instantiate(reportTmp) as GameObject;
				reportObjArr[i] = reportObj;
				reportObj.transform.parent = reportParent;
				reportObj.transform.localPosition = Vector3.zero;
				reportObj.transform.localScale = Vector3.one;
				strArr[0] = datas[i].killBossTime;
				strArr[1] = datas[i].playerName;
				strArr[2] = datas[i].bossName;
				strArr[3] = CommandConfigManager.Instance.lastBattleData.bossBattleFinalKillAardJunGong.ToString();
				//strArr[4] = LastBattleManagement.Instance.killBossCount.ToString();
				strArr[4] = datas[i].killBossCount;
				reportObj.GetComponent<UILabel>().text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_Report"),strArr);
				reportObj.SetActive(true);
			}
			reportGrid.repositionNow = true;
		}
		fightReportInfo.SetActive(true);
	}
	// 清理战报界面//
	void cleanFightReprotPanel()
	{
		reportDescLabelPanel.transform.localPosition = reportDescLabelPanelPos;
		reportDescLabelPanel.clipOffset = Vector2.zero;
		fightReportInfo.SetActive(false);
		if(reportObjArr != null)
		{
			for(int i=0;i<reportObjArr.Length;i++)
			{
				GameObject.Destroy(reportObjArr[i]);
			}
			reportObjArr = null;
		}
	}
	// 设置活动未开启界面//
	void setNotOpenPanel()
	{
		//UiManager.Instance.backGround.switchBackGround("lastBattle_beiJing");
		lastBattleWinBattleBtn.enabled = false;
		//lastBattleWinDonateBtn.enabled = false;
		lastBattleWinFightInfoBtn.enabled = false;
		if(bossBattlePanelInfo.activeSelf && buttonFightInfo.activeSelf)
		{
			buttonFightInfo.SetActive(false);
			bossBattlePanelInfo.SetActive(false);
		}
		if(!preparePanelInfo.activeSelf && !buttonDonate.activeSelf)
		{
			//UiManager.Instance.backGround.switchBackGround("lastBattle_beiJing");
			buttonDonate.SetActive(true);
			preparePanelInfo.SetActive(true);
		}
		if(!notOpenPanel.activeSelf)
		{
			notOpenPanel.SetActive(true);
		}
		if(fireEffectObj.activeSelf)
		{
			fireEffectObj.SetActive(false);
		}
		processValue.text = "0%";
		countdown.text = "00:00:00";
		processFront.fillAmount = 0;
		processFrontTmp.fillAmount = 0;
	}
	// 设置备战阶段界面//
	void setPreparePanel()
	{
		//UiManager.Instance.backGround.switchBackGround("lastBattle_beiJing");
		if(prepareEndTime > currentTime)
		{
			countdown.text = TimeKit.timeTransform (prepareEndTime * 1000 - currentTime * 1000);
		}
		else 
		{
			countdown.text = "00:00:00";
			if(isUpdateBoss)
			{
				isUpdateBoss = false;
				LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
				init.lastBattleInitAccess(()=>{
					UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
					showDetialBtnTips();
					updateBossBattlePanel();
					isUpdateBoss = true;
				}); 
			}
		}
		if(bossBattlePanelInfo.activeSelf && buttonFightInfo.activeSelf)
		{
			buttonFightInfo.SetActive(false);
			bossBattlePanelInfo.SetActive(false);
		}
		if(notOpenPanel.activeSelf)
		{
			notOpenPanel.SetActive(false);
		}
		if(!preparePanelInfo.activeSelf && !buttonDonate.activeSelf)
		{
//			updatePreparePanel();
//			initBattlePrepareProcess();
			//updateBattlePrepareProcess();
			//UiManager.Instance.backGround.switchBackGround("lastBattle_beiJing");
			buttonDonate.SetActive(true);
			preparePanelInfo.SetActive(true);
			lastBattleWinDonateBtn.enabled = true;
			lastBattleWinBattleBtn.enabled = true;
		}
		if(!fireEffectObj.activeSelf)
		{
			fireEffectObj.SetActive(true);
		}
	}
	// 设置boss战界面//
	void setBossPanel()
	{
		if(canSwichBg)
		{
			//UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		}
		if(activityEndTime > currentTime)
		{
			bossBattleCountDown.text = TimeKit.timeTransform (activityEndTime * 1000 - currentTime * 1000);
		}
		if(activityEndTime <= currentTime)
		{
			bossBattleCountDown.text = "00:00:00";
			if(isUpdateAward)
			{
				isUpdateAward = false;
				LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
				init.lastBattleInitAccess(()=>{
					setNotOpenPanel();
					showProcessPrizeBtnTips();
					isUpdateAward = true;
				});
			}
		}
		if(preparePanelInfo.activeSelf && buttonDonate.activeSelf)
		{
            buttonDonate.SetActive(false);
            preparePanelInfo.SetActive(false);
		}
		if(!bossBattlePanelInfo.activeSelf && !buttonFightInfo.activeSelf)
		{
			//updateBossBattlePanel();
			//UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
			buttonFightInfo.SetActive(true);
			bossBattlePanelInfo.SetActive(true);
			lastBattleWinFightInfoBtn.enabled = true;
			lastBattleWinBattleBtn.enabled = true;
		}
	}
	// 初始化跳关界面//
	void initPassPanel()
	{
		passBtnLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_PassBtnLabel"),
		                                  (LastBattleManagement.Instance.battleID + CommandConfigManager.Instance.lastBattleData.battleSkipCount).ToString());
		boxColliderCtrl(false);
		passPanel.SetActive(true);
		MaskWindow.UnlockUI();
	}

	// 更新备战界面//
	void updatePreparePanel()
	{
		nvShenBlessLV.text = "LV " + LastBattleManagement.Instance.nvBlessLv;
		nvShenBlessDesc.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_NvShenBlessDesc"),LastBattleManagement.Instance.nvBlessLv);
		// 挑战次数//
		fightTimes.text = LastBattleManagement.Instance.battleCount + "/" + CommandConfigManager.Instance.lastBattleData.battleTotalCount;
		// 小怪挑战数据//
		fightProcessInfoStrs[0] = LastBattleManagement.Instance.battleID.ToString();
		fightProcessInfoStrs[1] = LastBattleManagement.Instance.battleWinCount.ToString();
		fightProcessValue.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_FightProcess"),fightProcessInfoStrs);
	}

	int addCount = 0;
	// 刷新小怪挑战次数倒计时//
	void updateBattleCountTime()
	{
		if(nextUpdateBattleCountTime == ServerTimeKit.getCurrentSecond())
		{
			addCount = CommandConfigManager.Instance.lastBattleData.battleAddTime + LastBattleManagement.Instance.battleCount;
			if(addCount >= CommandConfigManager.Instance.lastBattleData.battleTotalCount)
			{
				fightTimes.text =  CommandConfigManager.Instance.lastBattleData.battleTotalCount + "/" + CommandConfigManager.Instance.lastBattleData.battleTotalCount;
			}
			else
			{
				fightTimes.text =  addCount + "/" + CommandConfigManager.Instance.lastBattleData.battleTotalCount;
			}
		}
		nextUpdateBattleCountTime = LastBattleManagement.Instance.getNextUpdateBattleCountTime(ServerTimeKit.getCurrentSecond()); 
		if(nextUpdateBattleCountTime == 0)
		{
			fightTimesDesc.text = "";
		}
		else
		{
			fightTimesDesc.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_TimeDesc"),TimeKit.timeTransform (nextUpdateBattleCountTime * 1000 - ServerTimeKit.getCurrentSecond() * 1000));
		}
	}
	// 刷新捐献时间倒计时//
	void updateDonationTime()
	{
		if(LastBattleManagement.Instance.donationNextUpdateTime < 0)
		{
			donateCountdown.text = LanguageConfigManager.Instance.getLanguage("LastBattle_RefreshEnd");
			return;
		}
		if(LastBattleManagement.Instance.donationNextUpdateTime - ServerTimeKit.getSecondTime() <= 0)
		{
			donateCountdown.text = "00:00:00";
		}
		else
		{
			donateCountdown.text = TimeKit.timeTransform (LastBattleManagement.Instance.donationNextUpdateTime * 1000 - ServerTimeKit.getSecondTime() * 1000);
		}
	}
	
	// 更新boss战界面//
	public void updateBossBattlePanel()
	{
		if(LastBattleManagement.Instance.getBossInfo(LastBattleManagement.Instance.bossID) != null)
		{
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + LastBattleManagement.Instance.getBossInfo(LastBattleManagement.Instance.bossID).imageID, bossTexture);
			//bossNameSprite.spriteName = "bossName_" + LastBattleManagement.Instance.getBossInfo(LastBattleManagement.Instance.bossID).nameID;
		}
		else
		{
			//bossNameSprite.spriteName = "None";
		}

		StartCoroutine(showBossHp());
		updateBossWeak();
		nvShenLvBossPanel.text = "LV." + LastBattleManagement.Instance.nvBlessLv.ToString();
		propAddValue.text = "+" + LastBattleManagement.Instance.nvBlessLv + "%";
		bossBattleCount.text = LastBattleManagement.Instance.bossCount + "/" + CommandConfigManager.Instance.lastBattleData.bossBattleTotalCount;

	}
	long oldHp;
	// 设置boss血量//
	void setBossHp()
	{
		oldHp = 0;
		if(LastBattleManagement.Instance.currentBossHP == LastBattleManagement.Instance.bossTotalHP)
		{
			bossHpFront.fillAmount = 1;
			initDeletHpTmp(LastBattleManagement.Instance.currentBossHP,LastBattleManagement.Instance.bossTotalHP);
		}
		else 
		{
			if(PlayerPrefs.HasKey(LastBattleManagement.lastBattleBossIDKey + UserManager.Instance.self.uid))
			{
				if(PlayerPrefs.GetInt(LastBattleManagement.lastBattleBossIDKey + UserManager.Instance.self.uid) == LastBattleManagement.Instance.bossID)
				{
					if(PlayerPrefs.HasKey(LastBattleManagement.lastBattleBossHpKey + UserManager.Instance.self.uid))
					{
						oldHp = StringKit.toLong(PlayerPrefs.GetString(LastBattleManagement.lastBattleBossHpKey + UserManager.Instance.self.uid));
						bossHpFront.fillAmount = (float)oldHp / LastBattleManagement.Instance.bossTotalHP;
						initDeletHpTmp(LastBattleManagement.Instance.currentBossHP,oldHp);
					}
					else 
					{
						bossHpFront.fillAmount = 1;
						initDeletHpTmp(LastBattleManagement.Instance.currentBossHP,LastBattleManagement.Instance.bossTotalHP);
					}
				}
				else
				{
					bossHpFront.fillAmount = 1;
					initDeletHpTmp(LastBattleManagement.Instance.currentBossHP,LastBattleManagement.Instance.bossTotalHP);
				}
			}
			else 
			{
				bossHpFront.fillAmount = 1;
				initDeletHpTmp(LastBattleManagement.Instance.currentBossHP,LastBattleManagement.Instance.bossTotalHP);
			}

			if(oldHp != 0)
			{
				if(oldHp > LastBattleManagement.Instance.currentBossHP)
				{
					hpReduceValue.text = (LastBattleManagement.Instance.currentBossHP - oldHp).ToString();
					hpReduceValue.gameObject.SetActive(true);
				}
			}
			else 
			{
				if(LastBattleManagement.Instance.bossTotalHP > LastBattleManagement.Instance.currentBossHP)
				{
					hpReduceValue.text = (LastBattleManagement.Instance.currentBossHP - LastBattleManagement.Instance.bossTotalHP).ToString();
					hpReduceValue.gameObject.SetActive(true);
				}
			}
		}
		newBossHp = (float)LastBattleManagement.Instance.currentBossHP / LastBattleManagement.Instance.bossTotalHP;
		bossHp.text = LastBattleManagement.Instance.currentBossHP + "/" + LastBattleManagement.Instance.bossTotalHP;
		bossHpFront.fillAmount = (float)LastBattleManagement.Instance.currentBossHP / LastBattleManagement.Instance.bossTotalHP;
		
		PlayerPrefs.SetInt(LastBattleManagement.lastBattleBossIDKey + UserManager.Instance.self.uid , LastBattleManagement.Instance.bossID);
		PlayerPrefs.SetString(LastBattleManagement.lastBattleBossHpKey + UserManager.Instance.self.uid , LastBattleManagement.Instance.currentBossHP.ToString());
		
		//isUpdateBossHpBar = true;
		m_hpStartTime = Time.time;
	}
	// 扣血块//
	void deletBossHpBar()
	{
		if(deletObj != null)
		{
			bossHpTmpTweenPos = deletObj.AddComponent<TweenPosition>();
			bossHpTmpTweenPos.enabled = false;
			bossHpTmpTweenPos.from = deletObj.transform.localPosition;
			bossHpTmpTweenPos.to = new Vector3(deletObj.transform.localPosition.x + CommandConfigManager.Instance.getLastBattleXDistance(),deletObj.transform.localPosition.y - CommandConfigManager.Instance.getLastBattleYDistance(),deletObj.transform.localPosition.z);
			bossHpTmpTweenPos.duration = CommandConfigManager.Instance.getLastBattleHpDownTime();
			EventDelegate.Add(bossHpTmpTweenPos.onFinished,deletBossHpCallBack);
			bossHpTmpTweenPos.enabled = true;
		}
	}
	void deletBossHpCallBack()
	{
		if(deletObj != null)
		{
			Destroy(deletObj);
			deletObj = null;
		}
	}
	void updateBossHpBar()
	{
		if(newBossHp != bossHpFront.fillAmount)
		{
			bossHpFront.fillAmount = Mathf.Lerp (bossHpFront.fillAmount, newBossHp, Time.deltaTime);
		}
		else
		{
			isUpdateBossHpBar = false;
		}
	}
	// 初始化扣血块//
	float deletHpWidth;
	float x_distance;
	GameObject deletObj;
	void initDeletHpTmp(long currentHp,long oldHp)
	{
		if(oldHp >= currentHp)
		{
			x_distance = float.Parse(((float)(LastBattleManagement.Instance.bossTotalHP - oldHp)/LastBattleManagement.Instance.bossTotalHP).ToString("0.0")) * bossHpFront.width;
			deletHpWidth = float.Parse(((float)(oldHp - currentHp)/LastBattleManagement.Instance.bossTotalHP).ToString("0.0")) * bossHpFront.width;
			if(deletHpWidth != 0)
			{
				deletObj = GameObject.Instantiate(bossHpTmpObj) as GameObject;
				deletObj.GetComponent<UISprite>().width = (int)deletHpWidth;
				deletObj.transform.parent = bossHpTmpObj.transform.parent;
				deletObj.transform.localScale = Vector3.one;
				deletObj.transform.localPosition = new Vector3(bossHpTmpPos.x - x_distance,bossHpTmpPos.y,bossHpTmpPos.z);
				deletObj.SetActive(true);
			}
		}
	}
	int bossBattleCountValue = 0;
	// 刷新boss挑战次数倒计时//
	void updateBossBattleCountTime()
	{
		if(LastBattleManagement.Instance.nextBossCountUpdateTime != 0)
		{
			if(LastBattleManagement.Instance.nextBossCountUpdateTime == ServerTimeKit.getSecondTime())
			{
				bossBattleCountValue = LastBattleManagement.Instance.bossCount++;
				if(bossBattleCountValue >= CommandConfigManager.Instance.lastBattleData.bossBattleTotalCount)
				{
					bossBattleCount.text = CommandConfigManager.Instance.lastBattleData.bossBattleTotalCount + "/" + CommandConfigManager.Instance.lastBattleData.bossBattleTotalCount;
				}
				else
				{
					bossBattleCount.text = bossBattleCountValue + "/" + CommandConfigManager.Instance.lastBattleData.bossBattleTotalCount;
				}
			}
			if(LastBattleManagement.Instance.nextBossCountUpdateTime - ServerTimeKit.getSecondTime() <= 0)
			{
				LastBattleManagement.Instance.nextBossCountUpdateTime += CommandConfigManager.Instance.lastBattleData.bossBattleCountUpdateTime; 
			}
			else
			{
				bossBattleUpdateTime.text = "(" + TimeKit.timeTransformDHMS (LastBattleManagement.Instance.nextBossCountUpdateTime - ServerTimeKit.getSecondTime()) + ")";
			}
		}
		else 
		{
			if(bossBattleUpdateTime.text != "")
			{
				bossBattleUpdateTime.text = "";
			}
		}
	}
	// 进入挑战小怪下一关副本//
	void intoBattleFight()
	{
		LastBattleFightFPort fPort = FPortManager.Instance.getFPort ("LastBattleFightFPort") as LastBattleFightFPort;
		fPort.lastBattleFightAccess(fightReport,LastBattleManagement.Instance.battleID + 1);
	}
	// 进入挑战小怪跳关后副本//
	void intoBattleFightSkip()
	{
		LastBattleFightFPort fPort = FPortManager.Instance.getFPort ("LastBattleFightFPort") as LastBattleFightFPort;
		fPort.lastBattleFightAccess(fightReport,LastBattleManagement.Instance.battleID + CommandConfigManager.Instance.lastBattleData.battleSkipCount);
	}
	// 战报//
	void fightReport()
	{
		canSwichBg = false;
		boxColliderCtrl(true);
		passPanel.SetActive(false);
		MaskWindow.instance.setServerReportWait(true);
		GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
	}
	// 备战阶段点击挑战//
	void clickBattleBtnInPrepare()
	{
		if(LastBattleManagement.Instance.battleCount <= 0)
		{
			// 飘字提示，挑战次数不足//
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("LastBattle_times_limit"));
			});
			return;
		}
		// 提示军功已满//
		if(LastBattleManagement.Instance.creatJunGongMaxTip())
			return;
		// 连胜次数到3//
		if(LastBattleManagement.Instance.battleWinCount == CommandConfigManager.Instance.lastBattleData.battleWinCount)
		{
			initPassPanel();
		}
		else
		{
			// 进入战斗//
			intoBattleFight();
		}
	}
	// boss战阶段点击挑战//
	void clickBattleBtnInBoss()
	{
		if(LastBattleManagement.Instance.bossCount <= 0)
		{
			// 飘字提示，挑战次数不足//
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("LastBattle_times_limit"));
			});
			return;
		}
		// 提示军功已满//
		if(LastBattleManagement.Instance.creatJunGongMaxTip())
			return;
		intoBossBattle();
	}
	// 进入挑战boss//
	void intoBossBattle()
	{
		LastBattleBossBattleFPort fPort = FPortManager.Instance.getFPort ("LastBattleBossBattleFPort") as LastBattleBossBattleFPort;
		fPort.lastBattleBossBattleAccess(fightReport);
	}
	// 领取进度奖//
	void processAward()
	{
		if(cureentProcessPrize != null)
		{
			if(!isPropStorageFull(cureentProcessPrize.prizes))// 仓库未满//
			{
				MaskWindow.LockUI ();
				LastBattleProcessAwardFPort fPort = FPortManager.Instance.getFPort ("LastBattleProcessAwardFPort") as LastBattleProcessAwardFPort;
				fPort.lastBattleAwardtAccess(updateNextProcessPrize,cureentProcessPrize.id);
			}
			else
			{
				// 飘字提示，仓库已满请清理//
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull"));
				});
			}
		}
	}
	// 领奖飘字//
	void showGetPrize(PrizeSample[] prizes)
	{
		UiManager.Instance.createPrizeMessageLintWindow(prizes);
		// 是否有英雄之章展示//
		for(int i=0;i<prizes.Length;i++)
		{
			if(prizes[i].type == PrizeType.PRIZE_CARD)
			{
				Card card = CardManagerment.Instance.createCard(prizes[i].pSid);
				if(card != null)
				{
					if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed(card)) {
						StartCoroutine(Utils.DelayRun(() => {
							UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
								win.init(LanguageConfigManager.Instance.getLanguage("s0418"), 0.8f);
							});
						},0.7f));
					}
				}
			}
		}
	}
	// 刷新进度奖励按钮角标tips //
	void showProcessPrizeBtnTips()
	{
		for(int i=0;i<LastBattleProcessPrizeConfigManager.Instance.processPrize.Count;i++)
		{
			if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].state == LastBattleProcessPrizeState.CAN_RECEVIE)
			{
				processPrizeBtnTip.SetActive(true);
				return;
			}
		}
		processPrizeBtnTip.SetActive(false);
	}
	// 备战阶段详情按钮tips//
	public void showDetialBtnTips()
	{
		for(int i=0;i<LastBattleProcessPrizeConfigManager.Instance.processPrize.Count;i++)
		{
			if(LastBattleProcessPrizeConfigManager.Instance.processPrize[i].state == LastBattleProcessPrizeState.CAN_RECEVIE)
			{
				detailTips.SetActive(true);
				return;
			}
		}
		detailTips.SetActive(false);
	}
	//验证相关仓库是否满
	private bool isPropStorageFull (PrizeSample[] propArr)
	{
		PrizeSample prop;
		bool isfull = false;
		for(int i=0;i<propArr.Length;i++)
		{
			prop = propArr[i];
			if (prop == null)
				return false;
			switch (prop.type) {
			case PrizeType.PRIZE_CARD:
				if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllRole ().Count > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_BEAST:
				if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllBeast ().Count > StorageManagerment.Instance.getBeastStorageMaxSpace ()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_EQUIPMENT:
				if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllEquip ().Count > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_MAGIC_WEAPON:
				if (prop.getPrizeNumByInt() + StorageManagerment.Instance.getAllMagicWeapon().Count > StorageManagerment.Instance.getMagicWeaponStorageMaxSpace()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_PROP:
				if (StorageManagerment.Instance.getProp (prop.pSid) != null) {
					isfull = false;
				} else {
					if (1 + StorageManagerment.Instance.getAllProp ().Count > StorageManagerment.Instance.getPropStorageMaxSpace ()) {
						isfull = true;
					} else {
						isfull = false;
					}
				}
				break;
			}
			return isfull;
		}
		return isfull;		
	}

	public void tweenerMessageGroupIn (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Forward;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0.2f;
		}
		tween.Play (true);
	}

	public void tweenerMessageGroupOut (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Reverse;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0;
		}
		tween.Play (true);
	}

	void showFire(int countdown)
	{
		if(countdown == 3)
		{
			fires[0].SetActive(false);
		}
		else if(countdown == 4)
		{
			fires[1].SetActive(false);
		}
		else if(countdown == 5)
		{
			fires[2].SetActive(false);
		}
		else if(countdown == 6)
		{
			fires[3].SetActive(false);
		}
		else if(countdown == 7)
		{
			fires[4].SetActive(false);
		}
		else if(countdown == 8)
		{
			fires[5].SetActive(false);
		}
		else if(countdown == 9)
		{
			fires[6].SetActive(false);
		}
		else if(countdown == 10)
		{
			fires[7].SetActive(false);
		}
		else if(countdown == 11)
		{
			fires[8].SetActive(false);
		}
		else if(countdown == 12)
		{
			fires[9].SetActive(false);
		}
		else if(countdown == 13)
		{
			fires[10].SetActive(false);
		}
		else if(countdown == 14)
		{
			fires[11].SetActive(false);
			StartCoroutine(fireOn());
		}
	}

	IEnumerator fireOn()
	{
		yield return new WaitForSeconds(1);
		for(int i=0;i<fires.Length;i++)
		{
			fires[i].SetActive(true);
		}
		fireCountdown = 0;
	}
	// 断线重连//
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		cleanUI();
		boxColliderCtrl(true);
		LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
		init.lastBattleInitAccess(()=>{
			if(timer != null)
			{
				timer.stop();
			}
			setTimer();
		});
	}
	void cleanUI()
	{
		notOpenPanel.SetActive(false);
		preparePanelInfo.SetActive(false);
		bossBattlePanelInfo.SetActive(false);
		passPanel.SetActive(false);
		processInfoPanel.SetActive(false);
        buttonDonate.SetActive(false);
        buttonFightInfo.SetActive(false);
	}
	void cleanProcessPrizeObj()
	{
		if(goodsObj != null && goodsObj.Length > 0)
		{
			for(int i=0;i<goodsObj.Length;i++)
			{
				GameObject.Destroy(goodsObj[i]);
			}
		}
	}
	void initProcessPrizeObj(LastBattleProcessPrizeSample cureentProcessPrize)
	{
		goodsObj = new GameObject[cureentProcessPrize.prizes.Length];
		for(int j=0;j<cureentProcessPrize.prizes.Length;j++)
		{
			goodsObj[j] = GameObject.Instantiate(goodsTmp.gameObject) as GameObject;
			goodsObj[j].transform.parent = processPrizesPos[j].parent;
			goodsObj[j].transform.localPosition = processPrizesPos[j].localPosition;
			goodsObj[j].transform.localScale = Vector3.one;
			goodsObj[j].GetComponent<GoodsView>().init(cureentProcessPrize.prizes[j]);
			goodsObj[j].GetComponent<GoodsView>().fatherWindow = this;
		}
		processPrizeTittle.text = cureentProcessPrize.name + LanguageConfigManager.Instance.getLanguage("LastBattle_ProcessBossDesc1");
	}

	IEnumerator showProcess()
	{
		yield return new WaitForSeconds(1);
		updateBattlePrepareProcess();
	}
			
	IEnumerator showBossHp()
	{
		yield return new WaitForSeconds(1);
		setBossHp();
	}



}

