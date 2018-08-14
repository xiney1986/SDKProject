using UnityEngine;
using System.Collections;

/**
 * 获得公会申请列表接口
 * @author 汤琦
 * */
public class MiniGuildGetApplyListFPort : MiniBaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_APPLYLIST);
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//		ErlType type = message.getValue ("msg") as ErlType; 
//		if(type.getValueString() == "non_mem")
//		{
//			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
//				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),GuildManagerment.Instance.closeAllGuildWindow);
//			});
//			return;
//		}
//		ErlArray array = type as ErlArray;
//		GuildManagerment.Instance.clearIds();
//		if(array.Value.Length > 0)
//		{
//
//			for (int i = 0; i < array.Value.Length; i++) {
//				GuildManagerment.Instance.addIds(array.Value[i].getValueString());
//			}
//		}
		parseKVMsg (message);
		if (callback != null)
			callback ();
	}

}
