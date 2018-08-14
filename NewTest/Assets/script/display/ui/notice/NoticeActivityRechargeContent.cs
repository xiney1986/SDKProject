using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoticeActivityRechargeContent : MonoBase {
    public ContentNoticeActivity content;
    private Notice notice;
    //public UILabel titleName;
    public UILabel desLabel;
    public UISprite desTitle1;
    public UISprite desTitle2;
    public UILabel timeLabel;
    private bool receiveSuccess;
    public GameObject NoticeActivityRechargePrefab;
    public GameObject noticeIntroducePrefab;
    public WindowBase win;//活动窗口
    public ButtonBase chargeButton;

    private void onReceive() {
        content.initContent(notice, win, this);
        content.Initialize();
    }
    public void initContent(Notice notice, WindowBase win) {
        this.notice = notice;
        this.win = win;
        chargeButton.setFatherWindow(this.win);
        if (notice.getSample().type == NoticeType.NEW_RECHARGE || notice.getSample().type == NoticeType.NEW_CONSUME) {
            FPortManager.Instance.getFPort<NoticeActiveGetFPort>().access((notice.getSample().content as SidNoticeContent).sids, () => {
                onReceive();
                initDesTime();
            });
        } else {
            onReceive();
            initDesTime();
        }

        //        if (notice.getSample().timeLimit[0] != 0)
        //        {
        //            titleName.gameObject.SetActive(false);
        //        }

    }

    private void initDesTime() {
        if (notice.getSample().name == Language("notice12")) {
            desTitle1.spriteName = "notice_lj";
            desTitle2.spriteName = "notice_cz";
        } else if (notice.getSample().name == Language("notice13")) {
            desTitle1.spriteName = "notice_lj";
            desTitle2.spriteName = "notice_xf";
        } else if (notice.getSample().name == Language("notice15")) {
            desTitle1.spriteName = "notice_xs";
            desTitle2.spriteName = "notice_cz";
        } else if (notice.getSample().name == Language("notice20")) {
            desTitle1.spriteName = "notice_xs";
            desTitle2.spriteName = "notice_xf";
        } else if (notice.getSample().name == Language("notice16")) {
            desTitle1.spriteName = "notice_mr";
            desTitle2.spriteName = "notice_cz";
        } else if (notice.getSample().name == Language("notice17")) {
            desTitle1.spriteName = "notice_xs";
            desTitle2.spriteName = "notice_dh";
        } else if (notice.getSample().name == Language("notice18")) {
            desTitle1.spriteName = "notice_xs";
            desTitle2.spriteName = "notice_qg";
        }


        if (notice.isTimeLimit()) {
            int[] time = notice.getShowTimeLimit();
            if (time == null) {
                timeLabel.gameObject.SetActive(true);
                timeLabel.text = LanguageConfigManager.Instance.getLanguage("s0138");
                return;
            }
            timeLabel.gameObject.SetActive(true);
            timeLabel.text = LanguageConfigManager.Instance.getLanguage("notice02", TimeKit.dateToFormat(time[0], LanguageConfigManager.Instance.getLanguage("notice04")),
                                                                        TimeKit.dateToFormat(time[1] - 1, LanguageConfigManager.Instance.getLanguage("notice04")));
        } else
            timeLabel.text = LanguageConfigManager.Instance.getLanguage("notice03");
    }

    public void updateWindow(bool isOpenHeroRoad) {
        if (isOpenHeroRoad)
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("HeroRoad_open"));
        updateWindowBack();
    }
    private void updateWindowBack() {
        if (content == null) // 重新加载时可能被刷掉
            return;
        float y = content.transform.localPosition.y;
        //		content.gameObject.GetComponent<UIScrollView> ().ResetPosition ();
        //		content.gameObject.GetComponent<UIScrollView> ().SetDragAmount (0, 0, false);

        content.reLoad();
        StartCoroutine(Utils.DelayRunNextFrame(() => {
            //content.jumpToPos(y);
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

    void OnDisable() {

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
    void rechargeCallback() {
        initContent(notice, win);
        //		UiManager.Instance.switchWindow<NoticeActivityRechargeWindow>((win)=>{
        //			win.initWindow(notice,this);
        //		});
    }
}
