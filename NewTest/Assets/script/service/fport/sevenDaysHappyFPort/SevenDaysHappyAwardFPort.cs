using UnityEngine;
using System.Collections;

public class SevenDaysHappyAwardFPort : BaseFPort
{
	private CallBack callBack;

	public void access (int missonID, int prizeID,CallBack callBack)
	{
		this.callBack = callBack;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SEVENDAYSHAPPY_REWARD); 
		message.addValue ("task_id", new ErlInt (missonID));//任务id
		message.addValue ("sid", new ErlInt (prizeID));//奖励id
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		string result = type.getValueString();
		if(result == "ok")// 领取成功//
		{
			if(callBack != null)
			{
				callBack();
			}
		}
		else
		{
			result = LanguageConfigManager.Instance.getLanguage("sevenDaysHappy_" + result);
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, result, null);
			});
		}
	}
}
