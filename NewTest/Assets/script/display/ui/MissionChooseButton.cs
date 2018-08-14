using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionChooseButton : ButtonBase
{

	public UISprite sign;
	public UILabel levelLimit;
	public UILabel missionName;
	public Mission mission;
	public GameObject newEffect;
	public GameObject special;//特殊关卡相关信息


	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		GuideManager.Instance.doGuide ();
		if (UserManager.Instance.self.getPvEPoint () == 0) { 
			UiManager.Instance.openDialogWindow <PveUseWindow> ();
		} else{
			//设置选择的副本过程sid
			FuBenManagerment.Instance.selectedMissionSid=mission.sid;
			UiManager.Instance.openWindow<TeamPrepareWindow> ((win) => {
				win.Initialize (mission,TeamPrepareWindow.WIN_MISSION_ITEM_TYPE);
				win.missionWinItem.GetComponent<MissionWinItem>().updateButton(mission);
			}
			);
		}
	}

	public void updateButton (Mission mission )
	{
		this.mission = mission;
		string spName = mission.getMissionType ();

		if (spName == MissionShowType.NEW || spName == MissionShowType.COMPLET) {
			sign.gameObject.SetActive (true);
			sign.spriteName = spName;
		} else {
			sign.gameObject.SetActive (false);
		}

		if (spName == MissionShowType.NEW) {
			PlayerPrefs.SetInt (UserManager.Instance.self.uid + PlayerPrefsComm.MISSION_NEW, mission.sid);
			sign.gameObject.SetActive (false);
			newEffect.SetActive (true);

//			GameObject obj = (fatherWindow as MissionChooseWindow).signNew;
//			obj.SetActive (true);
//			obj.transform.position = sign.transform.position;
//			iTween.ScaleTo (obj, iTween.Hash ("x", 1, "y", 1, "time", 0.2f, "easetype", "linear"));
//			StartCoroutine (Utils.DelayRun (() => {
//				obj.SetActive (false);
//				sign.gameObject.SetActive (true);
//			}, 0.2f));
		}

		missionName.text = mission.getMissionName ();

		if (spName == MissionShowType.LEVEL_LOW) {
			levelLimit.gameObject.SetActive (true);
			levelLimit.text = "Lv." + mission.getRequirLevel () + LanguageConfigManager.Instance.getLanguage ("s0160");
		} else {
			levelLimit.gameObject.SetActive (false);
		}

		UIButton tmp=gameObject.GetComponent<UIButton>();

		if (StringKit.toInt(mission.getOther () [0]) == 1) {
			special.gameObject.SetActive(true);

			tmp.normalSprite="bar_1yellow";
			tmp.hoverSprite="bar_1yellow";
			tmp.pressedSprite="bar_1yellow";
		}else{
			tmp.normalSprite="bar_1";
			tmp.hoverSprite="bar_1CheckOn";
			tmp.pressedSprite="bar_1CheckOn";

		}
	} 

}
