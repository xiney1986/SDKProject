using UnityEngine;
using System.Collections.Generic;

public class DoubleRMBContent : MonoBase
{
	public ButtonBase UI_BtnSure;
	public UILabel UI_Time;
    public UILabel desc;
    public UILabel btnSureName;
    public GameObject noOpenTip;
    public UILabel time;
	private NoticeWindow mFatherWindow;
	private int mTime;
	private string mTipsStr;
	private ActiveTime activeTime;
	private DoubleRMBNotice notice;
	private Timer timer;

	private void Start ()
	{
		UIEventListener.Get (UI_BtnSure.gameObject).onClick = onClickSure;
		showTime ();
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (showTime);
		timer.start ();

	}
//
//    private void OnEnable()
//    {
//        bool isRecharge = DoubleRMBManagement.Instance.IsRecharge;
//        if (isRecharge)
//        {
//            UI_BtnSure.disableButton(true);
//            UI_BtnSure.textLabel.text = Language("recharge02");
//        }
//    }
//

	private void onClickSure (GameObject go)
	{
		MaskWindow.LockUI ();
		UiManager.Instance.openWindow<rechargeWindow> (); 
	}

	private void showTime ()
	{
		int now = ServerTimeKit.getSecondTime ();
		int start = activeTime.getDetailStartTime ();
		int end = activeTime.getDetailEndTime ();
        noOpenTip.SetActive(false);
        if (now < start) {
            mTipsStr = "";
			//mTipsStr = Language ("doubleRMB_02") + TimeKit.timeTransformDHMS (start - now);
            noOpenTip.SetActive(true);
            time.text = Language("doubleRMB_02") + "\n" + TimeKit.timeTransformDHMS(start - now);
            desc.text = LanguageConfigManager.Instance.getLanguage("doubleRMB_06");
			UI_BtnSure.disableButton(true);
            UI_BtnSure.gameObject.SetActive(false);
		} else if (start <= now && now <= end) {
            UI_BtnSure.gameObject.SetActive(true);
			mTipsStr = Language ("doubleRMB_03") + TimeKit.timeTransformDHMS (end - now);
            desc.text = LanguageConfigManager.Instance.getLanguage("doubleRMB_07");
			UI_BtnSure.disableButton(false);
		} else {
            UI_BtnSure.gameObject.SetActive(true);
            mTipsStr = Language("doubleRMB_04");
            btnSureName.text = LanguageConfigManager.Instance.getLanguage("doubleRMB_08");
            UI_BtnSure.disableButton(true);
			timer.stop ();
			timer = null;
		}
        if (DoubleRMBManagement.Instance.isEnd) {
            mTipsStr = Language("doubleRMB_04");
            desc.text = LanguageConfigManager.Instance.getLanguage("doubleRMB_08");
            btnSureName.text = LanguageConfigManager.Instance.getLanguage("doubleRMB_09");
            UI_BtnSure.disableButton(true);
        } else {
            btnSureName.text = LanguageConfigManager.Instance.getLanguage("s0324");
        }
		UI_Time.text = mTipsStr;
	}

	public void initContent (NoticeWindow win, Notice notice)
	{
		mFatherWindow = win;
		this.notice = notice as DoubleRMBNotice;
		activeTime = ActiveTime.getActiveTimeByID (notice.sid);
		NoticeSample sample = notice.getSample ();
	}

	void OnDestroy ()
	{
		if (timer != null) {
            timer.stop();
            TimerManager.Instance.removeTimer(timer);
			timer = null;
		}
	}
}

