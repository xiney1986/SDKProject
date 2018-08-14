using UnityEngine;
using System;

/**
 * 用户初始化接口
 * @author zhanghaishan
 * */
public class UserFPort : BaseFPort
{
	
	private CallBack callback;

	public void access (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INIT_USER);
		//	Debug.LogWarning("callback"+ callback);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
//		Debug.LogWarning("read UserFPort ");
		parseKVMsg (message);
		callback ();
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlArray arr = message.getValue ("msg") as ErlArray;
		int i = 0;
		string uid = arr.Value [i++].getValueString ();
		string nickname = arr.Value [i++].getValueString ();
		int style = StringKit.toInt (arr.Value [i++].getValueString ());
		int money = StringKit.toInt (arr.Value [i++].getValueString ());
		int rmb = StringKit.toInt (arr.Value [i++].getValueString ());
		string mainCardUid = arr.Value [i++].getValueString ();
		long exp = StringKit.toLong (arr.Value [i++].getValueString ());
		int maxLevel = StringKit.toInt (arr.Value [i++].getValueString ());
		long vipExp = StringKit.toLong (arr.Value [i++].getValueString ());
		int executionPve = StringKit.toInt (arr.Value [i++].getValueString ());
		int executionPveMax = StringKit.toInt (arr.Value [i++].getValueString ());
		long executionPveSpeed = StringKit.toLong (arr.Value [i++].getValueString ());
		int executionPvp = StringKit.toInt (arr.Value [i++].getValueString ());
		int executionPvpMax = StringKit.toInt (arr.Value [i++].getValueString ());
		long executionPvpSpeed = StringKit.toLong (arr.Value [i++].getValueString ());
		int executionChv = StringKit.toInt (arr.Value [i++].getValueString ());
		int executionChvMax = StringKit.toInt (arr.Value [i++].getValueString ());
		long executionChvSpeed = StringKit.toLong (arr.Value [i++].getValueString ());
		int storePve = StringKit.toInt (arr.Value [i++].getValueString ());
		int storePveMax = StringKit.toInt (arr.Value [i++].getValueString ());
		long storePveSpeed = StringKit.toLong (arr.Value [i++].getValueString ());
		int honorLevel = StringKit.toInt (arr.Value [i++].getValueString ());
		int honor = StringKit.toInt (arr.Value [i++].getValueString ());
		string guildId = arr.Value [i++].getValueString ();
		string guildName = arr.Value [i++].getValueString ();
		guildName = (guildName.Equals ("none") || guildName.Equals ("[]")) ? LanguageConfigManager.Instance.getLanguage ("s0484") : guildName;
		int firendsNum = StringKit.toInt (arr.Value [i++].getValueString ());
		int titleId = StringKit.toInt (arr.Value [i++].getValueString ());
		int arenaScore = StringKit.toInt (arr.Value [i++].getValueString ());
		int activeScore = StringKit.toInt (arr.Value [i++].getValueString ());
		int winNum = StringKit.toInt (arr.Value [i++].getValueString ());
		int winNumDay = StringKit.toInt (arr.Value [i++].getValueString ());
		int winRankDay = StringKit.toInt (arr.Value [i++].getValueString ());
		long serverTime = long.Parse (arr.Value [i++].getValueString ());
		int star = StringKit.toInt (arr.Value [i++].getValueString ());
		int merit = StringKit.toInt (arr.Value [i++].getValueString ());
		int hightPoint = StringKit.toInt (arr.Value [i++].getValueString ());
		int[] vipAwardSids = parseVipAwardSids (arr.Value [i++] as ErlArray);
		int lastLevelupRewardSid = StringKit.toInt (arr.Value [i++].getValueString ());
		int battlePlayVelocity = PlayerPrefs.GetInt (uid + PlayerPrefsComm.BATTLE_PLAY_VELOCITY);
		int prestige = StringKit.toInt (arr.Value [i++].getValueString ());
		int ladderRank = StringKit.toInt (arr.Value [i++].getValueString ());
		int onlineTime = StringKit.toInt (arr.Value [i++].getValueString ());
		bool canFrist = (arr.Value [i++].getValueString()).Equals("true");
		long mountsExp = StringKit.toLong (arr.Value [i++].getValueString ());
		MountsManagerment.Instance.updateMountsExp(mountsExp);
		int guildFightPower = StringKit.toInt (arr.Value [i++].getValueString ());
		int guildFightPowerMax = StringKit.toInt (arr.Value [i++].getValueString ());
		UserManager.Instance.createSelf (uid, nickname, style, money, rmb, mainCardUid, exp, maxLevel, vipExp, vipAwardSids, lastLevelupRewardSid, executionPve, executionPveMax,
		                                 executionPveSpeed, executionPvp, executionPvpMax, executionPvpSpeed, executionChv, executionChvMax, executionChvSpeed,
		                                 storePve,storePveMax,storePveSpeed,
		                                 winNum, winNumDay, winRankDay, honorLevel, honor, guildId, guildName,
		                                 firendsNum, titleId, arenaScore, activeScore, serverTime, star, merit, hightPoint, battlePlayVelocity, prestige, ladderRank, onlineTime,canFrist,
		                                 guildFightPower,guildFightPowerMax);

	}

	private int[] parseVipAwardSids (ErlArray sids)
	{
		int[] awardSids = new int[sids.Value.Length];
		for (int i = 0; i<sids.Value.Length; i++) {
			awardSids [i] = StringKit.toInt ((sids.Value [i] as ErlType).getValueString ());
		}
		return awardSids;
	}
}
