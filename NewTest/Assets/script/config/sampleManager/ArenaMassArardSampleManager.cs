using System;
 
/**
 * 经验模板管理器
 * @author longlingquan
 * */
public class ArenaMassArardSampleManager:SampleConfigManager
{
	//单例
	private static ArenaMassArardSampleManager instance;

    public ArenaMassArardSampleManager ()
	{
        base.readConfig (ConfigGlobal.CONFIG_ARENAMASSAWARD);
	}

	public static ArenaMassArardSampleManager Instance {
		get{
			if(instance==null)
				instance=new ArenaMassArardSampleManager();
			return instance; 
		}
	}
	
	 
	//解析经验值数据
	public override void parseSample (int sid)
	{
        ArenaMassArardSample sample = new ArenaMassArardSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

    public ArenaMassArardSample getSampleBySid(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples [sid] as ArenaMassArardSample;
    }
}  