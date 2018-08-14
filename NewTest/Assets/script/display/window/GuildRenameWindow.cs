using UnityEngine;
using System.Collections;

/// <summary>
/// 公会改名窗口
/// </summary>
public class GuildRenameWindow : WindowBase
{
	/** 输入 */
	public UIInput input;
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 调用重命名接口
	/// </summary>
	private void guildRename (string name)
	{
		GuildRenameFport fport =FPortManager.Instance.getFPort ("GuildRenameFport") as GuildRenameFport;
		fport.guildRename (name, renameCallBack);

	}

	/// <summary>
	/// 回调
	/// </summary>
	private void renameCallBack (string newName)
	{
		MaskWindow.UnlockUI ();
		GuildManagerment.Instance.getGuild ().name = newName;
		GuildManagerment.Instance.getGuild ().isCanRename = false;
		GuildMainWindow mainWindow = UiManager.Instance.getWindow<GuildMainWindow> ();
		this.finishWindow ();
		if ( mainWindow != null) {
			mainWindow.UpdateUI();
		}

	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonSure") {
			if(!checkName())
				return;
			guildRename (input.value);
		} else if (gameObj.name == "buttonCancel") {
			this.finishWindow ();
		}
	}

	/// <summary>
	/// 检查公会名字合法性
	/// </summary>
	private bool checkName ()
	{
		if (string.IsNullOrEmpty (input.value)) {
			UiManager.Instance.createMessageLintWindow( LanguageConfigManager.Instance.getLanguage ("Guild_1120"));
			return false;
		}
		if (input.value.Length > 6) {
			input.value = input.value.Substring (0, 6);
			UiManager.Instance.createMessageLintWindow( LanguageConfigManager.Instance.getLanguage ("Guild_87"));
			return false;
		} else {
			return true;
		}
	}

}
