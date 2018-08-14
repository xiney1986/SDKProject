using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChapterSelectWindow : WindowBase
{
    public GameObject UI_ActivityTips;
	public GameObject activityTips;
	public GameObject lastBattleTips;
	public GameObject punitiveTips;
	public GameObject huoDongTips;
	public GameObject oneOnOneBossTips;

	public ButtonBase punitiveButton;//讨伐按钮
	public ButtonBase activityButton;//活动按钮
	public ButtonBase practiceButton;//修炼按钮
	public ButtonBase oneOnOneBossButton;//单挑Boss按钮
    public ButtonBase clmbTowerButton;//爬塔按钮
	public UILabel activityText;//活动显示是否开启文字
	public UILabel punitiveText;//讨伐显示次数文字
	public UILabel practiceText;//修炼显示次数文字
    public UILabel challengeTimes;//单挑boss次数

	// 调整ui后文字描述和按钮//
	public UILabel new_punitiveText;//讨伐显示次数文字
	public UILabel new_practiceText;//修炼显示次数文字
	public ButtonBase new_punitiveButton;//讨伐按钮
	public ButtonBase new_activityButton;//活动按钮
	public ButtonBase new_practiceButton;//修炼按钮
	public ButtonBase new_oneOnOneBossButton;//单挑Boss按钮
	public ButtonBase new_clmbTowerButton;//爬塔按钮
	public ButtonBase new_lastBattleButton;// 末日决战按钮//
	public GameObject oneOnOneBossMask;// 未开放遮罩//
	public GameObject lastBattleMask;// 未开放遮罩//
	public GameObject clmbTowerMask;// 未开放遮罩//
	public UILabel oneOnOneBossLV;// 开放等级//
	public UILabel lastBattleLV;// 开放等级//
	public UILabel clmbTowerLV;// 开放等级//

	private int mapId = 1;
	private Chapter[] activitys;//缓存当前活动列表
    public ContentChapterSelect content;
    /** 计时器 */
    private Timer timer;

	private bool isShowPractice;

	public ButtonBase zhuXianBtn;// 主线剧情按钮//
	public ButtonBase huoDongBtn;// 活动按钮//
	public UILabel zhuXianLabel;
	public UILabel huoDongLabel;
	public GameObject zhuXianContent;// 主线剧情content//
	public GameObject huoDongContent;// 活动content//

	Vector3 topPosition;// 选项置顶位置//
	Vector3 old_oneOnOneBossPos;
	Vector3 old_lastBattlePos;
	public Transform oneOnOneBossPos;// 单挑boss选项位置//
	public Transform lastBattlePos;// 末日决战选项位置//
	public Transform practisePos;// 女神试炼选项位置 默认在第一个//


	private void updateButtonPostion(){
		if(GuideManager.Instance.isEqualStep(126003000)){//可以显示讨伐
			new_punitiveButton.transform.parent=huoDongContent.transform;
			new_punitiveButton.transform.localPosition=new Vector3(0f,0f,0f);			
		}
        else if (GuideManager.Instance.isEqualStep(103003000)) {
            new_punitiveButton.transform.parent = huoDongContent.transform;
            new_punitiveButton.transform.localPosition = new Vector3(0f, 0f, 0f);
			new_practiceButton.transform.parent=huoDongContent.transform;
			new_practiceButton.transform.localPosition=new Vector3(0f,-180f,0f);
		}
        else if (GuideManager.Instance.isEqualStep(120003000)) {
            new_punitiveButton.transform.parent = huoDongContent.transform;
            new_punitiveButton.transform.localPosition = new Vector3(0f, 0f, 0f);
            new_practiceButton.transform.parent = huoDongContent.transform;
            new_practiceButton.transform.localPosition = new Vector3(0f, -180f, 0f);
			new_activityButton.transform.parent=huoDongContent.transform;
			new_activityButton.transform.localPosition=new Vector3(0f,-360f,0f);			
		}
        else if (GuideManager.Instance.isEqualStep(831003000))
	    {
            new_punitiveButton.transform.parent = huoDongContent.transform;
            new_punitiveButton.transform.localPosition = new Vector3(0f, 0f, 0f);
            new_practiceButton.transform.parent = huoDongContent.transform;
            new_practiceButton.transform.localPosition = new Vector3(0f, -180f, 0f);
            new_activityButton.transform.parent = huoDongContent.transform;
            new_activityButton.transform.localPosition = new Vector3(0f, -360f, 0f);
            new_oneOnOneBossButton.transform.parent = huoDongContent.transform;
            new_oneOnOneBossButton.transform.localPosition = new Vector3(0f, -540f, 0f);
	    }
        else if (GuideManager.Instance.isEqualStep(140003000)) {
            new_punitiveButton.transform.parent = huoDongContent.transform;
            new_punitiveButton.transform.localPosition = new Vector3(0f, -720f, 0f);
            new_practiceButton.transform.parent = huoDongContent.transform;
            new_practiceButton.transform.localPosition = new Vector3(0f, -180f, 0f);
            new_activityButton.transform.parent = huoDongContent.transform;
            new_activityButton.transform.localPosition = new Vector3(0f, -360f, 0f);
            new_oneOnOneBossButton.transform.parent = huoDongContent.transform;
            new_oneOnOneBossButton.transform.localPosition = new Vector3(0f, -540f, 0f);
			new_clmbTowerButton.transform.parent=huoDongContent.transform;
			new_clmbTowerButton.transform.localPosition=new Vector3(0f,0f,0f);
		}else 
		{
		    if (UserManager.Instance.self.getUserLevel()>=11)
		    {
                new_punitiveButton.transform.parent = huoDongContent.transform;
                new_punitiveButton.transform.localPosition = new Vector3(0f, 0f, 0f);
		    }
            if (UserManager.Instance.self.getUserLevel() >= 16) {
                new_practiceButton.transform.parent = huoDongContent.transform;
                new_practiceButton.transform.localPosition = new Vector3(0f, -180f, 0f);
            }
            if (UserManager.Instance.self.getUserLevel() >= 24) {
                new_activityButton.transform.parent = huoDongContent.transform;
                new_activityButton.transform.localPosition = new Vector3(0f, -360f, 0f);
            }
            if (UserManager.Instance.self.getUserLevel() >= 30) {
                new_oneOnOneBossButton.transform.parent = huoDongContent.transform;
                new_oneOnOneBossButton.transform.localPosition = new Vector3(0f, -540f, 0f);
            }
            if (UserManager.Instance.self.getUserLevel() >= 40) {
                new_clmbTowerButton.transform.parent = huoDongContent.transform;
                new_clmbTowerButton.transform.localPosition = new Vector3(0f, -720f, 0f);
            }
            if (UserManager.Instance.self.getUserLevel() >= 42) {
                new_lastBattleButton.transform.parent = huoDongContent.transform;
                new_lastBattleButton.transform.localPosition = new Vector3(0f, -900f, 0f);
            }
		}

	}
	private void updateButtonNoGuide(){
		new_punitiveButton.transform.parent=huoDongContent.transform;
		new_punitiveButton.transform.localPosition=new Vector3(0f,0f,0f);
		new_practiceButton.transform.parent=huoDongContent.transform;
		new_practiceButton.transform.localPosition=new Vector3(0f,-180f,0f);
		new_activityButton.transform.parent=huoDongContent.transform;
		new_activityButton.transform.localPosition=new Vector3(0f,-360f,0f);
		new_oneOnOneBossButton.transform.parent=huoDongContent.transform;
		new_oneOnOneBossButton.transform.localPosition=new Vector3(0f,-540f,0f);
		new_clmbTowerButton.transform.parent=huoDongContent.transform;
		new_clmbTowerButton.transform.localPosition=new Vector3(0f,-720f,0f);
		new_lastBattleButton.transform.parent=huoDongContent.transform;
		new_lastBattleButton.transform.localPosition=new Vector3(0f,-900f,0f);
	}
	void updateFirendButton(){
		new_punitiveButton.transform.parent=huoDongContent.transform;
		new_punitiveButton.transform.localPosition=new Vector3(0f,-900f,0f);
		new_practiceButton.transform.parent=huoDongContent.transform;
		new_practiceButton.transform.localPosition=new Vector3(0f,-180f,0f);
		new_activityButton.transform.parent=huoDongContent.transform;
		new_activityButton.transform.localPosition=new Vector3(0f,-360f,0f);
		new_oneOnOneBossButton.transform.parent=huoDongContent.transform;
		new_oneOnOneBossButton.transform.localPosition=new Vector3(0f,-540f,0f);
		new_clmbTowerButton.transform.parent=huoDongContent.transform;
		new_clmbTowerButton.transform.localPosition=new Vector3(0f,-720f,0f);
		new_lastBattleButton.transform.parent=huoDongContent.transform;
		new_lastBattleButton.transform.localPosition=new Vector3(0f,-0f,0f);
	}
    protected override void begin() {
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
        base.begin();
        new_punitiveButton.transform.localPosition = new Vector3(10000f, 10000f, 0f);
        new_practiceButton.transform.localPosition = new Vector3(10000f, 10000f, 0f);
        new_activityButton.transform.localPosition = new Vector3(10000f, 10000f, 0f);
        new_oneOnOneBossButton.transform.localPosition = new Vector3(10000f, 10000f, 0f);
        new_clmbTowerButton.transform.localPosition = new Vector3(10000f, 10000f, 0f);
        new_lastBattleButton.transform.localPosition = new Vector3(10000f, 10000f, 0f);
		if(!GameManager.Instance.skipGuide){
			updateButtonPostion();
		}else{
			updateButtonNoGuide();
		}

        if (GuideManager.Instance.getOnTypp() == 41 && GuideManager.Instance.isMoriJuezhan)
        {
            updateFirendButton();
        }
        else
        {
            DateTime dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
            int[] data = CommandConfigManager.Instance.getOneOnOneBossData();
            // 末日决战选项置顶//
            if (UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.lastBattleData.openLevel && TimeKit.getWeekCHA(dt.DayOfWeek) == CommandConfigManager.Instance.lastBattleData.dayOfWeek) {
                new_lastBattleButton.transform.parent = huoDongContent.transform;
                new_lastBattleButton.transform.localPosition = new Vector3(0f, -0f, 0f);
                new_punitiveButton.transform.parent = huoDongContent.transform;
                new_punitiveButton.transform.localPosition = new Vector3(0f, -900f, 0f);
            }
            // 单挑boss置顶//          
            else if (UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.getOneOnOneBossLimitLv()) {
                for (int i = 0; i < data.Length; i++) {
                    if (TimeKit.getWeekCHA(dt.DayOfWeek) == data[i]) {
                        new_punitiveButton.transform.parent = huoDongContent.transform;
                        new_punitiveButton.transform.localPosition = new Vector3(0f, -540f, 0f);
                        new_oneOnOneBossButton.transform.parent = huoDongContent.transform;
                        new_oneOnOneBossButton.transform.localPosition = new Vector3(0f, 0f, 0f);
                    }
                }
            }
        }
        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(refreshTips);
        timer.start();
		initFubenData ();
		if (GuideManager.Instance.isEqualStep(123001000) || GuideManager.Instance.isEqualStep(140003000)||
		    GuideManager.Instance.isEqualStep(126003000)||GuideManager.Instance.isEqualStep(103003000)||
		    GuideManager.Instance.isEqualStep(120003000)) {
			GuideManager.Instance.guideEvent ();
		}
		GuideManager.Instance.doFriendlyGuideEvent();
	}
    public void refreshTips() {
        if (this == null || !gameObject.activeInHierarchy) {
            if (timer != null) {
                timer.stop();
                timer = null;
            }
            return;
        }
        //punitiveButton.gameObject.transform.FindChild("Tips").gameObject.SetActive(false);
		//punitiveTips.SetActive(false);
		lastBattleTips.SetActive(false);
		oneOnOneBossTips.SetActive(false);
        DateTime dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());//获取服务器时间
		//showPuntiveTips(dt);
		if(UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.lastBattleData.openLevel && TimeKit.getWeekCHA(dt.DayOfWeek) == CommandConfigManager.Instance.lastBattleData.dayOfWeek)
		{
			lastBattleTips.SetActive(true);
		}
		int[] data = CommandConfigManager.Instance.getOneOnOneBossData();
		if(UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.getOneOnOneBossLimitLv())
		{
			for(int i = 0; i < data.Length; i++)
			{
				if(TimeKit.getWeekCHA(dt.DayOfWeek) == data[i])
				{
					oneOnOneBossTips.SetActive(true);
				}
			}
		}
    }
	void showPuntiveTips(DateTime dt)
	{
		string week = dt.DayOfWeek.ToString();
		if (UserManager.Instance.self.getUserLevel() > WarChooseWindow.LIMITLEVEL && (week == "Friday" || week == "Saturday" || week == "Sunday")) {
			Chapter chapterWar = FuBenManagerment.Instance.getWarChapter ();
			punitiveTips.SetActive(chapterWar.num < CommandConfigManager.Instance.getTaoFaMaxCount());
		}
		else
		{
			punitiveTips.SetActive(false);
		}
	}
	public void showPractice () {
		isShowPractice = true;
	}

	protected override void DoEnable ()
	{
		topPosition = practisePos.localPosition;
		old_oneOnOneBossPos = oneOnOneBossPos.localPosition;
		old_lastBattlePos = lastBattlePos.localPosition;

		base.DoEnable (); //2014.7.2 modified
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
        //爬塔副本按钮显示
        showClmbTowerButtonInfo();
        //初始化单挑boss信息
        showOneOnOneBossInfo();
		showLastBattle();
        if (timer != null) {
            timer.stop();
            timer = null;
        }
        refreshTips();
		//UiManager.Instance.backGroundWindow.switchToDark();
	}
    public override void DoDisable() {
        base.DoDisable();
        if (timer != null) {
            timer.stop();
            timer = null;
        }
		rePosition();
    }
	
	//初始化副本信息.获得玩家历史数据
	private void initFubenData ()
	{
		FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
		port.info (loadData, ChapterType.STORY);  
	}

	//请求数据完成
	private void loadData ()
	{
		MissionAwardFPort fport = FPortManager.Instance.getFPort ("MissionAwardFPort") as MissionAwardFPort;
		fport.getMissionInfo(()=>{
			showChapter();
			showTop ();
			if (isShowPractice) intoPracticeFuben ();
		});
	}
	//重新请求数据（处理断线重连）
	public void loadDataForOnNet()
	{
		MissionAwardFPort fport = FPortManager.Instance.getFPort ("MissionAwardFPort") as MissionAwardFPort;
		fport.getMissionInfo(()=>{
		});
	}
	//显示章节
	private void showChapter()
	{
		List<ChapterSample> cs = new List<ChapterSample>();
		int[] chapterIDs = FuBenManagerment.getAllShowStoryChapter (1);
		for (int i = 0; i < chapterIDs.Length; i++) {
			ChapterSample sample = ChapterSampleManager.Instance.getChapterSampleBySid (chapterIDs[i]);
			cs.Add(sample);
		}
		cs.Reverse();
		if(content.gameObject.activeSelf)
		{
			content.init(cs);
		}
		MaskWindow.UnlockUI ();
	}

	//显示顶部按钮信息
	private void showTop ()
	{
		//显示讨伐副本信息按钮
		showPunitive ();
		//显示修炼副本信息按钮
		showPracticeButtonInfo ();
		//初始化活动副本信息
		initActivityInfo ();
	}

	//讨伐副本信息按钮
	private void showPunitive ()
	{
		Chapter chapterWar = FuBenManagerment.Instance.getWarChapter ();
		if (chapterWar != null) {
//			punitiveText.gameObject.SetActive (true);
//			punitiveText.text = Language ("s0144") + " " + chapterWar.getNum () + "/" + chapterWar.getMaxNum ();
			new_punitiveText.gameObject.SetActive(true);
			new_punitiveText.text = chapterWar.getNum () + "/" + chapterWar.getMaxNum ();
			//punitiveButton.gameObject.SetActive (true);
			showPuntiveTips(TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime()));
		} else {
			punitiveButton.gameObject.SetActive (false);
		}
	}
    //玩家等级是否达到副本开启等级
    private void showClmbTowerButtonInfo() {
        int limit = CommandConfigManager.Instance.getTowerLimitLevel();
        if (UserManager.Instance.self.getUserLevel() < limit) {
            clmbTowerButton.gameObject.SetActive(false);
			clmbTowerMask.SetActive(true);
			clmbTowerLV.text = string.Format(LanguageConfigManager.Instance.getLanguage("openLV"),limit.ToString());
			clmbTowerLV.gameObject.SetActive(true);
        } else {
            //clmbTowerButton.gameObject.SetActive(true);
			clmbTowerMask.SetActive(false);
			clmbTowerLV.gameObject.SetActive(false);
        }
    }
	//修炼副本信息按钮
	private void showPracticeButtonInfo ()
	{
		//practiceButton.gameObject.SetActive (true);
		Chapter chapter = FuBenManagerment.Instance.getPracticeChapter ();
		if (chapter != null) {
			//practiceText.text = Language ("s0592") + " " + FuBenManagerment.Instance.getPracticeChapter ().getNum () + "/" + FuBenManagerment.Instance.getPracticeChapter ().getMaxNum ();
			new_practiceText.text = FuBenManagerment.Instance.getPracticeChapter ().getNum () + "/" + FuBenManagerment.Instance.getPracticeChapter ().getMaxNum ();
		} else {
			//practiceText.gameObject.SetActive (false);
			new_practiceText.gameObject.SetActive(false);
		}
	}
    private void showOneOnOneBossInfo() {
        oneOnOneBossButton.gameObject.SetActive(false);
        if (UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.getOneOnOneBossLimitLv()) {
            //oneOnOneBossButton.gameObject.SetActive(true);
			oneOnOneBossMask.SetActive(false);
			oneOnOneBossLV.gameObject.SetActive(false);
        }
		else
		{
			oneOnOneBossMask.SetActive(true);
			oneOnOneBossLV.text = string.Format(LanguageConfigManager.Instance.getLanguage("openLV"),CommandConfigManager.Instance.getOneOnOneBossLimitLv().ToString());
			oneOnOneBossLV.gameObject.SetActive(true);
		}
//        challengeTimes.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_001", ("8" + "/" + CommandConfigManager.Instance.getTimesOfDay()));
    }
	private void showLastBattle()
	{
		if(UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.lastBattleData.openLevel)
		{
			lastBattleMask.SetActive(false);
			lastBattleLV.gameObject.SetActive(false);
		}
		else
		{
			lastBattleMask.SetActive(true);
			lastBattleLV.text = string.Format(LanguageConfigManager.Instance.getLanguage("openLV"),CommandConfigManager.Instance.lastBattleData.openLevel.ToString());
			lastBattleLV.gameObject.SetActive(true);
		}
	}
	//初始化活动信息，从第一个开始循环获取，获取完全部信息后显示界面
	private void  initActivityInfo ()
	{
		FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
		port.info (activityBack, ChapterType.ACTIVITY_CARD_EXP);
	}

	//活动返回
	private void activityBack ()
	{
		//暂时处理，不理解委托的主类销毁，委托不执行
		if (UiManager.Instance.getWindow<ChapterSelectWindow> ()) { 
			activitys = FuBenManagerment.Instance.getSortByTimeChapter (); 
			//activityButton.gameObject.SetActive (true);
			activityText.gameObject.SetActive (true);
			if (FuBenManagerment.Instance.HasActivityOpen ()) {
				activityText.text = LanguageConfigManager.Instance.getLanguage ("s0148");
			} else {
				activityText.text = LanguageConfigManager.Instance.getLanguage ("S0360");
			}

            /////tips
            ActivityChapter[] infos = FuBenManagerment.Instance.getActivityChapters();
            int cur = 0;
			int[] missions;
			Mission mission;
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].isOpen() && infos[i].getNum() > 0)
                {
					missions = FuBenManagerment.Instance.getAllShowMissions (infos[i].sid);
					for(int j=0;j<missions.Length;j++)
					{
						mission = MissionInfoManager.Instance .getMissionBySid (missions[j]) ;
						if(mission != null && UserManager.Instance.self.getUserLevel() >= mission.getRequirLevel())
						{
							cur++;
							break;
						}
					}
                }
            }
            //UI_ActivityTips.SetActive(cur > 0);
			activityTips.SetActive(cur > 0);
		}
		showHuoDongBtnTips();
		MaskWindow.UnlockUI ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "close") {
			finishWindow ();
		}
		else if (gameObj.name == "crusade") {
			GuideManager.Instance.doGuide ();
			FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
			port.info (intoWarFuben, ChapterType.WAR);
		}
		else if (gameObj.name == "activity") {
			GuideManager.Instance.doGuide ();
			intoActivityFuben ();
		}
		else if (gameObj.name == "practice") {
			GuideManager.Instance.doGuide ();
			intoPracticeFuben ();
        } else if (gameObj.name == "clmbTower") {//进入爬塔界面
			if(UserManager.Instance.self.getUserLevel() < CommandConfigManager.Instance.getTowerLimitLevel())
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("OneOnOneBoss_level_limit"));
				});
				MaskWindow.UnlockUI();
				return;
			}
            FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
            port.info(intoTowerFuben, ChapterType.TOWER_FUBEN);
        } else if (gameObj.name == "oneOnOneBoss") {//进入单挑boss界面
			if(UserManager.Instance.self.getUserLevel() < CommandConfigManager.Instance.getOneOnOneBossLimitLv())
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("OneOnOneBoss_level_limit"));
				});
				MaskWindow.UnlockUI();
				return;
			}
            UiManager.Instance.openWindow<OneOnOneBossWindow>();
			//TextTipWindow.Show (Language ("S0360"));
		}
		else if(gameObj.name == "lastBattle")
		{
			GuideManager.Instance.doFriendlyGuideEvent();
			if(UserManager.Instance.self.getUserLevel() < CommandConfigManager.Instance.lastBattleData.openLevel)
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("OneOnOneBoss_level_limit"));
				});
				MaskWindow.UnlockUI();
				return;
			}
			LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
			init.lastBattleInitAccess(()=>{
				UiManager.Instance.openWindow<LastBattleWindow>();
			});
		}
		else if(gameObj.name == "zhuXianBtn")
		{

			showZhuXianContent();
		}
		else if(gameObj.name == "huoDongBtn")
		{
			showHuoDongContent();
		}
		else {
			ChapterSelectItemView item = gameObj.GetComponent<ChapterSelectItemView> ();
			if (item != null) {
				GuideManager.Instance.doGuide ();
				FuBenManagerment.Instance.selectedChapterSid = item.data.sid;
				FuBenManagerment.Instance.selectedMapSid = item.data.missions [0];
				UiManager.Instance.openWindow<MissionChooseWindow> ();
			}
		}
	}
    /// <summary>
    /// 进入爬塔界面
    /// </summary>
    private void intoTowerFuben() {
        //添加过程记录
        if (FuBenManagerment.Instance.getTowerChapter() == null) return;
        FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getTowerChapter().sid;//爬塔副本章节sid
        FuBenManagerment.Instance.selectedMapSid = mapId;
        GuideManager.Instance.doGuide();
        UiManager.Instance.openWindow<ClmbTowerChooseWindow>();
    }

	//进入活动的副本
	private void intoActivityFuben ()
	{
		//添加过程记录
		FuBenManagerment.Instance.selectedMapSid = mapId;		
		//下面才开始打开活动副本的章节选择
		UiManager.Instance.openWindow<ActivityChapterWindow> ();		
	}

	//进入讨伐副本
	private void intoWarFuben ()
	{
		if (FuBenManagerment.Instance.getWarChapter () == null)
			return;	
		
		//添加过程记录
		FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getWarChapter ().sid;
		FuBenManagerment.Instance.selectedMapSid = mapId;
		UiManager.Instance.openWindow<WarChooseWindow> ();	
	}

	//进入修炼的副本
	private void intoPracticeFuben ()
	{
		//添加过程记录
		FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getPracticeChapter ().sid;
		FuBenManagerment.Instance.selectedMapSid = mapId;

		UiManager.Instance.openWindow<TeamPrepareWindow> ((win) => {
			win.Initialize (FuBenManagerment.Instance.selectedChapterSid, TeamPrepareWindow.WIN_PRACTICE_ITEM_TYPE);
		});
	}

	public override void OnOverAnimComplete ()
	{
		base.OnOverAnimComplete ();
		content.cleanAll();
	}

	public void showZhuXianContent()
	{
		zhuXianContent.SetActive(true);
		zhuXianBtn.disableButton(true);
		zhuXianLabel.color = TapContentBase.activeLabelColor;
		huoDongBtn.disableButton(false);
		huoDongLabel.color = TapContentBase.normalLabelColor;
		huoDongContent.SetActive(false);
		if(zhuXianContent.transform.childCount <= 0)
		{
			initFubenData();
		}
		MaskWindow.UnlockUI ();
	}

	public void showHuoDongContent()
	{
		zhuXianBtn.disableButton(false);
		zhuXianLabel.color = TapContentBase.normalLabelColor;
		huoDongBtn.disableButton(true);
		huoDongLabel.color = TapContentBase.activeLabelColor;
		huoDongContent.SetActive(true);
		zhuXianContent.SetActive(false);
		if (GuideManager.Instance.isEqualStep(123001000) || GuideManager.Instance.isEqualStep(140003000)||
		    GuideManager.Instance.isEqualStep(126003000)||GuideManager.Instance.isEqualStep(103003000)||
		    GuideManager.Instance.isEqualStep(120003000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent();
		}
		GuideManager.Instance.doFriendlyGuideEvent();
		MaskWindow.UnlockUI ();
	}

	public void showHuoDongBtnTips()
	{
		if(activityTips.activeSelf || lastBattleTips.activeSelf || punitiveTips.activeSelf || oneOnOneBossTips.activeSelf)
		{
			huoDongTips.SetActive(true);
		}
		else 
		{
			huoDongTips.SetActive(false);
		}
	}

	void rePosition()
	{
		practisePos.localPosition = topPosition;
		oneOnOneBossPos.localPosition = old_oneOnOneBossPos;
		lastBattlePos.localPosition = old_lastBattlePos;
	}
}
