using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class IntensifyCostManager : SampleConfigManager
{
	//单例
	private static IntensifyCostManager _Instance;
	private static bool _singleton = true;
	
	public IntensifyCostManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_INTENSIFYCOST);
	}
	
	
	
	public static IntensifyCostManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new IntensifyCostManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	public int[] getCostListBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		
		return samples [sid] as int[];
	} 
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		string dataStr = getSampleDataBySid (sid); 
		string[] strArr = dataStr.Split ('|');
		//	int sid=strArr[0];
		
		string[] strList = strArr[1].Split (',');
		
		int[] sample = new int[strList.Length]; 
		for(int i=0;i< strList.Length;i++){
			
			sample[i]=StringKit.toInt(strList[i]);
			
		}
		
		samples.Add (sid, sample);
	}
	
}
