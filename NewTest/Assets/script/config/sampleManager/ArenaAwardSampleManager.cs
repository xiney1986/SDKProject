using System;
using System.Collections.Generic;
/**
 * 竞技场奖励模板管理器
 * */
using UnityEngine;

public class ArenaAwardSampleManager:SampleConfigManager
{
	//单例
	private static ArenaAwardSampleManager instance;

	public ArenaAwardSampleManager ()
	{

        base.readConfig (ConfigGlobal.CONFIG_ARENA_AWARD);
	}
	
	public static ArenaAwardSampleManager Instance {
		get{
			if(instance==null)
				instance=new ArenaAwardSampleManager();
			return instance;
		}
	} 
	

	public ArenaAwardSample getArenaAwardSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as ArenaAwardSample;
	}

	 
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		ArenaAwardSample sample = new ArenaAwardSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	public List<ArenaAwardSample> getSamplesByType(int type)
    {
        List<ArenaAwardSample> list = new List<ArenaAwardSample>();
        int[] ids = new int[data.Count];
        int pos = 0;
        foreach (object key in data.Keys)
        {
            ids[pos++] = (int)key;
        }
        Array.Sort(ids);

        for(int i = 0; i < ids.Length; i++)
        {
            ArenaAwardSample sample = getArenaAwardSampleBySid(ids[i]);
            if(sample.type == type)
            {
                list.Add(sample);
            }
        }
        return list;
    }

    /// <summary>
    /// 根据赛事ID获取奖励描述
    /// </summary>
    public string getGuessPrizeDescription(int finalEventId)
    {
        foreach(object key in data.Keys)
        {
            ArenaAwardSample sample = getArenaAwardSampleBySid((int)key);
            if(sample.type == ArenaAwardSample.TYPE_GUESS && sample.condition == finalEventId)
            {
                return sample.prizeDescription;
            }
        }
        return "";
    }

    /// <summary>
    /// 获取竞猜积分奖励领取方式（普通，双倍，3倍）
    /// </summary>
    /// <returns></returns>
    public int getIntegralPrizesType(int sid)
    {
        ArenaAwardSample sample = getArenaAwardSampleBySid(sid);
        return StringKit.toInt(sample.prizeDescription);
    }
}

