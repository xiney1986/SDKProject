using UnityEngine;
using System.Collections;

/// <summary>
/// 英雄之章条目(已经废弃的类)
/// </summary>
public class HeroRoadWinItem : MonoBase
{
	public GameObject roadItemRoot;
	public GameObject talentitem;
	public GameObject rootItem;
	HeroRoadItem heroRoadItem;

	public void 	Initialize(HeroRoadItem heroRoadItem)
	{
		this.heroRoadItem = heroRoadItem;
		ShowHeroRoadItem ();
		UpdateTalent ();
	}

	void ShowHeroRoadItem()
	{
		heroRoadItem.gameObject.transform.parent = rootItem.transform;
		heroRoadItem.transform.localPosition = new Vector3(0,0,0);
		heroRoadItem.buttonFight.SetActive (false);
		heroRoadItem.descLabel.gameObject.SetActive (true);
	}
		
	void UpdateTalent ()
	{
		Utils.DestoryChilds (roadItemRoot);
		HeroRoad heroRoad = heroRoadItem.heroRoad;
		MissionSample[] missions=heroRoad.getMissionsByChapter ();
		if (missions == null) return;
		EvolutionSample sample = EvolutionManagerment.Instance.getEvoInfoByType (heroRoad.sample.sid);
		if (sample == null) return;
		int[] awakeInfo = heroRoad.getAwakeInfo ();
		for (int i = 0,j=0; i < awakeInfo.Length; i++) 
		{
			if (awakeInfo [i] == -1)
				continue;
			int state = awakeInfo [i];
			GameObject talentitemObj= Instantiate (talentitem) as GameObject;
		    CardAttrTalentItem talentItem=talentitemObj.GetComponent<CardAttrTalentItem>();
			talentItem.text2.text = missions[i].other[3];
			talentItem.gameObject.SetActive(true);
			talentItem.transform.parent=roadItemRoot.transform;
			talentItem.transform.localPosition=new Vector3(0,-115+(j*-100),0);
			talentItem.transform.localScale=new Vector3(0.8f,0.8f,1);
			if (heroRoad.activeCount < (i+1))
			{
				talentItem.text2.color = Color.gray;
			}
			else
			{
				talentItem.text2.color = Color.white;
			}
			talentItem.text1.gameObject.SetActive (true);
			talentItem.text1.text = string.Format (LanguageConfigManager.Instance.getLanguage ("s0445"), i + 1, getTalentNeedTimes(sample,j));
			j++;
		}
	}

	private int getTalentNeedTimes(EvolutionSample sample,int index)
	{
		int loc=index;
		if (loc > sample.getTalentNeedTimes ().Length - 1)
			loc = sample.getTalentNeedTimes ().Length-1;
		if (loc < 0)
			loc = 0;
		return sample.getTalentNeedTimes()[loc];
	}

	public void doClieckEvent(TeamPrepareWindow window,GameObject gameObject,int m_sid)
	{
		if(gameObject.name == "startHeroRoadButton")
		{
			int teamId=ArmyManager.PVE_TEAMID;

			GuideManager.Instance.doGuide (); 
			GuideManager.Instance.closeGuideMask ();	


			//英雄之章中点击挑战,玩家可能在其他副本并且行动力不足
			MissionSample sample = MissionSampleManager.Instance.getMissionSampleBySid (m_sid);
			int cSid = sample.chapterSid;
			int type = ChapterSampleManager.Instance.getChapterSampleBySid (cSid).type;
            if (type != ChapterType.HERO_ROAD && UserManager.Instance.self.getPvEPoint () < 1) {
				UiManager.Instance.openDialogWindow<PveUseWindow> ();
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

			int requestCombat=sample.getRecommendCombat();

			//战斗力不足提示
			if (currentCombat < requestCombat) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.dialogCloseUnlockUI=false;
					string tip=(sample.teamType==TeamType.Main)?Language("combatTip_01",requestCombat.ToString()):Language("combatTip_02",requestCombat.ToString());
					win.initWindow (2, Language ("s0094"), Language ("s0093"), tip, window.msgBack);
				});
				return;
			}

			window.intoFubenBack ();
		}
	}
}
