using UnityEngine;
using System.Collections;
/// <summary>
/// 神秘商店购买端口
/// </summary>
public class MysticalShopBuyFPort : BaseFPort {
	private CallBack<int,int> callback;
	private int sid = 0;
	private int num = 0;
	public MysticalShopBuyFPort(){

	}
	public void buyGoods (int sid,int index, CallBack<int,int> callback)
	{  
		this.sid = sid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MYSTICAL_BUY);  
		message.addValue ("goods_id", new ErlInt (sid));
		message.addValue("index",new ErlInt(index));//商品sid
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		if (str == "ok") {
			if (callback != null)
				callback (sid, 1);
		} else {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0207"));
		}
		callback = null;
	}

}
