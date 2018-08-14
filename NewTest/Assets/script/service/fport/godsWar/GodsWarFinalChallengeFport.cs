using UnityEngine;
using System;

/**
 * 淘汰赛战斗
 * @author gc
 * */
public class GodsWarFinalChallengeFport : BaseFPort
{
    CallBack<bool> callback;

    public void access (CallBack<bool> callback,string replayID)
	{   
        this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_CHALLENGE_FINAL);
		message.addValue("id", new ErlString(replayID));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
        if (callback != null)
        {
			if(str.getValueString()=="ok"&&str != null)
			{
				FuBenManagerment.Instance.isGodsWarFinal = true;
				callback(true);
				MaskWindow.UnlockUI();
			}  
        }
	}
}
