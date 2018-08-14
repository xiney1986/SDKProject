using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SweepEventWindow : WindowBase
{
	
	public DelegatedynamicContent awardContent;//奖励容器
	public GameObject goodsPrefab;//奖励预制体
	public ButtonBase luckStarButton;
	public ButtonBase pvpButton;
	public UILabel luckStarNumLabel;
	public UILabel pvpNumLabel;
	private List<PrizeSample> awardList;//转换后的奖励列表
	private Award award;//副本奖励实体
	
	
	private string timeLabel;
	public UILabel pkTime;
	private Timer timer;

	protected override void begin ()
	{
		base.begin ();
		awardContent.SetUpdateItemCallback (onUpdateItem);
		awardContent.SetinitCallback (initItem);

		updateScrapContent ();
		starMethod ();
		pvpNumMethod ();
		
		
		pkTime.gameObject.SetActive (false);
		startTimer ();
		MaskWindow.UnlockUI ();
	}

	public override void OnOverAnimComplete ()
	{
		base.OnOverAnimComplete ();
		awardContent.cleanAll ();
		stopTimer ();
	}

	private void startTimer ()
	{
		if (timer == null) {
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
			timer.addOnTimer (updatePvpTime);
			timer.start ();
		}
	}

	private void stopTimer ()
	{
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}

	private void updatePvpTime ()
	{
		if (PvpInfoManagerment.Instance.getPvpInfo () != null) {
			int pvpTime = PvpInfoManagerment.Instance.getPvpTime (null);
			if (pvpTime > 0) {
				int minute = pvpTime / 60;
				string minuteStr;
				int second = pvpTime % 60;
				string secondStr;
				if (minute < 10) {
					minuteStr = "0" + minute;
				} else {
					minuteStr = minute.ToString ();
				}
				if (second < 10) {
					secondStr = "0" + second;
				} else {
					secondStr = second.ToString ();
				}
				timeLabel = minuteStr + " : " + secondStr;
				if (pkTime != null) {	
					if (!pkTime.gameObject.activeSelf) {
						pkTime.gameObject.SetActive (true);
					}
					pkTime.text = timeLabel;
				}			
			} else {
				pkFinish ();
			}
		} else {		
			pkFinish ();
		}
	}

	private void pkFinish ()
	{
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
	
	void updateScrapContent ()
	{
		award = SweepManagement.Instance.getPveAward ();
		if (award != null) {
			awardList = AllAwardViewManagerment.Instance.exchangeAwardToPrize (award);
			awardContent.reLoad (awardList.Count);
		} else {
			awardContent.cleanAll ();	
		}
	}
	
	GameObject onUpdateItem (GameObject item, int i)
	{
		if (item == null) {
			item = NGUITools.AddChild (awardContent.gameObject, goodsPrefab);
		}
		
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (awardList [i]);
		
		return item;
	}
	GameObject initItem (  int i)
	{
		GameObject	item = NGUITools.AddChild (awardContent.gameObject, goodsPrefab);
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (awardList [i]);
		
		return item;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (BattleManager.isWaitForBattleData) {
			MaskWindow.UnlockUI ();
			return;
		}
		if (gameObj.name == "close") {
			UiManager.Instance.openWindow<MainWindow> ((win) => {
				SweepManagement.Instance.clearPvpAward ();
			});
		} else if (gameObj.name == "buttonOver") {
			if (PvpInfoManagerment.Instance.getPvpTime (null) > 0 || SweepManagement.Instance.getPvpNum () > 0) {
				UiManager.Instance.openDialogWindow<MessageWindow> (
					(win) => {
					string info = LanguageConfigManager.Instance.getLanguage ("sweepTip_07");
					
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"),
					                info, over);
				});
			} else {
				finishSweep ();
			}
		} else if (gameObj.name == "pkButton") {
			BattleGlobal.pvpMode = EnumPvp.sweep;
			
			PvpInfo pvpInfo = PvpInfoManagerment.Instance.getPvpInfo ();
			if (pvpInfo != null && PvpInfoManagerment.Instance.getPvpTime (null) > 0) {
				PvpRankInfoFPort port1 = FPortManager.Instance.getFPort<PvpRankInfoFPort> ();
				port1.access (() => {
					startTimer ();
					PvpInfoManagerment.Instance.setPvpType (PvpInfo.TYPE_PVP_SWEEP);
					UiManager.Instance.openDialogWindow<PvpInfoWindow> ();
				});
			} else {
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
						UiManager.Instance.openDialogWindow<PvpInfoWindow> ();
					});
					
				});
			}
		} else if (gameObj.name == "starButton") {
			UiManager.Instance.openWindow<BattleDrawWindow> ();
		}
	}
	
	void over (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			MaskWindow.UnlockUI ();
			return;
		}
		finishSweep ();
	}
	
	private void finishSweep ()
	{
		SweepFinishFPort port = FPortManager.Instance.getFPort<SweepFinishFPort> ();
		port.finish (() => {
			BattleGlobal.pvpMode = EnumPvp.nomal;
			PvpInfoManagerment.Instance.clearDate ();
			SweepManagement.Instance.clearData ();
			UiManager.Instance.openWindow<MainWindow> ((win) => {
				SweepManagement.Instance.clearPvpAward ();
			});
			//等级引导跳转
			if (!GuideManager.Instance.isGuideComplete ()) {
				UiManager.Instance.initNewPlayerGuideLayer ();
				GuideManager.Instance.openGuideMask ();
			}
		});
	}
	
	//幸运星数量
	void starMethod ()
	{
			FubenGetStarFPort userFPort = FPortManager.Instance.getFPort ("FubenGetStarFPort") as FubenGetStarFPort;
			userFPort.getStar (UserManager.Instance.self.initStarNum, updateStar);//初始化用户的星星数
	}

	private void updateStar ()
	{
		luckStarNumLabel.transform.parent.gameObject.SetActive (UserManager.Instance.self.starSum / 100 > 0);
		luckStarNumLabel.text = (UserManager.Instance.self.starSum / 100).ToString ();
	}
	
	void pvpNumMethod ()
	{
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

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		BattleManager.isWaitForBattleData = false;
		UiManager.Instance.createMessageLintWindow (Language ("reconnection_03"));
	}
}
