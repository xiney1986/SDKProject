using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * 获得公会祭坛信息接口
 * @author 汤琦
 * */
public class GuildGetAltarFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_ALTAR);
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
		if(array.Value.Length > 0)
		{
			int bossSid = StringKit.toInt(array.Value[0].getValueString());
			long hurtSum = long.Parse(array.Value[1].getValueString());
			int count = StringKit.toInt(array.Value[2].getValueString());
			ErlArray arrays = array.Value[3] as ErlArray;
			List<GuildAltarRank> ranks = new List<GuildAltarRank>();
			for (int i = 0; i < arrays.Value.Length; i++) {
				ErlArray temp = arrays.Value[i] as ErlArray;
				string sid = temp.Value[0].getValueString();
				string playerName = temp.Value[1].getValueString();
				long hurtValue = long.Parse(temp.Value[2].getValueString());
				GuildAltarRank rankTemp = new GuildAltarRank(sid,playerName,hurtValue);
				ranks.Add(rankTemp);
			}
			GuildManagerment.Instance.createGuildAltar(bossSid,hurtSum,count,ranks);

		}
		
		if(callback != null)
			callback();
		
	}
}
