using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得决赛奖励信息
 * @author yxl
 * */
public class ArenaReceiveAwardFinalFPort : BaseFPort
{
    CallBack<bool> callback;
  
    public void access (CallBack<bool> callback)
	{   
        this.callback = callback; 
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_RECEIVE_AWARD_FINAL);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType msg = message.getValue ("msg") as ErlType;
        if (callback != null)
        {
            callback(msg != null && msg.getValueString() == "ok");
        }
    }
}
