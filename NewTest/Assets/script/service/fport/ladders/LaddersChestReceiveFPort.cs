
using System;

/// <summary>
/// 宝箱领取
/// </summary>
public class LaddersChestReceiveFPort:BaseFPort
{
	public LaddersChestReceiveFPort ()
	{
	}
	
	private CallBack<string> callback;

	public void apply(int _chestIndex,CallBack<string> _callback)
	{  		
		this.callback = _callback;	
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_CHEST_RECEIVE);	
		message.addValue("group",new ErlInt(_chestIndex));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		string msg=str.getValueString();
		if(msg!="ok")//成功领取宝箱后 刷新
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