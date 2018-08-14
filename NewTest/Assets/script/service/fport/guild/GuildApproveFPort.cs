using UnityEngine;
using System.Collections;

/**
 * 批准加入公会接口
 * @author 汤琦
 * */
public class GuildApproveFPort : BaseFPort
{
	private CallBack callback;
	private string uid;
	public void access (string uid,CallBack callback)
	{   
		this.uid = uid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.APPROVE_JOIN_GUILD);
		message.addValue ("applyrole", new ErlString (uid));
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

            GuildApprovalInfo guildApprovalInfo = GuildManagerment.Instance.removeGuildApproval(uid);
			GuildManagerment.Instance.getGuild().membership += 1;
            if (guildApprovalInfo != null)
            {
                UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Guild_102", guildApprovalInfo.name));
            }
		}
		else if(type.getValueString() == "not_in_apple")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_70"),null);
			});
		}
		else if(type.getValueString() == "is_member")
		{
			GuildManagerment.Instance.removeGuildApproval(uid);
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_86"),null);
			});
		}

		if(callback != null)
			callback();
		
		
	}
}
