using System;
 
/**
 * 经验模板管理器
 * @author longlingquan
 * */
public class ArenaRobotUserSampleManager:SampleConfigManager
{
	//单例
	private static ArenaRobotUserSampleManager _Instance;
	private static bool _singleton = true;

	public ArenaRobotUserSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
        base.readConfig (ConfigGlobal.CONFIG_ROBOT);
	}

	public static ArenaRobotUserSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new ArenaRobotUserSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public override void parseSample (int sid)
	{
        ArenaRobotUserSample sample = new ArenaRobotUserSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}

    public ArenaRobotUserSample getSampleBySid (int sid)
    {
        if (!isSampleExist (sid))
            createSample (sid); 
        return samples [sid] as ArenaRobotUserSample;
    }  
}  