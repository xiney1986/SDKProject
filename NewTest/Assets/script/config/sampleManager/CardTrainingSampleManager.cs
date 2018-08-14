using UnityEngine;
using System.Collections;

public class CardTrainingSampleManager : SampleConfigManager
{

    public static CardTrainingSampleManager Instance
    {
        get
        {
            return SingleManager.Instance.getObj("CardTrainingSampleManager") as CardTrainingSampleManager;
        }
    }

    public CardTrainingSampleManager()
    {
        base.readConfig(ConfigGlobal.CONFIG_CARDTRAINING);
    }

    //获得模板对象
    public CardTrainingSample getDataBySid(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples[sid] as CardTrainingSample;
    }

    //解析模板数据
    public override void parseSample(int sid)
    {
        CardTrainingSample sample = new CardTrainingSample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }
}
