using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackRechargeContent : MonoBase {
    public ContentBackRecharge content;
    private Notice notice;
    //public UILabel titleName;
    public UILabel desLabel;
    public UISprite desTitle1;
    public UISprite desTitle2;
    public UILabel timeLabel;
    public GameObject backRechargePrefab;
    public WindowBase win;//活动窗口
    public ButtonBase chargeButton;
	int endTimes =0;

    private void onReceive() {
		//回归累计充值// 
		BackPrizeRechargeInfoFPort bpif1 = FPortManager.Instance.getFPort ("BackPrizeRechargeInfoFPort") as BackPrizeRechargeInfoFPort;
		bpif1.BackPrizeRechargeInfoAccess(mCallBack);
    }
	public void mCallBack()
	{
		content.initContent(notice, win, this);
		content.Initialize();
	}
    public void initContent(Notice notice, WindowBase win) {
		endTimes = BackPrizeRechargeInfo.Instance.endTimes;
        this.notice = notice;
        this.win = win;
        chargeButton.setFatherWindow(this.win);
		onReceive();
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
            content.jumpToPos(y);
        }));

    }

    void rechargeCallback() {
        initContent(notice, win);
        //		UiManager.Instance.switchWindow<NoticeActivityRechargeWindow>((win)=>{
        //			win.initWindow(notice,this);
        //		});
    }

	void Update()
	{
		if((endTimes - ServerTimeKit.getSecondTime()) <= 0)// 双倍经验时间已结束//
		{
			timeLabel.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_06");
		}
		else
		{
			timeLabel.text = LanguageConfigManager.Instance.getLanguage("goddnessShake01") + TimeKit.timeTransform ((endTimes - ServerTimeKit.getSecondTime())*1000.0d);
		}
	}
}
