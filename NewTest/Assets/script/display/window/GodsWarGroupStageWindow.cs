using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 诸神之战分组赛窗口
/// gc
/// </summary>
public class GodsWarGroupStageWindow : WindowBase
{

	/** 挑战次数 */
	public UILabel lblTimes;
	/** 挑战时间 */
	public UILabel lblCD; 
	/** 我的排名 */
	public UILabel lblRank; 
	/** 我的组 */
	public UILabel lblMyTeam; 
	/** 今日积分 */
	public UILabel lblIntegral;
	/** 每日积分宝箱 */
	public UILabel lblEverydayBox;
	/** 累计积分 */
	public UILabel lblTotalIntegral; 
	/** 积分赛大标签 */
	public UILabel lblTeamIntegralBattle;
	public ButtonBase integralButton;
	public GameObject timeProgress;
	public GameObject timeItemPrefab;
    public GameObject EffectPointGameObject;
	/** 对手prefab */
	public GameObject godsWarEnemyPrefab;

	/** 飘雪特效点 */
	public GameObject backgroundEffectRoot;

	/** 刷新对手按钮 */
	public ButtonBase btnRefreshEnemy; 
	/** 对手数组 */
	public GameObject[] enemyItems;
	/** 刷新cd */
	public UILabel lblUpdateCD;
	/** 关闭 */
	public GameObject buttonClose;
	/** 只有从首页进入页面才会提示 */
	[HideInInspector]
	public bool
		showTeamDialog;
	/** 诸神之战管理 */
	GodsWarManagerment godsWarManager;
	/** 对手列表 */
	List<GodsWarUserInfo> enemyList;
	/** 状态检测 */
	Timer checkStateTimer;
	/** 刷新时间timer */
	Timer timer;
	/** 连胜特效 */
	public GameObject winEffect;
	public GameObject loseEffect;
	public GameObject value;
	public GameObject lianShengZhongJie;
	Animation lianshengzhongzhi_anim;
	/** 额外积分 */
	public UILabel lblExtraIntegral;
	/** 连胜值 */
	public UISprite winValue;
    /**刷新遮罩 */
    public GameObject rushObj;
	/// <summary>
	/// 上次载入记录连胜次数
	/// </summary>
	private int firstWinValue;
    private int[] rushTimeByDay;
    private bool canRush = true;
	/** 我的信息 */
	GodsWarUserInfo myGodsWarInfo;
	/** 打完出来是否胜负，在获取海选对手前赋值 */
	[HideInInspector]
	public int
		isWin = -1;

	public override void OnAwake ()
	{
		base.OnAwake ();
		isWin = -1;
		lianshengzhongzhi_anim = lianShengZhongJie.GetComponent<Animation>();
	}

	void Update()
	{
		if(lianShengZhongJie.activeSelf == true && !lianshengzhongzhi_anim.isPlaying)
		{
			lianShengZhongJie.SetActive(false);
		}
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		
	}

	protected override void begin ()
	{
        MaskWindow.LockUI();
        UiManager.Instance.backGround.switchBackGround("godsWar_back");
        GodsWarManagerment.Instance.getGodsWarStateInfo(() =>
        {
            if (GodsWarManagerment.Instance.StateInfo != 1)//N久以后读取数据不是小组赛就会主界面
	    {
                UiManager.Instance.openMainWindow();
                return;
            }
        });
	    UiManager.Instance.godsWarGroupStageWindow = this;
        if (UiManager.Instance.isInGodsBattle) {
            UiManager.Instance.isInGodsBattle = false;
            UiManager.Instance.BackToWindow<MainWindow>();
            UiManager.Instance.backGround.switchBackGround("backGround_1");
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("godsCloseInfo"));
            });
            return;
        }
        FPortManager.Instance.getFPort<GodsWarGroupStageFPort>().access(() =>
        {
            if (!isAwakeformHide) initialization();
            else startTimer();
            if (ResourcesManager.Instance.allowLoadFromRes) {
                cacheFinish();
            } else {
                if (!isAwakeformHide)
                {
                    cacheModel();
                }
                else
                {
                    cacheFinish();
                }
               
            }
            rushObj.SetActive(false);
            //MaskWindow.UnlockUI();
        });

	}

    public void updateWin()
    {
        EffectManager.Instance.CreateEffectCtrlByCache(EffectPointGameObject.transform, "Effect/UiEffect/Miaosha", null);
        FPortManager.Instance.getFPort<GodsWarGroupStageFPort>().access(() => {
        if (!isAwakeformHide) initialization();
        else startTimer();
        if (ResourcesManager.Instance.allowLoadFromRes) {
            cacheFinish();
        } else {
            if (!isAwakeformHide) {
                cacheModel();
            } else {
                cacheFinish();
            }

        }
        rushObj.SetActive(false);
    MaskWindow.UnlockUI();
});
    }

    /// <summary>
	/// 播放连胜动画
	/// </summary>
	public void playWinEffect ()
	{
		if (godsWarManager.self.numOfWinning > 0) {
			firstWinValue = godsWarManager.self.numOfWinning;
			winEffect.gameObject.SetActive (true);
			//loseEffect.gameObject.SetActive (isWin == 1);
			lianShengZhongJie.SetActive(isWin == 1);
			int num = 0;
			if (godsWarManager.self.numOfWinning == -1)
				num = 1;
			//10连胜单独处理
			if (godsWarManager.self.numOfWinning == 10) {
				value.gameObject.SetActive (true);
				winValue.spriteName = "1";
			} else {
				winValue.spriteName = (godsWarManager.self.numOfWinning + num).ToString ();
			} 
			lblExtraIntegral.text = LanguageConfigManager.Instance.getLanguage ("godsWar_50", (godsWarManager.self.numOfWinning + 9 + num).ToString ());

		} else {
            //loseEffect.gameObject.SetActive(isWin == 1 && firstWinValue > 0);
			lianShengZhongJie.SetActive(isWin == 1 && firstWinValue > 0);
		    firstWinValue = 0;
//			StartCoroutine(Utils.DelayRun(()=>{//临时代替动画销毁
//				loseEffect.gameObject.SetActive (false);
//			},2f));
			winEffect.gameObject.SetActive (false);
		}
		int integralDesc = IncAttributeGlobal.Instance.getIntAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_GODSWAR_INTEGRAL);
		if (integralDesc >= 0 && isWin != -1) {
			integralDesc = GodsWarManagerment.Instance.self.totalIntegral - integralDesc;
			if (integralDesc > 0) {
			}
			IncAttributeGlobal.Instance.removeAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_GODSWAR_INTEGRAL);
		}
		string str = null;
		if (isWin == 0) {
			str = LanguageConfigManager.Instance.getLanguage ("godsWar_98", integralDesc.ToString ());
		} else if (isWin == 1) {
			str = LanguageConfigManager.Instance.getLanguage ("godsWar_99", integralDesc.ToString ());
		}
		this.isWin = -1;
		if (!string.IsNullOrEmpty (str)) {
			TextTipWindow.ShowNotUnlock (str);
		}
	}

	/// <summary>
	/// 初始化基本信息
	/// </summary>
	private void initialization ()
	{
		godsWarManager = GodsWarManagerment.Instance;
		EffectManager.Instance.CreateEffectCtrlByCache (backgroundEffectRoot.transform, "Effect/UiEffect/MeteorShower", null);
		startTimer ();
		initEnemyButtons ();
	}

	/// <summary>
	/// 开启时间计时器
	/// </summary>
	private void startTimer ()
	{
		//checkStateTimer = TimerManager.Instance.getTimer (1000);
		//checkStateTimer.addOnTimer (checkState);
		//checkStateTimer.start ();
        string[] rushTime = CommandConfigManager.Instance.godsWarsRushTimes;
        rushTimeByDay = new int[rushTime.Length];
        for (int i = 0; i < rushTime.Length; i++) {
            rushTimeByDay[i] = StringKit.toInt(rushTime[i].Split(':')[0]) * 3600 + StringKit.toInt(rushTime[i].Split(':')[1]) * 60 + StringKit.toInt(rushTime[i].Split(':')[2]);
        }
		timer = TimerManager.Instance.getTimer (1000);
		timer.addOnTimer (updateTime);
		timer.start ();
	}

	/// <summary>
	/// 初始化刷新对手点击按钮
	/// </summary>
	private void initEnemyButtons ()
	{
		btnRefreshEnemy.fatherWindow = this;
		btnRefreshEnemy.onClickEvent = doUpdateEnemy;
	}

	/// <summary>
	/// 预加载模型
	/// </summary>
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

	/// <summary>
	/// 预加载结束，获取小组赛信息
	/// </summary>
	void cacheFinish ()
	{
		if (showTeamDialog) {
			showTeamDialog = false;
		}
		//FPortManager.Instance.getFPort<GodsWarGroupStageFPort> ().access (OnInfoLoad);
	    OnInfoLoad();
	    //MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 载入具体信息
	/// </summary>
	public void OnInfoLoad ()
	{
		playWinEffect ();
		if (godsWarManager.self == null) {
			MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_22"), (msg) => {
				buttonEventBase (buttonClose);
			});
			return;
		}
        //string currentMassEnemyUid = GodsWarManagerment.Instance.currentMassEnemyUid;
        //GodsWarUserInfo currentMassEnemy = null;
        //GameObject currentEnemyLocation = null;
		updateMyInfo ();
        updateEnemyInfo();
        MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 更新我的信息
	/// </summary>
	public void  updateMyInfo ()
	{
		myGodsWarInfo = GodsWarManagerment.Instance.self;
		godsWarManager = GodsWarManagerment.Instance;
		lblTimes.text = LanguageConfigManager.Instance.getLanguage ("godsWar_23", (godsWarManager.maxChallengeCount - myGodsWarInfo.usedChallgeNum) > 0 ? (godsWarManager.maxChallengeCount - myGodsWarInfo.usedChallgeNum).ToString () : "0", godsWarManager.maxChallengeCount.ToString ());
        int team = StringKit.toInt(GodsWarManagerment.Instance.self.bigTeam);
	    string bigt = "";
	    if (team == 110) bigt = LanguageConfigManager.Instance.getLanguage("godsWar_70l");
        if (team == 111) bigt = LanguageConfigManager.Instance.getLanguage("godsWar_71l");
        if (team == 112) bigt = LanguageConfigManager.Instance.getLanguage("godsWar_72l");
        lblMyTeam.text = LanguageConfigManager.Instance.getLanguage ("godsWar_32", bigt+myGodsWarInfo.smallTeam);
		if (myGodsWarInfo.rank == -1)
			lblRank.text = LanguageConfigManager.Instance.getLanguage ("godsWar_36");
		else
			lblRank.text = LanguageConfigManager.Instance.getLanguage ("godsWar_33", myGodsWarInfo.rank.ToString ()) + (myGodsWarInfo.rank < 5 ? LanguageConfigManager.Instance.getLanguage ("godsWar_34") : "");
		lblIntegral.text = LanguageConfigManager.Instance.getLanguage ("godsWar_24", myGodsWarInfo.todayIntegral.ToString ());
		lblTotalIntegral.text = LanguageConfigManager.Instance.getLanguage ("godsWar_25", myGodsWarInfo.totalIntegral.ToString ());
		lblTeamIntegralBattle.text = LanguageConfigManager.Instance.getLanguage ("godsWar_27", myGodsWarInfo.smallTeam);
		updateIntegralAward ();
		setProgess ();

	}
	/// <summary>
	/// 更新每日积分领奖信息
	/// </summary>
	public void updateIntegralAward ()
	{
		//int max = GodsWarPrizeSampleManager.Instance.getEveryDayMaxIntegral ();
		int max = 0;
		string str = "";
		bool isFlashBox = false;
		bool istrue = true;
		//所有积分阶段列表
		List<int> integralList = GodsWarPrizeSampleManager.Instance.getIntegralList ();
		//已经领取积分的列表
		List<int> integralGetList = GodsWarManagerment.Instance.currentScores;
		int todayIntegral = GodsWarManagerment.Instance.self.todayIntegral;
		if (integralGetList == null)
			integralGetList = new List<int> ();
		for (int i = 0; i < integralList.Count && istrue; i++) {
			if (todayIntegral >= integralList [i]) {
				if (!integralGetList.Contains (integralList [i])) {
					str = LanguageConfigManager.Instance.getLanguage ("godsWar_101");
					isFlashBox = true;
					istrue = false;
				}
			} else {
				max = integralList [i];
				istrue = false;
			}
		}
		if (istrue) {
			str = LanguageConfigManager.Instance.getLanguage ("godsWar_102");
		}

		UITweener position = integralButton.GetComponentInChildren<TweenPosition> ();
		UITweener rotation = integralButton.GetComponentInChildren<TweenRotation> ();
		position.enabled = false;
		rotation.enabled = false;
		if (isFlashBox) {
			lblEverydayBox.text = str;
			position.enabled = true;
			rotation.enabled = true;
		} else {
			position.enabled = false;
			rotation.enabled = false;
			lblEverydayBox.text = LanguageConfigManager.Instance.getLanguage ("godsWar_26", myGodsWarInfo.todayIntegral.ToString (), max.ToString ());
			if (istrue)
				lblEverydayBox.text = str;
		}
			
		
	}
	/// <summary>
	/// 检测当前状态是否正确，不正确则返回主界面
	/// </summary>
	private void checkState ()
	{
		int state = GodsWarManagerment.Instance.getWeekOfDayState ();
		if (state != 2) {//如果不是小组赛则将玩家弹出此界面
			if (checkStateTimer != null)
				checkStateTimer.stop ();
            UiManager.Instance.openMainWindow();
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage ("godsWar_128"));
            });
		}
        if(checkStateTimer!=null&&!gameObject.activeInHierarchy)checkStateTimer.stop();
	}

	/// <summary>
	/// 刷新时间
	/// </summary>
	public void updateTime ()
	{
        if (this == null || !gameObject.activeInHierarchy) {
            if (timer != null) {
                timer.stop();
                timer = null;
            }
            return;
        }
		int nextUpdateTime=0;
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
	    int currentTime = serverDate.Hour*3600 + serverDate.Minute*60 + serverDate.Second;
	    for (int i=0;i<rushTimeByDay.Length;i++)
	    {
	        if (currentTime < rushTimeByDay[i])
	        {
	            nextUpdateTime = rushTimeByDay[i] - currentTime;
	            break;
	        }
	        if (i==rushTimeByDay.Length-1)
	        {
	            nextUpdateTime = 24*3600 - currentTime + rushTimeByDay[0];
	        }
	    }
        if (nextUpdateTime <= 5&&canRush)
        {
            canRush = false;
            UiManager.Instance.destoryWindowByName("MessageWindow");
            MaskWindow.LockUI();
            rushObj.SetActive(true);
            StartCoroutine(Utils.DelayRun(() => {
                AutoUpdateEnemy();
            }, 5));
            
                   
	    }
        if (currentTime > 0 && currentTime <= 3 && canRush)
        {
            canRush = false;
            MaskWindow.LockUI();
            StartCoroutine(Utils.DelayRun(() => {
                UiManager.Instance.destoryWindowByName("GodsWarIntegralAwardWindow");
                AutoUpdateEnemy();
            }, 4));
	    }
        lblUpdateCD.text = timeTransform(nextUpdateTime);

	}
	/// <summary>
	/// 转换时间time为决定时间
	/// </summary>
	private string timeTransform (int time)
	{
		int desHour = time / 3600;
		int desMinute = (time - (time / 3600) * 3600) / 60;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
		int desSecond = time - desHour * 3600 - desMinute * 60;

		return changeString (desHour) + ":" + changeString (desMinute) + ":" + changeString (desSecond);
	}

	private string changeString (int time)
	{
		if (time < 10)
			return "0" + time;
		return time + "";
	}


	/// <summary>
	/// 执行刷新对手事件(花费钻石)
	/// </summary>
	private void doUpdateEnemy (GameObject obj)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.dialogCloseUnlockUI = false;
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"),
			               LanguageConfigManager.Instance.getLanguage ("godsWar_96", GodsWarInfoConfigManager.Instance ().getSampleBySid (3001).num[0].ToString ()), (msg) => {
				if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
				    if (UserManager.Instance.self.getRMB() < GodsWarInfoConfigManager.Instance().getSampleBySid(3001).num[0])
				    {
                        //UiManager.Instance.openDialogWindow<MessageLineWindow>((winn) =>
                        //{
                        //    winn.Initialize(LanguageConfigManager.Instance.getLanguage("godsWar_1412"));
                        //});
                        UiManager.Instance.openDialogWindow<MessageWindow>((winn) => {
                            winn.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0324"),
                                           LanguageConfigManager.Instance.getLanguage("godsWar_1412"), (msgg) => {
                                               if (msgg.buttonID == MessageHandle.BUTTON_RIGHT) {
                                                  //finishWindow();
                                                   UiManager.Instance.openWindow<rechargeWindow>();
                                               } else {
                                                   MaskWindow.UnlockUI();
                                               }
                                           });
                        });
				    }
				    else
				    {
                        FPortManager.Instance.getFPort<GodsWarRefreshEnemyFPort>().access(updateEnemyInfo);
                        MaskWindow.UnlockUI();
				    }
					
				} else {
					//finishWindow();
					MaskWindow.UnlockUI ();
				}
					
			});
		});

	}
	/// <summary>
	/// 固定时间点自动刷新对手
	/// </summary>
	private void AutoUpdateEnemy ()
	{
        FPortManager.Instance.getFPort<GodsWarGroupStageFPort>().access(() =>
        {
            playWinEffect();
            updateMyInfo();
            updateEnemyInfo();
            //FPortManager.Instance.getFPort<GodsWarRefreshEnemyFPort>().access(updateEnemyInfo);
        });
		
	}

	/// <summary>
	/// 更新比赛进度
	/// </summary>
	private void setProgess ()
	{
		Utils.RemoveAllChild (timeProgress.transform);
		GameObject go = NGUITools.AddChild (timeProgress, timeItemPrefab);
		GodsWarTimeProgress itme = go.GetComponent<GodsWarTimeProgress> ();
		itme.fatherWindow = this;
		itme.initTime ();
	}

	/// <summary>
	/// 获得日期
	/// </summary>
	public string getDateTime (int secondTime)
	{
		return TimeKit.dateToFormat (secondTime, LanguageConfigManager.Instance.getLanguage ("notice04"));
	}

	/// <summary>
	/// 更新对手信息
	/// </summary>
	public void  updateEnemyInfo ()
	{
		godsWarManager = GodsWarManagerment.Instance;
		List<GodsWarUserInfo> list = godsWarManager.getEnemyList ();
		for (int i=0; i<list.Count; i++) {

		    if (list[i].uid == "-1")
		    {
                enemyItems[i].SetActive(false);
                continue;
		    }
            enemyItems[i].SetActive(true);
            Utils.DestoryChilds(enemyItems[i]);
			GameObject go = NGUITools.AddChild (enemyItems [i], godsWarEnemyPrefab) as GameObject;
			GodsWarEnemyItem item = go.GetComponent<GodsWarEnemyItem> ();
			item.initEnemyInfo (i + 1, list [i].uid, list [i].serverName, list [i].name, list [i].level, list [i].winScore, list [i].challengedWin, list [i].headIcon, iscanPaly);
			item.fatherWindow = this;
		}
	    if (!canRush)
	    {
            canRush = true;
            MaskWindow.UnlockUI();
            rushObj.SetActive(false);
	    }
        //
	}

	/// <summary>
	/// 进入战斗
	/// </summary>
	private void iscanPaly (GodsWarUserInfo enemy)
	{
		if (enemy.challengedWin) {
			MaskWindow.UnlockUI ();
			return;
		}
		//挑战次数用尽
		if (godsWarManager.getChallengeCount () <= 0) {
//			MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_48"), (msg) => {
//				//buttonEventBase (buttonClose);
//				MessageWindow mw = UiManager.Instance.getWindow<MessageWindow>("MessageWindow");
//				mw.finishWindow();
//			});

			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("godsWar_48"), (msg)=>{
					win.finishWindow();
				},MessageAlignType.center);
			});

			return;
		}
		if (!ArmyManager.Instance.checkArmyMemberCount ()) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI = false;
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0442"), LanguageConfigManager.Instance.getLanguage ("s0093"),
				                LanguageConfigManager.Instance.getLanguage ("MassPlayerWindow_MembersNotEnough"), (msg) => {
					if (msg.buttonID == MessageHandle.BUTTON_LEFT) 
						UiManager.Instance.openWindow<TeamEditWindow> ((wins) => {
							wins.comeFrom = TeamEditWindow.FROM_PVP;
						});
					else if (msg.buttonID == MessageHandle.BUTTON_RIGHT)
						sureFight (enemy);
				});
			});
		} else
			sureFight (enemy);
	}

	/// <summary>
	/// 确认出战
	/// </summary>
	void sureFight (GodsWarUserInfo user)
	{
		GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleForGodsWar;
		FPortManager.Instance.getFPort<GodsWarChallengeFport> ().access ((success) => {
			if (success) {
				MaskWindow.instance.setServerReportWait (true);
				IncAttributeGlobal.Instance.setAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_GODSWAR_INTEGRAL, GodsWarManagerment.Instance.self == null ? 0 : GodsWarManagerment.Instance.self.totalIntegral);
				godsWarManager.currentMassEnemyUid = user.uid;
			} else {
				MessageWindowShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_47"), (msg) => {
					GameManager.Instance.battleReportCallback = null;
					MaskWindow.instance.setServerReportWait (false);
					finishWindow ();
				});
			}
		}, user.serverName, user.uid);
	}
	/// <summary>
	/// 更新挑战次数
	/// </summary>
	void UpdateChallengeCount ()
	{
		int count = godsWarManager.getChallengeCount ();
		lblTimes.text = LanguageConfigManager.Instance.getLanguage ("godsWar_45") + ": " + count + "/" + godsWarManager.maxChallengeCount;
	}

	void OnDestroy ()
	{
	    UiManager.Instance.godsWarGroupStageWindow = null;
        //if (checkStateTimer != null)
        //    checkStateTimer.stop ();
		if ( timer != null)
			timer.stop ();
		//防止不属于次战斗但进入该战斗退出流程
		FuBenManagerment.Instance.isGodsWarGroup = false;
	}

	void MessageWindowShowAlert (string msg, CallBackMsg callback)
	{
		MessageWindow.ShowAlert (msg, (ev) => {
			if (callback != null)
				callback (ev);
		});
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "button_close") {
			IncAttributeGlobal.Instance.removeAttribute (AttributeGlobalCommon.INC_ATTRIBUTES_GODSWAR_INTEGRAL);
			finishWindow ();
		} else if (gameObj.name == "integral") {
			UiManager.Instance.openDialogWindow<GodsWarIntegralAwardWindow> ((win) => {
				win.initUI (updateIntegralAward);});
		}
		if (gameObj.name == "buttonHelp") {
			UiManager.Instance.openWindow<GodsWarPreparWindow> ();
		} else if (gameObj.name == "button_rank") {
			UiManager.Instance.openWindow<GodsWarGroupRankWindow> ();

		} else if (gameObj.name == "button_team") {
			UiManager.Instance.openWindow<TeamEditWindow> ((wins) => {
				wins.comeFrom = TeamEditWindow.FROM_PVP;
			});

		} else if (gameObj.name == "buttonFinalAward") {

			UiManager.Instance.openWindow<GodsWarIntegralRankAwardWindow> ((win) => {
				win.initUI ();});

		} else if (gameObj.name == "buttonMeritShop") {
			UiManager.Instance.openWindow<GodsWarShopWindow> ((win) => {
				win.initWindow (() => {});});
		}
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
        UiManager.Instance.openMainWindow();
		//begin();
	}
}
