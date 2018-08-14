using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildMemberWindow : WindowBase {
	public GuildMemberContent memberContent;//成员
	public GameObject buttonGroup2;//成员分页按钮组
	public GameObject guildMemberItem;
	public ButtonBase outGuild; //退出公会
	public ButtonBase inviteGuild;
	public ButtonBase approveGuld; 

	protected override void begin () {
		base.begin ();
        if (isAwakeformHide) {
            updateMemberInfo();
        }
		MaskWindow.UnlockUI ();
	}
	/** 激活 */
	protected override void DoEnable () {
		base.DoEnable ();
		if (fatherWindow is GuildMainWindow) {
			GuildMainWindow win=fatherWindow as GuildMainWindow;
			UiManager.Instance.backGround.switchSynToDynamicBackground (win.launcherPanel, "gangBG", BackGroundCtrl.gangSize);
		}
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		updateMember ();
	}
	public void updateMember () {
		GuildManagerment.Instance.clearUpdateMsg ();
		updateMemberInfo ();
	}
	public void updateMemberInfo () {
		GuildGetMembersFPort fport = FPortManager.Instance.getFPort ("GuildGetMembersFPort") as GuildGetMembersFPort;
		fport.access (getApproveList);
	}
	private void getApproveList () {
		GuildGetApprovalListFPort approvaFport = FPortManager.Instance.getFPort ("GuildGetApprovalListFPort") as GuildGetApprovalListFPort;
		approvaFport.access (initMemberContent);
	}
	public void initMemberContent () {
		memberContent.reLoad ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
		else if (gameObj.name == "buttonInvite") {
			UiManager.Instance.openDialogWindow<GuildInviteMinWindow> ((win) => { });
		}
		else if (gameObj.name == "buttonApprove") {
			openGuildAppWindow ();
		}
		else if (gameObj.name == "buttonOutGuild") {
			if (GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_PRESIDENT && GuildManagerment.Instance.getGuild ().membership > 1) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_62"), null);
				});
			}
			else {
				string str = "";
				if (GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_PRESIDENT && GuildManagerment.Instance.getGuild ().membership == 1) {
					str = LanguageConfigManager.Instance.getLanguage ("Guild_69");
				}
				else {
					str = LanguageConfigManager.Instance.getLanguage ("Guild_60");
				}
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), str, exitBack);
				});
			}
		} 
	}
	private void exitBack (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		GuildExitFPort fport = FPortManager.Instance.getFPort ("GuildExitFPort") as GuildExitFPort;
		fport.access (()=>{
			UiManager.Instance.openMainWindow();
		});
	}
	private void openGuildAppWindow () {
		UiManager.Instance.openWindow<GuildAppWindow> ((win) => {
			win.initWindow ();
		});
	}
}