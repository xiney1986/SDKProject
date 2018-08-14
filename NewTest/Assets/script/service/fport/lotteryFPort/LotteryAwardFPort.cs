using UnityEngine;
using System.Collections;

public class LotteryAwardFPort : BaseFPort
{
	private CallBack callBack;

	public void lotteryAwardFPorttAccess(int sid,int itemID,CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LOTTERY_AWARD);
		message.addValue("sid",new ErlInt(sid));
		message.addValue("item",new ErlInt(itemID));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		string result = type.getValueString();
		string strTips = "";
		// 选注成功//
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
			strTips = LanguageConfigManager.Instance.getLanguage("Lottery_" + result);
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, strTips, null);
			});
		}
	}
}
