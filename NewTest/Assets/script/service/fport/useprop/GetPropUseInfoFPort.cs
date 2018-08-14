using UnityEngine;
using System;

/**
 * 获取某个道具的当日使用次数
 * @author 杨小珑
 * */
public class GetPropUseInfoFPort : BaseFPort
{
	private CallBack<int> callback;
	
	public void access (int sid, CallBack<int> callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_PROP_USE_INFO); 
		message.addValue ("sid", new ErlInt (sid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray ls = message.getValue ("msg") as ErlArray;
		callback(StringKit.toInt(ls.Value[1].getValueString()));
	}
}
