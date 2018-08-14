using UnityEngine;
using System.Collections;

public class GuildGetFightRankFPort : BaseFPort {
	private CallBack callBack;
	public void access(CallBack callBack){
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_AREA_GUILD_RANK);
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		ErlType type = message.getValue ("msg") as ErlType;
	}
	
}
