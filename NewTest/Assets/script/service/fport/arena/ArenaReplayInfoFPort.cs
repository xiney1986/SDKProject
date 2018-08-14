using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获取回看信息
 * @author yxl
 * */
public class ArenaReplayInfoFPort : BaseFPort
{
    CallBack<ArenaReplayInfo> callback;
  
    public void access (CallBack<ArenaReplayInfo> callback,int arena_step,int index)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_REPLAY_INFO);
        message.addValue("arena_step",new ErlString(arena_step.ToString()));
        message.addValue("index",new ErlString(index.ToString()));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlArray array = message.getValue ("msg") as ErlArray;
        if (array != null)
        {
            ArenaReplayInfo info = new ArenaReplayInfo();
            int score1 = StringKit.toInt(array.Value[0].getValueString());
            int score2 = StringKit.toInt(array.Value[1].getValueString());

            //user1
            ErlArray arr = array.Value[2] as ErlArray;
            info.user1 = new ArenaReplayInfoUser();
            info.user1.score = score1;
            info.user1.uid = arr.Value[0].getValueString();
            info.user1.name = arr.Value[1].getValueString();
            info.user1.style = StringKit.toInt(arr.Value[2].getValueString());
            info.user1.win = bool.Parse(arr.Value[3].getValueString());

            //user2
            arr = array.Value[3] as ErlArray;
            info.user2 = new ArenaReplayInfoUser();
            info.user2.score = score2;
            info.user2.uid = arr.Value[0].getValueString();
            info.user2.name = arr.Value[1].getValueString();
            info.user2.style = StringKit.toInt(arr.Value[2].getValueString());
            info.user2.win = bool.Parse(arr.Value[3].getValueString());

            
            arr = array.Value[4] as ErlArray;
            info.winUids = new List<string>();
            foreach(ErlType er in arr.Value)
            {
                info.winUids.Add(er.getValueString());
            }

            if (callback != null)
            {
                callback(info);
            }
        }
	}
}
