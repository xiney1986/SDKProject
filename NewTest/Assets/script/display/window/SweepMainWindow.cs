using UnityEngine;
using System.Collections;
/// <summary>
/// 扫荡进行中窗口
/// </summary>
public class SweepMainWindow : WindowBase {

	/**fields */
	/**PVE行动力bar */
	public barCtrl UI_PVEBar;
	/**PVE行动力恢复时间显示label */
	public UILabel UI_PVETime;
	/**PVE行动力数值 */
	public UILabel UI_PVEValue;
	/**PVP挑战次数的bar */
	public PvpPowerBar UI_PVPBar;
	/**PVP挑战次数恢复时间显示label */
	public UILabel UI_PVPTime;
	/**标题 那个副本那个章节 那个难度 */
	public UILabel titleText;
	/**把标题分成3个label 在副本扫荡中使用 */
	public UILabel[] titleTextFB;
	/**每次花费行动力,boss挑战次数 */
	//public UILabel numPerTimeLabel;
	/**花费的时间 */
	//public UILabel timeLabel;
	/**倒计时 */
	public UILabel label_time;
	public UILabel sweepTip;
	/**完成的扫荡次数 */
	public UILabel label_times;
	public UISprite hardSprint;
	public GameObject doingg;
	private  int  fubenPve ;
	private EnumSweep type;
	private int missionLevel;
	private Mission mission;
	private int difficulty;
	private bool needUpdate;

	/*method */
	/// <summary>
	/// 初始化
	/// </summary>
	public void M_init (int _missionSid, int _difficulty) {
		mission = MissionInfoManager.Instance.getMissionBySid (_missionSid);
		int chapterType = mission.getChapterType ();
		if (chapterType == ChapterType.STORY) {
			type = EnumSweep.fuben;
		}
		else if (chapterType == ChapterType.WAR) {
			type = EnumSweep.boss;
		}
		else {
			Debug.LogError ("Invalid Mission Type=" + chapterType);
			return;
		}
		difficulty = _difficulty;
		int skipVip = 0;
		switch (type) {
		case EnumSweep.fuben:
			update_fuben ();
			skipVip = SweepConfigManager.Instance.skipStoryVipMinLevel;
			sweepTip.text=LanguageConfigManager.Instance.getLanguage("sweepTip_20");
			break;
		case EnumSweep.boss:
			update_boss ();
			skipVip = SweepConfigManager.Instance.skipBossVipMinLevel;
			sweepTip.text=LanguageConfigManager.Instance.getLanguage("sweepTip_21");
			break;
		}
		init (type, difficulty);
		needUpdate = SweepManagement.Instance.M_getNeedUpdateFlag (type);
		int cdTimePre = SweepManagement.Instance.SweepCostTime;
		int minutes = cdTimePre / 60;
		
		M_udpateView ();
		M_updateLoading ();
		
	}
	/*更新副本名字 */
	private void M_updateTitle_Fuben () {
		string missionName = MissionInfoManager.Instance.getMissionDetailName (mission.sid, difficulty);
		titleText.text = missionName;
	}
	/*更新Boss名字 */
	private void M_updateTitle_Boss () {
		string name = mission.getMissionName ();
		titleText.text = name;
	}
	public void init (EnumSweep _type, int _level) {	
		switch (_type) {
		case EnumSweep.fuben:
			update_baseInfo ();
			update_fuben ();
			break;
		case EnumSweep.boss:
			update_baseInfo ();
			update_boss ();
			break;
		}
	}
	/**更新倒计时和完成扫荡的次数 */
	private void M_udpateView () {
		label_time.text = SweepManagement.Instance.M_getTimeInfo (type);
		label_times.text = SweepManagement.Instance.M_getTimesInfo (type).ToString(); 
	}
	/**更新loading图标 表示还在扫荡中 */
	private void M_updateLoading () {
		if(needUpdate)doingg.SetActive(true);
		else doingg.SetActive(false);
	}
	protected override void DoUpdate () {
		base.DoUpdate ();
		updatePVE_PVP ();
		if (needUpdate && Time.frameCount % 30 == 0) {
			needUpdate = SweepManagement.Instance.M_getNeedUpdateFlag (type);
			M_udpateView ();
			if (!needUpdate) {
				M_updateLoading ();
			}
		}
	}
	/**更新一些基础的信息 包括行动力 挑战次数等等 */
	private void update_baseInfo () {
		LanguageConfigManager lang = LanguageConfigManager.Instance;
		User user = UserManager.Instance.self;
		//updatePVE_PVP ();
	}
	/**更新行动力 挑战次数的两个bar */
	private void updatePVE_PVP () {
//		UI_PVEBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
//		UI_PVPBar.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
//		UI_PVEValue.text = UserManager.Instance.self.getPvEPoint () + "/" + UserManager.Instance.self.getPvEPointMax ();
//		UI_PVETime.gameObject.SetActive (!UserManager.Instance.self.isPveMax ());
//		if (UI_PVETime.gameObject.activeSelf)
//			UI_PVETime.text = UserManager.Instance.getNextPveTime ().Substring (3);
//		if (UserManager.Instance.self.isPvpMax ()) {
//			UI_PVPTime.gameObject.SetActive (false);
//		} else {
//			UI_PVPTime.gameObject.SetActive (true);
//			UI_PVPTime.text = UserManager.Instance.getNextPvpTime ().Substring (3);
//		}
	}
	/**如果是副本扫荡 则更新相关信息 */
	private void update_fuben () {
		int sid = mission.sid;
		int pveCost = FuBenManagerment.Instance.getPveCostMissionSid (sid);
		int skipVipMinLevel = SweepConfigManager.Instance.skipStoryVipMinLevel;
		int sweepCostTime = SweepConfigManager.Instance.perStoryCdTime / 60;
		string[] missionDetailName = MissionInfoManager.Instance.getMissionDetailNameforFuben (sid, difficulty);
		for (int i=0; i<titleTextFB.Length; i++) {
			titleTextFB [i].text = missionDetailName [i];
		}
		//numPerTimeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepCost_1", pveCost.ToString ());
		//timeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepTime", sweepCostTime.ToString ());
		if(missionDetailName[2].Substring(1,2)==LanguageConfigManager.Instance.getLanguage("s0450").Substring(0,2))hardSprint.spriteName="common";
		if(missionDetailName[2].Substring(1,2)==LanguageConfigManager.Instance.getLanguage("s0451").Substring(0,2))hardSprint.spriteName="hard";
		if(missionDetailName[2].Substring(1,2)==LanguageConfigManager.Instance.getLanguage("s0452").Substring(0,2))hardSprint.spriteName="nightmare";
		
		if (pveCost <= 0) {
			Debug.LogError ("fuben need Pve <= 0 ?");
			return;
		}
	}
	/**更新boss副本相关的信息 */
	private void update_boss () {
		int sweepCostTime = SweepConfigManager.Instance.perBossCdTime / 60;
		int skipVipMinLevel = SweepConfigManager.Instance.skipBossVipMinLevel;
		titleText.text =mission.getMissionName().Substring(0,mission.getMissionName().Length-2);
		//numPerTimeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepCost_2", "1");
		//timeLabel.text = LanguageConfigManager.Instance.getLanguage ("sweepTime", sweepCostTime.ToString ());
		Chapter chapter = FuBenManagerment.Instance.getWarChapter ();
	}
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "button_close") {
			UiManager.Instance.openWindow<MainWindow> ();
		}
		else if (gameObj.name == "button_checkOut") {
			
			bool vipOk = M_checkUserVipLevelOk ();
			if (vipOk) {
				SweepManagement.Instance.initSweepAwardInfo ();
				return;
			}
			
			if (needUpdate) {
				string currentTimsInfo = SweepManagement.Instance.M_getTimesInfo (type);
				string info = LanguageConfigManager.Instance.getLanguage ("sweepTip_03", currentTimsInfo);
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => { 
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), info, M_onMessageHandler);
				});
			}
			else {
				SweepManagement.Instance.initSweepAwardInfo ();
			}
		}
		else if (gameObj.name == "button_finish") {
			bool vipOk = M_checkUserVipLevelOk ();
			if (vipOk) {
				SweepManagement.Instance.initSweepAwardInfo ();
			}
			else {
				if (needUpdate) {

					//UiManager.Instance.openWindow<rechargeWindow> (); 
					UiManager.Instance.openDialogWindow<VipInfoWindow>((win)=>{
						win.initInfo(type);
					});
				}
				else {
					SweepManagement.Instance.initSweepAwardInfo ();
				}
			}
		}
	} 
	/**检查玩家pvp等级和扫荡的需求等级 */
	private bool M_checkUserVipLevelOk () {
		int vipMinLevel = 0;
		if (type == EnumSweep.fuben) {
			vipMinLevel = SweepConfigManager.Instance.skipStoryVipMinLevel;
		}
		else if (type == EnumSweep.boss) {
			vipMinLevel = SweepConfigManager.Instance.skipBossVipMinLevel;
		}		
		return UserManager.Instance.self.getVipLevel () >= vipMinLevel;
	}
	/**更新奖励物品信息 */
	private void M_onMessageHandler (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		SweepManagement.Instance.initSweepAwardInfo ();
	}
}
