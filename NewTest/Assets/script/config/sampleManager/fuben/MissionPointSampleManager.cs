using System;
 
/**
 * 副本关卡点模板管理器
 * @author longlingquan
 * */
public class MissionPointSampleManager:SampleConfigManager
{
	private static MissionPointSampleManager _Instance;
	private static bool _singleton = true;
	
	public static MissionPointSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new MissionPointSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public MissionPointSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_MISSION_POINT);
	}
	
	public MissionPointSample getMissionPointSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as MissionPointSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		MissionPointSample sample = new MissionPointSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
} 

