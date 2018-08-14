using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivityChooseWindow : WindowBase
{
	public ContentActivityChoose content;
	public ButtonActivityChapter info;
    public UILabel numLabel;
    public UILabel time;
    public UITexture bannerBg;
	ActivityChapter chapter;
	List<PrizeSample> prizes = new List<PrizeSample> ();
	List<int> prizeSid = new List<int> ();
	List<int> prizeNum = new List<int> ();
	public GameObject activityChoosePrefab;
    public GameObject goodsPrefab;
	int[] missionList;

    /// <summary>
    /// 断线重连
    /// </summary>
    public override void OnNetResume() {
       base.OnNetResume();
       ChapterSelectWindow chapter = new ChapterSelectWindow();
       FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
       port.info(updateTimes, ChapterType.ACTIVITY_CARD_EXP);
    }
	protected override void begin ()
	{
		base.begin ();
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		content.Initialize (missionList,chapter);
        
        if (!FuBenManagerment.Instance.isWarActiveWin)
        {
           // startWarCD();
            FuBenManagerment.Instance.isWarActiveFuben = false;
            FuBenManagerment.Instance.isWarActiveWin = true;
        }
        else if (FuBenManagerment.Instance.ActiveWinAward != null)
        {
            UiManager.Instance.openDialogWindow<WarAwardWindow>((win) =>
            {
                win.init(FuBenManagerment.Instance.ActiveWinAward);
            });
        } else {
            FuBenManagerment.Instance.isWarActiveFuben = false;
        }
		MaskWindow.UnlockUI ();
	}
    public void updateTimes() {
        numLabel.text = LanguageConfigManager.Instance.getLanguage("s0146lw011") + ":" + (chapter.getNum() > 0 ? chapter.getNum() : 0);
    }
	protected override void DoEnable ()
	{
		refreshData ();
	}

	public void refreshData ()
	{
	
		chapter= FuBenManagerment.Instance.getActivityChapterBySid (FuBenManagerment.Instance.selectedChapterSid);
        updateTimes();
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/activity/activity_" + chapter.getThumbIconID(), bannerBg);
		setTitle(chapter.getChapterName());
		missionList = FuBenManagerment.Instance.getAllShowMissions (FuBenManagerment.Instance.selectedChapterSid);
		//info.updateActive (chapter);
		setInfo ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
		else if(gameObj.name=="buyButton"){
            int maxBuyNum = GoodsBuyCountManager.Instance.getSampleByGoodsSid(chapter.sid).vipMaxCount[UserManager.Instance.self.getVipLevel()];
            if (chapter.getReBuyNum() >= maxBuyNum) {
                UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                    win.initWindow(2, LanguageConfigManager.Instance.getLanguage("recharge01"), LanguageConfigManager.Instance.getLanguage("s0093"),
                                    LanguageConfigManager.Instance.getLanguage("s015316"), (msgHandle) => {
                                        if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
                                            UiManager.Instance.openWindow<VipWindow>();
                                        }
                                    });
                });
            } else {
                UiManager.Instance.openDialogWindow<BuyWindow>((win) => {
                    win.init(chapter, Mathf.Min((maxBuyNum - chapter.Buyed), timesCanbuyWithRMB()), 1, PrizeType.PRIZE_RMB, (msg) => { msgBack(msg); });
                });
            }
        }
	}
    //现有的钻石可购买的挑战次数
    public int timesCanbuyWithRMB() {
        int num = 0;
        int[] prises = GoodsBuyCountManager.Instance.getSampleByGoodsSid(chapter.sid).prise;
        int rmb = prises.Length <= chapter.getReBuyNum() ? prises[prises.Length - 1] : prises[chapter.getReBuyNum()];
        int maxBuyNum = GoodsBuyCountManager.Instance.getSampleByGoodsSid(chapter.sid).vipMaxCount[UserManager.Instance.self.getVipLevel()];
        for (int i = 1; i <= maxBuyNum; i++) {
            if (UserManager.Instance.self.getRMB() < ((i / 2) * (rmb + (rmb + (i - 1) * (prises[1] - prises[0]))))) {
                num = i - 1;
                return num;
            }
        }
        return maxBuyNum;
    }
    void msgBack(MessageHandle cbm)
    {
        if (UserManager.Instance.self.getRMB() < cbm.costNum)
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("gi007"));
                return;
            });
        }
        if (cbm.msgEvent == msg_event.dialogCancel) {
            return;
        }
        ActiveFuBenBuyFPort port = FPortManager.Instance.getFPort("ActiveFuBenBuyFPort") as ActiveFuBenBuyFPort;
        port.buyGoods(chapter.getChapterType(), cbm.msgNum, buySuccess);
        
    }

    void buySuccess(int type,int num){
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
        {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("activetyinfolw01"));
        });
        chapter.setReBuyNum(chapter.getReBuyNum() + num);
        chapter.addNum(num);
        content.Initialize(missionList, chapter);
        setInfo();
        
    }
	private void setInfo(){
        updateTimes();
		prizes.Clear ();
		prizeSid.Clear ();
		prizeNum.Clear ();
		PrizeSample[] cur;
		Mission tmpMission;
		foreach (int each in missionList) {
			//MissionInfoManager.Instance .getMissionBySid (each);
			tmpMission =  MissionInfoManager.Instance .getMissionBySid (each);
			cur = tmpMission.getPrizes ();
			bool finding = false;
			if(cur != null){
				for(int i=0 ;i< cur.Length; i++){
					if(prizes != null){
						for(int j=0;j<prizes.Count;j++){
							if(cur[i].pSid == prizes[j].pSid){
								for(int k =0;k<prizeSid.Count;k++){
									if(prizeSid[k]==cur[i].pSid)
										prizeNum[k] += (StringKit.toInt( cur[i].num));
								}
								finding = true;
							}
						}
						if(!finding){
							prizeSid.Add(cur[i].pSid);
							prizeNum.Add(StringKit.toInt( cur[i].num));
							prizes.Add(cur[i]);
						}
					}
					else{
						prizeSid.Add(cur[i].pSid);
						prizeNum.Add(StringKit.toInt( cur[i].num));
						prizes.Add(cur[i]);
					}
				}
			}
		}
        //if (prizes != null) {
        //    for (int i = 0; i < prizes.Count && i < 3; i++) {
        //        awardsShow[i].gameObject.SetActive(true);
        //        awardsShow[i].init(prizes[i], getPrizeNum(prizes[i]));
        //    }
        //}
        //prizes = mission.getPrizes();
	}
	void FixedUpdate ()
	{  
		
		if (chapter != null) {
			chapter.updateTime();
            time.text = chapter.getTodyDec();
            if (!chapter.iconColor()) {
                bannerBg.color = Color.gray;
            } else
                bannerBg.color = Color.white;
            //activityTime.text = chapter.getTimeDesc ();
		} 
	}
	private int getPrizeNum(PrizeSample sample){
		for (int i=0; i<prizeSid.Count; i++) {
			if(prizeSid[i]==sample.pSid)
				return prizeNum[i];
		}
		return 0;
	}
}
