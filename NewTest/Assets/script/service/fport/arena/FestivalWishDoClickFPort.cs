using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 许愿点击事件通信
 * */
public class FestivalWishDoClickFPort : BaseFPort
{
    CallBack callback;
  
    public void access (int sid, CallBack callback)
	{   
        this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.DO_FESTIVALWISH_INFO);
		message.addValue("sid",new ErlInt(sid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType erl = message.getValue ("msg") as ErlType;
		string msg = erl.getValueString();
		if(msg == "ok")
		{
			if (callback != null)
			{
				callback();
				callback = null;
			}
		}
		else
		{
			MessageWindow.ShowAlert (msg);
			if (callback != null)
				callback = null;
		}
	}
}
