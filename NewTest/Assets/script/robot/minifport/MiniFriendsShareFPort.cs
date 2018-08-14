using UnityEngine;
using System.Collections;

/**
 * 好友分享通讯端口
 * @author 陈世惟
 * */
public class MiniFriendsShareFPort : MiniBaseFPort
{

	public const int TYPE_INIT = 1;//初始化我的分享
	public const int TYPE_SHARE = 2;//一键分享给好友
	public const int TYPE_PRAISE = 3;//一键点赞
	private int sendType;
	private CallBack callback;
	private CallBack<int,int> callbackII;
	private CallBack<int,int,int,int> callbackIIII;

	/// <summary>
	/// 初始化我的分享端口
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void initShareInfo (CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_INIT;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_SHARE);
		access (message);
	}

	/// <summary>
	/// 一键分享端口
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void oneKeyShare (CallBack<int,int> callbackII)
	{
		this.callbackII = callbackII;
		sendType = TYPE_SHARE;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_ONEKEY_SHARE);
		access (message);
	}

	/// <summary>
	/// 一键点赞端口
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void onekeyPraise (CallBack<int,int,int,int> callbackIIII)
	{
		this.callbackIIII = callbackIIII;
		sendType = TYPE_PRAISE;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_ONEKEY_PRAISE);
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
//		base.read (message);
//
//		switch(sendType)
//		{
//		//初始化我的分享
//		case TYPE_INIT:
//			ErlType msg = message.getValue ("msg") as ErlType;
//			
//			if (msg is ErlArray) {
//				ErlArray array = msg as ErlArray;
//				
//				if (array == null)
//					return ;
//				
//				FriendsShareManagerment.Instance.createFriendsShareByErlArray(array);
//				
//				if (callback != null)
//					callback ();
//			}
//			else {
//				MonoBase.print (GetType () + "============error:"+msg);
//			}
//			break;
//
//		//一键分享给好友
//		case TYPE_SHARE:
//			ErlType msgSHARE = message.getValue ("msg") as ErlType;
//			
//			if (msgSHARE is ErlArray) {
//				ErlArray array = msgSHARE as ErlArray;
//				
//				if (array == null)
//					return ;
//
//				FriendsShareManagerment.Instance.setShareInfoUse();
//				
//				int j = 0;
//				int sid = StringKit.toInt (array.Value [j++].getValueString ());
//				int num = StringKit.toInt (array.Value [j++].getValueString ());
//
//				MonoBase.print("-------------------------------------->>>>sid="+sid + ",num="+num);
//				if (callbackII != null)
//					callbackII(sid,num);
//			}
//			else {
//				MonoBase.print (GetType () + "============error:"+msgSHARE);
//			}
//			break;
//
//		//一键点赞
//		case TYPE_PRAISE:
//			ErlType msgPRAISE = message.getValue ("msg") as ErlType;
//			
//			if (msgPRAISE is ErlArray) {
//				ErlArray array = msgPRAISE as ErlArray;
//				
//				if (array == null)
//					return ;
//				
//				int j = 0;
//				int sid = StringKit.toInt (array.Value [j++].getValueString ());
//				int num = StringKit.toInt (array.Value [j++].getValueString ());
//				int praiseNum = StringKit.toInt (array.Value [j++].getValueString ());
//				int money = StringKit.toInt (array.Value [j++].getValueString ());
//
//				FriendsShareManagerment.Instance.setPraiseNum(praiseNum);
//				FriendsShareManagerment.Instance.setPraiseInfoUse(praiseNum);
//				MonoBase.print("-------------------------------------->>>>sid="+sid + ",num="+num + ",use=" + praiseNum);
//				if (callbackIIII != null)
//					callbackIIII(sid,num,praiseNum,money);
//			}
//			else {
//				MonoBase.print (GetType () + "============error:"+msgPRAISE);
//			}
//			break;
//		}
	}
}
