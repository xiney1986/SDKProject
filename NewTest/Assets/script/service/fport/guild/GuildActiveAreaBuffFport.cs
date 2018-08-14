using UnityEngine;
using System.Collections;
/// <summary>
/// 激活公会领地buff
/// </summary>
public class GuildActiveAreaBuffFport : BaseFPort {
	private CallBack<int> callBack;
	private int expend;
	/// <summary>
	/// 激活buff 祝福：1  鼓舞:2
	/// </summary>
	public void access(int type,int expend,CallBack<int> callBack){
		this.callBack = callBack;
		this.expend = expend;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_ACTIVE_AREABUFF);
		msg.addValue("type",new ErlInt(type));
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

		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "ok") {
			if (callBack != null){
				callBack(expend);
				callBack = null;
			}
		}

	}
}
