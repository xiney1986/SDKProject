using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 获取公会战队伍血量信息
/// </summary>
public class GetGuildFightTeamHpInfoFPort : BaseFPort {
	CallBack callback;
	public GetGuildFightTeamHpInfoFPort (){
	}
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILDWAR_GET_HP);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType _ls =message.getValue ("msg") as ErlType;
		if(_ls is ErlArray){

			ErlArray ls = _ls as ErlArray;
			ArmyManager.Instance.hp = new List<ArmyManager.CardHp>();
			ArmyManager.Instance.hp.Clear();
			ArmyManager.CardHp c ;
			for (int i = 0; i<ls.Value.Length; i++) {
				c = new ArmyManager.CardHp();
				//一个队员血量信息
				ErlArray ary = ls.Value [i] as ErlArray; 
				//CardUid
				c.cardUid = ary.Value [0].getValueString ();
				//当前血量
				c.currentHp = StringKit.toInt (ary.Value [1].getValueString ());
				//最大血量
				c.maxHp = StringKit.toInt (ary.Value [2].getValueString ());
				
				ArmyManager.Instance.hp.Add(c);
				ArmyManager.Instance.isdeath = 999;
			}
		}
		else {
			int isdeath = StringKit.toInt(_ls.getValueString());
			ArmyManager.Instance.isdeath = isdeath;
		}
		if(callback!=null)
			callback();
	}
}
