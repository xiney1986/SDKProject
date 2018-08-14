using UnityEngine;
using System.Collections;

/**阵型模板管理器
  *负责阵型模板信息的初始化  
  *@author 汤琦
  **/
public class FormationSampleManager : SampleConfigManager
{
	private static FormationSampleManager _Instance;
	private static bool _singleton = true;
	//单例
	public static FormationSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new FormationSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public FormationSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_FORM); 
	}
	//解析阵型数据
	public override void parseSample (int sid)
	{
		FormationSample sample = new FormationSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr);
		samples.Add (sid, sample); 
	}
	//获得阵型模板对象
	public FormationSample getFormationSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
		{
			createSample (sid);
		}
			 
		return samples [sid] as FormationSample;
	}
	
	//获得所有阵型
	public int[] getAllFormation()
	{
		int[] sids = new int[data.Keys.Count];
		int count = 0;
		foreach (int key in data.Keys) {
			sids[count] = key;
			count ++;
		}
		return sids;
	}
}
