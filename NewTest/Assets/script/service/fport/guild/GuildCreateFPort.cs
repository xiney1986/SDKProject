using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 创建公会接口
 * @author 汤琦
 * */
public class GuildCreateFPort : BaseFPort
{
	private CallBackMsg callback;
	
	public void access (string name ,string costType,int autoJoin, CallBackMsg callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.CREATE_GUILD);
		message.addValue ("consume", new ErlString (costType));
		message.addValue ("guildname", new ErlString (name));
		message.addValue ("auto_join",new ErlInt(autoJoin));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "ok") {
			GuildGetInfoFPort fport = FPortManager.Instance.getFPort("GuildGetInfoFPort") as GuildGetInfoFPort;
			fport.access(msgBack);
		}
		else if(type.getValueString () == "name_repeat")
		{
		   UiManager.Instance.openDialogWindow<MessageWindow> ((win)=> {
			win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_5"),null);
			});
		}
		else if(type.getValueString () == "mask_word")
		{
			UiManager.Instance.openDialogWindow<MessageWindow> ((win)=> {
			win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_6"),null);
			});
		}
		else if(type.getValueString () == "mask_word")
		{
			UiManager.Instance.openDialogWindow<MessageWindow> ((win)=> {
			win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_1"),null);
			});
		}
		else if(type.getValueString() == "non_mem")
		{
			UiManager.Instance.openDialogWindow<MessageWindow> ((win)=> {
			win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),null);
			});
		} else if (type.getValueString() == "limit") {
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("Guild_error0"));
		}

	}

	private void msgBack()
	{
		UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
			win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_9"),callback);
		});
	}


}
