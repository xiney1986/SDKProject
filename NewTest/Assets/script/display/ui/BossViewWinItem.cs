using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 讨伐
/// </summary>
public class BossViewWinItem : MonoBase
{
	/* const */
	/** 讨伐购买次数单价 */
	public const int PRICE_BOSS_COUNT = 250;
	/** 讨伐冷却时间 */
	public const int WAR_CD = 10;

	/* gameobj fields */
	/** 讨伐次数 */
	public UILabel warCountLabel;
	/** 容器 */
	public SampleDynamicContent sampleContent;
	/** 战斗按钮 */
	public ButtonBase warButton;
	/** 购买按钮 */
	public ButtonBase buyTimeButton;
	/** 首通奖励节点 */
	public GameObject firstAwardParent;
	/** 奖励节点 */
	public GameObject awardParent;
//	/** 出战队伍 */
//	public UILabel fightTeamNameLabel;
	/** 父窗口 */
	public WindowBase fatherWindow;
	/** 当前选择副本 */
	public Mission activeMission;
	/** 副本列表 */
	public List<Mission> missionList;
	/** 右箭头 */
	public UISprite rightArrow;
	/** 左箭头 */
	public UISprite leftArrow;
	/** 扫荡说明 */
	public UILabel label_SweepTip;
	/** 扫荡按钮 */
	public ButtonBase btn_Sweep;

	/* fields */
	/** 当前选择预制体 */
	private ButtonBossView activeShowItem;
	/** 讨伐失败倒计时 */
	private Timer timer;

	/* methods */
	/// <summary>
	/// 初始化资源信息
	/// </summary>
	public void Initialize (List<GameObject>[] awardItems, List<Mission> missionList, Mission _activeMission)
	{
		this.activeMission = _activeMission;
		this.missionList = missionList;
		updateShow (awardItems);
		checkCDTime ();
		begin ();
	}
	/// <summary>
	/// 初始化容器
	/// </summary>
	private void begin ()
	{
		sampleContent.startIndex = getActiveMissionIndex ();
		sampleContent.maxCount = missionList.Count;
		sampleContent.callbackUpdateEach = updatePage;
		sampleContent.onCenterItem = updateActivePage;
		sampleContent.init ();
		updateActivePage (sampleContent.getCenterObj ());
		if (!FuBenManagerment.Instance.isWarAttackBossWin) {
			startWarCD ();
			FuBenManagerment.Instance.isWarAttackBossWin = true;
		} else if (FuBenManagerment.Instance.warWinAward != null) {
			UiManager.Instance.openDialogWindow <WarAwardWindow> ((win)=>{
				win.init (FuBenManagerment.Instance.warWinAward);
			});
		}
	}
    void OnDisable() {
        if (timer != null) {
            timer.stop();
            timer = null;
        }
    }
	/// <summary>
	/// 获得当前副本的序号
	/// </summary>
	private int getActiveMissionIndex ()
	{
		int index = 0;
		if (activeMission != null && activeMission.sid != missionList [0].sid) {
			for (int i=0; i<missionList.Count; i++) {
				if (missionList [i].sid == activeMission.sid) {
					index = i;
					break;
				}
			}
		}
		return index;
	}
	/// <summary>
	/// 更新容器对象
	/// </summary>
	private void updatePage (GameObject obj)
	{
		int index = StringKit.toInt (obj.name) - 1;
		if (this.missionList == null || index >= this.missionList.Count || this.missionList [index] == null) {
			return;
		}
		activeShowItem = obj.GetComponent<ButtonBossView> ();
		Mission mis = missionList [index];
		activeShowItem.updateBoss (mis);
	}
	/// <summary>
	/// 更新讨伐扫荡按钮信息
	/// </summary>
	private void updateSweep (Mission _misssion)
	{
		bool sweepEnable = getSweepEnable ();
		string type = _misssion.getMissionType ();
		if (type == MissionShowType.COMPLET && sweepEnable) {
			btn_Sweep.disableButton (false);
		} else {
			btn_Sweep.disableButton (true);
		}
		if (sweepEnable) {
			label_SweepTip.gameObject.SetActive (false);
		} else {
			label_SweepTip.gameObject.SetActive (true);
			label_SweepTip.text = LanguageConfigManager.Instance.getLanguage ("sweepTip_06", SweepConfigManager.Instance.bossWarSweepMinLevel.ToString ());
		}
	}

	private bool getSweepEnable ()
	{
		return UserManager.Instance.self.getUserLevel () >= SweepConfigManager.Instance.bossWarSweepMinLevel;
	}
	/// <summary>
	/// 更新容器对象
	/// </summary>
	private void updateActivePage (GameObject obj)
	{
		int index = StringKit.toInt (obj.name) - 1;
		Mission mis = missionList [index];
		setFaterWindowTitle (mis);
		updateSweep (mis);

		TeamPrepareWindow faWnd = fatherWindow as TeamPrepareWindow;	
		if (faWnd.getMission () != missionList [index]) {
			faWnd.setMissionByBossView (missionList [index]);
		}

		if (index == 0) {
			leftArrow.alpha = 0;
		} else {
			leftArrow.alpha = 1;
		}

		if (index >= missionList.Count - 1) {
			rightArrow.alpha = 0;
		} else {
			rightArrow.alpha = 1;
		}		

	}
	/// <summary>
	/// 设置标题
	/// </summary>
	public void setFaterWindowTitle (Mission mis)
	{
		if (mis != null) {
			Card boss = CardManagerment.Instance.createCard (mis.getBossSid ());
			if (boss != null) 
				(fatherWindow as TeamPrepareWindow).setTitle (boss.getName ());
		}
	}

	public void updateShow (List<GameObject>[] awardItems)
	{
		string teamType = Language (MissionSampleManager.Instance.getMissionSampleBySid (activeMission.sid).teamType == TeamType.All ? "s0567" : "s0566");
		warCountLabel.text = LanguageConfigManager.Instance.getLanguage ("s0146") + ":" + FuBenManagerment.Instance.getWarChapter () .getNum () + "/" + FuBenManagerment.Instance.getWarChapter ().getMaxNum ();
//		fightTeamNameLabel.text = Language ("s0440") + LanguageConfigManager.Instance.getLanguage ("s0066") + teamType;
		updateButton ();
		updateAwardItemShow (awardItems);
	}

	public void updateTapShow (Mission _activeMission, List<GameObject>[] awardItems)
	{
		this.activeMission = _activeMission;
		if (awardParent.transform.childCount > 0) {
			for (int i = 0; i < awardParent.transform.childCount; i++) {
				Destroy (awardParent.transform.GetChild (i).gameObject);
			}
		}
		if (firstAwardParent.transform.childCount > 0) {
			for (int i = 0; i < firstAwardParent.transform.childCount; i++) {
				Destroy (firstAwardParent.transform.GetChild (i).gameObject);
			}
		}
		updateShow (awardItems);
	}

	public void updateAwardItemShow (List<GameObject>[] awardItems)
	{
		if (awardItems == null || awardItems.Length == 0)
			return;
		updateGeneralAwardItemShow (awardItems [TeamPrepareWindow.PRIZES_GENERAL_TYPE]);
		if (awardItems.Length > 1)
			updateFirstAwardItemShow (awardItems [TeamPrepareWindow.PRIZES_FIRST_TYPE]);
	}

	public void updateGeneralAwardItemShow (List<GameObject> items)
	{
		if (items == null)
			return;
		GameObject tempObj;
		for (int i = 0; i < items.Count; i++) {
			tempObj = items [i];
			tempObj.transform.parent = awardParent.transform;
			tempObj.transform.localPosition = new Vector3 (i * 110, 0, 0);
			tempObj.transform.localScale = Vector3.one;
			tempObj.SetActive (true);
			if (i >= 5) {
				tempObj.SetActive (false);
			}
		}
	}

	public void updateFirstAwardItemShow (List<GameObject> items)
	{
		if (items == null)
			return;

		GameObject tempObj;
		for (int i = 0; i < items.Count; i++) {
			tempObj = items [i];
			tempObj.transform.parent = firstAwardParent.transform;
			tempObj.transform.localPosition = new Vector3 (i * 110, 0, 0);
			tempObj.transform.localScale = Vector3.one;
			tempObj.SetActive (true);
			if (i >= 5) {
				tempObj.SetActive (false);
			}
		}
	}

	public void doClieckEvent (TeamPrepareWindow window, GameObject gameObject, int m_sid)
	{
		if (gameObject.name == "startBossButton") {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.closeGuideMask ();
			int teamId = ArmyManager.PVE_TEAMID;

			if (FuBenManagerment.Instance.getWarChapter ().getNum () <= 0) {
				buyTimeConfirm ();
				return;
			}
			MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (m_sid);

			int currentCombat = 0;
			if (sample.teamType == TeamType.All) {
				currentCombat = ArmyManager.Instance.getTeamAllCombat (teamId);
			} else if (sample.teamType == TeamType.Main) {
				currentCombat = ArmyManager.Instance.getTeamCombat (teamId);
			}

			//combatTip_01 主力|本关卡由主力队伍出战，您的主力队伍战斗力不足%1，挑战有较大危险，确定要进入吗？
			//combatTip_02  全部|本关卡需要10人出战，您的10人战斗力不足%1，挑战有较大危险，确定要进入吗？

			//战斗力不足提示
			int requestCombat = sample.getRecommendCombat ();
			if (currentCombat < requestCombat) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.dialogCloseUnlockUI=false;
					string tip = (sample.teamType == TeamType.Main) ? Language ("combatTip_01", requestCombat.ToString ()) : Language ("combatTip_02", requestCombat.ToString ());
					win.initWindow (2, Language ("s0094"), Language ("s0093"), tip, window.msgBack);
				});
				return;
			}
			window.intoFubenBack ();
		} else if (gameObject.name == "sweepBossButton") {//扫荡

            if (PvpInfoManagerment.Instance.getPvpInfo() != null) {
                string msg = LanguageConfigManager.Instance.getLanguage("pvpend04");
                UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                    win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0093"), LanguageConfigManager.Instance.getLanguage("s0094"), msg, (msgHandle) => {
                        if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
                            win.dialogCloseUnlockUI = true;
                        } else {
                            PvpInfoManagerment.Instance.clear();
                            GuideManager.Instance.doGuide();
                            GuideManager.Instance.closeGuideMask();

                            if (FuBenManagerment.Instance.getWarChapter().getNum() <= 0) {
                                buyTimeConfirm();
                                return;
                            }
                            //判断玩家是否有足够的存储空间
                            if (FuBenManagerment.Instance.isStoreFull()) {
                                return;
                            }
                            UiManager.Instance.openWindow<SweepNumberWindow>((w) => {
                                w.init(EnumSweep.boss, m_sid, 1);
                                w.setDescriptOff();
                            });
                        }
                    }, MessageAlignType.center);
                });

            } else {
                GuideManager.Instance.doGuide();
                GuideManager.Instance.closeGuideMask();

                if (FuBenManagerment.Instance.getWarChapter().getNum() <= 0) {
                    buyTimeConfirm();
                    return;
                }
                //判断玩家是否有足够的存储空间
                if (FuBenManagerment.Instance.isStoreFull()) {
                    return;
                }
                UiManager.Instance.openWindow<SweepNumberWindow>((w) => {
                    w.init(EnumSweep.boss, m_sid, 1);
                    w.setDescriptOff();
                });
            }

		} else if (gameObject.name == "buyButton") {
			int viplv = UserManager.Instance.self.getVipLevel ();
			if (viplv <= 0) {
//				UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("s0153"), null);
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("recharge01"), LanguageConfigManager.Instance.getLanguage ("s0093"),
					               LanguageConfigManager.Instance.getLanguage ("s0153"), (msgHandle) => {
						if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
							UiManager.Instance.openWindow<VipWindow> ();
						}
					});
				});

			} else {
				if (FuBenManagerment.Instance.getWarChapter ().getNum () >= FuBenManagerment.Instance.getWarChapter ().getMaxNum ()) { 
					UiManager.Instance.openDialogWindow <MessageWindow> ((win) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), "", LanguageConfigManager.Instance.getLanguage ("s0156"), null);	
					});
				} else {
					buyTimeConfirm ();
				}
			}
		}
	}

	/// <summary>
	/// 更新按钮状态
	/// </summary>
	public void updateButton ()
	{
		if (activeMission.getRequirLevel () > UserManager.Instance.self.getUserLevel () || !FuBenManagerment.Instance.isCompleteLastMission (activeMission.sid)
		    || UserManager.Instance.self.getWarCDTime () > 0) {
			warButton.disableButton (true);
		} else {
			warButton.disableButton (false);
		}
	}

	public void buyNumCallBack (int buyCount)
	{
		if (buyCount > 0) {
			UiManager.Instance.openDialogWindow <MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), "", LanguageConfigManager.Instance.getLanguage ("s0155", buyCount + ""), null);
			});
			FuBenManagerment.Instance.getWarChapter ().addBuyed (buyCount);
			warCountLabel.text = LanguageConfigManager.Instance.getLanguage ("s0146") + ":" + FuBenManagerment.Instance.getWarChapter () .getNum () + "/" + FuBenManagerment.Instance.getWarChapter ().getMaxNum ();
		} else {
			UiManager.Instance.openDialogWindow <MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0385"), null);
			});
		}
	}

	void buyTimeConfirm ()
	{
		int viplv = UserManager.Instance.self.getVipLevel ();
		if (viplv <= 0) {
//				UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("s0153"), null);
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("recharge01"), LanguageConfigManager.Instance.getLanguage ("s0093"),
			               LanguageConfigManager.Instance.getLanguage ("s0153"), (msgHandle) => {
					if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
						UiManager.Instance.openWindow<VipWindow> ();
					}
				});
			});
			return;
		}
		int canBuyCount = FuBenManagerment.Instance.getWarChapter ().getCanBuyNum ();
		if (canBuyCount <= 0) {
			UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("s0385"), null);		
			return;
		}
		int gapNum = FuBenManagerment.Instance.getWarChapter ().getMaxNum () - FuBenManagerment.Instance.getWarChapter ().getNum ();
		UiManager.Instance.openDialogWindow<BuyWindow> ((window) => {
			BuyWindow.BuyStruct buyStruct = new BuyWindow.BuyStruct ();
			buyStruct.iconId = ResourcesManager.ICONIMAGEPATH + "87";
			buyStruct.unitPrice = PRICE_BOSS_COUNT;
			window.init (buyStruct, Mathf.Min (canBuyCount, gapNum), 1, PrizeType.PRIZE_RMB, (msg) => {			
				if (msg.msgEvent != msg_event.dialogCancel) {
					if (msg.msgNum * PRICE_BOSS_COUNT > UserManager.Instance.self.getRMB ())
						MessageWindow.ShowRecharge (LanguageConfigManager.Instance.getLanguage ("s0158"));
					else {
						FuBenBuyWarNumFPort port = FPortManager.Instance.getFPort ("FuBenBuyWarNumFPort") as FuBenBuyWarNumFPort;
						port.buyNum (buyNumCallBack, msg.msgNum);
					}
				}
			});
			window.dialogCloseUnlockUI=false;
		});
	}

	/// <summary>
	/// 讨伐战败后设置计算冷却时间打开
	/// </summary>
	public void startWarCD () {
		UserManager.Instance.self.startWarCD ();
		checkCDTime ();
	}

	/// <summary>
	/// 判断是否需要计算冷却时间
	/// </summary>
	private void checkCDTime () {
		if (UserManager.Instance.self.getWarCDTime () > 0 && timer == null) {
			warButton.disableButton (true);
			warButton.textLabel.text = "" + UserManager.Instance.self.getWarCDTime ();
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
			timer.addOnTimer (updateCoolingTime);
			timer.start ();
		} else if (UserManager.Instance.self.getWarCDTime () == 0 && timer != null) {
			updateCoolingTime ();
		}
	}
	/// <summary>
	/// 更新冷却数据
	/// </summary>
	private void updateCoolingTime () {
		if (UserManager.Instance.self.getWarCDTime () <= 0) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			updateButton ();
			warButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("warchoose09");
		} else {
			warButton.disableButton (true);
			warButton.textLabel.text = "" + UserManager.Instance.self.getWarCDTime ();
		}
	}
	/// <summary>
	/// 清除时间管理
	/// </summary>
	public void clearTimer ()
	{
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
}