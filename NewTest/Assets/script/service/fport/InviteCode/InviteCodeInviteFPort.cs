using UnityEngine;
using System.Collections;

/**
 * 领取激活奖励通讯端口
 * @author 陈世惟
 * */
public class InviteCodeInviteFPort : BaseFPort {

	private CallBack callback;//立即执行
	private CallBack backClose;//中转传入需要隐藏的窗口
	private CallBack backOpen;//中转传入需要打开的窗口
	

	public void access (string uid,CallBack _call,CallBack _call2,CallBack _call3)
	{
		this.callback = _call;
		this.backClose = _call2;
		this.backOpen = _call3;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INVITECODE_INVITE);
		message.addValue ("inviter", new ErlString (uid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		if(str.getValueString() == "ok")
		{
			ErlType type = message.getValue ("award") as ErlType;
			UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{	
				win.Initialize(AllAwardViewManagerment.Instance.addAwards(type),LanguageConfigManager.Instance.getLanguage("s0120"));
			});
			if(callback!=null)
				callback();
		}
		
		//邀请码不存在
		if(str.getValueString() == "inviter_not_exist")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0335"),null);
			});
			return;
		}
		//邀请码上限限制
		if(str.getValueString() == "invitee_limit")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0333"),null);
			});
			return;
		}
		//已经激活过邀请码
		if(str.getValueString() == "already_invite")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0333"),null);
			});
			return;
		}
		
	}
}
