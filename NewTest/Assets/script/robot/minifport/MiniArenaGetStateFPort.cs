using UnityEngine;
using System;

/**
 * 获取当前竞技场状态
 * @author yxl
 * */
public class MiniArenaGetStateFPort : MiniBaseFPort
{

	public CallBack callback;

	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_STATE);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		parseKVMsg (message);
		callback ();
	}
	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
	}
}
