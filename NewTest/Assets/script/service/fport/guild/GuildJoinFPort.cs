using UnityEngine;
using System.Collections;

/**
 * 申请加入公会接口
 * @author 汤琦
 * */
public class GuildJoinFPort : BaseFPort
{
	private CallBack<int> callback;
	private GuildApplyWindow win;
	 
	public void access (string uid,CallBack<int> callback,WindowBase win)
	{   
		this.win = win as GuildApplyWindow;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.JOIN_GUILD);
		message.addValue ("guilduid", new ErlString (uid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString() == "non_mem")
        {
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"), null, LanguageConfigManager.Instance.getLanguage("Guild_90"), GuildManagerment.Instance.closeAllGuildWindow);
			});
        } else if (type.getValueString() == "limit_time")
        {
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"), null, LanguageConfigManager.Instance.getLanguage("Guild_68_U"), null);
			});
        } else if (type.getValueString() == "ok")
        {
            if (callback != null)
                callback(0);
		}else if(type.getValueString()=="join"){
			if (callback != null)
				callback(1);
		}
		else if (type.getValueString() == "is_member")
        {
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"), null, LanguageConfigManager.Instance.getLanguage("Guild_71"), intoBack);
			});
        } else
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("Guild_104"));
        }
	}

	private void intoBack(MessageHandle msg)
	{
		GuildGetInfoFPort fport = FPortManager.Instance.getFPort ("GuildGetInfoFPort") as GuildGetInfoFPort;
		fport.access (()=>{
			win.destoryWindow();
			GuildManagerment.Instance.openWindow();
		});
	}

}
