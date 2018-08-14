using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 天梯主窗口
/// </summary>
public class LaddersWindow : WindowBase
{
	
	public LaddersPlayerContent playerContent;
	public LaddersChestContent chestContent;
	public UILabel label_refreshTime;
	public UITexture texture_userHead;
	public UILabel label_userName;
	public UILabel label_level;
	public UILabel label_rank;
	public UILabel label_combat;
	public UILabel label_laddersTimes;
	public UIPlayTween tweenHelp;
	/** 声望进度条 */
	public barCtrl prestigeExpBar;
	/** 声望文字显示 */
	public UILabel label_prestige;
	public UILabel label_lastRecord;
	public UILabel label_title;
	public UISprite sprite_vip;
	public UISprite sprite_medalBg;
	[HideInInspector]
	public int
		lastPrestigeLevel;
	[HideInInspector]
	public bool
		fightBack, fightWin; //是否从天梯战斗回来 是否胜利

	private bool needUpdate;
	private int tempV;
	private int lastLadderRank;
    public GameObject miaoShaoPerfab;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void init () {
		M_updateView ();
        fightBackShow ();
		if (GuideManager.Instance.isEqualStep (133003000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
		if (!isAwakeformHide) {
			lastLadderRank = LaddersManagement.Instance.currentPlayerRank;
			tempV = StorageManagerment.Instance.tmpStorageVersion;
		}
	}

	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		tempV = StorageManagerment.Instance.tmpStorageVersion;
		LaddersGetInfoFPort newFport = FPortManager.Instance.getFPort<LaddersGetInfoFPort> ();
		newFport.apply ((hasApply) =>
		{
		    //LaddersManagement.Instance.CurrentOppPlayer = null;
			M_updateView ();
			lastLadderRank = LaddersManagement.Instance.currentPlayerRank;
		});
        M_updateView();
	}
	/// <summary>
	/// 战斗结束后 回来需要显示战斗中获得的奖励，战斗成功和失败 显示的提示不一样 需要区分
	/// </summary>
	/// <param name="awards">Awards.</param>
	private void showCallBack (Award[] awards)
	{
		if (fightWin) {
			UiManager.Instance.openDialogWindow<AllAwardViewWindow> ((win) => {
				win.Initialize (awards [0], getAwardBack, null);
				win.showComfireButton (true, Language ("ladderButton"));
				int currentRank = LaddersManagement.Instance.currentPlayerRank;
				if (currentRank != lastLadderRank) {
					win.topLabel.text = Language ("laddersTip_39") + "," + Language("laddersTip_28", currentRank);
					lastLadderRank = currentRank;
				} else {
					win.topLabel.text = Language ("laddersTip_39") + "," + Language ("laddersTip_32");
				}
			});
		} else {
			UiManager.Instance.openDialogWindow<AllAwardViewWindow> ((win) => {
				win.Initialize (awards [0], getAwardBack, null);
				win.showComfireButton (true, Language ("ladderButton"));
				if (LaddersManagement.Instance.currentBattleIsFriendHelp) {
					win.topLabel.text = Language ("laddersTip_40") + "," + Language("laddersTip_29");
				} else {
					win.topLabel.text = Language ("laddersTip_40") + "," + Language("laddersTip_31");
				}
			});
			/*
			TextTipWindow.Show (Language ("laddersTip_27", awards [0].prestigeGap.ToString (), awards [0].moneyGap.ToString ()), 0.5f);
			StartCoroutine(Utils.DelayRun (showPrestigeLevel, 0.6f));
			*/
		}
	}
	/// <summary>
	/// 战斗会有奖励 结束完后应该展示
	/// </summary>
	private void fightBackShow ()
	{
		if (!fightBack)
			return;
	    if (GameManager.Instance.isCanBeSecondSkill &&
	        PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1)
	    {
            EffectManager.Instance.CreateEffectCtrlByCache(miaoShaoPerfab.transform, "Effect/UiEffect/Miaosha", null);
	        //miaoShaoPerfab.SetActive(true);
	        StartCoroutine(Utils.DelayRun(() =>
	        {
	            AwardManagerment.Instance.addFunc(AwardManagerment.AWARDS_LADDER, showCallBack);
	            fightBack = false;
	            GameManager.Instance.isCanBeSecondSkill = false;
                //miaoShaoPerfab.SetActive(false);
	        }, 2f));
	    }
	    else
	    {
            AwardManagerment.Instance.addFunc(AwardManagerment.AWARDS_LADDER, showCallBack);
            fightBack = false;
	    }
		
	}
	/// <summary>
	/// 领取完 战斗奖励后回调 需要判断是否有奖励进入临时仓库
	/// </summary>
	private void getAwardBack ()
	{
		if (StorageManagerment.Instance.tmpStorageVersion != tempV) {
			//MessageWindow.ShowAlert (Language ("goddnessShake16"), showPrestigeLevel);
            MaskWindow.UnlockUI();
		} else {
			showPrestigeLevel ();
		}
		tempV = StorageManagerment.Instance.tmpStorageVersion;
	}
	
	//声望升级特效
	public void showPrestigeLevel ()
	{
		if (LaddersManagement.Instance.M_getCurrentPlayerTitle ().index > lastPrestigeLevel) {
			lastPrestigeLevel = LaddersManagement.Instance.M_getCurrentPlayerTitle ().index;
			UiManager.Instance.openDialogWindow<LaddersTitleWindow> ((win) => {
				win.playPrestigeLevelUpEffect ();
			});
		}
	}
	/// <summary>
	/// 显示声望等级
	/// </summary>
	/// <param name="msg">Message.</param>
	public void showPrestigeLevel (MessageHandle msg)
	{
		showPrestigeLevel ();
	}

	/// <summary>
	/// 领取宝箱完后 判断是否播放升级动画
	/// </summary>
	public void M_onReceiveChestBox ()
	{
		showPrestigeLevel ();
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		//UiManager.Instance.backGroundWindow.switchToDark ();
	}

	void Update ()
	{
		if (needUpdate && Time.frameCount % 30 == 0) {
			M_updateRefreshCutdown ();
		}
	}
	/// <summary>
	/// 更新主视图 宝箱信息/玩家信息
	/// </summary>
	public void M_updateView ()
	{
		playerContent.M_updateAll (LaddersManagement.Instance.Players.M_getPlayers ());
		chestContent.M_updateAll (LaddersManagement.Instance.Chests.M_getChests ());

		M_updateUserInfo ();
		M_updateRefreshCutdown ();
	}

	public void M_updateAfterBattle ()
	{

	}
	/// <summary>
	/// 当点击玩家后 开始战斗 如果是机器人 则直接开始战斗，如果是真人则显示此人的玩家信息 用UID来判断
	/// </summary>
	/// <param name="_item">_item.</param>
	public void M_onClickPlayer (Ladders_PlayerItem _item)
	{
		if (LaddersManagement.Instance.maxFightTime - LaddersManagement.Instance.currentChallengeTimes <= 0) {
			Vip vip = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.vipLevel);
			if (vip != null){
				if(LaddersManagement.Instance.buyFightCount < vip.privilege.laddersCountBuyAdd){
					openBuyWindow(vip);
				}
				else if(LaddersManagement.Instance.buyFightCount < VipManagerment.Instance.getVipbyLevel(12).privilege.laddersCountBuyAdd){
					openVipWindow();
				}
				else{
					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("laddersTip_10"), null);
				}
			}
			else{
				openVipWindow();
			}
			return;
		}

		LaddersPlayerInfo clickPlayer = _item.data;
		LaddersManagement.Instance.CurrentOppPlayer = clickPlayer;

		if (clickPlayer.uid == "0") {	
			string selfUid = UserManager.Instance.self.uid;		
			LaddersManagement.Instance.currentBattleIsFriendHelp = false;
			PvpInfoManagerment.Instance.sendLaddersFight (selfUid, M_onRequestLadderBattleCmp);

		} else {
			LaddersGetPlayerInfoFPort fport = FPortManager.Instance.getFPort ("LaddersGetPlayerInfoFPort") as LaddersGetPlayerInfoFPort;
			fport.access (clickPlayer.uid,10, M_onGetPlayerInfoCmp);
		}
	}
	/// <summary>
	/// 请求战斗开始后 回调 已判断请求是否正确
	/// </summary>
	/// <param name="msg">Message.</param>
	private void M_onRequestLadderBattleCmp (string msg)
	{
		if (!msg.Equals ("ok")) {
			MessageWindow.ShowAlert (Language ("laddersTip_33"));
			if (UiManager.Instance.emptyWindow != null) {
				UiManager.Instance.emptyWindow.finishWindow ();
			}
			OnNetResume ();

		} else {
			GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
			MaskWindow.instance.setServerReportWait (true);
		}
		

	}

	/// <summary>
	/// 点击宝箱事件处理
	/// </summary>
	/// <param name="_item">_item.</param>
	public void M_onClickBox (Ladders_BoxItem _item)
	{
		int index = _item.data.index;
		if (_item.data.receiveEnble) {
			LaddersChestRefreshFPort fport = FPortManager.Instance.getFPort ("LaddersChestRefreshFPort") as LaddersChestRefreshFPort;
			fport.apply (index, () => {
				LaddersChestInfo newChest = LaddersManagement.Instance.Chests.M_getChest (index - 1);
				M_openChest (newChest);
			});
		} else {
			M_openChest (_item.data);
		}
	}
	/// <summary>
	/// 点击宝箱 打开宝箱窗口 宝箱中的奖励是固定的 有money 和prestige构成，而这两种奖励的数据又是由公式计算而得
	/// </summary>
	/// <param name="_item">_item.</param>
	private void M_openChest (LaddersChestInfo _item)
	{
		int money = LaddersManagement.Instance.M_chestMoney (_item);
		PrizeSample prize_1 = new PrizeSample (PrizeType.PRIZE_MONEY, 0, money);
		
		int prestige = LaddersManagement.Instance.M_chestPrestige (_item);
		PrizeSample prize_2 = new PrizeSample (PrizeType.PRIZE_PRESTIGE, 0, prestige);
		
		UiManager.Instance.openDialogWindow<LaddersChestsWindow> ((win) => {
			win.initAward (_item, new PrizeSample[]{prize_1,prize_2});
		});
	}
	/// <summary>
	/// 点击战报记录中的 超链 ，回播战报
	/// </summary>
	/// <param name="url">URL.</param>
	public void M_onClickRecord (string url)
	{
		UiManager.Instance.openWindow<EmptyWindow> ((win) => {
			LaddersBattleReplayFPort fport = new LaddersBattleReplayFPort ();
			fport.apply (StringKit.toInt (url), null); 
		});
	}
	/// <summary>
	/// 更新玩家的当前信息 vip,战斗力，称号，奖章等
	/// </summary>
	public void M_updateUserInfo ()
	{
		User self = UserManager.Instance.self;
		int vipLv = self.getVipLevel ();
		int combat = ArmyManager.Instance.getTeamCombat (ArmyManager.PVP_TEAMID);

		LaddersTitleSample currentTitle = LaddersManagement.Instance.M_getCurrentPlayerTitle ();
		if (currentTitle != null) {
			label_title.text = LaddersManagement.Instance.M_getCurrentPlayerTitle ().name;
		} else {
			label_title.text = Language ("laddersTip_14");
		}

		LaddersMedalSample currentMedal = LaddersManagement.Instance.M_getCurrentPlayerMedal ();
		if (currentMedal != null) {
			sprite_medalBg.spriteName = "medal_" + Math.Min (currentMedal.index + 1, 5);
		} else {
			sprite_medalBg.spriteName = "medal_0";
		}

		label_userName.text = self.nickname;
		if (vipLv > 0) {
			sprite_vip.gameObject.SetActive (true);
			sprite_vip.spriteName = "vip" + vipLv;
		} else {
			sprite_vip.gameObject.SetActive (false);
		}

		label_level.text = "Lv." + self.getUserLevel ().ToString ();

		label_rank.text = Language ("laddersPrefix_01") + LaddersManagement.Instance.currentPlayerRank.ToString ();
		label_combat.text = Language ("laddersPrefix_02") + combat.ToString ();

		//获取自身声望值
		int myPrestige = UserManager.Instance.self.prestige;
		//通过自身声望值获取对应称号
		LaddersTitleSample currentTitleSample=LaddersConfigManager.Instance.config_Title.M_getTitle(myPrestige);
		//通过自身称号获取下一级称号
		LaddersTitleSample nextTitleSample=LaddersConfigManager.Instance.config_Title.M_getTitleByIndex(currentTitleSample.index+1);
		//如果不存在下一级称号
		if (nextTitleSample == null) {
			prestigeExpBar.updateValue(myPrestige,myPrestige);
			label_prestige.text = myPrestige + "/" + myPrestige;
		}
		//存在下一级称号
		else {
			if (LaddersConfigManager.Instance.config_Title.isMaxIndex(currentTitleSample.index+1)) {
				prestigeExpBar.updateValue(nextTitleSample.minPrestige,nextTitleSample.minPrestige);
				label_prestige.text =nextTitleSample.minPrestige + "/" + nextTitleSample.minPrestige;
			} else {
				prestigeExpBar.updateValue(myPrestige,nextTitleSample.minPrestige);
				label_prestige.text = myPrestige + "/" + nextTitleSample.minPrestige;
			}
		}
		label_laddersTimes.text = Language ("laddersPrefix_04", (LaddersManagement.Instance.maxFightTime - LaddersManagement.Instance.currentChallengeTimes).ToString (), LaddersManagement.Instance.maxFightTime.ToString ());

		UserManager.Instance.setSelfHeadIcon (texture_userHead);

		LaddersRecordInfo lastRecord = LaddersManagement.Instance.Records.M_getLastRecord ();
		if (lastRecord != null) {
			label_lastRecord.text = lastRecord.description;
		} else {
			label_lastRecord.text = "";
		}
	}
	/// <summary>
	/// 更新天梯对手刷新倒计时
	/// </summary>
	private void M_updateRefreshCutdown ()
	{
		int time = LaddersManagement.Instance.M_getRefreshCutdown ();
		needUpdate = time > 0;
		label_refreshTime.text = TimeKit.timeTransform (time * 1000);
	}
	/// <summary>
	/// 按钮事件点击处理
	/// </summary>
	/// <param name="gameObj">Game object.</param>
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		string btnName = gameObj.name;
		switch (btnName) {
		case "close":
			finishWindow ();
			break;
		case "btn_award":				
			UiManager.Instance.openWindow<LaddersAwardWindow> ();
			break;
		case "btn_medal":
			UiManager.Instance.openWindow<LaddersMedalWindow> ();
			break;
		case "btn_refresh":
			if (LaddersManagement.Instance.Chests.M_checkHasReceiveChest ()) {
				MessageWindow.ShowAlert (Language ("laddersTip_17"));
				return;
			}
			if (needUpdate) {
				int price = LaddersConfigManager.Instance.config_Const.price_refreshPlayer;
				string tipInfo = Language ("laddersTip_02", price.ToString ());
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => { 
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), tipInfo, M_onMessageHandler);
				});

			} else {
				M_refreshRequest ();
			}
			break;
		case "btn_team":
			UiManager.Instance.openWindow<TeamEditWindow> ((win) => {
				win.setComeFrom (TeamEditWindow.FROM_LADDERS);
			});
			break;
		case "btn_rank":
			UiManager.Instance.openWindow<RankWindow> ((win) => {
				win.selectTabType = RankManagerment.TYPE_LADDER;
				MaskWindow.UnlockUI ();
			});
			break;	
		case "btn_record":
			UiManager.Instance.openDialogWindow<LaddersRecordsWindow> ();
			break;
		case "btn_addLaddersTimes":
			if (LaddersManagement.Instance.currentChallengeTimes < 1) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena38"), null);
				return;
			}
			Vip vip = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.vipLevel);
			if (vip == null || LaddersManagement.Instance.buyFightCount >= vip.privilege.laddersCountBuyAdd) {
				openVipWindow();
				return;
			}
			openBuyWindow(vip);
			break;
		case "buttonHelp":
			tweenerMessageGroupIn (tweenHelp);
			MaskWindow.UnlockUI();
			break;
		case "buttonCloseHelp":
			tweenerMessageGroupOut (tweenHelp);
			MaskWindow.UnlockUI();
			break;
		}
	}
	/// <summary>
	/// 点击刷新对手后 给出提示
	/// </summary>
	private void M_showRefreshedTip ()
	{
		//laddersTip_11|对手已刷新
		//laddersTip_12|请先领取宝箱奖励
		TextTipWindow.Show (Language ("laddersTip_11"));
	}
	/// <summary>
	/// 刷新对手对话框 关闭后处理
	/// </summary>
	/// <param name="msg">Message.</param>
	private void M_onMessageHandler (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		int price = LaddersConfigManager.Instance.config_Const.price_refreshPlayer;
		if (price > UserManager.Instance.self.getRMB ()) {
			MessageWindow.ShowRecharge (LanguageConfigManager.Instance.getLanguage ("s0158"));
		} else {
			M_refreshRequest ();
		}
	}
	/// <summary>
	/// 发送刷新玩家请求
	/// </summary>
	private void M_refreshRequest ()
	{
		LaddersRefreshPlayerFPort fport = FPortManager.Instance.getFPort<LaddersRefreshPlayerFPort> ();
		fport.apply (M_onRefreshCmp, !needUpdate);
	}
	/// <summary>
	/// 刷新对手后回调
	/// </summary>
	/// <param name="_msg">_msg.</param>
	private void M_onRefreshCmp (string _msg)
	{
		if (_msg == "ok") {
			LaddersGetInfoFPort fport = FPortManager.Instance.getFPort<LaddersGetInfoFPort> ();
			fport.apply (M_onReGetInfoCmp);
		} else {
			MaskWindow.UnlockUI ();
		}
	}
	/// <summary>
	/// 刷新对手后回调 更新视图
	/// </summary>
	/// <param name="_value">If set to <c>true</c> _value.</param>
	private void M_onReGetInfoCmp (bool _value)
	{
		M_updateView ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 点击对手后 如果是真人，则请求改对手信息后 回调处理：显示玩家信息面板
	/// </summary>
	/// <param name="_playerInfo">_player info.</param>
	private void M_onGetPlayerInfoCmp (PvpOppInfo _playerInfo)
	{
		PvpPlayerWindow.comeFrom = PvpPlayerWindow.FROM_LADDERS;
		UiManager.Instance.openWindow<PvpPlayerWindow> (
			(win) => {
			win.teamType = 10;
			win.currentLaddersPlayer = LaddersManagement.Instance.CurrentOppPlayer;
			win.initInfo (_playerInfo);
		});
	}
	///<summary>
	/// 打开购买窗口
	/// </summary>
	private void openBuyWindow(Vip vip){
		UiManager.Instance.openDialogWindow<BuyWindow> ((win) => {
			win.init (new LaddersChallengePrice (), Mathf.Min (LaddersManagement.Instance.currentChallengeTimes, vip.privilege.laddersCountBuyAdd - LaddersManagement.Instance.buyFightCount),
			          1, PrizeType.PRIZE_RMB, (msg) => {
				if (msg.msgEvent == msg_event.dialogOK) {
					LaddersChallengePrice price = msg.msgInfo as LaddersChallengePrice;
					if (price.getPrice (msg.msgNum) > UserManager.Instance.self.getRMB ())
						MessageWindow.ShowRecharge (LanguageConfigManager.Instance.getLanguage ("s0158"));
					else
						FPortManager.Instance.getFPort<LaddersBuyChallengeFport> ().access (msg.msgNum, (success) => {
							if (success) {
								LaddersManagement.Instance.currentChallengeTimes -= msg.msgNum;
								LaddersManagement.Instance.buyFightCount += msg.msgNum;
								M_updateUserInfo ();
								UiManager.Instance.openDialogWindow<MessageLineWindow> ((showWin) => {
									showWin.Initialize (LanguageConfigManager.Instance.getLanguage ("s0056",LanguageConfigManager.Instance.getLanguage("Arena01") , msg.msgNum.ToString()));
								});
							}
						});
				}
			});
		});
	}
	///<summary>
	/// 打开VIP窗口
	/// </summary>
	private void openVipWindow()
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("recharge01"), LanguageConfigManager.Instance.getLanguage ("s0093"),
			                LanguageConfigManager.Instance.getLanguage ("laddersTip_22"), (msgHandle) => {
				if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
					UiManager.Instance.openWindow<VipWindow> ();
				}
			});
		});
	}

	/** 动态消息进入动画  */
	public void tweenerMessageGroupIn (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Forward;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0.4f;
		}
		tween.Play (true);
	}
	/** 动态消息退出动画  */
	public void tweenerMessageGroupOut (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Reverse;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0;
		}
		tween.Play (true);
	}
}

