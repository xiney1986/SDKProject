using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 签到奖励容器
/// </summary>
public class NoticeActivitySignInContent : MonoBase {
    public const int SIGN_IN_NO = 0;//没签到
    public const int SIGN_IN_YES = 1;//已签到
    public const int ALLSECONDSOFDAY = 86400;

    public UITexture monthNum1;//月份数字
    public UITexture monthNum2;
    public GameObject signAwardContent;//签到奖励容器
    public GameObject allSignAwardPoint;//全勤奖励位置
    public UILabel signTotalNum;//所有签到次数统计
    public ButtonBase buttonAllSignAward;//全勤奖励按钮
    public GameObject signInButton;//签到按钮
    public WindowBase win;
    public GameObject goodsView;

    private Notice notice;
    private PrizeSample allSignPrize;
    private int allSignSid;
    private Timer timer;//计时器（用于活动结束时的判断）
    private ActiveTime activeTime;
    int count = 0;
    int signedCount = 0;
	/// <summary>
	/// 初始化容器
	/// </summary>
	public void initContent (Notice notice, WindowBase win)
	{
		this.notice = notice;
		this.win = win;
        initButton();
        startTimer();
	}
    private void startTimer() {//开始计时
        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(notice.sid);
        activeTime = ActiveTime.getActiveTimeByID(sample.timeID);
        timer.addOnTimer(updateNoticeWindow);
        timer.start();
    }
    private void updateNoticeWindow() {
        int currentSecond = ServerTimeKit.getCurrentSecond();
        if (currentSecond == ALLSECONDSOFDAY && ServerTimeKit.getSecondTime() < activeTime.getEndTime()) {//凌晨0点刷新界面
            initButton();
        }
        if (ServerTimeKit.getSecondTime() >= activeTime.getEndTime() || !notice.isValid()) {//活动结束，刷新整个活动界面
            if (UiManager.Instance.getWindow<NoticeWindow>() != null) {
                UiManager.Instance.getWindow<NoticeWindow>().initTopButton();
            }
            timer.stop();
        }
    }
    public void OnDisable() {
        if(timer != null)
        timer.stop();
    }

    public void updateUI() {
        int days = ServerTimeKit.getCurrentMonth();
        if (days == 1 || days == 3 || days == 5 || days == 7 || days == 8 || days == 10 || days == 12) {
            count = 31;
        } else if (days == 4 || days == 6 || days == 9 || days == 11) {
            count = 30;
        } else {
            if (ServerTimeKit.getYear() % 4 == 0 || (ServerTimeKit.getYear() % 100 == 0 && ServerTimeKit.getYear() % 400 == 0)) {
                count = 29;
            } else count = 28;
        }
        if (monthNum1 != null && monthNum2 != null) {
            if (ServerTimeKit.getCurrentMonth() < 10) {
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SIGNIN_TEXTURE + ("month_0" + ServerTimeKit.getCurrentMonth()), monthNum1);
                monthNum1.transform.localPosition = new Vector3(-115, 63, 0);
                monthNum2.gameObject.SetActive(false);
            } else {
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SIGNIN_TEXTURE + ("month_01"), monthNum1);
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SIGNIN_TEXTURE + ("month_0" + (ServerTimeKit.getCurrentMonth() - 10)), monthNum2);
                monthNum1.transform.localPosition = new Vector3(-132, 63, 0);
                monthNum2.transform.localPosition = new Vector3(-95, 63, 0);
            }
        }
        if (signTotalNum != null) {
                signTotalNum.text = (SignInManagerment.Instance.stateList.Count > count ? count : SignInManagerment.Instance.stateList.Count) + "/" + count;
        }
        if (buttonAllSignAward != null) {
            buttonAllSignAward.disableButton(true);
            int data = ServerTimeKit.getDataBySeconds(activeTime.getStartTime());
            if (SignInManagerment.Instance.stateList.Count >= (count - data + 1))
            {
                int ssid = StringKit.toInt(notice.sid + "" + ServerTimeKit.getCurrentMonth());
                SignInSample sample = SignInSampleManager.Instance.getSignInSampleBySid(ssid);
                if(sample != null &&!SignInManagerment.Instance.stateList.Contains(sample.allSignSid))
                    buttonAllSignAward.disableButton(false);
            }
        }
    }
	public void initButton() 
    {
        buttonAllSignAward.fatherWindow = win;
        buttonAllSignAward.onClickEvent = getAllSignAward;
        GetSignInInfoFport fport = FPortManager.Instance.getFPort("GetSignInInfoFport") as GetSignInInfoFport;
        fport.getSignInInfo(loadPrize);
	}

	/// <summary>
	/// 加载数据
	/// </summary>
    public void loadPrize() {
        Utils.RemoveAllChild(signAwardContent.transform);
        updateUI();
        int ssid = StringKit.toInt(notice.sid + "" + ServerTimeKit.getCurrentMonth());
        SignInSample sample = SignInSampleManager.Instance.getSignInSampleBySid(ssid);
        if (sample == null) return;
        allSignPrize = sample.allSignPrize;
        allSignSid = sample.allSignSid;
        List<PrizeSample> prize = sample.list;
        List<int> sids = sample.daySids;//日期sid
        List<int> types = sample.types;//奖励类型（用于背景的显示）
        for (int i = 0; i < count; i++) {
            GameObject signBtn = NGUITools.AddChild(signAwardContent, signInButton);
            signBtn.transform.localScale = new Vector3(0.55f, 0.55f, 0);
            signBtn.transform.localPosition = new Vector3(-216 + (int)(i % 7) * 71, 155 - (int)(i / 7) * 80, 0);
            SignInButton signIn = signBtn.GetComponent<SignInButton>();
            signIn.init(this);
            if (SignInManagerment.Instance.stateList.Contains(sids[i]))//签过的
                signIn.init(prize[i], SIGN_IN_YES, i + 1, sids[i],types[i]);
            else
                signIn.init(prize[i], SIGN_IN_NO, i + 1, sids[i], types[i]);
            signIn.fatherWindow = win;
        }
        GameObject allSignPrizeObj = NGUITools.AddChild(allSignAwardPoint, goodsView);
        allSignPrizeObj.transform.localScale = new Vector3(0.8f, 0.8f, 0);
        allSignPrizeObj.GetComponent<GoodsView>().init(allSignPrize);
        allSignPrizeObj.GetComponent<GoodsView>().fatherWindow = win;
    }
    /// <summary>
    /// 全勤奖按钮执行事件
    /// </summary>
    /// <param name="gameObj"></param>
    public void getAllSignAward(GameObject gameObj) {
        SignInFport fport = FPortManager.Instance.getFPort("SignInFport") as SignInFport;
        fport.signIn(allSignSid,1,getAwardCallback);
        
    }
    public void getAwardCallback() {
        if (isStorageFull(allSignPrize) || !StorageManagerment.Instance.isTempStorageFull(StringKit.toInt(allSignPrize.num))) {
            UiManager.Instance.createPrizeMessageLintWindow(allSignPrize);
	if (isStorageFull(allSignPrize)) {
                if (allSignPrize.type == PrizeType.PRIZE_CARD) {
                    Card card = CardManagerment.Instance.createCard(allSignPrize.pSid);
                    if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed(card)) {
                        UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
                            win.init(LanguageConfigManager.Instance.getLanguage("s0418"), 1.5f);
                        });
                    }
                }
            }
            if (!isStorageFull(allSignPrize)) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("signInTips1"));
                });
            }
            buttonAllSignAward.disableButton(true);//领过奖励后置灰
            return;
        }
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("signInTips2"));
        });
    }
    /// <summary>
    /// 检测是否有足够的空间容纳奖励
    /// </summary>
    /// <param name="prize"></param>
    /// <returns></returns>
    private bool isStorageFull(PrizeSample prize) {
        switch (prize.type) {
            case PrizeType.PRIZE_CARD:
                if (StorageManagerment.Instance.isRoleStorageFull(StringKit.toInt(prize.num))) return false;
                break;
            case PrizeType.PRIZE_EQUIPMENT:
                if (StorageManagerment.Instance.isEquipStorageFull(StringKit.toInt(prize.num))) return false;
                break;
            case PrizeType.PRIZE_MAGIC_WEAPON:
                if (StorageManagerment.Instance.isMagicWeaponStorageFull(StringKit.toInt(prize.num))) return false;
                break;
            case PrizeType.PRIZE_PROP:
                if (StorageManagerment.Instance.isPropStorageFull(0)) return false;
                break;
            case PrizeType.PRIZE_STARSOUL:
                if (StorageManagerment.Instance.isStarSoulStorageFull(StringKit.toInt(prize.num))) return false;
                break;
        }
        return true;
    }
}

