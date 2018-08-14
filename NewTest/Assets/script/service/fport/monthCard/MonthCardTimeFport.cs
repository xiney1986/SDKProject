
/// <summary>
/// 月卡购买时间通信
/// </summary>
public class MonthCardTimeFport:BaseFPort
{
	
	private CallBack<int> callback;
	public void apply(CallBack<int> _callback)
	{  		
		this.callback = _callback;	
		ErlKVMessage message = new ErlKVMessage (FrontPort.MONTHCARD_TIMEINFO);	
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
//		int timeResult = StringKit.toInt (message.getValue ().ToString ());
		int timeResult = StringKit.toInt (str.getValueString());
		if (callback != null)
		{
			callback(timeResult);
			callback = null;
		}
	}
}
