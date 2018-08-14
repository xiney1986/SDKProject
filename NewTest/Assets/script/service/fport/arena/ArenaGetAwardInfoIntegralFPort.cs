using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得积分奖励信息
 * @author yxl
 * */
public class ArenaGetAwardInfoIntegralFPort : BaseFPort
{
    CallBack<List<ArenaAwardInfo>> callback;
  
    public void access (CallBack<List<ArenaAwardInfo>> callback)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_AWARD_INFO_INTEGRAL);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
        int myIntegeral = 0;
        List<int> sids = new List<int>();
        ErlArray array = message.getValue ("msg") as ErlArray;
        if (array != null)
        {
            myIntegeral = StringKit.toInt(array.Value[0].getValueString());
            array = array.Value[1] as ErlArray;
            if(array != null)
            {
                for(int i = 0; i < array.Value.Length; i++)
                {
                    int sid = StringKit.toInt(array.Value[i].getValueString());
                    sids.Add(sid);
                }
            }
        }
        ArenaManager.instance.finalMyIntergal = myIntegeral;

        List<ArenaAwardSample> samples = ArenaAwardSampleManager.Instance.getSamplesByType(ArenaAwardWindow.TYPE_INTEGRAL);
		ArenaAwardManager.Instance.integralAwardInfos.Clear ();
        foreach (ArenaAwardSample sample in samples)
        {
            ArenaAwardInfo info = new ArenaAwardInfo();
            info.sample = sample;
            info.condition = myIntegeral;

            for(int i = 0; i < sids.Count; i++)
            {
                if(sids[i] == sample.sid)
                {
                    info.received = true;
                    break;
                }
            }

			ArenaAwardManager.Instance.integralAwardInfos.Add(info);
        }
        if (callback != null)
        {
			callback(ArenaAwardManager.Instance.integralAwardInfos);
			callback = null;
        }
    }
}
