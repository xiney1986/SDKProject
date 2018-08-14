using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得竞猜奖励信息
 * @author yxl
 * */
public class ArenaReceiveAwardGuessFPort : BaseFPort
{
    CallBack<bool> callback;
  
    public void access (CallBack<bool> callback,int id)
    {   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_RECEIVE_AWARD_GUESS);
        message.addValue("arena_step", new ErlString(id.ToString()));
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
