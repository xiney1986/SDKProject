using UnityEngine;
using System.Collections;

public class RebateSendFPort : BaseFPort
{
	private CallBack callBack;
	public RebateSendFPort()
	{

	}

	public void access (int index, CallBack callBack)
	{
		this.callBack = callBack;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SEND_REBATE); 
		message.addValue ("index", new ErlInt (index));//领取第几天
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
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, result, null);
			});
		}
	}
}
