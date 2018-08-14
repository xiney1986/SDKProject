
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 升级奖励配置文件管理
/// </summary>
public class LevelupRewardSampleManager :ConfigManager
{
	
	private static LevelupRewardSampleManager _Instance;
	private static bool _singleton = true;
	
	public LevelupRewardSampleManager()
	{
		if (_singleton)
			return;
		rewardSampleTable=new Hashtable();
		rewardLevelTable=new Hashtable();
		base.readConfig(ConfigGlobal.CONFIG_LEVELUP_REWARD);
	}
	 
	public static LevelupRewardSampleManager Instance
	{
		get
		{
			
			if (_Instance == null)
			{
				_singleton = false;
				_Instance = new LevelupRewardSampleManager();
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

	private Hashtable rewardSampleTable;
	private Hashtable rewardLevelTable;


	//解析配置文件 需要子类覆盖
	public override void parseConfig (string str)
	{

		parseReward(str);
		/*
		string[] strArr=str.Split('|');
		for(int i=0,length=strArr.Length;i<length;i++)
		{
			parseReward(strArr[i]);
		}
		*/
	} 
	//129251|描述1|5|1,0,50000#2,0,100
	private int firstSid=0;
	private void parseReward(string str)
	{
		PrizeSample item;
		string[] strArr=str.Split('|');

		int sid=StringKit.toInt(strArr[0]);
		string des=strArr[1];
		int level=StringKit.toInt(strArr[2]);
		string[] prizeArr=strArr[3].Split('#');
		int minLevel=StringKit.toInt(strArr[4]);
		int manLevel=StringKit.toInt(strArr[5]);
		LevelupSample levelupItem=new LevelupSample();
		levelupItem.level=level;
		levelupItem.sid=sid;
		levelupItem.descs=des.Split('#');
		levelupItem.samples=new List<PrizeSample>();
		levelupItem.showMinLevel=minLevel;
		levelupItem.showManLevel=manLevel;

		if(firstSid==0)
		{
			firstSid=sid;
		}
		PrizeSample	prizeItem;
		for(int i=0,length=prizeArr.Length;i<length;i++)
		{
			prizeItem=new PrizeSample(prizeArr[i],',');
			levelupItem.samples.Add(prizeItem);
		}


		rewardLevelTable.Add(level,levelupItem);
		rewardSampleTable.Add(sid,levelupItem);
	}

	/// <summary>
	/// 根据等级返回对应的奖励
	/// </summary>
	/// <returns>The sample by level.</returns>
	/// <param name="level">Level.</param>
	public LevelupSample getSampleByLevel(int level)
	{
		if(rewardLevelTable.ContainsKey(level))
		{
			return rewardLevelTable[level] as LevelupSample;
		}
		return null; 
	}
	/// <summary>
	/// 根据奖励sid返回对应的奖励
	/// </summary>
	/// <returns>The sample by sid.</returns>
	/// <param name="sid">Sid.</param>
	public LevelupSample getSampleBySid(int sid)
	{
		if(sid==0)
		{
			sid=firstSid;
		}
		if(rewardSampleTable.ContainsKey(sid))
		{
			return rewardSampleTable[sid] as LevelupSample;
		}
		return null;
	}
	/*
	private int getKeyByLevel(int level,bool largerThan)
	{
		int tempkey=0;
		foreach(int key in rewardLevelTable.Keys)
		{
			if(largerThan)
			{
				if(key<level)
				{
					continue;
				}
				if(tempkey==0)
				{
					tempkey=key;
					continue;
				}
				if(key-level<tempkey-level)
				{
					tempkey=key;
				}
			}else
			{
				if(key>level)
				{
					continue;
				}
				if(tempkey==0)
				{
					tempkey=key;
					continue;
				}
				if(key-level>tempkey-level)
				{
					tempkey=key;
				}
			}
		}
		return tempkey;
	}
	*/
	
}
/// <summary>
/// 升级奖励的实体封装
/// </summary>
public class LevelupSample
{
	public int level;
	public int sid;
	public string[] descs;
	public List<PrizeSample> samples;
	public int showMinLevel;//显示的最小等级限制
	public int showManLevel;//显示的最大等级限制
}


