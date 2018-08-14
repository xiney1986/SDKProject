using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/**
 *  商店按钮节点
 * */
public class BoxShowItem : MonoBase {
    public ButtonBase BoxButton;
    public UILabel itemTitle;
    public UILabel itemInfo;
    public UILabel itemInfo1;
    private int missionSid;
    public WindowBase fawin;
    //初始化商品条目信息
	public void updateItem (int i) {
        int[] nums = CommandConfigManager.Instance.towerBoxAward;
        string[] dec = CommandConfigManager.Instance.towerBoxDec;
        itemTitle.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow23",nums[i].ToString());
        itemInfo.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow24", nums[i].ToString());
        itemInfo1.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow25",dec[i]);
        missionSid=ChapterSampleManager.Instance.getChapterSampleBySid(ClmbTowerConfigManager.Instance.getClmbTowerMap(1).chapterSids).missions[nums[i]-1];
        if (!FuBenManagerment.Instance.isFistIntoAward(missionSid)) {
            BoxButton.gameObject.SetActive(true);
            BoxButton.onClickEvent = gotoBox;
        } else {
            BoxButton.gameObject.SetActive(false);
        }
	}
    void gotoBox(GameObject bo) {
        //和后台通信拿到奖励pool
        TowerBeginAwardInfo fport = FPortManager.Instance.getFPort("TowerBeginAwardInfo") as TowerBeginAwardInfo;
        fport.access(intoTowerShow);
    }
    void intoTowerShow(int i) {
        ClmbTowerManagerment.Instance.intoTpye = 2;
        ClmbTowerManagerment.Instance.boxMissionSid = missionSid;
        if (i == 0) {//奖池里没有任何东西
            UiManager.Instance.openWindow<TowerShowWindow>((win) => {
                win.init(null, 1, missionSid);
            });
        } else {//奖池里有东西的情况（）飘字提示有宝箱没有开完
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow30"));
            });
            ClmbTowerManagerment.Instance.countieOPenBox(ClmbTowerManagerment.Instance.missionSid);
        }
        fawin.finishWindow();
    }
}
