using UnityEngine;
using System;

/**
 * 每日积分奖励
 * @author gc
 * */
public class GodsWarIntegralAwardFport : BaseFPort
{
    CallBack callback;
	int score;

    public void access (CallBack callback,int score,int hit)
	{   
        this.callback = callback;
		this.score = score;
        ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETINTERAL_AWARD);
		message.addValue("score", new ErlInt(score));
		message.addValue("hit", new ErlInt(hit));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
        if (callback != null)
        {
			if(str.getValueString()=="ok"&&str != null)
			{
				callback();
			}  
			else
			{
				UiManager.Instance.createMessageLintWindow(str.getValueString());
				if(callback!=null)
					callback=null;
			}
        }

			
	}
}
