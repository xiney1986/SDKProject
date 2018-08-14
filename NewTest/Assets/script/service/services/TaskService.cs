using System;

/**
 * 任务服务
 * @author huangzhenghan
 * */
public class TaskService:BaseFPort
{
	
	public TaskService ()
	{
		
	}

	public override void read (ErlKVMessage message)
	{ 
		ErlArray array = message.getValue ("msg") as ErlArray;
//		for(int i=0;i < array.Value.Length;i++)
//		{
			TaskManagerment.Instance.updateAllTask(array);
//		}
	}
}

