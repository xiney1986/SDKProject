using System;
using System.Collections.Generic;

/// <summary>
/// 获取副本噩梦挑战已经重置的次数
/// </summary>
public class FubenBuyChallengeTimesInfoFport:BaseFPort
{
	private CallBack<int> callback;

	public void access (int fsid,CallBack<int> _callback)
	{
		callback = _callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_BOUGHT_CHALLENGE_TIMES);	
		message.addValue ("fbid", new ErlInt (fsid));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		string msg = (message.getValue ("msg") as ErlType).getValueString ();
		if(callback != null) 
		{
			callback (StringKit.toInt(msg));
			callback = null;
		}
	}
}


