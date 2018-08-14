using UnityEngine;
using System.Collections;

public class Task 
{
	public Task (int sid)
	{
		this.sid = sid;
		
	} 
	
	public int sid = 0;//sid
	public int index = 0;
	public long condition = -1;
	
	public void updateProgress(int index)
	{
		if(index > TaskSampleManager.Instance.getTaskSampleBySid(sid).condition.conditions.Length)
		{
			this.index = TaskSampleManager.Instance.getTaskSampleBySid(sid).condition.conditions.Length;
		}
		else
		{
			this.index = index;
		}
	}
	
	public void updateConditionKey(long condition)
	{
		this.condition = condition;
	}
	
	
}
