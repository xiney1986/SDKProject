using UnityEngine;
using System.Collections;

/**
 * 好友前台通讯端口控制
 * @author 陈世惟
 * */
public class MiniFriendsFPort : MiniBaseFPort
{

	public const int TYPE_INIT = 1;//初始化
	public const int TYPE_DEL = 2;//删除
	public const int TYPE_APPLY = 3;//申请好友
	public const int TYPE_AGREE = 4;//同意好友申请
	public const int TYPE_REFUSE = 5;//拒绝好友申请
	public const int TYPE_RECOMMEND = 6;//获得推荐好友
	public const int TYPE_FIND = 7;//查找玩家
	private int sendType;
	private CallBack callback;
	private FriendInfo newFriendInfo;
	private string refuseUid;

	/// <summary>
	/// 获取好友信息
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void initFriendsInfo (CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_INIT;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_FRIENDS_INFO);
		access (message);
	}

	/// <summary>
	/// 删除好友
	/// </summary>
	/// <param name="_uid">好友UID.</param>
	/// <param name="callback">Callback.</param>
	public void deleteFriend (string _uid, CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_DEL;
		ErlKVMessage message = new ErlKVMessage (FrontPort.DELETE_FRIEND);
		message.addValue ("uid", new ErlString (_uid));
		access (message);
	}

	/// <summary>
	/// 申请好友
	/// </summary>
	/// <param name="_uid">好友UID.</param>
	/// <param name="callback">Callback.</param>
	public void applyFriend (string _uid, CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_APPLY;
		ErlKVMessage message = new ErlKVMessage (FrontPort.APPLY_FRIEND);
		message.addValue ("uid", new ErlString (_uid));
		access (message);
	}

	/// <summary>
	/// 同意好友申请
	/// </summary>
	/// <param name="_info">好友FriendInfo.</param>
	/// <param name="callback">Callback.</param>
	public void agreeFriend (FriendInfo _info, CallBack callback)
	{
		this.callback = callback;
		newFriendInfo = _info;
		sendType = TYPE_AGREE;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HANDLE_APPLY_FRIEND);
		message.addValue ("uid", new ErlString (_info.getUid ()));
		message.addValue ("type", new ErlInt (1));
		access (message);
	}

	/// <summary>
	/// 拒绝好友申请
	/// </summary>
	/// <param name="_uid">好友UID.</param>
	/// <param name="callback">Callback.</param>
	public void refuseFriend (string _uid, CallBack callback)
	{
		this.callback = callback;
		refuseUid = _uid;
		sendType = TYPE_REFUSE;
		ErlKVMessage message = new ErlKVMessage (FrontPort.HANDLE_APPLY_FRIEND);
		message.addValue ("uid", new ErlString (_uid));
		message.addValue ("type", new ErlInt (2));
		access (message);
	}

	/// <summary>
	/// 获得在线推荐好友
	/// </summary>
	/// <param name="_uid">好友UID.</param>
	/// <param name="callback">Callback.</param>
	public void recommendFriend (CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_RECOMMEND;
		ErlKVMessage message = new ErlKVMessage (FrontPort.RECOMMEND_FRIEND);
		access (message);
	}

	/// <summary>
	/// 查找玩家
	/// </summary>
	/// <param name="_type">0为uid，1为角色名.</param>
	/// <param name="_info">uid，角色名.</param>
	/// <param name="callback">Callback.</param>
	public void findFriend (int _type, string _info, CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_FIND;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FIND_FRIEND);
		message.addValue ("type", new ErlInt (_type));
		message.addValue ("target", new ErlString (_info));
		access (message);
	}

	public void parseKVMsgTypeInit (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
		parseKVMsgTypeInit (message);
		if (callback != null)
			callback ();
	}

	private void ShowMsg (string _str)
	{
//		UiManager.Instance.createMessageWindowByOneButton(_str,null);
		UiManager.Instance.createMessageLintWindow (_str);
	}
}
