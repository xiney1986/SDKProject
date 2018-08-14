using UnityEngine;
using System.Collections;

public class NoticeConsumeRebateContent : MonoBase
{
	public ContentNoticeActivity
		content;
	public MessageHandle tmpMsg;
	public Notice notice;
	public GameObject  noticeActivityShopPrefab;
	public UILabel desLabel;
	public UILabel timeLabel;
	public ButtonBase rechargeButton;
	public UILabel rmbNumLabel;
    public GameObject offect;
    public UILabel timeNoOpen;
    public GameObject noOpenObj;

	[HideInInspector]
	public WindowBase
		win;//活动窗口
	public NoticeActiveServerInfo serverInfo;
	private Timer timer;
	private ActiveTime activeTime;
	public void refreshContent ()
	{
		initContent (notice, win);
	}

	public void initContent (Notice notice, WindowBase win)
	{
		this.notice = notice;
		this.win = win;
		rechargeButton.fatherWindow = win;
		rechargeButton.onClickEvent = (gameobj) => {
			UiManager.Instance.openWindow<rechargeWindow> ();
		};
		rmbNumLabel.text = "X " + UserManager.Instance.self.getRMB ();
		FPortManager.Instance.getFPort<NoticeActiveGetFPort> ().access (notice.sid, () => {
			serverInfo = NoticeActiveManagerment.Instance.getActiveInfoBySid (notice.sid) as NoticeActiveServerInfo;
            content.initContent(notice, win, this);
            content.Initialize();
            initDesTime ();
		});
	}

	private void initDesTime ()
	{
		//能进到这里说明可以看到
		desLabel.text = notice.getSample ().activiteDesc;
	    if (timer == null)
            timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
            timer.addOnTimer(updateTime);
            timer.start(); 
			
	}

	private void updateTime ()
	{
		activeTime = (notice as ConsumeRebateNotice).activeTime;
		activeTime.doRefresh ();
		int now = ServerTimeKit.getSecondTime ();
		if (activeTime.getEndTime () == 0)
			timeLabel.text = Language ("notice03");
		else if (now < activeTime.getDetailStartTime ()) {
			//活动还未开启
            timeNoOpen.text = Language("ConsumeRebate_05", TimeKit.timeTransformDHMS(activeTime.getDetailStartTime() - now));
            timeLabel.gameObject.SetActive(false);
            offect.SetActive(false);
            noOpenObj.SetActive(true);
			//timeLabel.text = Language ("ConsumeRebate_05", TimeKit.timeTransformDHMS (activeTime.getDetailStartTime () - now));
		}
		else if (activeTime.getDetailStartTime () <= now && now < activeTime.getDetailEndTime ()) {
            timeLabel.gameObject.SetActive(true);
            offect.SetActive(true);
            offect.transform.localPosition=new Vector3(0f,17f,0f);
            noOpenObj.SetActive(false);
			timeLabel.text = Language ("ConsumeRebate_06", TimeKit.timeTransformDHMS (activeTime.getDetailEndTime () - now));
		}
		else {
			timeLabel.text = Language ("s0211");
            timeLabel.gameObject.SetActive(true);
            offect.SetActive(true);
            offect.transform.localPosition = new Vector3(0f, 17f, 0f);
            noOpenObj.SetActive(false);
			OnDisable ();
		}
	}

	void OnDisable ()
	{
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
}
