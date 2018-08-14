using UnityEngine;
using System.Collections;

/**
 * 清理人出公会接口
 * @author 汤琦
 * */
public class GuildClearMemFPort : BaseFPort
{
	private CallBack callback;
	private string uid;
	//所要清理的人的uid
	public void access (string uid,CallBack callback)
	{   
		this.uid = uid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_CLEANMEM);
		message.addValue ("member", new ErlString (uid));
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
			GuildManagerment.Instance.removeMember(uid);
			GuildManagerment.Instance.getGuild().membership -= 1;
			if(callback != null)
				callback();
		}
		else if(type.getValueString() == "position_limit") {
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("guildmanager3"));

		}
	}
}
