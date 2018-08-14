using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SdkFriendFPort  :BaseFPort
{
	public const int TYPE_INIT = 1;//获取momo好友信息
	public const int TYPE_SEND = 2;//赠送体力
	public const int TYPE_GET = 3;//领取体力
	public const int TYPE_INVITE = 4;// 邀请好友
	public const int TYPE_PRIZE_INIT = 5;//初始邀请奖励
	public const int TYPE_PRIZE_LEVELINIT = 8;//初始邀请奖励
	public const int TYPE_REBATK_MSG = 6;//获得充值返利信息
	public const int TYPE_PRIZE_GET= 7;//领取等级奖励
	public const int TYPE_ADD_SDKFRIEND = 9;//加sdk好友通信
	private int sendType;
	private CallBack callBack;
	private CallBack<bool> callbackt;
	private CallBack<int> callbacki;
	private CallBack<string> callbacks;
	private CallBack<ErlArray> callbackA;
	
	public void getsdkFriendsInfoMsg(string uids)
	{
		getsdkFriendsMsg(uids,null);
	}
	
	public void getsdkFriendsInfoMsg(string uids,CallBack callback)
	{
		sendType = TYPE_INIT;

		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_GET_INT);
		if (!string.IsNullOrEmpty (uids)) 
		{
		   msg.addValue ("uids", new ErlString (uids));
		}
		access(msg);
		this.callBack = callback;
	}
	
	public void sendsdkFriendsMsg(string uid)
	{
		sendsdkFriendsMsg(uid,null);
	}
	
	public void sendsdkFriendsMsg(string uid,CallBack<string> callback)
	{
		sendType = TYPE_SEND;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_SEND);
		msg.addValue("uid", new ErlString(uid));
		access(msg);
		this.callbacks = callback;
	}
	
	public void sendAddsdkFriendsMsg(string uid,CallBack<string> callback)
	{
		sendType = TYPE_ADD_SDKFRIEND;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_ADD_SDK_FRIEND);
		msg.addValue("uid", new ErlString(uid));
		access(msg);
		this.callbacks = callback;
	}
	
	public void getsdkFriendsMsg(string uid)
	{
		sendsdkFriendsMsg(uid,null);
	}
	
	public void getsdkFriendsMsg(string uid,CallBack<string> callback)
	{
		sendType = TYPE_GET;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_GET);
		msg.addValue("uid", new ErlString(uid));
		access(msg);
		this.callbacks = callback;
	}

	public void sendInviteMsg (string uid,CallBack<string> callback)
	{
		sendType = TYPE_INVITE;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_INVITE);
		msg.addValue("momo_id", new ErlString(uid));
		access(msg);
		this.callbacks = callback;
	}

	// 获取好友返利奖励
	public void getserverPrizesMsg(string prizeuids,CallBack<ErlArray> callback)
	{
		sendType = TYPE_PRIZE_INIT;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_MONEYPRIZE);
		access(msg);
		this.callbackA = callback;
	}
	//  获取好友等级奖励
	public void getserverLevelPrizesMsg(string prizeuids,CallBack<ErlArray> callback)
	{
		sendType = TYPE_PRIZE_LEVELINIT;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_LEVELPRIZE);
		access(msg);
		this.callbackA = callback;
	}
	
	public void sendgetbackPrizesMsg(string prizeuid,CallBack<ErlArray> callback)
	{
		sendType = TYPE_REBATK_MSG;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_PRIZE_BACK);
		//msg.addValue("friendlevel", new ErlString(prizeuid));
		access(msg);
		this.callbackA = callback;
	}
	
	public void sendgetlevelPrizesMsg(string prizeuid,CallBack<string> callback)
	{
		sendType = TYPE_PRIZE_GET;
		ErlKVMessage msg = new ErlKVMessage(FrontPort.SDK_FRIEND_PRIZE_LEVEL);
		msg.addValue("role_lv", new ErlString(prizeuid));
		access(msg);
		this.callbacks = callback;
	}
	
	
	
	public override void read (ErlKVMessage message)
	{
		if (sendType == TYPE_INIT)
		{
			ErlType msg = message.getValue ("msg") as ErlType;
			if (msg is ErlArray) 
			{
				ErlArray array = msg as ErlArray;
				
				if (array == null)
					return;
				
				SdkFriendManager.Instance.createsdkFriendsInfos(array);
				
				if (callBack != null)
					callBack ();
			}
		} else if (sendType == TYPE_SEND) {
			
			string msg = (message.getValue ("msg") as ErlType).getValueString();
			if (msg != null) 
			{

				if (callbacks != null)
				{
					callbacks(msg);
				}
			}
			
		} else if (sendType == TYPE_GET) {
			
			string msg = (message.getValue ("msg") as ErlType).getValueString();
			if (msg != null) 
			{
				
				if (callbacks != null)
				{
					callbacks(msg);
				}
			}
			
		} else if (sendType == TYPE_INVITE) {
			string msg = (message.getValue ("msg") as ErlType).getValueString();
			if (msg != null) 
			{
				if (callbacks != null)
				{
					callbacks(msg);
				}
			}
			
		}  else if (sendType == TYPE_PRIZE_INIT) {
			ErlType msg = message.getValue ("msg") as ErlType;
			if (msg is ErlArray) 
			{
				ErlArray array = msg as ErlArray;
				if (array == null)
					return;
				if (callbackA != null)
				{
					callbackA(array);
				}
			}
			
		} else if (sendType == TYPE_PRIZE_LEVELINIT) {

			ErlType msg = message.getValue ("msg") as ErlType;
			if (msg is ErlArray) 
			{
				ErlArray array = msg as ErlArray;
				if (array == null)
					return;
				if (callbackA != null)
				{
					callbackA(array);
				}
			}
		} else if (sendType == TYPE_REBATK_MSG) {
			
			ErlType msg = message.getValue ("msg") as ErlType;
			if (msg is ErlArray) 
			{
				ErlArray array = msg as ErlArray;
				
				if (callbackA != null)
					callbackA (array);
			}
			
		} else if (sendType == TYPE_PRIZE_GET) {
			
			string msg = (message.getValue ("msg") as ErlType).getValueString();
			if (msg != null) 
			{
				if (callbacks != null)
				{
					callbacks(msg);
				}
			}
			
		} else if (sendType == TYPE_ADD_SDKFRIEND ) {
			
			string msg = (message.getValue ("msg") as ErlType).getValueString();
			if (msg != null) 
			{
				if (callbacks != null)
				{
					callbacks(msg);
				}
			}
			 
		}
	}




}
