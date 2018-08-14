using UnityEngine;
using System.Collections;

/**
 * 初始化任务接口
 * @author longlingquan
 * */
public class InitTaskFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TASK_GET);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		parseKVMsg (message);
		if (callback != null)
			callback ();
		
	}
	//解析ErlKVMessgae
	public void parseKVMsg(ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array == null)
			return ;
		TaskManagerment.Instance.updateAllTask(array);
	}
}
