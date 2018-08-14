using UnityEngine;
using System.Collections;

/**
 * PVP信息窗口
 * @author 汤琦
 * */
public class PvpInfoWindow : WindowBase
{
	public UILabel rankValue;
	public UILabel winValue;
	public UILabel integralValue;
	public UILabel integralRankValue;
	public UILabel pkTime;
	private string timeLabel;
	public ButtonBase challengeButton;//挑战按钮

	private Timer timer;

	protected override void DoEnable ()
	{
		base.DoEnable ();
		//默认关闭挑战按钮
		challengeButton.disableButton (true);
	}

	protected override void begin ()
	{
		base.begin ();
		rankValue.text = PvpInfoManagerment.Instance.getPvpRankInfo ().rank.ToString ();
		winValue.text = PvpInfoManagerment.Instance.getPvpWinNum ().ToString ();
		updatePvpTime ();
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
		timer.addOnTimer (updatePvpTime);
		timer.start ();
		MaskWindow.UnlockUI ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
//		PvpInfoManagerment.Instance.getPvpTime (null);
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
				challengeButton.disableButton (false);
			} else {
				timeLabel = LanguageConfigManager.Instance.getLanguage ("s0215");
				challengeButton.disableButton (true);
				if (timer != null) {
					timer.stop ();
					timer = null;
				}
			}	
			pkTime.text = timeLabel;
		} else {
			challengeButton.disableButton (true);
			pkTime.text = LanguageConfigManager.Instance.getLanguage ("s0222");
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
		}
	}
	  
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if(gameObj.name == "close"){
			finishWindow ();
			MaskWindow.UnlockUI();
			return 	;
		}
		else if (gameObj.name == "challengeButton") {
			if (PvpInfoManagerment.Instance.getPvpInfo () == null)
				return;

			if (PvpInfoManagerment.Instance.getPvpInfo ().rule == "match") {
				EventDelegate.Add (OnHide, () => {
					UiManager.Instance.openWindow<PvpMainWindow> ();
				});

			} else if (PvpInfoManagerment.Instance.getPvpInfo ().rule == "cup") {
				EventDelegate.Add (OnHide, () => {
					UiManager.Instance.openWindow<PvpCupWindow> ();
				});
			} 
		} else if (gameObj.name == "prizeButton") {
			EventDelegate.Add (OnHide, () => {
					
				UiManager.Instance.openWindow<PvpPrizeWindow> ();
					
			});


		} else if (gameObj.name == "rankButton") {
			MaskWindow.LockUI ();
			EventDelegate.Add (OnHide, () => {
				UiManager.Instance.openWindow<RankWindow> ();
			});
		}  
		finishWindow ();


	}
	


}
