using UnityEngine;
using System;

/**
 * 清楚挑战等待CD
 * @author yxl
 * */
public class ArenaResetCDFPort : BaseFPort
{
    CallBack<bool> callback;
  
    public void access (CallBack<bool> callback)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_RESET_CD);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
        if (callback != null)
        {
            callback(str != null && str.getValueString() == "ok");
        }
	}
}
