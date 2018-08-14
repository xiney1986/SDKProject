using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 分享抽大奖容器
/// </summary>
public class ShareDrawContent : MonoBase {

    private Notice notice;
    public UILabel timesLabel;//抽奖次数
    public WindowBase win;
    private Timer timer1;
    private ActiveTime activeTime;
    private NoticeSample sample;
    private int noticeOpenTime;//活动开启时间 
    private int noticeCloseTime;// 活动结束时间
    public ButtonBase drawButton;// 抽奖按钮
    public ButtonBase shareButton;// 分享按钮
    public List<ShareDrawButton> prizeList;//所有奖励集
    private int index = 0;//当前激活的奖励索引
    public Timer timer;//计时器
    private int cycles = 0;//圈数
    private bool isSpeedDown = false;//是否减速
    private List<PrizeSample> psList;
    private EffectCtrl effectCtrl;
    private bool isEndDraw = false;//是否结束抽奖事件
    private int checkPoint = 0;//选中的位置
    private bool isDrawing = false;//是否正在抽奖中
    public UITextList audioList;//广播信息
    string cacheStr;
    //public GameObject endTip;
    //public GameObject endBottom;
    public const int MAX_TIMES = 12;

    private Dictionary<int, EffectCtrl> mEffectDic;
    private ActiveTime activeTime1;

    /// <summary>
    /// 初始化容器
    /// </summary>
    public void initContent(Notice notice, WindowBase win) {
        this.notice = notice;
        this.win = win;
        initButton();
        startTimer();
        setNoticeOpenTime();
        //refreshNoticeTime();
        initDataContent();

    }
    /// <summary>
    /// 初始化抽奖按钮
    /// </summary>
    public void initButton() {
        drawButton.fatherWindow = win;
        shareButton.fatherWindow = win;
        drawButton.onClickEvent = doDrawEvent;
        shareButton.onClickEvent = doShareEvent;
    }

    /// <summary>
    /// 初始化界面
    /// </summary>
    public void initDataContent() {
        mEffectDic = new Dictionary<int, EffectCtrl>();
        cacheRes();
    }

    /// <summary>
    /// 预加载缓存
    /// </summary>
    void cacheRes() {
        string[] _list = new string[]{	
			"Effect/UiEffect/battleDrawWindow_StarsDraw"
		};
        ResourcesManager.Instance.cacheData(_list, cacheWindowFinish, "base");
    }

    public void cacheWindowFinish(List<ResourcesData> _list) {
        init();
        MaskWindow.UnlockUI();

    }
    public void init() {
        loadData();
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    private void loadData() {
        psList = getRandomList();
        for (int i = 0; i < prizeList.Count; i++) {
            prizeList[i].initInfo(psList[i]);
            prizeList[i].fatherWindow = win;

        }
        loadInfo();
    }

    /// <summary>
    /// 获得随机列表
    /// </summary>
    private List<PrizeSample> getRandomList() {
        List<PrizeSample> list = ShareDrawSampleManager.Instance.getShareDrawSampleBySid(notice.sid).list;
        return list;
    }

    /// <summary>
    /// 加载界面
    /// </summary>
    private void loadInfo() {
        if (ShareDrawManagerment.Instance.canDrawTimes == 0) {
            drawButton.disableButton(true);
        }else drawButton.disableButton(false);
        timesLabel.text = ShareDrawManagerment.Instance.canDrawTimes + "/" + MAX_TIMES;
    }

    /// <summary>
    /// 执行抽奖事件
    /// </summary>
    public void doDrawEvent(GameObject obj) {
        this.isDrawing = true;
        setDrawbuttonState();
        if (!isEndDraw && ShareDrawManagerment.Instance.canDrawTimes > 0) {
            clearEffect();
            if (effectCtrl != null)
                effectCtrl.destoryThis();
            ShareDrawFport port = FPortManager.Instance.getFPort("ShareDrawFport") as ShareDrawFport;
            port.ShareDraw(notice.sid, drawOne);
        } else {
            isDrawing = false;
            drawButton.disableButton(true);
            MaskWindow.UnlockUI();
        }
    }
    /// <summary>
    /// 执行分享事件
    /// </summary>
    /// <param name="obj"></param>
    public void doShareEvent(GameObject obj) {
        if (Application.platform == RuntimePlatform.WindowsEditor && Application.platform == RuntimePlatform.OSXEditor)
        {
            if (ShareDrawManagerment.Instance.isFirstShare == 0) {
                SdkOneKeyShareFport fport = FPortManager.Instance.getFPort("SdkOneKeyShareFport") as SdkOneKeyShareFport;
                fport.sendDrawShareSuccess(SdkOneKeyShareFport.TYPE_SHARE_DRAW, () => {
                    loadInfo();
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("shareDraw04"));
                    });
                });
            } else {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("shareDraw04"));
                });

            }
            return;
        }
        UiManager.Instance.openDialogWindow<OneKeyShareWindow>((win) =>
        {
            win.intoType(1,loadInfo);
            win.initWin();
        });
        
    }

    /// <summary>
    ///检查相关仓库是否满 
    /// </summary>
    private bool isStorageFulls(int index) {
        PrizeSample sample = null;
        if (psList != null)
            sample = psList[index];
        int num = 0;
        if (sample != null) {
            num = StringKit.toInt(sample.num);
        }
        if (sample.type == PrizeType.PRIZE_CARD && StorageManagerment.Instance.isRoleStorageFull(num)) {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0172"));
            return true;
        } else if (sample.type == PrizeType.PRIZE_EQUIPMENT && StorageManagerment.Instance.isEquipStorageFull(num)) {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("storeFull_equip"));
            return true;
        } else if (sample.type == PrizeType.PRIZE_MAGIC_WEAPON && StorageManagerment.Instance.isMagicWeaponStorageFull(num)) {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("storeDull_magicWeapon"));
            return true;
        } else if (sample.type == PrizeType.PRIZE_STARSOUL && StorageManagerment.Instance.isStarSoulStorageFull(num)) {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("storeFull_starSoul"));
            return true;
        } else if (StorageManagerment.Instance.isTempStorageFull(20)) {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0173"));
            return true;
        }
        return false;
    }
    /// <summary>
    ///单次抽奖
    /// </summary>
    private void drawOne(int pSid) {
        List<int> sidList = ShareDrawSampleManager.Instance.getShareDrawSampleBySid(notice.sid).prizeSidList;
        if (sidList.Count > 0) {
            for (int i = 0; i < sidList.Count; i++) {
                if (pSid == sidList[i]) {
                    this.checkPoint = i;
                }
            }
        }
        for (int i = 0; i < prizeList.Count; i++) {
            prizeList[i].clearDate();
        }
        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(playResultOne);
        timer.start();
    }

    /// <summary>
    ///播放效果单抽
    /// </summary>
    private void playResultOne() {
        index++;
        if (index >= prizeList.Count) {
            cycles++;
            index = 0;
        }
        for (int i = 0; i < prizeList.Count; i++) {
            if (i == index) {
                moveEffect(prizeList[i].transform);
            }
        }
        if (cycles == 4 && index == changeIndex()) {
            isSpeedDown = true;
            timer.reset();
        }
        if (isSpeedDown) {
            speedDown();
        } else {
            speedUp();
        }
    }

    /// <summary>
    /// 移动特效
    /// </summary>
    private void moveEffect(Transform trans) {
        if (effectCtrl == null)
            effectCtrl = EffectManager.Instance.CreateEffect(trans, "Effect/UiEffect/battleDrawWindow_StarsDraw");
        effectCtrl.transform.position = trans.position;
    }
    /// <summary>
    /// 清理特效
    /// </summary>
    private void clearEffect() {
        foreach (KeyValuePair<int, EffectCtrl> item in mEffectDic) {
            prizeList[item.Key].clearDate();
            EffectManager.Instance.removeEffect(item.Value);
        }
        mEffectDic.Clear();
    }
    /// <summary>
    /// 改变选择项
    /// </summary>
    private int changeIndex() {
        int temp = checkPoint - 4;
        if (temp < 0) {
            temp = prizeList.Count + temp;
        }
        return temp;
    }

    /// <summary>
    /// 加速
    /// </summary>
    private void speedUp() {
        timer.delayTime -= 300;
        if (timer.delayTime <= 100) {
            timer.delayTime = 1;
        }
    }

    /// <summary>
    /// 减速
    /// </summary>
    private void speedDown() {
        timer.delayTime += 100;
        if (index == checkPoint) {
            StartCoroutine(playEffect(index));
            timer.stop();
            timer = null;
            cycles = 0;
            isSpeedDown = false;
            if (isStorageFulls(checkPoint)) {
                isDrawing = false;
                return;
            } else {
                ShareDrawManagerment.Instance.canDrawTimes -= 1;
                string name = QualityManagerment.getQualityColor(prizeList[index].getPrize().getQuality()) + prizeList[index].getPrize().getPrizeName();
                init();//更新
                UiManager.Instance.createMessageLintWindowNotUnLuck(LanguageConfigManager.Instance.getLanguage("superDraw_18", name, prizeList[index].getPrize().getPrizeNumByInt().ToString()));
                isDrawing = false;
            }
            setDrawbuttonState();
        }

    }

    public void setDrawbuttonState() {
        if (ShareDrawManagerment.Instance.canDrawTimes > 0 && !isDrawing)
            drawButton.disableButton(false);
        else
            drawButton.disableButton(true);
    }
    /// <summary>
    /// 播放特效
    /// </summary>
    private IEnumerator playEffect(int i) {
        yield return new WaitForSeconds(0.2f);
        if (mEffectDic.ContainsKey(i)) yield break;
        effectCtrl = EffectManager.Instance.CreateEffect(prizeList[i].transform, "Effect/UiEffect/Surroundeffect", "Surroundeffect_y");
        effectCtrl.transform.localPosition = Vector3.zero;
        effectCtrl.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        effectCtrl.transform.parent = prizeList[i].transform;
        mEffectDic.Add(i, effectCtrl);
    }

    /// <summary>
    /// 开启计时器
    /// </summary>
    private void startTimer() {
        if (timer1 == null)
            timer1 = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(notice.sid);
        activeTime1 = ActiveTime.getActiveTimeByID(sample.timeID);
        timer1.addOnTimer(updateNotice);
        timer1.start();
    }

    private void updateNotice() {
        if (ServerTimeKit.getSecondTime() > activeTime1.getEndTime()) {
            if (UiManager.Instance.getWindow<NoticeWindow>() != null) {
                UiManager.Instance.getWindow<NoticeWindow>().initTopButton();
            }
            timer1.stop();
        }
    }
    /// <summary>
    /// 更新界面
    /// </summary>
    public void updateUI() {
        if (ShareDrawManagerment.Instance.canDrawTimes == 0)
            drawButton.disableButton(true);
        else
            drawButton.disableButton(false);
    }

    /// <summary>
    /// 终止计时器
    /// </summary>
    public void OnDisable() {
        if (timer != null) {
            timer.stop();
            timer = null;
        }
        if (timer1 != null) {
            timer1.stop();
        }
        clearEffect();
    }

    /// <summary>
    /// 设置活动开启时间
    /// </summary>
    public void setNoticeOpenTime() {
        activeTime = ActiveTime.getActiveTimeByID(this.notice.getSample().timeID);
        noticeOpenTime = activeTime.getDetailStartTime();
        noticeCloseTime = activeTime.getDetailEndTime();
    }
}

