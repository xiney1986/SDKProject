using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**PVP奖励模板管理器
  *负责PVP奖励模板信息的初始化 
  *@author 汤琦
  **/
public class PvpPrizeSampleManager : SampleConfigManager
{
	//单例
	private static PvpPrizeSampleManager instance;
	private List<PvpPrizeSample> list;//连胜奖励
	private List<PvpPrizeSample> listMax;//最高连胜奖
	private const int WINSTREAK = 1;
	private const int WINSTREAKMAX = 2;
	
	public PvpPrizeSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_PVPPRIZE);
	}
	
	public static PvpPrizeSampleManager Instance {
		get{
			if(instance==null)
				instance=new PvpPrizeSampleManager();
			return instance;
		}
	}
	
	//获得连胜奖励信息
	public PvpPrizeSample[] getWinStreak ()
	{
		return list.ToArray();
	}
	//获得最高连胜奖励信息
	public PvpPrizeSample[] getWinStreakMax ()
	{
		return listMax.ToArray();
	}
	
	//解析配置
	public override void parseConfig (string str)
	{ 
		PvpPrizeSample be = new PvpPrizeSample (str);
		if(be.tapCount == WINSTREAK)
		{
			if(list == null)
				list = new List<PvpPrizeSample>();
			list.Add(be);
		}
		else if(be.tapCount == WINSTREAKMAX)
		{
			if(listMax == null)
				listMax = new List<PvpPrizeSample>();
			listMax.Add(be);
		}
	}

}
