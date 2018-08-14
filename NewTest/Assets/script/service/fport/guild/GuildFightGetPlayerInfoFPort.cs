using UnityEngine;
using System.Collections;

/// <summary>
/// 公会战中查看玩家信息
/// </summary>
public class GuildFightGetPlayerInfoFPort : BaseFPort
{
	private CallBack<PvpOppInfo> callBack;
	public void access (string serverID, string uid, int index, CallBack<PvpOppInfo> callBack)
	{
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILDWAR_GET_GARRISON_INFO);
		msg.addValue ("target_server", new ErlString (serverID));
		msg.addValue ("target_guild_uid", new ErlString (uid));
		msg.addValue("index",new ErlInt(index));
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);

		if ((message.getValue ("msg") as ErlType)!=null &&(message.getValue ("msg") as ErlType).getValueString () == "error") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_54"));
			UiManager.Instance.BackToWindow<GuildMainWindow>();
			callBack = null;
			return;
		}

		ErlArray array = message.getValue ("msg") as ErlArray;
		PvpOppInfo oppInfo = PvpOppInfo.paresInfo (array);
		if (callBack != null) {
			callBack(oppInfo);
			callBack = null;
		}
	}
	
}
