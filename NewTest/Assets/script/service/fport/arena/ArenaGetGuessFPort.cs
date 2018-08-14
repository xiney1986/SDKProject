using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得竞猜信息
 * @author yxl
 * */
public class ArenaGetGuessFPort : BaseFPort
{
    CallBack<ArenaGuessUser[]> callback;
  
    public void access (CallBack<ArenaGuessUser[]> callback,int setp,int index)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_GUESS);
        message.addValue("arena_step",new ErlString(setp.ToString()));
        message.addValue("index",new ErlString(index.ToString()));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
    { 
        ErlArray str = message.getValue ("msg") as ErlArray;
        if (str != null)
        {
            ArenaGuessUser[] users = new ArenaGuessUser[2];
            for(int i = 0; i < 2; i++)
            {
                ErlArray arr = str.Value[i] as ErlArray;
                if(arr != null)
                {
                    int pos = 0;
                    ArenaGuessUser user = new ArenaGuessUser();
                    user.uid = arr.Value[pos++].getValueString();
                    user.name = arr.Value[pos++].getValueString();
                    user.headIcon = StringKit.toInt(arr.Value[pos++].getValueString());
                    user.level = StringKit.toInt(arr.Value[pos++].getValueString());
                    user.guild = arr.Value[pos++].getValueString();
                    user.select = bool.Parse(arr.Value[pos++].getValueString());
                    users[i] = user;
                }
            }

            if(callback !=null)
            {
                callback(users);
            }
        }
	}
}
