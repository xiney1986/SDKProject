using UnityEngine;
using System.Collections;

// 末日决战挑战boss//
public class LastBattleBossBattleFPort : BaseFPort
{
	private CallBack callBack;

	public void lastBattleBossBattleAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LASTBATTLEBOSSBATTLE);
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
