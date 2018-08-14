using UnityEngine;
using System.Collections;

public class LotteryBuyFPort : BaseFPort
{
	private CallBack callBack;

	public void lotteryBuyFPortAccess(int sid,string num,CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.BUY_LOTTERY);
		message.addValue("sid",new ErlInt(sid));
		message.addValue("numbers",new ErlString(num));
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
			//LotteryManagement.Instance.selectNumList.Clear();
		}
	}
}
