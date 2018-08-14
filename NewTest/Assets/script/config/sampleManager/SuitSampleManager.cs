using System;
 
/**
 * 套装模板管理器
 * @author longlingquan
 * */
public class SuitSampleManager:SampleConfigManager
{
	private static SuitSampleManager _Instance;
	private static bool _singleton = true;
	
	public static SuitSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new SuitSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public SuitSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_SUIT); 
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		SuitSample sample = new SuitSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr);
		samples.Add (sid, sample);
	}
	
	//获得装备模板对象
	public SuitSample getSuitSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as SuitSample;
	}
}

public class EquipSid {
    public int ySid;//橙色sid
    public int rSid;//红色sid
} 

