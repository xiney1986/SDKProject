using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**公会BOSS奖励模板管理器
  *负责公会BOSS奖励模板信息的初始化 
  *@author 汤琦
  **/
public class GuildPrizeSampleManager : SampleConfigManager
{
	private static GuildPrizeSampleManager instance;
	private List<GuildBossPrizeSample> prizes;
	
	public static GuildPrizeSampleManager Instance {
		get{
			if(instance==null)
				instance=new GuildPrizeSampleManager();
			return instance;
		}
	}
	
	public GuildPrizeSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GUILDBOSSPRIZE);
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		GuildBossPrizeSample be = new GuildBossPrizeSample (str);
		if(prizes == null)
			prizes = new List<GuildBossPrizeSample>();
		prizes.Add(be);
	}

	public int getPrizesSum()
	{
		if(prizes == null)
			return 0;
		else
			return prizes.Count;
	}
	
	/** 获取所有公会奖励信息 */
	public List<GuildBossPrizeSample> getPrizes()
	{
		return prizes;
	}

}
