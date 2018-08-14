using UnityEngine;
using System.Collections;

public class DivineGetInfoFPort : BaseFPort
{
    CallBack callback;
	
    public void access (CallBack callback)
	{ 
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.DIVINE_INFO); 
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
        callback();
	}

	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		ErlArray arr = message.getValue("msg") as ErlArray;
		if (arr != null)
		{
			UserManager.Instance.self.divineFortune = StringKit.toInt(arr.Value[0].getValueString());
			UserManager.Instance.self.canDivine = StringKit.toInt(arr.Value[2].getValueString()) == 0;
		}
	}
}
