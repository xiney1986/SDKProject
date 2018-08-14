using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class TaskSampleManager : SampleConfigManager
{
	//单例
	private static TaskSampleManager _Instance;
	private static bool _singleton = true;
	public TaskSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_TASK);
	}
	
	public static TaskSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new TaskSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	//获得任务模板对象
	public TaskSample getTaskSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as TaskSample;
	} 
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		TaskSample sample = new TaskSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	//获得指定限制的所有任务sid
	public int[] getAllTask()
	{
		List<int> list = new List<int>();
		foreach (int key in data.Keys) { 
			list.Add (key); 
		}
		return list.ToArray();
	}
}

