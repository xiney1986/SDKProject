using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RefineSampleManager : SampleConfigManager
{
    private static RefineSampleManager instance;
    private static List<RefineSample> list;

    public RefineSampleManager ()
	{

        base.readConfig(ConfigGlobal.CONFIG_REFINE_EQUIP);
	}
    public static RefineSampleManager Instance
    {
        get
        {
            if (instance == null)
                instance = new RefineSampleManager();
            return instance;
        }
    }
    //获取精炼模板对象
    public RefineSample getRefineSampleBySid(int sid)
    {
        //if (!isSampleExist(sid))
        //    createSample(sid);
        //return samples[sid] as RefineSample;

        if (list == null || list.Count <= 0) return null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].sid == sid)
            {
                return list[i];
            }
        }
        return null;
    }


    public override void parseConfig(string str)
    {
        RefineSample be = new RefineSample(str);
        if (list == null)
            list = new List<RefineSample>();
        list.Add(be);
    }
    //public override void parseSample(int sid)
    //{
    //    RefineSample sample = new RefineSample();
    //    string dataStr = getSampleDataBySid(sid);
    //    sample.parse(sid, dataStr);
    //    samples.Add(sid, sample);
    //}
}
