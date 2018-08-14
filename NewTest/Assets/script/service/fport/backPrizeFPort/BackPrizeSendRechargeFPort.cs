using UnityEngine;
using System.Collections;

public class BackPrizeSendRechargeFPort : BaseFPort
{
	private CallBack callBack;

	public BackPrizeSendRechargeFPort()
	{
		
	}

	public void access (int sid, CallBack callBack)
	{
		this.callBack = callBack;
		ErlKVMessage message = new ErlKVMessage (FrontPort.BACKPRIZE_SENDRECHARGE); 
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
			if(BackPrizeRechargeInfo.Instance.receviedCount == BackRechargeConfigManager.Instance.getRechargeList().Count)// 领取奖励完毕，关闭入口//
			{
				BackPrizeRechargeInfo.Instance.isActive = false;
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
