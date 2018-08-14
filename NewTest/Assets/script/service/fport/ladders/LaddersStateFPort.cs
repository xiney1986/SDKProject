using System;

/// <summary>
/// 获取天梯状态
/// </summary>
public class LaddersStateFPort:BaseFPort
{
	private CallBack callback;

	public void apply (CallBack _callback)
	{  	
		this.callback = _callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_STATE);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		LaddersManagement.Instance.nextTime = StringKit.toInt ((message.getValue ("msg") as ErlType).getValueString ());
		if (callback != null) {
			callback ();
			callback = null;
		}
	}
}


