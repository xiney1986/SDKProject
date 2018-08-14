using System;
using System.Collections.Generic;

/// <summary>
/// 天梯购买挑战次数的购买的请求
/// </summary>
public class LaddersBuyChallengeFport:BaseFPort
{
	private CallBack<bool> callback;

	public void access (int count,CallBack<bool> _callback)
	{
		callback = _callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_BUY_COUNT);	
		message.addValue ("number", new ErlInt (count));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string msg = (message.getValue ("msg") as ErlType).getValueString ();
		string info =string.Empty;
		switch(msg)
		{
			case "buy_limit":
				info=LanguageConfigManager.Instance.getLanguage("laddersTip_22");
				break;
			case "limit_rmb":
				info=LanguageConfigManager.Instance.getLanguage("laddersTip_23");
				break;
			case "num_limit":
				info=LanguageConfigManager.Instance.getLanguage("laddersTip_24");
				break;
			case "":
				info=LanguageConfigManager.Instance.getLanguage("laddersTip_25");
				break;
		}
		if(info!=string.Empty)
		{
			TextTipWindow.Show (info);		
		}
		if(callback != null) 
		{
			callback (msg.Equals("ok"));
			callback = null;
		}
	}
}


