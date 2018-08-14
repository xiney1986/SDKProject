using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 好友前台通讯端口控制
 * @author 陈世惟
 * */
public class FriendsFPort : BaseFPort
{

	public const int TYPE_INIT = 1;//初始化
	public const int TYPE_DEL = 2;//删除
	public const int TYPE_APPLY = 3;//申请好友
	public const int TYPE_AGREE = 4;//同意好友申请
	public const int TYPE_REFUSE = 5;//拒绝好友申请
	public const int TYPE_RECOMMEND = 6;//获得推荐好友
	public const int TYPE_FIND = 7;//查找玩家
	private int sendType;
	private 	CallBack callback;
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


	public override void read (ErlKVMessage message)
	{
		//初始化好友信息
		if (sendType == TYPE_INIT) {

			if (parseKVMsgTypeInit (message) && callback != null) {

					callback ();

			}
		}
		
		//删除好友
		else if (sendType == TYPE_DEL) {
			
			string str = (message.getValue ("msg") as ErlAtom).Value;
			
			switch (str) {
			case "ok":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendDEL_ok"));
				if (callback != null)
					callback ();
				break;
			case "not_friend":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendDEL_not_friend"));
				break;
			case "uid_error":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendDEL_uid_error"));
				break;
			default:
				MonoBase.print (GetType () + "error:" + str);
				break;
			}
			
		}
		
		//申请好友
		else if (sendType == TYPE_APPLY) {
			
			string str = (message.getValue ("msg") as ErlAtom).Value;
			
			switch (str) {
			case "ok":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_ok"));
				break;
			case "uid_error":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_uid_error"));
				break;
			case "already_apply":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_already_apply"));
				break;
			case "already_friend":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_already_friend"));
				break;
			case "size_limit_1":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_size_limit"));
				break;
			case "size_limit_2":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_size_limit_other"));
				break;
			default:
				MonoBase.print (GetType () + "============error:" + str);
				break;
			}
			if (callback != null)
				callback ();
		}
		
		//处理好友申请
		else if (sendType == TYPE_AGREE || sendType == TYPE_REFUSE) {
			
			string str = (message.getValue ("msg") as ErlAtom).Value;
			
			
			switch (str) {
			case "ok":
				if (sendType == TYPE_AGREE) {
					FriendsManagerment.Instance.addFriend (newFriendInfo);
					FriendsManagerment.Instance.refuseFriendApply (newFriendInfo.getUid ());
					ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAGREE_ok"));
				} else if (sendType == TYPE_REFUSE) {
					FriendsManagerment.Instance.refuseFriendApply (refuseUid);
				}
				
				if (callback != null)
					callback ();
				break;
			case "uid_error":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_uid_error"));
				if (callback != null)
					callback ();
				break;
			case "size_limit_1":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_size_limit"));
				break;
			case "size_limit_2":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_size_limit_other"));
				FriendsManagerment.Instance.refuseFriendApply (refuseUid);
				if (callback != null)
					callback ();
				break;
			case "already_friend":
				//				ShowMsg(LanguageConfigManager.Instance.getLanguage("FriendAPPLY_already_friend"));
				FriendsManagerment.Instance.refuseFriendApply (refuseUid);
				if (callback != null)
					callback ();
				break;
			case "no_apply":
				//				ShowMsg(LanguageConfigManager.Instance.getLanguage("FriendAPPLY_no_apply"));
				FriendsManagerment.Instance.refuseFriendApply (refuseUid);
				if (callback != null)
					callback ();
				break;
			case "type_error":
				ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_type_error"));
				break;
			default:
				MonoBase.print (GetType () + "============error:" + str);
				break;
			}
		}
		
		
		//获得推荐好友
		else if (sendType == TYPE_RECOMMEND || sendType == TYPE_FIND) {
			
			ErlType msg = message.getValue ("msg") as ErlType;
			
			if (msg is ErlArray) {
				ErlArray array = msg as ErlArray;
				
				if (array == null)
					return;
				
				FriendsManagerment.Instance.createRecommendFriends (array);
				
				if (callback != null)
					callback ();
			} else {
				string str = (message.getValue ("msg") as ErlAtom).Value;
				switch (str) {
				case "target_error":
					ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_type_error"));
					break;
				case "info_error":
					ShowMsg (LanguageConfigManager.Instance.getLanguage ("FriendAPPLY_type_error"));
					break;
				case "no_find":
					ShowMsg (LanguageConfigManager.Instance.getLanguage ("Friend_no_find"));
					break;
				default:
					MonoBase.print (GetType () + "============error:" + str);
					break;
				}
			}
		}
	}

	//解析ErlKVMessgae
	public bool parseKVMsgTypeInit (ErlKVMessage message)
	{
		ErlType msg = message.getValue ("msg") as ErlType;
		if (msg is ErlArray) {
			ErlArray array = msg as ErlArray;
			if (array != null) {
				FriendsManagerment.Instance.createFriendsByErlArray (array);
				return true;
			}
		} else {
			MonoBase.print (GetType () + "============error:" + msg);
		}
		return false;
	}

	private void ShowMsg(string _str){
		GameManager.Instance.StartCoroutine(Utils.DelayRun(()=>{
			UiManager.Instance.createMessageLintWindow (_str);
		},1f));
	}
}
