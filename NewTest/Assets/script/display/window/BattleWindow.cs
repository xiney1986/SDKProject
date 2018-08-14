using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleWindow : WindowBase
{
	const float minDistance = 0.003f;
	public UIScrollBar playerMonsterBar;
	public float playerBarData;
	bool isPlayerShine;
	public UIScrollBar enemyMonsterBar;
	public float enemyBarData;
	bool isEnemyShine;
	public GameObject winAnimPoint;
	public GameObject failAnimPoint;
	public GameObject playerBarParticle;
	public GameObject enemyBarParticle;
	public ObjAnimCtrl banner;
	/** 战斗强化界面 */
	public GameObject battleStrengItem;
	public MonsterBuffCtrl playerBeastBar;
	public MonsterBuffCtrl enemyBeastBar;
	public maxPowerBuffCtrl maxPowerHitBar;
	/** 战斗类型框logo特效--比如:十人战,替补战 */
	public battleTypeBarCtrl battleTypeBar;
	public ComboBarCtrl comboBar;
	public GameObject starGroup;
	public GameObject buttonBack;
	public UILabel buttonBackText;
	public GameObject playMultiplButton;
	public UILabel playMultiplText;
	/** 战斗失败选择的强化按钮(临时用) */
	string chooseStrengButton;
	// 末日决战 女神赐福buff//
	public NvShenBlessBuffCtrl nvShenBless;
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
		autoOpen();
		updateBattlePlayVelocity ();
		MaskWindow.instance.setServerReportWait (false);
	}
	/// <summary>
	/// 自动开启2倍数
	/// </summary>
	void autoOpen(){
		if(MissionInfoManager.Instance.mission!=null){
			if(MissionInfoManager.Instance.mission.sid==41005&&FuBenManagerment.Instance.isNewMission (ChapterType.STORY,41005)&&PlayerPrefs.GetInt (UserManager.Instance.self.uid + "_" + "2Xfist", 0)==0){
				PlayerPrefs.SetInt(UserManager.Instance.self.uid + "_" + "2Xfist",1);
				UserManager.Instance.self.setBattlePlayVelocity(1);
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize(LanguageConfigManager.Instance.getLanguage("s0162l2"));
				});
			}
		}
	}
	public override void DoDisable ()
	{
		base.DoDisable ();
		Time.timeScale = GameManager.Instance.gameSpeed;
	}
	
	public void playBeastBuff (string buffName, string PicName, bool isPlayer)
	{
		if (isPlayer) {
			playerBeastBar.gameObject.SetActive (true);
			playerBeastBar.init (buffName, PicName, isPlayer);
			
		} else {
			enemyBeastBar.gameObject.SetActive (true);
			enemyBeastBar.init (buffName, PicName, isPlayer);
		}
		
	}
	
	public void playMaxPowerHitBuff ()
	{
		maxPowerHitBar.gameObject.SetActive (true);	
		maxPowerHitBar.init ();
	}

	public void playNvShenBlessBuff()
	{
		nvShenBless.gameObject.SetActive(true);
		nvShenBless.init(LastBattleManagement.Instance.nvBlessLv.ToString());
	}
	
	public 	void playBattleTypeBar ()
	{
		battleTypeBar.init ();
	}
	
	public override void OnAwake ()
	{
		base.OnAwake ();
		UiManager.Instance.battleWindow = this;
	}
	
	protected override void begin ()
	{
		base.begin ();
		
		if (GameManager.Instance.battleFast)
			Time.timeScale = 10;
		
		if (BattleManager.battleData.playerTeamInfo.guardianForce != null)
			changeMonsterBarValue (BattleManager.battleData.playerTeamInfo.guardianForce.hp * 0.01f, true);
		if (BattleManager.battleData.enemyTeamInfo.guardianForce != null)
			changeMonsterBarValue (BattleManager.battleData.enemyTeamInfo.guardianForce.hp * 0.01f, false); 
		
		//    int chapterType = MissionInfoManager.Instance.mission.getChapterType();
		
		if (!isAwakeformHide) {
			Mission mis = MissionInfoManager.Instance.mission;
			BattleDataErlang data = BattleManager.battleData;
			bool showBack = true;
			if (HeroRoadManagerment.Instance.currentHeroRoad != null)
				showBack = true;
			else if (data.isArenaMass || data.isArenaFinal || data.isPvP || data.isLadders || data.isLaddersRecord || data.isGuildBoss || data.isMineralFightRecord||data.isGodsWarGroupFight||data.isGodsWarFinal || data.isOneOnOneBossFight)
				showBack = true;
			else if (mis != null && mis.getChapterType () == ChapterType.PRACTICE) {
				showBack = true;
			} else if (mis != null && mis.getChapterType () == ChapterType.STORY && FuBenManagerment.Instance.getMyStarNumByMissionSid (mis.sid) == 0) {
				showBack = false;
			} else if (mis != null && FuBenManagerment.Instance.isNewMission (mis.getChapterType (), mis.sid)) {
				showBack = false;
			}

			
			//showBack = true;//临时开放跳过战斗
			setButtonBackActive (showBack);
			MaskWindow.UnlockUI ();
		}
		

	}
	
	private void setButtonBackActive (bool isActive)
	{
		if (isActive) 
			buttonBackText.text = LanguageConfigManager.Instance.getLanguage ("pvpend01");
		else
			buttonBackText.text = "";
		buttonBack.GetComponent<BoxCollider> ().enabled = isActive;
	}
	
	//播放新手特效退出游戏
	public void guideEffectExit ()
	{
		//BattleManager.Instance.awardFinfish ();
		UiManager.Instance.openDialogWindow<WhiteScreenWindow> ();
		StartCoroutine (Utils.DelayRun (() => {
			BattleManager.Instance.awardFinfish ();
		}, 2));
	}

	//显示胜利或者失败标语,竞技场决赛不管输赢默认播放胜利动画
	void playAnimBattleOver ()
	{
		if (BattleManager.battleData.isLaddersRecord || BattleManager.battleData.isMineralFightRecord || BattleManager.battleData.isOneOnOneBossFight || BattleManager.battleData.isLastBattleBossBattle)
			return;
		if (BattleManager.battleData.isArenaFinal)
			return;
		if (BattleManager.battleData.winnerID == TeamInfo.ENEMY_CAMP && !BattleManager.battleData.isArenaFinal) {
			MissionInfoManager.Instance.autoGuaji=false;
			passObj _obj = MonoBase.Create3Dobj ("Effect/UiEffect/battleAnim");
			if (failAnimPoint.transform.childCount == 0) {
				_obj.obj.transform.parent = failAnimPoint.transform;
				_obj.obj.transform.localPosition = Vector3.zero;
				_obj.obj.transform.localScale = Vector3.one;
				BattleAnimCtrl battleAnimCtrl = _obj.obj.GetComponent<BattleAnimCtrl> ();
			    if (BattleManager.battleData.isGodsWarGroupFight)
			    {
			        battleAnimCtrl.lianShengZhongJie.transform.localPosition = Vector3.zero;
                    battleAnimCtrl.lianShengZhongJie.SetActive(true);
			    }
			    else
			    {
                    battleAnimCtrl.battleFail.transform.localPosition = Vector3.zero;
                    battleAnimCtrl.battleFail.SetActive(true);
			    }
				//battleAnimCtrl.battleFail.transform.localPosition = Vector3.zero;
				//battleAnimCtrl.battleFail.SetActive (true);
			}
			
		} else {
			
			passObj _obj = MonoBase.Create3Dobj ("Effect/UiEffect/battleAnim");
			if (winAnimPoint.transform.childCount == 0) {
				_obj.obj.transform.parent = winAnimPoint.transform;
				_obj.obj.transform.localPosition = Vector3.zero;
				_obj.obj.transform.localScale = Vector3.one;
				BattleAnimCtrl battleAnimCtrl = _obj.obj.GetComponent<BattleAnimCtrl> ();
				battleAnimCtrl.battleWin.transform.localPosition = Vector3.zero;
				battleAnimCtrl.battleWin.SetActive (true);
			}
			
		}	
	}

	public IEnumerator  beginBattleOverAnim ()
	{
		//锁屏幕,防止出去后狂点按钮报错
		MaskWindow.LockUI ();

		Time.timeScale = GameManager.Instance.gameSpeed;
		yield return new WaitForSeconds (1f);

		playAnimBattleOver ();

		BattleDataErlang currentbattleData = BattleManager.battleData;

		if (BattleManager.battleData.isGMFight) {
            BattleManager.Instance.awardFinfish(); 
			yield break;
		}
        //恶魔挑战不播放战后动画
        if (BattleManager.battleData.isOneOnOneBossFight) {
            hideOther();
            UiManager.Instance.openDialogWindow<BossAwardWindow>((win) => {
                win.init(BattleManager.Instance.battleAwards, () => {
                    UiManager.Instance.switchWindow<EmptyWindow>((window) => {
                    ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outOneOnOneBossWindow);
                    });
                    BattleManager.battleData.isOneOnOneBossFight = false;
                });
            });
            yield break;
        }
		//  末日决战boss战阶段不播放胜利或失败动画//
		if(BattleManager.battleData.isLastBattleBossBattle)
		{
			hideOther();
			UiManager.Instance.openDialogWindow<LastBattleAwardWindow>((win) => {
				win.init(BattleManager.Instance.battleAwards, () => {
					UiManager.Instance.switchWindow<EmptyWindow>((window) => {
						ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outLastBattleWindow);
					});
					BattleManager.battleData.isLastBattleBossBattle = false;
				});
			});
			yield break;
		}
		// 末日决战结算界面//
		if(BattleManager.battleData.isLastBattle)
		{
			// 胜利//
			if(currentbattleData.winnerID == TeamInfo.OWN_CAMP)
			{
				StartCoroutine(showLastBattleAward());
				yield break;
			}

		}
        if (currentbattleData.isArenaMass || currentbattleData.isArenaFinal || currentbattleData.isGuide || currentbattleData.isLadders || currentbattleData.isPractice || currentbattleData.isGuildFight || currentbattleData.isMineralFightRecord||currentbattleData.isGodsWarFinal)
        {

			bool isWin = currentbattleData.winnerID == TeamInfo.OWN_CAMP;

            if (currentbattleData.isMineralFightRecord) {
                string name = BattleManager.battleData.winnerID == TeamInfo.ENEMY_CAMP ? currentbattleData.replayEnemyName : currentbattleData.replayAttackerName;
                TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("Arena39", name));
                yield return new WaitForSeconds(1.5f);
            }

			if (currentbattleData.isArenaFinal) {
				string name = BattleManager.battleData.winnerID == TeamInfo.ENEMY_CAMP ? currentbattleData.replayEnemyName : currentbattleData.replayAttackerName;
				TextTipWindow.ShowNotUnlock (LanguageConfigManager.Instance.getLanguage ("Arena39", name));

				yield return new WaitForSeconds (1.5f);
			}
			if (currentbattleData.isGodsWarFinal) {
				string name = BattleManager.battleData.winnerID == TeamInfo.ENEMY_CAMP ? currentbattleData.replayEnemyName : currentbattleData.replayAttackerName;
				TextTipWindow.ShowNotUnlock (LanguageConfigManager.Instance.getLanguage ("Arena39", name));
				
				yield return new WaitForSeconds (1.5f);
			}
			if (currentbattleData.isArenaMass) {//delay do
				if (isWin)
					yield return new WaitForSeconds (4f);
				else
					yield return new WaitForSeconds (4.5f);
			} else	if (currentbattleData.isLadders) {
				if (currentbattleData.isLaddersRecord) {
					string name = BattleManager.battleData.winnerID == TeamInfo.ENEMY_CAMP ? currentbattleData.replayEnemyName : currentbattleData.replayAttackerName;
					TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("Arena39", name));					
					yield return new WaitForSeconds (1.5f);
				} else {
					LaddersManagement.Instance.currentChallengeTimes++;
					LaddersWindow lw = UiManager.Instance.getWindow<LaddersWindow> ();
					lw.fightBack = true;
					lw.fightWin = isWin;

					if (isWin)
						yield return new WaitForSeconds (4f);
					else
						yield return new WaitForSeconds (4.5f);
				}
			}else if(currentbattleData.isGuildFight){
				GuildAreaWindow areaWindow = UiManager.Instance.getWindow<GuildAreaWindow>();
				if(areaWindow != null){
					areaWindow.isWin = isWin;
					areaWindow.isFightBack = true;
				}
				if (isWin)
					yield return new WaitForSeconds (2f);
				else
					yield return new WaitForSeconds (2.5f);
			} 
			else if (currentbattleData.isGuide) {

				yield return new WaitForSeconds (4.5f);
			} else if (currentbattleData.isPractice) {
				yield return new WaitForSeconds (4.5f);
			}
			//锁屏幕,防止出去后狂点ArenaAuditionsWindow的关闭按钮报错

			BattleManager.Instance.awardFinfish ();
			yield break;
		}

		//世界首领输赢都出去
		if (WorldBossManagerment.Instance.isAttackBoss) {
			yield return new WaitForSeconds (3f);
			hideOther ();
			BattleManager.Instance.awardFinfish ();
			yield break;
		}
        //if (BattleManager.battleData.winnerID != TeamInfo.ENEMY_CAMP&&MissionInfoManager.Instance.mission!=null) {
        //    if(MissionInfoManager.Instance.mission.getChapterType() == ChapterType.TOWER_FUBEN)
        //    {
        //        yield return new WaitForSeconds(3f);
        //        hideOther();
        //        BattleManager.Instance.awardFinfish();
        //        yield break;
        //    }  
        //}

		//敌人胜利的话
		if (BattleManager.battleData.winnerID == TeamInfo.ENEMY_CAMP && !currentbattleData.isGodsWarGroupFight && !currentbattleData.isOneOnOneBossFight) {//诸神战小组赛失败了也有奖励

			int chapterType = 0;
			if (MissionInfoManager.Instance.mission != null)
				chapterType = MissionInfoManager.Instance.mission.getChapterType ();

			//讨伐失败了没奖励的，提前出去
			if (FuBenManagerment.Instance.isWarAttackBoss) {
				FuBenManagerment.Instance.isWarAttackBossWin = false;
				popBattleStrengItem ();
				yield break;
			}
            if (FuBenManagerment.Instance.isWarActiveFuben)
            {
                FuBenManagerment.Instance.isWarActiveWin = false;
                popBattleStrengItem();
                yield break;
            }

			//公会首领，修炼
			if ((GuildManagerment.Instance.isGuildBattle || chapterType == ChapterType.PRACTICE) && !BattleManager.battleData.isHeroRoad) {

				if (BattleManager.battleData.isPvP == false) {
					yield return new WaitForSeconds (3f);
					//非pvp输了没奖励提前走掉
					hideOther ();
					BattleManager.Instance.awardFinfish ();
					yield break;
				}

            }
			if (MissionInfoManager.Instance.isTowerFuben() && !BattleManager.battleData.isHeroRoad) {
                yield return new WaitForSeconds(3f);
                hideOther();
                BattleManager.Instance.awardFinfish();
                if (MissionInfoManager.Instance.isTowerFuben()) {//判断是爬塔副本
                    UiManager.Instance.openDialogWindow<MessageWindow>((win) => {//弹出卡片全部阵亡提示
                        win.initWindow(1, LanguageConfigManager.Instance.getLanguage("ladderButton"), "", LanguageConfigManager.Instance.getLanguage("towerShowWindow06"), (msgHandle) => {
                            UiManager.Instance.missionMainWindow.outTowerFuBen();
                        });
                    });
                }
                yield break;
            } else {
                popBattleStrengItem();
                yield break;
            }
            
            //if (chapterType == ChapterType.TOWER_FUBEN) {
            //    UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
            //        win.initWindow(1, LanguageConfigManager.Instance.getLanguage("ladderButton"), "", LanguageConfigManager.Instance.getLanguage("towerShowWindow06"), (msgHandle) => {
            //            UiManager.Instance.missionMainWindow.outTowerFuBen();
            //        });
            //    });
            //    yield break;
            //}else {
            //    popBattleStrengItem ();
            //    yield break;
            //}
		}
		if (BattleManager.battleData.isPractice) {
			BattleManager.Instance.awardFinfish ();
			yield break;
		}


		//建立奖励管理
		AwardDisplayCtrl ctrl = BattleManager.Instance.gameObject.AddComponent<AwardDisplayCtrl> ();
		//英雄之章直接战斗
		if (HeroRoadManagerment.Instance.isCurrentDirectFight ()) {
			yield return new WaitForSeconds (3f);
			ctrl.Initialize (BattleManager.Instance.battleAwards, AwardManagerment.FB_END);
			BattleManager.Instance.playerTeam.hideAllParter ();
			BattleManager.Instance.enemyTeam.hideAllParter (); 
//			ArmyManager.Instance.unActiveArmy ();
			yield break;
		}
		
		if (BattleManager.battleData.isPvP)
			ctrl.Initialize (BattleManager.Instance.battleAwards, AwardManagerment.PVP);
		else if (BattleManager.battleData.isArenaMass)
			ctrl.Initialize (BattleManager.Instance.battleAwards, AwardManagerment.ARENA);
		else if(BattleManager.battleData.isGodsWarGroupAward)
		{
			ctrl.Initialize (BattleManager.Instance.battleAwards, AwardManagerment.AWARDS_GODSWAR_GROUP);
		}
		else
			ctrl.Initialize (BattleManager.Instance.battleAwards, AwardManagerment.PVE); 

		
		//yield return new WaitForSeconds (0.3f);
		
		
		
		//星星
		if (!BattleManager.battleData.isPvP && MissionInfoManager.Instance.mission!=null)
		{
			if(MissionInfoManager.Instance.mission.getChapterType () == ChapterType.STORY)
				yield return new WaitForSeconds (showStar ());
		}
				
		BattleManager.Instance.enemyTeam.hideAllParter (); 
		if(!BattleManager.battleData.isOneOnOneBossFight)
		BattleManager.Instance.playerExpAdd (ctrl.activeAward);
		float waitTime = 1.5f;
		
		if (UserManager.Instance.self.getVipLevel () > 0)
			waitTime += 0.3f;
		
		
		yield return new WaitForSeconds (waitTime);	

		bool isHaveBeast = false;//是否有女神上阵并获得经验
		bool isBeastLvUp = false;//女神是否升级
		BattleManager.Instance.playerTeam.hideAllParter ();
		CharacterData tempGuardianForce = BattleManager.Instance.playerTeam.GuardianForce;
		if (tempGuardianForce != null && !BattleManager.Instance.playerTeam.GuardianForce.role.isMaxLevel ()) {
			EXPAward expAward = BattleManager.Instance.getEXPAwardByStoreID (tempGuardianForce, ctrl.activeAward);
			if (expAward != null) {
				isHaveBeast = true;
				//召唤兽进场
				BattleManager.Instance.playerTeam.showGuardianForce ();
				//召唤兽exp
				yield return new WaitForSeconds (0.5f);
				if (BattleManager.Instance.GuardianForceExpAdd (tempGuardianForce, expAward))
					isBeastLvUp = true;
			}
		}

		hideOther ();
		yield return new WaitForSeconds (isHaveBeast ? (isBeastLvUp ? 2.5f : 1.5f) : 0.5f);
		BattleManager.Instance.EffectCamera.gameObject.SetActive (false);

		//讨伐战斗胜利结束处理,提前出去，不然会走AwardDisplayCtrl的openNextWindow，造成闪窗口现象
		if (FuBenManagerment.Instance.isWarAttackBoss && BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP) {
			//赢了需要打通副本
			FuBenManagerment.Instance.completeMission (MissionInfoManager.Instance.mission.sid, MissionInfoManager.Instance.mission.getChapterType (), MissionInfoManager.Instance.mission.starLevel);
			//赢了需要扣除次数
			FuBenManagerment.Instance.intoMission (MissionInfoManager.Instance.mission.sid, MissionInfoManager.Instance.mission.starLevel);
			FuBenManagerment.Instance.warWinAward = ctrl.award;

			BattleManager.Instance.awardFinfish ();
			if (MissionManager.instance != null) {
				MissionManager.instance.missionClean ();
			}
			MissionInfoManager.Instance.clearMission ();
			yield break;
		}
        //活动副本战斗胜利结束处理,提前出去，不然会走AwardDisplayCtrl的openNextWindow，造成闪窗口现象
        if (FuBenManagerment.Instance.isWarActiveFuben && BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP)
        {
            //赢了需要扣除次数
            FuBenManagerment.Instance.intoMission(MissionInfoManager.Instance.mission.sid, MissionInfoManager.Instance.mission.starLevel);
            FuBenManagerment.Instance.ActiveWinAward = ctrl.award;

            BattleManager.Instance.awardFinfish();
            if (MissionManager.instance != null)
            {
                MissionManager.instance.missionClean();
            }
            MissionInfoManager.Instance.clearMission();
            yield break;
        }

		ctrl.openNextWindow ();

		if (ctrl.award == null || ctrl.award.Length == 0) {
			yield return new WaitForSeconds (3f);	
		}
	}
	
	/// <summary>
	/// 弹出战斗强化界面
	/// </summary>
	private void popBattleStrengItem ()
	{
		MaskWindow.UnlockUI ();
		battleStrengItem.gameObject.SetActive (true);
		MissionInfoManager.Instance.autoGuaji=false;
	}
	
	private void changeStrengButton (GameObject strengButton)
	{
		strengButton.gameObject.SetActive (true);
	}

	public void hideOther ()
	{
		battleStrengItem.gameObject.SetActive (false);
		failAnimPoint.SetActive (false);
		winAnimPoint.SetActive (false);
		starGroup.SetActive (false);
	}

	private void smartTips ()
	{
		////设置数据
		string key = UserManager.Instance.self.uid + PlayerPrefsComm.BATTLE_SMART_TIPS + BattleManager.battleSid;//BattleManager.lastMissionEvent.sid;
		TeamInfoPlayer bossCard = null;
		for (int i = 0; i < BattleManager.battleData.enemyTeamInfo.list.Count; i++) {
			if (CardSampleManager.Instance.getRoleSampleBySid (BattleManager.battleData.enemyTeamInfo.list [i].sid).cardType == 3) {
				bossCard = BattleManager.battleData.enemyTeamInfo.list [i];
				break;
			}
		}


		int type = 0;
        
		//可能有挂机的情况
		if (MissionInfoManager.Instance.mission == null || BattleManager.battleData.isPvP)
			type = -1;
		else
			type = MissionInfoManager.Instance.mission.getChapterType ();
        
		string str = "";

		/////显示相应提示
		if (type == ChapterType.WAR && !BattleManager.battleData.isHeroRoad) {
            if (bossCard == null)
                str = LanguageConfigManager.Instance.getLanguage("smartTips_01");
            else
            {
                if (bossCard.hp / bossCard.maxHp < 0.35f)
                    str = LanguageConfigManager.Instance.getLanguage("smartTips_02");
                else
                    str = bossCard != null ? smartFirstBattleTips(key) : LanguageConfigManager.Instance.getLanguage("smartTips_01");
            }
		} else if (type == ChapterType.STORY  && !BattleManager.battleData.isHeroRoad) {
			int recommendedCombat = MissionSampleManager.Instance.getMissionSampleBySid (MissionInfoManager.Instance.mission.sid).getRecommendCombat (MissionInfoManager.Instance.mission.starLevel);
            if (bossCard != null)
                str = LanguageConfigManager.Instance.getLanguage("smartTips_03");
            else
            {
            if ((recommendedCombat * 0.85f) > ArmyManager.Instance.getTeamCombat (ArmyManager.PVE_TEAMID))
				str = LanguageConfigManager.Instance.getLanguage ("smartTips_01");
			else
				str = smartFirstBattleTips (key);
            }
		}

		if (str.Length > 0) {
			PlayerPrefs.SetString (key, ServerTimeKit.getSecondTime ().ToString ());
			//讨伐 英雄之章不需要战败放弃副本友善指引
			if (type != ChapterType.WAR && type != ChapterType.HERO_ROAD) {
				UiManager.Instance.openDialogWindow<SmartTipsWindow> ((win) => {
					win.UI_Arrow1.SetActive(str == LanguageConfigManager.Instance.getLanguage("smartTips_01"));
					win.UI_Arrow2.SetActive(!win.UI_Arrow1.activeSelf);
					win.init (str);
				});
			}
		}
	}
	
	private string smartFirstBattleTips (string key)
	{
		string cache = PlayerPrefs.GetString (key);
		if (!string.IsNullOrEmpty (cache) && TimeKit.getDateTime (int.Parse (cache)).DayOfYear == TimeKit.getDateTime (ServerTimeKit.getSecondTime ()).DayOfYear)
			return LanguageConfigManager.Instance.getLanguage ("smartTips_01");
		else
			return LanguageConfigManager.Instance.getLanguage ("smartTips_02");
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name.StartsWith ("streng")) {
			if(BattleManager.battleData != null)
			{
				BattleManager.battleData.isHeroRoad = false;
			}
			ChooseStrengButton = gameObj.name;
			//UiManager.Instance.gameCamera.clearFlags = CameraClearFlags.SolidColor;
			ScreenManager.Instance.loadScreen (1, null, GameManager.Instance.outStrengItem);
		} else if (gameObj.name == "confirmButton") {
			smartTips ();
			BattleManager.Instance.awardFinfish ();
            hideOther();
		} else if (gameObj.name == "playMultiplButton") {
			if (!GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWOVERSID)) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("battle08"));
				return;
			}
			int maxUpLimit = User.BATTLE_PLAY_MULTIPLE.Length - 1;
			bool isMessage = false;
			int limitLevel = 30;
			if (UserManager.Instance.self.getUserLevel () < limitLevel && UserManager.Instance.self.vipLevel == 0) {
				maxUpLimit = User.BATTLE_PLAY_MULTIPLE.Length - 2;
				isMessage = true;
			}
			bool isAdd = UserManager.Instance.self.addBattlePlayVelocity (maxUpLimit);
			if (!isAdd && isMessage) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("battle09", limitLevel.ToString ()));
			}
			updateBattlePlayVelocity ();
			MaskWindow.UnlockUI ();
		} else {
			setButtonBackActive (false);
			BattleManager.Instance.Stop (BattleManager.Instance.skipBattle);
			MaskWindow.UnlockUI ();
		}
	}



	/// <summary>
	/// 更新战斗播放速度
	/// </summary>
	private void updateBattlePlayVelocity ()
	{
		float playVelocity = UserManager.Instance.self.getBattlePlayVelocity ();
		Time.timeScale = playVelocity;
		int showPlayVelocity = UserManager.Instance.self.getShowBattlePlayVelocity ();
		playMultiplText.text = "X " + showPlayVelocity.ToString ();
	}
	
	public void changeMonsterBarValue (float a, bool isPlayerSide)
	{
		UIScrollBar _tmpBar = null;
		float _data = 0;
		
		if (isPlayerSide) {
			_tmpBar = playerMonsterBar;
			_data = playerBarData;
		} else {
			_tmpBar = enemyMonsterBar;	
			_data = enemyBarData;		
		}
		
		_data += a;
		
		if (_data > 1 && a > 0) {
			_data = 1;	
		}
		
		if (_data <= 0 && a < 0) {
			_data = 0;
		} 
		
		if (isPlayerSide)
			playerBarData = _data;
		else
			enemyBarData = _data; 
		
	}
	
	void Update ()
	{
		if (Mathf.Abs (playerMonsterBar.barSize - playerBarData) <= minDistance) {
			playerMonsterBar.barSize = playerBarData;
		}
		if (Mathf.Abs (enemyMonsterBar.barSize - enemyBarData) <= minDistance) {
			enemyMonsterBar.barSize = enemyBarData;
		}		
		
		//这里是闪烁monsterBar
		if (playerMonsterBar.barSize >= 1 - minDistance) { 
			playerBarParticle.SetActive (true); 
		} else {
			playerBarParticle.SetActive (false);
			
		}
		
		if (enemyMonsterBar .barSize >= 1 - minDistance) {
			enemyBarParticle.SetActive (true);
		} else {
			enemyBarParticle.SetActive (false);			
		} 
		
		if (playerMonsterBar.barSize == playerBarData && enemyMonsterBar.barSize == enemyBarData)
			return;
		
		float _v = NGUIMath.Lerp (playerMonsterBar.barSize, playerBarData, Time.deltaTime * 4);
		if (_v < 0.01f)
			_v = 0.01f;
		playerMonsterBar.barSize = _v;
		
		_v = NGUIMath.Lerp (enemyMonsterBar.barSize, enemyBarData, Time.deltaTime * 4);		
		if (_v < 0.01f)
			_v = 0.01f;			
		enemyMonsterBar.barSize = _v; 
	}
	
	//开始星星动画,返回用时
	private float showStar ()
	{
		clearAnger ();
		List<CharacterData> list = BattleManager.Instance.getActionCharacters ();
		List<CharacterData> listLife = new List<CharacterData> ();
		foreach (CharacterData data in list) {
			if (data.characterCtrl != null && data.camp == TeamInfo.OWN_CAMP && !data.role.isBeast () && data.server_hp > 0) {
				listLife.Add (data);
			}
			if (listLife.Count >= 5) {
				break;
			}
		}
		int length = Mathf.Min (5, listLife.Count);
		GameObject starContent = starGroup.transform.FindChild (length.ToString ()).gameObject;
		starContent.SetActive (true);
		
		float flayTime = 0.3f;
		float delay = 0.1f;
		for (int i = 0; i < listLife.Count; i++) {
			int index = i >= 5 ? i - 5 : i;
			CharacterData data = listLife [i];
			GameObject star = starContent.transform.FindChild (index.ToString ()).gameObject;
			GameObject effect = EffectManager.Instance.CreateEffect (star.transform.parent, "Effect/UiEffect/battleWin_StarFly").gameObject;
			effect.transform.localPosition = star.transform.localPosition;
			effect.transform.localScale = Vector3.zero;
			
			ArrayList paramList = new ArrayList ();
			paramList.Add (listLife);
			paramList.Add (i);
			paramList.Add (starContent);
			paramList.Add (star);
			paramList.Add (effect);
			
			Vector3 from = data.characterCtrl.transform.position;
			from = BattleManager.Instance.BattleCamera.WorldToViewportPoint (from);
			from.z = 0;
			from = UiManager.Instance.gameCamera.ViewportToWorldPoint (from);
			iTween.MoveFrom (effect, iTween.Hash ("x", from.x, "y", from.y, "time", flayTime, "easetype", "linear", "oncompletetarget", gameObject, "oncomplete", "OnStarMoveCompleted", "oncompleteparams", paramList, "delay", i * delay,
			                                      "onstart", "OnStarMoveStart", "onstarttarget", gameObject, "onstartparams", effect));
		}
		return (0.4f + delay) * listLife.Count;
	}
	
	//当星星开始移动时,恢复星星状态
	private void OnStarMoveStart (GameObject effect)
	{
		effect.transform.localScale = Vector3.one;
	}
	
	//当星星飞行完成时播放爆炸特效,显示星星
	private void OnStarMoveCompleted (ArrayList paramList)
	{
		List<CharacterData> listLife = paramList [0] as List<CharacterData>;
		int index = (int)paramList [1];
		GameObject starContent = paramList [2] as GameObject;
		GameObject star = paramList [3] as GameObject;
		GameObject effect = paramList [4] as GameObject;
		Destroy (effect);
		if (index >= 5)
			star.GetComponent<UISprite> ().spriteName = "star_2";
		star.transform.localScale = new Vector3 (0, 0, 1);
		iTween.ScaleTo (star, iTween.Hash ("x", 1, "y", 1, "time", 0.2f, "easetype", "linear", "delay", 0.2f));
		EffectManager.Instance.CreateEffect (star.transform, "Effect/UiEffect/Starburst");
		
	}
	
	/* properties */
	/// <summary>
	/// 战斗失败选择的强化按钮(临时用) 
	/// </summary>
	public string ChooseStrengButton {
		get { return chooseStrengButton; }
		set { chooseStrengButton = value; }
	}

	/// <summary>
	/// 清空怒气信息，停止粒子效果播放，重置怒气条
	/// </summary>
	public void clearAnger(){
		playerBarData = 0f;
		enemyBarData = 0f;
		playerMonsterBar.barSize = 0f;
		enemyMonsterBar.barSize = 0f;
	}

	// show末日决战奖励界面//
	public IEnumerator showLastBattleAward ()
	{
		yield return new WaitForSeconds (2f);
		hideOther();
		UiManager.Instance.openDialogWindow<LastBattleAwardWindow>((win) => {
			win.init(BattleManager.Instance.battleAwards, () => {
				UiManager.Instance.switchWindow<EmptyWindow>((window) => {
					ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outLastBattleWindow);
				});
				BattleManager.battleData.isLastBattle = false;
			});
		});
	}
}
