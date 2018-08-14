using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivityChooseButton : ButtonBase
{

	public UILabel levelLimit;
	public Mission mission;
	public ActivityChapter chapter;
	public UISprite missionNameBg;
	public GameObject show;
    private ActivityChooseWindow father;
    public ContentChooseBar content;
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (UserManager.Instance.self.getUserLevel () < mission.getRequirLevel ())
		{
			MaskWindow.UnlockUI();
			return;
		}
		if ((FuBenManagerment.Instance.getActivityChapterBySid (mission.getChapterSid ())).getNum () == 0) {
            int maxBuyNum = GoodsBuyCountManager.Instance.getSampleByGoodsSid(chapter.sid).vipMaxCount[UserManager.Instance.self.getVipLevel()];
            if (chapter.getReBuyNum() >= maxBuyNum) {
                UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                    win.initWindow(2, LanguageConfigManager.Instance.getLanguage("recharge01"), LanguageConfigManager.Instance.getLanguage("s0093"),
                                    LanguageConfigManager.Instance.getLanguage("s015316"), (mesgHandle) => {
                                        if (mesgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
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
	    //不在检查体力
        //if ((UserManager.Instance.self.getPvEPoint ()+UserManager.Instance.self.getStorePvEPoint()) < mission.allCostPve) {
        //    UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
        //        win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("s0163", mission.allCostPve.ToString ()), intoFubenBack);
        //    });
        //    return;	
        //}
//		UiManager.Instance.openWindow<TeamPrepareWindow>((win) => {
//			win.Initialize (mission,TeamPrepareWindow.WIN_ACTIVITY_ITEM_TYPE); 
//		});
//		int teamId = ArmyManager.PVE_TEAMID;	
//		MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (mission.sid);
//		int cSid = sample.chapterSid;
//		int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;
        //if (UserManager.Instance.self.getPvEPoint () < 1) {
        //    UiManager.Instance.openDialogWindow<PveUseWindow> ();
        //    return; 
        //}
        if ((FuBenManagerment.Instance.getActivityChapterBySid(mission.getChapterSid())).getNum() > 0) {
            MessageHandle msgHandle = new MessageHandle();
            msgHandle.buttonID = MessageHandle.BUTTON_RIGHT;
            intoFubenBack(msgHandle);
        }
	}
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
    void msgBack(MessageHandle mh) {
        if (UserManager.Instance.self.getRMB() < mh.costNum) {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
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
    void buySuccess(int type, int num) {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("activetyinfolw01"));
        });
        chapter.setReBuyNum(chapter.getReBuyNum() + num);
        chapter.addNum(num);
        father = this.fatherWindow as ActivityChooseWindow;
        father.numLabel.text = LanguageConfigManager.Instance.getLanguage("s0146lw011") + ":" + (chapter.getNum() > 0 ? chapter.getNum() : 0);//刷新次数
    }
    //初始化条目信息
	public void updateButton (Mission mis,ActivityChapter _chapter)
	{
		mission = mis;
        chapter = _chapter;
        content.init(this.fatherWindow);
        content.Initialize(mission);//初始化奖励
		//textLabel.text = mission.getMissionName ();
		if (mission.getMissionName () == LanguageConfigManager.Instance.getLanguage ("activityfuben01")) {
			missionNameBg.spriteName ="icon_text8";
		}
		if (mission.getMissionName () == LanguageConfigManager.Instance.getLanguage ("activityfuben02")) {
			missionNameBg.spriteName ="icon_text7";
		}
		if (mission.getMissionName () == LanguageConfigManager.Instance.getLanguage ("activityfuben03")) {
			missionNameBg.spriteName ="icon_text6";
		}
		if (mission.getMissionName () == LanguageConfigManager.Instance.getLanguage ("activityfuben04")) {
			missionNameBg.spriteName ="icon_text10";
		}
		if (mission.getMissionName () == LanguageConfigManager.Instance.getLanguage ("activityfuben06")) {
			missionNameBg.spriteName ="icon_text9";
		}
		if (mission.getMissionName () == LanguageConfigManager.Instance.getLanguage ("activityfuben05")) {
			missionNameBg.spriteName ="icon_text13";
		}
		if (UserManager.Instance.self.getUserLevel () < mission.getRequirLevel ()) {
			show.SetActive(false);
			levelLimit.gameObject.SetActive (true);
			levelLimit.text = "Lv." + mission.getRequirLevel () + LanguageConfigManager.Instance.getLanguage ("s0160");
            return;
		} else {
			show.SetActive(true);
			levelLimit.gameObject.SetActive (false);
  
		}
	 
	}

	public void intoFubenBack (MessageHandle msgHandle)
	{
		if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
			//判断玩家是否有足够的存储空间
			if (FuBenManagerment.Instance.isStoreFull ()) {
				return;
			}
            FuBenManagerment.Instance.tmpStorageVersion = StorageManagerment.Instance.tmpStorageVersion;
            (FPortManager.Instance.getFPort("ActiveFubenIntoFPort") as ActiveFubenIntoFPort).intoRoad(mission.sid, () =>
            {
                //直接战斗等后台推战报
                MaskWindow.instance.setServerReportWait(true);
                GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
                MissionInfoManager.Instance.saveMission(mission.sid, 1);
            });
		}
	}
}
