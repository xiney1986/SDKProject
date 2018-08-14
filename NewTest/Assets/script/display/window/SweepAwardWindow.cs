using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 扫荡奖励窗口
 * @author 陈世惟
 * */
public class SweepAwardWindow : WindowBase {

	/*fields */
	/** 奖励容器*/
	public DelegatedynamicContent awardContent;
	/**奖励预制体 */
	public GameObject goodsPrefab;
	/**星削 */
	public UILabel starLabel;
	/**幸运星 */
	public UILabel luckyStarLabel;
	/**转换后的奖励列表 */
	private List<PrizeSample> awardList;
	/**副本奖励实体 */
	private Award award;
	/**icon节点里面的 */
	/**幸运星点击按钮 */
	public ButtonBase luckStarButton;
	/**挑战点击按钮 */
	public ButtonBase pvpButton;
	/**幸运星个数显示label */
	public UILabel luckStarNumLabel;
	/**挑战次数显示label */
	public UILabel pvpNumLabel;
	/**PK倒计时显示label */
	public UILabel pkTime;
	/**原来走完流程那一套 */
	public ButtonBase btn_Finish;
	/*整合界面以后的完成界面 */
	public ButtonBase btn_Finish_fuben;
	/*已经领取label*/
	public UILabel isDone;
	private Timer timer;
	private string timeLabel;
	private bool awardIsDone=false;
	
	
	public override void OnStart () {
		if (SweepManagement.Instance.type == EnumSweep.fuben) {
			btn_Finish.gameObject.SetActive (false);
			btn_Finish_fuben.gameObject.SetActive (true);
		}
		else {
			btn_Finish.gameObject.SetActive (true);
			btn_Finish_fuben.gameObject.SetActive (false);
		}
	}
	
	protected override void begin () {
		base.begin ();
		awardContent.SetUpdateItemCallback (onUpdateItem);
		awardContent.SetinitCallback (initItem);
		isDone.gameObject.SetActive(false);
		if(awardIsDone){
			isDone.gameObject.SetActive(true);
		}
		if (!isAwakeformHide) {		
			updateScrapContent ();
			updatePVPAward ();
		}
		starMethod ();
		pvpNumMethod ();
		if(BattleManager.isWaitForBattleData == false){
			MaskWindow.UnlockUI ();
		}

		if (SweepManagement.Instance.isTempAdd ()) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0122"));
			SweepManagement.Instance.setTempStoreageType ();
		}

        if (UiManager.Instance.getWindow<PvpEventAwardWindow>() != null) {
            UiManager.Instance.getWindow<PvpEventAwardWindow>().gameObject.SetActive(true);
        }
	}
    public override void DoDisable() {
        base.DoDisable();
        if(UiManager.Instance.getWindow<PvpEventAwardWindow>() != null)
            UiManager.Instance.getWindow<PvpEventAwardWindow>().gameObject.SetActive(false);
    }
	public void  setDondShow(bool isShow){
		this.awardIsDone=isShow;
	}
	public void initWindow (Award _awards) {
		this.award = _awards;
	}
	/*更新中间副本扫荡结束奖励 */
	void updateScrapContent () {
		if (award != null)
			updateContent ();
		

	}
	/*更新中间副本扫荡结束奖励(容器更新) */
	private void updateContent () {
		awardList = AllAwardViewManagerment.Instance.exchangeAwardToPrize (award);
		awardList = AllAwardViewManagerment.Instance.contrastToList (awardList);
		awardList = AllAwardViewManagerment.Instance.Sort (awardList);
		awardContent.reLoad (awardList.Count);
		if (award != null && !award.isNull ()) {
			starLabel.text = (award.starGap > 0 ? award.starGap : 0).ToString ();
			luckyStarLabel.text = (award.luckyStarGap > 0 ? award.luckyStarGap : 0).ToString ();
		}
	}
	/**有没有PVP奖励 有的话就弹框 */
	private void updatePVPAward () {
		Award awards = SweepManagement.Instance.getPveAward ();
		if (awards != null) {
			string str = getAwardNum ();
			UiManager.Instance.openDialogWindow<PvpEventAwardWindow> ((pvpwin) => {
				pvpwin.init (StringKit.toInt (str), awards);
				PvpInfoManagerment.Instance.clearDate ();
			});
		}
	}
	GameObject onUpdateItem (GameObject item, int i) {
		PrizeSample prizeSample = awardList [i];
		if (item == null) {
			item = NGUITools.AddChild (awardContent.gameObject, goodsPrefab);
		}
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	GameObject initItem (int i) {
		PrizeSample prizeSample = awardList [i];
		
		GameObject item = NGUITools.AddChild (awardContent.gameObject, goodsPrefab);
		
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "close" || gameObj.name == "buttonNext") {
			if (award != null) {
				UiManager.Instance.openDialogWindow<MessageLineWindow> ((winn) => {
					winn.Initialize (LanguageConfigManager.Instance.getLanguage 
					                 ("sweepTip_19"));
				});
			}
			UiManager.Instance.openWindow<MainWindow> ((win) => {
				SweepManagement.Instance.clearPvpAward ();
			});
		}
		else if (gameObj.name == "buttonfinish") {
			finishSweep ();
		}
		else if (gameObj.name == "pkButton") {
			BattleGlobal.pvpMode = EnumPvp.sweep;			
			PvpInfo pvpInfo = PvpInfoManagerment.Instance.getPvpInfo ();
			if (pvpInfo != null && PvpInfoManagerment.Instance.getPvpTime (null) > 0) {
				PvpRankInfoFPort port1 = FPortManager.Instance.getFPort<PvpRankInfoFPort> ();
				port1.access (() => {
					startTimer ();
					PvpInfoManagerment.Instance.setPvpType (PvpInfo.TYPE_PVP_SWEEP);
					UiManager.Instance.openDialogWindow<PvpInfoWindow> ((win)=>{
						win.dialogCloseUnlockUI=false;
					});
				});
			}
			else {
				SweepPKFPort port = FPortManager.Instance.getFPort<SweepPKFPort> ();
				port.getPK (() => {
					PvpInfoManagerment.Instance.clearDate ();
					PvpInfoManagerment.Instance.initPvpEvent ();
					PvpInfoManagerment.Instance.setPvpType (PvpInfo.TYPE_PVP_SWEEP);
					SweepManagement.Instance.usePvpNum ();
					PvpRankInfoFPort port1 = FPortManager.Instance.getFPort<PvpRankInfoFPort> ();
					port1.access (() => {
						startTimer ();
						PvpInfoManagerment.Instance.setPvpType (PvpInfo.TYPE_PVP_SWEEP);
						UiManager.Instance.openDialogWindow<PvpInfoWindow> ((win)=>{
							win.dialogCloseUnlockUI=false;
						});
					});
					
				});
			}
		}
		else if (gameObj.name == "starButton") {
			UiManager.Instance.openWindow<BattleDrawWindow> ();
		}
		else if (gameObj.name == "buttonOver") {
			if (PvpInfoManagerment.Instance.getPvpTime (null) > 0 || SweepManagement.Instance.getPvpNum () > 0) {
				UiManager.Instance.openDialogWindow<MessageWindow> (
					(win) => {
					string info = LanguageConfigManager.Instance.getLanguage ("sweepTip_07");
					
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"),
					                info, over);
				});
			}
			else {
				finishSweep ();
			}
		}
	}
	/**PVP可挑战次数 */
	void pvpNumMethod () {
		if (pvpNumLabel == null) {
			return;
		}
		
		int pvpCount = SweepManagement.Instance.getPvpNum ();
		if (PvpInfoManagerment.Instance.getPvpTime (null) > 0) {
			pvpCount++;
		}
		
		pvpNumLabel.transform.parent.gameObject.SetActive (pvpCount > 0);
		pvpNumLabel.text = pvpCount.ToString ();
	}
	/**更新幸运星数量 */
	private void starMethod () {
		if (UserManager.Instance.self.getlastStarSum () == 0 || (award != null && award.luckyStarGap > 0)) {
			FubenGetStarFPort userFPort = FPortManager.Instance.getFPort ("FubenGetStarFPort") as FubenGetStarFPort;
			userFPort.getStar (UserManager.Instance.self.initStarNum, updateStar);//初始化用户的星星数
		}
		else {
			updateStar ();
		}
	}
	/**更新星星数量 */
	private void updateStar () {
		luckStarNumLabel.transform.parent.gameObject.SetActive (UserManager.Instance.self.getlastStarSum () / 100 > 0);
		luckStarNumLabel.text = (UserManager.Instance.self.getlastStarSum () / 100).ToString ();
	}
	/**计时器 挑战剩余时间记录时间 */
	private void startTimer () {
		if (timer == null) {
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
			timer.addOnTimer (updatePvpTime);
			timer.start ();
		}
	}
	/**更新挑战剩余时间方法 */
	private void updatePvpTime () {
		if (PvpInfoManagerment.Instance.getPvpInfo () != null) {
			int pvpTime = PvpInfoManagerment.Instance.getPvpTime (null);
			if (pvpTime > 0) {
				int minute = pvpTime / 60;
				string minuteStr;
				int second = pvpTime % 60;
				string secondStr;
				if (minute < 10) {
					minuteStr = "0" + minute;
				}
				else {
					minuteStr = minute.ToString ();
				}
				if (second < 10) {
					secondStr = "0" + second;
				}
				else {
					secondStr = second.ToString ();
				}
				timeLabel = minuteStr + " : " + secondStr;
				if (pkTime != null) {	
					if (!pkTime.gameObject.activeSelf) {
						pkTime.gameObject.SetActive (true);
					}
					pkTime.text = timeLabel;
				}			
			}
			else {
				pkFinish ();
			}
		}
		else {		
			pkFinish ();
		}
	}
	/**挑战完成方法 */
	private void pkFinish () {
		if (PvpInfoManagerment.Instance != null) {
			PvpInfoManagerment.Instance.clearDate ();
		}
		
		if (pkTime != null) {
			pkTime.gameObject.SetActive (false);
		}
		pvpNumMethod ();
		
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
	void over (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			//MaskWindow.UnlockUI ();
			return;
		}
		finishSweep ();
	}
	/*点完成按钮以后的动作 */
	private void finishSweep () {
		SweepFinishFPort port = FPortManager.Instance.getFPort<SweepFinishFPort> ();
		port.finish (() => {
			BattleGlobal.pvpMode = EnumPvp.nomal;
			PvpInfoManagerment.Instance.clearDate ();
			SweepManagement.Instance.clearData ();
			//等级引导跳转
			if (!GuideManager.Instance.isGuideComplete ()) {
				UiManager.Instance.initNewPlayerGuideLayer ();
				GuideManager.Instance.openGuideMask ();
				UiManager.Instance.openWindow<MainWindow> ();
			}
			else {
				EventDelegate.Add (onDestroy, () => {
					if (UiManager.Instance.getWindow<TeamPrepareWindow> () != null) {
						TeamPrepareWindow fwin = UiManager.Instance.getWindow<TeamPrepareWindow> ();
						if (fwin.getShowType () == TeamPrepareWindow.WIN_MISSION_ITEM_TYPE) {
							fwin.updateMissionInfo ();
							fwin.missionWinItem.GetComponent<MissionWinItem> ().updateButton (fwin.getMission ());
						}
					}
				});
				finishWindow ();
			}
		});
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume () {
		base.OnNetResume ();
		BattleManager.isWaitForBattleData = false;
		UiManager.Instance.createMessageLintWindow (Language ("reconnection_03"));
	}
	/*得到连胜次数 */
	private string getAwardNum () {	   
		return PvpInfoManagerment.Instance.getPvpRankInfo ().win + 3 + "";
	}

	GameObject initItem (GameObject obj, int i) {
		PrizeSample prizeSample = awardList [i];
		
		GameObject item = NGUITools.AddChild (awardContent.gameObject, goodsPrefab);
		
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	protected override void DoEnable () {
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}
	public override void OnBeginCloseWindow () {
		base.OnBeginCloseWindow ();

		MaskWindow.LockUI();
	}
}