using UnityEngine;
using System.Collections;

/**
 * 公会公告修改接口
 * @author 汤琦
 * */
public class GuildNoticeAmendFPort : BaseFPort
{
	private CallBack callback;
	private string content;
	
	public void access (string content,CallBack callback)
	{   
		if(content == "")
		{
			content = "[]";
		}
		this.content = content;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_AMEND_NOTICE);
		message.addValue ("notice", new ErlString (content));
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
			GuildManagerment.Instance.updateNotice(content);
		if(callback != null)
			callback();
	}
}
