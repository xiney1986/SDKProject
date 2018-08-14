using UnityEngine;
using System.Collections;

/**
 * 拒绝申请加入公会接口
 * @author 汤琦
 * */
public class GuildInviteRejectFPort : BaseFPort
{

	private CallBack callback;
	private string uid;
	
	public void access (string uid,CallBack callback)
	{   
		this.uid = uid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_INVITE_REJECT);
		message.addValue ("guilduid", new ErlString (uid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if(type.getValueString() == "non_mem")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),GuildManagerment.Instance.closeAllGuildWindow);
			});
		}
		else if(type.getValueString() == "ok")
		{
			GuildManagerment.Instance.removeGuildInviteByUid(uid);
			if(callback != null) 
				callback();
		}
		else if(type.getValueString() == "none")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_84"),null);
			});
		}

	}
}
