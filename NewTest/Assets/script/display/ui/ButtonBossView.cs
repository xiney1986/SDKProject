using UnityEngine;
using System.Collections;

public class ButtonBossView : ButtonBase
{
	public UITexture cardImage;
	public UISprite lockBack;
	Mission mission;
	
	public void  updateBoss (Mission mis)
	{
		mission = mis;
		if (mis == null)
			return;
		Card boss = CardManagerment.Instance.createCard (mis.getBossSid ());
		if (boss == null)
			return;
		cardImage.alpha = 1;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + boss.getImageID (), cardImage);
		if (UserManager.Instance.self.getUserLevel () < mission.getRequirLevel ()) {
			showLevelLock ();
		} else if (!FuBenManagerment.Instance.isCompleteLastMission (mis.sid)) {
			showCompleteLock ();
		} else {
			hideLevelLock ();
		}
	}

	void hideLevelLock ()
	{
		lockBack.gameObject.SetActive (false);
		textLabel.gameObject.SetActive (false);
	}

	void showLevelLock ()
	{
		lockBack.gameObject.SetActive (true);
		textLabel.gameObject.SetActive (true);
		textLabel.text = "Lv." + mission.getRequirLevel () + LanguageConfigManager.Instance.getLanguage ("s0160");
		textLabel.color = Color.red;
	}
	
	void showCompleteLock ()
	{
		lockBack.gameObject.SetActive (true);
		textLabel.gameObject.SetActive (true);
		textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0175");
		textLabel.color = Color.red;
	}
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
	} 
}
