using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获取小组赛信息
 * @author gc
 * */
public class GodsWarGroupStageFPort : BaseFPort
{
	CallBack callback;
  
	public void access (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_GODSWARGROUPSTAGE_INFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array != null) {
			int pos = 0;
			GodsWarUserInfo user = new GodsWarUserInfo ();
			user.bigTeam = array.Value [pos++].getValueString ();
			user.smallTeam = changeTeamtype(StringKit.toInt(array.Value [pos++].getValueString ()));
			user.rank = StringKit.toInt (array.Value [pos++].getValueString ());
			user.numOfWinning = StringKit.toInt (array.Value [pos++].getValueString ());
			user.usedChallgeNum = StringKit.toInt (array.Value [pos++].getValueString ());
			user.todayIntegral = StringKit.toInt (array.Value [pos++].getValueString ());
			user.totalIntegral = StringKit.toInt (array.Value [pos++].getValueString ());
			GodsWarManagerment.Instance.challengeCount = user.usedChallgeNum;//已经挑战的次数
			GodsWarManagerment.Instance.maxChallengeCount = GodsWarInfoConfigManager.Instance().getSampleBySid(1001).num[0];//最大挑战次数
			ErlType tmp = new ErlType();
			ErlArray enemyArray = array.Value [pos++] as ErlArray;
			if (enemyArray != null) {
				List<GodsWarUserInfo> list = new List<GodsWarUserInfo> ();
                list.Clear();
				for (int i = 0; i < enemyArray.Value.Length; i++) {
					tmp = enemyArray.Value [i];
					ErlArray empArray = tmp as ErlArray;
					GodsWarUserInfo enemy = new GodsWarUserInfo ();
					enemy.massPosition =i;
				    if (empArray.Value.Length == 0)
				    {
                        enemy.uid = "-1";
				        list.Add(enemy);continue;
				    }//如果为0表示该位置没有对手
					int tmpPos = 0;
					enemy.serverName = empArray.Value [tmpPos++].getValueString ();
					enemy.uid = empArray.Value [tmpPos++].getValueString ();
					enemy.name = empArray.Value [tmpPos++].getValueString ();
					enemy.headIcon = StringKit.toInt (empArray.Value [tmpPos++].getValueString ());
					enemy.level = StringKit.toInt (empArray.Value [tmpPos++].getValueString ());
					enemy.winScore = StringKit.toInt(empArray.Value [tmpPos++].getValueString ());
					enemy.challengedWin = StringKit.toInt (empArray.Value [tmpPos++].getValueString ()) == 1;
					list.Add (enemy);
				}
				GodsWarManagerment.Instance.setEnemyList (list);
				GodsWarManagerment.Instance.self = user;
			}
			List<int> scores = new List<int>();
			ErlArray mpt = array.Value[pos++] as ErlArray;
		    if (mpt.Value.Length != 0)
		    {
		        for (int i = 0; i < mpt.Value.Length; i++)
		        {
		            scores.Add(StringKit.toInt(mpt.Value[i].getValueString()));
		        }
		        GodsWarManagerment.Instance.currentScores = scores;
		    }
		    else GodsWarManagerment.Instance.currentScores = null;

			if (callback != null) {
				callback ();
			}
		} else {
            MessageWindow.ShowAlert ((message.getValue ("msg") as ErlType).getValueString ());
			if (callback != null)
				callback = null;
		}
	}
	private string changeTeamtype(int type)
	{
		string team;
		switch(type)
		{
		case 0:
			team = "A";
			break;
		case 1:
			team = "B";
			break;
		case 2:
			team = "C";
			break;
		case 3:
			team = "D";
			break;
		case 4:
			team = "E";
			break;
		case 5:
			team = "F";
			break;
		case 6:
			team = "G";
			break;
		case 7:
			team = "H";
			break;
		default:
			team="";
			break;
		}
		return team;
	}
}
