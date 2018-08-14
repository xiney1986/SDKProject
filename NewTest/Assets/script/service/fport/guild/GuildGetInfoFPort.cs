using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得自己公会信息接口
 * @author 汤琦
 * */
public class GuildGetInfoFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_GUILDINFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray array = type as ErlArray;
			int index = 0;
			string uid = array.Value [index++].getValueString ();
			string name = array.Value [index++].getValueString ();
			int level = StringKit.toInt (array.Value [index++].getValueString ());
			int membership = StringKit.toInt (array.Value [index++].getValueString ());
			int membershipMax = StringKit.toInt (array.Value [index++].getValueString ());
			int livenessing = StringKit.toInt (array.Value [index++].getValueString ());
			int livenessed = StringKit.toInt (array.Value [index++].getValueString ());
			string declaration = array.Value [index++].getValueString ();
			string notice = array.Value [index++].getValueString ();
			string presidentName = array.Value [index++].getValueString ();
			int job = StringKit.toInt (array.Value [index++].getValueString ());
			int contributioning = StringKit.toInt (array.Value [index++].getValueString ());
			int contributioned = StringKit.toInt (array.Value [index++].getValueString ());
			ErlArray msgArray = array.Value [index++] as ErlArray;
			List<GuildMsg> list = new List<GuildMsg> ();
			for (int i = 0; i < msgArray.Value.Length; i++) {
				GuildMsg msg = new GuildMsg (msgArray.Value [i].getValueString ());
				list.Add (msg);
			}
			int todayDonateTimes = StringKit.toInt (array.Value [index++].getValueString ());
			int rename = StringKit.toInt(array.Value[index++].getValueString());
			bool isCanRename = false;
			if(rename == 1)
			{
				isCanRename = true;
			}
			int autoJoin=StringKit.toInt(array.Value[index++].getValueString());
			int joinTime = StringKit.toInt(array.Value[index++].getValueString());
			int haveFirstAward=StringKit.toInt(array.Value[index++].getValueString());
            if (index < array.Value.Length)
                GuildManagerment.Instance.selfScore = StringKit.toInt(array.Value[index++].getValueString());
			GuildManagerment.Instance.createGuild (uid, name, level, membership, membershipMax, livenessing, livenessed, declaration, notice, presidentName, job, contributioning, contributioned, list,todayDonateTimes,isCanRename,autoJoin,joinTime,haveFirstAward);
		} else {
			GuildManagerment.Instance.setGuild(null);
		}
	}
}
