using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得决赛奖励信息
 * @author yxl
 * */
public class ArenaGetAwardInfoFinalFPort : BaseFPort
{
    CallBack<List<ArenaAwardInfo>,bool> callback;
  
    public void access (CallBack<List<ArenaAwardInfo>,bool> callback)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_AWARD_INFO_FINAL);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        List<ArenaAwardSample> samples = ArenaAwardSampleManager.Instance.getSamplesByType(ArenaAwardWindow.TYPE_FINAL);
        List<ArenaAwardInfo> infos = new List<ArenaAwardInfo>();

		ArenaAwardSample sample;
		for(int i=samples.Count-1;i>=0;i--)
        {
			sample=samples[i];
            ArenaAwardInfo info = new ArenaAwardInfo();
            info.sample = sample;
            infos.Add(info);
        }
        ErlType type = message.getValue("msg") as ErlType;
        bool flag = type.getValueString() == "1";

        if (callback != null)
        {
            callback(infos,flag);
        }
    }
}
