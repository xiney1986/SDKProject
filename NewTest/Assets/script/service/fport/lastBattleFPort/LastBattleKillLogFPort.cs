using UnityEngine;
using System.Collections;
using System;

public class LastBattleKillLogFPort : BaseFPort
{
	private CallBack callBack;
	public void killLogAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LASTBATTLEKILLLOG);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		LastBattleKillBossData data;
		ErlArray info = message.getValue ("msg") as ErlArray;
		LastBattleManagement.Instance.killBossCount = StringKit.toInt(info.Value[0].getValueString());
		ErlList infos1 = info.Value[1] as ErlList;
		ErlArray infos;
		if(infos1 != null)
		{
			LastBattleManagement.Instance.killBossDatas.Clear();
			for(int i=0;i<infos1.Value.Length;i++)
			{
				infos = infos1.Value[i] as ErlArray;
				if(infos != null)
				{
					data = new LastBattleKillBossData();
					//data.killBossTime =  TimeKit.timeTransform(StringKit.toInt(infos.Value[0].getValueString()) * 1000);
					data.killBossTime =  transformTime(StringKit.toInt(infos.Value[0].getValueString()));
					data.serverName = infos.Value[1].getValueString();
					data.playerName = infos.Value[2].getValueString();
					if(BossInfoSampleManager.Instance.getBossInfoSampleBySid(StringKit.toInt(infos.Value[3].getValueString())) != null)
					{
						data.bossName = BossInfoSampleManager.Instance.getBossInfoSampleBySid(StringKit.toInt(infos.Value[3].getValueString())).name;
					}
					else
					{
						data.bossName = "";
					}
					data.killBossCount = infos.Value[4].getValueString();
					LastBattleManagement.Instance.killBossDatas.Add(data);
				}
			}
		}
		if(callBack != null)
		{
			callBack();
		}
	}

	public string transformTime(int time)
	{
		string timeStr = "";
		DateTime dt = TimeKit.getDateTime(time);
		timeStr = dt.Hour + ":" + dt.Minute + ":" + dt.Second;
		return timeStr;
	}
}
