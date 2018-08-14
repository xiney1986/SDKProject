using UnityEngine;
using System.Collections;

/// <summary>
/// 扫荡次数选择窗口
/// </summary>
public class SweepNumberWindow : WindowBase
{
	/**fileds */
    public UITexture bg1;
    public UITexture bg2;
	/*const */
	/**扫荡的最小数量和最大数量 */
	public  int MaxNum = 3;
	public const int MinNum = 1;
	/**人物头像 */
	public UITexture UI_Head;
	/**VIP图标 */
	public UISprite UI_VIPIcon;
	/**人物名字 */
	public UILabel UI_Name;
	public UILabel UI_Medal;
	public UISprite UI_StarIcon;
	public UILabel UI_StarLabel;
	/**等级label */
	public UILabel UI_LevelLabel;
	/**经验等级条 */
	public barCtrl UI_LevelSlider;
	/**PVE点数Bar */
	public barCtrl UI_PVEBar;
	public UILabel UI_PVETime;
	/**PVE点数 显示数值 */
	public UILabel UI_PVEValue;
	/**坐骑行动力点数bar */
	public barCtrl storeBar;
	/**坐骑行动力点数value */
	public UILabel store_value;
	/**坐骑行动力时间 */
	public UILabel store_time;
	/**PVP点数Bar */
	public GameObject[] UI_PVPBars;
	/**PVP点数恢复时间 */
	public UILabel UI_PVPTime;
	/**标题 */
	public UILabel titleText;
	/** 标题 分成三个部分 */
	public UILabel[] titleTextFB;
	/**扫荡数量 */
	public UILabel numberText;
	/**每次花费行动力或boss挑战次数 */
	public UILabel numPerTimeLabel;
	/** 花费的时间 */
	public UILabel timeLabel;//花费时间
	/**选择扫荡次数的滑动条 */
	public UISlider slider;
	/**三个扫荡说明描述 */
	public UILabel descript_1;
	public UILabel descript_2;
	public UILabel descript_3;
	public GameObject descriptOFf;
	public ButtonBase sweepButton;
	/**需要的行动力数 */
	public UILabel needCostText;
	/**玩家挑战是否可选 */
	public bool PvpTrigger = true;
	/**是否勾选进行玩家挑战 */
	public UIToggle pkCheckBox;
	private EnumSweep type;
	private Mission sweepMission;
	private int missionLevel;
	private  int  fubenPve ;
	private int  now = 1;
    private bool isCanSuccese = false;
	public GameObject fubenobj;
	public UILabel bossNumm;

	/*method*/
	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="maxPve">副本需求的行动力.</param>
	public void init (EnumSweep _type, int  _sid, int _level)
	{
		type = _type;
		sweepMission = MissionInfoManager.Instance.getMissionBySid (_sid);
		missionLevel = _level;
		switch (_type) {
		case EnumSweep.fuben:
			pkCheckBox.gameObject.SetActive (true);
			pkCheckBox.value = PvpTrigger;
			fubenobj.gameObject.SetActive(true);
			bossNumm.gameObject.SetActive(false);
			update_baseInfo ();
			update_fuben ();
			break;
		case EnumSweep.boss:
			pkCheckBox.gameObject.SetActive (false);
			fubenobj.gameObject.SetActive(false);
			bossNumm.gameObject.SetActive(true);
			update_baseInfo ();
			update_boss ();
			break;
		}
		slider.onDragFinished = valueChange;
		initSweepCount ();
		MaskWindow.UnlockUI ();
	}
	/** 第一次打开界面，默认最大可扫荡次数*/
	private void initSweepCount ()
	{
		int use = 0;
		switch (type) {
		case EnumSweep.fuben:
			MaxNum = missionLevel == 3 ? FuBenManagerment.Instance.getTimesByMissionSid (sweepMission.sid) : 3;
			use = Mathf.Min (Mathf.Max (1, (UserManager.Instance.self.getPvEPoint ()+UserManager.Instance.self.getStorePvEPoint()) / FuBenManagerment.Instance.getPveCostMissionSid (sweepMission.sid)), MaxNum);
			break;
		case EnumSweep.boss:
			Chapter chapter = FuBenManagerment.Instance.getWarChapter ();
			if(chapter.getNum()>=MaxNum)MaxNum=chapter.getNum();
			use = Mathf.Min (Mathf.Max (1, chapter.getNum ()), MaxNum);
			break;
		}
		slider.value = (float)(use * 1.0 / MaxNum);
	}
	private void valueChange ()
	{
		double one = 1.0 / MaxNum;
		float num = Mathf.Min ((float)(System.Math.Truncate (slider.value / one) + 1), (float)MaxNum);
		slider.value = (float)(num * one);
	}
	protected override void DoUpdate ()
	{
		base.DoUpdate ();
		updatePVE_PVP ();
		changeNeedNum();
	}
	/**更新一些基础信息 */
	private void update_baseInfo ()
	{
		LanguageConfigManager lang = LanguageConfigManager.Instance;
		User user = UserManager.Instance.self;
		UserManager.Instance.setSelfHeadIcon (UI_Head);
		UI_VIPIcon.gameObject.SetActive (user.vipLevel > 0);
		if (UI_VIPIcon.gameObject.activeSelf)
			UI_VIPIcon.spriteName = "vip" + user.vipLevel;
		UI_Name.text = lang.getLanguage ("pvpPlayerWindow01") + user.nickname;
		UI_Medal.text = LaddersConfigManager.Instance.config_Title.M_getTitle (UserManager.Instance.self.prestige).name;
		UI_StarIcon.spriteName = HoroscopesManager.Instance.getStarByType (user.star).getSpriteName ();
		UI_StarLabel.text = HoroscopesManager.Instance.getStarByType (user.star).getName ();
		UI_LevelLabel.text = "LV." + user.getUserLevel ();
		UI_LevelSlider.updateValue (user.getLevelExp (), user.getLevelAllExp ());
		updatePVE_PVP ();
	}
	/**这里时时更新行动力或boss挑战次数的恢复时间 */
	private void updatePVE_PVP ()
	{
		UI_PVEBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
		UI_PVEValue.text = UserManager.Instance.self.getPvEPoint () + "/" + UserManager.Instance.self.getPvEPointMax ();
		UI_PVETime.gameObject.SetActive (!UserManager.Instance.self.isPveMax ());
		if (UI_PVETime.gameObject.activeSelf)
			UI_PVETime.text = UserManager.Instance.getNextPveTime ().Substring (3);
		//////////////
		bool flag=UserManager.Instance.self.getStorePvEPoint()>0;
		UI_PVEBar.gameObject.SetActive(true);
		storeBar.gameObject.SetActive(true);

		//UI_PVPBar.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
		for(int ii=0;ii<UI_PVPBars.Length;ii++){
			if(UserManager.Instance.self.getPvPPoint ()>=(ii+1))UI_PVPBars[ii].SetActive(true);
			else UI_PVPBars[ii].SetActive(false);
		}
		if (UserManager.Instance.self.isPvpMax ()) {
			UI_PVPTime.gameObject.SetActive (false);
		} else {
			UI_PVPTime.gameObject.SetActive (true);
			UI_PVPTime.text = UserManager.Instance.getNextPvpTime ().Substring (3);
		}

		//if(flag){
			storeBar.updateValue (UserManager.Instance.self.getStorePvEPoint (), UserManager.Instance.self.getStorePvEPointMax ());
			store_value.text = UserManager.Instance.self.getStorePvEPoint () + "/" + UserManager.Instance.self.getStorePvEPointMax ();
			if (!UserManager.Instance.self.isPveMax () || UserManager.Instance.self.isStorePveMax ()) {
				store_time.gameObject.SetActive (false);
			} else {
				store_time.gameObject.SetActive (true);
				store_time.text = UserManager.Instance.getNextMountsPveTime ().Substring (3);
			}
		//}else{
			UI_PVEBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
			UI_PVEValue.text = UserManager.Instance.self.getPvEPoint () + "/" + UserManager.Instance.self.getPvEPointMax ();
//			if (UserManager.Instance.self.isPveMax ()) {
//				UI_PVETime.gameObject.SetActive (false);
//			} else {
//				UI_PVETime.gameObject.SetActive (true);
//				UI_PVETime.text = UserManager.Instance.getNextPveTime ().Substring (3);
//			}
		//}
	}
	/**对副本扫荡的信息更新 */
	private void update_fuben ()
	{
		int sid = sweepMission.sid;
		int pveCost = FuBenManagerment.Instance.getPveCostMissionSid (sid);
		int skipVipMinLevel = SweepConfigManager.Instance.skipStoryVipMinLevel;
		int sweepCostTime = SweepConfigManager.Instance.perStoryCdTime / 60;
		string[] missionDetailName = MissionInfoManager.Instance.getMissionDetailNameforFuben (sid, missionLevel);
		for(int i=0;i<missionDetailName.Length;i++){
			titleTextFB[i].text = missionDetailName[i];
		}
		numPerTimeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepCost_1", pveCost.ToString ());
		timeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepTime", sweepCostTime.ToString ());

		descript_1.text = LanguageConfigManager.Instance.getLanguage ("sweepTip_01");
		descript_2.text = LanguageConfigManager.Instance.getLanguage ("sweepTip_08", skipVipMinLevel.ToString ());
		descript_3.text = LanguageConfigManager.Instance.getLanguage ("sweepTip_15");

		if (pveCost <= 0) {
			Debug.LogError ("fuben need Pve <= 0 ?");
			return;
		}
		changeNum (MinNum);
		if (MinNum > 0) {
			slider.value = MinNum / (float)MaxNum;
		}
	}
	/**boss扫荡的信息更新 */
	private void update_boss ()
	{
		int sweepCostTime = SweepConfigManager.Instance.perBossCdTime / 60;
		int skipVipMinLevel = SweepConfigManager.Instance.skipBossVipMinLevel;

		titleText.text =sweepMission.getMissionName().Substring(0,sweepMission.getMissionName().Length-2);
		bossNumm.text = LanguageConfigManager.Instance.getLanguage ("s0146") + ":" + FuBenManagerment.Instance.getWarChapter () .getNum () + "/" + FuBenManagerment.Instance.getWarChapter ().getMaxNum ();
		numPerTimeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepCost_2", "1");
		timeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepTime", sweepCostTime.ToString ());

		descript_1.text = LanguageConfigManager.Instance.getLanguage ("sweepTip_01");
		descript_2.text = LanguageConfigManager.Instance.getLanguage ("sweepTip_08", skipVipMinLevel.ToString ());
		descript_3.text = LanguageConfigManager.Instance.getLanguage ("sweepTip_15");
		Chapter chapter = FuBenManagerment.Instance.getWarChapter ();
		changeNum (MinNum);
		if (MinNum > 0) {
			slider.value = MinNum / (float)MaxNum;
		}
	}
	/**滑动条更新 */
	public void onSliderChange ()
	{
		float current = slider.value;
		float per = 1.0f / (MaxNum - MinNum + 1);
		if (current == 0) {
			current = per;
		}
		int a = Mathf.CeilToInt (current / per);
		changeNum (a);
	}
	/**滑动挑战次数上面数值更新 */
	void changeNum (int num)
	{
		now = num;
		changeNeedNum();
		numberText.text = now.ToString ();
		if (missionLevel == 3 && FuBenManagerment.Instance.getTimesByMissionSid (sweepMission.sid) < now) {
			sweepButton.disableButton (true);
		} else {
			sweepButton.disableButton (false);
		}
	}
	/**滑动挑战次数上面的需要行动力或boss挑战次数数值更新 */
	void changeNeedNum(){
		if(type==EnumSweep.fuben){
			string numm=(now*FuBenManagerment.Instance.getPveCostMissionSid (sweepMission.sid)).ToString();
			if(StringKit.toInt(numm)>(UserManager.Instance.self.getPvEPoint()+UserManager.Instance.self.getStorePvEPoint()))numm=Colors.RED+numm;
				needCostText.text=LanguageConfigManager.Instance.getLanguage("sweepCost_3",numm);
		}else{
			string numBoss=now.ToString();
			if(now> FuBenManagerment.Instance.getWarChapter ().getNum())numBoss=Colors.RED+numBoss;
			needCostText.text=LanguageConfigManager.Instance.getLanguage("sweepCost_4",numBoss);
			
		}
	}
	protected override void begin ()
	{
		base.begin ();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.OTHER_TEXTURE + "waikuang",bg1);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.OTHER_TEXTURE + "neitu", bg2);
		MaskWindow.UnlockUI();
	}
    public override void OnNetResume() {
        base.OnNetResume();
        if (sweepMission != null) {
            if (!isCanSuccese) return;
            int armyId = ArmyManager.Instance.activeID;
            int triggerPKEvent = 0;
            if (type == EnumSweep.fuben) {
                triggerPKEvent = pkCheckBox.value ? 1 : 0;
            }
            SweepManagement.Instance.clearData();
            SweepBeginFPort fport = FPortManager.Instance.getFPort<SweepBeginFPort>();
            fport.startSweep(sweepMission.sid, missionLevel, armyId, now, triggerPKEvent, M_onNetResumeCallBack);
        }
    }
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name == "buttonOk") {
			if (now == 0) {
				return;
			}
			if (missionLevel == 3 && FuBenManagerment.Instance.getTimesByMissionSid (sweepMission.sid) < now) {
				TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("MISSION_ERROR_01"));
				return;
			}
			if (M_pvpEventConfirm ()) {
				M_doSweep ();
			}
		}
	}
	/**开始扫荡  */
	private void M_doSweep ()
	{
		bool checkOk = false;
		switch (type) {
		case EnumSweep.fuben:
			checkOk = M_fubenMissionConfirm ();
			break;
		case EnumSweep.boss:
			checkOk = M_bossMissionConfirm ();
			break;
		}
        isCanSuccese = checkOk;
		if (checkOk) {
			int armyId = ArmyManager.Instance.activeID;

			int triggerPKEvent = 0;
			if (type == EnumSweep.fuben) {
				triggerPKEvent = pkCheckBox.value ? 1 : 0;
			}
			SweepManagement.Instance.clearData ();
			SweepBeginFPort fport = FPortManager.Instance.getFPort<SweepBeginFPort> ();
			fport.startSweep (sweepMission.sid, missionLevel, armyId, now, triggerPKEvent, M_onServerCallBack);
		}
	}
    private void M_onNetResumeCallBack(string msg) {
        if (SweepManagement.Instance.hasSweepMission) {
            int minVipLevel = 0;
            switch (type) {
                case EnumSweep.boss:
                    minVipLevel = SweepConfigManager.Instance.skipBossVipMinLevel;
                    break;
                case EnumSweep.fuben:
                    minVipLevel = SweepConfigManager.Instance.skipStoryVipMinLevel;
                    break;
            }
            if (UserManager.Instance.self.getVipLevel() >= minVipLevel) {
                SweepManagement.Instance.initSweepAwardInfo();
            } else {
                UiManager.Instance.openWindow<SweepMainWindow>((win => {
                    win.M_init(sweepMission.sid, missionLevel);
                }));
                //finishWindow();
            }
        } else if (msg == "already_in_sweep") {
            SweepManagement.Instance.initSweepInfo(M_reCall);
            FuBenManagerment.Instance.sweepMission(sweepMission.sid, missionLevel, now);
        } else {
            finishWindow();
        }
    }
	/**后台通信回调 */
	private void M_onServerCallBack (string msg)
	{
		if (SweepManagement.Instance.hasSweepMission) {
			int minVipLevel = 0;
			switch (type) {
			case EnumSweep.boss:
				minVipLevel = SweepConfigManager.Instance.skipBossVipMinLevel;
				break;
			case EnumSweep.fuben:
				minVipLevel = SweepConfigManager.Instance.skipStoryVipMinLevel;
				break;
			}
			if (UserManager.Instance.self.getVipLevel () >= minVipLevel) {
				SweepManagement.Instance.initSweepAwardInfo ();
			} else {
				UiManager.Instance.openWindow<SweepMainWindow> ((win => {
					win.M_init (sweepMission.sid, missionLevel);
				}));
				//finishWindow();
			}
		} else if (msg == "already_in_sweep") {
			SweepManagement.Instance.initSweepInfo (M_reCall);
		} else {
			finishWindow ();
		}
	}
	/**如果已经在扫荡中，则重新请求扫荡信息 */
	private void M_reCall ()
	{
		if (SweepManagement.Instance.hasSweepMission) {
			if (SweepManagement.Instance.state == 0) {//如果当前扫荡状态 为扫荡中 即为0
				M_onServerCallBack (string.Empty);
			} else if (SweepManagement.Instance.state == 1) {//如果扫荡状态为扫荡结束 即为1 
				UiManager.Instance.openWindow<SweepAwardWindow> ();
			}
		} else {
			finishWindow ();
		}
	}
	/**查看是否还有玩家挑战 */
	private bool M_pvpEventConfirm ()
	{
		if (type != EnumSweep.fuben) {
			return true;
		}		
		if (PvpInfoManagerment.Instance.getPvpTime (null) > 0) {
			M_showPvpConfirm ();
			return false;
		} else {
			return true;
		}
	}
	/**提示框,有玩家挑战 PK时间*/
	private void M_showPvpConfirm ()
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => { 
			string info = LanguageConfigManager.Instance.getLanguage ("sweepTip_05");
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), info, M_onMessageHandler);
		});
	}
	/// <summary>
	/// M选择框回调
	/// </summary>
	/// <param name="msg">Message.</param>
	private void M_onMessageHandler (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			//finishWindow();
			return;
		}

		PvpInfoManagerment.Instance.clearDate ();
		M_doSweep ();
	}
	/**副本里面能不能进行扫荡 */
	private bool M_fubenMissionConfirm ()
	{
		int sid = sweepMission.sid;
		int pveCost = FuBenManagerment.Instance.getPveCostMissionSid (sid);
		int currentPve = UserManager.Instance.self.getPvEPoint ()+UserManager.Instance.self.getStorePvEPoint();

		if (currentPve >= pveCost * now) {
			return true;
		}
		int totalPveMax=CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample>(CommonConfigSampleManager.PvePowerMax_SID).pvePowerMax;
		int currentMaxPve = totalPveMax+UserManager.Instance.self.getStorePvEPointMax();
		string tipInfo = string.Empty;
		if (currentMaxPve < pveCost * now) {
			tipInfo = LanguageConfigManager.Instance.getLanguage ("sweepTip_14");
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, tipInfo, null);
			});
			return false;
		}


		tipInfo = LanguageConfigManager.Instance.getLanguage ("sweepTip_02", (pveCost * now - currentPve).ToString ());
		UiManager.Instance.openDialogWindow<PveUseWindow> ((win) => {
			win.updateTipInfo (tipInfo);
		});

		return false;
	}
	/**boss副本里面能不能进行扫荡 */
	private bool M_bossMissionConfirm ()
	{

		Chapter chapter = FuBenManagerment.Instance.getWarChapter ();
		int currentTimes = chapter.getNum ();
		if (currentTimes >= now) {
			return true;
		}
		int viplv = UserManager.Instance.self.getVipLevel ();
		if (viplv <= 0) {
			UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("s0153"), null);
			return false;
		}
		int canBuyCount = FuBenManagerment.Instance.getWarChapter ().getCanBuyNum ();
		if (canBuyCount <= 0) {
			UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("s0385"), null);		
			return false;
		}
		int gapNum = FuBenManagerment.Instance.getWarChapter ().getMaxNum () - FuBenManagerment.Instance.getWarChapter ().getNum ();
		UiManager.Instance.openDialogWindow<BuyWindow> ((window) => {
			BuyWindow.BuyStruct buyStruct = new BuyWindow.BuyStruct ();
			buyStruct.iconId = ResourcesManager.ICONIMAGEPATH + "87";
			buyStruct.unitPrice = BossViewWinItem.PRICE_BOSS_COUNT;
			window.init (buyStruct, Mathf.Min (canBuyCount, gapNum), 1, PrizeType.PRIZE_RMB, (msg) => {			
				if (msg.msgEvent != msg_event.dialogCancel) {
					if (msg.msgNum * BossViewWinItem.PRICE_BOSS_COUNT > UserManager.Instance.self.getRMB ())
						MessageWindow.ShowRecharge (LanguageConfigManager.Instance.getLanguage ("s0158"));
					else {
						FuBenBuyWarNumFPort port = FPortManager.Instance.getFPort ("FuBenBuyWarNumFPort") as FuBenBuyWarNumFPort;
						port.buyNum (M_buyNumCallBack, msg.msgNum);
					}
				}
			});
		});
		return false;
	}
	/// <summary>
	/// 购买讨伐次数
	/// </summary>
	/// <param name="buyCount">购买的数量</param>
	public void M_buyNumCallBack (int buyCount)
	{
		if (buyCount > 0) {
			UiManager.Instance.openDialogWindow <MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), "", LanguageConfigManager.Instance.getLanguage ("s0155", buyCount + ""), null);
			});
			FuBenManagerment.Instance.getWarChapter ().addBuyed (buyCount);
		} else {
			UiManager.Instance.openDialogWindow <MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0385"), null);
			});
		}
	}
	///<summary>
	/// 关闭无关描述
	/// </summary>
	public void setDescriptOff(){
		descriptOFf.SetActive (false);
	}
}
