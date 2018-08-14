using UnityEngine;
using System;

/**
 * 购买挑战次数
 * @author yxl
 * */
public class ArenaBuyChallengeCountFport : BaseFPort
{
    CallBack<bool> callback;

    public void access (int number,CallBack<bool> callback)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_BUY_CHALLENGE_COUNT);
        message.addValue("number", new ErlString(number.ToString()));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
        if (str != null && callback != null)
        {
            callback(str.getValueString() == "ok");
        }
	}
}
