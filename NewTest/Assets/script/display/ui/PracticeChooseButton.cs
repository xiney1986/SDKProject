using UnityEngine;
using System.Collections;

public class PracticeChooseButton : ButtonBase
{
	public UISprite sign;
	//public UILabel levelLimit;
	public UILabel missionName;
	//public Mission mission;
	Mission mission;

	public void updateButton (Mission mis)
	{
		mission = mis;
		textLabel.text = mission.getMissionName ();
		changeSign (mission.getMissionType ());
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (FuBenManagerment.Instance.getPracticeChapter ().getNum () == 0) {
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage ("s0343"));
			return;
		} else if (UserManager.Instance.self.getUserLevel () < mission.getRequirLevel ()) {
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage ("s0342"));
			return;
		}
		GuideManager.Instance.doGuide(); 
		fatherWindow.finishWindow ();
		UiManager.Instance.openWindow<TeamPrepareWindow>((win)=>{
			win.Initialize (mission.sid); 
		});
	}

	public void changeSign (string spName)
	{
	 
		if (spName == MissionShowType.NEW || spName == MissionShowType.COMPLET) {
			sign.gameObject.SetActive (true);
			sign.spriteName = spName;
		} else {
			sign.gameObject.SetActive (false);
		}
		textLabel.text = mission.getMissionName ();

	if (mission.getMissionType () == MissionShowType.NOT_COMPLETE_LAST_MISSION) {
			textLabel.gameObject.SetActive (true); 
			textLabel.text += "      " + Colors.RED + LanguageConfigManager.Instance.getLanguage ("s0175"); 
		}
	}
}
