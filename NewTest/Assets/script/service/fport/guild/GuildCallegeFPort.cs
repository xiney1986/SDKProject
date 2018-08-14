using UnityEngine;
using System.Collections;

/**
 * 公会挑战接口
 * @author 汤琦
 * */
public class GuildCallegeFPort : BaseFPort 
{
	private CallBack callback;
	
	public void access (int arrayid,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_CHALLENGE);
		message.addValue ("arrayid", new ErlInt (arrayid));
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
			GuildManagerment.Instance.isGuildBattle = true;
			if(callback != null)
				callback();
		}
		
		
	}
}
