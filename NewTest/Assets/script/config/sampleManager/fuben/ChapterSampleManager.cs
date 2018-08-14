using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 副本章节模板管理器
 * @author longlingquan
 * */
public class ChapterSampleManager:SampleConfigManager
{
	private static ChapterSampleManager _Instance;
	private static bool _singleton = true;
	
	public static ChapterSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new ChapterSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set {  
			_Instance = value;
		}
	}

	public ChapterSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_CHAPTER);
	}
	
	public ChapterSample getChapterSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as ChapterSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		ChapterSample sample = new ChapterSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}


	private List<ChapterSample> csList;
	public List<ChapterSample> getSamples()
	{
		if(csList==null)
		{
			csList=new List<ChapterSample>();
			ChapterSample newItem;
			foreach(int key in data.Keys)
			{
				newItem=getChapterSampleBySid(key);
				if(newItem.type==ChapterType.STORY)
				{
					csList.Add(newItem);
				}
			}
		}
		return csList;
	}
} 

