using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankGetGuildInfoFPort : BaseFPort
{
	private CallBack<Guild,List<GuildMember>> callback;
	
	public void access (string guideId,CallBack<Guild,List<GuildMember>> callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_GUILDSAMPLEINFO);
		message.addValue ("uid", new ErlString (guideId));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
        if (array == null)
        {
            if(callback != null)
                callback(null,null);
            return;
        }
		ErlType[] vs = array.Value;

		Guild guild = new Guild ();
		guild.uid = vs [0].getValueString ();
		guild.level = StringKit.toInt (vs[1].getValueString());
		guild.name = vs [2].getValueString ();
		guild.membership = StringKit.toInt (vs[3].getValueString());
		guild.membershipMax = StringKit.toInt (vs[4].getValueString());
		guild.livenessing = StringKit.toInt (vs[5].getValueString());
		guild.livenessed = StringKit.toInt (vs[6].getValueString());

		array = vs [7] as ErlArray;
		List<GuildMember> list = new List<GuildMember> ();
		for (int i = 0; i < array.Value.Length; i++) {
			ErlArray a2 = array.Value[i] as ErlArray;
			vs = a2.Value;
			GuildMember m = new GuildMember();
			m.uid = vs [0].getValueString ();
			m.icon = StringKit.toInt (vs[1].getValueString());
			m.name = vs [2].getValueString ();
			m.level = StringKit.toInt (vs[3].getValueString());
			m.job = StringKit.toInt (vs[4].getValueString());
			list.Add(m);

			if(m.job == GuildJobType.JOB_PRESIDENT)
				guild.presidentName = m.name;
		}

		if (callback != null) {
			callback(guild,list);
		}
	}

}
