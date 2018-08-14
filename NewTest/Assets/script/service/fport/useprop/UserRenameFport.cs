using UnityEngine;
using System.Collections;

/// <summary>
/// 人物改名
/// </summary>
public class UserRenameFport : BaseFPort {
	private CallBack<string> callBack;
	private string name;
	public void access (int sid,string name, CallBack<string> callBack)
	{
		this.callBack = callBack;
		this.name = name;
		ErlKVMessage message = new ErlKVMessage (FrontPort.USE_PROP); 
		message.addValue ("sid", new ErlInt (sid));//sid
		message.addValue ("num", new ErlInt (1));//数量
		message.addValue ("name", new ErlString (name));//名称
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type.getValueString () == "condition_err") {
		}
		/** 名称非法 */
		else if (type.getValueString () == "name_error") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("roleNameWindow_Validname"));
		} 
		/** 长度超标 */
		else if (type.getValueString () == "name_length_err") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("roleName01"));
		}
		/** 名称重复 */
		else if (type.getValueString () == "name_repeat") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("roleName02"));
		}
		/** 改名成功 */
		else if (type.getValueString ()=="ok") {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("roleName03"));
			if(callBack != null)
				callBack(name);
		}

	}

}
