using UnityEngine;
using System.Collections;

/**
 * 取消加入公会申请接口
 * @author 汤琦
 * */
public class GuildCancelJoinFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (string uid,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.CANCEL_JOIN_GUILD);
		message.addValue ("guilduid", new ErlString (uid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if(type.getValueString() == "non_mem")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),null);
			});
			return;
		}
		if(callback != null)
			callback();
	}
}
