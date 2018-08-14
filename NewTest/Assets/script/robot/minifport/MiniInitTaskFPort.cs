using UnityEngine;
using System.Collections;

/**
 * 初始化任务接口
 * @author longlingquan
 * */
public class MiniInitTaskFPort : MiniBaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TASK_GET);
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}
	
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		callback ();
		
	}
}
