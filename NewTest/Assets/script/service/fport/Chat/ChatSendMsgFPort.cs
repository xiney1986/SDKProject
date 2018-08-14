using UnityEngine;
using System.Collections;

/**
 * 初始化聊天发送接口
 * @authro 陈世惟 
 * */
public class ChatSendMsgFPort : BaseFPort {

	private CallBack callback;
	
	/// <summary>
	/// 传入消息数据到接口发送
	/// </summary>
	/// <param name='channel'>
	/// Channel.1=世界，2=公会
	/// </param>
	/// <param name='msg'>
	/// Message.聊天内容，展示为NULL
	/// </param>
	/// <param name='type'>
	/// Type.展示类型，card=卡片，equipment=装备
	/// </param>
	/// <param name='uid'>
	/// Uid.装备或卡片UID
	/// </param>
	/// <param name='call'>
	/// Call.回调
	/// </param>
	public void access (int channel,string msg,string type,string uid,CallBack call)
	{
		this.callback = call;
		ErlKVMessage message = new ErlKVMessage (FrontPort.CHAT_SEND);
		message.addValue ("channel", new ErlInt (channel));
		if(msg != null)
		{
			message.addValue ("str", new ErlString (msg));
		}
		else
		{
			message.addValue ("type", new ErlString (type));
			message.addValue ("uid", new ErlString (uid));
		}
		access (message);
	}

    /// <summary>
    /// 发送聊天给好友
    /// </summary>
    public void access(string friendUid, string roleuid, int channel, string msg, string type, string uid, CallBack call)
    {
        this.callback = call;
        ErlKVMessage message = new ErlKVMessage(FrontPort.CHAT_SEND);
        message.addValue("channel", new ErlInt(channel));
        message.addValue("frienduid", new ErlString(friendUid));
        message.addValue("roleuid", new ErlString(roleuid));
        if (msg != null)
        {
            message.addValue("str", new ErlString(msg));
        }
        else
        {
            message.addValue("type", new ErlString(type));
            message.addValue("uid", new ErlString(uid));
        }
        access(message);
    }

	
	public override void read (ErlKVMessage message)
	{
        
		string str = (message.getValue ("msg") as ErlAtom).Value;
		if(str == "ok")
		{
			if(callback!=null)
				callback();
		}
		else if(str == "cd_limit")
		{
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0307"));
            
			if(callback!=null)
				callback();
		}
		else if(str == "no_mem" || str == "no_guild")
		{
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Guild_82"));
		}
        else if (str == "offline")
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0490"));
        }
        else if (str == "not_friend")
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0492"));
        }
        else
        {
            MonoBase.print(GetType() + "error:" + str);
        }
		
	}
}
