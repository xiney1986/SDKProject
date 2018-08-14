using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获取决赛信息
 * @author yxl
 * */
public class ArenaFinalFPort : BaseFPort
{
    CallBack callback;
  
    public void access (CallBack callback)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_FINAL);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlArray array = message.getValue ("msg") as ErlArray;
        if (array != null)
        {
			ArenaManager.instance.self = null;
            int state = ArenaManager.instance.state = StringKit.toInt(array.Value[0].getValueString());
            ArenaManager.instance.stateEndTime = StringKit.toInt(array.Value[1].getValueString());
            ArenaManager.instance.finalRound = StringKit.toInt(array.Value[2].getValueString());
            ArenaManager.instance.finalCD = StringKit.toInt(array.Value[3].getValueString()) + 60;
            if(ArenaManager.instance.finalCD < 60)
            {
                ArenaManager.instance.finalDelay = ArenaManager.instance.finalCD;
            } else
            {
                ArenaManager.instance.finalDelay = 0;
            }

            if(state >= ArenaManager.STATE_64_32 && state <= ArenaManager.STATE_RESET)
            {
                array = array.Value[4] as ErlArray;
                ArenaManager.instance.finalInfoList = new List<ArenaFinalInfo>();
                for(int i = 0; i < 127; i++)
                {
                    int pos = 0;
                    ErlArray arr = array.Value[i] as ErlArray;
                    ArenaFinalInfo info = new ArenaFinalInfo();
                    info.startTime = StringKit.toInt(arr.Value[pos++].getValueString());

                    if(arr.Value.Length > 2)
                    {
                        info.guessStartTime = StringKit.toInt(arr.Value[pos++].getValueString());
                        info.guessEndTime = StringKit.toInt(arr.Value[pos++].getValueString());

                        info.uid = arr.Value[pos++].getValueString();
                        info.userName = arr.Value[pos++].getValueString();
                        info.userIcon = StringKit.toInt(arr.Value[pos++].getValueString());
                        info.lose = bool.Parse(arr.Value[pos++].getValueString());
                        info.index = StringKit.toInt(arr.Value[pos++].getValueString());
                        info.guessed = bool.Parse(arr.Value[pos++].getValueString());
                    }
                    else if(arr.Value[pos].getValueString() != "none")
                    {
                        info.userId = arr.Value[pos].getValueString();
                    }
                    ArenaManager.instance.finalInfoList.Add(info);
                }
            }
            else
            {
                ArenaManager.instance.finalInfoList = null;
            }
        }
        else
        {
            ArenaManager.instance.finalInfoList = null;
            ArenaManager.instance.state = -1;
        }
        if (callback != null)
        {
            callback();
			callback = null;
        }
	}
}
