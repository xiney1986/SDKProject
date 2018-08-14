using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PveBuyFPort : BaseFPort
{

    public PveBuyFPort()
	{
	}

	private CallBack callback;
	public void access(CallBack _callback)
	{  		
		this.callback = _callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.PVE_BUY);	
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
        //string msg=string.Empty;
        //if(str==null)
        //{
        //    UnityEngine.Debug.LogError ("Ladders Request Fail!");
        //}else
        //{
        //    msg=str.getValueString();
        //    if(msg!="ok")
        //    {
        //        UnityEngine.Debug.LogError (msg);
        //    }
        //}

        if (str.getValueString() == "ok") {
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }

	}
}
