using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class MainWindow :WindowBase
{
	//按钮
	public ButtonBase totalPrizeButton;//累积登录奖励按钮
	public ButtonBase oneRmbButton;	//首冲奖励按钮
	public ButtonBase doubleRMBButton; //双倍充值
	public ButtonBase foreShowButton;//预告按钮
	public ButtonBase increaseButton;//强化提示按钮
	public ButtonBase buttonExchange;//兑换
	public ButtonBase buttonRank;//排行榜
	public ButtonChat chatButton;//聊天按钮
    public ButtonBase xianshiButton;//限时活动按钮
	//public ButtonBase shareButton;//分享按钮
	public ButtonBase intoFubenButton;//进副本
	public ButtonBase resolveButton;//进副本
	public ButtonBase growupButton;
    public ButtonBase zhouNianQingButton;//周年庆按钮
	public ButtonBase backButton;// 回归按钮//
	public ButtonBase sevenDaysHappyBtn;// 七日狂欢按钮//
    public ButtonBase YaoQingButton;//邀请按钮
	//文本
	public UILabel pveValue;
	public UILabel mountsPveValue;
	public UILabel mountsPveTimeLabel;
	/** 玩家等级 */
	public UILabel levelLabel;
	public UILabel moneyLabel;
	public UILabel rmbLabel;
	public UILabel nameLabel;
	public UILabel pveTimeLabel;
	public UILabel pvpTimeLabel;
	/** 队伍战斗力 */
	public UILabel combat;
	public UILabel storeNum; // 可开宝箱数
	public UILabel exchangeNum; //兑换数量
	public UILabel foreShowPrompt;//预告提示
	public UILabel taskedNum;//任务数量
	public UILabel arenaNum;//竞技场数量
	public UILabel luckyNum;//抽奖总次数
	public UILabel growupNum;//成长计划可领取数量
	public UILabel mailNum;
	public UILabel friendNum;
	public UILabel heroRoadCountLabel;
	public UILabel announcementNum; //日常活动可领取的数量
	public UILabel xianshiNum; //限时活动可领取数量
    public UILabel zhouNianQingNum;
	public UILabel backButtonNum;
	public UILabel sevenDaysHappyNum;
	public UILabel UI_CardTrainingNum; //卡牌训练可用数量
	public UILabel invitationNum;//提示是否有未领取的邀请奖励
	public UILabel magicWeaponNum;// 可合成神器数量//
	public GameObject guildInviteTip;//公会邀请提示
	/** 友情指引说明 */
	public UILabel labelFriendlyGuideDesc;
	/** Vip等级 */
	public UILabel vipLevel;

	//背景
	public UISprite storeBackground; //仓库数量背景
	public UISprite exchangeBackground; //兑换数量背景
	public UISprite noticeBackground;//公告按钮背景
	public UISprite taskBackground;//任务数量背景
	public UISprite arenaBackground; //竞技场数量背景
	public UISprite luckyBackground;//抽奖次数背景
	public UISprite luckyFreeBackground;//抽奖免费背景
	public UISprite chatBackground;//聊天按钮背景
	public UISprite growupBackground;//成长计划数量按钮背景
	public UISprite mailNumBackground;
	public UISprite invitationBackground;
	public UISprite friendNumBackground;
	public UISprite announcementBackground; // 日常活动数量背景
	public UISprite xianshiBackground; // 限时活动数量背景
    public UISprite zhouNianQingBackground;//周年庆数量背景
	public UISprite magicWeaponBackground;// 可合成神器数量背景//
	public GameObject UI_CardTrainingNumBg; //卡牌训练可以位置数量背景
	//图片
	public UITexture userHead;

	/** 近景 */
	public GameObject bgFront;
	/** 中景 */
	public GameObject bgCenter;
	/** 远景 */
	public GameObject bgFar;
	/** 背景根节点 */
	public GameObject bgParent;

	/** 小精灵友情指引面板 */
	public GameObject objFriendlyGuide;
	/** 队伍按钮位置 */
	public Transform teamEditTransform;
	public UIDragScrollView maskDragSV;
	public UICenterOnChild centerOnChild;
	public UIScrollView launcher;
	public UIPanel launcherPanel;
	public barCtrl pveBar;
	public barCtrl pvestoreBar;
	public GameObject pvestoreEffect;
	public PvpPowerBar pvpBar;
	public barCtrl ExpBar;
	public barCtrl vipBar;
//	public int oneRmb_state = -1;//首冲情况: -1 不清楚需要获取    -2 首冲并且领奖了 <<--不显示图标    || 显示图标-->>    0 没有过首冲,1已经首冲没领奖,
	//public EffectCtrl  totalLoginEffect;
	public EffectCtrl  noticeEffect;
    public EffectCtrl zhouNianQingEffect;
	public EffectCtrl backButtonEffect;
	public EffectCtrl sevenDaysHappyEffect;
	public EffectCtrl xianshiEffect;
	public EffectCtrl  totalLoginEffect;
	public UIGrid guideGrid;//第三页排版
	public GameObject[] pageObjs;
	public static int sort = 0;
	private Timer timer;
	private Timer arenaTimer;
	/// <summary>
	/// 诸神战时间
	/// </summary>
	private Timer godsWartimer;
	int prayBeginTime = HoroscopesManager.Instance.getBeginTime ();
	int prayEndTime = HoroscopesManager.Instance.getEndTime ();
    bool isHave = false;
	public EffectCtrl oneRmbEffect;
	private float pageNum;//当前停留的页面
	/**英雄解锁进度类 */
	public MissionHeroPropessItem heroPropess;
	public ButtonLevelupReward levelupRewardButton;
	//static bool firstShowWeb = true;   //2014.7.9 modified
	public GameObject mysticalShopFlag;//神秘商店有刷新标志
//	public GameObject momoButton;
	public ButtonBase sdkButton;
	public string[] sdkIdsName;
	/** 公会战图标 */
	public UISprite guildFightTip;
    public GameObject tips;
    public GameObject godsAmni;
    public GameObject commandAmni;
    public Transform godsTranform;

	bool initLottery = true;

//	//分享图标控制
//	public void showShare ()
//	{
//		shareButton.gameObject.SetActive (false);
//		if (FriendsShareManagerment.Instance.getShareInfo () != null)
//			shareButton.gameObject.SetActive (true);
//		else if (FriendsShareManagerment.Instance.getPraiseNum () > 0 && FriendsShareManagerment.Instance.getPraiseInfo () != null)
//			shareButton.gameObject.SetActive (true);
//	}

	private void showTaskNum ()
	{
		if (TaskManagerment.Instance.getCompleteTaskNum () == 0) {
			taskBackground.gameObject.SetActive (TaskManagerment.Instance.getEveryDayTaskNoCompleteCount () != 0);
			taskedNum.text = "!";
		} else {
			taskBackground.gameObject.SetActive (true);
			taskedNum.text = TaskManagerment.Instance.getCompleteTaskNum ().ToString ();
		}
	}

	private void resetArenaInfo ()
	{
		//bugfix 窗口隐藏时候不要更新,会报错
		if (this == null || !gameObject.activeInHierarchy) {
			if (arenaTimer != null) {
				arenaTimer.stop ();
				arenaTimer = null;
			}
			return;
		}
		ArenaManager manager = ArenaManager.instance;
		if (manager.state >= ArenaManager.STATE_64_32 && manager.state <= ArenaManager.STATE_CHAMPION) {
			if (arenaTimer == null && manager.finalCD > 0) {
				arenaTimer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
				arenaTimer.addOnTimer (showArenaFinalInfo);
				arenaTimer.start ();
			}
		}
		showArenaNum ();
	}

    private void showArenaNumm()
    {
        int openLevel = GameConfig.Instance.getInt(GameConfig.SID_ARENA_OPEN_LEVEL);
        int guessNum = 0;
        if (UserManager.Instance.self.getUserLevel() >= openLevel) {
            guessNum = ArenaManager.computeGuessNumber();
            if (guessNum > 0) tips.SetActive(true);
            else
            {

                tips.SetActive(isMinShow());
            }
        }
    }

    private bool isMinShow()
    {
        for (int i = 0; i < 2; i++) {
            if (MiningManagement.Instance.minerals[i] != null) {
                if (MiningManagement.Instance.GetRemainTime(i) <= 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

	private void showArenaNum ()
	{
        if (tips.activeSelf) return;
        arenaBackground.gameObject.SetActive(false);
        tips.SetActive(isMinShow());
		arenaNum.text = "";
		int openLevel = GameConfig.Instance.getInt (GameConfig.SID_ARENA_OPEN_LEVEL);
		int guessNum = 0;
		if (UserManager.Instance.self.getUserLevel () >= openLevel) {
			guessNum = ArenaManager.computeGuessNumber ();
		}
		if (guessNum == 0) {
			if(UserManager.Instance.self.getUserLevel () >= 12){
				FPortManager.Instance.getFPort<ArenaMassFPort> ().access(()=>{
					guessNum += ArenaManager.instance.getChallengeCount();
					if(guessNum > 0){
						arenaBackground.gameObject.SetActive (true);
						arenaNum.text = "!";
					}
				});
			}
			if(UserManager.Instance.self.getUserLevel () >= 20){
				FPortManager.Instance.getFPort<LaddersGetInfoFPort> ().apply((isTrue)=>{
					guessNum += LaddersManagement.Instance.maxFightTime - LaddersManagement.Instance.currentChallengeTimes;
					if(guessNum > 0){
						arenaBackground.gameObject.SetActive (true);
						arenaNum.text = "!";
					}
				});
			}
		} else {
			arenaBackground.gameObject.SetActive (true);
			arenaNum.text = "!";
		}
	}


	//设置首冲状态
	private void showOneRmb ()
	{
		Recharge oneRmb = RechargeManagerment.Instance.getOneRmb ();
		//还没充值过或者异常
		if (oneRmb.num < 1) {
			if (UserManager.Instance.self.getUserLevel () >= 5) {//显示
				if (!oneRmbButton.gameObject.activeSelf)
					oneRmbButton.gameObject.SetActive (true);
			} else {
				if (oneRmbButton.gameObject.activeSelf)
					oneRmbButton.gameObject.SetActive (false);
			}
		} else {
			//已经领取过了
			if (oneRmb.count > 0 && !RechargeManagerment.Instance.canFirst) {
				if (oneRmbButton.gameObject.activeSelf)
					oneRmbButton.gameObject.SetActive (false);
			} else {
				if (UserManager.Instance.self.getUserLevel () >= 5) {//显示
					if (!oneRmbButton.gameObject.activeSelf)
						oneRmbButton.gameObject.SetActive (true);
					if (oneRmb.count > 0)
						oneRmbEffect.gameObject.SetActive (false);
					else if (!oneRmbEffect.gameObject.activeSelf) {
						oneRmbEffect.gameObject.SetActive (true);
					}
				} else {
					if (oneRmbButton.gameObject.activeSelf)
						oneRmbButton.gameObject.SetActive (false);
				}
			}
		}
	}

	public void updateDoubleRmb ()
	{
		if (oneRmbButton.gameObject.activeSelf)
			return;
		doubleRMBButton.gameObject.SetActive (DoubleRMBManagement.Instance.IsCanShow (NoticeType.DOUBLE_RMB_SID));
	}
	public void updateOneRmb()
	{
		if (RechargeManagerment.Instance.canFirst)
			oneRmbButton.gameObject.SetActive (true);
		else
			oneRmbButton.gameObject.SetActive (false);
	}

	/// <summary>
	/// 更新卡牌训练数量提示
	/// </summary>
	public void UpdateCardTrainingTips ()
	{
		int num = CardTrainingManagerment.Instance.getCanUseLocation ();
		UI_CardTrainingNumBg.SetActive (num > 0);
		UI_CardTrainingNum.text = num > 0 ? num.ToString () : "";
	}
    /// <summary>
    /// 更新天国宝藏开采完成提示
    /// </summary>
    public void updateArenaTips() {
        if (tips != null) {
            tips.SetActive(false);
            for (int i = 0; i < 2; i++) {
                if (MiningManagement.Instance.minerals[i] != null) {
                    if (MiningManagement.Instance.GetRemainTime(i) <= 0) {
                        tips.SetActive(true);
                        return;
                    }
                }
            }
        }
    }
	/// <summary>
	/// 更新聊天按钮好友聊天显示状态
	/// </summary>
	public void UpdateChatMsgTips ()
	{
		chatButton.setShowTips (ChatManagerment.Instance.IsHaveNewHaveMsg ());
	}
	public void showGuildInviateNum ()
	{
		Guild guild = GuildManagerment.Instance.getGuild ();
		if (guild != null) {
			List<GuildApprovalInfo> applyList = GuildManagerment.Instance.getGuildApprovalList ();
			/** 只有会长和副会长才能看到待审批提示 */
			if (guild != null && (guild.job == GuildJobType.JOB_PRESIDENT || guild.job == GuildJobType.JOB_VICE_PRESIDENT)) {
				if (applyList == null || applyList.Count == 0) {
					guildInviteTip.SetActive (false);
				} else {
					guildInviteTip.SetActive (true);
				}
			} else {
				guildInviteTip.SetActive (false);
			}
		} else {
			List<GuildRankInfo> inviteList = GuildManagerment.Instance.getGuildInviteList ();
			if (inviteList == null || inviteList.Count == 0) {
				guildInviteTip.SetActive (false);
			} else {
				guildInviteTip.SetActive (true);			
			}
		}
	}

	public void showHeroRoadNum ()
	{
		int count = HeroRoadManagerment.Instance.getCanBeChallengingTimes ();
		if (count > 0) {
			heroRoadCountLabel.text = count.ToString ();
			heroRoadCountLabel.transform.parent.gameObject.SetActive (true);
		} else {
			heroRoadCountLabel.transform.parent.gameObject.SetActive (false);
		}
	}

	public void showMailNum ()
	{
		if ((MailManagerment.Instance.getUnReadMailNum () + StorageManagerment.Instance.getAllTemp ().Count) > 0) {
			//mailNum.text = (MailManagerment.Instance.getUnReadMailNum () + StorageManagerment.Instance.getValidAllTemp (StorageManagerment.Instance.getAllTemp ()).Count).ToString ();
			mailNum.text = "!";
			mailNumBackground.gameObject.SetActive (true);
		} else {
			mailNum.text = "";
			mailNumBackground.gameObject.SetActive (false);		
		}
	}

	public void showFriendNum ()
	{
		if (FriendsManagerment.Instance.getRequestAmount () > 0 || FriendsManagerment.Instance.getCanReceiveGift ()) {
			friendNum.text = "!";//FriendsManagerment.Instance.getRequestAmount ().ToString ();
			friendNumBackground.gameObject.SetActive (true); 
		} else {
			friendNum.text = "";
			friendNumBackground.gameObject.SetActive (false); 
		}
	}

	public void showIncNum ()
	{
        commandAmni.SetActive(false);
        godsAmni.SetActive(false);
        DateTime dtTime = TimeKit.getDateTime(ServerTimeKit.getSecondTime());
        int nowWint = TimeKit.getWeekCHA(dtTime.DayOfWeek);
	    if (nowWint == 6 || nowWint == 7) commandAmni.SetActive(true);
	    else
	    {
	        godsAmni.SetActive(true);
            
	    }
		if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID)) {
			return;
		}
		int num = IncreaseManagerment.Instance.showSum ();
		float i = Mathf.Round (launcherPanel.clipOffset.x / 615);
		if (i == 1 && increaseButton.gameObject.activeSelf) {
			if (num > 0) {
				increaseButton.gameObject.SetActive (true);
                if (increaseButton.transform.localScale == Vector3.zero) {
                    TweenScale ts = TweenScale.Begin(increaseButton.gameObject, 0.2f, Vector3.one);
                    ts.method = UITweener.Method.EaseOut;
                }
				increaseButton.textLabel.text = "!";
                if (godsAmni.activeInHierarchy) godsTranform.localPosition = Vector3.zero;
			} else {
				increaseButton.transform.localScale = Vector3.zero;
				increaseButton.textLabel.text = "";
                increaseButton.gameObject.SetActive(false);
			}
			return;
		}
		if (i != 1) {
            increaseButton.transform.localScale = Vector3.zero;
            increaseButton.gameObject.SetActive(false);
            return;
		} else {
			if (num > 0) {
				increaseButton.transform.localScale = Vector3.zero;
				increaseButton.gameObject.SetActive (true);
				TweenScale ts = TweenScale.Begin (increaseButton.gameObject, 0.2f, Vector3.one);
				ts.method = UITweener.Method.EaseOut;
				increaseButton.textLabel.text = "!";
                if (godsAmni.activeInHierarchy) godsTranform.localPosition = Vector3.zero;
			} else {
				increaseButton.transform.localScale = Vector3.zero;
				increaseButton.textLabel.text = "";
                increaseButton.gameObject.SetActive(false);
			}
		}
	}

	private void showStoreNum ()
	{
		int count = StorageManagerment.Instance.getPropsCount (PropType.PROP_TYPE_CHEST, UserManager.Instance.self.getUserLevel ());
		if (count > 0) {
			storeBackground.gameObject.SetActive (true);
			storeNum.gameObject.SetActive (true);
			storeNum.text = count.ToString ();
		} else {
			storeBackground.gameObject.SetActive (false);
			storeNum.gameObject.SetActive (false);
		}
	}
	/// <summary>
	/// /显示是否有未领取的邀请奖励
	/// </summary>
	private void showUnusedInvitationAward ()
	{
		if (InviteCodeManagerment.Instance.hasInvitationAward ()) {
			invitationNum.gameObject.SetActive (true);
			invitationBackground.gameObject.SetActive (true);
			invitationNum.text = "!";
		} else {
			invitationNum.gameObject.SetActive (false);
			invitationBackground.gameObject.SetActive (false);
		}
	}

	private void showExchangeNum ()
	{
		int count = 0;
		for (int i = 0; i < 3; ++i)
			count += ExchangeManagerment.Instance.getExchangeCount (i, ExchangeType.COMMON);

		if (count > 0) {
			exchangeBackground.gameObject.SetActive (true);
			exchangeNum.gameObject.SetActive (true);
			exchangeNum.text = count.ToString ();
		} else {
			exchangeBackground.gameObject.SetActive (false);
			exchangeNum.gameObject.SetActive (false);
		}
	}
	
	private void showLuckyDrawNum ()
	{
		showLuckyDraw ();
		if (luckyFreeBackground.gameObject.activeSelf)
			return;
		int count = 0;
		count += LuckyDrawManagerment.Instance.getLuckyDrawBySid (81002).getShowCostNum () / LuckyDrawManagerment.Instance.getLuckyDrawBySid (81002).ways [0].getCostPrice (0);
		count += LuckyDrawManagerment.Instance.getLuckyDrawBySid (81001).getShowCostNum () / LuckyDrawManagerment.Instance.getLuckyDrawBySid (81001).ways [0].getCostPrice (0);
		if (StorageManagerment.Instance.checkProp (71006, 1)) {
			int divisor = LuckyDrawManagerment.Instance.getLuckyDrawBySid (81003).ways [0].getCostPrice (0);
			count += LuckyDrawManagerment.Instance.getLuckyDrawBySid (81003).getShowCostNum () / divisor;
		}
		if (StorageManagerment.Instance.checkProp (71005, 1)) {
			int divisor = LuckyDrawManagerment.Instance.getLuckyDrawBySid (81004).ways [0].getCostPrice (0);
			count += LuckyDrawManagerment.Instance.getLuckyDrawBySid (81004).getShowCostNum () / divisor;
		}
		if (count > 0) {
			luckyNum.text = count.ToString ();
			luckyNum.gameObject.SetActive (true);
			luckyBackground.gameObject.SetActive (true);
		} else {
			luckyNum.text = "";
			luckyBackground.gameObject.SetActive (false);
		}
	}

	int canGetWeaponCount = 0;
	List<Exchange> scrapList = null;
	// 显示可合成神器//
	private void showMagicWeaponNum()
	{
		scrapList = MagicWeaponScrapManagerment.Instance.getMagicWeaponScrapList() ;
		if(scrapList.Count >= 0)
		{
			canGetWeaponCount = MagicWeaponScrapManagerment.Instance.canGetMagicWeaponCount(scrapList);
		    if(canGetWeaponCount > 0)
			{
				magicWeaponNum.text = canGetWeaponCount + "";
				magicWeaponNum.gameObject.SetActive(true);
				magicWeaponBackground.gameObject.SetActive(true);
			}
			else
			{
				magicWeaponNum.text = "";
				magicWeaponNum.gameObject.SetActive(false);
				magicWeaponBackground.gameObject.SetActive(false);
			}
		}
	}

	public override void OnAwake ()
	{
		launcherPanel.clipOffset = new Vector2 (615, 0);
		launcherPanel.transform.localPosition = new Vector3 (-615, 0, 0);
		centerOnChild.onFinished = slideEvent;

		///初始化系统设置
		FPortManager.Instance.getFPort<SystemSettingsFport> ().GetInfo (null);
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		
	    sdkButton.gameObject.SetActive (false);

		setBgToggle (true);
		UiManager.Instance.mainWindow = this;
		UiManager.Instance.backGround.switchMainToDynamicBackground (launcher.GetComponent<UIPanel>(),"backGround_1");
        YaoQingButton.gameObject.SetActive(CommandConfigManager.Instance.yaoQingButtonFlag == 1);//邀请按钮开启关闭
		UpdateCardTrainingTips ();
	}
	public void updateLevelupRewardButton ()
	{
		int lastLevelupSid = LevelupRewardManagerment.Instance.lastRewardSid;
		if (lastLevelupSid > 0) {
			lastLevelupSid++;//to show next levelupReward
		}
		LevelupSample rewardSample = LevelupRewardSampleManager.Instance.getSampleBySid (lastLevelupSid);
		if (rewardSample == null||HeroGuideManager.Instance.checkHaveGuid()||HeroGuideManager.Instance.checkHaveExistGuidInMain()||!HeroGuideManager.Instance.isShowLevelAward()) {//the top level,no reward,disvisible
			levelupRewardButton.gameObject.SetActive (false);
		} else {
			if(rewardSample.showMinLevel<=UserManager.Instance.self.getUserLevel()&&rewardSample.showManLevel>UserManager.Instance.self.getUserLevel()){
				levelupRewardButton.gameObject.SetActive (false);
			}else {
				levelupRewardButton.gameObject.SetActive (true);
				levelupRewardButton.init (rewardSample.level, rewardSample.samples [0]);
			}
		}
	}
	/// <summary>
	/// 展示过期的激活任务
	/// </summary>
	public void heroOldShow(){
		HeroGuideSample hero=HeroGuideManager.Instance.getOldSampleInMain();
		if(hero==null)return;
		Card cardd=HeroGuideManager.Instance.getSuitCard(hero);
		heroPropess.gameObject.SetActive(true);
		heroPropess.updatePropess(false,cardd,hero.prizeSample[0].type,hero);
	}
	private Card getFistGoddess(int sid){
		List<BeastEvolve> list =BeastEvolveManagerment.Instance.getAllBest();//女神样本
		ArrayList beastList=StorageManagerment.Instance.getAllBeast();//已经有的女神
		bool flag=false;
		int flagNUm=0;
		if(beastList==null){
			return  CardManagerment.Instance.createCard (sid);
		}else{
			for(int i=0;i<list.Count;i++){
				flag=false;
				for(int j=0;j<beastList.Count;j++){
					flagNUm=i;
					if((list[i] as BeastEvolve).getBeast(0).getName()==(beastList[j] as Card).getName()){
						flag=true;
						break;
					}
				}
				if(!flag)return(list[flagNUm] as BeastEvolve).getBeast(0);
			}
			return  CardManagerment.Instance.createCard (sid);
		}
	}
	protected override void begin ()
	{ 
		base.begin ();
       // iTween.Stop();
		UiManager.Instance.clearWindows (this);
		//获取诸神战的当前状态
		if(CardManagerment.Instance.showChatEquips!=null)
			CardManagerment.Instance.showChatEquips.Clear ();
		toggleVipGrowup ();
		if (!GuideManager.Instance.showGuideNewFun ()) {
			GuideManager.Instance.guideEvent ();
		}
		if (UserManager.Instance.self.getUserLevel () >= 13) {
			buttonRank.gameObject.SetActive (true);//排行榜
		}
		if (GameManager.Instance.skipGuide || GuideManager.Instance.guideSid >= GuideGlobal.OVERSID) {
			buttonExchange.gameObject.SetActive (true);//兑换
			chatButton.gameObject.SetActive (true);//聊天
		}
		if (UserManager.Instance.self.getUserLevel () >= 6) {
			resolveButton.gameObject.SetActive (true);//分解
		}
        showXianshiButton();
		showBackButton();
        showZhouNianQingButton();
		showSevenDaysHappyBtn();
		showIco ();
		gridGuideIco ();
        GodsWarManagerment.Instance.getGodsWarStateInfo(() => { initFriendlyGuid(); });
		showPlayerInfo ();
		UpdateChatMsgTips ();
		UpdateCardTrainingTips ();
        updateArenaTips();
	    showArenaNumm();
		showMailNum ();
		showMagicWeaponNum();
		showTeamCombat ();
		showHeroRoadNum ();
		showFriendNum ();
		showLuckyDrawNum ();
		showTaskNum ();
		showUnusedInvitationAward ();
		UserManager.Instance.setSelfHeadIcon (userHead);
		showStoreNum ();
		showExchangeNum ();
		showGuildInviateNum ();
		nameLabel.text = UserManager.Instance.self.nickname;
		showIncNum ();
        updateIncOjb();
		updateMysticalShow();//是否显示神秘商店有新刷新物品
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (refreshData);
		timer.start ();
		arenaTimer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		arenaTimer.addOnTimer (showArenaFinalInfo);
		arenaTimer.start ();
		BattleGlobal.pvpMode = EnumPvp.nomal;//每次回到主界面都重置pvp模式
		// 启动监视跨天，刷新玩家数据
		if (GameManager.Instance.loginDay == 0)
			GameManager.Instance.loginDay = ServerTimeKit.getDayOfYear ();//设置登录时天数
		GameManager.Instance.timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		GameManager.Instance.timer.addOnTimer (refreshInfo);
		GameManager.Instance.timer.start ();
		showForeShow ();
		heroPropess.gameObject.SetActive(false);
		levelupRewardButton.gameObject.SetActive (false);
		if(HeroGuideManager.Instance.checkHaveExistGuidInMain()){
			heroOldShow();
		}else{
			if (GuideManager.Instance.isOverStep (GuideGlobal.NEWOVERSID)) {
				updateLevelupRewardButton();
			}
		}
		//累积登录奖励按钮特效
		totalPrize ();
		if(UserManager.Instance.self.getStorePvEPoint () == UserManager.Instance.self.getStorePvEPointMax ()&&UserManager.Instance.self.getStorePvEPoint()>0 && UserManager.Instance.pveStoreEffect != true){
			pvestoreEffect.SetActive(true);
		}else{
			pvestoreEffect.SetActive(false);
		}
		//MaskWindow.UnlockUI (true);
		if (GuideManager.Instance.isGuideComplete () && !GuideManager.Instance.isHaveGuide) {
			if (GameManager.Instance.isFirstLoginOpenBulletin && BulletinManager.Instance.getButtletinList () != null && BulletinManager.Instance.getButtletinList ().Count > 0) {
				UiManager.Instance.openDialogWindow<BulletinWindow> ((win)=>{
					if (UserManager.Instance.self.canDivine && UserManager.Instance.self.getUserLevel () >= 7) {
					win.dialogCloseUnlockUI = false;
					}
				});
			} else {
				if (UserManager.Instance.self.canDivine && UserManager.Instance.self.getUserLevel () >= 7) {
					UiManager.Instance.openDialogWindow<DivineWindow> ();
				} else {
					MaskWindow.UnlockUI (true);
				}
			}
		} else {
			MaskWindow.UnlockUI (true);
		}


		if (UiManager.Instance.ActiveLoadingWindow != null)
			UiManager.Instance.ActiveLoadingWindow.finishWindow ();

		// 是否在七日狂欢活动中//
		if(SevenDaysHappyManagement.Instance.getActiveMissonEndTime() - ServerTimeKit.getSecondTime() > 0)
		{
			SevenDaysHappyTouchTaskFPort fport = FPortManager.Instance.getFPort ("SevenDaysHappyTouchTaskFPort") as SevenDaysHappyTouchTaskFPort;		
			fport.SevenDaysHappyTouchTaskAccess ();
		}

		//2014.7.9 modified 
		/*
		if (AndroidSDKManager.Instance != null && !string.IsNullOrEmpty (androidWebURL) && GuideManager.Instance.isGuideComplete () && GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWOVERSID) && firstShowWeb) {

	
			#if UNITY_IPHONE 
			AndroidSDKManager.Instance.showWebPage (iosWebURL +"?pid="+AndroidSDKManager.Instance.plantformID+ "&sid=" + ServerManagerment.Instance.lastServer.sid);
			#else
			AndroidSDKManager.Instance.showWebPage (androidWebURL + "?pid=" + AndroidSDKManager.Instance.plantformID + "&sid=" + ServerManagerment.Instance.lastServer.sid);
			#endif
			firstShowWeb = false;
		}
        */
		ArmyManager.Instance.copyPveArmyTOPvp ();
		setBgPos ();
	}
    private void updateIncOjb() {
            if (IncreaseManagerment.Instance.showSum() <= 0 && GuideManager.Instance.isHaveNewFriendlyGuide() == 0) {
                increaseButton.gameObject.SetActive(false);
            } else {
                float i = Mathf.Round (launcherPanel.clipOffset.x / 615);
                if (i != 1) increaseButton.gameObject.SetActive(false);
                else
                {
                    commandAmni.SetActive(false);
                    godsAmni.SetActive(false);
                    DateTime dtTime = TimeKit.getDateTime(ServerTimeKit.getSecondTime());
                    int nowWint = TimeKit.getWeekCHA(dtTime.DayOfWeek);
                    if (nowWint == 6 || nowWint == 7) commandAmni.SetActive(true);
                    else
                    {
                        godsAmni.SetActive(true);
                        godsTranform.localPosition = Vector3.zero;
                    }
                    increaseButton.gameObject.SetActive(true);
                    if (godsAmni.activeInHierarchy) godsTranform.localPosition = Vector3.zero;
                }
            } 
        
    }
	private void totalPrize ()
	{
		//累积登录奖励按钮特效
		totalPrizeButton.gameObject.SetActive (TotalLoginManagerment.Instance.isShowPrize ());
 
	}
    /// <summary>
    /// 判断是否有限时活动，若果没有则隐藏按钮
    /// </summary>
    private void showXianshiButton()
    {
        List<Notice> array = NoticeManagerment.Instance.getValidNoticeList(NoticeEntranceType.LIMIT_NOTICE);
        if(array == null || array.Count < 1)
            xianshiButton.gameObject.SetActive(false);
        else
            xianshiButton.gameObject.SetActive(true);
    }
    private void showZhouNianQingButton() {		

        List<Notice> array = NoticeManagerment.Instance.getValidNoticeList(NoticeEntranceType.ZHOUNIANQING_NOTICE);
        if (array == null || array.Count < 1)
            zhouNianQingButton.gameObject.SetActive(false);
        else
            zhouNianQingButton.gameObject.SetActive(true);
    }
	private void showBackButton()
	{
		List<Notice> array = NoticeManagerment.Instance.getValidNoticeList(NoticeEntranceType.BACK_NOTICE);
		if(array == null || array.Count < 1)
		{
			backButton.gameObject.SetActive(false);
			backButtonNum.gameObject.SetActive(false);
			backButtonEffect.gameObject.SetActive(false);
		}
		else
		{
			backButton.gameObject.SetActive(true);
			if(BackPrizeLoginInfo.Instance.loginDays <= BackPrizeInfoFPort.tottalLoginDays)
			{
				if(BackPrizeLoginInfo.Instance.loginDays - BackPrizeLoginInfo.Instance.receivedDays.Count > 0 || BackPrizeRechargeInfo.Instance.getCanRecevieCount() - BackPrizeRechargeInfo.Instance.getReceviedCount() > 0)
				{
					backButtonNum.gameObject.SetActive(true);
					backButtonEffect.gameObject.SetActive(true);;
				}
				else
				{
					backButtonNum.gameObject.SetActive(false);
					backButtonEffect.gameObject.SetActive(false);
				}
			}
			else
			{
				if(BackPrizeInfoFPort.tottalLoginDays - BackPrizeLoginInfo.Instance.receivedDays.Count > 0 || BackPrizeRechargeInfo.Instance.getCanRecevieCount() - BackPrizeRechargeInfo.Instance.getReceviedCount() > 0)
				{
					backButtonNum.gameObject.SetActive(true);
					backButtonEffect.gameObject.SetActive(true);
				}
				else
				{
					backButtonNum.gameObject.SetActive(false);
					backButtonEffect.gameObject.SetActive(false);;
				}
			}
		}
	}
	private void showSevenDaysHappyBtn()
	{
		if(ServerTimeKit.getSecondTime() < SevenDaysHappyManagement.Instance.getEndTime())
		{
			sevenDaysHappyBtn.gameObject.SetActive(true);
			if(SevenDaysHappyManagement.Instance.canReceviedCount > 0)
			{
				sevenDaysHappyNum.gameObject.SetActive(true);
				sevenDaysHappyEffect.gameObject.SetActive(true);
			}
			else
			{
				sevenDaysHappyNum.gameObject.SetActive(false);
				sevenDaysHappyEffect.gameObject.SetActive(false);
			}
		}
		else
		{
			sevenDaysHappyBtn.gameObject.SetActive(false);
		}
	}
	//显示预告
	private void showForeShow ()
	{
		for (int i = 0; i < ForeShowConfigManager.Instance.levels.Count; i++) {
			if (UserManager.Instance.self.getUserLevel () < ForeShowConfigManager.Instance.levels [i]) {
				ForeShowConfigManager.Instance.index = i;
				foreShowPrompt.text = ForeShowConfigManager.Instance.prompt [i];
				foreShowButton.textLabel.text = ForeShowConfigManager.Instance.names [i];
				foreShowPrompt.gameObject.SetActive (true);
				foreShowButton.gameObject.SetActive (true);
				return;
			}
		}
	}

	private void showArenaFinalInfo ()
	{
		if (this == null || !gameObject.activeInHierarchy) {
			if (arenaTimer != null) {
				arenaTimer.stop ();
				arenaTimer = null;
			}
			return;
		}
		ArenaManager manager = ArenaManager.instance;
		if (!manager.isStateCorrect ()) { // 下次定时器再进行竞猜次数的更新
			FPortManager.Instance.getFPort<ArenaGetStateFPort> ().access (null);
			return;
		}
		if (manager.state >= ArenaManager.STATE_64_32 && manager.state <= ArenaManager.STATE_CHAMPION) {
			if (manager.finalCD > 0) {
				manager.finalCD--;
			} else {
				if (arenaTimer != null) {
					arenaTimer.stop ();
					arenaTimer = null;
					FPortManager.Instance.getFPort<ArenaFinalFPort> ().access (resetArenaInfo);
				}
			}
		}
	}
	
	private void showPlayerInfo ()
	{
		moneyLabel.text = UserManager.Instance.self.getMoney ().ToString ();
		rmbLabel.text = UserManager.Instance.self.getRMB ().ToString ();
		levelLabel.text = UserManager.Instance.self.getUserLevel ().ToString ();

		updatePve ();
		updatePvp ();
		
 
		ExpBar.updateValue (UserManager.Instance.self.getLevelExp (), UserManager.Instance.self.getLevelAllExp ());
		vipBar.updateValue (UserManager.Instance.self.getVipEXP (), UserManager.Instance.self.getVipEXPUp ());
		vipLevel.text = UserManager.Instance.self.vipLevel.ToString ();
		showNotice ();
		showOneRmb ();
		showTotalLoginEffect ();
		updateDoubleRmb ();

	}

	void refreshData ()
	{
		//若主界面看不到，不做如下的更新
		if (this == null || !gameObject.activeInHierarchy) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			return;
		}
		updateMysticalShow();
		updatePve ();
		updatePvp ();
		showNotice ();
		showLuckyDraw ();
		updateGuildFightShow ();
		//玩家没动静就提示他点前进
		if (timer != null && timer.currentCount % 5 == 0 && timer.currentCount > 2 && !MaskWindow.instance.maskUI.activeSelf && GuideManager.Instance.isOverStep (123001000)
			&& intoFubenButton.transform.parent.transform.localPosition.y == -411 && GuideManager.onType == 0) {
			showMoveEffect ();
		}

	}
	void updateMysticalShow(){
		if(MysticalShopConfigManager.Instance.isCanShowFlag("main")||GoodsBuyCountManager.Instance.isCanShowFlag("showte"))mysticalShopFlag.SetActive(true);
		else mysticalShopFlag.SetActive(false);
	}
	void showLuckyDraw ()
	{
		if (LuckyDrawManagerment.Instance.HasFree ()) {
			luckyNum.gameObject.SetActive (false);
			luckyBackground.gameObject.SetActive (false);
			luckyFreeBackground.gameObject.SetActive (true);
		} else
			luckyFreeBackground.gameObject.SetActive (false);
	}

	void updateGuildFightShow(){
		
		if (GuildFightSampleManager.Instance ().isShowGuildFightFlag ()) {
			guildFightTip.gameObject.SetActive(true);
		} else {
			guildFightTip.gameObject.SetActive(false);
		}
	}

	void updatePvp ()
	{
		pvpBar.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
		if (UserManager.Instance.self.isPvpMax ()) {
			pvpTimeLabel.gameObject.SetActive (false);
		} else {
			pvpTimeLabel.gameObject.SetActive (true);
			pvpTimeLabel.text = UserManager.Instance.getNextPvpTime ().Substring (3);
		}

	}

	void updatePve ()
	{
        //bool flag = UserManager.Instance.self.getStorePvEPoint () > 0;
        pveBar.gameObject.SetActive (true);
        //pvestoreBar.gameObject.SetActive (flag);
        //if (flag) {
        //    pvestoreBar.updateValue (UserManager.Instance.self.getStorePvEPoint (), UserManager.Instance.self.getStorePvEPointMax ());
        //    mountsPveValue.text = UserManager.Instance.self.getStorePvEPoint () + "/" + UserManager.Instance.self.getStorePvEPointMax ();
        //    if (!UserManager.Instance.self.isPveMax () || UserManager.Instance.self.isStorePveMax ()) {
        //        mountsPveTimeLabel.gameObject.SetActive (false);
        //    } else {
        //        mountsPveTimeLabel.gameObject.SetActive (true);
        //        mountsPveTimeLabel.text = UserManager.Instance.getNextMountsPveTime ().Substring (3);
        //    }
        //} else {
			pveBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
            pveValue.text = UserManager.Instance.self.getPvEPoint() + UserManager.Instance.self.getStorePvEPoint() + "/" + UserManager.Instance.self.getPvEPointMax();
			if (UserManager.Instance.self.isPveMax ()) {
				pveTimeLabel.gameObject.SetActive (false);
			} else {
				pveTimeLabel.gameObject.SetActive (true);
				pveTimeLabel.text = UserManager.Instance.getNextPveTime ().Substring (3);
			}
		//}


	}

	public void showGuideGoToFuBenEffect ()
	{
		StartCoroutine (Utils.DelayRun (() => {
			showMoveEffect ();
		}, 1.5f));
	}
	
	void showMoveEffect ()
	{
		EffectCtrl a = EffectManager.Instance.CreateEffect (intoFubenButton.transform, "Effect/UiEffect/feature_open");

		if (a == null)
			return;

		Object audioObj = a.gameObject.GetComponent ("AudioPlayer");
		if (audioObj != null) {
			Destroy (audioObj);
		}
		StartCoroutine (Utils.DelayRun (() => {
			Destroy (a.gameObject, 2f);
		}, 1.5f));
	}

	private void showTotalLoginEffect ()
	{
		int number = TotalLoginManagerment.Instance.getActiveAwardNum ();
		if (number > 0) {
			totalLoginEffect.gameObject.SetActive (true);
		} else {
			totalLoginEffect.gameObject.SetActive (false);
		}
	}
	/// <summary>
	/// 获取并限时活动提示数字
	/// </summary>
	private void showNotice ()
	{
		int now = ServerTimeKit.getSecondTime ();
		int current = ServerTimeKit.getCurrentSecond ();
		int[] info = NoticeManagerment.Instance.getHeroEatInfo ();
		int count = 0;
		int xianshiCount = 0;
        int zhouNianQing = 0;
		count += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.TOPUPNOTICE,NoticeEntranceType.DAILY_NOTICE);
		count += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.TIME_RECHARGE,NoticeEntranceType.DAILY_NOTICE);
		count += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.NEW_RECHARGE,NoticeEntranceType.DAILY_NOTICE);
		count += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.COSTNOTICE,NoticeEntranceType.DAILY_NOTICE);
		count += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.NEW_CONSUME,NoticeEntranceType.DAILY_NOTICE);
		count += NoticeManagerment.Instance.getCanReceiveExchangeByTime (now, NoticeType.EXCHANGENOTICE,NoticeEntranceType.DAILY_NOTICE);

		xianshiCount += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.TOPUPNOTICE,NoticeEntranceType.LIMIT_NOTICE);
		xianshiCount += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.TIME_RECHARGE,NoticeEntranceType.LIMIT_NOTICE);
		xianshiCount += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.NEW_RECHARGE,NoticeEntranceType.LIMIT_NOTICE);
		xianshiCount += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.COSTNOTICE,NoticeEntranceType.LIMIT_NOTICE);
		xianshiCount += NoticeManagerment.Instance.getCanReceiveRechargeByTime (now, NoticeType.NEW_CONSUME,NoticeEntranceType.LIMIT_NOTICE);
		xianshiCount += NoticeManagerment.Instance.getCanReceiveExchangeByTime (now, NoticeType.EXCHANGENOTICE,NoticeEntranceType.LIMIT_NOTICE);

		Notice noticeTemp;
		//答题活动 是否开启
		noticeTemp = NoticeManagerment.Instance.getNoticeByType (NoticeType.QUIZ_EXAM);
		if (noticeTemp != null && (noticeTemp as QuizNotice).isCanAnswer ()) {
			NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeTemp.sid);
			if(tmp.entranceId == NoticeEntranceType.DAILY_NOTICE)
				count++;
			else if(tmp.entranceId == NoticeEntranceType.LIMIT_NOTICE)
				xianshiCount++;
		}

		//问卷调查 是否开启
		noticeTemp = NoticeManagerment.Instance.getNoticeByType (NoticeType.QUIZ_SURVEY);
		if (noticeTemp != null && (noticeTemp as QuizNotice).isCanAnswer ()) {
			NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeTemp.sid);
			if(tmp.entranceId == NoticeEntranceType.DAILY_NOTICE)
				count++;
			else if(tmp.entranceId == NoticeEntranceType.LIMIT_NOTICE)
				xianshiCount++;
		}
		if(NoticeManagerment.Instance.turnSpriteData!=null)
		{
			if (UserManager.Instance.self.getUserLevel () >= 25 && NoticeManagerment.Instance.turnSpriteData.num > 0)
				count += NoticeManagerment.Instance.turnSpriteData.num;
		}
		if (NoticeManagerment.Instance.xs_turnSpriteData!=null)
		{
            if (UserManager.Instance.self.getUserLevel() >= CommandConfigManager.Instance.getLimitLevel()) {
                if (NoticeManagerment.Instance.xs_turnSpriteData.num > 0)
                    xianshiCount += NoticeManagerment.Instance.xs_turnSpriteData.num;
            }
			
		}
		if (SuperDrawManagerment.Instance.superDraw==null)
			NoticeManagerment.Instance.getSuperDrawInfo();
		else
		{
			if(SuperDrawManagerment.Instance.superDraw.canUseNum>0)
				xianshiCount += SuperDrawManagerment.Instance.superDraw.canUseNum;
		}
		if (!(NoticeManagerment.Instance.getAlchemyConsume () > 0))
			count++;
		if (info [1] < now && now < info [2] && info [3] == 0)
			count++;
		if (NoticeManagerment.Instance.getMonthCardRewardState () == NoticeManagerment.MONTHCARD_STATE_VALID)
			count++;
//		if (RechargeManagerment.Instance.getOneRmbState () == RechargeManagerment.ONERMB_STATE_VALID)
//			count++;

        noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.LIMIT_COLLECT);
        if (noticeTemp != null)
        {
            NoticeSample noticeSample = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeTemp.sid);
            foreach (int sid in (noticeSample.content as SidNoticeContent).sids)
            {
                LimitCollectSample  collectSample = NoticeActiveManagerment.Instance.getActiveInfoBySid(sid) as LimitCollectSample;
                if ( collectSample != null && collectSample.isCanReceive())
                {
                    xianshiCount++;
                }
            }
        }
        //周年庆入口显示特效、次数
        noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.SIGNIN);
        if (noticeTemp != null && noticeTemp.isValid()) { 
        SignInSample sample = SignInSampleManager.Instance.getSignInSampleBySid(StringKit.toInt(noticeTemp.sid + "" + ServerTimeKit.getCurrentMonth()));
        NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeTemp.sid);
            if(sample == null) return;
            List<int> sids = sample.daySids;
            if (!isHave && noticeTemp.isValid()) {
                GetSignInInfoFport fport = FPortManager.Instance.getFPort("GetSignInInfoFport") as GetSignInInfoFport;
                fport.getSignInInfo(null);
                isHave = true;
            }
            if (!SignInManagerment.Instance.stateList.Contains(sids[ServerTimeKit.getDayOfMonth() - 1])) {
                if (tmp.entranceId == NoticeEntranceType.DAILY_NOTICE)
                    count++;
                else 
                    zhouNianQing++;
            }
        }
        noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.SHAREDRAW);
        if (noticeTemp != null && noticeTemp.isValid()) {
            NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeTemp.sid);
            if (ShareDrawManagerment.Instance.isFirstShare == 0) {
                if (tmp.entranceId == NoticeEntranceType.LIMIT_NOTICE)
                    xianshiCount++;
                else 
                    zhouNianQing++;
            }
            if (ShareDrawManagerment.Instance.canDrawTimes != 0) {
                if(tmp.entranceId == NoticeEntranceType.LIMIT_NOTICE)
                    xianshiCount += ShareDrawManagerment.Instance.canDrawTimes;
                else 
                    zhouNianQing += ShareDrawManagerment.Instance.canDrawTimes;
            }
        }
		ArrayList dailyList = TaskManagerment.Instance.getDailyRebateTask();
		for(int i=0;i<dailyList.Count;i++)
		{
			if(TaskManagerment.Instance.isComplete(dailyList[i] as Task))
				count ++;
		}

		// 周卡是否可领取//
        noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.WEEKCARD);
        if (noticeTemp != null) {
            NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeTemp.sid);
            if (WeekCardInfo.Instance.recevieState == WeekCardRecevieState.recevie) {
                if (tmp.entranceId == NoticeEntranceType.DAILY_NOTICE) 
                    count++;
                else 
                    zhouNianQing++;
            }
        }
		// 福袋可领取个数//
        noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.XIANSHI_FANLI);
        if (noticeTemp != null) {
            NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeTemp.sid);
            if (RebateInfoManagement.Instance.canRecevieCount > 0) {
                if(tmp.entranceId == NoticeEntranceType.LIMIT_NOTICE)
                    xianshiCount += RebateInfoManagement.Instance.canRecevieCount;
                else 
                    zhouNianQing += RebateInfoManagement.Instance.canRecevieCount;
            }
        }

		// 大乐透//
		noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.LOTTERY);
		if(noticeTemp != null && noticeTemp.isValid() && initLottery)
		{
			initLottery = false;
			LotteryInfoFPort fPort = FPortManager.Instance.getFPort("LotteryInfoFPort") as LotteryInfoFPort;
			fPort.lotteryInfoAccess(null);
		}
		if((noticeTemp as LotteryNotice).isActivityOpen())
		{
			count += LotteryManagement.Instance.getLotteryCount();
		}
		count += LotteryManagement.Instance.selectedAwardCount;

		if (count > 0) {
			announcementNum.text = count.ToString ();
			announcementNum.gameObject.SetActive (true);
			announcementBackground.gameObject.SetActive (true);
			noticeEffect.gameObject.SetActive (true);
		} else {
			announcementNum.gameObject.SetActive (false);
			announcementBackground.gameObject.SetActive (false);
			noticeEffect.gameObject.SetActive (false);
		}

		if (xianshiCount > 0) {
			xianshiNum.text = xianshiCount.ToString ();
			xianshiNum.gameObject.SetActive (true);
			xianshiBackground.gameObject.SetActive (true);
			xianshiEffect.gameObject.SetActive (true);
		}
		else {
            xianshiNum.gameObject.SetActive(false);
            xianshiBackground.gameObject.SetActive(false);
			xianshiEffect.gameObject.SetActive (false);
		}
        if (zhouNianQing > 0) {
            zhouNianQingNum.text = zhouNianQing.ToString();
            zhouNianQingNum.gameObject.SetActive(true);
            zhouNianQingBackground.gameObject.SetActive(true);
            zhouNianQingEffect.gameObject.SetActive(true);
        } else {
            zhouNianQingNum.gameObject.SetActive(false);
            zhouNianQingBackground.gameObject.SetActive(false);
            zhouNianQingEffect.gameObject.SetActive(false);
        }

	}

	void ccc (Transform  obj, Transform target)
	{ 
		Matrix4x4 matri = new Matrix4x4 ();
		Vector3 pos = target.localPosition;
		
		matri = target.parent.localToWorldMatrix;
		pos = matri.MultiplyPoint3x4 (pos);  
		
		matri = UiManager.Instance.UIScaleRoot .transform.worldToLocalMatrix;
		obj.parent = UiManager.Instance.UIScaleRoot.transform;
		obj.localPosition = matri.MultiplyPoint3x4 (pos);   
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{   
		base.buttonEventBase (gameObj);

		if (gameObj.name == "fuben") {
			//强制引导期间，没有在正确的步骤是不能进副本的
			if (GuideManager.Instance.isLessThanStep (50001000) && GuideManager.Instance.isDoesNotEqualStep (9002000) &&
				GuideManager.Instance.isDoesNotEqualStep (13002000) && GuideManager.Instance.isDoesNotEqualStep (20002000) &&
				GuideManager.Instance.isDoesNotEqualStep (30001000)) {
				MaskWindow.UnlockUI ();
				MonoBase.print ("I can't into fuben,guide=" + GuideManager.Instance.guideSid);
				return;
			}
			//按需要执行步骤
			if (!GuideManager.Instance.isGuideComplete ()) {
				if (GuideManager.Instance.isEqualStep (9002000) || GuideManager.Instance.isEqualStep (13002000) || GuideManager.Instance.isEqualStep (20002000)
					|| GuideManager.Instance.isEqualStep (103002000) || GuideManager.Instance.isEqualStep (126002000) || GuideManager.Instance.isEqualStep (120002000)
					|| GuideManager.Instance.isEqualStep (30001000)||GuideManager.Instance.isEqualStep(140002000)) {
					GuideManager.Instance.doGuide ();
				} else {
					MonoBase.print ("isGuideComplete,I can't into fuben,guide=" + GuideManager.Instance.guideSid);
				}
			}
			//指定步骤黑屏预告
			if (GuideManager.Instance.guideSid == 9003000) {
				MaskWindow.LockUI ();
				string desc = ChapterSampleManager.Instance.getChapterSampleBySid (FuBenManagerment.getAllShowStoryChapter (1) [0]).name;
				UiManager.Instance.openDialogWindow<NewChapterWindow> ((win) => {
					win.initWin (LanguageConfigManager.Instance.getLanguage ("NewChapter01", "1"), desc, () => {
						FuBenManagerment.Instance.inToFuben ();
					});
				});
				return;
			}
			FuBenManagerment.Instance.inToFuben ();
		} else if (gameObj.name == "teamEdit") {
			if (!GuideManager.Instance.isGuideComplete ()) {
				ArmyManager.Instance.cleanAllEditArmy ();
			}
			GuideManager.Instance.doGuide ();
            TeamEmtpyInfoFPort fport = FPortManager.Instance.getFPort<TeamEmtpyInfoFPort>();
            fport.access(openTeamEmtpyWindow);
		} else if (gameObj.name == "06_heroEdit") {
            if (GuideManager.Instance.doingEvoLution) {
                UiManager.Instance.openWindow<CardStoreWindow>((win) => {
                    win.setIntoTyoe(true);
                });
            } else {
                UiManager.Instance.openWindow<CardStoreWindow>();
                GuideManager.Instance.doFriendlyGuideEvent();
            }
		} else if (gameObj.name == "beastEdit") { 
			GuideManager.Instance.doGuide ();
			UiManager.Instance.openWindow<GoddessWindow> ();
		} else if (gameObj.name == "arena") { 
			UiManager.Instance.openWindow<ArenaNavigateWindow> ();
		} else if (gameObj.name == "shopButton") {  
			UiManager.Instance.openWindow<ShopListWindow> ();
			mysticalShopFlag.SetActive(false);
		} else if (gameObj.name == "luckyButton") { 
			GuideManager.Instance.doGuide ();
			UiManager.Instance.openWindow<LuckyDrawWindow> ();
		} else if (gameObj.name == "totalLoginButton") {
			UiManager.Instance.openWindow<TotalLoginWindow> ((win) => {
				win.Initialize ();
			});
		} else if (gameObj.name == "taskButton") {
			UiManager.Instance.openWindow<TaskWindow> ((win) => {
				if (TaskManagerment.Instance.getMainLineTaskedNum () > 0) {
					win.initTap (1);
				} else {
					win.initTap (0);
				}
			});
		
		} else if (gameObj.name == "buttonAnnouncement") {
			if (GuideManager.Instance.isEqualStep (127002000)) {
				GuideManager.Instance.doGuide ();
				UiManager.Instance.openWindow<NoticeWindow> ((win) => {
					win.updateSelectButton (NoticeType.GODDNESS_SHAKE_SID);
					win.entranceId = NoticeEntranceType.DAILY_NOTICE;
				});
			} else {
				UiManager.Instance.openWindow<NoticeWindow> ((win)=>{
					win.entranceId = NoticeEntranceType.DAILY_NOTICE;
				});
			}
		} else if (gameObj.name == "pveBar" || gameObj.name == "pvestoreBar") {
			if(pvestoreEffect.activeSelf){
				pvestoreEffect.SetActive(false);
				UserManager.Instance.pveStoreEffect = true;
			}
			UiManager.Instance.openDialogWindow<LinePowerShowWindow> ();
		}
        else if (gameObj.name == "addPvePoint")
        {
            UiManager.Instance.openDialogWindow<PveUseWindow>();
        }
        else if (gameObj.name == "foreShowButton") {
			UiManager.Instance.createMessageWindowByOneButton (ForeShowConfigManager.Instance.getContents (), null);
		} else if (gameObj.name == "chatButton") {
			UiManager.Instance.openWindow<ChatWindow> ((win) => {
				win.initChatWindow (ChatManagerment.Instance.sendType - 1);
			});
			if (sort > ChatManagerment.Instance.getAllChat ().Count) {
				++sort;
			} else {
				sort = ChatManagerment.Instance.getAllChat ().Count;
			}
		} else if (gameObj.name == "vipBar") {
			UiManager.Instance.openWindow<VipWindow> ((win) => {
				win.updateInfo ();
			});
		} else if (gameObj.name == "oneRmbButton") {
			GuideManager.Instance.doGuide ();
			UiManager.Instance.openWindow<NoticeWindow> ((win) => {
				NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.ONERMB_SID);
				win.entranceId = tmp.entranceId;
				win.updateSelectButton (NoticeType.ONERMB_SID);//首冲条目写死
			});
		} else if (gameObj.name == doubleRMBButton.name) {
			UiManager.Instance.openWindow<NoticeWindow> ((win) => {
				NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.ONERMB_SID);
				win.entranceId = tmp.entranceId;
				win.updateSelectButton (NoticeType.DOUBLE_RMB_SID);
			});
		} else if (gameObj.name == "10_buttonMail") {
			UiManager.Instance.openWindow<MailWindow> ((win) => {
				win.Initialize (0);
			});
		} else if (gameObj.name == "xianshiButton") {
			UiManager.Instance.openWindow<NoticeWindow> ((win)=>{
				win.entranceId = NoticeEntranceType.LIMIT_NOTICE;
			});
        } else if (gameObj.name == "zhouNianQingButton") {
            UiManager.Instance.openWindow<NoticeWindow>((win) => {
                win.entranceId = NoticeEntranceType.ZHOUNIANQING_NOTICE;
            });
        } else if (gameObj.name == "01_buttonStore") {
            GuideManager.Instance.doGuide();
            if (StarSoulManager.Instance.getStarSoulInfo() == null) {
                // 与服务器通讯
                (FPortManager.Instance.getFPort("StarSoulFPort") as StarSoulFPort).getStarSoulInfoAccess(() => {
                    UiManager.Instance.openWindow<StoreWindow>();
                });
            } else {
                UiManager.Instance.openWindow<StoreWindow>();
            }
        } else if (gameObj.name == "01_cmagicweapon") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openWindow<MagicWeaponStoreWindow>();
        } else if (gameObj.name == "02_buttonExchange") {
            if (GuideManager.Instance.isEqualStep(106003000)) {
                GuideManager.Instance.doGuide();
            }
            UiManager.Instance.openWindow<ExChangeWindow>();
        } else if (gameObj.name == "07_buttoninSell") {
            UiManager.Instance.openWindow<SellWindow>((win) => {
                win.Initialize();
            });
        } else if (gameObj.name == "08_buttoninResolve") {
            UiManager.Instance.openWindow<ResolveWindow>((win) => {
                win.Initialize();
            });
        } else if (gameObj.name == "04_buttoninvitation") {
            UiManager.Instance.openWindow<InviteCodeWindow>();
        } else if (gameObj.name == "05_buttonFriend") {
            UiManager.Instance.openWindow<FriendsWindow>((win) => {
                win.initWin(0);
            });
        } else if (gameObj.name == "11_buttonSetting") {
            UiManager.Instance.openDialogWindow<SystemSettingsWindow>();
        } else if (gameObj.name == "buttonUnion") {
            GuildFightSampleManager.Instance().saveGuildFigthFlag();
            guildFightTip.gameObject.SetActive(false);
            GuideManager.Instance.doGuide();
            GuildGetInfoFPort fport = FPortManager.Instance.getFPort("GuildGetInfoFPort") as GuildGetInfoFPort;
            fport.access(openGuildWindow);
        } else if (gameObj.name == "button1_HeroRoad") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openWindow<HeroRoadWindow>();
        } else if (gameObj.name == "button3_mainCard") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openWindow<MainCardEvolutionWindow>();
        } else if (gameObj.name == "button4_StarSoul") {
            GuideManager.Instance.doGuide();
            MaskWindow.LockUI(true);
            UiManager.Instance.openWindow<StarSoulWindow>((win) => {
                win.init(1);
            });
        } else if (gameObj.name == "button6_Mounts") {
            UiManager.Instance.openWindow<MountsWindow>((win) => {
                win.init(1);
            });
        } else if (gameObj.name == "button5_Training") {
            UiManager.Instance.openWindow<CardTrainingWindow>();
        } else if (gameObj.name == "button2_Honor") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openWindow<HonorWindow>((win) => {
                win.updateInfo();
            });
        } else if (gameObj.name == "IncreaseButton") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openDialogWindow<IncreaseWayWindow>();
        } else if (gameObj.name == "03_buttonPicture") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openWindow<PictureWindow>();
        } else if (gameObj.name == "rmbNode") {
            UiManager.Instance.openWindow<rechargeWindow>();
        } else if (gameObj.name == "headNode") {
            UiManager.Instance.openDialogWindow<PlayerInfoWindow>((win) => {
                win.showUI();
            });
        } else if (gameObj.name == "09_buttonRank") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openWindow<RankWindow>();

        } else if (gameObj.name == "button7_GoddessAstrolabe") {
            GuideManager.Instance.doGuide();
            UiManager.Instance.openWindow<GoddessAstrolabeMainWindow>();
        }else if (gameObj.name == "buttonToSee") {//友情指引的去看看
			objFriendlyGuide.SetActive (false);
			GuideManager.Instance.setOnType (friendlyGuideType);
			GuideManager.Instance.doFriendlyGuideEvent ();
            StartCoroutine(Utils.DelayRun(() => {
                MaskWindow.UnlockUI();
            }, 0.7f));
            //MaskWindow.UnlockUI ();
		} else if (gameObj.name == "buttonNotToSee") {//友情指引的放弃指引
			objFriendlyGuide.SetActive (false);
			GuideManager.Instance.setOnType (friendlyGuideType);
			GuideManager.Instance.withoutFriendlyGuide ();
			MaskWindow.UnlockUI ();
		} else if (gameObj.name == "buttonGrowup") {
			//如果没有投资过，显示投资界面
			if (GrowupAwardMangement.Instance.prestoreMoney == 0) {
				UiManager.Instance.openWindow<GrowupInvestWindow> ();
			} else {
				UiManager.Instance.openWindow<GrowupAwardWindow> ();
			}
			//否则显示领取
		} else if (gameObj.name == "13_buttonBulletin") {
			if (BulletinManager.Instance.getButtletinList () != null && BulletinManager.Instance.getButtletinList ().Count > 0) {
				UiManager.Instance.openDialogWindow<BulletinWindow> ();
			} else {
				TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("Bulletin_err01"));
				MaskWindow.UnlockUI ();
			}
		}else if(gameObj.name=="heroShowPoint"){
			HeroGuideSample heroGuidee=null;
			if(HeroGuideManager.Instance.checkHaveExistGuidInMain()){
				heroGuidee=HeroGuideManager.Instance.getOldSampleInMain();
			}
			if(heroGuidee!=null){
				if(heroGuidee.prizeSample[0].type==5){
					CardBookWindow.Show (CardManagerment.Instance.createCard(heroGuidee.prizeSample[0].pSid), CardBookWindow.SHOW, null);
				}else if(heroGuidee.prizeSample[0].type==6){
					UiManager.Instance.openWindow<BeastAttrWindow>((win)=>{
						win.Initialize(getFistGoddess(heroGuidee.prizeSample[0].pSid),4);
					});
				}else if(heroGuidee.prizeSample[0].type==7){
					UiManager.Instance.openDialogWindow<MainCardSurmountShowWindow>((win)=>{
						win.init(StringKit.toInt(heroGuidee.prizeSample[0].num)+3);
					});
					
				}

            } else {
                MaskWindow.UnlockUI();
            }
        } else if (gameObj.name == "levelupAwardButton") {
            UiManager.Instance.openDialogWindow<LevelupRewardWindow>((win) => {
                win.init(updateLevelupRewardButton);
            });
		} else if(gameObj.name == "backButton"){
			UiManager.Instance.openWindow<NoticeWindow>((win) => {
				win.entranceId = NoticeEntranceType.BACK_NOTICE;
			});
		} else if(gameObj.name == "sevenDaysHappyButton"){
			if(SevenDaysHappyManagement.Instance.getSevenDaysHappySampleDic().Count > 0)
			{
				UiManager.Instance.openWindow<SevenDaysHappyWindow>((win) => {
					win.defaultSelectSid = SevenDaysHappyManagement.Instance.getDayIndex();
					win.initTopButton();
				});
			}
			else
			{
				SevenDaysHappyInfoFPort fport = FPortManager.Instance.getFPort ("SevenDaysHappyInfoFPort") as SevenDaysHappyInfoFPort;
				fport.SevenDaysHappInfoAccess(()=>{
					UiManager.Instance.openWindow<SevenDaysHappyWindow>((win) => {
						win.defaultSelectSid = SevenDaysHappyManagement.Instance.getDayIndex();
						win.initTopButton();
					});
				});
			}
		}else {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0358"));
        }
	}
    void openTeamEmtpyWindow(List<int> ids)
    {
        UiManager.Instance.openWindow<TeamEditWindow>((win) =>
        {
            win.initInfo(ids);
            win.setComeFrom(TeamEditWindow.FROM_MAINMENU);
        });
    }
	private void openGuildWindow ()
	{
		GuildManagerment.Instance.openWindow ();
	}
	
//	private void openTaskWindow ()
//	{
//		UiManager.Instance.openWindow<TaskWindow>((TaskWindow win)=>{
//			win.Initialize (windowBack);
//		});
//		hideWindow ();
//	}
	//窗口回调方法
	private void windowBack ()
	{
		UiManager.Instance.openWindow<MainWindow> ();
	}

	public override void OnBeginCloseWindow ()
	{
		base.OnBeginCloseWindow ();
		setBgToggle (false);
	}
	
	public override void DoDisable ()
	{ 
		base.DoDisable ();

		if (timer != null) {
			timer.stop ();
			timer = null;
		}
		UiManager.Instance.mainWindow = null;
		//UiManager.Instance.destoryWindowByName ("radioWindow");
	}



	// 检测跨天情况，对一些数据进行刷新
	public void refreshInfo ()
	{
		if (this == null || !gameObject.activeInHierarchy) {
			if (GameManager.Instance.timer != null) {
				GameManager.Instance.timer.stop ();
				GameManager.Instance.timer = null;
			}
			return;
		}
		//只有在主界面，才进行检测
		if (GameManager.Instance.loginDay != ServerTimeKit.getDayOfYear ()) {
			GameManager.Instance.loginDay = ServerTimeKit.getDayOfYear ();
			(FPortManager.Instance.getFPort ("InitTaskFPort") as InitTaskFPort).access (showTaskInfo);
			(FPortManager.Instance.getFPort ("InitLuckyDrawFPort") as InitLuckyDrawFPort).init (showLuckyInfo);
			(FPortManager.Instance.getFPort ("PyxFPort") as PyxFPort).pyxInfo (showHallowsInfo);

			FuBenManagerment.Instance.cleanAll ();
			//跨天清理副本数据后需要重新获取，不然会有BUG
			initFuBenStoryData ();
		}

//		if ((ServerTimeKit.getCurrentSecond () > prayBeginTime) && (ServerTimeKit.getCurrentSecond () < prayEndTime)) {
//		//	noticing = true;
//			noticeEffect.gameObject.SetActive(true);
//		}
	}

	private void initFuBenStoryData ()
	{
		FuBenInfoFPort fport = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort; 
		fport.info (initFuBenWarData, ChapterType.STORY);
	}
	
	private void initFuBenWarData ()
	{
		FuBenInfoFPort fport = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort; 
		fport.info (totalLoginAward, ChapterType.WAR);
	}

	private void totalLoginAward ()
	{
		TotalLoginFPort totalLoginFPort = FPortManager.Instance.getFPort ("TotalLoginFPort") as TotalLoginFPort;
		totalLoginFPort.access (totalPrize);	
	}
	
	private void showHallowsInfo ()
	{
		GameObject win = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/intensifyHallowsWindow");
		if (win != null) {
			(win.GetComponent<IntensifyHallowsWindow> ()).initInfo ();
		}
	}
	
	private void showTaskInfo ()
	{
		GameObject win = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/taskWindow");
		if (win != null) {
			(win.GetComponent<TaskWindow> ()).updateContent ();
		} else {
			showTaskNum ();
		}
	}
	
	private void showLuckyInfo ()
	{
		GameObject win = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/luckyDrawWindow");
		if (win != null) {
			(win.GetComponent<LuckyDrawWindow> ()).updateList ();
		}
	}

	private void slideEvent ()
	{
		showIncNum ();
	}
	
	//显示队伍战斗力
	void showTeamCombat ()
	{
		int pveCombat = ArmyManager.Instance.getTeamAllCombat (1);
		int pvpCombat = ArmyManager.Instance.getTeamAllCombat (3);
		combat.text = Mathf.Max (pveCombat,pvpCombat) + "";
	}

	#region 等级引导

	/// <summary>
	/// 跳转0-2页,新手专用，带跳步骤！
	/// </summary>
	public void jumpToPage (int page, bool isDoGuide)
	{
		maskDragSV.enabled = false;
		centerOnChild.CenterOn (pageObjs [page].transform);
		if (isDoGuide) {
			StartCoroutine (Utils.DelayRun (() => {
				GuideManager.Instance.doGuide ();
				GuideManager.Instance.guideEvent ();
				GuideManager.Instance.doFriendlyGuideEvent ();
			}, 0.7f));
		}
		StartCoroutine (Utils.DelayRun (() => {
			maskDragSV.enabled = true;
			MaskWindow.UnlockUI ();
		}, 1.2f));
	}

	/// <summary>
	/// 图标排序
	/// </summary>
	public void gridGuideIco ()
	{
		StartCoroutine (Utils.DelayRun (() => {
			guideGrid.repositionNow = true;
		}, 0.1f));
	}
	/// <summary>
	/// 根据引导开启图标
	/// </summary>
	public void showIco ()
	{
		openButton (buttonExchange, 106003000);//兑换
	}

	private void openButton (ButtonBase _button, int guideSid)
	{
		if (GuideManager.Instance.isOverStep (guideSid)) {
			_button.gameObject.SetActive (true);
			if (GuideManager.Instance.isEqualStep (guideSid))
				showButtonEffect (_button);
		}
	}

	private void showButtonEffect (ButtonBase _button)
	{
		EffectManager.Instance.CreateEffect (_button.transform, "Effect/UiEffect/feature_open");
	}

	#endregion

	#region 友情引导

	private int friendlyGuideType = 0;

	/// <summary>
	/// 初始化友情指引提示面板
	/// </summary>
	private void initFriendlyGuid ()
	{
        commandAmni.SetActive(false);
        godsAmni.SetActive(false);
        DateTime dtTime = TimeKit.getDateTime(ServerTimeKit.getSecondTime());
        int nowWint = TimeKit.getWeekCHA(dtTime.DayOfWeek);
	    if (nowWint == 6 || nowWint == 7) commandAmni.SetActive(true);
	    else
	    {
	        godsAmni.SetActive(true);
	        godsTranform.localPosition = Vector3.zero;
	    }
		if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID)) {
			GameManager.Instance.isFormMissionByGuide = false;
			return;
		}
		friendlyGuideType = GuideManager.Instance.isHaveNewFriendlyGuide ();
		if (friendlyGuideType != 0) {
			if (GameManager.Instance.isFormMissionByGuide) {
				jumpToPage (1, false);
			}
			if (!increaseButton.gameObject.activeSelf) {
				increaseButton.gameObject.SetActive (true);
				TweenScale ts = TweenScale.Begin (increaseButton.gameObject, 0.2f, Vector3.one);
				ts.method = UITweener.Method.EaseOut;
			}
			objFriendlyGuide.SetActive (true);
			labelFriendlyGuideDesc.text = GuideManager.Instance.getFriendlyGuideStr (friendlyGuideType);
            if (godsAmni.activeInHierarchy) godsTranform.localPosition = Vector3.zero;
		} else {
			objFriendlyGuide.SetActive (false);
            if (IncreaseManagerment.Instance.showSum() < 0) {
                increaseButton.gameObject.SetActive(false);
            } 
		}
		GameManager.Instance.isFormMissionByGuide = false;
	}

	/// <summary>
	/// 切换显示vip和growup图标
	/// </summary>
	private void toggleVipGrowup ()
	{
		if (GrowupAwardMangement.Instance.GetAwardList () == null) {
			GrowupAwardMangement.Instance.InitAwards (() => {
				showGrowup ();
			});
		} else {
			showGrowup ();
		}
	}

	private void showGrowup ()
	{
		//.成长计划全部领取完毕，图标直接消失
		if (GrowupAwardMangement.Instance.GetAwardList () != null && GrowupAwardMangement.Instance.GetAwardList ().Count == 0 || 
		//玩家等级大于等于61级时，如果未购买成长计划，图标消失
			GuideManager.Instance.isLessThanStep (50001000)) {
			growupButton.gameObject.SetActive (false);
			growupNum.gameObject.SetActive (false);
			growupBackground.gameObject.SetActive (false);
		} else {
			growupButton.gameObject.SetActive (true);
			int num = GrowupAwardMangement.Instance.GetAwardNum ();
			if (num != 0) {
				growupNum.text = num.ToString ();
				growupNum.gameObject.SetActive (true);
				growupBackground.gameObject.SetActive (true);
			} else {
				growupNum.gameObject.SetActive (false);
				growupBackground.gameObject.SetActive (false);
			}
		}
	}
	#endregion

	float xPosNow =-1;

	/// <summary>
	/// 设置背景位移
	/// </summary>
	private void setBgPos () {
		bgFront.transform.localPosition = new Vector3 (-xPosNow / 5, bgFront.transform.localPosition.y, 0);
		bgCenter.transform.localPosition = new Vector3 (-xPosNow / 10, bgCenter.transform.localPosition.y, 0);
		bgFar.transform.localPosition = new Vector3 (-xPosNow / 15, bgFar.transform.localPosition.y, 0);
	}

	/// <summary>
	/// 设置背景开关
	/// </summary>
	private void setBgToggle (bool b) {
		xPosNow = launcherPanel.clipOffset.x;
		setBgPos ();
		bgParent.SetActive (b);
	}

	void Update ()
	{
		if (xPosNow != launcherPanel.clipOffset.x) {
			setBgPos ();
			xPosNow = launcherPanel.clipOffset.x;
		}

		if(BackPrizeLoginInfo.Instance.loginTime == 0)
		{
			BackPrizeLoginInfo.Instance.loginTime = ServerTimeKit.getLoginTime();
		}
		if(ServerTimeKit.getMillisTime() >= BackPrizeLoginInfo.Instance.getSecondDayTime(BackPrizeLoginInfo.Instance.loginTime))// 跨天//
		{
			BackPrizeLoginInfo.Instance.loginTime = ServerTimeKit.getMillisTime();
			// 回归登录//
			BackPrizeInfoFPort bpif = FPortManager.Instance.getFPort ("BackPrizeInfoFPort") as BackPrizeInfoFPort;
			bpif.BackPrizeLoginInfoAccess(showBackButton);
			// 周卡//
			WeekCardInfoFPort wcif = FPortManager.Instance.getFPort ("WeekCardInfoFPort") as WeekCardInfoFPort;
			wcif.WeekCardInfoAccess(showZhouNianQingNotice);
			// 福袋//
			RebateInfoFPort rif = FPortManager.Instance.getFPort ("RebateInfoFPort") as RebateInfoFPort;
			rif.RebateInfoAccess(showZhouNianQingNotice);
		}
	}
	void showZhouNianQingNotice()
	{
		int zhouNianQing = 0;
		Notice noticeTemp;
		//周年庆入口显示特效、次数
		noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.SIGNIN);
		if (noticeTemp != null) { 
			SignInSample sample = SignInSampleManager.Instance.getSignInSampleBySid(StringKit.toInt(noticeTemp.sid + "" + ServerTimeKit.getCurrentMonth()));
			if(sample == null) return;
			List<int> sids = sample.daySids;
			if (!isHave) {
				GetSignInInfoFport fport = FPortManager.Instance.getFPort("GetSignInInfoFport") as GetSignInInfoFport;
				fport.getSignInInfo(null);
				isHave = true;
			}
			if (!SignInManagerment.Instance.stateList.Contains(sids[ServerTimeKit.getDayOfMonth() - 1])) {
				zhouNianQing++;
			}
		}
		noticeTemp = NoticeManagerment.Instance.getNoticeByType(NoticeType.SHAREDRAW);
		if (noticeTemp != null) {
			if (ShareDrawManagerment.Instance.isFirstShare == 0) {
				zhouNianQing++;
			}
			if (ShareDrawManagerment.Instance.canDrawTimes != 0) {
				zhouNianQing += ShareDrawManagerment.Instance.canDrawTimes;
			}
		}
		// 周卡是否可领取//
		if(WeekCardInfo.Instance.recevieState == WeekCardRecevieState.recevie)
		{
			zhouNianQing++;
		}
		// 福袋可领取个数//
		if(RebateInfoManagement.Instance.canRecevieCount > 0)
		{
			zhouNianQing += RebateInfoManagement.Instance.canRecevieCount;
		}
		if (zhouNianQing > 0) {
			zhouNianQingNum.text = zhouNianQing.ToString();
			zhouNianQingNum.gameObject.SetActive(true);
			zhouNianQingBackground.gameObject.SetActive(true);
			zhouNianQingEffect.gameObject.SetActive(true);
		} else {
			zhouNianQingNum.gameObject.SetActive(false);
			zhouNianQingBackground.gameObject.SetActive(false);
			zhouNianQingEffect.gameObject.SetActive(false);
		}
	}
	public void update_RMB()
	{
		rmbLabel.text=UserManager.Instance.self.getRMB().ToString();
	}
}