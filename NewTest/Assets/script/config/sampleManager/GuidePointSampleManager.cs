using UnityEngine;
using System.Collections;
using System;

/**
  *指引点位信息模板
  *@author 汤琦
  **/
public class GuidePointSampleManager : SampleConfigManager
{
	//单例
	private static GuidePointSampleManager _Instance;
	private static bool _singleton = true;

	public GuidePointSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_GUIDE_POINT); 
	}

	public static GuidePointSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new GuidePointSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	//获得指引点位模板对象
	public GuidePointSample getGuidePointSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as GuidePointSample;
	}  

	
	//解析模板数据
	public override void parseSample (int sid)
	{
		GuidePointSample sample = new GuidePointSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}
