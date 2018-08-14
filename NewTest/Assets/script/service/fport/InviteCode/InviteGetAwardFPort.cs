using UnityEngine;
using System.Collections;

public class InviteGetAwardFPort : BaseFPort {

	private CallBack callback;

	public void access (int _awardSid,CallBack call)
	{
		this.callback = call;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INVITECODE_GET_INVITEAWARD);
		message.addValue ("award_sid", new ErlInt (_awardSid));
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
		//奖品不存在
		if(str.getValueString() == "sid_error")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0321"),null);
			});
			return;
		}
		//奖品已领取
		if(str.getValueString() == "alread_award")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0333"),null);
			});
			return;
		}
		//条件不满足
		if(str.getValueString() == "condition_limit")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0334"),null);
			});
			return;
		}
	}
}
