using UnityEngine;
using System.Collections;

public class MiniDivineGetInfoFPort : MiniBaseFPort
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
	}
}
