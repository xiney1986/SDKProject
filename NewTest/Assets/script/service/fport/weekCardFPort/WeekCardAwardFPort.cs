using UnityEngine;
using System.Collections;

public class WeekCardAwardFPort : BaseFPort
{
	private CallBack callBack;
	
	public WeekCardAwardFPort()
	{
		
	}

	public void access (int sid, CallBack callBack)
	{
		this.callBack = callBack;
		ErlKVMessage message = new ErlKVMessage (FrontPort.WEEKCARD_AWARD); 
		message.addValue ("sid", new ErlInt (sid));//领取条目id
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
			result = LanguageConfigManager.Instance.getLanguage("weekCard_"+type.getValueString());
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, result, null);
			});
		}
	}
}
