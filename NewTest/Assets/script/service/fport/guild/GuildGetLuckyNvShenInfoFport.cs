using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 获取幸运女神主窗口信息接口
/// </summary>
public class GuildGetLuckyNvShenInfoFport : BaseFPort
{
	private CallBack callback;

	public void getLuckyNvShenInfo (CallBack callBack)
	{
		this.callback = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_GETLUCKYNVSHEN);
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray array = type as ErlArray;
			int index = 0;
			//基础信息
			int shakeCount = StringKit.toInt (array.Value [index++].getValueString ());
			int reShakeCount = StringKit.toInt (array.Value [index++].getValueString ());
			int selfIntegral = StringKit.toInt (array.Value [index++].getValueString ());
			int topIntegral = StringKit.toInt (array.Value [index++].getValueString ());
			int guildIntegral = StringKit.toInt (array.Value [index++].getValueString ());
			GuildManagerment.Instance.createGuildLuckyNvShenInfo (selfIntegral, guildIntegral, topIntegral, shakeCount, reShakeCount);

			//上次投掷结果，如果未领取状态
			ErlType resultType = array.Value [index++] as ErlType;
			ErlArray resultArray = resultType as ErlArray;

			if (resultArray.Value.Length == 5) {
				string [] results = new string[5];
				for (int i = 0; i<5; ++i) {
					results [i] = resultArray.Value [i].getValueString ();
				}
				//生成结果缓存
				GuildManagerment.Instance.createGuildShakeResult (results);
			} else {
				//清空结果
				GuildManagerment.Instance.clearGuildShakeResult ();
			}

			//排行榜信息
			RankManagerment.Instance.guildShakeList.Clear ();
			ErlType rankType = array.Value [index++] as ErlType;
			ErlArray rankArray = rankType as ErlArray;
			if (rankArray.Value.Length > 0) {
				for (int i = 0; i < rankArray.Value.Length; i++) {
					ErlArray temps = rankArray.Value [i] as ErlArray;
					if (temps != null && temps.Value.Length == 3) {
						string uid = temps.Value [0].getValueString ();
						string playerName = temps.Value [1].getValueString ();
						int integral = StringKit.toInt (temps.Value [2].getValueString ());
						GuildShakeRankItem temp = new GuildShakeRankItem (uid, playerName, integral);
						RankManagerment.Instance.guildShakeList.Add (temp);
					}
				}	
			}
			if (callback != null)
				callback ();
		}
	}
}
