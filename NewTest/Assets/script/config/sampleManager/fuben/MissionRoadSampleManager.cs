using System;
using System.Collections.Generic;

public class MissionRoadSampleManager:SampleConfigManager
{
	private static MissionRoadSampleManager _Instance;
	private static bool _singleton = true;
	
	public static MissionRoadSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new MissionRoadSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	public MissionRoadSampleManager ()
	{
		cacheMissionRoadSamples=new Dictionary<int, MissionRoadSample>();
		/*
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_MISSION_ROAD);
		*/
	}
	
	public MissionRoadSample getMissionRoadSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as MissionRoadSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		//41001|Environment_01|014,038,046,061,083,118
		//41005|Environment_01|missionRoad_Grassland|10|s,m,l
		string dataStr = getSampleDataBySid (sid); 
		string[] strArr=dataStr.Split('|');

		int mapId=StringKit.toInt(strArr[0]);
		string environment=strArr[1];
		string road=strArr[2];
		int pointCount=StringKit.toInt(strArr[3]);
		string segmentTypes=strArr[4];

		MissionRoadSample sample = new MissionRoadSample(mapId,road,environment,pointCount,segmentTypes);
		samples.Add (sid, sample);
	}

	public Dictionary<int,MissionRoadSample> cacheMissionRoadSamples;
	
} 

