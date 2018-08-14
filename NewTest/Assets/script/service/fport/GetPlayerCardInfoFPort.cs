using UnityEngine;
using System.Collections;

/// <summary>
/// 获得玩家信息通信（卡片或者装备）
/// </summary>
public class GetPlayerCardInfoFPort : BaseFPort
{
	private CallBack<ServerCardMsg> callback;

	public void getCard (string _user_uid, string _card_uid, CallBack<ServerCardMsg> call)
	{
		this.callback = call;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_PLAYERINFO_CARD);
		message.addValue ("card_uid", new ErlString (_card_uid));
		message.addValue ("role_uid", new ErlString (_user_uid));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;

		//目标卡片不存在
		if (type.getValueString () == "error") {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0411"));
			return;
		}

		ErlArray goods = type as ErlArray;
		if (callback != null) {
			callback (CardManagerment.Instance.createCardByChatServer (goods));
			callback = null;
		}else{
			//默认开卡片浏览
			CardBookWindow.Show (CardManagerment.Instance.createCardByChatServer (goods) , CardBookWindow.CLICKCHATSHOW);
		}
	}
}
