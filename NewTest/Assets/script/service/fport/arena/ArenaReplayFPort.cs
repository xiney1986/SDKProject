using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 回看
 * @author yxl
 * */
public class ArenaReplayFPort : BaseFPort
{
    CallBack callback;
  
    public void access (CallBack callback,int index,int index1,int index2)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_REPLAY);
        message.addValue("arena_step",new ErlString(index.ToString()));
        message.addValue("index1",new ErlString(index1.ToString()));
        message.addValue("index2",new ErlString(index2.ToString()));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlArray array = message.getValue ("msg") as ErlArray;
        
        if (callback != null)
        {
            callback();
        }
	}
}
