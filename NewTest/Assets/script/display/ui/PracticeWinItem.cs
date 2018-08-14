using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 女神试炼
/// </summary>
public class PracticeWinItem : MonoBase 
{
	/** 奖励条目容器 */
	public GameObject awardItemsContent;
	/** 女神试炼描述 */
	public UILabel desc;
	/** 开始试炼按钮 */
	public ButtonBase startPracticeButton;
	/** 出战队伍 */
	public UILabel fightTeamNameLabel;
	/** 最大挑战记录次数 */
	public UILabel maxCountLabel;
	/***/
	public UILabel timesLabel;

	/**恢复次数倒计时*/
	public UILabel label_dueTime;

    private int mSid;

	public void Initialize(List<GameObject>[] awardItems, int sid)
	{
        mSid = sid;
		updateShow (awardItems);
	}

	bool needUpdate=false;
	void Update()
	{
		if(needUpdate&&Time.frameCount%20==0)
		{
			int time=FuBenManagerment.Instance.practiceDueTime-ServerTimeKit.getSecondTime();
			if(time>0)
			{
				label_dueTime.text = TimeKit.timeTransform(time*1000);
			}
			else
			{
				updateTimes();
				label_dueTime.text="";
			}
		}
	}
	public void doClieckEvent(TeamPrepareWindow window,GameObject gameObject,Mission mission,int m_sid)
	{
		if(gameObject.name == "startPracticeButton")
		{
			GuideManager.Instance.doGuide (); 
			GuideManager.Instance.closeGuideMask ();	
			int teamId=ArmyManager.PVE_TEAMID;

			if (FuBenManagerment.Instance.getPracticeChapter ().getNum () == 0) {
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage ("s0343"));
				return;
			} else if (UserManager.Instance.self.getUserLevel () < mission.getRequirLevel ()) {
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage ("s0342"));
				return;
			}


			MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (m_sid);

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
		if (gameObject.name == "buttonHelp") {
			UiManager.Instance.openDialogWindow<GeneralDesWindow>((win)=>{
				win.initialize(LanguageConfigManager.Instance.getLanguage("practiceWindow_desc"),LanguageConfigManager.Instance.getLanguage("practiceWindow_descTitle"),"");
			});
		}
	}
	private void updateTimes() 
    {
        FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
        port.init(timesCallBack, ChapterType.PRACTICE);
	}
    private void timesCallBack() {
        timesLabel.text = FuBenManagerment.Instance.getPracticeChapter().getNum() + "/" + FuBenManagerment.Instance.getPracticeChapter().getMaxNum();
        if (FuBenManagerment.Instance.practiceDueTime > 0 && FuBenManagerment.Instance.practiceDueTime > ServerTimeKit.getSecondTime()) {
            needUpdate = true;
        } else {
            needUpdate = false;
        }
    }
	public void updateShow(List<GameObject>[] awardItems)
	{
        string teamType = Language(MissionSampleManager.Instance.getMissionSampleBySid(mSid).teamType == TeamType.All ? "s0567" : "s0566");
		maxCountLabel.text = UserManager.Instance.self.practiceHightPoint.ToString ();
	    fightTeamNameLabel.text = Language("s0440") + LanguageConfigManager.Instance.getLanguage("s0066") + teamType;
		desc.text = LanguageConfigManager.Instance.getLanguage ("practiceWindow_desc");
		updateAwardItemShow (awardItems);
		updateTimes();
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
			tempObj.transform.localPosition = new Vector3 (i * 100, 0, 0);
			tempObj.transform.localScale = new Vector3(0.9f,0.9f,1f);
			tempObj.SetActive(true);
		}
	}
}
