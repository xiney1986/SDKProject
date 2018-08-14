using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
** 诸神战淘汰赛窗口
**/
public class GodsWarFinalWindow : WindowBase
{
	public TapButtonBase[] tabButtons;
	public TapContentBase tapContent;
	/// <summary>
	/// 战场管理
	/// </summary>
	private GodsWarManagerment manager;
	/// <summary>
	/// 组别类型(前台0:青铜组 1:白银组 2:黄金组)big_id
	/// </summary>
	private int type;
	/// <summary>
	/// 战场类别
	/// </summary>
	private int index = 1;
	/// <summary>
	/// 淘汰赛基础信息列表
	/// </summary>
	public List<GodsWarFinalUserInfo> users;
	/// <summary>
	/// 我的支持信息
	/// </summary>
	public List<GodsWarMySuportInfo> mySuportInfo;
	/// <summary>
	/// 玩家所在点位
	/// </summary>
	public GameObject[] points;
	/// <summary>
	/// 玩家信息预制体
	/// </summary>
	public GameObject userInfoPrefab;
	/// <summary>
	/// 对战点位信息Item(战报)
	/// </summary>
	public GodsWarFinalPointItem[] items;
	/// <summary>
	/// 对战点位
	/// </summary>
	public List<GodsWarFinalPoint> finalPoints;
	/// <summary>
	/// 神魔大战开始时间
	/// </summary>
	public UILabel lblTime;
	/// <summary>
	/// 时间进度
	/// </summary>
	public GameObject timeProgress;
	/// <summary>
	/// 时间条目预设
	/// </summary>
	public GameObject timeItemPrefab;
	/// <summary>
	/// 淘汰赛冠军头像
	/// </summary>
	public UITexture winnerIcon;
	/// <summary>
	/// 淘汰赛冠军名字
	/// </summary>
	public UILabel lblWinner;
	/// <summary>
	/// 淘汰赛界面
	/// </summary>
	public GameObject center;
	/// <summary>
	/// 神魔大战界面
	/// </summary>
	public GameObject shemMo;
	/// <summary>
	/// 神魔大战点位
	/// </summary>
	public GameObject[] pointShenMo;
	/// <summary>
	/// 神魔大战条目
	/// </summary>
	public GameObject shemMoItemPrefab;
	/// <summary>
	/// 支持或查看战报
	/// </summary>
	public ButtonBase zhanOrReplay;
	/// <summary>
	/// 右移动
	/// </summary>
	public ButtonBase leftButton;
	/// <summary>
	/// 左移动
	/// </summary>
	public ButtonBase rightButton;
	/// <summary>
	/// 默认组别
	/// </summary>
	private int defaultBigId = 1;
	/// <summary>
	/// 大战名称
	/// </summary>
	public UILabel lblBig_id;
	/// <summary>
	/// 神魔大战是否开启
	/// </summary>
	private bool isOpenShenMo = false;
	/// <summary>
	/// 检测时间
	/// </summary>
	private Timer checkStateTimer;
	/// <summary>
	/// 可点击冠军头像
	/// </summary>
	public ButtonBase winnerIconButton;
	/// <summary>
	/// 冠军信息
	/// </summary>
	GodsWarFinalUserInfo winner;

    public UISprite mofangorshengwang;
    /** 刷新时间timer */
    Timer timer;
    private bool rushFlag=false;
    public UITexture bgTexture;
	protected override void DoEnable ()
	{
		base.DoEnable ();
		//UiManager.Instance.backGround.switchBackGround ("battleMap_11");

	}
	protected override void begin ()
	{
	    if (UiManager.Instance.isInGodsBattle)
	    {
	        UiManager.Instance.isInGodsBattle = false;
	        UiManager.Instance.BackToWindow<MainWindow>();
            UiManager.Instance.backGround.switchBackGround("backGround_1");
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                   win.Initialize(LanguageConfigManager.Instance.getLanguage("godsCloseInfo"));
               });
	        return;
	    }
        UiManager.Instance.godsWarFinalWindow = this;
		if (isAwakeformHide) {
			updateTapBg ();
            OnDataLoad();
			MaskWindow.UnlockUI ();
			return;
		}
        timer = TimerManager.Instance.getTimer(1000);
        timer.addOnTimer(updateTime);
        timer.start();
		OnDataLoad ();
	}

    void updateTime()
    {
        setShenMoOpenTime();
        System.DateTime serverDate = ServerTimeKit.getDateTime();
        godsWarTime godsTimes=GodsWarInfoConfigManager.Instance().getSampleBySid(6001).times[2];
        int currnetTime=serverDate.Hour*3600 + serverDate.Minute*60 + serverDate.Second;
        int rushTime = godsTimes.hour*3600 + godsTimes.minute*60;
        if ((rushTime - currnetTime>0&&rushTime - currnetTime < 300) ||( currnetTime-rushTime>0&&currnetTime-rushTime<100))
        {
            zhanOrReplay.gameObject.SetActive(false);
            return;
        }
        if (!rushFlag&&rushTime - currnetTime < 0)
        {
            rushFlag = true;
            if (UiManager.Instance.isInGodsBattle) {
                UiManager.Instance.isInGodsBattle = false;
                UiManager.Instance.BackToWindow<MainWindow>();
                UiManager.Instance.backGround.switchBackGround("backGround_1");
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("godsCloseInfo"));
                });
                return;
            }
            OnDataLoad();
        }
        
    }
	/// <summary>
	/// 更新背景和tap背景
	/// </summary>
	private void updateTapBg ()
	{
		if (index == 0) {
			UiManager.Instance.backGround.switchBackGround ("godswar_bg");
			if (tabButtons [2].spriteBg.spriteName.EndsWith ("2"))
				tabButtons [2].spriteBg.spriteName = tabButtons [2].spriteBg.spriteName.Replace ("2", "");
		} else {
			UiManager.Instance.backGround.switchBackGround ("battleMap_11");
            if (index - 1 == 0) mofangorshengwang.spriteName = "tianshen";
            else mofangorshengwang.spriteName = "mowang";
			if (tabButtons [index - 1].spriteBg.spriteName.EndsWith ("2"))
				tabButtons [index - 1].spriteBg.spriteName = tabButtons [index - 1].spriteBg.spriteName.Replace ("2", "");
		}
	}
	/// <summary>
	/// 开启时间计时器
	/// </summary>
	private void startTimer ()
	{
		checkStateTimer = TimerManager.Instance.getTimer (1000);
		checkStateTimer.addOnTimer (checkState);
		checkStateTimer.start ();
	}
	/// <summary>
	/// 检测当前状态是否正确（主要是清理分组校验）
	/// </summary>
	private void checkState ()
	{
		//校验星期
		int currentDayState = GodsWarManagerment.Instance.getWeekOfDayState ();
		if (currentDayState == 2) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_130"), (msg) => {
				finishWindow ();
			});
		}
		//校验清理分组时间（这里和后台商量固定22点为清理时间，所以在21:59:59分时进入界面则弹回主界面）
		int dayOfWeek = TimeKit.getWeekCHA (ServerTimeKit.getDateTime ().DayOfWeek);
		if (dayOfWeek == 7 && ServerTimeKit.getDateTime ().Hour == 21 && ServerTimeKit.getDateTime ().Minute == 59 && ServerTimeKit.getDateTime ().Second == 59) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_131"), (msg) => {
				finishWindow ();
			});
		}
	}

	void OnDestroy ()
	{
	    UiManager.Instance.godsWarFinalWindow = null;
		if (checkStateTimer != null)
			checkStateTimer.stop ();
        if(timer!=null)timer.stop();
	}

	/// <summary>
	/// 设置初始大组和域名
	/// </summary>
	public void setBigidAndYuming ()
	{
		GodsWarManagerment.Instance.setTypeIndex ();
	}

	/// <summary>
	/// 向后台拿基础数据
	/// </summary>
	public void OnDataLoad ()
	{
		manager = GodsWarManagerment.Instance;
		type = manager.getType ();
		setBigId (manager.type);
		defaultBigId = manager.type;
		initButton ();
		index = manager.tapIndex;
		//神魔大战标签
		if (index == 0) {
			updateTapBg ();
			center.gameObject.SetActive (false);
			shemMo.gameObject.SetActive (true);
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "shenzhantaizi", bgTexture);
			if (!isOpenShenMo && GodsWarManagerment.Instance.getGodsWarStateInfo () != GodsWarManagerment.STATE_HAVE_ZIGE_FINAL && GodsWarManagerment.Instance.getGodsWarStateInfo () != GodsWarManagerment.STATE_NOT_ZIGE_FINAL) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_111"));
				//显示支持或查看战报按钮与否
				zhanOrReplay.gameObject.SetActive (false);
			} else {
				//进入神魔大战界面
				FPortManager.Instance.getFPort<GodsWarFinalInfoFPort> ().access (type, index, OnDataLoadShenMoInfo);
			}
            setShenMoOpenTime();
		} else {
			center.gameObject.SetActive (true);
            setProgess(index);
            
			shemMo.gameObject.SetActive (false);
			updateTapBg ();
			FPortManager.Instance.getFPort<GodsWarFinalInfoFPort> ().access (type, index, getMySuportInfo);
		}
		MaskWindow.UnlockUI ();

	}

	/// <summary>
	/// 更新比赛进度
	/// </summary>
	private void setProgess (int indexxx)
	{
        timeProgress.SetActive(true);
        Utils.RemoveAllChild(timeProgress.transform);
		GameObject go = NGUITools.AddChild (timeProgress, timeItemPrefab);
		GodsWarTimeProgress itme = go.GetComponent<GodsWarTimeProgress> ();
		itme.fatherWindow = this;
	    if (indexxx == 0)
	        itme.initTime();
	    else
	    {
	        int day = TimeKit.getWeekCHA (ServerTimeKit.getDateTime ().DayOfWeek);
            if (day != 4) timeProgress.SetActive(false);
	        else
	        { 
                itme.initFinal();
	        }
	    }
			
	}
	/// <summary>
	/// 设置神魔大战开始时间
	/// </summary>
	private void setShenMoOpenTime ()
	{
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
		int day = TimeKit.getWeekCHA (serverDate.DayOfWeek);
	    if (day < 5)
	        lblTime.text = LanguageConfigManager.Instance.getLanguage("godsWar_126",
	            getDateTime(ServerTimeKit.getSecondTime() + 86400*(5 - day)),
	            GodsWarInfoConfigManager.Instance().getSampleBySid(8001).times[0].hour + ":" +
	            (GodsWarInfoConfigManager.Instance().getSampleBySid(8001).times[0].minute == 0
	                ? "00"
	                : GodsWarInfoConfigManager.Instance().getSampleBySid(8001).times[0].minute + ""));
	    else
	    {
            int currentTime = serverDate.Hour * 3600 + serverDate.Minute * 60 + serverDate.Second;
            godsWarTime gwt = GodsWarInfoConfigManager.Instance().getSampleBySid(8001).times[0];
	        if (day == 5)
	        {
	            if (currentTime < gwt.hour*3600 + gwt.minute*60)
	            {
                    lblTime.text = LanguageConfigManager.Instance.getLanguage("godsWar_126",
                getDateTime(ServerTimeKit.getSecondTime() + 86400 * (5 - day)),
                GodsWarInfoConfigManager.Instance().getSampleBySid(8001).times[0].hour + ":" +
                (GodsWarInfoConfigManager.Instance().getSampleBySid(8001).times[0].minute == 0
                    ? "00"
                    : GodsWarInfoConfigManager.Instance().getSampleBySid(8001).times[0].minute + ""));
	            }else if (currentTime > gwt.hour * 3600 + gwt.minute * 60 && currentTime < gwt.hour * 3600 + gwt.minute * 60 + 1261) lblTime.text = LanguageConfigManager.Instance.getLanguage("godsWar_1277");
                 else lblTime.text = LanguageConfigManager.Instance.getLanguage("godsWar_127");
	        }
	    }
			
       // if (zhanOrReplay.gameObject.activeInHierarchy && zhanOrReplay.textLabel.text == LanguageConfigManager.Instance.getLanguage("godsWar_125")) lblTime.text = LanguageConfigManager.Instance.getLanguage("godsWar_127");
	}
	/// <summary>
	/// 获得日期
	/// </summary>
	public string getDateTime (int secondTime)
	{
		return TimeKit.dateToFormat (secondTime, LanguageConfigManager.Instance.getLanguage ("notice04"));
	}
	/// <summary>
	/// 初始化按钮状态
	/// </summary>
	public void initButton ()
	{
		rightButton.fatherWindow = this;
		leftButton.fatherWindow = this;
		winnerIconButton.fatherWindow = this;
		rightButton.onClickEvent = doRightEvent;
		leftButton.onClickEvent = doLeftEvent;
		winnerIconButton.onClickEvent = doViewWinnerInfo;
		winnerIconButton.GetComponent<Collider> ().enabled = false;
		if (defaultBigId == 2)
			rightButton.gameObject.SetActive (false);
		if (defaultBigId == 0)
			leftButton.gameObject.SetActive (false);
	}
	/// <summary>
	/// 执行组别向右转换事件
	/// </summary>
	public void doRightEvent (GameObject obj)
	{
		if (defaultBigId >= 2) {
			rightButton.gameObject.SetActive (false);
			return;
		}
		defaultBigId += 1;
		leftButton.gameObject.SetActive (true);
		if (defaultBigId >= 2)
			rightButton.gameObject.SetActive (false);
		else 
			rightButton.gameObject.SetActive (true);
		setBigId (defaultBigId);
		changeInfoByBigId (defaultBigId);
	}

	/// <summary>
	/// 执行组别向左转换事件
	/// </summary>
	public void doLeftEvent (GameObject obj)
	{
		if (defaultBigId <= 0) {
			leftButton.gameObject.SetActive (false);
			return;
		}
		defaultBigId -= 1;
		rightButton.gameObject.SetActive (true);
		if (defaultBigId <= 0)
			leftButton.gameObject.SetActive (false);
		else
			leftButton.gameObject.SetActive (true);
		setBigId (defaultBigId);
		changeInfoByBigId (defaultBigId);
	}

	/// <summary>
	/// 执行查看冠军信息事件
	/// </summary>
	public void doViewWinnerInfo (GameObject obj)
	{
		if (winner.serverName == "") {
			MaskWindow.UnlockUI ();
			return;
		}
		UiManager.Instance.openDialogWindow<GodsWarUserInfoWindow> ((win) => {
			win.initWindow (winner.serverName, winner.name, winner.uid, type, index, () => {});
		});
	}
	/// <summary>
	/// 设置大组别信息
	/// </summary>
	/// <param name="big_id">Big_id.</param>
	public void setBigId (int big_id)
	{
		switch (big_id) {
		case 0:
			lblBig_id.text = LanguageConfigManager.Instance.getLanguage ("godsWar_70");
			GodsWarManagerment.Instance.type = 0;
			break;
		case 1:
			lblBig_id.text = LanguageConfigManager.Instance.getLanguage ("godsWar_71");
			GodsWarManagerment.Instance.type = 1;
			break;
		case 2:
			lblBig_id.text = LanguageConfigManager.Instance.getLanguage ("godsWar_72");
			GodsWarManagerment.Instance.type = 2;
			break;
		default:
			break;
		}
	}

	public void changeInfoByBigId (int id)
	{	
		OnDataLoad ();
	}
	/// <summary>
	/// 向后台拿去我的支持信息
	/// </summary>
	public void getMySuportInfo ()
	{
		FPortManager.Instance.getFPort<GodsWarGetMySuportFport> ().access (initData);
	}
	/// <summary>
	/// 初始化数据(index,1： 圣域战场 2：魔域战场 0：神域战场)
	/// </summary>
	public void initData ()
	{
		users = manager.finalInfoList;
		for (int i=0; i<points.Length; i++) {
			GodsWarFinalUserInfo info = new GodsWarFinalUserInfo ();
			for (int j = 0; j<users.Count; j++) {
				if (users [j].index - 1 == i)
					info = users [j];
			}
			Utils.RemoveAllChild (points [i].transform);
			GameObject go = NGUITools.AddChild (points [i], userInfoPrefab);
			GodsWarFinalUserInfoItem item = go.transform.GetComponent<GodsWarFinalUserInfoItem> ();
			item.initUI (type, index, info, this);
		}

		OnDataLoadPointInfo ();
	}
	/// <summary>
	/// 向后台拿淘汰赛对战点位数据
	/// </summary>
	void OnDataLoadPointInfo ()
	{
		type = manager.getType ();
		index = manager.tapIndex;
		FPortManager.Instance.getFPort<GodsWarGetPvpInfoFPort> ().access (type, index, initPointItem);	
	}
	/// <summary>
	/// 向后台拿神魔大战点位数据
	/// </summary>
	void OnDataLoadShenMoInfo ()
	{
		FPortManager.Instance.getFPort<GodsWarGetFinalFPort> ().access (type, index, loadModel);	
	}
	/// <summary>
	/// 载入模型
	/// </summary>
	public void loadModel ()
	{
		if (ResourcesManager.Instance.allowLoadFromRes) {
			initPointShenMoItem ();
		} else {
			cacheModel ();
		}
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
			initPointShenMoItem ();
		}, "other");
	}
	/// <summary>
	/// 初始化神魔对战点位信息
	/// </summary>
	public void initPointShenMoItem ()
	{
		List<GodsWarFinalUserInfo> shenmoUser = GodsWarManagerment.Instance.shenMoUserlist;
		finalPoints = GodsWarManagerment.Instance.shenMoPointlist;
		if (finalPoints != null) {
			if (shenmoUser != null) {
				for (int i = 0; i <shenmoUser.Count; i++) {
					Utils.RemoveAllChild (pointShenMo [shenmoUser [i].yu_ming - 1].transform);
					GameObject go = NGUITools.AddChild (pointShenMo [shenmoUser [i].yu_ming - 1], shemMoItemPrefab);
					GodsWarShenMoItem item = go.GetComponent<GodsWarShenMoItem> ();
					GodsWarFinalUserInfo user = new GodsWarFinalUserInfo();
					if(finalPoints.Count >0){
						if(finalPoints[0].isHaveWinner!=-1){
							user = finalPoints[0].winner; 
						}
					}
					if(user.serverName!=null){
						if(user.serverName ==shenmoUser[i].serverName && user.uid == shenmoUser[i].uid)
						{
							item.initEnemyInfo (shenmoUser [i].uid, shenmoUser [i].serverName, shenmoUser [i].name, shenmoUser [i].headIcon, type, index,false);
						}else{
							item.initEnemyInfo (shenmoUser [i].uid, shenmoUser [i].serverName, shenmoUser [i].name, shenmoUser [i].headIcon, type, index,true);
						}
					}
					else{
						item.initEnemyInfo (shenmoUser [i].uid, shenmoUser [i].serverName, shenmoUser [i].name, shenmoUser [i].headIcon, type, index,false);
					}
					item.fatherWindow = this;
				}
			}
			if (finalPoints [0].replayIDs.Length <= 0) {
				if (finalPoints [0].isSuport == -1) {
                    zhanOrReplay.gameObject.SetActive(true);
					zhanOrReplay.textLabel.text = LanguageConfigManager.Instance.getLanguage ("godsWar_124");
					zhanOrReplay.onClickEvent = doZhanEvent;
				} else {
					zhanOrReplay.gameObject.SetActive (false);
				}
			} else {
                zhanOrReplay.gameObject.SetActive(true);
				zhanOrReplay.textLabel.text = LanguageConfigManager.Instance.getLanguage ("godsWar_125");
				zhanOrReplay.onClickEvent = doRepalyEvent;
			}
		}
	}
	/// <summary>
	/// 初始化对战点位信息
	/// </summary>
	public void initPointItem ()
	{
		finalPoints = GodsWarManagerment.Instance.godsWarFinalPoints;
		if (finalPoints != null) {
			for (int i = 0; i < finalPoints.Count; i++) {
				items [finalPoints [i].localID - 1].init (finalPoints [i], type, index, this, getMySuportInfo);
				if (finalPoints [i].localID == 15) {
					initGoupChapingUI (finalPoints [i]);
				}
			}
		}
	}

	/// <summary>
	/// 初始化半决赛ui
	/// </summary>
	public void initGoupChapingUI (GodsWarFinalPoint point)
	{
		FPortManager.Instance.getFPort<GodsWarGetPlayerInfoFPort> ().access (type, index, point.localID, () => {
			List<GodsWarFinalUserInfo> user = GodsWarManagerment.Instance.pvpGodsWarFinalInfo;
			for (int j = 0; j < user.Count; j++) {
				if (point.isHaveWinner == -1)
					return;
				isOpenShenMo = true;
				if (user [j].serverName == point.winner.serverName && user [j].uid == point.winner.uid) {
					ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (user [j].headIcon), winnerIcon);
					lblWinner.text = user [j].name + "." + user [j].serverName;
					winner = user [j];
					winnerIconButton.GetComponent<Collider> ().enabled = true;
				}
			}
		});
	}
	/// <summary>
	/// 决赛点赞
	/// </summary>
	public void doZhanEvent (GameObject obj)
	{
		if (finalPoints [0].isSuport != -1) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_95"));
			return;
		}

		UiManager.Instance.openDialogWindow<GodsWarSuportWindow> ((win) => {
			win.initWindow (type, 0, finalPoints [0].localID, () => {
				zhanOrReplay.gameObject.SetActive (false);
				zhanOrReplay.onClickEvent = doRepalyEvent;
			});});
	}
	/// <summary>
	/// 决赛查看战报
	/// </summary>
	public void doRepalyEvent (GameObject obj)
	{
		UiManager.Instance.openDialogWindow<GodsWarReplayWindow> ((win) => {
			win.initWindow (finalPoints [0], () => {});});
	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{

		if (!enable)
			return;
		for (int i = 0; i < tabButtons.Length; i++) {
			if (tabButtons [i].gameObject == gameObj) {
			    if (i == 0)
			    {
                    mofangorshengwang.spriteName = "tianshen";
                    manager.tapIndex = 1;
			    }
				else
			    if (i == 1)
			    {
			       mofangorshengwang.spriteName="mowang";
					manager.tapIndex = 2; 
			    }  
				else if (i == 2) {
					doNotChangeTap ();	
				}
				//tabButtons[i].spriteBg.spriteName = tabButtons[i].spriteBg.spriteName.Replace("2","");
			} else if (!tabButtons [i].spriteBg.spriteName.EndsWith ("2"))
				tabButtons [i].spriteBg.spriteName = tabButtons [i].spriteBg.spriteName + "2";
		}
		MaskWindow.LockUI ();
		OnDataLoad ();
	}
	/// <summary>
	/// 诸神战未开启时，不切换界面
	/// </summary>
	public void doNotChangeTap ()
	{
		if (!isOpenShenMo && GodsWarManagerment.Instance.getGodsWarStateInfo () != GodsWarManagerment.STATE_HAVE_ZIGE_FINAL && GodsWarManagerment.Instance.getGodsWarStateInfo () != GodsWarManagerment.STATE_NOT_ZIGE_FINAL) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("godsWar_111"));
		} else
			manager.tapIndex = 0;
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
        OnDataLoad();
	    if (UiManager.Instance != null && UiManager.Instance.godsWarSuportWindow != null)
	    {
	        UiManager.Instance.godsWarSuportWindow.destoryWindow();
	        UiManager.Instance.godsWarSuportWindow = null;
	    } 
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			UiManager.Instance.openMainWindow ();
		} 
		if (gameObj.name == "buttonRank") {
			UiManager.Instance.openWindow<GodsWarSuportRankWindow> ((win) => {
				win.initWindow (type);
			});
		}
		if (gameObj.name == "buttonFinalAward") {
			UiManager.Instance.openWindow<GodsWarFinalRankWindow> ((win) => {
				win.initWindow (type);
			});
		}
		if (gameObj.name == "buttonTeam") {
			UiManager.Instance.openWindow<TeamEditWindow> ((wins) => {
				wins.comeFrom = TeamEditWindow.FROM_PVP;
			});
		}
		if (gameObj.name == "buttonHelp") {
			UiManager.Instance.openWindow<GodsWarPreparWindow> ();
		}
			
		if (gameObj.name == "mySuportButton")
		{
		    FPortManager.Instance.getFPort<GodsWarGetMySuportFport>().access(() =>
		    {
                UiManager.Instance.openDialogWindow<GodsWarMySuportWindow>((win) => {
                    win.initWindow(() => { });
                });
		    });	
		} else if (gameObj.name == "buttonMeritShop") {
			UiManager.Instance.openWindow<GodsWarShopWindow> ((win) => {
				win.initWindow (() => {});});
		}
	}

}
