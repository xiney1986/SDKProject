
using System;
using UnityEngine;

public class ButtonHeroEat:ButtonBase
{
	[HideInInspector]
	public HeroEatContent
		content;
	int totalPveMax;
	int heroEatPve;

	void Awake ()
	{
		totalPveMax = CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample> (CommonConfigSampleManager.PvePowerMax_SID).pvePowerMax;
		heroEatPve = CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample> (CommonConfigSampleManager.PvePowerMax_SID).heroEatPve;
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
        
		if (UserManager.Instance.self.getPvEPoint () >= totalPveMax) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0361", totalPveMax.ToString()));
		} else if (UserManager.Instance.self.getPvEPoint () + heroEatPve > totalPveMax) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"),
				                LanguageConfigManager.Instance.getLanguage ("heroEatContent06", (totalPveMax - UserManager.Instance.self.getPvEPoint ()).ToString ()),
				                heroEat);
			});
		} else {
			heroEat (heroEatPve);
		}
	}

	private void heroEat (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			heroEat (totalPveMax - UserManager.Instance.self.getPvEPoint ());
		}
	}

	private void heroEat (int pve)
	{
		NoticetHeroEatFPort port = FPortManager.Instance.getFPort ("NoticetHeroEatFPort") as NoticetHeroEatFPort;
		port.access (() => {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("heroEatContent07", pve.ToString ()));
		});
	}
}

