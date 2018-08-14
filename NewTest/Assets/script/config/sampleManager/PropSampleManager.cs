using System;
 
/**
 * 道具模板管理器
 * @author longlingquan
 * */
public class PropSampleManager:SampleConfigManager
{
	//单例
	private static PropSampleManager _Instance;
	private static bool _singleton = true; 

	public PropSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_PROP);
	}

	public static PropSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new PropSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	 
	//解析道具数据
	public override void parseSample (int sid)
	{
		PropSample sample = new PropSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得道具模板对象
	public PropSample getPropSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as PropSample;
	}
}  