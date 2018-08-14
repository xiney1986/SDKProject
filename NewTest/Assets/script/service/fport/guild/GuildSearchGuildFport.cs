using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 查找公会
/// </summary>
public class GuildSearchGuildFport : BaseFPort {
	private CallBack<GuildRankInfo> callBack;
	public void searchGuild(string name,CallBack<GuildRankInfo> callBack){
		this.callBack = callBack;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_SEARCH);
		message.addValue ("name", new ErlString (name));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "none_guild") {
			if(callBack!=null)
				callBack(null);
		} else if (type.getValueString () == "none_name") {
			//Name is space!!
		} else {
			ErlArray array = type as ErlArray;
			ErlArray temp = array.Value[0] as ErlArray;
			int index = 0;
			string uid = temp.Value[index++].getValueString();
			int level = StringKit.toInt(temp.Value[index++].getValueString());
			string name = temp.Value[index++].getValueString();
			string declaration = temp.Value[index++].getValueString();
			int membership = StringKit.toInt(temp.Value[index++].getValueString());
			int membershipMax = StringKit.toInt(temp.Value[index++].getValueString());
			int liveness = StringKit.toInt(temp.Value[index++].getValueString());
			GuildRankInfo info = new GuildRankInfo(uid,level,name,membership,membershipMax,declaration,liveness);
			if(callBack != null)
					callBack(info);

		}

	}
}
