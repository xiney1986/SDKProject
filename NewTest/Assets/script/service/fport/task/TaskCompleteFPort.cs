using UnityEngine;
using System.Collections;

/**
 * 完成任务接口
 * @author longlingquan
 * */
public class TaskCompleteFPort : BaseFPort
{
	private CallBack callback;
	private TaskWindow window;
	private DailyRebateContent dailyRebateContent;
	
	public void access (int sid, TaskWindow window, CallBack callback)
	{   
		this.window = window;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TASK_COMPLETE);
		message.addValue ("sid", new ErlInt (sid));//任务sid
		access (message);
	}
	public void access (int sid, DailyRebateContent dailyContent, CallBack callback)
	{   
		this.dailyRebateContent = dailyContent;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TASK_COMPLETE);
		message.addValue ("sid", new ErlInt (sid));//任务sid
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		if (str == "ok") {
			int sid = StringKit.toInt ((message.getValue ("sid") as ErlType).getValueString ());
			TaskManagerment.Instance.completeTask (sid);
			callback ();
		} else if (str == "achieve_timeout") {//跨天，需要重新刷新任务
			if(window != null)
				window.updateContent ();
			else if(dailyRebateContent != null)
				dailyRebateContent.updateAllItems();
				//dailyRebateContent.updateDailyRebateContent();

		} else {
			if(window != null)
				window.updateContent ();
			else if(dailyRebateContent != null)
				dailyRebateContent.updateDailyRebateContent();
			if (callback != null)
				callback = null;
		}
	}
}
