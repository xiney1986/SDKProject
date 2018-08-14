using UnityEngine;
using System.Collections;

public class StarPrayFPort : BaseFPort
{
	CallBack callback;
	CallBack<int> callbacki;

	public void access (CallBack<int> callbacki)
	{
		this.callbacki = callbacki;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STAR_PRAY);
		access (message);
	}

	public void access (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STAR_PRAY);
		access (message);
	}

	public void access (CallBack callback, int goodsID, int num)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STAR_PRAY2);
		message.addValue ("goods_id", new ErlInt (goodsID));//商品sid
		message.addValue ("num", new ErlInt (num));//商品数量
		access (message);
	}

	//[[[1,[281479271677953,6,巨石的华莱土,0,17,9],11426,11426,2]],7200]
	//[[[2,10,11427,11423,2]],7200]
	//[摇结果信息,CD] 
	//摇结果信息=[一次,...]
	//一次=[1=玩家|2=女神,[role_uid,style,name,vip,level,star]]=摇到得玩家|star=女神,普通sid,幸运sid,倍数
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		ErlArray msg = message.getValue ("msg") as ErlArray;
		if (msg != null) {
			HoroscopesManager.Instance.cacheResult (msg, message.getValue ("cd") as ErlType);
			if (callback != null)
				callback ();
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
}
