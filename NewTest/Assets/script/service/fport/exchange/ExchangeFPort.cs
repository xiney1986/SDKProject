using System;
 
/**
 * 兑换接口
 * @auhtor longlingquan
 * */
public class ExchangeFPort:BaseFPort
{
	CallBack<int,int> callback;
	int exchangeID;
	int exchangeNum;

	public ExchangeFPort ()
	{
		
	}
	
	public void exchange (int sid, int num, CallBack<int,int> callback)
	{
		exchangeID = sid;
		exchangeNum = num;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EXCHANGE_GET);  
		message.addValue ("sid", new ErlInt (sid));//商品sid
		message.addValue ("num", new ErlInt (num));//商品数量
		access (message);
	
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == FPortGlobal.SYSTEM_OK) {
			callback (exchangeID, exchangeNum);
		}else if(type.getValueString()==FPortGlobal.SYSTEM_FALSE_L){
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("sli001l"));
			if (callback != null)
				callback = null;
		}else {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("sli002l"));
			if (callback != null)
				callback = null;
		}
	}
} 

