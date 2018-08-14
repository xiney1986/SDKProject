using System;
 
/**装备模板管理器
  *负责装备模板信息的初始化 
  *@author longlingquan
  **/
public class MagicWeaponSampleManager:SampleConfigManager
{
	//单例
	private static MagicWeaponSampleManager instance;

    public MagicWeaponSampleManager()
	{

        base.readConfig(ConfigGlobal.CONFIG_M_WEAPON);
	}

    public static MagicWeaponSampleManager Instance {
		get{
			if(instance==null)
                instance = new MagicWeaponSampleManager();
			return instance;
		}
	} 
	
	//获得装备模板对象
	public MagicWeaponSample getMagicWeaponSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
        return samples[sid] as MagicWeaponSample;
	}   
	
	//解析模板数据
	public override void parseSample (int sid)
	{
        MagicWeaponSample sample = new MagicWeaponSample(); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
} 

