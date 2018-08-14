using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 限时活动副本(已废弃的类)
/// </summary>
public class ActivityWinItem : MonoBase {

	public ButtonActivityChapter info;
	/** 出战队伍 */
	public UILabel fightTeamNameLabel;
	/** 奖励条目容器 */
	public GameObject awardItemsContent;
	/** 开始活动按钮 */
	public ButtonBase startActivityButton;

	public void Initialize(List<GameObject>[] awardItems, int sid)
	{
		updateShow (awardItems, sid);
	}
	public void updateShow(List<GameObject>[] awardItems, int sid)
	{
        string teamType = Language(MissionSampleManager.Instance.getMissionSampleBySid(sid).teamType == TeamType.All ? "s0567" : "s0566");
        fightTeamNameLabel.text = Language("s0440") + LanguageConfigManager.Instance.getLanguage("s0066") + teamType;
		Chapter chapter= FuBenManagerment.Instance.getActivityChapterBySid (FuBenManagerment.Instance.selectedChapterSid);
		info.updateActive ((ActivityChapter)chapter);
		updateAwardItemShow (awardItems);
	}
	public void updateAwardItemShow(List<GameObject>[] awardItems)
	{
		if (awardItems == null || awardItems.Length == 0) return;
		GameObject tempObj;
		List<GameObject> items = awardItems [TeamPrepareWindow.PRIZES_GENERAL_TYPE];
		if(items==null) return;
		for (int i = 0; i < items.Count; i++) 
		{
			tempObj = items [i];
			tempObj.transform.parent = awardItemsContent.transform;
			tempObj.transform.localPosition = new Vector3 (i * 110, 0, 0);
			tempObj.transform.localScale = new Vector3(0.9f,0.9f,1f);
			tempObj.SetActive(true);
			if(i>=5)
			{
				tempObj.SetActive(false);
			}
		}
	}
	public void doClieckEvent(TeamPrepareWindow window,GameObject gameObject,int m_sid)
	{
		if (gameObject.name == "startActivityButton") 
		{
			GuideManager.Instance.doGuide (); 
			GuideManager.Instance.closeGuideMask ();	
			int teamId=ArmyManager.PVE_TEAMID;

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
			int requestCombat=sample.getRecommendCombat();
			if (currentCombat < requestCombat) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.dialogCloseUnlockUI=false;
					string tip=(sample.teamType==TeamType.Main)?Language("combatTip_01",requestCombat.ToString()):Language("combatTip_02",requestCombat.ToString());
					win.initWindow (2, Language("s0094"), Language ("s0093"), tip, window.msgBack);
				});
				return;
			}
			window.intoFubenBack ();
		}
	}
}
