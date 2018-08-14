using System;
 
/**装备模板管理器
  *负责装备模板信息的初始化 
  *@author longlingquan
  **/
public class TowerAwardSampleManager:SampleConfigManager
{
	//单例
	private static TowerAwardSampleManager instance;

    public TowerAwardSampleManager()
	{

        base.readConfig(ConfigGlobal.CONFIG_CLMB_AWARD);
	}

    public static TowerAwardSampleManager Instance {
		get{
			if(instance==null)
                instance = new TowerAwardSampleManager();
			return instance;
		}
	} 
	
	//获得装备模板对象
	public TowerAwardSample getTowerAwardSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
        return samples[sid] as TowerAwardSample;
	}   
	
	//解析模板数据
	public override void parseSample (int sid)
	{
        TowerAwardSample sample = new TowerAwardSample(); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
} 

