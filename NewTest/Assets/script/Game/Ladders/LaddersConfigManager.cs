
using System;

/// <summary>
/// 天梯相关配置表
/// </summary>
using System.Collections;
using System.Collections.Generic;

public class LaddersConfigManager
{
	private static LaddersConfigManager instance;

	public static LaddersConfigManager Instance {
		get {
			if (instance == null) {
				instance = SingleManager.Instance.getObj ("LaddersConfigManager") as LaddersConfigManager;
			}
			return instance;
		}
	}

	private LaddersAwardConfig mCconfig_Award;

	public LaddersAwardConfig config_Award {
		get {
			if (mCconfig_Award == null) {
				mCconfig_Award = new LaddersAwardConfig ();
			}
			return mCconfig_Award;
		}
	}

	private LaddersMedalConfig mConfig_Medal;

	public LaddersMedalConfig config_Medal {
		get {
			if (mConfig_Medal == null) {
				mConfig_Medal = new LaddersMedalConfig ();
			}
			return mConfig_Medal;
		}
	}

	private LaddersTitleConfig mConfig_Title;

	public LaddersTitleConfig config_Title {
		get {
			if (mConfig_Title == null) {
				mConfig_Title = new LaddersTitleConfig ();
			}
			return mConfig_Title;
		}
	}

	private LaddersConstConfig mConfig_Const;

	public LaddersConstConfig config_Const {
		get {
			if (mConfig_Const == null) {
				mConfig_Const = new LaddersConstConfig ();
			}
			return mConfig_Const;
		}

	}

	public LaddersConfigManager ()
	{
		/*
		config_Award = new LaddersAwardConfig ();
		config_Medal = new LaddersMedalConfig ();
		config_Title = new LaddersTitleConfig ();
		*/
	}
}
/// <summary>
/// 天梯常用配置 ex:宝箱系数，购买次数单价，刷新价格
/// </summary>
public class LaddersConstConfig:ConfigManager
{
	/** 宝箱系数 */
	public LaddersChestFactorSample chestFactor;
	/** 购买挑战次数价格 */
	public int price_challengeTimes = 0;
	/** 刷新玩家价格 */
	public int price_refreshPlayer = 0;
	/** 天梯开放等级 */
	public int open_minLevel = 0;
	/** 机器人最低等级 */
	public int robotMinLv = 0;
	/** 机器人最高等级 */
	public int robotMaxLv = 0;

	public LaddersConstConfig ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LADDERS_CONST);
	}

	public override void parseConfig (string str)
	{
		string[] strArr=str.Split('|');
		chestFactor = new LaddersChestFactorSample (strArr[0]);
		price_challengeTimes = StringKit.toInt(strArr[1]);
		price_refreshPlayer = StringKit.toInt(strArr[2]);
		open_minLevel = StringKit.toInt(strArr[3]);
		robotMinLv = StringKit.toInt(strArr[4]);
		robotMaxLv = StringKit.toInt(strArr[5]);
	}
}

//天梯奖章配置
public class LaddersMedalConfig:ConfigManager
{
	private List<LaddersMedalSample> samples;
	private int index=0;
	public LaddersMedalConfig ()
	{
		samples = new List<LaddersMedalSample> ();
		base.readConfig (ConfigGlobal.CONFIG_LADDERS_MEDAL);
	}

	public override void parseConfig (string str)
	{
		LaddersMedalSample sample = new LaddersMedalSample (str);
		sample.index=index;
		samples.Add (sample);
		index++;
	}
	/// <summary>
	/// 返回所有的奖章
	/// </summary>
	/// <returns>The medals.</returns>
	public List<LaddersMedalSample> M_getMedals()
	{
		return samples;
	}
	/// <summary>
	/// 根据等级获得对应奖章
	/// </summary>
	/// <returns>The medal.</returns>
	/// <param name="rank">Rank.</param>
	public LaddersMedalSample M_getMedal (int rank)
	{
		if(rank<1)
		{
			return null;
		}
		for (int i=0,length=samples.Count; i<length; i++) {
			if (rank<=samples [i].minRank) {
				return samples [i];
			}
		}
		return null;
	}
	/// <summary>
	/// 根据Sid返回奖章
	/// </summary>
	public LaddersMedalSample M_getMedalBySid(int sid)
	{
		foreach(LaddersMedalSample item in samples)
		{
			if(item.sid==sid)
			{
				return item;
			}
		}
		return null;
	}

	/// <summary>
	/// 根据Sid返回奖章index
	/// </summary>
	public int M_getMedalIndexBySid(int sid)
	{
		for(int i=0;i< samples.Count;i++)
		{
			if(samples[i].sid==sid)
			{
				return i;
			}
		}
		return -1;
	}


}

//天梯称号配置
public class LaddersTitleConfig:SampleConfigManager
{
	private List<LaddersTitleSample> samples;
	private int index=0;
	public LaddersTitleConfig ()
	{
		samples=new List<LaddersTitleSample>();
		base.readConfig (ConfigGlobal.CONFIG_LADDERS_TITLE);
	}

	public override void parseConfig (string str)
	{
		LaddersTitleSample sample=new LaddersTitleSample(str);
		sample.index=index;
		samples.Add (sample);
		index++;
	}
	/// <summary>
	/// 返回所有的称号
	/// </summary>
	/// <returns>The medals.</returns>
	public List<LaddersTitleSample> M_getTitles()
	{
		return samples;
	}

	/// <summary>
	/// 根据声望值获得对应称号的index
	/// </summary>
	public int M_getTitleIndex (int prestige)
	{
		int lenth=samples.Count;
		for (int i=0; i<lenth; i++) {
			if (prestige <= samples [i].minPrestige) {
				return i > 0 ? i - 1 : i;
				//return i;
			}
		}
		return lenth-1;
	}


	/// <summary>
	/// 根据声望值获得对应称号
	/// </summary>
	/// <returns>The medal.</returns>
	/// <param name="rank">Rank.</param>
	public LaddersTitleSample M_getTitle (int prestige)
	{
		int lenth=samples.Count;
		for (int i=0; i<lenth; i++) {
			if (prestige < samples [i].minPrestige) {
				return samples[UnityEngine.Mathf.Clamp(i-1,0,lenth-1)];
			}
		}
		return samples[lenth-1];
	}
	/// <summary>
	/// 根据索引返回称号
	/// </summary>
	/// <returns>The title by index.</returns>
	/// <param name="index">Index.</param>
	public LaddersTitleSample M_getTitleByIndex(int index)
	{
		if (index >= samples.Count - 1) {
			index = samples.Count - 1;
		}
		return samples[index];
	}

	/// <summary>
	/// 对应序数是否是满级
	/// </summary>
	public bool isMaxIndex (int index)
	{
		if (index >= samples.Count - 1) {
			return true;
		} else {
			return false;
		}
	}
	/// <summary>
	/// 根据Sid返回称号
	/// </summary>
	/// <returns>The title by sid.</returns>
	/// <param name="sid">Sid.</param>
	public LaddersTitleSample M_getTitleBySid(int sid)
	{
		foreach(LaddersTitleSample item in samples)
		{
			if(item.sid==sid)
			{
				return item;
			}
		}
		return null;
	}
}

// 天梯奖励配置
public class LaddersAwardConfig:ConfigManager
{
	private List<LaddersAwardSample> samples;
	private int index=0;
	public LaddersAwardConfig ()
	{
		samples=new List<LaddersAwardSample>();
		base.readConfig (ConfigGlobal.CONFIG_LADDERS_AWARD);
	}
	public override void parseConfig (string str)
	{
		LaddersAwardSample sample=new LaddersAwardSample(str);
		sample.index=index;
		samples.Add (sample);
		index++;
	}

	/// <summary>
	/// 返回所有奖励
	/// </summary>
	/// <returns>The awards.</returns>
	public List<LaddersAwardSample> M_getAwards ()
	{
		return samples;
	}
	/// <summary>
	/// 根据等级获得对应奖励
	/// </summary>
	/// <returns>The medal.</returns>
	/// <param name="rank">Rank.</param>
	public LaddersAwardSample M_getAward (int rank)
	{
		for (int i=0,lenth=samples.Count; i<lenth; i++) 
		{
			if (rank<=samples [i].minRank) 
			{
				return samples [i];
			}
		}
		return null;
	}
}

