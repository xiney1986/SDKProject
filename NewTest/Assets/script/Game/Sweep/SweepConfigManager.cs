
using System;
using System.Collections;
using System.Collections.Generic;

public class SweepConfigManager :ConfigManager
{
	
	private static SweepConfigManager instance;

	public SweepConfigManager()
	{
		base.readConfig(ConfigGlobal.CONFIG_SWEEP);
	}
	
	public static SweepConfigManager Instance
	{
		get
		{
			
			if (instance == null)
			{
				instance = new SweepConfigManager();
			}
			return instance;
		}
	}
	/// <summary>
	/// 讨伐副本扫荡开放等级要求
	/// </summary>
	public int bossWarSweepMinLevel=0;
	/// <summary>
	/// 剧情扫荡开放等级要求
	/// </summary>
	public int storyWarSweepMinLevel=0;

	/// <summary>
	/// 讨伐副本挂机免cd所需vip最低等级
	/// </summary>
	public int skipBossVipMinLevel=0;

	/// <summary>
	/// 剧情副本挂机免cd所需vip最低等级
	/// </summary>
	public int skipStoryVipMinLevel=0;

	/// <summary>
	/// 讨伐副本cd
	/// </summary>
	public int perBossCdTime=0;

	/// <summary>
	///剧情副本cd
	/// </summary>
	public int perStoryCdTime=0;
	

	public override void parseConfig(string str)
	{
		//30|15|7|8|180|180|挂机：讨伐副本开放等级、剧情副本开放等级、讨伐副本挂机免cd所需vip、剧情副本挂机免cd所需vip、讨伐副本cd、剧情副本cd
		string[] arrStr=str.Split('|');

		bossWarSweepMinLevel=StringKit.toInt(arrStr[0]);
		storyWarSweepMinLevel=StringKit.toInt(arrStr[1]);

		skipBossVipMinLevel=StringKit.toInt(arrStr[2]);
		skipStoryVipMinLevel=StringKit.toInt(arrStr[3]);

		perBossCdTime=StringKit.toInt(arrStr[4]);
		perStoryCdTime=StringKit.toInt(arrStr[5]);
	} 
}

