using UnityEngine;
using System.Collections;

/**
 * 爵位提升通信
 * @author 汤琦
 * */
public class HonorUpFPort : BaseFPort
{
	private CallBack callback;
	
	public HonorUpFPort ()
	{
	}

	public void access (CallBack callback)
	{ 
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HONOR_UP);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType msg = message.getValue ("msg") as ErlType;

		if(msg.getValueString() == "ok")
		{
			ErlType value = message.getValue ("value") as ErlType;
			UserManager.Instance.self.updateHonor(StringKit.toInt(value.getValueString()));
			callback();
		}
	}

}
