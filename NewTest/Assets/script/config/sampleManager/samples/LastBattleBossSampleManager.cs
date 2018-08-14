using System;
 

public class LastBattleBossSampleManager:SampleConfigManager
{
    //单例
	private static LastBattleBossSampleManager instance;

	public LastBattleBossSampleManager()
	{
		base.readConfig(ConfigGlobal.CONFIG_LASTBATTLEBOSS);
	}

	public static LastBattleBossSampleManager Instance {
        get {
            if (instance == null)
				instance = new LastBattleBossSampleManager();
            return instance;
        }
        set {
            instance = value;
        }
    }

    //解析模板数据
    public override void parseSample(int sid) {
		LastBattleBossSample sample = new LastBattleBossSample();
        string dataStr = getSampleDataBySid(sid);
        sample.parse(sid, dataStr);
        samples.Add(sid, sample);
    }

    //获得模板对象
	public LastBattleBossSample getBossInfoSampleBySid(int sid) {
        if (!isSampleExist(sid)) {
            createSample(sid);
        }
		return samples[sid] as LastBattleBossSample;
    } 
} 

