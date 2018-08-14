using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 战斗管理器 
  * @author 李程
  * */ 

public class  BattleManager: MonoBase,IResCacher
{
	public 	const  float WAITTIME = 0.6f;//战斗片段间隔等待时间;

	public 	static  Vector3 SACLEOF10V10 = new Vector3 (0.8f, 0.8f, 0.8f);
	public 	static  Vector3 SACLEOFNPC = new Vector3 (1.2f, 1.2f, 1.2f);
	public 	static  Vector3 SACLEOF3V3 = new Vector3 (1.2f, 1.2f, 1.2f);
	public	static BattleDataErlang battleData;
	private static BattleManager _instance;
	public 	static bool  isWaitForBattleData = false;
	public static MissionEventSample lastMissionEvent;//刚副本中的event;
	public static long battleSid = 0;//副本战斗的唯一ID(当前副本+点索引)
	public float battleInfoWaitTime = WAITTIME;
	public	BattleTeamManager playerTeam;
	public	BattleTeamManager enemyTeam;
	List<CharacterData> actionCharacterList;
	List<BattleInfo> battleInfoList; // 战斗播放片段信息列表
	public Transform battleFieldRoom;
	public Transform playerSide;
	public Transform enemySide;
	public Camera  BattleCamera;
	public Camera  EffectCamera;
	//public GameObject  BattleBackGround;
	public bool isBattleBegin = false;
	public Award[]  battleAwards;
	public int  activeInfoIndex = 0;
	public int activeClipIndex = 0;
	//public List<CharacterData> levelUpList;
	// Use this for initialization
	public Transform npcPoint;
	public Transform nvshenPoint;
	/** 场景特效父节点 */
	public Transform screenEffectPoint;
	public bool npcShowTime;//进入npc上场模式
	/** 战斗胜利失败动画缓存特效资源 */
	public Dictionary<string ,ResourcesData> battleCacheRes;
	public int talkSid; //下回合对话sid
	public bool isCameraScaleNow = false;//是否镜头在动作中
	public bool effectExit;//是否播放新手特效退出战斗
	public int effectSid;//播放特效的卡片
	bool hasAward;//是否后台已经发奖

	public static BattleManager Instance {
		get {
			if (_instance == null) {
				GameObject tmp = GameObject.Find ("3Dscreen/root");
				if (tmp != null)
					_instance = tmp.GetComponent<BattleManager> ();
			}
			return _instance;
		}
		set {
			if (_instance == null)
				_instance = value;
		}
	}


	
	/** 清理缓存的战斗资源 */
	public void cleanCacheBattleRes (string Path)
	{
		if (battleCacheRes == null || !battleCacheRes.ContainsKey (Path))
			return;
		ResourcesManager.Instance.UnloadAssetBundleBlock (battleCacheRes [Path], false, "effect");
		battleCacheRes.Remove (Path);
	}

	public void addCardToActionCharacters (CharacterData data)
	{
		actionCharacterList.Add (data);
	}

	public List<CharacterData> getActionCharacters ()
	{
		return actionCharacterList;
	}
	
	public BattleInfo getActiveBattleInfo ()
	{ 
		return battleInfoList [activeInfoIndex];
	}
	
	public void nextClip ()
	{
		activeClipIndex += 1;
		activeInfoIndex = 0;
		if (activeClipIndex > battleData.battleClip.Count) {
			Debug.LogError ("clip over");
			return;
		} 
		loadInfoFromClip (activeClipIndex); 
	}

	public  void nextBattle ()
	{ 
		StartCoroutine (nextBattleDelay ());  
	}

	public  void changeBackGroundColor (Color color)
	{ 
	 
//		iTween.ValueTo (gameObject, iTween.Hash ("from", UiManager.Instance.backGround.ImagePanel .renderer.material.GetColor ("_color"), "to", color, "onupdate", "colorChange", "oncomplete", "colorChangeOver", "time", 0.5f));
	}
	
	void colorChangeOver ()
	{
		
		 
	}
	//战斗完成出口,战败选择强化不走这里
	public void awardFinfish ()
	{
		//战斗回去不显示进度
		LoadingWindow.isShowProgress = false;


		if (UiManager.Instance.battleWindow != null)
			UiManager.Instance.battleWindow.hideOther ();

		//世界首领
		if (WorldBossManagerment.Instance.isAttackBoss) {
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
				ScreenManager.Instance.loadScreen (1, null, () => {
					UiManager.Instance.switchWindow<GuildAltarWindow> ((winI) => {
						winI.initWindow ();
						WorldBossManagerment.Instance.isAttackBoss = false;
					});
				});
			});
		}
		//公会首领
		if (GuildManagerment.Instance.isGuildBattle) {
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
				ScreenManager.Instance.loadScreen (1, null, GameManager.Instance.outGuildBattle);
			});
		}
		/** 公会战 */
		else if (battleData.isGuildFight) {
			LoadingWindow.isShowProgress = false;
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {				
				ScreenManager.Instance.loadScreen (1, null, () => {
					UiManager.Instance.BackToWindow<GuildAreaWindow> ();
				});
			});
        }
            //天梯
        else if (battleData.isLadders) {
            LoadingWindow.isShowProgress = false;
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {

                ScreenManager.Instance.loadScreen(1, null, () => {
                    if (battleData.isLaddersRecord) {
                        //重播处理
                    } else {
                        if (BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP) {
                            LaddersManagement.Instance.CurrentOppPlayer.isDefeated = true;
                            LaddersManagement.Instance.M_updateChestStatus();
                        }
                    }
                    //UiManager.Instance.emptyWindow.finishWindow();
                    UiManager.Instance.BackToWindow<LaddersWindow>().init();
                });
            });
            //GameManager.Instance.outLadderBattle
        }
            //讨伐首领
        else if (FuBenManagerment.Instance.isWarAttackBoss) {
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outWarBattle);
            });
        } else if (FuBenManagerment.Instance.isWarActiveFuben) {
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outActiveBattle);
            });
        }
            //英雄之章
        else if (HeroRoadManagerment.Instance.isCurrentDirectFight()) {
            //没副本界面直接进战斗的情况
            if (BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP) {
                HeroRoadManagerment.Instance.currentHeroRoad.conquestCount++;
            }
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outHeroRoadBattle);
            });


        }
            //竞技场
        else if (battleData.isArenaMass || battleData.isArenaFinal) {
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outArenaBattle);
            });
        }
            //扫荡PVP
        else if (BattleGlobal.pvpMode == EnumPvp.sweep) {
            BattleGlobal.pvpMode = EnumPvp.nomal;
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outSweepPvpBattle);
            });
        }
            //抢矿结束
        else if (battleData.isMineralFight) {
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outMiningWindow);
            });
        }
            //小组赛战斗结束
        else if (battleData.isGodsWarGroupFight) {
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outGodsWarWindow);
            });
        }
            //淘汰赛战斗结束
        else if (battleData.isGodsWarFinal) {
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outGodsWarWindow);
            });
        } else if (battleData.isGMFight) {
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(1, null, () => { UiManager.Instance.CurrentWindow.finishWindow(); });
            });
		} else if (battleData.isLastBattle) {
			UiManager.Instance.switchWindow<EmptyWindow>((win) => {
				ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outLastBattleWindow);
			});
		}else {
            //约定:回副本前先用emptyWindow替换battleWindow,避免窗口遗留
            UiManager.Instance.switchWindow<EmptyWindow>((win) => {
                ScreenManager.Instance.loadScreen(4, null, () => {
                    UiManager.Instance.switchWindow<MissionMainWindow>();
                });
            });
        }
		ResourcesManager.Instance.UnloadAssetBundleList ("battleEffect", false);
		ResourcesManager.Instance.UnloadAssetBundleList ("texture", false);
	}

	public void backMainWindow ()
	{
		ScreenManager.Instance.loadScreen (1, null, null);
	}
	 
	public void battleOver ()
	{
		LoadingWindow.isShowProgress = false;
		//标记这轮pvp战斗已经回访
		PvpInfoManagerment.Instance.isCurrentRoundBattlePlayed = true;

        if (battleData.isOneOnOneBossFight) {
            AttackBossOneOnOneManager.Instance.selectedCard = null;
            AwardManagerment.Instance.addFunc(AwardManagerment.BOSS_INFO_AWARD, setAwardCallBack);
            return;
        }
		if(battleData.isLastBattleBossBattle)
		{
			AwardManagerment.Instance.addFunc(AwardManagerment.LASTBATTLE_BOSS_AWARD, setAwardCallBack);
			return;
		}
		if(battleData.isLastBattle)
		{
			AwardManagerment.Instance.addFunc(AwardManagerment.LASTBATTLE_AWARD, setAwardCallBack);
			return;
		}
		if (battleData.isArenaFinal || battleData.isLadders || battleData.isGuide || battleData.isGuildBoss || battleData.isGuildFight ||
            battleData.isMineralFightRecord || battleData.isGMFight || battleData.isGodsWarFinal) {
			UiManager.Instance.battleWindow.StartCoroutine (UiManager.Instance.battleWindow.beginBattleOverAnim ());
			return;
		}
		//讨伐BOSS特殊流程
		if (FuBenManagerment.Instance.isWarAttackBoss || FuBenManagerment.Instance.isWarActiveFuben) {
			//战败没奖励，直接结束战斗
			if (battleData.winnerID == TeamInfo.ENEMY_CAMP) {
				UiManager.Instance.battleWindow.StartCoroutine (UiManager.Instance.battleWindow.beginBattleOverAnim ());
			} else {
				AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_BATTLE, setAwardCallBack);
			}
			return;
		}

		//抢矿
		if (FuBenManagerment.Instance.isMineralAttack) {
			//战败没奖励，直接结束战斗
			if (battleData.winnerID == TeamInfo.ENEMY_CAMP) {
				UiManager.Instance.battleWindow.StartCoroutine (UiManager.Instance.battleWindow.beginBattleOverAnim ());
			} else {
				AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_MINERAL_WAR, setMineralAwrad);
			}
			return;
		}
		//神战小组赛
		if (FuBenManagerment.Instance.isGodsWarGroup) {
			//战败没奖励，直接结束战斗
//			if (battleData.winnerID == TeamInfo.ENEMY_CAMP) {
//				UiManager.Instance.battleWindow.StartCoroutine (UiManager.Instance.battleWindow.beginBattleOverAnim ());
//			} else {
			AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_GODSWAR_GROUP, setAwardCallBack);//无论战败与否都有奖励
//			}
			return;
		}

		//修炼副本特殊流程
		if (MissionInfoManager.Instance.mission != null) {

			if (MissionInfoManager.Instance.mission.getChapterType () == ChapterType.PRACTICE && !battleData.isHeroRoad) {
				BattleGlobal.isBackFromBattle = true;
				battleData.isPractice = true;
				BattleGlobal.isPracticeWarWin = battleData.winnerID == TeamInfo.OWN_CAMP;
				//修炼副本 战败后有奖励
				if (battleData.winnerID == TeamInfo.ENEMY_CAMP) {
					AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_BATTLE, (awards) => {
						MissionAward.Instance.parcticeAwards = awards;
						UiManager.Instance.battleWindow.StartCoroutine (UiManager.Instance.battleWindow.beginBattleOverAnim ());	
					}); 	
				} else {
					UiManager.Instance.battleWindow.StartCoroutine (UiManager.Instance.battleWindow.beginBattleOverAnim ());	
				}
				return;
			}
		}

		if (battleData.winnerID == TeamInfo.ENEMY_CAMP) {		
			if (battleData.isPvP == true) {
				//	enemyTeam.hideAllParter (); 
				PvpInfoManagerment.Instance.clearDate (); //战败后清理pvp
				AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_BATTLE, setAwardCallBack);
            } else {
				UiManager.Instance.battleWindow .StartCoroutine (UiManager.Instance.battleWindow .beginBattleOverAnim ());		
			}
        } else {
			if (MissionInfoManager.Instance.mission != null && MissionInfoManager.Instance.mission.getChapterType() == ChapterType.TOWER_FUBEN && !battleData.isHeroRoad) {
                //UiManager.Instance.battleWindow.StartCoroutine(UiManager.Instance.battleWindow.beginBattleOverAnim());
                AwardManagerment.Instance.addFunc(AwardManagerment.AWARDS_BATTLE, setAwardCallBack);
            } else {
                addFunc();
            }
        }
	}

	private void addFunc ()
	{
		
		AwardManagerment.Instance.addFunc (HeroRoadManagerment.Instance.isCurrentDirectFight () ? AwardManagerment.AWARDS_HERO_ROAD : AwardManagerment.AWARDS_BATTLE, setAwardCallBack);

	}
	
	public void  playerExpAdd (Award battleAward)
	{ 
		if (battleAward.exps == null)
			return;
		
		for (int i=0; i<  battleAward.exps.Count; i++) {	 
			foreach (CharacterData each in  BattleManager.Instance.actionCharacterList) {
				if (each.storeID == battleAward.exps [i].id && each.isGuardianForce == false && each.camp == TeamInfo.OWN_CAMP) {

					Card targetCard = battleAward.exps [i].cardLevelUpData.levelInfo.orgData as Card;
					int lv = battleAward.exps [i].cardLevelUpData.levelInfo.newLevel - battleAward.exps [i].cardLevelUpData.levelInfo.oldLevel;
					int combat = battleAward.exps [i].cardLevelUpData.levelInfo.newCardCombat - battleAward.exps [i].cardLevelUpData.levelInfo.oldCardCombat;
					EffectManager.Instance.CreateGetExpLabel (2, each.characterCtrl, battleAward.exps [i].expGap, lv, combat);

					//如果update后等级提升，则放效果
					if (lv > 0) {
						cardLevelUp (battleAward.exps [i], each);
					}
					
					break;
				}
			}
			
		}
	}

	void	cardLevelUp (EXPAward exp, CharacterData role)
	{
		EffectManager.Instance.CreateEffect (role.characterCtrl.hitPoint, "Effect/Single/shengji");
	}

	public bool  GuardianForceExpAdd (CharacterData guardianForce, EXPAward expAward)
	{
		if (guardianForce == null)
			return false;	
		//返回是否召唤兽升级
		bool levelup = false;
		int lv = expAward.cardLevelUpData.levelInfo.newLevel - expAward.cardLevelUpData.levelInfo.oldLevel;
		int combat = expAward.cardLevelUpData.levelInfo.newCardCombat - expAward.cardLevelUpData.levelInfo.oldCardCombat;
		EffectManager.Instance.CreateGetExpLabel (1, guardianForce.characterCtrl, expAward.expGap, lv, combat);
		//如果update后等级提升，则放效果
		if (lv > 0) {
			levelup = true;
			cardLevelUp (expAward, guardianForce);
		} 
		return levelup;  
	}

	public EXPAward getEXPAwardByStoreID (CharacterData guardianForce, Award battleAward)
	{
		if (guardianForce == null || battleAward == null || battleAward.exps == null)
			return null;
		for (int i=0; i<  battleAward.exps.Count; i++) {			
			if (guardianForce.storeID == battleAward.exps [i].id) {
				return battleAward.exps [i];
			}
		}
		return null;
	}
	
	void colorChange (Color _color)
	{ 
		UiManager.Instance.backGround.ImagePanel .renderer.material.SetColor ("_color", _color); 
	}

	IEnumerator nextBattleDelay ()
	{
		if (_stop) {
			_stopCallback ();
			yield break;
		}

		yield return new WaitForSeconds (battleInfoWaitTime);
		
		battleInfoWaitTime = WAITTIME;
		activeInfoIndex += 1;
		
		if (activeInfoIndex >= battleData.battleClip [activeClipIndex].battleInfo.Count) {
			//如果达到当前的info条目极限，则下回合
			if (activeInfoIndex >= battleData.battleClip [activeClipIndex].battleInfo.Count) {
				//下个片段
				nextClip ();
			}
		}
		 
		if (battleData.battleClip [activeClipIndex].battleInfo == null || battleData.battleClip [activeClipIndex].battleInfo.Count == 0)
			yield break;
		//下个info;
		battleInfoList [activeInfoIndex].Play ();
	 	
	}

	private bool _stop;
	private CallBack _stopCallback;

	public void Stop (CallBack stopCallback)
	{
		_stop = true;
		this._stopCallback = stopCallback;
	}

	private bool StopIfSkip ()
	{
		if (_stop) {
			_stop = false;
			_stopCallback ();
			return true;
		}
		return false;
	}

	public Vector3 getOffset (Vector3 newPosition, bool isPlayerTeam)
	{
		Vector3 positionOffset;
		if (isPlayerTeam) {
	
			positionOffset = new Vector3 (newPosition.x, newPosition.y + 0.2f, newPosition.z + 0.2f);
		} else {

			positionOffset = new Vector3 (newPosition.x, newPosition.y + 0.2f, newPosition.z - 0.2f);
		}
		return positionOffset;
	}

	void Awake ()
	{
		BattleManager.Instance = this;
	}

	void Start ()
	{
		//UiManager.Instance.effectCamera.gameObject.SetActive(true);
		battleFieldRoom = GameObject.Find ("3Dscreen/root").transform;
		playerSide = GameObject.Find ("3Dscreen/root/playerSide").transform;
		enemySide = GameObject.Find ("3Dscreen/root/enemySide").transform;
		BattleCamera = GameObject.Find ("3Dscreen/root/Camera").GetComponent<Camera> ();
		EffectCamera = GameObject.Find ("3Dscreen/root/Camera/effectCamera").GetComponent<Camera> ();



		npcPoint = GameObject.Find ("3Dscreen/root/npcPoint").transform;
		nvshenPoint = GameObject.Find ("3Dscreen/root/nvshenPoint").transform;
		screenEffectPoint = GameObject.Find ("3Dscreen/root/screenEffectPoint").transform;
		BuffManager.Instance.init (); 
		float orthographicSize = (float)Screen.height / (float)Screen.width / 1.5f;
		if (orthographicSize < 1)
			orthographicSize = 1;
		BattleCamera.orthographicSize = orthographicSize;
		EffectCamera.orthographicSize = orthographicSize;		

		//注入获得奖励回调函数  暂时默认为pve
        
		if (ResourcesManager.Instance.allowLoadFromRes)
			BattleManager.Instance.StartCoroutine (BattleManager.Instance.battleStart ());
		else
			BattleManager.Instance.StartCoroutine (BattleManager.Instance.cacheData ()); 


	}
	
	//设置战后奖励
	private void setAwardCallBack (Award[] award)
	{
		battleAwards = award;
		if (battleAwards == null) {
			awardFinfish ();	
			return;
		}

		UiManager.Instance.battleWindow.StartCoroutine (UiManager.Instance.battleWindow .beginBattleOverAnim ());
	}

	//设置战后奖励
	private void setMineralAwrad (Award[] award)
	{
		MiningManagement.Instance.FightPrizes = AllAwardViewManagerment.Instance.exchangeAwards (award);
		UiManager.Instance.battleWindow.StartCoroutine (Utils.DelayRun (() => {
			awardFinfish ();
		}, 1f));
	}

	public void skipBattle ()
	{
		
		List<CharacterData> ctrlList = getActionCharacters ();
		foreach (CharacterData characterData in ctrlList) {
			if (battleData.hpMap.ContainsKey (characterData.serverID)) {
				if (characterData == null || characterData.characterCtrl == null || characterData.characterCtrl.HpBar == null)
					continue;
				BattleHpInfo info = battleData.hpMap [characterData.serverID];
				int hp = Mathf.Max (characterData.fixedServer_hp + info.hp, 0);
				characterData.server_hp = hp;
				characterData.characterCtrl.HpBar.updateValue (characterData.server_hp, characterData.server_max_hp);
				characterData.characterCtrl.HpBar.lockBar ();

				//替补战中 主力死了激活替补
				//battleData.battleType =BattleType.BATTLE_SUBSTITUTE;
				if (characterData.server_hp <= 0 && battleData.battleType == BattleType.BATTLE_SUBSTITUTE)
					switchPlayerInSkillBattle (characterData);
				

			}
		}
		battleOver ();
	}

	void switchPlayerInSkillBattle (CharacterData characterData)
	{
		//队伍管理器里的队伍逻辑数据中找出对应替补
		BattleTeamManager teamM = characterData.parentTeam;
		TeamInfoPlayer sub = teamM.teamInfo.getSubstitute (characterData.TeamPosIndex);
		//替补战的情况,主力死了换替补上来显示
		if (sub != null) {
			//主力下场
			characterData.characterCtrl.outBattleField ();
			teamM.players.Remove (characterData);

			for(int i=0;i<teamM.players.Count;i++)
			{
				if(teamM.players[i].serverID == sub.id)
				{
					return;
				}
			}
			//创建替补的战斗用数据
			CharacterData subData = BattleCharacterManager.Instance.CreateBattleCharacterData (sub, teamM, BattleManager.battleData.battleType);
			teamM.players.Add (subData);
			teamM.CreateCharacterInstance (subData);//创建图像实例

			//计算新上来的替补的最终血量
			if (battleData.hpMap != null && subData != null && battleData.hpMap.ContainsKey (subData.serverID)) {
				BattleHpInfo info = battleData.hpMap [subData.serverID];
				int hp = Mathf.Max (subData.fixedServer_hp + info.hp, 0);
				subData.server_hp = hp;
				subData.characterCtrl.HpBar.updateValue (subData.server_hp, subData.server_max_hp);
				subData.characterCtrl.HpBar.lockBar ();
			}
				
		}

	}

	public IEnumerator cacheData ()
	{

		List<string> resList = new List<string> ();
		//	ListKit.AddRange (resList, list);

	 
		CallBack<string> addIfNeed = (str) =>
		{
			if (!string.IsNullOrEmpty (str) && !resList.Contains (str)) {
				resList.Add (str);
			}
		};

		if (FuBenManagerment.Instance.isWarAttackBoss || WorldBossManagerment.Instance.isAttackBoss) {
			addIfNeed ("Effect/Other/BossScene");
		}

		foreach (BattleClipErlang clip in battleData.battleClip) {
			foreach (BattleInfoErlang info in clip.battleInfo) {
				foreach (BattleSkillErlang skill in info.battleSkill) {
					int sid = skill.skillMsg.skillSID;
					
					if (SkillSampleManager.Instance.data.ContainsKey (sid)) {

						addIfNeed (SkillManagerment.Instance.getDamageEffect (sid));
						addIfNeed (SkillManagerment.Instance.getSpellEffect (sid));
						addIfNeed (SkillManagerment.Instance.getSkillBulletPerfab (sid));
						addIfNeed (SkillManagerment.Instance.getAroundEffectPerfab (sid));
					
						int buf = SkillSampleManager.Instance.getSkillSampleBySid (sid).buffSid;
						if (BuffSampleManager.Instance.data.ContainsKey (buf)) {
							addIfNeed (BuffManagerment.Instance.getDurationEffect (buf));
							addIfNeed (BuffManagerment.Instance.getResPath (buf));
							addIfNeed (BuffManagerment.Instance.getDurationEffectForBoss (buf));
						}

					} else if (BuffSampleManager.Instance.data.ContainsKey (sid)) {
						addIfNeed (BuffManagerment.Instance.getDurationEffect (sid));
						addIfNeed (BuffManagerment.Instance.getResPath (sid));
						addIfNeed (BuffManagerment.Instance.getDurationEffectForBoss (sid));
					}

				}
				
			}
		}
	 

		//	list = resList.ToArray ();

		ResourcesManager.Instance.cacheData (resList.ToArray (), (cacheList) => {
			StartCoroutine (battleStart ());
		}, "battleEffect");
		yield break;
	}

	public IEnumerator battleStart ()
	{
		//BattleBackGround = Create3Dobj ("Effect/Other/3D_background").obj.transform.GetChild (0).gameObject;
		//	BattleManager.isWaitForBattleData = false;
		LoadBattleData ();
		yield return new WaitForSeconds (1.5f);
		ResourcesManager.Instance.cacheProgress = 1f;


		while (UiManager.Instance.battleWindow==null)
			yield return 1;
		
		BattleManager.isWaitForBattleData = false;
		//等待切屏完成
		AudioManager.Instance.PlayAudio (109);
		if (StopIfSkip ())
			yield break;
		//remove some nouse window;
		UiManager.Instance.destoryWindowByName ("pvpCupFightWindow");

		if (StopIfSkip ())
			yield break;

		// 末日决战挑战小怪或boss战//
		if(BattleManager.battleData.isLastBattle || BattleManager.battleData.isLastBattleBossBattle)
		{
			string add = "x" + (1 + LastBattleManagement.Instance.nvBlessLv * CommandConfigManager.Instance.lastBattleData.nvShenAdd);
			UiManager.Instance.battleWindow.playNvShenBlessBuff();
			foreach (CharacterData each in playerTeam.players) {
				EffectManager.Instance.CreateEffect (each.characterCtrl.hitPoint, "Effect/Single/fullForceAttack");
				//EffectManager.Instance.CreateBuffNumText (each.characterCtrl.transform, add, 0.1f);
				EffectManager.Instance.CreateBuffNumTextForLastBattle(each.characterCtrl.transform, add, 0.1f);
			}
			yield return new WaitForSeconds (1f);
		}

		if (BattleManager.battleData.battleType == BattleType.BATTLE_TEN || BattleManager.battleData.battleType == BattleType.BATTLE_SUBSTITUTE) {
			
			if (UiManager.Instance.battleWindow != null) {
				UiManager.Instance.battleWindow .playBattleTypeBar ();
			}
			yield return new WaitForSeconds (2.4f);
			
		}
        
		if (StopIfSkip ())
			yield break;

		// pvp全力一击
		if (BattleManager.battleData.pvpType == 2 && UiManager.Instance.battleWindow != null) {
			UiManager.Instance.battleWindow .playMaxPowerHitBuff ();
			foreach (CharacterData each in playerTeam.players) {
				EffectManager.Instance.CreateEffect (each.characterCtrl.hitPoint, "Effect/Single/fullForceAttack");
				EffectManager.Instance.CreateBuffNumText (each.characterCtrl.transform, "x1.5", 0.1f);
			}
			yield return new WaitForSeconds (1.5f);
		}

		if (StopIfSkip ())
			yield break;

		//召唤兽进场
		float BeastTime = 0;
        if (playerTeam.GuardianForce != null && UiManager.Instance.battleWindow != null) {
			playerTeam.PlayMonsterBuff ();
			EffectManager.Instance.CreateEffect (playerTeam.TeamHitPoint, "Effect/AOE/beastbuffAOE");

			foreach (CharacterData each in playerTeam.players) {
				EffectManager.Instance.CreateEffect (each.characterCtrl.hitPoint, "Effect/Single/beastbuff");
			}
			BeastTime = 2f;
		}
        if (enemyTeam.GuardianForce != null && UiManager.Instance.battleWindow != null) {
            enemyTeam.PlayMonsterBuff();
            EffectManager.Instance.CreateEffect(enemyTeam.TeamHitPoint, "Effect/AOE/beastbuffAOE");

            foreach (CharacterData each in enemyTeam.players) {
                EffectManager.Instance.CreateEffect(each.characterCtrl.hitPoint, "Effect/Single/beastbuff");
            }
            BeastTime = 2f;

        }

		
		yield return new WaitForSeconds (0.5f + BeastTime);
        
		if (StopIfSkip ())
			yield break;

		battleInfoList [activeInfoIndex].Play ();
		isBattleBegin = true;
	

	}

	void cameraSacle (float data)
	{
		BattleManager.Instance.EffectCamera.orthographicSize = data;
		BattleManager.Instance.BattleCamera.orthographicSize = data;
	}

	public void	shakeCamera ()
	{
		iTween.ShakePosition (BattleCamera.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
		iTween.ShakePosition (UiManager.Instance.backGround .gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));	
	}

	public void	cameraLookAt (Vector3 pos, float time, SkillCtrl skill)
	{
		
		if (isCameraScaleNow == true)
			return;
		
		isCameraScaleNow = true;
		float returnTime = time * 0.2f;
		float delay = time - returnTime;
		
		Vector3 oldPos = BattleCamera.transform.position;
		pos = new Vector3 (pos.x, BattleCamera.transform.position.y, pos.z);//忽略传入位置的y轴
		
		iTween.MoveTo (BattleCamera.gameObject, iTween.Hash ("position", pos, "time", returnTime));
		iTween.ValueTo (BattleManager.Instance.gameObject, iTween.Hash ("from", 1, "to", 0.6f, "onupdate", "cameraSacle", "time", returnTime));
		UiManager.Instance.removeAllEffect ();
		EffectManager .Instance.CreateSkillBanner (skill.serverData.sample.getName ());
		
		iTween.MoveTo (BattleCamera.gameObject, iTween.Hash ("delay", delay, "position", oldPos, "time", returnTime));
		iTween.ValueTo (BattleManager.Instance.gameObject, iTween.Hash ("delay", delay, "from", 0.6f, "to", 1, "onupdate", "cameraSacle", "oncomplete", "cameraLookatOver", "time", returnTime));
	}
	
	void cameraLookatOver ()
	{
		isCameraScaleNow = false;
	}

	public List<CharacterData>  getAllBattleCharacterData ()
	{

		return  actionCharacterList;
	}

	/// <summary>
	/// 获取战斗卡片数据类
	/// </summary>
	/// <param name="serverID">战斗角色在战斗中的唯一uid</param>
	public CharacterData  getBattleCharacterData (int serverID)
	{ 
		foreach (CharacterData each in actionCharacterList) {
			if (serverID == each.serverID)
				return each;
		}
		return null;
	}

	/** 设置行动优先级--目前只用于经验,升级,附加经验等的显示优先级 */
	void setActionCharacterList ()
	{
		if (actionCharacterList == null)
			actionCharacterList = new List<CharacterData> ();
		
		foreach (CharacterData each in playerTeam.players) {
			addCardToActionCharacters (each);
		 
		}
		
		if (playerTeam.GuardianForce != null)	
			addCardToActionCharacters (playerTeam.GuardianForce);

		
		foreach (CharacterData each in enemyTeam.players) {
			addCardToActionCharacters (each);
		 
		}	
		
		if (enemyTeam.GuardianForce != null)
			addCardToActionCharacters (enemyTeam.GuardianForce);

		
	}
	
	void LoadBattleData ()
	{
		string path = null;
		//如果是从战斗进新战斗,那么:
		UiManager.Instance.hideWindowByName ("backGroundWindow");
		//判断是否是公会BOSS战，直接进战斗，如果是就某人加载一个战斗地图
        if (GuildManagerment.Instance.isGuildBattle || battleData.isLadders) {
			UiManager.Instance.backGround.switchBackGround ("battleMap_7");
		} else if (HeroRoadManagerment.Instance.isCurrentDirectFight () || battleData.isArenaMass || battleData.isArenaFinal) {
			UiManager.Instance.backGround.switchBackGround ("battleMap_101");
        } else if (FuBenManagerment.Instance.isWarAttackBoss || WorldBossManagerment.Instance.isAttackBoss) {
			UiManager.Instance.backGround.switchBackGround ("battleMap_201");
			path = "Effect/Other/BossScene";
		} else {
			int bgId;
			if (MissionInfoManager.Instance.mission == null || MissionInfoManager.Instance.mission.getPlayerPoint () == null) {
				bgId = 7;
			} else {
				bgId = MissionInfoManager.Instance.mission.getPlayerPoint ().getBattleBg ();
			}
			UiManager.Instance.backGround.switchBackGround ("battleMap_" + bgId);
		}
		addSceneEffect (path);
		changeBackGroundColor (new Color (0.1f, 0.1f, 0.1f, 1f));
		playerTeam = new BattleTeamManager (true, battleData.playerTeamInfo, battleData.battleType);
		enemyTeam = new BattleTeamManager (false, battleData.enemyTeamInfo, battleData.battleType);
	 
		setActionCharacterList ();
		activeClipIndex = 0;

		//读取第一段战斗片段
		loadInfoFromClip (activeClipIndex);
	}

	/// <summary>
	/// 添加场景特效
	/// </summary>
	/// <param name="path">特效路径.</param>
	void addSceneEffect (string path)
	{
		if (string.IsNullOrEmpty (path)) {
			return;
		}
		GameObject obj = MonoBase.Create3Dobj (path).obj;
		obj.transform.parent = screenEffectPoint;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
	}

	public IEnumerator setTimeScale ()
	{
		Time.timeScale = 0.2f;
		yield return new WaitForSeconds (0.3f);
		Time.timeScale = 1;
		
	}
	/// <summary>
	/// 获得当前回合
	/// </summary>
	public int getClipFrame ()
	{
		
		return battleData.battleClip [activeClipIndex].frame;
	}

	void loadInfoFromClip (int index)
	{ 
		//检测回合开始时有没剧情对话
		if (BattleManager.Instance.activeInfoIndex == 0)
			BattleManager.Instance.talkSid = getTurnStartPlotSid (getClipFrame ());

		//战斗结束
		if (battleData.battleClip [index].isWinnerClip) {
			battleOver ();
//]			MonoBase.print ("battle over!!!");
			return;
		}
		
		if (battleInfoList == null)
			battleInfoList = new List<BattleInfo> ();
		else
			battleInfoList.Clear ();
		
		if (index >= battleData.battleClip.Count) {
			//Debug.LogError ("clip over index=" + index + "   battleData.battleClip.Count= " + battleData.battleClip.Count);
			return;
		}
		for (int i=0; i<battleData.battleClip[index].battleInfo.Count; i++) {
			BattleInfoErlang each = battleData.battleClip [index].battleInfo [i];
			if (each.battleSkill.Count != 0) { 
				if (index + 1 < BattleManager.battleData.battleClip.Count && BattleManager.battleData.battleClip [index + 1].isWinnerClip && i == battleData.battleClip [index].battleInfo.Count - 1) {
					battleInfoList.Add (new BattleInfo (each, true));
				} else {
					battleInfoList.Add (new BattleInfo (each, false));
				}
			}
		}
		if (battleInfoList.Count == 0) {
			nextClip ();			
		}
	}

	public IEnumerator   showTalk (int type, bool isStartTalk, CallBack talkOverBack)
	{

		if (BattleManager.Instance.talkSid == 0 || !FuBenManagerment.Instance.isNewMission (ChapterType.STORY, MissionInfoManager.Instance.getMission ().sid)) {
			BattleManager.Instance.battleInfoWaitTime += 1f;
			talkOverBack ();
			yield break;
		}
		
		yield return new WaitForSeconds (0.2f);
		if (MissionInfoManager.Instance.autoGuaji) {
			UiManager.Instance.openDialogWindow<TalkWindow> ((win2) => {
				win2.Initialize (BattleManager.Instance.talkSid, talkOverBack, null, true);
			});
			
		} else {
			UiManager.Instance.openDialogWindow<TalkWindow> ((win2) => {
				win2.Initialize (BattleManager.Instance.talkSid, talkOverBack, null);
			});
		}


	}

	/// <summary>
	/// 获得NPC上场或下场的剧情sid ,index npc触发索引,  isStart 开始或结束时的剧情
	/// </summary>
	public int getNPCPlotSid (int index, bool isStart)
	{
		if (BattleManager.lastMissionEvent == null)
			return 0;
		
//		if (BattleManager.lastMissionEvent .eventType != MissionEventType.FIGHT)
//			return  0;

		Plot p = null;
		if (BattleManager.lastMissionEvent .npcPlots != null && BattleManager.lastMissionEvent .npcPlots.ContainsKey (index)) {
			p = BattleManager.lastMissionEvent .npcPlots [index];
			if (p == null)
				return  0;
		} else {
			return  0;
		}
		//string str = BattleManager.lastMissionEvent.plots [index];
		//每回合的剧情信息
		//string[] nowplots = str.Split (',');
		if (isStart) {
			return p.beginSid;
		} else {
			return p.endSid;
		}
	}
	
	/// <summary>
	/// 获得回合开始的剧情sid
	/// </summary>
	public int getTurnStartPlotSid (int index)
	{
		if (BattleManager.lastMissionEvent == null)
			return 0;
		
//		if (BattleManager.lastMissionEvent .eventType != MissionEventType.FIGHT && BattleManager.lastMissionEvent .eventType != MissionEventType.BOSS )
//			return  0;

		Plot p = null;
		if (BattleManager.lastMissionEvent .countPlots != null && BattleManager.lastMissionEvent .countPlots.ContainsKey (index)) {
			p = BattleManager.lastMissionEvent .countPlots [index];
			//读取后移除防止2次说话(npc上场是自建回合可能导致后面的战斗回合数+1了.再次触发对话)
			BattleManager.lastMissionEvent .countPlots.Remove (index);
			if (p == null)
				return  0;
		} else {
			return  0;
		}
		return p.beginSid;
	}

}
