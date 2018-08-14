using System;
/// <summary>
/// 升级奖励通信
/// </summary>
public class LevelupRewardFPort:BaseFPort
{
	
	private const string DO_GET="GET";
	private const string DO_RECEIVE="RECEIVE";

	
	
	private CallBack getCallback;
	private CallBack<bool> receiveCallback;

	private string tempCmd;
	/// <summary>
	/// 请求奖励信息 看当前是否有升级奖励可以领取
	/// </summary>
	/// <param name="_callback">_callback.</param>
	public void access_get (CallBack _callback )
	{   
		this.getCallback = _callback;
		tempCmd=DO_GET;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LEVELUP_REWARD_GET);
		access (message);
	}
	/// <summary>
	/// 领取升级奖励
	/// </summary>
	/// <param name="_callback">_callback.</param>
	/// <param name="sid">领取奖励对应的sid</param>
	public void access_receive(CallBack<bool> _callback,int sid)
	{
		this.receiveCallback = _callback;
		tempCmd=DO_RECEIVE;
		toRewardSid=sid;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LEVELUP_REWARD_RECEIVE);	
		message.addValue("award_id",new ErlInt(sid));
		access (message);
	}
	private int toRewardSid;
	
	
	public override void read (ErlKVMessage message)
	{		
		ErlType msg = message.getValue ("msg") as ErlType;
		switch(tempCmd)
		{
			case DO_GET:
				int lastSid=StringKit.toInt(msg.getValueString());
				LevelupRewardManagerment.Instance.lastRewardSid=lastSid;	
				if (getCallback != null)
				{
					getCallback ();
					getCallback=null;
				}
				break;
			case DO_RECEIVE:
				string infoMsg1=msg.getValueString();
				if(infoMsg1 == "ok")
				{
					
					LevelupRewardManagerment.Instance.lastRewardSid=toRewardSid;					
					LevelupRewardManagerment.Instance.receiveEnabled=false;
					TextTipWindow.ShowNotUnlock (LanguageConfigManager.Instance.getLanguage ("s0120"));
				}else
				{			
					MessageWindow.ShowAlert (infoMsg1);
				}
				if (receiveCallback != null)
				{
					receiveCallback (infoMsg1.Equals("ok"));
					receiveCallback=null;
				}
				break;
		}		
		 
	}
}


