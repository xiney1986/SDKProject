using UnityEngine;
using System.Collections;

/**
 * 公会卸任副会长接口
 * @author 汤琦
 * */
public class GuildResignViceFPort : BaseFPort
{
	private CallBack callback;
	private string uid;
	
	public void access (string uid,CallBack callback)
	{   
		this.uid = uid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.RESIGN_VICE);
		message.addValue ("vice", new ErlString (uid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if(type.getValueString() == "ok")
		{
			GuildManagerment.Instance.updateJobByUid(uid,GuildJobType.JOB_COMMON);
			if(callback != null)
				callback();
		}
		else if(type.getValueString() == "non_mem")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),null);
			});
			return;
		}
	}
}
