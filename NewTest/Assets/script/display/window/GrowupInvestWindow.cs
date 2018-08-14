using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// growup投资窗口
/// </summary>
public class GrowupInvestWindow: WindowBase { 

	/* fields */
	//投资项目
	public UIToggle invest500;
	public UIToggle invest2000;
	/** 定时器 */
	private Timer timer;
	/** 活动结束时间 */
	int awardCloseTime;
	public UILabel countdownTimeLabel;

	/** method */
	/** begin */
	protected override void begin () {
		base.begin ();
		if (GrowupAwardMangement.Instance.GetAwardList () == null ) {
			GrowupAwardMangement.Instance.InitAwards (doBegin);
		}else{
			doBegin();
		}
	}
	/** 执行begin */
	private void doBegin () {
//		updateTimer ();
		MaskWindow.UnlockUI ();
	}
	//断线重新连接
	public override void OnNetResume () {
		base.OnNetResume ();
		doBegin ();
	}
	/***/
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		}
		else if (gameObj.name == "button_info") {
			UiManager.Instance.openDialogWindow<GrowupRebateWindow> ();
		}
		else if (gameObj.name == "button_invest") {
			int invest_value = 0;
			if (!(invest500.value || invest2000.value)) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("gi004"));
				return;
			}
			else if (invest500.value) {
				invest_value = 15000;
			}
			else if (invest2000.value) {
				invest_value = 50000;
			}
			if (UserManager.Instance.self.getRMB () < invest_value) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("gi007"));
			}else if(invest_value == 15000 && UserManager.Instance.self.vipLevel < 6){
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("gi010"));
			}else if(invest_value == 50000 && UserManager.Instance.self.vipLevel < 7){
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("gi013"));
			}
			else {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("gi008", invest_value.ToString ()), (handleMsg) => {
						if (handleMsg.buttonID == MessageHandle.BUTTON_RIGHT) {
//							win.finishWindow ();
							GrowupAwardMangement.Instance.Invest (invest_value, () => {
								if(GrowupAwardMangement.Instance.investStatas == "ok"){
									UiManager.Instance.switchWindow<GrowupAwardWindow> ();
								}else{
									UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("gi009"));
								}

							});
						}
					});
				});
			}
		}
		else if (gameObj.name == "button_recharge") {
			UiManager.Instance.openWindow<rechargeWindow> ();
		}
	}
	/** 更新计时器 */
//	private void updateTimer () {
//		if (!GrowupAwardMangement.Instance.isValid ()) {
//			return;
//		}
//		if (timer == null) {
//			setAwardOpenTime ();
//			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
//			timer.addOnTimer (refreshAwardTime);
//			timer.start (true);
//		}
//		else {
//			refreshAwardTime ();
//		}
//	}
	/** 设置活动开启时间 */
//	public void setAwardOpenTime () {
//		awardCloseTime = GrowupAwardMangement.Instance.activeTime.getDetailEndTime ();
//	}
	/**刷新时间*/
//	private void refreshAwardTime () {
//		long remainCloseTime = awardCloseTime - ServerTimeKit.getSecondTime ();
//		if (remainCloseTime >= 0) {
//			countdownTimeLabel.gameObject.SetActive (true);
//			countdownTimeLabel.text = TimeKit.timeTransformDHMS (remainCloseTime);
//		}
//		else {
//			countdownTimeLabel.gameObject.SetActive (false);
//		}
//	}
	/** 消耗 */
	public override void DoDisable () {
		base.DoDisable ();
//		clear ();
	}
	/** 清理 */
//	private void clear () {
//		if (timer != null)
//			timer.stop ();
//		timer = null;
//	}
}