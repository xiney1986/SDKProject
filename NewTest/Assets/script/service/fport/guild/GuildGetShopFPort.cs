using UnityEngine;
using System.Collections;

/**
 * 获得公会商店自己物品信息接口
 * @author 汤琦
 * */
public class GuildGetShopFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_GETGOODS);
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
			for (int i = 0; i < array.Value.Length; i++) {
				ErlArray temp = array.Value[i] as ErlArray;
				string sid = temp.Value[0].getValueString();
				ErlArray temps = temp.Value[1] as ErlArray;
				int num = StringKit.toInt(temps.Value[0].getValueString());
				int buyTime = StringKit.toInt(temps.Value[1].getValueString());
				//GuildGood shop = new GuildGood(sid,num,buyTime);
				//GuildManagerment.Instance.createGuildShop(shop);
			}
		}
		if(callback != null)
			callback();
	}
}
