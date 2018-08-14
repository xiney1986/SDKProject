using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeekCardSampleManager :ConfigManager 
{
	private static WeekCardSampleManager _Instance;
	private static bool _singleton = true;
	public Dictionary<int,WeekCardSample> weekCards;
	//public PrizeSample[] prizes;
	public Dictionary<int,PrizeSample[]> prizesDic;
	
	public WeekCardSampleManager()
	{
		if (_singleton)
			return;
		base.readConfig(ConfigGlobal.CONFIG_WEEK_CARD);
	}
	
	public static WeekCardSampleManager Instance
	{
		get
		{
			
			if (_Instance == null)
			{
				_singleton = false;
				_Instance = new WeekCardSampleManager();
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
		string[] strArr=str.Split('|');
		string weekCardInfo = strArr[0];
		string prizeInfo = strArr[1];
		string limitLv = strArr[2];
		WeekCardSample sample;
		if(weekCards == null)
		{
			weekCards = new Dictionary<int,WeekCardSample>();
		}
		sample = new WeekCardSample(weekCardInfo,prizeInfo,limitLv);
		if(!weekCards.ContainsKey(sample.id))
		{
			weekCards.Add(sample.id,sample);
			if(prizesDic == null)
			{
				prizesDic = new Dictionary<int,PrizeSample[]>();
			}
			if(!prizesDic.ContainsKey(sample.id))
			{
				prizesDic.Add(sample.id,sample.prizes);
			}
		}
	}

	public WeekCardSample getSampleByID(int id)
	{
		return weekCards[id];
	}

	public PrizeSample[] getPrizes(Dictionary<int,WeekCardSample> weekcards)
	{
		PrizeSample[] prizes = new PrizeSample[weekcards.Count];
//		int i=0;
//		foreach (KeyValuePair<int,WeekCardSample> kv in weekcards)
//		{
//			prizes[i++] = kv.Value.prize;
//		}
		return prizes;
	}

}
