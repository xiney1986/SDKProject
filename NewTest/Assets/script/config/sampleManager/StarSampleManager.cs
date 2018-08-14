using UnityEngine;
using System.Collections;

public class StarSampleManager : SampleConfigManager
{
    private static StarSampleManager _Instance;
    private static bool _singleton = true;

    public StarSampleManager()
	{
        if (_singleton)
            return;
        base.readConfig(ConfigGlobal.CONFIG_STAR);
	}

    public static StarSampleManager Instance
    {
        get
        {

            if (_Instance == null)
            {
                _singleton = false;
                _Instance = new StarSampleManager();
                _singleton = true;
                return _Instance;
            }
            else
                return _Instance;
        }
        set
        {
            _Instance = value;
        }
    }

    public override void parseSample(int sid)
    {
        StarSample sample = new StarSample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }

    //获得经验值模板对象
    private StarSample getStarSampleBySid(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples[sid] as StarSample;
    }

    public int getIconId(int sid)
    {
        return getStarSampleBySid(sid).iconId;
    }

    public string getName(int sid)
    {
        return getStarSampleBySid(sid).name;
    }

    public string getLanguageId(int sid)
    {
        return getStarSampleBySid(sid).languageId;
    }
	
}
