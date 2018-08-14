using UnityEngine;
using System;

/**
 * 用户初始化接口
 * @author zhanghaishan
 * */
public class MiniUserFPort : MiniBaseFPort
{
	
	private CallBack callback;
	
	public void access (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INIT_USER);
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//		ErlArray arr = message.getValue ("msg") as ErlArray;
//		int i = 0;
//		string uid = arr.Value [i++].getValueString ();
//		Debug.LogWarning ("==========uid===========" + uid);
//		string nickname = arr.Value [i++].getValueString ();
//		int style = StringKit.toInt (arr.Value [i++].getValueString ());
//		int money = StringKit.toInt (arr.Value [i++].getValueString ());
//		int rmb = StringKit.toInt (arr.Value [i++].getValueString ());
//		string mainCardUid = arr.Value [i++].getValueString ();
//		long exp = StringKit.toLong (arr.Value [i++].getValueString ());
//		int maxLevel = StringKit.toInt (arr.Value [i++].getValueString ());
//		long vipExp = StringKit.toLong (arr.Value [i++].getValueString ());
//		int executionPve = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionPveMax = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionPveSpeed = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionPvp = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionPvpMax = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionPvpSpeed = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionChv = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionChvMax = StringKit.toInt (arr.Value [i++].getValueString ());
//		int executionChvSpeed = StringKit.toInt (arr.Value [i++].getValueString ());
//		int honorLevel = StringKit.toInt (arr.Value [i++].getValueString ());
//		int honor = StringKit.toInt (arr.Value [i++].getValueString ());
//		string guildId = arr.Value [i++].getValueString ();
//		string guildName = arr.Value [i++].getValueString ();
//		int firendsNum = StringKit.toInt (arr.Value [i++].getValueString ());
//		int titleId = StringKit.toInt (arr.Value [i++].getValueString ());
//		int arenaScore = StringKit.toInt (arr.Value [i++].getValueString ());
//		int activeScore = StringKit.toInt (arr.Value [i++].getValueString ());
//		int winNum = StringKit.toInt (arr.Value [i++].getValueString ());
//		int winNumDay = StringKit.toInt (arr.Value [i++].getValueString ());
//		int winRankDay = StringKit.toInt (arr.Value [i++].getValueString ());
//		long serverTime = long.Parse (arr.Value [i++].getValueString ());
//		int star = StringKit.toInt(arr.Value[i++].getValueString());
//		int merit = StringKit.toInt (arr.Value [i++].getValueString ());
//		
//		MonoBehaviour.print ("====================UserFPort read end length=" + i + "===============  uid=" + uid);
		parseKVMsg (message);
		callback ();
	}
}
