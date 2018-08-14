using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 刷新对手信息
 * @author gc
 * */
public class GodsWarRefreshEnemyFPort : BaseFPort
{
	CallBack callback;
  
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_GODSWARREFRESHENEMY);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlArray enemyArray = message.getValue ("msg") as ErlArray;
		if (enemyArray != null) {
			ErlType tmp = new ErlType();
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
                    list.Add(enemy);
			        continue; //如果为0表示该位置没有对手

			    }
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

			if (callback != null) {
				callback ();
			}
		} else {
            MessageWindow.ShowAlert ((message.getValue ("msg") as ErlType).getValueString ());
			if (callback != null)
				callback = null;
		}
	}
}
