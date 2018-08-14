using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * 免费刷新对手
 * @author yxl
 * */
public class ArenaRefreshEnemyFPort : BaseFPort
{
    CallBack<string> callback;
  
    public void access (CallBack<string> callback)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_REFRESH_ENEMY);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType msg = message.getValue ("msg") as ErlType; 
        ErlArray array = msg as ErlArray;
        if (array != null)
        {
            int pos = 0;
            ArenaUserInfo user = new ArenaUserInfo();
            user.uid = array.Value [pos++].getValueString();
            user.name = array.Value [pos++].getValueString();
            user.headIcon = StringKit.toInt(array.Value [pos++].getValueString());
            user.integral = StringKit.toInt(array.Value [pos++].getValueString());
            user.team = StringKit.toInt(array.Value [pos++].getValueString());
            //ArenaManager.instance.lastUpdateEnemyTime = StringKit.toInt(array.Value [pos++].getValueString());
            user.rank = StringKit.toInt(array.Value [pos++].getValueString());
            ArenaManager.instance.maxChallengeCount = StringKit.toInt(array.Value [pos++].getValueString());
            
            ErlType tmp = array.Value [pos++];
            if (tmp is ErlArray)
            {
                ErlArray tmpArry = tmp as ErlArray;
                int posTmp = 0;
                ArenaManager.instance.challengeTime = StringKit.toInt(tmpArry.Value [posTmp++].getValueString());
                ArenaManager.instance.challengeCount = StringKit.toInt(tmpArry.Value [posTmp++].getValueString());
            }
            
            tmp = array.Value [pos++];
            if (tmp is ErlArray)
            {
                ErlArray tmpArry = tmp as ErlArray;
                int posTmp = 0;
                ArenaManager.instance.buyChallengeTime = StringKit.toInt(tmpArry.Value [posTmp++].getValueString());
                ArenaManager.instance.buyChallengeCount = StringKit.toInt(tmpArry.Value [posTmp++].getValueString());
            }

			ArenaManager.instance.challengeLastUpdateTime = StringKit.toInt (array.Value [pos++].getValueString ());
//            tmp = array.Value [pos++];
//            if (tmp is ErlArray)
//            {
//                ErlArray tmpArry = tmp as ErlArray;
//                int posTmp = 0;
////                ArenaManager.instance.challengeUseTime = StringKit.toInt(tmpArry.Value [posTmp++].getValueString());
////                ArenaManager.instance.challengeLastUpdateTime = StringKit.toInt(tmpArry.Value [posTmp++].getValueString());
//				//UnityEngine.Debug.LogError("ArenaManager.instance.challengeLastUpdateTime" + ArenaManager.instance.challengeLastUpdateTime.ToString());
//				ArenaManager.instance.challengeLastUpdateTime = StringKit.toInt(tmpArry.Value [posTmp++].getValueString());
//            }
            
            ErlArray enemyArray = array.Value [pos++] as ErlArray;
            if (enemyArray != null)
            {
                List<ArenaUserInfo> list = new List<ArenaUserInfo>();
                for (int j = 0; j < enemyArray.Value.Length; j++)
                {
                    tmp = enemyArray.Value [j];
                    int tmpPos = 0;
                    ErlArray empArray = tmp as ErlArray;
                    ArenaUserInfo enemy = new ArenaUserInfo();
                    enemy.massPosition = j + 1;
                    if (empArray.Value [0].getValueString() == "npc")
                    {
                        enemy.challengedWin = StringKit.toInt(empArray.Value [1].getValueString()) == 1;
                        
                        int count = ArenaRobotUserSampleManager.Instance.data.Count;
                        int id = UnityEngine.Random.Range(1, count + 1);
                        ArenaRobotUserSample sample = ArenaRobotUserSampleManager.Instance.getSampleBySid(id);
                        enemy.headIcon = sample.headIcon;
                        enemy.name = sample.name;
                        enemy.level = sample.level;
                        enemy.npc = true;
                    } else
                    {
                        enemy.uid = empArray.Value [tmpPos++].getValueString();
                        enemy.name = empArray.Value [tmpPos++].getValueString();
                        enemy.headIcon = StringKit.toInt(empArray.Value [tmpPos++].getValueString());
                        enemy.level = StringKit.toInt(empArray.Value [tmpPos++].getValueString());
                        enemy.vipLevel = StringKit.toInt(empArray.Value [tmpPos++].getValueString());
                        enemy.challengedWin = StringKit.toInt(empArray.Value [tmpPos++].getValueString()) == 1;
                    }
                    
                    list.Add(enemy);
                }
                ArenaManager.instance.setEnemyList(list);
                ArenaManager.instance.self = user;
                
                ArenaManager.instance.massBattleType = StringKit.toInt(array.Value [pos++].getValueString());
            }
            
            if (callback != null)
            {
                callback(null);
            }
        } else if (callback != null)
        {
            callback(msg.getValueString());
        }
	}
}
