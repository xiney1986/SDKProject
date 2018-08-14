using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlePrepareWindow : WindowBase
{
	int battleMode;//战斗模式
	public bool isPvP;//谁否是pvp战 
	public bool isLadders;//是否是天梯战
	bool isBossBattle;//是否为boss战
	 
	public BattleFormationCard[] playerFormationData;
	public BattleFormationCard[] enemyFormationData;
	public GameObject player5v5Root;
	public GameObject player10v10Root;
	public GameObject enemy5v5Root;
	public GameObject enemy10v10Root;
	[HideInInspector]
	public GameObject playerFormationObj;
	[HideInInspector]
	public GameObject enemyFormationObj;
	public ButtonTeamPlayerViewInMission buttonBoss;
	public ButtonTeamPlayerViewInMission[] buttonPlayerParter10v10;//10vs10的玩家上场队员5个位置
	public ButtonTeamPlayerViewInMission[] buttonEnemyParter10v10;
	public ButtonTeamPlayerViewInMission[] buttonPlayerParter5v5;
	public ButtonTeamPlayerViewInMission[] buttonEnemyParter5v5;
	public ButtonBase ButtonBattleStart;
	public ButtonTeamPlayerViewInMission[] activePlayerParter;
	public ButtonTeamPlayerViewInMission[] activeEnemyParter;
	FormationSample playerFormationSample;
	FormationSample enemyFormationSample;
	public Army savingArmy ;
	private CallBack doBattle;
	public Army oldArmy;//初始化的队伍
	public List<int> idss;
    public GameObject starPrefab;//星星
//	private CallBack windowBack;
	
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	void openEmptyForLevelNewBack(List<int> ids){
		idss=ids;
		loadFormation ();
		updateParter ();
		MaskWindow.UnlockUI ();
	}
	public int getBattleMode ()
	{
		return battleMode;
	}

	private void saveArmy ()
	{
		//相同直接关闭窗口不保存
		if (ArmyManager.Instance.compareArmy (savingArmy, oldArmy)) {
			saveArmyCallBack ();
			return;
		}  
		List<string> oldCards = ArmyManager.Instance.getAllArmyCardsExceptMining ();//改变队伍前的卡片上阵情况
		List<string> oldBeasts = ArmyManager.Instance.getFightBeasts ();//改变队伍前的召唤兽情况
		Army[] arr = new Army[1]{savingArmy};
		ArmyUpdateFPort port = FPortManager.Instance.getFPort ("ArmyUpdateFPort") as ArmyUpdateFPort;
		port.access (arr, () => {
			saveArmyCallBack ();
			ArmyManager.Instance.updateBackChangeState (oldCards, oldBeasts,1);
		}); 
	}

	void saveArmyCallBack ()
	{
		if (BattleManager.Instance == null) {
			//防止finishWindow时返回missionMainWindow时显示称号和名字等一系列不希望显示的东西
			if(UiManager.Instance.missionMainWindow != null)
				UiManager.Instance.missionMainWindow.isShowEffectRoot = false;
			finishWindow ();
			EventDelegate.Add (onDestroy, () => {
				ArmyManager.Instance.updateArmy (oldArmy.armyid, savingArmy);
				//告诉挂机系统等pvp战报,屏蔽窗口其他操作
				BattleManager.isWaitForBattleData = true;
				GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattle;
				doBattle ();
			});

 
		} else {
			ArmyManager.Instance.updateArmy (oldArmy.armyid, savingArmy);
			GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattle;
			doBattle ();
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonBattleStart") {

			if (isLadders) {
				PvpOppInfo oppInfo = LaddersManagement.Instance.CurrentOppPlayer.playerInfo;
				PvpInfoManagerment.Instance.setLadderPvpOppInfo (oppInfo);

			} else if (isPvP && PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
				UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0420"));
				});
				return;
			}
			saveArmy ();
		} else if (gameObj.name == "close") {

			if (BattleManager.Instance != null)
				BattleManager.Instance.awardFinfish ();
			else
				finishWindow ();

			if (MissionManager.instance != null) {
				MissionManager.instance.showAll ();
				MissionManager.instance.setBackGround ();
			}

			MaskWindow.UnlockUI ();
			if (MissionInfoManager.Instance.mission != null && MissionInfoManager.Instance.mission.getPlayerPoint () != null) {
				MissionInfoManager.Instance.mission.getPlayerPoint ().isBattlePrepared = false;
			}
		}		
	}

	public void switchParter (BattleFormationCard main, BattleFormationCard sub, int index)
	{
		//主卡不空,判断下主卡是否相同
		if (main != null) {
			if (savingArmy.players [index] != main.card.uid) {
				savingArmy.players [index] = main.card.uid;
			}
		} else {
			savingArmy.players [index] = "0";
		} 
	
		//找出替补卡
		if (sub != null) {
			if (savingArmy.alternate [index] != sub.card.uid) {
				savingArmy.alternate [index] = sub.card.uid;
			}
		} else {
			savingArmy.alternate [index] = "0";
		}
 

		//更新主卡的显示
	
		activePlayerParter [index].updateButton (main, sub, ButtonTeamPlayerViewInMission.BATTLEPREPARE, index,false);
 
	
		if (battleMode == BattleType.BATTLE_TEN||battleMode==BattleType.BATTLE_SUBSTITUTE) {
			activePlayerParter [index + 5].updateButton (sub, main, ButtonTeamPlayerViewInMission.BATTLEPREPARE, index,true);
 
		}
		
		
	}
	/// <summary>
	/// 战斗类型10人或替补之类  是否pvp 是否boss战
	/// </summary>
	public void Initialize (int mode, bool isPvP, bool isBossBattle, CallBack battleCallback)
	{
		if (isPvP) {
			savingArmy = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (3));
			oldArmy = ArmyManager.Instance.getArmy (3);
		} else {
			savingArmy = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (1));
			oldArmy = ArmyManager.Instance.getArmy (1);
		}

		this.isPvP = isPvP;
		this.doBattle = battleCallback;
		battleMode = mode; 
		this.isBossBattle = isBossBattle;
		
		if (isPvP) {
			playerFormationData = PvpInfoManagerment.Instance.getUserBFCards ();
			enemyFormationData = PvpInfoManagerment.Instance.getBattleFormationCards ();
		} else {
			playerFormationData = MissionInfoManager.Instance.mission.mine;
			enemyFormationData = MissionInfoManager.Instance.mission.enemy;	
		}
		
		if (battleMode == BattleType.BATTLE_FIVE) {
			player10v10Root.SetActive (false);
			enemy10v10Root.SetActive (false);
			buttonBoss.gameObject.SetActive (false);
			player5v5Root.SetActive (true);
			enemy5v5Root.SetActive (true);
			
			activePlayerParter = buttonPlayerParter5v5;
			activeEnemyParter = buttonEnemyParter5v5;
		}else if(battleMode == BattleType.BATTLE_SUBSTITUTE){
			player5v5Root.SetActive (false);
			enemy5v5Root.SetActive (false);
			buttonBoss.gameObject.SetActive (false);
			player10v10Root.SetActive (true);
			enemy10v10Root.SetActive (true);
			activePlayerParter = buttonPlayerParter10v10;
			activeEnemyParter = buttonEnemyParter10v10;
		}
		else if (battleMode == BattleType.BATTLE_FIVE  && isBossBattle) {
			player10v10Root.SetActive (false);
			enemy10v10Root.SetActive (false);
			enemy5v5Root.SetActive (false);
			buttonBoss.gameObject.SetActive (true);
			player5v5Root.SetActive (true);
			activePlayerParter = buttonPlayerParter5v5;
		}else if(battleMode == BattleType.BATTLE_SUBSTITUTE&&isBossBattle){
			player5v5Root.SetActive (false);
			enemy5v5Root.SetActive (false);
			enemy10v10Root.SetActive (false);
			buttonBoss.gameObject.SetActive (true);
			player10v10Root.SetActive (true);
			
			activePlayerParter = buttonPlayerParter10v10;
		}
		else if (battleMode == BattleType.BATTLE_TEN && !isBossBattle) {
			player5v5Root.SetActive (false);
			enemy5v5Root.SetActive (false);
			buttonBoss.gameObject.SetActive (false);
			player10v10Root.SetActive (true);
			enemy10v10Root.SetActive (true);	
			activePlayerParter = buttonPlayerParter10v10;
			activeEnemyParter = buttonEnemyParter10v10;
		} else if (battleMode == BattleType.BATTLE_TEN && isBossBattle) {
			player5v5Root.SetActive (false);
			enemy5v5Root.SetActive (false);
			enemy10v10Root.SetActive (false);
			buttonBoss.gameObject.SetActive (true);
			player10v10Root.SetActive (true);
			activePlayerParter = buttonPlayerParter10v10;
		}	
		
		//更新战斗按钮文字
		if (battleMode == BattleType.BATTLE_SUBSTITUTE) {
			ButtonBattleStart.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0186");	
		} else if (battleMode == BattleType.BATTLE_TEN) {
			//只有10v10人才有资格叫 大乱斗
			ButtonBattleStart.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0191");	
		} else {
			ButtonBattleStart.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0187");	
		}
		TeamEmtpyInfoFPort fport = FPortManager.Instance.getFPort<TeamEmtpyInfoFPort> ();
		fport.access (openEmptyForLevelNewBack);

		
	}
	public void autoBegin(){
		if(MissionInfoManager.Instance.autoGuaji&&!MissionInfoManager.Instance.mSettings[1]){
			if (isLadders) {
				PvpOppInfo oppInfo = LaddersManagement.Instance.CurrentOppPlayer.playerInfo;
				PvpInfoManagerment.Instance.setLadderPvpOppInfo (oppInfo);
				
			} else if (isPvP && PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
				UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0420"));
				});
				return;
			}
			saveArmy ();
		}else if(MissionInfoManager.Instance.autoGuaji&&MissionInfoManager.Instance.mSettings[1]){
			MissionInfoManager.Instance.autoGuaji=false;
		}
	}
	//取得对应屏幕位置
	public void setPosition (int index, bool isplayer)
	{
		//得到对应点位
		Transform point = null;
		ButtonTeamPlayerViewInMission button = null;
		if (isplayer) {
			button = activePlayerParter [index];
			if (getBattleMode () == BattleType.BATTLE_FIVE || getBattleMode () == BattleType.BATTLE_SUBSTITUTE) {
				point = playerFormationObj.transform.FindChild ((FormationManagerment.Instance.getLoctionByIndex (oldArmy.formationID, index)).ToString ());
			} else {
				point = playerFormationObj.transform.FindChild ((index + 1).ToString ());
			}
		} else {
			button = activeEnemyParter [index];
			if (getBattleMode () == BattleType.BATTLE_FIVE || getBattleMode () == BattleType.BATTLE_SUBSTITUTE)
				point = enemyFormationObj.transform.FindChild (enemyFormationData [index] .loc.ToString ());
			else
				point = enemyFormationObj.transform.FindChild ((index + 1).ToString ());
		}
		//button.transform.position = new Vector3 (point.position.x, point.position.y, button.transform.position.z);
	}
	
	
	//更新队员信息
	void updateParter ()
	{
		
		//我方信息更新
		for (int i=0; i< activePlayerParter.Length; i++) {
			//当3人或者4人阵,移除一些座位
			if (oldArmy.formationID == 1) {
				if (i == 0 || i == 4 || i == 5 || i == 9) {
					continue;
				}
			}
			if (oldArmy.formationID == 2) {
				if (i == 4 || i == 9) {
					continue;
				}
			}	
			//5人或者5人替补战(血战)
			if (battleMode == BattleType.BATTLE_FIVE) {
			
				if (activePlayerParter [i] != null) {		
					activePlayerParter [i].cleanButton ();
					setPosition (i, true);
					
					activePlayerParter [i].updateButton (playerFormationData [i], playerFormationData [i + 5], ButtonTeamPlayerViewInMission.BATTLEPREPARE, i,false);
		 
				}
			} else if (battleMode == BattleType.BATTLE_SUBSTITUTE) {
				
				if (activePlayerParter [i] != null) {		//确认有足够按钮
					activePlayerParter [i].cleanButton ();
					setPosition (i, true);
					
					//替补战,如果主战为空,替补自动上哦
//					if (playerFormationData [i] != null) {
//						activePlayerParter [i].updateButton (playerFormationData [i], playerFormationData [i + 5], ButtonTeamPlayerViewInMission.BATTLEPREPARE, i);
//					} else if (playerFormationData [i] == null && playerFormationData [i + 5] != null) {
//						activePlayerParter [i].updateButton (playerFormationData [i + 5], playerFormationData [i], ButtonTeamPlayerViewInMission.BATTLEPREPARE, i);
//					}
					if (i < 5 && activePlayerParter [i] != null) {
						if (playerFormationData [i] != null) { 
							activePlayerParter [i].updateButton (playerFormationData [i], playerFormationData [i + 5], ButtonTeamPlayerViewInMission.BATTLEPREPARE, i,false);
						}
					}
					if (i >= 5 && activePlayerParter [i] != null) {
						if (playerFormationData [i] != null) { 
							activePlayerParter [i].updateButton (playerFormationData [i], playerFormationData [i - 5], ButtonTeamPlayerViewInMission.BATTLEPREPARE, i - 5,true);
						}
					}	
				}
			} else {
				activePlayerParter [i].cleanButton ();
				setPosition (i, true);
				if (i < 5 && activePlayerParter [i] != null) {
					if (playerFormationData [i] != null) { 
					
					
						activePlayerParter [i].updateButton (playerFormationData [i], playerFormationData [i + 5], ButtonTeamPlayerViewInMission.BATTLEPREPARE, i,false);
					}
				}
				if (i >= 5 && activePlayerParter [i] != null) {
					if (playerFormationData [i] != null) { 
						
						activePlayerParter [i].updateButton (playerFormationData [i], playerFormationData [i - 5], ButtonTeamPlayerViewInMission.BATTLEPREPARE, i - 5,true);
					}
				}	
			}
			
		}
		
		//敌方信息更新
		if (isBossBattle) {
			//boss战单独处理
			buttonBoss.updateButton (enemyFormationData [0], null, ButtonTeamPlayerViewInMission.BATTLEPREPARE, 0,false);	
		} else {
			if (isPvP) {
				for (int i=0; i< activeEnemyParter.Length; i++) {
				
					if (enemyFormationData [i] != null) {
						activeEnemyParter [i].cleanButton ();
						setPosition (i, false);
						activeEnemyParter [i].updateButton (enemyFormationData [i], null, ButtonTeamPlayerViewInMission.PVPINTO, i,false);
					}
				}
			} else {
				for (int i=0; i< activeEnemyParter.Length; i++) {
			
					if (i < enemyFormationData.Length) {
						if (enemyFormationData [i] != null) {
							setPosition (i, false);
							activeEnemyParter [i].gameObject.SetActive (true);
							activeEnemyParter [i].cleanButton ();
							activeEnemyParter [i].updateButton (enemyFormationData [i], null, ButtonTeamPlayerViewInMission.BATTLEPREPARE, i,false);
						} 
					}
				}	
				
			}
		}
	}
	
	
	
	
	//读取对应的阵形位置
	void loadFormation ()
	{
		
		GameObject obj = null;
		//创建玩家阵形
		if (battleMode == BattleType.BATTLE_FIVE || battleMode == BattleType.BATTLE_SUBSTITUTE) {	

			obj = FormationManagerment.Instance.loadFormationPrefab (oldArmy.getLength (), player5v5Root, true);
		} else if (battleMode == BattleType.BATTLE_TEN) {
			obj = FormationManagerment.Instance.loadFormationPrefab (999, player10v10Root, true);
		}
		
		
		playerFormationObj = obj;
		playerFormationSample = FormationSampleManager.Instance.getFormationSampleBySid (oldArmy.formationID);
		
		
		//创建敌人阵形
		
		//获得敌人阵形数据
		
		if (battleMode == BattleType.BATTLE_FIVE || battleMode == BattleType.BATTLE_SUBSTITUTE) {	
			if (isPvP) {
				//todo
				int length = FormationSampleManager.Instance.getFormationSampleBySid (PvpInfoManagerment.Instance.getOpp ().formation).getLength ();
				obj = FormationManagerment.Instance.loadFormationPrefab (length, enemy5v5Root, false);
			} else {
				obj = FormationManagerment.Instance.loadFormationPrefab (BattleManager.lastMissionEvent.battleNum, enemy5v5Root, false);
			}
		} else if (battleMode == BattleType.BATTLE_TEN) {
			obj = FormationManagerment.Instance.loadFormationPrefab (10, enemy10v10Root, false);
		}
	
		enemyFormationObj = obj;
		//enemyFormationSample =FormationSampleManager.Instance.getFormationSampleBySid(ArmyManager.Instance.getActiveArmy ().arrayid);

	}

	protected override void DoEnable ()
	{
		base.DoEnable (); //2014.7.2 9:50 modified 
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();

	}

	//这里挂掉的肯定是再战斗场景里面
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		MessageWindow.ShowAlert (Language ("reconnection_01"), (msg) => {
			if (BattleManager.Instance != null)
				BattleManager.Instance.awardFinfish ();
			else
				finishWindow ();
		});
	}
}
