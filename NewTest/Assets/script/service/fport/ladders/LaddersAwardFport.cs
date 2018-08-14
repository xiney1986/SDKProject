
/// <summary>
/// 天梯奖励领取的请求
/// </summary>
public class LaddersAwardFport:BaseFPort
{
		
		private CallBack<string> callback;
		public void apply(CallBack<string> _callback)
		{  		
			this.callback = _callback;	
			ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_AWARD_RECEIVE);	
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
