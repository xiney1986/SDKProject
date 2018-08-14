using UnityEngine;
using System.Collections;

/**
 * 领取禮包碼奖励通讯端口
 * */
public class InvitewindowFPort : BaseFPort {
	
	private CallBack callback;//立即执行
	private CallBack backClose;//中转传入需要隐藏的窗口
	private CallBack backOpen;//中转传入需要打开的窗口
	
	
	public void access (string uid,CallBack _call,CallBack _call2,CallBack _call3)
	{
		this.callback = _call;
		this.backClose = _call2;
		this.backOpen = _call3;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INVITECODE_GET_GIFT);
		message.addValue ("giftcode", new ErlString (uid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		if(str.getValueString() == "ok")
		{
			if(callback!=null)
				callback();
		}
		
		//邀请码不存在
		if(str.getValueString() == "not_gift_code")
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("invitecode11"));
			return;
		}
		//邀请码上限限制
		if(str.getValueString() == "already_used")
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("invitecode12"));

			return;
		}
		//已经激活过邀请码
		if(str.getValueString() == "overdue")
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("invitecode13"));
			return;
		}

		if (str.getValueString() == "not_get")
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("invitecode14"));
			return;
		}
		
	}
}
