using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 获取公会战队伍信息
/// </summary>
public class GetGuildFightTeamFPort : BaseFPort
{
	CallBack callback;

	public GetGuildFightTeamFPort ()
	{
	}

	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TEAM_GUILDFIGHT);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlArray ls = message.getValue ("msg") as ErlArray;
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
			ArmyManager.Instance.createArmy (armyid, arrayid, beastid, team, alternate, state); 
		}
		if(callback!=null)
			callback();
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
