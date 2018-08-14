using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得决赛奖励信息
 * @author yxl
 * */
public class ArenaReceiveAwardIntegralFPort : BaseFPort
{
    CallBack<bool> callback;
  
    public void access (CallBack<bool> callback,int awardSid,int type)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_RECEIVE_AWARD_INTEGRAL);
        message.addValue("sid", new ErlString(awardSid.ToString()));
        message.addValue("hit",new ErlInt(type));
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
