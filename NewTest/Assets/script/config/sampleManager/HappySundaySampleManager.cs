using System.Collections.Generic;
using System.Collections;

public class HappySundaySampleManager : SampleConfigManager
{

    public static HappySundaySampleManager Instance
    {
        get
        {
            return SingleManager.Instance.getObj("HappySundaySampleManager") as HappySundaySampleManager;
        }
    }

    public HappySundaySampleManager()
    {
        base.readConfig(ConfigGlobal.CONFIG_HAPPYSUNDAY);

        foreach (DictionaryEntry item in data)
        {
            if (item.Key != null)
                parseSample((int)item.Key);
        }
    }


    //获得模板对象
    public HappySundaySample getDataBySid(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples[sid] as HappySundaySample;
    }

    

    //解析模板数据
    public override void parseSample(int sid)
    {
        HappySundaySample sample = new HappySundaySample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }
}
