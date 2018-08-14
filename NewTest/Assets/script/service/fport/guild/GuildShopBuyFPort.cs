using UnityEngine;
using System.Collections;

/**
 * 公会商店购买接口
 * @author 汤琦
 * */
public class GuildShopBuyFPort : BaseFPort
{
	private CallBack<int,int> callback;
	private const string KEY_CARD = "card";
	private const string KEY_EQUIP = "equip";
	private const string KEY_TOOL = "goods";//后台用的是goods
	private const string KEY_BEAST = "beast";
	private const string KEY_MSG = "msg";
	private int sid = 0;
	private int num = 0;
	
	public void access (int sid, int num,CallBack<int,int> callback)
	{   
		this.sid = sid;
		this.num = num;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GUILD_SHOPBUY);
		message.addValue ("goods_id", new ErlInt (sid));//商品sid
		message.addValue ("num", new ErlInt (num));//商品数量
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType msg = message.getValue ("msg") as ErlType;
		if(msg != null && msg.getValueString() == "non_mem")
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_90"),GuildManagerment.Instance.closeAllGuildWindow);
			});
			return;
		}

		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array != null) {
			StorageManagerment.Instance.parseAddStorageProps(array);
			if (callback != null)
				callback (sid, num);
		} else {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0207"));
		}
		callback = null;
	}
}
