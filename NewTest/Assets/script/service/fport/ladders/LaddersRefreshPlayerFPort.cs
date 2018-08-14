/// <summary>
/// 天梯玩家刷新
/// </summary>
public class LaddersRefreshPlayerFPort:BaseFPort
{
	public LaddersRefreshPlayerFPort ()
	{
	}
	
	private CallBack<string> callback;
	public void apply(CallBack<string> _callback,bool _free)
	{  		
		this.callback = _callback;
		ErlKVMessage message;
		if(_free)
		{
			message	= new ErlKVMessage (FrontPort.LADDERS_Refresh_Free);	
		}else
		{
			message=new ErlKVMessage(FrontPort.LADDERS_Refresh_Money);
		}
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;

		string msg=str.getValueString();
		if(msg!="ok")
		{
			MessageWindow.ShowAlert (msg);
		}
			
		if (callback != null)
		{
			callback(msg);
			callback = null;
		}
	}
}

