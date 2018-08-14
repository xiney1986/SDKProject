using UnityEngine;
using System.Collections;

public class OpenGiftBagFport : BaseFPort
{
	CallBack callback;
	private Prop prop;
	private PrizeSample[] prizes;
	
	public OpenGiftBagFport ()
	{
		
	}
	
	public void access (int num, Prop prop, CallBack callback)
	{
		this.callback = callback;
		this.prop = prop;
		ErlKVMessage message = new ErlKVMessage (FrontPort.USE_PROP); 
		message.addValue ("sid", new ErlInt (prop.sid));//sid
		message.addValue ("index", new ErlInt (prop.index));//索引，没用了
		message.addValue ("num", new ErlInt (num));//索引
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "ok") {
			callback ();
		} else {
			MessageWindow.ShowAlert (type.getValueString ());
			if (callback != null)
				callback = null;
		}
	}
}
