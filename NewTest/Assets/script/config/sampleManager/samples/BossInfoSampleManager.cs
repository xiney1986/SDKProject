using System;
 

public class BossInfoSampleManager:SampleConfigManager
{
    //单例
    private static BossInfoSampleManager instance;

    public BossInfoSampleManager()
	{
        base.readConfig(ConfigGlobal.CONFIG_BOSS_INFO);
	}

    public static BossInfoSampleManager Instance {
        get {
            if (instance == null)
                instance = new BossInfoSampleManager();
            return instance;
        }
        set {
            instance = value;
        }
    }

    //解析模板数据
    public override void parseSample(int sid) {
        BossInfoSample sample = new BossInfoSample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }

    //获得模板对象
    public BossInfoSample getBossInfoSampleBySid(int sid) {
        if (!isSampleExist(sid)) {
            createSample(sid);
        }
        return samples[sid] as BossInfoSample;
    } 
} 

