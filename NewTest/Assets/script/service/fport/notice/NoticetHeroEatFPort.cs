using UnityEngine;
using System.Collections;

public class NoticetHeroEatFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_HERO_EAT);
		message.addValue ("affiche_id", new ErlInt (NoticeManagerment.Instance.getHeroEatInfo () [0]));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string msg = (message.getValue ("msg") as ErlType).getValueString (); 
		if (msg == "ok") {
			NoticeManagerment.Instance.getHeroEatInfo () [3] += 1;
			if (callback != null)
				callback ();
		} else if (msg == "not_ative_time") {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("goddnessShake18"));
			callback = null;
		}
		else {
			MessageWindow.ShowAlert (msg);
			if (callback != null)
				callback = null;
		}
	}
}
