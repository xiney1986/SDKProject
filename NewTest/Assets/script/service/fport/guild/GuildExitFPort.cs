using UnityEngine;
using System.Collections;

/**
 * 退出公会接口
 * @author 汤琦
 * */
public class GuildExitFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_EXIT);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "non_mem") {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_90"), GuildManagerment.Instance.closeAllGuildWindow);
			});
		} else if (type.getValueString () == "ok") {
			GuildManagerment.Instance.clearAllDate ();
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_61"), null);
			});
			if (callback != null)
				callback ();
		} else if (type.getValueString () == "time_limit") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_37"));
		}

	
		
	}
}
