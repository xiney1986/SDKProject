using UnityEngine;
using System.Collections;

/// <summary>
/// 自动审批接口 
/// </summary>
public class GuildAutoJoinFPort : BaseFPort {
	private CallBack callback;
	private int isOpen=0;
	public void access (int index)
	{ 
		isOpen=index;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_AUTO_JOIN);
		message.addValue ("auto_join", new ErlInt (index));//开还是关
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		ErlType msg = message.getValue ("msg") as ErlType;
		if(msg != null && msg.getValueString() == "ok")
		{
			GuildManagerment.Instance.getGuild().autoJoin=isOpen;
			if(isOpen==1){
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("guide_autoJoin2"));
				return ;
			}
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("guide_autoJoin3"));
			return;

		}
		else if(msg != null && msg.getValueString() == "limit")
		{
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("guide_autoJoin4"));
			return ;
		}else
		{
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("guide_autoJoin5"));
			return;
		}
	}
}
