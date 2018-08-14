using System;
using System.Collections.Generic;

/// <summary>
/// 天梯好友助战次数购买
/// </summary>
public class LaddersFriendHelpFPort:BaseFPort
{
	List<FriendsInvite> invitefriend=new List<FriendsInvite>();
    public LaddersFriendHelpFPort()
	{
	}

	private CallBack<List<FriendsInvite>> callback;
	public void access(CallBack<List<FriendsInvite>> _callback)
	{  		
		this.callback = _callback;
		ErlKVMessage message = new ErlKVMessage(FrontPort.LADDERS_FRIENDSHELP);	
		access (message);

	}

	public override void read (ErlKVMessage message)
	{
		ErlArray data = message.getValue ("msg") as ErlArray;
		if (data == null) {
			return;
		}		
		ErlArray itemData;
		string key = string.Empty;
		for (int i=0,length=data.Value.Length; i<length; i++) {
			itemData = data.Value [i] as ErlArray;
			key = itemData.Value [0].getValueString ();

			switch (key) {
			case "friend_invite":
				parseInvite(itemData.Value [1] as ErlArray);
				break;
			}	
		}
		if (callback != null) {
			callback(invitefriend);
			callback = null;
		}
	}

	private void parseInvite(ErlArray _data)
	{
		invitefriend.Clear();
		ErlArray tempArray;
		for(int i = 0 ;i < _data.Value.Length ; i++){
			tempArray = _data.Value[i] as ErlArray;
			FriendsInvite tmp = new FriendsInvite();
			tmp.uid = tempArray.Value[0].getValueString();
			tmp.inviteNum =StringKit.toInt( tempArray.Value[1].getValueString());
			invitefriend.Add(tmp);
		}
	}
	public class FriendsInvite
	{
		public string uid;
		public int inviteNum;
	}
}


