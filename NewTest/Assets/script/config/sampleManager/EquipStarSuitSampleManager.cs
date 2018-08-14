using System;

/**
 * 套装模板管理器
 * */
public class EquipStarSuitSampleManager:SampleConfigManager
{
	private static EquipStarSuitSampleManager _Instance;
	private static bool _singleton = true;
	
	public static EquipStarSuitSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new EquipStarSuitSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	public EquipStarSuitSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_EQUIPSTAR_SUIT); 
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		EquipStarSuitSample sample = new EquipStarSuitSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr);
		samples.Add (sid, sample);
	}
	
	//获得装备模板对象
	public EquipStarSuitSample getEquipStarSuitSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as EquipStarSuitSample;
	}     
} 

