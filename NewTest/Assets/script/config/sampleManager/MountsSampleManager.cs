using System;

/**
 * 样本管理器
 */
public class MountsSampleManager : SampleConfigManager {

	public MountsSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_MOUNTS); 
	}
	
	//单例
	private static MountsSampleManager _Instance;
	private static bool _singleton = true;
	
	public static MountsSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new MountsSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	//获得角色模板对象
	public MountsSample getMountsSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as MountsSample;
	}

	public override void parseSample (int sid)
	{
		MountsSample sample = new MountsSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}
