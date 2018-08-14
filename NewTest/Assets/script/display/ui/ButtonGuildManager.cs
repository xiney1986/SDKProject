using UnityEngine;
using System.Collections;

/**
 * 公会管理按钮
 * @author 汤琦
 * */
public class ButtonGuildManager : ButtonBase {
	public GuildMember member;
	public override void DoClickEvent () {
		base.DoClickEvent ();
		if (textLabel.text == LanguageConfigManager.Instance.getLanguage ("Guild_38")) {
			GuildImpeachFPort fport = FPortManager.Instance.getFPort ("GuildImpeachFPort") as GuildImpeachFPort;
			fport.access (appBack);
		}
		else {
			UiManager.Instance.openDialogWindow<GuildManagerWindow> ((win) => {
				win.SetFatherWindow (fatherWindow);
				win.initWindow (member);
			});
		}
	}
	private void appBack () {
		GuildMemberWindow gmw = UiManager.Instance.getWindow<GuildMemberWindow> ();
		if (gmw != null)
			gmw.updateMember ();
	}
}