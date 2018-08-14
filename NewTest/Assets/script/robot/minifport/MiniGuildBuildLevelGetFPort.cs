using UnityEngine;
using System.Collections;

/**
 * 获得公会建筑等级通信
 * @author 汤琦
 * */
public class MiniGuildBuildLevelGetFPort : MiniBaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_BUILD_LEVEL);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
//		ErlType type = message.getValue ("msg") as ErlType;
//		if(type.getValueString() == "non_mem")
//		{
//			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
//				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),GuildManagerment.Instance.closeAllGuildWindow);
//			});
//		}
//		else if(type is ErlArray)
//		{
//			ErlArray array = type as ErlArray;
//			for (int i = 0; i < array.Value.Length; i++) {
//				ErlArray temp = array.Value[i] as ErlArray;
//				GuildManagerment.Instance.updateBuild(temp.Value[0].getValueString(),StringKit.toInt(temp.Value[1].getValueString()));
//			}
//		}
	
		if(callback != null)
			callback();
		
		
	}
}
