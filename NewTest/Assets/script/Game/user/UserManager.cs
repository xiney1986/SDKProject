using System;
using UnityEngine;
using System.Collections.Generic;

/**
 * 用户管理器
 * @author zhanghaishan
 * */
public class UserManager
{

    public User self = null;
    public const int PVE_SPEED = 450 * 1000;//行动力恢复速度 暂时定一分钟450
    public const int GUILD_TIME_SPEED = 4 * 60 * 60;//金币公会捐献的时间间隔为4小时
                                                    //public const int PVE_SPEED = 10000;//行动力恢复速度 暂时定一分钟
    public const int PVP_SPEED = 30 * 60 * 1000;//体力恢复速度
                                                //	public const int PVP_SPEED = 10000;
    public const int CHV_SPEED = 5 * 60 * 1000;//英雄之章挑战次数恢复速度
    public const int TIMER_DELAY = 1 * 1000;//计时器频率，秒
    public const int TIMER_MINUTE = 60 * 1000;//计时器频率，分钟
    public const int CHATSHOWLEVEL = 9;//聊天图标显示等级
                                       /** 登录时,当前完成的总数量 */
    public int currentRequestCount = 0;
    /** 登录时，请求的总数量 */
    public const int TOTAL_LOGIN_REQUEST = 30;
    int[] icons = new int[] { 404, 405, 401, 408, 407, 402 };
    public bool pveStoreEffect = false;
    public static UserManager Instance
    {

        get { return SingleManager.Instance.getObj("UserManager") as UserManager; }
    }

    public void setSelfHeadIcon(UITexture obj)
    {
        ResourcesManager.Instance.LoadAssetBundleTexture(UserManager.Instance.self.getIconPath(), obj);
    }

    public void createSelf(string uid, string nickname, int style, int money, int rmb, string mainCardUid, long exp, int maxLevel, long vipExp, int[] vipAwardSids,
                            int lastLevelupRewardSid, int executionPve, int executionPveMax, long executionPveSpeed, int executionPvp, int executionPvpMax, long executionPvpSpeed,
                            int executionChv, int executionChvMax, long executionChvSpeed,
                            int storePve, int storePveMax, long storePveSpeed,
                            int winNum, int winNumDay, int winRankDay, int honorLevel, int honor, string guildId, string guildName, int firendsNum, int titleId,
                            int arenaScore, int activeScore, long serverTime, int star, int merit, int hightPoint, int battlePlayVelocity, int prestige, int ladderRank, int onlineTime,
                            bool canFrist, int guildFightPower, int guildFightPowerMax)
    {


        //先同步时间
        ServerTimeKit.initTime(serverTime, Time.realtimeSinceStartup, onlineTime);


        if (self == null)
        {
            self = new User();
        }

        self.uid = uid;
        //		//添加守护天使信息,如果存在不添加
        if (PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME + self.uid) == "")
            PlayerPrefs.SetString(PlayerPrefsComm.ANGEL_USER_NAME + self.uid, "not");
        self.nickname = nickname;
        self.style = style;
        self.updateMoney(money);
        self.updateRMB(rmb);
        self.mainCardUid = mainCardUid;
        self.updateExp(exp);
        self.maxLevel = maxLevel;
        self.updateVipExp(vipExp);
        if (self.getVipLevel() > 0 && PlayerPrefs.GetString(PlayerPrefsComm.VIP_EXCHANGE_TIP) != self.uid)
            PlayerPrefs.SetString(PlayerPrefsComm.VIP_EXCHANGE_TIP, "ok");
        else
            PlayerPrefs.SetString(PlayerPrefsComm.VIP_EXCHANGE_TIP, "not");
        self.updateVipRewardLevel(vipAwardSids);
        self.updateLevelupRewardLastSid(lastLevelupRewardSid);
        self.setPvEPoint(executionPve);
        self.setPvEPointMax(executionPveMax);

        //executionPveSpeed:上次回复的时间点和现在时间点的差值
        self.setPveFirstTime(UserManager.PVE_SPEED - executionPveSpeed);
        self.setPvPPoint(executionPvp);
        self.setPvPPointMax(executionPvpMax);
        self.setPvpFirstTime(UserManager.PVP_SPEED - executionPvpSpeed);
        self.setChvPoint(executionChv);
        self.setChvPointMax(executionChvMax);
        self.setChvFirstTime(UserManager.CHV_SPEED - executionChvSpeed);
        self.setStorePvEPoint(storePve);
        self.setStorePvEPointMax(storePveMax);
        self.setStorePveFirstTime(MountsConfigManager.Instance.getPveSpeed() - storePveSpeed);

        self.winNum = winNum;
        self.winNumDay = winNumDay;
        self.winRankDay = winRankDay;
        self.honorLevel = honorLevel;
        self.honor = honor;
        self.guildId = guildId;
        self.guildName = guildName;
        self.firendsNum = firendsNum;
        self.titleId = titleId;
        self.arenaScore = arenaScore;
        self.activeScore = activeScore;
        self.merit = merit;
        self.startCountdown();
        self.star = star;
        self.practiceHightPoint = hightPoint;
        self.setBattlePlayVelocity(battlePlayVelocity);
        self.prestige = prestige;
        self.updateLadderRank(ladderRank);
        RechargeManagerment.Instance.canFirst = canFrist;
        self.guildFightPower = guildFightPower;
        self.guildFightPowerMax = guildFightPowerMax;
    }

    public void updateUserPvE(int pve)
    {
        if (self == null)
            return;
        self.setPvEPoint(pve);
    }

    /// <summary>
    /// 更新坐骑行动力
    /// </summary>
    public void updateMountsPvE(int pve)
    {
        if (self == null)
            return;
        self.setStorePvEPoint(pve);
    }

    //更新用户体力
    public void updateUserPvP(int pvp)
    {
        if (self == null)
            return;
        self.setPvPPoint(pvp);

    }

    //更新用户行动力速度 pve_t 表示已经流逝的时间
    public void updateUserPvESpeed(int pve_t)
    {
        self.setPveFirstTime(pve_t);
    }

    /// <summary>
    /// 更新坐骑行动力速度 pve_t 表示已经流逝的时间
    /// </summary>
    public void updateStorePvESpeed(int pve_t)
    {
        self.setStorePveFirstTime(pve_t);
    }

    //更新用户英雄之章挑战次数
    public void updateUserChv(int chv)
    {
        if (self == null)
            return;
        self.setChvPoint(chv);

    }

    public string getNextPveTime()
    {
        long time = self.updatePve();
        if (time > 0)
        {
            return TimeKit.timeTransform(time);
        }
        return "";
    }

    /// <summary>
    /// 获得坐骑行动力倒计时
    /// </summary>
    public string getNextMountsPveTime()
    {
        long time = self.updateStorePve();
        if (time > 0)
        {
            return TimeKit.timeTransform(time);
        }
        return "";
    }

    public string getNextPvpTime()
    {
        long time = self.updatePvp();
        if (time > 0)
        {
            return TimeKit.timeTransform(time);
        }
        return "";

    }

    //得到英雄之章挑战次数恢复速度
    public string getNextChvTime()
    {
        long time = self.updateChv();
        if (time > 0)
        {
            return TimeKit.timeTransform(time);
        }
        return "";
    }

    //更新用户体力速度 pvp_t 表示已经流逝的时间
    public void updateUserPvPSpeed(int pvp_t)
    {
        if (self == null)
            return;
        self.setPvpFirstTime(pvp_t);
    }

    //更新英雄之章挑战次数速度 chv_t 表示已经流逝的时间
    public void updateUserChvSpeed(int chv_t)
    {
        if (self == null)
            return;
        self.setChvFirstTime(chv_t);
    }

    //更新用户行动力最大值
    public void updatePvEMax(int pve_m)
    {
        if (self == null)
            return;
        self.setPvEPointMax(pve_m);
    }

    /// <summary>
    /// 更新坐骑行动力最大值
    /// </summary>
    public void updateMountsPvEMax(int pve_m)
    {
        if (self == null)
            return;
        self.setStorePvEPointMax(pve_m);
    }

    //更新用户体力最大值
    public void updatePvPMax(int pvp_m)
    {
        if (self == null)
            return;
        self.setPvPPointMax(pvp_m);
    }

    //更新英雄之章挑战次数最大值
    public void updateChvMax(int chv_m)
    {
        if (self == null)
            return;
        self.setChvPointMax(chv_m);
    }
    #region 登录初始化
    public void login()
    {
        ActiveVSNFPort vsnFport = FPortManager.Instance.getFPort<ActiveVSNFPort>();
        vsnFport.getActiveVSN(initUser);
        MaskWindow.LockUI();
    }
    /// <summary>
    /// 设置登录的Loading进度
    /// </summary>
    /// <param name="step">完成一步+1</param>
    /// <param name="isDestroyLoading">If set to <c>true</c>销毁Loading窗口</param>
    void SetLoginProgress(int step, bool isDestroyLoading)
    {
        if (UiManager.Instance.ActiveLoadingWindow == null)
            return;
        currentRequestCount += step;
        float data = (float)currentRequestCount / TOTAL_LOGIN_REQUEST;
        UiManager.Instance.ActiveLoadingWindow.setProgress(data);
        if (isDestroyLoading)
        {
            currentRequestCount = 0;
            LoadingWindow.isShowProgress = false;
            UiManager.Instance.destoryWindowByName("LoginWindow");
            UiManager.Instance.ActiveLoadingWindow.destoryWindow();
        }
    }
    /** 初始化User */
    public void initUser()
    {
        ServerManagerment.Instance.loginServer();
        currentRequestCount = 0;
        if (UserManager.Instance.self == null)
            showLoginLoading();
        else
            loadUserData();
    }
    /// <summary>
    /// 显示登录的loading
    /// </summary>
    void showLoginLoading()
    {
        LoadingWindow.isShowProgress = true;
        if (UiManager.Instance.ActiveLoadingWindow == null)
        {
            UiManager.Instance.openDialogWindow<LoadingWindow>((win) =>
            {
                UiManager.Instance.ActiveLoadingWindow.justLoading = true;
                loadUserData();
            });
        }
        else
        {
            UiManager.Instance.ActiveLoadingWindow.justLoading = true;
            loadUserData();
        }
    }
    /// <summary>
    /// 从服务器加载玩家数据
    /// </summary>
    public void loadUserData()
    {
        UserFPort userport = FPortManager.Instance.getFPort<UserFPort>();
        userport.access(userInfoOK);
    }

    void userInfoOK()
    {
        //	Debug.LogWarning("card_storage");
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("card_storage", cardStorageOK);

    }

    void cardStorageOK()
    {
        //		Debug.LogWarning("card_storageok");
        //type仓库名字：card_storage，beast_storage，equip_storage，goods_storage，temp_storage
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("beast_storage", beastStorageOK);
    }

    void beastStorageOK()
    {
        //	Debug.LogWarning("beastStorageOK");
        //equip_storage，goods_storage，temp_storage
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("equip_storage", equipStorageOK);
    }

    void equipStorageOK()
    {
        //goods_storage，temp_storage
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("goods_storage", starSoulStorageOK);
    }

    void starSoulStorageOK()
    {
        //goods_storage，temp_storage
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("star_soul_storage", hunStarSoulStorageOK);
    }

    void hunStarSoulStorageOK()
    {
        //goods_storage，temp_storage
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("star_soul_draw_storage", goodsStorageOK);
    }

    void goodsStorageOK()
    {
        //，temp_storage
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("temp_storage", mountsStorageOK);
    }

    void mountsStorageOK()
    {
        SetLoginProgress(1, false);
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("mounts_storage", initMagicWeaponStoreOK);
    }
    //这里添加一个仓库（秘宝）
    void initMagicWeaponStoreOK()
    {
        SerializeStorageFPort stf = FPortManager.Instance.getFPort<SerializeStorageFPort>();
        stf.access("artifact_storage", tempStorageOK);
    }

    void tempStorageOK()
    {
        SetLoginProgress(1, false);
        UserNewFPort userNewFPort = FPortManager.Instance.getFPort<UserNewFPort>();
        userNewFPort.access(initStoryFuben);//初始化用户
    }

    void initStoryFuben()
    {
        SetLoginProgress(1, false);
        FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
        port.init(initTaofaFport, 1);
    }
    void initTaofaFport()
    {
        SetLoginProgress(1, false);
        FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
        port.init(initFriendsFPort, 501);
    }
    void initFriendsFPort()
    {
        SetLoginProgress(1, false);
        FriendsFPort port = FPortManager.Instance.getFPort("FriendsFPort") as FriendsFPort;
        port.initFriendsInfo(initGuideDate);
    }
    void initGuideDate()
    {
        SetLoginProgress(1, false);
        GuildGetInfoFPort prot = FPortManager.Instance.getFPort("GuildGetInfoFPort") as GuildGetInfoFPort;
        prot.access(InitMining);
    }
    void InitMining()
    {
        FPortManager.Instance.getFPort<GetMineralsFport>().access(InitMineralEnemyList);
    }
    void InitMineralEnemyList()
    {
        FPortManager.Instance.getFPort<GetMineralEnemiesFport>().access(InitArmy);
    }
    void InitArmy()
    {
        FPortManager.Instance.getFPort<ArmyGetFPort>().access(initUserOk);
    }

    //初始化用户完成，处理不分数据效验和更新
    private void initUserOk()
    {
        SetLoginProgress(1, false);
        //校验阵形
        if (ArmyManager.Instance.checkFormation())
        {
            Army[] list = new Army[3];
            list[0] = ArmyManager.Instance.getArmy(1);
            list[1] = ArmyManager.Instance.getArmy(2);
            list[2] = ArmyManager.Instance.getArmy(3);
            ArmyUpdateFPort port = FPortManager.Instance.getFPort<ArmyUpdateFPort>();
            port.access(list, getArenaFinal);
        }
        else
        {
            getArenaFinal();
        }
    }
    //获取决赛信息
    void getArenaFinal()
    {
        SetLoginProgress(1, false);
        ArenaManager manager = ArenaManager.instance;
        if (!manager.isStateCorrect() || manager.state == ArenaManager.STATE_WAIT || manager.state == ArenaManager.STATE_MASS_RESET || manager.state == ArenaManager.STATE_MASS)
        {
            getFuBenGetCurrent();
        }
        else
        {
            ArenaFinalFPort fport = FPortManager.Instance.getFPort<ArenaFinalFPort>();
            fport.access(getFuBenGetCurrent);
        }
    }

    //是否在副本中
    void getFuBenGetCurrent()
    {
        SetLoginProgress(1, false);
        FuBenGetCurrentFPort port = FPortManager.Instance.getFPort("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
        port.getInfo(getGuideInfo, false);
    }

    //获取新手信息
    void getGuideInfo(bool isInside)
    {
        SetLoginProgress(1, false);
        GuideManager.Instance.isLoginOnInMission = isInside;
        FPortManager.Instance.getFPort<GuideGetInfoFPort>().getInfo(getMail);
    }

    // 邮件
    void getMail()
    {
        SetLoginProgress(1, false);
        MailGetFPort fport = FPortManager.Instance.getFPort<MailGetFPort>();
        fport.access(getInviteCode);
    }
    //获取邀请码后台进度等信息
    void getInviteCode()
    {
        InviteCodeInfoFPort fport = FPortManager.Instance.getFPort<InviteCodeInfoFPort>();
        fport.access(getGuildSkill);
    }
    //公会技能--公会技能与有无公会无关
    void getGuildSkill()
    {
        SetLoginProgress(1, false);
        GuildGetSkillFPort fport = FPortManager.Instance.getFPort<GuildGetSkillFPort>();
        fport.access(getGuildBuilding);
    }
    //工会建筑
    void getGuildBuilding()
    {
        SetLoginProgress(1, false);
        if (GuildManagerment.Instance.getGuild() != null)
        {
            GuildBuildLevelGetFPort fport = FPortManager.Instance.getFPort<GuildBuildLevelGetFPort>();
            fport.access(getGuildInviate);
        }
        else
        {
            getGuildInviate();
        }
    }
    //工会邀请
    void getGuildInviate()
    {
        SetLoginProgress(1, false);
        if (GuildManagerment.Instance.getGuild() == null)
        {
            GuildGetInviteFPort fport = FPortManager.Instance.getFPort("GuildGetInviteFPort") as GuildGetInviteFPort;
            fport.access(getGuildApply);
        }
        else
        {
            getGuildApply();
        }
    }

    //工会审批
    void getGuildApply()
    {
        SetLoginProgress(1, false);
        if (GuildManagerment.Instance.getGuild() != null)
        {
            GuildGetApprovalListFPort fport = FPortManager.Instance.getFPort("GuildGetApprovalListFPort") as GuildGetApprovalListFPort;
            fport.access(getLaddersAttrEffect);
        }
        else
        {
            getLaddersAttrEffect();
        }
    }

    //天梯称号属性加成
    void getLaddersAttrEffect()
    {
        SetLoginProgress(1, false);
        LaddersAttrEffectFPort fport = FPortManager.Instance.getFPort<LaddersAttrEffectFPort>();
        fport.apply(initUserPvpInfo);
    }
    //初始玩家pvp信息
    void initUserPvpInfo()
    {
        SetLoginProgress(1, false);
        FPortManager.Instance.getFPort<PvpGetInfoFPort>().access(initQuiz);
    }

    void initQuiz()
    {
        SetLoginProgress(1, false);
        QuizManagerment.Instance.getQuestions(initTeamOpenIndex);
    }
    void initTeamOpenIndex()
    {
        TeamEmtpyInfoFPort fport = FPortManager.Instance.getFPort<TeamEmtpyInfoFPort>();
        fport.access(getWeeklyAward);
    }

    //获得周末送有没有奖励
    void getWeeklyAward(List<int> inds)
    {
        GuideManager.Instance.openIndex = inds;
        SetLoginProgress(1, false);
        WeeklyAwardFPort fport = FPortManager.Instance.getFPort<WeeklyAwardFPort>();
        fport.access(getHolidayAward);
    }
    //获得节日送有没有奖励
    void getHolidayAward()
    {
        SetLoginProgress(1, false);
        HolidayAwardFPort fport = FPortManager.Instance.getFPort<HolidayAwardFPort>();
        fport.access(TotalLoginManagerment.Instance.getHolidayActionsTate(), getTranningTime);
        //		if (TotalLoginManagerment.Instance.getHolidayActionsTate () != 0) {
        //
        //		} else {
        //			getGMNoticeActive ();
        //		}
    }
    void getTranningTime()
    {
        GoddessTrainingInit fport = FPortManager.Instance.getFPort("GoddessTrainingInit") as GoddessTrainingInit;
        fport.access(getGMNoticeActive);
    }

    void getGMNoticeActive(int time)
    {
        GuideManager.Instance.goddessTranningTime = time;
        SetLoginProgress(1, false);
        FPortManager.Instance.getFPort<NoticeGetGMActiveFPort>().access(getNoticeActiveInfo);
    }

    //获得公告活动信息
    void getNoticeActiveInfo()
    {
        SetLoginProgress(1, false);
        List<int> sidList = new List<int>();
        NoticeSample sample;
        NoticeSampleManager.Instance = null;
        foreach (Notice notice in NoticeManagerment.Instance.getNoticeList())
        {
            sample = notice.getSample();
            if (sample.type == NoticeType.CONSUME_REBATE)
                sidList.Add(sample.sid);
            else if (sample.type == NoticeType.NEW_RECHARGE || sample.type == NoticeType.NEW_CONSUME)
            {
                foreach (int sid in (sample.content as SidNoticeContent).sids)
                    sidList.Add(sid);
            }
            else if (sample.type == NoticeType.NEW_EXCHANGE)
            {
                foreach (NoticeActiveAndSid info in (sample.content as NewExchangeNoticeContent).actives)
                    sidList.Add(info.activeID);
            }
            else if (sample.type == NoticeType.DOUBLE_RMB)
            {
                sidList.Add(sample.sid);
            }
            else if (sample.type == NoticeType.ONERMB)
            {
                foreach (int sid in (sample.content as SidNoticeContent).sids)
                    sidList.Add(sid);
            }
            else if (sample.type == NoticeType.REMAKE_EQUIP)
            {
                sidList.Add(sample.sid);
            }
            else if (sample.type == NoticeType.LIMIT_COLLECT)
            {
                Debug.Log("Limit_collect!!!!");
                foreach (int sid in (sample.content as SidNoticeContent).sids)
                {
                    Debug.Log(sid);
                    sidList.Add(sid);
                }

            }
        }
        if (sidList.Count > 0)
        {
            FPortManager.Instance.getFPort<NoticeActiveGetFPort>().access(sidList.ToArray(), initBulletin);
        }
        else
        {
            initBulletin();
        }
    }

    void initBulletin()
    {
        //FPortManager.Instance.getFPort<BulletinFPort> ().getBulletin (initFinish);
        FPortManager.Instance.getFPort<BulletinFPort>().getBulletin(initBackPrizeInfo);
    }

    //回归登录领奖信息 //
    void initBackPrizeInfo()
    {
        BackPrizeInfoFPort bpif = FPortManager.Instance.getFPort("BackPrizeInfoFPort") as BackPrizeInfoFPort;
        bpif.BackPrizeLoginInfoAccess(initBackPrizeRechargeInfo);
    }

    // 回归累计充值信息//
    void initBackPrizeRechargeInfo()
    {
        BackPrizeRechargeInfoFPort bpif1 = FPortManager.Instance.getFPort("BackPrizeRechargeInfoFPort") as BackPrizeRechargeInfoFPort;
        bpif1.BackPrizeRechargeInfoAccess(initWeekCardInfo);
    }

    // 周卡信息//
    void initWeekCardInfo()
    {
        WeekCardInfoFPort fPort = FPortManager.Instance.getFPort("WeekCardInfoFPort") as WeekCardInfoFPort;
        fPort.WeekCardInfoAccess(initRebateInfo);
    }

    // 福袋信息//
    void initRebateInfo()
    {
        RebateInfoFPort rebateInfo = FPortManager.Instance.getFPort("RebateInfoFPort") as RebateInfoFPort;
        if (rebateInfo.diamondSample == null && rebateInfo.goldSample == null)
        {
            List<int> ids = RebateSampleManager.Instance.getAllIDs();
            rebateInfo.setDiamondSample(RebateSampleManager.Instance.getDiamondSampleByIDs(ids));
            rebateInfo.setGoldSample(RebateSampleManager.Instance.getGoldSampleByIDs(ids));
        }
        //rebateInfo.RebateInfoAccess(initFinish);
        //rebateInfo.RebateInfoAccess(initSevenDaysHappyInfo);
        rebateInfo.RebateInfoAccess(initShopInfo);
    }

    // 初始化商店//
    void initShopInfo()
    {
        InitShopFPort fport = FPortManager.Instance.getFPort("InitShopFPort") as InitShopFPort;
        fport.access(initSevenDaysHappyInfo);
    }

    // 七日狂欢//
    void initSevenDaysHappyInfo()
    {
        SevenDaysHappyInfoFPort info = FPortManager.Instance.getFPort("SevenDaysHappyInfoFPort") as SevenDaysHappyInfoFPort;
        info.SevenDaysHappInfoAccess(initFinish);
    }

    void initFinish()
    {
        SetLoginProgress(1, true);
        //通用 接口
        SdkManager.INSTANCE.SetRoleData(self.uid,self.nickname,self.getUserLevel().ToString(),ServerManagerment.Instance.lastServer.sid, ServerManagerment.Instance.lastServer.Namec);
        SdkManager.INSTANCE.SetData("enterServer", self.ToDic());
        SdkManager.INSTANCE.SetData("loginGameRole", self.ToDic());
        SdkManager.INSTANCE.EnterGame();
        //如果是断线重连
        DateTime ServerOpenTime = TimeKit.getDateTimeMin(ServerTimeKit.onlineTime);

        if (GameManager.Instance.getStartResumeConnect())
        {
            GameManager.Instance.ResumeConnectFinish();
        }
        else
        {
            GameManager.Instance.loginStateManagr.isConnectOK = true;
            // gotoMainWindow ();
        }
    }
    //得到头像路径
    public string getIconPath(int iconId)
    {
        return "texture/roleIcon/roleIcon_" + icons[iconId - 1];
    }
    //得到头像路径
    public string getImagePath(int iconId)
    {
        return ResourcesManager.CARDIMAGEPATH + icons[iconId - 1];
    }
    //得到模型prefab路径
    public string getModelPath(int iconId)
    {
        switch (iconId)
        {
            case 1:
                return "mission/ez";

            case 2:
                return "mission/girl";

            case 3:
                return "mission/swordsman";

            case 4:
                return "mission/archer";

            case 5:
                return "mission/maleMage";

            case 6:
                return "mission/mage";

        }
        return "mission/swordsman";
    }

    /** 打开活动窗口 */
    public bool openNoticeWindow(int entranceId)
    {
        int noticeSid = NoticeManagerment.Instance.getFirstBootReceiveByTime(entranceId);
        if (noticeSid > 0)
        {
            UiManager.Instance.openWindow<NoticeWindow>((win) =>
            {
                win.firstBootBySid(noticeSid);
            });
            return true;
        }
        else
        {
            //如果玩家领取了首充奖励。则弹出月卡、炼金、女神摇一摇任意一个有未完成次数的页面。
            //如果无配置限时活动弹出；已经领取了首充奖励；月卡、炼金、女神摇一摇都没有次数了，则按活动ID顺序弹出存在的页面
            int[] firstShowNotices = new int[] {
                NoticeType.ONERMB,
                NoticeType.MONTHCARD,
                NoticeType.ALCHEMY,
                NoticeType.HAPPY_TURN_SPRITE
            };
            //index = NoticeManagerment.Instance.getFirstNotice(firstShowNotices);			
            int sid = NoticeManagerment.Instance.getFirstNotice(firstShowNotices);
            if (sid != 0)
            {
                UiManager.Instance.openWindow<NoticeWindow>((win) =>
                {
                    //win.firstBoot (index);
                    win.firstBootBySid(sid);
                });
                return true;
            }
        }
        return false;
    }

    public void gotoMainWindow()
    {
        ArmyManager.Instance.recalculateAllArmyIds();
        //登录后自动清除退出提示窗，避免玩家跳过新手引导的风险
        if (UiManager.Instance.getWindow<ExitWindow>() != null && UiManager.Instance.getWindow<ExitWindow>().gameObject.activeSelf)
        {
            UiManager.Instance.getWindow<ExitWindow>().finishWindow();
        }

        //进入引导
        if (!GuideManager.Instance.isMissionComplete() || (GuideManager.Instance.isLessThanStep(50001000) && GuideManager.Instance.isLoginOnInMission)
            || !GuideManager.Instance.isGuideComplete())
        {
            UiManager.Instance.initNewPlayerGuideLayer();
            GuideManager.Instance.intoGuide();
        }
        else if (GuideManager.Instance.isMoreThanStep(50001000))
        {
            if (((TotalLoginManagerment.Instance.getHaveWeeklyAwardShow() && TotalLoginManagerment.Instance.getHaveWeeklyAward()) ||
                 TotalLoginManagerment.Instance.getHaveHolidayAward() ||
                 TotalLoginManagerment.Instance.getActiveAwardNum() > 0 || (TotalLoginManagerment.Instance.EverydayState && TotalLoginManagerment.Instance.isNewAward())) && self.getUserLevel() > 5)
                UiManager.Instance.openWindow<TotalLoginWindow>((win) =>
                {
                    win.Initialize(true);
                });
            else
            {
                //2014.8.21 杨大侠说不弹出活动了，太烦
                //					bool isOpen = openNoticeWindow ();
                //					if (!isOpen)
                UiManager.Instance.openWindow<MainWindow>();
            }
        }
        else
        {
            UiManager.Instance.openWindow<MainWindow>();
        }
    }
    #endregion
    public void exit()
    {
        MonoBase.print("dis connect");
        //关闭新手引导
        if (GuideManager.Instance.guideUI != null)
        {
            GuideManager.Instance.hideGuideUI();
        }

        //关闭连接 
        ConnectManager.manager().closeAllConnects();
        //清除登录状态
        ServerManagerment.Instance.isLogin = false;

        //清除奖励缓存
        AwardsCacheManager.clearAllCache();
        BattleManager.isWaitForBattleData = false;
        //清单列
        SingleManager.Instance.clean();
        //清理请求数据
        DataAccess.getInstance().clearDataAccess();
        //清理端口数据
        FPortManager.Instance.clearPorts();
        //清除时间
        if (GameManager.Instance.timer != null)
            TimerManager.Instance.removeTimer(GameManager.Instance.timer);

        //清除计时器
        TimerManager.Instance.clearAllTimer();
    }
}