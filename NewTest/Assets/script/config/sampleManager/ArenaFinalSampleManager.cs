using System;
using System.Collections.Generic;
/**
 * 决赛竞猜奖励
 * */
public class ArenaFinalSampleManager:SampleConfigManager
{
	//单例
	private static ArenaFinalSampleManager instance;

    public ArenaFinalSampleManager ()
	{
        base.readConfig (ConfigGlobal.CONFIG_ARENA_FINAL);
	}

    public static ArenaFinalSampleManager Instance {
		get{
			if(instance==null)
				instance=new ArenaFinalSampleManager();
			return instance;
		}
	}

    public override void parseSample (int sid)
    {
        ArenaFinalSample sample = new ArenaFinalSample (); 
        string dataStr = getSampleDataBySid (sid); 
        sample.parse (sid, dataStr); 
        samples.Add (sid, sample);
    }

    public ArenaFinalSample getSample(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples[sid] as ArenaFinalSample;
    }
}  