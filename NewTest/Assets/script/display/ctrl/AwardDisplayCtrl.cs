using UnityEngine;
using System.Collections;

public class AwardDisplayCtrl : MonoBase
{ 
	public int 	showSetp = 0;
	public Award[] award;
	public Award activeAward;
	int propsShowIndex;
	int	equipShowIndex;
	int	cardShowIndex;
	const int NOITEM = -1;
	const int SHOWOVER = -2;
	bool IsGetTreasure = false;//只是捡到宝箱
	bool allOver = false;

	public void openNextWindow ()
	{
		//	yield return new WaitForSeconds (0.2f);
		getNextData ();
	}
	
	public Award changeActiveAward (string type)
	{ 
		activeAward = getAwardByType (type);
		propsShowIndex = 0;
		equipShowIndex = 0;
		cardShowIndex = 0;
		showSetp = 1;
		IsGetTreasure = false;
		allOver = false;
		return activeAward;
	}

	public void Initialize (Award[] aw, string type)
	{
		award = aw;
		showSetp += 1;
		
		if (type == AwardManagerment.FB_END) {
			//上下面板收回去.准备显示副本奖励
			MissionInfoManager.Instance.autoGuaji=false;
			if (UiManager.Instance.missionMainWindow != null)
				UiManager.Instance.missionMainWindow.TweenerGroupOut ();

			if (MissionManager.instance != null)
				MissionManager.instance.cleanGameObj ();

			UiManager.Instance.switchWindow<FubenAwardWindow> (
				(window) => {
				window.init (aw);
			});
		
		} else if (type == AwardManagerment.FIRST) {
			getNextData ();
		} else if (type == AwardManagerment.MNGV) {
			getNextData ();
		} else	if (type == AwardManagerment.RES) {
			changeActiveAward (AwardManagerment.RES);
			getNextData ();
		} else	if (type == AwardManagerment.PVE) {
			changeActiveAward (AwardManagerment.PVE);
           // if (MissionInfoManager.Instance.mission.getChapterType() == ChapterType.TOWER_FUBEN) getNextData();
		} else	if (type == AwardManagerment.PVP) {
			changeActiveAward (AwardManagerment.PVP);
			//	getNextData ();
		} else	if (type == AwardManagerment.ARENA) {
			changeActiveAward (AwardManagerment.ARENA);
			//  getNextData ();
		} else	if (type == AwardManagerment.AWARDS_GODSWAR_GROUP) {
			changeActiveAward (AwardManagerment.AWARDS_GODSWAR_GROUP);
			//getNextData ();
        } else if (type == AwardManagerment.BOSS_INFO_AWARD) {
            changeActiveAward(AwardManagerment.BOSS_INFO_AWARD);
            getNextData();
		}else if (type == AwardManagerment.LASTBATTLE_AWARD) {
			changeActiveAward(AwardManagerment.LASTBATTLE_AWARD);
			getNextData();
		}

	}
	
	public Award getAwardByType (string type)
	{ 
		foreach (Award each in award) {
			if (each.type == type) 
				return each; 
		} 
		return null;
	}

	void getNextData ()
	{ 
		if (activeAward == null || activeAward.isNull () == true) {
			awardDisplayOver ();
			return;
		} 

		//战斗升级已经简化
//		if (BattleManager.Instance != null && BattleManager.Instance.levelUpList != null && activeAward.exps.Count > 0) {
//			if (activeAward.type == AwardManagerment.PVE || activeAward.type == AwardManagerment.PVP) {
//				if (cardLevelUpIndex < BattleManager.Instance.levelUpList.Count) {
//
//					UiManager.Instance.switchWindow<CardLevelUpWindow> (
//						(win) => {
//						win.Initialize (BattleManager.Instance.levelUpList [cardLevelUpIndex], this);	 
//						cardLevelUpIndex += 1;
//					});
//	
//					return;
//				}
//			}
//		}
		 
		if (showSetp == 1) {
			if (activeAward.moneyGap > 0) {
			
				//结算中金钱和exp不单独显示
				if (activeAward.type == AwardManagerment.FB_END)
					showSetp += 1; 
				
				if (activeAward.type == AwardManagerment.RES) {
					showSetp += 1; 
					return;
				}
				if (activeAward.type == AwardManagerment.FIRST) {
					UiManager.Instance.openWindow <ItemGivenWindow> (
						(win) => {

						win.Initialize (this, "money");
					}
					);
			
					showSetp += 1; 
					return;
				} 			
				if (activeAward.type == AwardManagerment.PVE || activeAward.type == AwardManagerment.PVP) {
					UiManager.Instance.openWindow<ItemGivenWindow> (
						(win) => {
						win.Initialize (this, "money");
					}
					);
		
					showSetp += 1; 
					return;
				} 						 
			} else {
				showSetp += 1; 
			}
		}
 
		if (showSetp == 2) {
			if (activeAward.expGap > 0) {
				
				if (activeAward.type == AwardManagerment.FB_END || activeAward.type == AwardManagerment.PVE || activeAward.type == AwardManagerment.PVP)
					showSetp += 1; 
				if (activeAward.type == AwardManagerment.RES) {
					showSetp += 1; 
					return;
				} 
				if (activeAward.type == AwardManagerment.FIRST) {
					UiManager.Instance.openWindow<ItemGivenWindow> (
						(win) => {
						win.Initialize (this, "exp");
					}	

					);
				
					showSetp += 1; 
					return;
				} 	
				if (activeAward.type == AwardManagerment.PVE) {
				
					UiManager.Instance.openWindow <ItemGivenWindow > (
						(win) => {
						win.Initialize (this, "exp");
					}
				
					);

					showSetp += 1; 
					return;
				} 	
			} else {
				showSetp += 1; 
			}
		}  
		
		if (showSetp == 3) {
			if (activeAward.rmbGap > 0) {
				if (activeAward.type == AwardManagerment.FB_END)
					showSetp += 1; 
				if (activeAward.type == AwardManagerment.RES) {
					showSetp += 1; 
					return;
				} 
				if (activeAward.type == AwardManagerment.FIRST) {

					UiManager.Instance.openWindow<ItemGivenWindow> (
						(win) => {
						win.Initialize (this, "rmb");
					}
					);

					showSetp += 1; 
					return;
				} 		
				if (activeAward.type == AwardManagerment.PVE || activeAward.type == AwardManagerment.PVP) {


					UiManager.Instance.openWindow<ItemGivenWindow> (
						(win) => {
						win.Initialize (this, "rmb");
					}
					);


					showSetp += 1; 
					return;
				} 	
			} else {
				showSetp += 1; 
			}
		}

		if (showSetp == 4) {
			if (activeAward.props.Count > 0) {
				if (propsShowIndex >= activeAward.props.Count) {
					propsShowIndex = SHOWOVER;
					showSetp += 1;
				} else { 
					if (activeAward.type != AwardManagerment.RES) {
						UiManager.Instance.openWindow<ItemGivenWindow> (
							(win) => {
							win.Initialize (activeAward.props [propsShowIndex], this);
							propsShowIndex += 1;
						}
						);
					} 
				}
			} else {
				showSetp += 1;
			}
		} 
			
		if (showSetp == 5) {
			if (activeAward.equips.Count > 0) {
				if (equipShowIndex >= activeAward.equips.Count) {
					equipShowIndex = SHOWOVER;
					showSetp += 1;
				} else {
					
					if (activeAward.type != AwardManagerment.RES) { 

						UiManager.Instance.openWindow<ItemGivenWindow> (
							(win) => {
							win.Initialize (activeAward.equips [equipShowIndex], this); 
							equipShowIndex += 1;
						}
						);
					} 
				}
			} else {
				showSetp += 1;
			}
		}
		if (showSetp == 6) {
			if (activeAward.cards.Count > 0) {
				if (cardShowIndex >= activeAward.cards.Count) {
					cardShowIndex = SHOWOVER; 
					allOver = true;
				} else {
					if (activeAward.type != AwardManagerment.RES) {
						UiManager.Instance.openWindow<ItemGivenWindow> ((win) => {
							if (cardShowIndex >= activeAward.cards.Count) {
								cardShowIndex = SHOWOVER; 
								allOver = true;
							} else {
								win.Initialize (activeAward.cards [cardShowIndex], this);
								cardShowIndex += 1;
							}
						});
					}
				}
			} else {
				allOver = true;
			}
		}
		 
		if (allOver == true) {
			//结束
			awardDisplayOver (); 
		} 
	}

	public  void awardDisplayOver ()
	{ 
		if (IsGetTreasure) {
			MonoBase.Destroy (this); 
			//	UiManager.Instance.openWindow<MissionMainWindow> ();
			MissionManager.instance.updateEventObj ();
			return; 
		}

		if (activeAward.type != AwardManagerment.PVE && activeAward.type != AwardManagerment.PVP && activeAward.type != AwardManagerment.ARENA&&activeAward.type!= AwardManagerment.AWARDS_GODSWAR_GROUP) { 
			if (activeAward.type == AwardManagerment.FB_END) {
				//通关奖励之后开宝箱	
				MissionManager.instance.getTreasureAward (); 
			} else if (activeAward.type == AwardManagerment.FIRST) {
				//首通奖励显示完成后退出
				MonoBase.Destroy (this); 
				MissionManager.instance.missionEnd ();
			} else if (activeAward.type == AwardManagerment.RES) {			
				MissionManager.instance.updateEventObj ();
				UiManager.Instance.missionMainWindow.updateUserInfo ();
				MonoBase.Destroy (this); 
			} else if (activeAward.type == AwardManagerment.MNGV) {		
				//开宝箱之后开首通奖励
				MissionManager.instance.getFirstBloodAward ();
			}

		} else { 
			if (activeAward.type == AwardManagerment.PVP) {
				bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;
				PvpInfoManagerment.Instance.result (isWin);
			} else {
				BattleManager.Instance.awardFinfish (); 
				MonoBase.Destroy (this); 
			}
		}
	}
}
