using UnityEngine;
using System.Collections;

/**
 * 召唤兽进化接口
 * @author longlingquan
 * */
public class BeastEvolveFPort : BaseFPort
{
	private CallBack callback;
	
	public BeastEvolveFPort ()
	{ 
		
	}

	public void access (string beastuid, CallBack callback) 
	{  
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.BEAST_EVOLUTION); 
		message.addValue ("beastuid", new ErlString (beastuid)); 
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string info = (message.getValue ("msg") as ErlType).getValueString ();
		if (info == FPortGlobal.SYSTEM_OK) {
			callback ();
		} else {
			MonoBase.print (GetType () + "error! info=" + info);
		}
	}
}
