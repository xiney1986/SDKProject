using UnityEngine;
using System.Collections;

/**
 * 公会主界面（没有加入公会）
 * @author 汤琦
 * */
public class GuildMainNoJoinWindow : WindowBase
{
	public UILabel guildInviteTip;
	protected override void begin ()
	{
		base.begin ();
		showGuildInviateNum ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "createButton") {
			finishWindow ();
			EventDelegate.Add (OnHide, () => {
				if (UserManager.Instance.self.getUserLevel () < GuildManagerment.CREATEGUILDLEVEL) {
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("Guild_2", GuildManagerment.CREATEGUILDLEVEL.ToString ()), null);
					});
					return;
				}
				UiManager.Instance.openWindow<GuildCreateWindow> ();
			});
		} else if (gameObj.name == "applyButton") {
			this.finishWindow();
			UiManager.Instance.openWindow<GuildApplyWindow> ();
		} else if (gameObj.name == "inviteButton") {
			GuildGetInviteFPort fport = FPortManager.Instance.getFPort ("GuildGetInviteFPort") as GuildGetInviteFPort;
			fport.access (openInviteWindow);
		} else if (gameObj.name == "close") {
			finishWindow ();
		}
	}

	public void showGuildInviateNum(){
		if (GuildManagerment.Instance.getGuildInviteList () == null || GuildManagerment.Instance.getGuildInviteList ().Count == 0) {
			guildInviteTip.gameObject.SetActive (false);
		} else {
			guildInviteTip.text=GuildManagerment.Instance.getGuildInviteList ().Count.ToString();
			guildInviteTip.gameObject.SetActive (true);			
		}
	}

	private void openInviteWindow ()
	{
		finishWindow ();
		EventDelegate.Add (OnHide, () => {
			UiManager.Instance.openWindow<GuildInviteWindow> ((win) => { 
				win.updateContent ();
			});
		});
	}




}
