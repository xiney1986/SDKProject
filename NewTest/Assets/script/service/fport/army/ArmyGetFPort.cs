using UnityEngine;
using System;

/**
 * 获得队伍接口
 * */
public class ArmyGetFPort : BaseFPort
{
	
	private CallBack callback;

	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.ARMY_GET);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		callback ();
	}

	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlArray ls = message.getValue ("msg") as ErlArray;
		int useId = 0;//使用中的队伍编号 
		for (int i = 0; i<ls.Value.Length; i++) {
			//一个队伍数据
			ErlArray ary = ls.Value [i] as ErlArray; 
			//队伍id
			int armyid = StringKit.toInt (ary.Value [0].getValueString ());
			//阵型id
			int arrayid = StringKit.toInt (ary.Value [1].getValueString ());
			//召唤兽id
			string beastid = ary.Value [2].getValueString ();
			//队伍成员
			ErlArray team_temp = ary.Value [3] as ErlArray;
			string[] team = getIntList (team_temp); 
			//替补成员
			ErlArray alternate_temp = ary.Value [4] as ErlArray;
			string[] alternate = getIntList (alternate_temp);
			//是否正在使用
			int state = StringKit.toInt (ary.Value [5].getValueString ());
			if (state == 1  && armyid !=4  && armyid != 5)  //4,5是守矿队伍，不参加竞技和副本
				useId = armyid;
			ArmyManager.Instance.createArmy (armyid, arrayid, beastid, team, alternate, state); 
		}
		if (useId != 0)
			ArmyManager.Instance.setActive (useId);
		
		// 最后一次使用的阵型
		ArmyManager.Instance.setActive (StringKit.toInt ((message.getValue ("last") as ErlType).getValueString ()));

	}

	private string[] getIntList (ErlArray arr)
	{
		string[] c = new string[arr.Value.Length];
		for (int i = 0; i < c.Length; i++) {
			c [i] = arr.Value [i].getValueString ();
		}
		return c;
	}
} 