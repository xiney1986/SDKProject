using UnityEngine;
using System.Collections;

/// <summary>
/// 领取公会幸运女神投掷奖励通讯端口
/// </summary>
public class GuildGetShakeReward : BaseFPort
{
	private CallBack<int> callBack;
	public void getShakeReward (CallBack<int> callBack)
	{
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_GETREWARDS);
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray array = type as ErlArray;
			string cmd = array.Value [0].getValueString ();
			if (cmd == "ok") {
				int livenessing = StringKit.toInt (array.Value [1].getValueString ());
				int livenessed = StringKit.toInt (array.Value [2].getValueString ());			
				int addValue  = StringKit.toInt (array.Value [3].getValueString ());
				Guild guild = GuildManagerment.Instance.getGuild();
				guild.livenessing = livenessing;
				guild.livenessed = livenessed;
				if (callBack != null)
					callBack (addValue);
			}
		}
	}
}
