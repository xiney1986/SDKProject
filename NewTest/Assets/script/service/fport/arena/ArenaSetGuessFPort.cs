using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得竞猜信息
 * @author yxl
 * */
public class ArenaSetGuessFPort : BaseFPort
{
    CallBack<bool> callback;
  
    public void access (CallBack<bool> callback,int setp,int index,string winuid)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_SET_GUESS);
        message.addValue("arena_step",new ErlString(setp.ToString()));
        message.addValue("index",new ErlString(index.ToString()));
        message.addValue("winuid",new ErlString(winuid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
        if(callback !=null)
        {
            callback(str.getValueString() == "ok");
        }
	}
}
