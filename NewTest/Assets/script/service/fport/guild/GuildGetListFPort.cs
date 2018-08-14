using UnityEngine;
using System.Collections;

/**
 * 获得公会列表接口
 * @author 汤琦
 * */
public class GuildGetListFPort : BaseFPort
{
	private CallBack<int> callback;
	private CallBack<bool> callback1;
	
	public void access (int start,int type,CallBack<int> callback,CallBack<bool> callback1)
	{   
		this.callback = callback;
		this.callback1 = callback1;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_GUILDLIST);
		message.addValue ("start", new ErlInt (start));
		message.addValue ("type", new ErlInt (type));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if(type is ErlArray)
		{
			GuildManagerment.Instance.clearGuildList();
			ErlArray array = type as ErlArray;
			ErlArray list = array.Value[0] as ErlArray;
			for (int i = 0; i < list.Value.Length; i++) {
				ErlArray temp = list.Value[i] as ErlArray;
				if(temp != null)
				{
					int index = 0;
					string uid = temp.Value[index++].getValueString();
					int level = StringKit.toInt(temp.Value[index++].getValueString());
					string name = temp.Value[index++].getValueString();
					string declaration = temp.Value[index++].getValueString();
					int membership = StringKit.toInt(temp.Value[index++].getValueString());
					int membershipMax = StringKit.toInt(temp.Value[index++].getValueString());
					int liveness = StringKit.toInt(temp.Value[index++].getValueString());
					GuildRankInfo info = new GuildRankInfo(uid,level,name,membership,membershipMax,declaration,liveness);
					GuildManagerment.Instance.createGuildList(info);
				}
			}
			int startIndex = StringKit.toInt(array.Value[1].getValueString());
			bool isMax = array.Value[1].getValueString().ToLower()=="true"?true:false;
			if(callback1 != null)
				callback1(isMax);
			if(callback != null)
				callback(startIndex);

		}
		else if(type.getValueString() == "none_guild")
		{
			if(callback1 != null)
				callback1(true);
			if(callback != null)
				callback(1);
		}
	}

}
