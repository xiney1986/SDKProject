using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 单笔倍数返利活动
/// </summary>
public class NoticeOneManyRechargeContent : MonoBase
{  
	public ContentNoticeActivity content;
	private Notice notice;
	public UILabel desLabel;
	public UILabel timeLabel;
	private bool receiveSuccess;
	public GameObject  NoticeOneManyRechargePrefab;
	public WindowBase win;//活动窗口
	public ButtonBase chargeButton;
	
	private void onReceive ()
	{
		content.initContent (notice, win, this);
		content.Initialize ();
	}
	public void initContent (Notice notice, WindowBase win)
	{
		this.notice = notice;
		this.win = win;
		chargeButton.setFatherWindow(this.win);
		if (notice.getSample ().type == NoticeType.ONE_MANY_RECHARGE) {
			FPortManager.Instance.getFPort<NoticeActiveGetFPort> ().access ((notice.getSample ().content as SidNoticeContent).sids, () => {
				onReceive ();
				initDesTime ();
			});
		} else {
			onReceive ();
			initDesTime ();
		}

//        if (notice.getSample().timeLimit[0] != 0)
//        {
//            titleName.gameObject.SetActive(false);
//        }

	}

	private void initDesTime ()
	{
		if (notice.isTimeLimit ()) {
			int[] time = notice.getShowTimeLimit ();
			if(time == null){
				timeLabel.gameObject.SetActive(true);
				timeLabel.text = LanguageConfigManager.Instance.getLanguage("s0138");
				return;
			}
			timeLabel.gameObject.SetActive(true);
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice02", TimeKit.dateToFormat (time [0], LanguageConfigManager.Instance.getLanguage ("notice04")),
			                                                            TimeKit.dateToFormat (time [1] - 1, LanguageConfigManager.Instance.getLanguage ("notice04")));
		} else
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice03");
	}
	private void updateWindowBack ()
	{
		if (content == null) // 重新加载时可能被刷掉
			return;
		float y = content.transform.localPosition.y;
//		content.gameObject.GetComponent<UIScrollView> ().ResetPosition ();
//		content.gameObject.GetComponent<UIScrollView> ().SetDragAmount (0, 0, false);

		content.reLoad ();
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			content.jumpToPos(y);
		}));

	}
	
//	public override void DoDisable ()
//	{
//		base.DoDisable ();
//		if(content!=null&&content.timer!=null)
//		{
//			content.timer.stop();
//		}
//	}

	void OnDisable ()
	{
		
	}
	
//	public override void buttonEventBase (GameObject gameObj)
//	{
//		base.buttonEventBase (gameObj);
//		if (gameObj.name == "close") {
//            finishWindow();
//		}
//		else if(gameObj.name == "buttonOk")
//		{
//			UiManager.Instance.switchWindow<rechargeWindow>((win)=>{
//				win.setReturnCallBack (rechargeCallback);
//			});
//			content.cleanAll();
//		}
//	}
	void rechargeCallback ()
	{
		initContent (notice, win);
//		UiManager.Instance.switchWindow<NoticeActivityRechargeWindow>((win)=>{
//			win.initWindow(notice,this);
//		});
	}
    public void reload()
    {
        initContent(this.notice, this.win);
    }
}
