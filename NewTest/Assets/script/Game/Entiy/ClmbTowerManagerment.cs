using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 公告管理器
 * 所有公告信息
 * @author 汤琦
 * */
public class ClmbTowerManagerment
{

	/** pat数据缓存 */
	public TurnSpriteData turnSpriteData;
    public int couccentIndex = -1;//当前放开的第几张牌
    CallBack callback=null;
    public TurnSpriteReward couccentAward = null;
    public CallBack getAwardSuccessCallBack;//得到奖励以后的回调
    public CallBack getGiveUpCallBack;//放弃领奖的回调
    public bool isCanGetAward = false;//有奖励被抽取了
    public int missionSid = 0;
    public int relotteryNum = 0;//今日已经开宝箱的次数
    public int relotteryBuyNum = 0;//今日已经购买开宝箱次数
    public int intoTpye = 0;//初始状态 1 副本进 2 外面进
    public int boxMissionSid = 0;
    public bool canPassBox = false;//跳过这个宝箱点
    public ClmbTowerManagerment()
	{

	}

    public static ClmbTowerManagerment Instance {
        get { return SingleManager.Instance.getObj("ClmbTowerManagerment") as ClmbTowerManagerment; }
	}
    public List<PrizeSample> getreAward() {
        if (turnSpriteData == null || turnSpriteData.rewardList.Count<=0) return null;
        List<PrizeSample> pl=null;
        List<TurnSpriteReward> rewardList = turnSpriteData.rewardList;
        for (int i = 0; i < rewardList.Count; i++) {
            TurnSpriteReward tsr = rewardList[i];
            if (pl == null) pl = new List<PrizeSample>();
            if (tsr.type == "card") pl.Add(new PrizeSample(5, tsr.sid, tsr.num,tsr.index));
            if (tsr.type == "equip") pl.Add(new PrizeSample(4, tsr.sid, tsr.num, tsr.index));
            if (tsr.type == "goods") pl.Add(new PrizeSample(3, tsr.sid, tsr.num, tsr.index));
            if (tsr.type == "artifact") pl.Add(new PrizeSample(21, tsr.sid, tsr.num, tsr.index));
        }
        return pl;
    }
    public PrizeSample getSelectAward() {
        PrizeSample pl = null;
        if (couccentAward != null) {
            if (couccentAward.type == "card") pl = new PrizeSample(5, couccentAward.sid, couccentAward.num, couccentAward.index);
            if (couccentAward.type == "equip") pl = new PrizeSample(4, couccentAward.sid, couccentAward.num, couccentAward.index);
            if (couccentAward.type == "goods") pl = new PrizeSample(3, couccentAward.sid, couccentAward.num, couccentAward.index);
            if (couccentAward.type == "artifact") pl = new PrizeSample(21, couccentAward.sid, couccentAward.num, couccentAward.index);
        } else {
            if (turnSpriteData == null || turnSpriteData.towerNotTurnRewardList.Count<=0) return pl;
            for (int i = 0; i < turnSpriteData.towerNotTurnRewardList.Count; i++) {
                if (turnSpriteData.towerNotTurnRewardList[i].index != 0) {
                    couccentAward = turnSpriteData.towerNotTurnRewardList[i];
                    if (couccentAward.type == "card") pl = new PrizeSample(5, couccentAward.sid, couccentAward.num, couccentAward.index);
                    if (couccentAward.type == "equip") pl = new PrizeSample(4, couccentAward.sid, couccentAward.num, couccentAward.index);
                    if (couccentAward.type == "goods") pl = new PrizeSample(3, couccentAward.sid, couccentAward.num, couccentAward.index);
                    if (couccentAward.type == "artifact") pl = new PrizeSample(21, couccentAward.sid, couccentAward.num, couccentAward.index);
                    return pl;
                }
            }
        }
        return pl;
    }
    public PrizeSample getPrizeFromAward(TurnSpriteReward arard) {
        PrizeSample pl = null;
        if (arard != null) {
            if (arard.type == "card") pl = new PrizeSample(5, arard.sid, arard.num, arard.index);
            if (arard.type == "equip") pl = new PrizeSample(4, arard.sid, arard.num, arard.index);
            if (arard.type == "goods") pl = new PrizeSample(3, arard.sid, arard.num, arard.index);
            if (arard.type == "artifact") pl = new PrizeSample(21, arard.sid, arard.num, arard.index);
        }
        return pl;
    }
    public void checkCanIntoTower() {
        //检查是不是第一次来这个副本
        if (!FuBenManagerment.Instance.isFistIntoAward(MissionInfoManager.Instance.mission.sid)) {
            giveUpAward();
        } else {
            TowerBeginAwardInfo fport = FPortManager.Instance.getFPort("TowerBeginAwardInfo") as TowerBeginAwardInfo;
            fport.access(intoTower);
        }
    }
    void intoTower(int i) {
        if (i == 3) {//over的直接跳过
            if (getGiveUpCallBack != null) getGiveUpCallBack();
        } else {
            string missionName = MissionInfoManager.Instance.mission.getMissionName();
            int missionSid = MissionInfoManager.Instance.mission.sid;
            bool isFirstAward = FuBenManagerment.Instance.isFistIntoAward(boxMissionSid);
            string towerLevel1 = missionSid >= 151010 ? LanguageConfigManager.Instance.getLanguage("towerShowWindow04", missionName.Substring(2, 2)) : LanguageConfigManager.Instance.getLanguage("towerShowWindow04", missionName.Substring(2, 1));
            string towerLevel2 = missionSid >= 151010 ? LanguageConfigManager.Instance.getLanguage("towerShowWindow03", missionName.Substring(2, 2)) : LanguageConfigManager.Instance.getLanguage("towerShowWindow03", missionName.Substring(2, 1));
            if (isFirstAward) {//第一次进去
                UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                    win.dialogCloseUnlockUI = false;
                    win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0093"),
                        isFirstAward ? towerLevel1 : towerLevel2, (msgHandle) => {
                            if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {//确定要开宝箱

                                intoTowerShow(i);
                            } else if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
                                giveUpAward();
                            }
                        });
                });
            }
        }
    }
    /// <summary>
    /// 放弃领奖方法
    /// </summary>
    public  void giveUpAward() {
        ClmbTowerManagerment.Instance.isCanGetAward = false;
        ClmbTowerManagerment.Instance.turnSpriteData = null;
        TowerAwardClearFPort ffport = FPortManager.Instance.getFPort("TowerAwardClearFPort") as TowerAwardClearFPort;
        ffport.accessGiveUp(() => {
            if (getGiveUpCallBack != null) getGiveUpCallBack();
        }, -1);
    }
    //今日宝库方法
    public void beginIntoTower(){//这里进入爬塔宝箱(0就是宝箱没有被开启，1就是宝箱开启了)
        TowerBeginAwardInfo fport = FPortManager.Instance.getFPort("TowerBeginAwardInfo") as TowerBeginAwardInfo;
        fport.access(intoTowerShow);
    }
    void intoTowerShow(int i) {
        if (i == 3) {//over的直接跳过
            if (getGiveUpCallBack != null) getGiveUpCallBack();
        }
        else if(i==0){//奖池里没有任何东西
            UiManager.Instance.openWindow<TowerShowWindow>((win) => {
                win.init(null,1,missionSid);
            });
        } else {//奖池里有东西的情况
            countieOPenBox(missionSid);
        }
    }
    /// <summary>
    /// 继续开宝箱
    /// </summary>
    public void countieOPenBox(int sid){
        if (turnSpriteData.towerRewardList.Count > 0) {
                UiManager.Instance.openWindow<TowerShowWindow>((win) => {
                    win.init(getreAward(), turnSpriteData.towerRewardList, callback,sid);
                });
            } else {
                UiManager.Instance.openWindow<TowerShowWindow>((win) => {
                    win.init(getreAward(), 2,sid);
                });
            }
    }
    public int getBoxMAxNum() {
        int[] vipBoxNum = CommandConfigManager.Instance.getVipBoxNum();
        int[] vipBuyBoxNum = CommandConfigManager.Instance.getVipBuyBoxNum();
        int[] getBoxAwardSid = CommandConfigManager.Instance.getBoxAwardS();
        int vipLv = UserManager.Instance.self.getVipLevel();
        int[] passSid=FuBenManagerment.Instance.getTowerInfos();
        int tempNum = 0;
        if (passSid == null || passSid.Length <= 0) tempNum = 0;
        else {
            for (int i = 0; i < passSid.Length;i++ ) {
                for (int j = 0; j < getBoxAwardSid.Length;j++ ) {
                    if (passSid[i] == getBoxAwardSid[j])tempNum++;
                }
            }
        }
        return   vipBoxNum[vipLv] + tempNum + relotteryBuyNum;
    }
    /// <summary>
    /// 检查是否可以购买开宝箱次数
    /// </summary>
    /// <returns></returns>
    public bool checkCanBeBuy() {
        if (getBoxMAxNum() <= relotteryNum) {
            int vipLv = UserManager.Instance.self.getVipLevel();//vip等级
            int vipBuyBoxNum = CommandConfigManager.Instance.getVipBuyBoxNum()[vipLv];//可以购买宝箱的最大次数 按VIP分级
            if (relotteryBuyNum >= vipBuyBoxNum) return false;
            return true;
        }
        return true; 
    }
    public bool needBuy() {
        if (getBoxMAxNum() <= relotteryNum) return true;
        return false;
    }
    

}