using System;
/// <summary>
/// 开始挑战天梯玩家
/// </summary>
public class LaddersFightFPort:BaseFPort
{
	private CallBack<string> callback;
	public LaddersFightFPort ()
	{
	}
	public void startFigth(CallBack<string> _callback,string _uid,int _playerIndex,int _groupIndex)
	{  	
		/*
		fighter_uid 挑战者id string 
		index 挑战第几个
		group 哪个分组 铜1.。金3
		*/
		this.callback = _callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_FIGHT);
		message.addValue ("fighter_uid", new ErlString(_uid));
		message.addValue ("index", new ErlInt(_playerIndex));
		message.addValue("group",new ErlInt(_groupIndex));	
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		string msg=str.getValueString();
		if(msg!="ok")
		{
			UnityEngine.Debug.LogWarning(msg);
		}
		if (callback != null)
		{
			callback(msg);
			callback = null;
		}
	}
}


