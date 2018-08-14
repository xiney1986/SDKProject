using System;
 
/**
 * 关卡模板管理器
 * @author longlingquan
 * */
public class MissionSampleManager:SampleConfigManager
{
	private static MissionSampleManager _Instance;
	private static bool _singleton = true;
	
	public static MissionSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new MissionSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public MissionSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_MISSION);
	}
	
	public MissionSample getMissionSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as MissionSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		MissionSample sample = new MissionSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
} 

