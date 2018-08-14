using UnityEngine;
using System.Collections;

public class LastBattleProcessAwardFPort : BaseFPort
{

	private CallBack callBack;

	// sid 进度奖励条目id//
	public void lastBattleAwardtAccess(CallBack _callBack,int sid)
	{
		this.callBack = _callBack;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.LASTBATTLEPROCESSAWARD);
		msg.addValue("sid",new ErlInt(sid));
		access(msg);
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
