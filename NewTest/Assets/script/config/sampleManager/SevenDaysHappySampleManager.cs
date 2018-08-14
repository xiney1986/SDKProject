using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SevenDaysHappySampleManager : ConfigManager 
{
	private static SevenDaysHappySampleManager _Instance;
	private static bool _singleton = true;

	Dictionary<int,SevenDaysHappySample> sampleDic = new Dictionary<int, SevenDaysHappySample>();

	List<SevenDaysHappyMisson> missonBeforeFilter = new List<SevenDaysHappyMisson>();

	public SevenDaysHappySampleManager()
	{
		if (_singleton)
			return;
		base.readConfig(ConfigGlobal.CONFIG_SEVENDAYSHAPPY);
	}

	public static SevenDaysHappySampleManager Instance
	{
		get
		{
			
			if (_Instance == null)
			{
				_singleton = false;
				_Instance = new SevenDaysHappySampleManager();
				_singleton = true;
				return _Instance;
			}
			else
				return _Instance;
		}
		set
		{
			_Instance = value;
		}
	}

	public override void parseConfig (string str)
	{
		SevenDaysHappySample sample;
		SevenDaysHappyDetail detail;
		string[] strArr = str.Split('|');

		int missoID = StringKit.toInt(strArr[0]);
		int dayID = StringKit.toInt(strArr[1]);

		string[] detailArr = strArr[2].Split(',');
		int detailType = StringKit.toInt(detailArr[0]);
		string detailName = detailArr[1];

		int missonType = StringKit.toInt(strArr[3]);
		string conditions = strArr[4];
		string prizes = strArr[5];
		int order = StringKit.toInt(strArr[6]);

		SevenDaysHappyMisson misson = new SevenDaysHappyMisson(missoID,dayID,detailType,detailName,missonType,conditions,prizes,order);
		missonBeforeFilter.Add(misson);
//		if(!sampleDic.ContainsKey(misson.dayID))
//		{
//			sample = new SevenDaysHappySample(misson.dayID);
//			detail = new SevenDaysHappyDetail(misson);
//			sample.detailsDic.Add(misson.detailType,detail);
//			sampleDic.Add(misson.dayID,sample);
//		}
//		else
//		{
//			sample = sampleDic[misson.dayID];
//			if(!sample.detailsDic.ContainsKey(misson.detailType))
//			{
//				detail = new SevenDaysHappyDetail(misson);
//				sample.detailsDic.Add(misson.detailType,detail);
//			}
//			else
//			{
//				detail = sample.detailsDic[misson.detailType];
//				detail.missonList.Add(misson);
//			}
//		}
	}

	public Dictionary<int,SevenDaysHappySample> getSampleDic()
	{
		return sampleDic;
	}

	public List<SevenDaysHappyMisson> getMissonBeforeFilter()
	{
		return missonBeforeFilter;
	}
}
