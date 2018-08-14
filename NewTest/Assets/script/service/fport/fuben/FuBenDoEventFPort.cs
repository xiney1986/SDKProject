using System;
 
/**
 * 副本执行事件
 * @author longlingquan
 * */
public class FuBenDoEventFPort:BaseFPort
{
	private const string EXECUTE = "execute_event";//事件执行成功 
	private const string NO_EXECUTE = "no_execute_event";//无事件
	private CallBack<bool> callback;
	
	public FuBenDoEventFPort ()
	{
		
	}
	
	public void doEvent (CallBack<bool> callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_EXECUTE_EVENT); 
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString ();   
		if (str == EXECUTE) {
			if (callback != null)
				callback (true);
		} else if (str == NO_EXECUTE) {
			if (callback != null)
				callback (false);
		}
	}
} 

