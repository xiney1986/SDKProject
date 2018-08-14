using UnityEngine;
using System.Collections;

/**
 * 公会管理窗口
 * @author 汤琦
 * */
public class GuildManagerWindow : WindowBase {

	private string uid;
	private string memberName;
	public ButtonBase button1;//转让会长
	public ButtonBase button2;//任命副会长
	public ButtonBase button3;//踢出公会
	public UIGrid grid;

	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public void initWindow (GuildMember member) {
		this.uid = member.uid;
		this.memberName = member.name;
		changeButton ();
	}
	private void changeButton () {
		//副会长、官员、普通会员 都无法转让会长，任命副会长
		if (GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_VICE_PRESIDENT || GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_COMMON || GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_OFFICER) {
			button1.gameObject.SetActive (false);
			button2.gameObject.SetActive (false);
			//副会长还能踢人
			if (GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_VICE_PRESIDENT) {
				button3.gameObject.SetActive (true);
			}
		}
		else {
			//可以转让会长,可以踢人
			button1.gameObject.SetActive (true);
			button3.gameObject.SetActive (true);
			//如果已经有副会长,就卸任，没有就任命
			if (GuildManagerment.Instance.isHaveVicePresident ()) {
				//先卸任,否则不显示按钮
				if (GuildManagerment.Instance.isVicePresident (uid)) {
					button2.textLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_74");
					button2.gameObject.SetActive (true);
				}
				else {
					button2.gameObject.SetActive (false);
				}
			}
			else {
				button2.textLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_73");
				button2.gameObject.SetActive (true);
			}
		}
		grid.repositionNow = true;
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "button1") {
			if (uid == UserManager.Instance.self.uid) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_63"), null);
				});

				return;
			}
			else {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("Guild_64"), appointBack);
				});

			}
		}
		else if (gameObj.name == "button2") {
			if (uid == UserManager.Instance.self.uid) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_65"), null);
				});
			}
			else {
				if (button2.textLabel.text == LanguageConfigManager.Instance.getLanguage ("Guild_73")) {
					GuildAppointViceFPort fport = FPortManager.Instance.getFPort ("GuildAppointViceFPort") as GuildAppointViceFPort;
					fport.access (uid, appBack);
				}
				else {
					GuildResignViceFPort fport = FPortManager.Instance.getFPort ("GuildResignViceFPort") as GuildResignViceFPort;
					fport.access (uid, appBack);
				}
			}

		}
		else if (gameObj.name == "button3") {
			if (uid == UserManager.Instance.self.uid) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_66"), null);
				});
			}
			else {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("Guild_101", memberName), clearMemFPort);
				});

			}
		}
		else if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	private void clearMemFPort (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		GuildClearMemFPort fport = FPortManager.Instance.getFPort ("GuildClearMemFPort") as GuildClearMemFPort;
		fport.access (uid, appBack);
	}
	private void appointBack (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		GuildAppointPresidentFPort fport = FPortManager.Instance.getFPort ("GuildAppointPresidentFPort") as GuildAppointPresidentFPort;
		fport.access (uid, appBack);
	}
	private void appBack () {
		GuildMemberWindow gmw = UiManager.Instance.getWindow<GuildMemberWindow> ();
		if (gmw != null)
			gmw.updateMember ();
		destoryWindow ();
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		changeButton ();
	}
}