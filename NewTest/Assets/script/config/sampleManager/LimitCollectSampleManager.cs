using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LimitCollectSampleManager : SampleConfigManager {
    public static LimitCollectSampleManager Instance{
        get{
            if (instance == null)
                instance = new LimitCollectSampleManager();
            return instance;
        }
    }
    private static LimitCollectSampleManager instance;

    public LimitCollectSampleManager() {
        base.readConfig(ConfigGlobal.CONFIG_NOTICE_LIMIT_COLLECT);
    }

    /// <summary>
    /// 根据SID获取模版
    /// </summary>
    public LimitCollectSample getSampleBySid(int sid) {
        if (!samples.ContainsKey(sid) ){
            LimitCollectSample sample = new LimitCollectSample();
            sample.parse(sid, data[sid].ToString());
            samples.Add(sid,sample);
        }
        return samples[sid] as LimitCollectSample;
    }

    /// <summary>
    /// 获取所有模版
    /// </summary>
    public List<LimitCollectSample> getAllSample() {
        List<LimitCollectSample> allSample = new List<LimitCollectSample>();
        foreach (DictionaryEntry entry in data) {
            int sid = int.Parse(entry.Key.ToString());
            allSample.Add(getSampleBySid(sid));
        }
        return allSample;
    }
}
