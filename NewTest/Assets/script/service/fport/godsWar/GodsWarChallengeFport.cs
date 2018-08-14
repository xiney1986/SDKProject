using UnityEngine;
using System;

/**
 * 小组赛战斗
 * @author gc
 * */
public class GodsWarChallengeFport : BaseFPort
{
    CallBack<bool> callback;

    public void access (CallBack<bool> callback,string serverName,string uid)
	{
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_CHALLENGE);
		message.addValue("server_name", new ErlString(serverName));
		message.addValue("role_uid", new ErlString(uid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
        if (callback != null)
        {
			if(str.getValueString()=="ok"&&str != null)
			{
				FuBenManagerment.Instance.isGodsWarGroup = true;
				callback(true);
			}  
        }
	}
}
