using UnityEngine;
using System.Collections;

// 末日决战挑战小怪//
public class LastBattleFightFPort : BaseFPort
{
	private CallBack callBack;
	// sid 副本id//
	public void lastBattleFightAccess(CallBack _callBack,int sid)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LASTBATTLEFIGHT);
		message.addValue ("sid", new ErlInt (sid));//挑战小怪副本id
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		string result = type.getValueString();
		if(result == "ok")
		{
			if(callBack != null)
			{
				callBack();
				callBack = null;
			}
		}
		else
		{
			result = LanguageConfigManager.Instance.getLanguage("LastBattle_" + result);
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, result, null);
			});
		}
	}
}
