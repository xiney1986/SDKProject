using System;
using UnityEngine;
using System.Collections;

//活动的章节按钮
public class ButtonActivityChapter : ButtonBase
{
	public UILabel numLabel;//次数
	public UILabel timeLabel;//时间
	public UITexture bgIcon;//广告图
	public UILabel dscc;//
	public ActivityChapter chapter;
	public ButtonBase buyButton;//购买按钮

	public void updateActive (ActivityChapter _chapter)
	{
		this.chapter = _chapter;
        if (!chapter.isOpen()) {
            buyButton.gameObject.SetActive(false);
        }
		buyButton.onClickEvent=buy;
		updateTimes ();
		ResourcesManager.Instance.LoadAssetBundleTexture ("texture/activity/activity_" + chapter.getThumbIconID(), bgIcon);
	}
	
	public void updateTimes(){
		numLabel.text = LanguageConfigManager.Instance.getLanguage("s0146lw011") + ":" + (chapter.getNum()> 0 ? chapter.getNum(): 0);
	}
	void buy(GameObject obj){
        int maxBuyNum = GoodsBuyCountManager.Instance.getSampleByGoodsSid(chapter.sid).vipMaxCount[UserManager.Instance.self.getVipLevel()];
        if (chapter.getReBuyNum() >= maxBuyNum)
        {
            UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
            {
                win.initWindow(2, LanguageConfigManager.Instance.getLanguage("recharge01"), LanguageConfigManager.Instance.getLanguage("s0093"),
                                LanguageConfigManager.Instance.getLanguage("s015316"), (msgHandle) =>
                                {
                                    if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT)
                                    {
                                        UiManager.Instance.openWindow<VipWindow>();
                                    }
                                });
            });
        }
        else
        {
            UiManager.Instance.openDialogWindow<BuyWindow>((win) => {
                win.init(chapter, Mathf.Min((maxBuyNum - chapter.Buyed), timesCanbuyWithRMB()), 1, PrizeType.PRIZE_RMB, (msg) => { msgBack(msg); });
            });
        }
	}
    //现有的钻石可购买的挑战次数
    public int timesCanbuyWithRMB() { 
        int num =0;
        int[] prises = GoodsBuyCountManager.Instance.getSampleByGoodsSid(chapter.sid).prise;
		int rmb=prises.Length<=chapter.getReBuyNum()?prises[prises.Length-1]:prises[chapter.getReBuyNum()];
        int maxBuyNum = GoodsBuyCountManager.Instance.getSampleByGoodsSid(chapter.sid).vipMaxCount[UserManager.Instance.self.getVipLevel()];
        for (int i = 1; i <= maxBuyNum; i++) {
            if (UserManager.Instance.self.getRMB() < ((i / 2) * (rmb + (rmb + (i - 1) * (prises[1] - prises[0]))))) {
                num = i - 1;
                return num;
            }
        }
        return maxBuyNum;
    }
	void msgBack(MessageHandle mh)
    {
        if (UserManager.Instance.self.getRMB() < mh.costNum)
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("gi007"));
                return;
            });
        }
        if (mh.msgEvent == msg_event.dialogCancel) {
            return;
        }
        ActiveFuBenBuyFPort port = FPortManager.Instance.getFPort("ActiveFuBenBuyFPort") as ActiveFuBenBuyFPort;
        port.buyGoods(chapter.getChapterType(), mh.msgNum, buySuccess);
	}
    void buySuccess(int type, int num)
    {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
        {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("activetyinfolw01"));
        });
        chapter.setReBuyNum(chapter.getReBuyNum()+num);
        chapter.addNum(num);
        updateTimes();
    }
	void FixedUpdate ()
	{  
		
		if (chapter != null) {
			chapter.updateTime();
			dscc.text=chapter.getTodyDec();
			if(!chapter.iconColor()){
				bgIcon.color=Color.gray;
			}else 
				bgIcon.color=Color.white;
			//timeLabel.text = chapter.getTimeDesc ();
		} 
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (chapter.isOpen ()) {
			//添加记录
			FuBenManagerment.Instance.selectedChapterSid = chapter.sid;
			UiManager.Instance.openWindow<ActivityChooseWindow> ();
		} else {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("prefabzc27"));
            });
		} 
	}

	
}
