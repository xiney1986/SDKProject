using System;

/// <summary>
/// 天梯第一次申请
/// </summary>
public class LaddersApplyFPort:BaseFPort
{
	public LaddersApplyFPort ()
	{
	}

	private CallBack<string> callback;
	public void apply(CallBack<string> _callback)
	{  		
		this.callback = _callback;	
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_APPLY);	
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		string msg=string.Empty;
		if(str==null)
		{
			UnityEngine.Debug.LogError ("Ladders Request Fail!");
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
			callback(msg);
			callback = null;
		}
	}
}


