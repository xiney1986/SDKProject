using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//时间配置管理器
//活动副本用本地固定的时间配置
//公告活动类用后台传过来的时间配置(因为经常改动)
public class TimeConfigManager : SampleConfigManager
{
	//单例
	private static TimeConfigManager instance;
	private List<TimeInfoSample> localTimeInfoList;

	public TimeConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_TIMEINFO);
	}

	public static TimeConfigManager Instance {
		get {
			if (instance == null)
				instance = new TimeConfigManager ();
			return instance;
		}
	}
	//获得所有本地时间配置
	public TimeInfoSample[] getLocalTimeInfo ()
	{
		return localTimeInfoList.ToArray ();
	}
	
	//解析本地配置
	public override void parseConfig (string str)
	{  
		TimeInfoSample be = new TimeInfoSample (str);
		if (localTimeInfoList == null)
			localTimeInfoList = new List<TimeInfoSample> ();
		localTimeInfoList.Add (be);
	}

	public   TimeInfoSample getTimeInfoSampleBySid (int sid)
	{

		foreach (TimeInfoSample each in TimeConfigManager.instance.localTimeInfoList) {
			if (each.sid == sid)
				return each;
		}
		return null;
	}

	//解析其他配置
	public  List<TimeInfoSample> parseServerConfig (string text)
	{  
		List<TimeInfoSample> List = new List<TimeInfoSample> ();

		string[] strArr = text.Replace ("\r", "").Split ('\n'); 
		for (int i=0; i<strArr.Length; i++) {
			string str = strArr [i];
			if (!(str.IndexOf ("#") == 0) && str != "") {
				TimeInfoSample be = new TimeInfoSample (str);
				localTimeInfoList.Add (be);
			}
		}    

		return List;
	}

	//需要修改 data samples 如果存在就覆盖，不存在添加
	public void updataSample (int sid, string dataStr)
	{
		//if (dataStr == "reset") {
		//	int index = localTimeInfoList.FindIndex (( a ) => { return a.sid == sid; });
		//	if (index != -1)
		//		localTimeInfoList.RemoveAt (index);
		//	parseConfig (data[sid] as string);
		//	return;
		//}

		if (data [sid] == null)
			data.Add (sid, dataStr);
		TimeInfoSample time = getTimeInfoSampleBySid (sid);
		if (time == null) {
			localTimeInfoList.Add (new TimeInfoSample (dataStr));
		} else {
			time.updataData (dataStr);
		}

	}
}
