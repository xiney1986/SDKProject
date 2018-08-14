using UnityEngine;
using System.Collections;

public class GuildRenameFport : BaseFPort
{
	private string name;
	private CallBack<string> callBack;
	public void guildRename (string name, CallBack<string> callBack)
	{
		this.callBack = callBack;
		this.name = name;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.GUILD_RENAME);
		msg.addValue ("name", new ErlString (name));
		access (msg);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		/** 名称非法 */
		if (type.getValueString () == "name_error") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_1119"));
		} 
		/** 长度超标 */
		else if (type.getValueString () == "name_length_err") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_87"));
		}
		/** 名称重复 */
		else if (type.getValueString () == "name_repeat") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_1118"));
		}
		/** 改名成功 */
		else if (type.getValueString () == "ok") {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_1117"));
			if (callBack != null)
				callBack (name);
		}
	}
}
