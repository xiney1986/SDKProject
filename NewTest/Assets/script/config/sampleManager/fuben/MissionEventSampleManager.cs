using System;
 
/**
 * 副本关卡点事件模板管理器
 * @author longlingquan
 * */
public class MissionEventSampleManager:SampleConfigManager
{
	private static MissionEventSampleManager _Instance;
	private static bool _singleton = true;
	
	public static MissionEventSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new MissionEventSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public MissionEventSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_MISSION_EVENT);
	}
	
	public MissionEventSample getMissionEventSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as MissionEventSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		MissionEventSample sample = new MissionEventSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
} 

