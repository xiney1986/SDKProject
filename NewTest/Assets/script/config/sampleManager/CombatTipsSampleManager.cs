using UnityEngine;
using System.Collections;


public class CombatTipsSampleManager : SampleConfigManager
{
    public static CombatTipsSampleManager Instance
    {
        get { return SingleManager.Instance.getObj("CombatTipsSampleManager") as CombatTipsSampleManager; }
    }

    public ArrayList keyList;

    public CombatTipsSampleManager()
    {
        base.readConfig(ConfigGlobal.CONFIG_COMBATTIPS);
    }


    public ArrayList GetAllSample()
    {
        if (samples.Count != data.Count)
        {
            foreach (DictionaryEntry item in data)
            {
                if (item.Key != null)
                    parseSample((int)item.Key);
            }
            keyList = new ArrayList(samples.Keys);
            keyList.Sort();
        }
        return keyList;
    }


    //获得模板对象
    public CombatTipsSample getDataBySid(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples[sid] as CombatTipsSample;
    }

    //解析模板数据
    public override void parseSample(int sid)
    {
        CombatTipsSample sample = new CombatTipsSample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }




}
