using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 获取领地信息
/// </summary>
public class GuildGetAreaFPort : BaseFPort
{
	private CallBack<GuildArea> callBack;
	public void access (string uid, string serverID, CallBack<GuildArea> callBack)
	{
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_GET_GUILDAREA);
		msg.addValue ("target_guild_uid", new ErlString (uid));
		msg.addValue ("target_server", new ErlString (serverID));
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
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "cd_limit") {
			if(callBack != null){
				callBack(null);
				callBack = null;
			}
		} else {
			if (type is ErlArray) {
				ErlArray array = type as ErlArray;
				GuildArea area = new GuildArea ();
				area.pointList = new List<GuildAreaPoint> ();
				int offset = 0;
				area.wishNum = StringKit.toInt (array.Value [offset++].getValueString ());
				area.inspireNum = StringKit.toInt (array.Value [offset++].getValueString ());
				ErlArray tempArray = array.Value [offset++] as ErlArray;
				for (int i =0; i <tempArray.Value.Length; i++) {
					ErlArray temp = tempArray.Value [i] as ErlArray;
					int index = 0;			
					string name = temp.Value [index++].getValueString ();
					int vipLevel = StringKit.toInt (temp.Value [index++].getValueString ());
					int headIconId = StringKit.toInt (temp.Value [index++].getValueString ());
					bool isNpc = temp.Value [index++].getValueString ().Equals ("1");
					int bloodNow = StringKit.toInt (temp.Value [index++].getValueString ());
					int bloodMax = StringKit.toInt (temp.Value [index++].getValueString ());			
					GuildAreaPoint point = new GuildAreaPoint (name, headIconId, bloodMax, bloodNow, vipLevel, isNpc);
					if (bloodNow == 0)
						area.hasKilled ++;
					area.pointList.Add (point);
				}
				if (callBack != null)
					callBack (area);
			}
		}

	}
}
