using UnityEngine;
using System.Collections;

public class NoticeAlchemyInfoFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_ALCHEMY_INFO);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		NoticeManagerment.Instance.setAlchemyNum (StringKit.toInt ((message.getValue ("msg") as ErlType).getValueString ()));
		if (callback != null)
			callback ();
	}
}
