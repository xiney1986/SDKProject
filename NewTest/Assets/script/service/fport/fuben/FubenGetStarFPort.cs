using System;
using System.Collections.Generic;

/**
 * 获得副本星星
 * @author zhoujie
 * */
public class FubenGetStarFPort:BaseFPort
{
	public FubenGetStarFPort ()
	{
		
	}
	
	private CallBack<int> callbacki;
	private CallBack callback;
	
	public void getStar (CallBack<int> callbacki,CallBack callback)
	{
		this.callbacki = callbacki;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_FB_STAR);  
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		callbacki (StringKit.toInt(type.getValueString()));
		if(callback != null)
		callback ();
	}
}