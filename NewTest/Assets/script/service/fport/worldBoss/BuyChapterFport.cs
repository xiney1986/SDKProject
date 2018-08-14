using UnityEngine;
using System.Collections;

/// <summary>
/// 世界首领通讯端口
/// </summary>
public class BuyChapterFport : BaseFPort {

	/** 回调函数 */
	private CallBack callback;
	private int sid;
	private int num;

	public void buyGoods (int sid, int num, CallBack callback)
	{  
		this.sid = sid;
		this.num = num;
		this.callback = callback;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.SHOP_BUY);  
		message.addValue ("goods_id", new ErlInt (sid));//商品sid
		message.addValue ("num", new ErlInt (num));//商品数量
		access (message);
	}
	public override void read (ErlKVMessage message) {
		base.read (message);
		ErlArray erlArray = message.getValue ("msg") as ErlArray;
		int index = 0;
		string returnType = erlArray.Value [index++].getValueString ();

		if (callback != null) {
			callback ();
			callback = null;
		}
	}

}
