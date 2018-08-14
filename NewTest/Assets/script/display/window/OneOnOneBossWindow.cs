
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 单挑boss窗口
/// </summary>
public class OneOnOneBossWindow : WindowBase {
	/** BOSSICON */
	public UITexture bossTexture;
	/** BOSS名称 */
	public UISprite bossNameSprite;
	/** 弱点描述 */
	public UILabel weakDes;//读配置
	/** 输出总伤害 */
	public UILabel totalDamage;//后台给
	/** 排行榜按钮 */
	public ButtonBase buttonRank;
	/** 战报按钮 */
	public ButtonBase buttonFightReport;
	/** 挑战按钮 */
	public ButtonBase buttonChallenge;
    /** 商店按钮 */
    public ButtonBase buttonShop;
    /** 可挑战次数 */
    public UILabel challengeTimes;//后台给
    public ButtonBase buyButton;
    //**********************************//
    //帮助信息
    public GameObject tips;
    //**********************************//
    //**********************************//
    //战报信息
    public GameObject fightTips;
    public UILabel[] infos;
	public GameObject fightReportTmp;// 战报描述模板//
	GameObject[] fightReports;
	public UIGrid fightReportGrid;
    //**********************************//
    public GameObject notOpen;//活动未开启
    public UILabel desc;
    public UILabel times;//剩余时间
    //计时器
    private Timer timer;

	bool updateNotOpen = true;
	bool updateOpen = true;

	protected override void begin () {
		base.begin ();

        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(updateInfo);
        timer.start();
		MaskWindow.UnlockUI ();
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		initWindow ();
	}

    private void updateInfo() {
        DateTime dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());//获取服务器时间
        int dayOfWeek = TimeKit.getWeekCHA(dt.DayOfWeek);
        int nowOfDay = ServerTimeKit.getCurrentSecond();
        int[] timeInfo = CommandConfigManager.Instance.getOneOnOneBossTimeInfo();//开放时间
        int[] data = CommandConfigManager.Instance.getOneOnOneBossData();//开放日期
        for (int i = 0; i < data.Length; i++) {
//            if (dayOfWeek == data[i] && (nowOfDay == timeInfo[0] || nowOfDay == timeInfo[1])) {
//                GetBossAttackFPort fport = FPortManager.Instance.getFPort("GetBossAttackFPort") as GetBossAttackFPort;
//                fport.access(CommandConfigManager.Instance.getBossFightSid(), updateUI);
//                return;
//            }
            if (dayOfWeek == data[i] && (nowOfDay >= timeInfo[0] && nowOfDay <= timeInfo[1])) {
				updateNotOpen = true;
				if(updateOpen)
				{
					updateOpen = false;
					GetBossAttackFPort fport = FPortManager.Instance.getFPort("GetBossAttackFPort") as GetBossAttackFPort;
					fport.access(CommandConfigManager.Instance.getBossFightSid(), updateUI);
				}
				updateTime(timeInfo,nowOfDay);
            }
			else
			{
				updateOpen = true;
				if(updateNotOpen)
				{
					updateNotOpen = false;
					updateUIForNotOpen();
				}
			}
        }
    }
	/** 激活 */
	protected override void DoEnable () {
		base.DoEnable ();
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
        if (!AttackBossOneOnOneManager.Instance.bossFightIsOpen()) {
			updateNotOpen = false;
			updateOpen = true;
			updateUIForNotOpen();
        }
		else
		{
			updateNotOpen = true;
			updateOpen = false;
			GetBossAttackFPort fport = FPortManager.Instance.getFPort("GetBossAttackFPort") as GetBossAttackFPort;
			fport.access(CommandConfigManager.Instance.getBossFightSid(), updateUI);
		}
	}

	// 活动未开启的界面刷新//
	public void updateUIForNotOpen()
	{
		weakDes.text = "";
		totalDamage.text = "0";
		times.text = "";
		times.gameObject.SetActive(false);
		int[] timeInfo = CommandConfigManager.Instance.getOneOnOneBossTimeInfo();//开放时间
		int[] data = CommandConfigManager.Instance.getOneOnOneBossData();//开放日期
		string week = "";
		switch (data[0]) {
		case 1:
			week = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_034");
			break;
		case 2:
			week = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_035");
			break;
		case 3:
			week = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_036");
			break;
		case 4:
			week = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_037");
			break;
		case 5:
			week = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_038");
			break;
		case 6:
			week = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_039");
			break;
		case 7:
			week = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_040");
			break;
		}
		desc.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_032", week, (timeInfo[0] / 3600).ToString());
		notOpen.gameObject.SetActive(true);
		bossNameSprite.gameObject.SetActive(false);
		challengeTimes.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
	}

	// 更新倒计时//
	public void updateTime(int[] timeInfo,int nowOfDay)
	{
		times.gameObject.SetActive(true);
		int timeTmp = (timeInfo[1] - nowOfDay) / 3600;
		if(timeTmp == 0)
		{
			timeTmp = (timeInfo[1] - nowOfDay) / 60;
			if(timeTmp == 0)
			{
				timeTmp = (timeInfo[1] - nowOfDay);
				//显示秒//
				times.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_406", timeTmp.ToString());
			}
			else
			{
				// 显示分钟//
				times.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_405", timeTmp.ToString());
			}
		}
		else
		{
			// 显示小时//
			times.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_031", timeTmp.ToString());
		}
	}

    //初始化boss界面(要跟后台通讯)
	public void initWindow () {
        GetBossAttackFPort fport = FPortManager.Instance.getFPort("GetBossAttackFPort") as GetBossAttackFPort;
        fport.access(CommandConfigManager.Instance.getBossFightSid(), updateUI);
        //updateUI();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonChallenge") {//卡片和女神选择
            openTeamEditWindow();
		}
		else if (gameObj.name == "buttonRank") {//排行榜
            UiManager.Instance.openWindow<BossAttackRankWindow>();
		}
		else if (gameObj.name == "close") {
			this.finishWindow ();
        } else if (gameObj.name == "buttonHelp") {
            if (!tips.activeSelf) {
                tips.SetActive(true);
                MaskWindow.UnlockUI();
            } else
                MaskWindow.UnlockUI();
        } else if (gameObj.name == "teamFightInfo") { //战报
			if (!fightTips.activeSelf) {
				fightTips.SetActive(true);
				updateFightInfo ();
				MaskWindow.UnlockUI();
			} else
				MaskWindow.UnlockUI();
        } else if (gameObj.name == "buttonShop") { //商店
            UiManager.Instance.openWindow<HuiJiShopWindow>();
        } else if (gameObj.name == "buttonCloseHelp") { //关闭帮助信息
            tips.SetActive(false);
            MaskWindow.UnlockUI();
		}else if(gameObj.name == "buttonCloseInfo"){
			if(fightReports != null && fightReports.Length > 0)
			{
				for(int i=0;i<fightReports.Length;i++)
				{
					GameObject.Destroy(fightReports[i]);
				}
			}
			fightTips.SetActive (false);
			MaskWindow.UnlockUI ();
        } else if (gameObj.name == "buyButton")
        {
            buyTimes();
        }
	}

    public void buyTimes()
    {

        int vipLevel = UserManager.Instance.self.vipLevel;
        //徽记达上限
        if (StorageManagerment.Instance.getProp(CommandConfigManager.Instance.getHuiJiMoneySid()) != null &&
            StorageManagerment.Instance.getProp(CommandConfigManager.Instance.getHuiJiMoneySid()).getNum() >=
            CommandConfigManager.Instance.getMaxNum())
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_423",
                    CommandConfigManager.Instance.getOpenBuyTimeVipLevel() + "",
                    CommandConfigManager.Instance.getBuyTimeGetHuiJi() + ""));
            });
            return;
        }
        //VIP等级不够
        if (vipLevel < CommandConfigManager.Instance.getOpenBuyTimeVipLevel()) {
            UiManager.Instance.openDialogWindow<TextTipWindow>((win) =>
            {
                win.init(
                    LanguageConfigManager.Instance
                        .getLanguage("OneOnOneBoss_421",
                            CommandConfigManager.Instance
                                .getOpenBuyTimeVipLevel() +
                            "",
                            CommandConfigManager.Instance
                                .getBuyTimeGetHuiJi() + ""),
                    1.5f);
            });
        } else {
            //购买次数用完
            if ((CommandConfigManager.Instance.getBossBuyTimesByVip()[vipLevel] -
                 AttackBossOneOnOneManager.Instance.buyTimes) <= 0)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_422"));
                });
            }
            else //可以购买
            {
                int canBuyNum = CommandConfigManager.Instance.getBossBuyTimesByVip()[vipLevel] -
                                AttackBossOneOnOneManager.Instance.buyTimes;
                int costPrice = CommandConfigManager.Instance.getBuyTimeCost();
                int maxBuyTimes = (UserManager.Instance.self.getRMB()/costPrice);
                BuyWindow.BossAttackTimeBuyStruct buyStruct = new BuyWindow.BossAttackTimeBuyStruct();
                buyStruct.iconId = ResourcesManager.ICONIMAGEPATH + "87";
                buyStruct.unitPrice = costPrice;
                buyStruct.descTime = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_427",
                    Mathf.Min(canBuyNum, maxBuyTimes) + "");
                buyStruct.descExtraGet = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_428",
                    CommandConfigManager.Instance.getBuyTimeGetHuiJi() + "");
                UiManager.Instance.openDialogWindow<BuyWindow>((win) =>
                {
                    win.init(buyStruct, Mathf.Min(canBuyNum, maxBuyTimes), 1, PrizeType.PRIZE_RMB, (msg) => {
                        if (msg.msgEvent != msg_event.dialogCancel) {
                            if (msg.msgNum*costPrice > UserManager.Instance.self.getRMB())
                            {
                                MessageWindow.ShowRecharge(LanguageConfigManager.Instance.getLanguage("s0158"));
                            }
                            else
                            {
                                //通讯
                                BossAttackTimeFPort fPort =
                                    FPortManager.Instance.getFPort("BossAttackTimeFPort") as BossAttackTimeFPort;
                                fPort.access(msg.msgNum, () =>
                                {
                                    AttackBossOneOnOneManager.Instance.buyTimes += msg.msgNum;
                                    UiManager.Instance.openDialogWindow<TextTipWindow>((wins) =>
                                    {
                                        wins.init(
                                            LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_426",
                                                msg.msgNum.ToString(),
                                                (msg.msgNum*CommandConfigManager.Instance.getBuyTimeGetHuiJi()) + ""),
                                            0.8f);
                                    });
                                    updateUI();
                                });
                            }
                        }
                    });
                });
            }
        }
    }

    /// <summary>
    /// 更新战报信息
    /// </summary>
    public void updateFightInfo() {
        List<FightInfo> list = AttackBossOneOnOneManager.Instance.getFightInfo();
//        for (int i = 0; i < infos.Length; i++) {
//            infos[i].text = "";
//        }
        if (list == null || list.Count == 0) {
            fightTips.gameObject.SetActive(true);
            return;
        }
        string infoDesc = "";
//        for (int i = 0; i < list.Count; i++) {
//            infos[i].gameObject.SetActive(true);
//            FightInfo info = list[i];
//            string cardName = QualityManagerment.getQualityColor(CardSampleManager.Instance.getRoleSampleBySid(info.cardSid).qualityId) + CardSampleManager.Instance.getRoleSampleBySid(info.cardSid).name+"[-]";
//            string beastName = "";
//            string bossName = BossInfoSampleManager.Instance.getBossInfoSampleBySid(info.bossSid).name;
//            if (info.beastSid != 0) {
//                beastName = CardSampleManager.Instance.getRoleSampleBySid(info.beastSid).name;
//                infoDesc = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_010", cardName, beastName, bossName, "[FF0000]" + info.damage + "[-]");
//            } else {
//                infoDesc = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_025", cardName, bossName, "[FF0000]" + info.damage + "[-]");
//            }
//            infos[i].text = (i+1) +"."+ infoDesc;
//        }

		fightReports = new GameObject[list.Count];
		GameObject fightReprotObj;
		FightInfo info;
		string cardName;
		string beastName;
		string bossName;
		for(int i=0;i<list.Count;i++)
		{
			fightReprotObj = GameObject.Instantiate(fightReportTmp) as GameObject;
			fightReports[i] = fightReprotObj;
			fightReprotObj.transform.parent = fightReportTmp.transform.parent;
			fightReprotObj.transform.localPosition = Vector3.zero;
			fightReprotObj.transform.localScale = Vector3.one;

			info = list[i];
            int qualityId = CardManagerment.Instance.createCard(info.cardSid).isMainCard()? StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).getQualityId(): 
                CardSampleManager.Instance.getRoleSampleBySid(info.cardSid).qualityId;
			cardName = QualityManagerment.getQualityColor(qualityId) + CardSampleManager.Instance.getRoleSampleBySid(info.cardSid).name+"[-]";
			beastName = "";
			bossName = BossInfoSampleManager.Instance.getBossInfoSampleBySid(info.bossSid).name;
			if (info.beastSid != 0) {
				beastName = CardSampleManager.Instance.getRoleSampleBySid(info.beastSid).name;
				infoDesc = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_010", cardName, beastName, bossName, "[FF0000]" + info.damage + "[-]");
			} else {
				infoDesc = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_025", cardName, bossName, "[FF0000]" + info.damage + "[-]");
			}
			//fightReprotObj.GetComponent<UILabel>().text = (i+1) +"."+ infoDesc;
			fightReprotObj.GetComponent<UILabel>().text = infoDesc;
			fightReprotObj.SetActive(true);
		}
		fightReportGrid.repositionNow = true;
    }

	/// <summary>
	/// 打开卡片选择窗口
	/// </summary>
	private void openTeamEditWindow () {

        DateTime dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());//获取服务器时间
        int dayOfWeek = TimeKit.getWeekCHA(dt.DayOfWeek);
        int nowOfDay = ServerTimeKit.getCurrentSecond();
        int[] timeInfo = CommandConfigManager.Instance.getOneOnOneBossTimeInfo();//开放时间
        int[] data = CommandConfigManager.Instance.getOneOnOneBossData();//开放日期
        for (int i = 0; i < data.Length; i++) {
            if (dayOfWeek == data[i] && nowOfDay > timeInfo[0] && nowOfDay < timeInfo[1]) {
                if (AttackBossOneOnOneManager.Instance.canChallengeTimes + AttackBossOneOnOneManager.Instance.buyTimes <= 0) {//次数用完
                    UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
                        win.init(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_033"), 0.8f);
                    });
                    return;
                }
                UiManager.Instance.openWindow<CardSelectWindow>((win) => {
                    win.Initialize(CardChooseWindow.CHATSHOW);
                });
                return;
            }
        }
        UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {//没开启活动
            win.init(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_018"), 0.8f);
        });
	}
	/// <summary>
	/// 更新UI
	/// </summary>
	private void updateUI () {
		notOpen.gameObject.SetActive(false);
        bossNameSprite.gameObject.SetActive(true);
        challengeTimes.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true);
        tips.SetActive(false);
		BossInfoSample bossSample = BossInfoSampleManager.Instance.getBossInfoSampleBySid(AttackBossOneOnOneManager.Instance.bossSid);
        if (bossSample == null) return;
        //boss形象
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + AttackBossOneOnOneManager.Instance.getBossIcon(), bossTexture);
        //boss弱点描述
        string des = AttackBossOneOnOneManager.Instance.getWeakNess();
        weakDes.text = des;
        //总伤害
        totalDamage.text = AttackBossOneOnOneManager.Instance.getTotalDamage();
        //挑战次数信息
	    challengeTimes.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_011",
	        ((AttackBossOneOnOneManager.Instance.canChallengeTimes + AttackBossOneOnOneManager.Instance.buyTimes) + "/" +
	         CommandConfigManager.Instance.getTimesOfDay()));
        //boss名字
        bossNameSprite.spriteName = "bossName_" + bossSample.nameID;
        AttackBossOneOnOneManager.Instance.damageTemp = StringKit.toLong(AttackBossOneOnOneManager.Instance.getTotalDamage());
		MaskWindow.UnlockUI ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
        if(timer != null)
		    timer.stop();
	}
}
