using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得竞猜奖励信息
 * @author yxl
 * */
public class ArenaGetAwardInfoGuessFPort : BaseFPort
{
    CallBack<List<ArenaAwardInfo>> callback;
  
    public void access (CallBack<List<ArenaAwardInfo>> callback)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_AWARD_INFO_GUESS);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        Dictionary<int,int> map = new Dictionary<int, int>();
        ErlArray array = message.getValue ("msg") as ErlArray;
        if (array != null)
        {
            for(int i = 0; i < array.Value.Length; i+=2)
            {
                int sid = StringKit.toInt(array.Value[i].getValueString());
                int num = StringKit.toInt(array.Value[i+1].getValueString());
                map.Add(sid,num);
            }
        }

        List<ArenaAwardSample> samples = ArenaAwardSampleManager.Instance.getSamplesByType(ArenaAwardWindow.TYPE_GUESS);
        List<ArenaAwardInfo> infos = new List<ArenaAwardInfo>();
        foreach (ArenaAwardSample sample in samples)
        {
            ArenaAwardInfo info = new ArenaAwardInfo();
            info.sample = sample;
            info.condition = map[sample.condition];
            infos.Add(info);
        }
        if (callback != null)
        {
            callback(infos);
        }
    }
}
