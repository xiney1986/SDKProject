using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 获得公会伤害排行接口
 * @author 汤琦
 * */
public class GuildGetRastRankFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_GETRASTRANK);
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
			return;
		}
		ErlArray array = type as ErlArray;
		GuildManagerment.Instance.clearGuildRastInfo();
		if(array.Value.Length > 0)
		{
			for (int i = 0; i < array.Value.Length; i++) {
				ErlArray temps = array.Value[i] as ErlArray;
				int sid = StringKit.toInt(temps.Value[0].getValueString());
				string playerName = temps.Value[1].getValueString();
				int rask = StringKit.toInt(temps.Value[2].getValueString());
				GuildRastInfo raskInfo = new GuildRastInfo(sid,playerName,rask);
				GuildManagerment.Instance.createGuildRastInfo(raskInfo);
			}

		}
		
		if(callback != null)
			callback();
		
	}
}
