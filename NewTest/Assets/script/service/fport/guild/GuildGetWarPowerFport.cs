using UnityEngine;
using System.Collections;

public class GuildGetWarPowerFport : BaseFPort {
	private CallBack<int> callBack;
	public void access(CallBack<int> callBack){
		this.callBack = callBack;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILDWAR_GET_WARPOWER);
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		if ((message.getValue ("msg") as ErlType)!=null &&(message.getValue ("msg") as ErlType).getValueString () == "error") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_54"));
			UiManager.Instance.BackToWindow<GuildMainWindow>();
			callBack = null;
			return;
		}

		if ((message.getValue ("msg") as ErlType).getValueString() == "ok") {
			int maxPower = StringKit.toInt((message.getValue("power") as ErlType).getValueString());
			if (callBack != null) {
				callBack (maxPower);
				callBack = null;
			}
		}

	}
}
