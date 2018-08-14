using System;
 
/**
 * 经验模板管理器
 * @author longlingquan
 * */
public class HeroRoadSampleManager:SampleConfigManager
{
	//单例
	private static HeroRoadSampleManager _Instance;
	private static bool _singleton = true;

	public HeroRoadSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_HERO_ROAD);
	}

	public static HeroRoadSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new HeroRoadSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	//解析经验值数据
	public override void parseSample (int sid)
	{
		HeroRoadSample sample = new HeroRoadSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

	public HeroRoadSample getSampleBySid(int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as HeroRoadSample;
	}
}  