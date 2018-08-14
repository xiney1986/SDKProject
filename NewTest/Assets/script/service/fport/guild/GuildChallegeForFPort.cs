using UnityEngine;
using System.Collections;

/**
 * 获得上次公会挑战阵型接口
 * @author 汤琦
 * */
public class GuildChallegeForFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_CHALLENGE_FORMATION);
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
			return;
		}
		int arrayid = StringKit.toInt(type.getValueString());

		if(callback != null)
			callback();
	}
}
