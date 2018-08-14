using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ButtonChallenge : ButtonBase {

	private ArenaActivityInfo ladderInfo;
	public LadderHegemoneyActiveNotice ladderNotice;
	private bool isEnable;


	public override void begin ()
	{
		base.begin ();
		ladderInfo = FuBenManagerment.Instance.getLadderActivityArray ();
		isEnable = UserManager.Instance.self.getUserLevel () >= ladderInfo.RequestMinLevel;

		if (isEnable) {
			FPortManager.Instance.getFPort<LaddersStateFPort> ().apply (null);
			if (LaddersManagement.Instance.Chests.M_getChests () [0] == null||ServerTimeKit.getSecondTime() > LaddersManagement.Instance.nextTime) {
				FPortManager.Instance.getFPort<LaddersGetInfoFPort> ().apply ((hasApply) => {
					if (!hasApply)
					{
						FPortManager.Instance.getFPort<LaddersApplyFPort> ().apply ((msg) => {
							if(msg.Equals("ok"))
							{
								FPortManager.Instance.getFPort<LaddersGetInfoFPort> ().apply (null);
							}
						});
					}
				});
			} else {
				//MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_01",ladderInfo.RequestMinLevel.ToString()));
			}
		}

	}


	public override void DoClickEvent ()
	{
		base.DoClickEvent ();

		isEnable = UserManager.Instance.self.getUserLevel () >= ladderInfo.RequestMinLevel;
		if (!isEnable) {
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_01",ladderInfo.RequestMinLevel.ToString()));
			return;
		}

		//Notice notice = NoticeManagerment.Instance.getNoticeListByType (NoticeType.LADDER_ACTION_TIME,NoticeEntranceType.LIMIT_NOTICE) ;
        List<Notice> notices = NoticeManagerment.Instance.getNoticesByType(NoticeType.LADDER_ACTION_TIME, NoticeEntranceType.LIMIT_NOTICE);
        int flag = 0;
        int flag1 = 0;
        if(notices!=null){
            for (int i = 0; i < notices.Count;i++ ) {
                ladderNotice = notices[i] as LadderHegemoneyActiveNotice;
                if (ladderNotice != null && ladderNotice.isInTimeLimit()) {
                    flag = 1;
                }
                if (ladderNotice != null && ladderNotice.isValid()) {
                    flag1 = 1;
                }
            }
            if(flag==0){
                MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("ladderruleprize5"));
                return;
            }
            if (flag1 == 1) {
                UiManager.Instance.switchWindow<LaddersWindow>((win) => {
                    win.init();
                    win.lastPrestigeLevel = LaddersManagement.Instance.M_getCurrentPlayerTitle().index;
                    if (LaddersManagement.Instance.Award.canReceive) {
                        UiManager.Instance.openDialogWindow<LaddersRankRewardWindow>((win1) => {
                            win1.closeCallback = win.showPrestigeLevel;
                        });
                    }
                });
            } else MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0171"));

        } else {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0171"));
        }

        //if (ladderNotice != null && !ladderNotice.isInTimeLimit ()) {
        //    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("ladderruleprize5"));
        //    return;
        //}

        //if (ladderNotice != null && ladderNotice.isValid()) {

        //    UiManager.Instance.switchWindow<LaddersWindow> ((win) => {
        //        win.init ();
        //        win.lastPrestigeLevel = LaddersManagement.Instance.M_getCurrentPlayerTitle ().index;
        //        if (LaddersManagement.Instance.Award.canReceive) {
        //            UiManager.Instance.openDialogWindow<LaddersRankRewardWindow> ((win1) => {
        //                win1.closeCallback = win.showPrestigeLevel;
        //            });
        //        }
        //    });
        //} else {
        //    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0171"));
        //}


	}
	

}
