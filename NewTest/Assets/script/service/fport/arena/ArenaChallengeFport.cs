using UnityEngine;
using System;

/**
 * 海选战斗
 * @author yxl
 * */
public class ArenaChallengeFport : BaseFPort
{
    CallBack<bool> callback;

    public void access (CallBack<bool> callback,int index)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_CHALLENGE);
        message.addValue("index", new ErlString(index.ToString()));
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
