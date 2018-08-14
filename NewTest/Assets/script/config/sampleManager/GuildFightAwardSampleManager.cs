using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildFightAwardSampleManager : SampleConfigManager {
	//单例
	private static GuildFightAwardSampleManager instance;
	
	public GuildFightAwardSampleManager ()
	{
		
		base.readConfig (ConfigGlobal.CONFIG_GUILD_FIGHT_AWARDS);
	}

	public static GuildFightAwardSampleManager Instance {
		get{
			if(instance==null)
				instance=new GuildFightAwardSampleManager();
			return instance;
		}
	} 
	private List<GuildFightAwardSample> list = new List<GuildFightAwardSample>();

	/** 获取所有的模版 */
	public List<GuildFightAwardSample> getAllSample(){
		if (list.Count != 0)
			return list;
		GuildFightAwardSample [] sampleArray = new GuildFightAwardSample[data.Count];
		int i = 0;
		foreach (DictionaryEntry  entry in data) {
			GuildFightAwardSample sample = new GuildFightAwardSample (); 
			int sid = int.Parse(entry.Key.ToString());
			string dataStr = getSampleDataBySid (sid); 
			sample.parse(sid,dataStr);
			sampleArray[i] = sample;
			++i;
		}
		SetKit.sort (sampleArray, new GuildFightAwardSampleCompare ());
		list.AddRange (sampleArray);
		return list;
	}
}

public class GuildFightAwardSampleCompare : Comparator
{
	public int compare (object a, object b)
	{
		GuildFightAwardSample itemA = a as GuildFightAwardSample;
		GuildFightAwardSample itemB = b as GuildFightAwardSample;
		if (itemA.sid > itemB.sid)
			return 1;
		else if (itemA.sid < itemB.sid)
			return -1;
		else  
			return 0;
	}
}

