using System;

/// <summary>
/// 天梯好友助战次数购买
/// </summary>
public class LaddersFriendHelpBuyFPort:BaseFPort
{
	public LaddersFriendHelpBuyFPort()
	{
	}

	private CallBack callback;
	public void access(CallBack _callback)
	{  		
		this.callback = _callback;
		ErlKVMessage message = new ErlKVMessage(FrontPort.LADDERS_BUY_INVITE);	
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		string msg=string.Empty;
		if(str==null)
		{
			UnityEngine.Debug.LogError ("LaddersFriendHelp Request Fail!");
		}else
		{
			msg=str.getValueString();
			if(msg!="ok")
			{
				UnityEngine.Debug.LogError (msg);
			}
		}

		if (callback != null)
		{
			callback();
			callback = null;
		}
	}
}


