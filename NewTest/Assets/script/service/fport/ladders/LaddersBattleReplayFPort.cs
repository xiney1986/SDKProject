using System;
/// <summary>
/// 天梯战斗记录 重播请求
/// </summary>
public class LaddersBattleReplayFPort : BaseFPort
{

		private CallBack callback;

		public void apply(int index,CallBack _callback)
		{  		
			this.callback = _callback;	
			ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_RECORD);	
			message.addValue("index",new ErlInt(index));
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
				callback();
				callback = null;
			}
		}
}

