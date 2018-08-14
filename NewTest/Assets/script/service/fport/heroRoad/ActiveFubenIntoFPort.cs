using UnityEngine;
using System.Collections;

public class ActiveFubenIntoFPort : BaseFPort
{

	public CallBack callback;

	public void intoRoad(int type,CallBack callback)
	{
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.ACTIVE_FUBEN_INTO); 
		message.addValue ("fbid",new ErlInt(type));
		//message.addValue ("arrayid",new ErlInt(arrayid));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
        string str = (message.getValue("msg") as ErlAtom).Value;

        if (str == "ok")
        {
            FuBenManagerment.Instance.isWarActiveFuben = true;
            if (callback != null)
            {
                callback();
            }
        }
        else
        {
            MessageWindow.ShowAlert(str);
        }
	}
}
