using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 剧情副本
/// </summary>
public class MissionWinItem : MonoBase
{

	public GameObject firstAwardParent;
	public GameObject awardParent;
	public GameObject[] joinMissionItems;
	public ButtonBase[] joinMissionButtons;
	public ButtonBase[] sweepMissionButtons;
	public UILabel[] sweepMissionTips;
	private int  missionSid;
	/** 难度通关星 */
	public UISprite[] starsSprite;
	/** 全通关行动力 */
	public UILabel needPveValue;
	/** 剩余挑战次数 */
	public UILabel timesValue;

	public void Initialize (List<GameObject>[] awardItems)
	{
		updateShow (awardItems);
	}

	public void updateShow (List<GameObject>[] awardItems)
	{
		int missionSid = FuBenManagerment.Instance.selectedMissionSid;
//		MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (missionSid);
//		ChapterSample sample = ChapterSampleManager.Instance.getChapterSampleBySid (ChapterSid);
		int pveCost = FuBenManagerment.Instance.getPveCostMissionSid (missionSid);
		needPveValue.text = pveCost.ToString ();
		int starTotalNum = FuBenManagerment.Instance.getStarNumByMissionSid (missionSid);
		int starCurrentNum = FuBenManagerment.Instance.getMyStarNumByMissionSid (missionSid);
		changeStarsSprite (starTotalNum, starCurrentNum);
		//显示章节图片
//		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CHAPTERDESCIMAGEPATH + ChapterSampleManager.Instance.getChapterSampleBySid (ChapterSid).thumbIcon, chapterImage);
		string teamType=string.Empty;
		MissionSample sample=MissionSampleManager.Instance.getMissionSampleBySid(missionSid);
		if(sample.teamType==TeamType.All)
		{
			teamType=Language("s0567");
		}else
		{
			teamType=Language("s0566");
		}

		updateAwardItemShow (awardItems);
	}
	/// <summary>
	/// 设置星
	/// <param name="starTotalNum">总星数</param>
	/// <param name="starCurrentNum">当前星</param>
	/// </summary>
	void changeStarsSprite (int starTotalNum, int starCurrentNum)
	{
		UISprite start;
		for (int i = 0; i < starsSprite.Length; i++) {
			start = starsSprite [i];
			int temp = i + 1;
			if (temp <= starTotalNum) {
				start.gameObject.SetActive (true);
				if (starCurrentNum >= temp)
					start.spriteName = "star";
				else
					start.spriteName = "star_b";
			} else {
				start.gameObject.SetActive (false);
			}
		}
	}

	public void updateButton (Mission mission)
	{
		missionSid = mission.sid;
		string spName = mission.getMissionType ();
		timesValue.text = FuBenManagerment.Instance.getTimesByMissionSid (mission.sid) + "/" + mission.getMaxNum(3);
		if (spName == MissionShowType.LEVEL_LOW) {
			int starTotalNum = FuBenManagerment.Instance.getStarNumByMissionSid (mission.sid);

			for (int i=0; i<joinMissionItems.Length; i++) {

				int temp = i + 1;
				if (temp <= starTotalNum) {
					joinMissionItems [i].SetActive (false);
					joinMissionButtons [i].disableButton (true);
					sweepMissionTips [i].gameObject.SetActive (false);
				} else
					joinMissionItems [i].SetActive (false);
			}
		} else {
			//当前副本的可打难度数
			int starTotalNum = FuBenManagerment.Instance.getStarNumByMissionSid (mission.sid);
			//当前副本已通过难度
			int starCurrentNum = FuBenManagerment.Instance.getMyStarNumByMissionSid (mission.sid);
			bool canSweep = getSweepEnable ();

			for (int i=0; i<joinMissionButtons.Length; i++) {

				int temp = i + 1;
				//只显示当前副本拥有的难度入口
				if (temp <= starTotalNum) {
					joinMissionItems [i].SetActive (true);
					//只有通过了前置难度才能打后面的
					if (i <= starCurrentNum) {
						if (i == 2 && FuBenManagerment.Instance.getTimesByMissionSid (mission.sid) == 0) {
							if(mission.getMaxNum(3)==5){
								joinMissionButtons [i].disableButton (false);
								joinMissionButtons [i].textLabel.text = LanguageConfigManager.Instance.getLanguage("missionWindow_01");
							}else{
								joinMissionButtons [i].disableButton (true);
							}
						} else {
							joinMissionButtons [i].disableButton (false);
						}
						if (i < starCurrentNum) {
							if (canSweep) {
								if (i == 2 && FuBenManagerment.Instance.getTimesByMissionSid (mission.sid) == 0) {
									sweepMissionButtons [i].disableButton (true);
								} else {
									sweepMissionButtons [i].disableButton (false);
								}
								sweepMissionTips [i].gameObject.SetActive (false);
							} else {
								sweepMissionButtons [i].disableButton (true);
								sweepMissionTips [i].gameObject.SetActive (true);
								sweepMissionTips [i].text = LanguageConfigManager.Instance.getLanguage ("sweepTip_06", SweepConfigManager.Instance.storyWarSweepMinLevel.ToString ());
							}
						} else {
							sweepMissionButtons [i].disableButton (true);
							sweepMissionTips [i].gameObject.SetActive (false);
							//sweepItemTip.SetActive(true);
							//sweepMissionTips[i].text=LanguageConfigManager.Instance.getLanguage("sweepTip_06",SweepConfigManager.Instance.storyWarSweepMinLevel.ToString());
						}
					} else {
						joinMissionButtons [i].disableButton (true);
						sweepMissionButtons [i].disableButton (true);
						sweepMissionTips [i].gameObject.SetActive (false);
					}
				} else
					joinMissionItems [i].SetActive (false);
			}
		}
	}

	private bool getSweepEnable ()
	{
		int playerLevel = UserManager.Instance.self.getUserLevel ();
		return playerLevel >= SweepConfigManager.Instance.storyWarSweepMinLevel;
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
			if (i >= 5) {
				tempObj.transform.localPosition = new Vector3 ((i - 5) * 92, -92, 0);
			} else {
				tempObj.transform.localPosition = new Vector3 (i * 92, 0, 0);
			}
			tempObj.transform.localScale = new Vector3 (0.8f, 0.8f, 0);
			tempObj.SetActive (true);

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
			tempObj.transform.localPosition = new Vector3 (i * 92, 0, 0);
			tempObj.transform.localScale = new Vector3 (0.8f, 0.8f, 0);
			tempObj.SetActive (true);
			if (i >= 5) {
				tempObj.SetActive (false);
			}
		}
	}

	public void doClieckEvent (TeamPrepareWindow window, GameObject gameObject, int m_sid)
	{
		if (gameObject.name.StartsWith ("startMissionButton")) {
			int missionLevel = StringKit.toInt (gameObject.name.Split ('&') [1]);
			int teamId = ArmyManager.PVE_TEAMID;

			MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (m_sid);
			int cSid = sample.chapterSid;
			int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;
			if (UserManager.Instance.self.getPvEPoint () < 1) {
				UiManager.Instance.openDialogWindow<PveUseWindow> ();
				return; 
			} else if (type == ChapterType.HERO_ROAD) {
				MissionEventSample e = new Mission (sample.sid).getPlayerPoint ().getPointEvent ();
				if (e != null && !UserManager.Instance.self.costCheck (e.cost, e.costType)) {
					UiManager.Instance.openDialogWindow<PveUseWindow> ();
					return;
				}
			} else if (missionLevel == 3 && FuBenManagerment.Instance.getTimesByMissionSid (sample.sid) <= 0) {
				if(sample.num[2]==5){ //噩梦次数限定为5次的关卡增加次数重功能
					int boughtTimes = int.MaxValue;
					FubenBuyChallengeTimesInfoFport infoFport = FPortManager.Instance.getFPort<FubenBuyChallengeTimesInfoFport>();
					infoFport.access(sample.sid,(num)=>{
						boughtTimes = num;
						Vip vip = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.vipLevel);
						int tmp = (vip == null ? 1 : vip.privilege.fubenResetTimes);
						if(boughtTimes >= tmp){ //可重置次数不足
							UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
								win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("recharge01"),
								               LanguageConfigManager.Instance.getLanguage("missionWindow_02"),openVipWindow);
							});
							return;
						}else{ //提示玩家是否要重置挑战次数
							UiManager.Instance.openDialogWindow<BuyTimesTipWindow>((win)=>{
								FubenChallengePrice price = new FubenChallengePrice(boughtTimes);
								win.initialize(LanguageConfigManager.Instance.getLanguage("missionWindow_03",price.getPriceString(),boughtTimes.ToString()),(value)=>{
									if(value){
										FubenBuyChallengeTimesFport fport = FPortManager.Instance.getFPort<FubenBuyChallengeTimesFport>();
										fport.access(sample.sid,(isOK)=>{
											if(isOK){
												TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("MISSION_SUCCESS_02"));//重置挑战次数成功
												FuBenManagerment.Instance.resetTimesByMissionSid(sample.sid);
												joinMissionButtons[2].textLabel.text = LanguageConfigManager.Instance.getLanguage("missionWinItem01");
												joinMissionButtons[2].disableButton(false);
												timesValue.text = FuBenManagerment.Instance.getTimesByMissionSid (sample.sid) + "/" + sample.num[2];
												if (FuBenManagerment.Instance.getTimesByMissionSid (sample.sid) == 0) {
													sweepMissionButtons [2].disableButton (true);
												} else {
													sweepMissionButtons [2].disableButton (false);
												}
											}
										});
									}
								});
							});
						}
					});
				}else{
					TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("MISSION_ERROR_01"));//挑战次数不足
				}
				return;
			}

			int currentCombat=0;
			if(sample.teamType==TeamType.All)
			{
				currentCombat=ArmyManager.Instance.getTeamAllCombat(teamId);
			}else if(sample.teamType==TeamType.Main)
			{
				currentCombat=ArmyManager.Instance.getTeamCombat(teamId);
			}

			//战斗力不足提示
			int requestCombat=sample.getRecommendCombat(missionLevel);
			if (currentCombat < requestCombat) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.dialogCloseUnlockUI=false;
					string tip=(sample.teamType==TeamType.Main)?Language("combatTip_01",requestCombat.ToString()):Language("combatTip_02",requestCombat.ToString());
					win.initWindow (2, Language("s0094"), Language ("s0093"), tip, window.msgBack);
					win.msg.msgNum=missionLevel;
					
				});
				return;
			}
			window.intoFubenBack (missionLevel);
		}
		else if (gameObject.name.StartsWith ("sweepMissionButton")) {//扫荡
			//判断玩家是否有足够的存储空间
			if (FuBenManagerment.Instance.isStoreFull ()) {
				return;
			}
			int missionLevel = StringKit.toInt (gameObject.name.Split ('&') [1]);
			UiManager.Instance.openWindow<SweepNumberWindow> ((win) => {
				win.init (EnumSweep.fuben, missionSid, missionLevel);
			});
		}
	}
	/// <summary>
	/// 打开VIP窗口
	/// </summary>
	private void openVipWindow (MessageHandle msg)
	{
        if (msg.buttonID == MessageHandle.BUTTON_RIGHT)
		    UiManager.Instance.openWindow<VipWindow> ();
	}
}
