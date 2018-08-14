using UnityEngine;
using System.Collections;

/**
 * 公会邀请搜索窗口
 * @author 汤琦
 * */
public class GuildInviteMinWindow : WindowBase
{
	public UIInput searchId;
	public ButtonBase inviteButton;
	public ContentFriends content;//查找容器
	public GameObject friendsBarPrefab;
	private FriendInfo[] infos;

	protected override void DoEnable ()
	{
		base.DoEnable ();
		inviteButton.disableButton(true);
	}

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if(gameObj.name == "buttonInvite")
		{
			if(searchId.value == "")
			{
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_56"),null);
					MaskWindow.UnlockUI();
				});
				return;
			}
			if(GuildManagerment.Instance.isInvite())
			{
				if (infos[0] != null) {

					GuildInviteJoinFPort fport = FPortManager.Instance.getFPort("GuildInviteJoinFPort") as GuildInviteJoinFPort;
					fport.access(infos[0].getUid(),null);
				}
			}
			else
			{
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_39"),null);
				});
			}
			MaskWindow.UnlockUI();
		}
		else if (gameObj.name == "buttonSelect")
		{
			if (searchId.value.Replace (" ", "") == "" || searchId.value == null) {
				UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("Friend_InputNull"), null);
				MaskWindow.UnlockUI();
				return;
			}
			UiManager.Instance.openDialogWindow<FriendFindWindow>((win)=>{
				win.initWin (searchId.value, callBackByFind,callBackByFind2);
			});

		}
		else if (gameObj.name == "close")
		{
			finishWindow();
		}
	}

	//查找回调
	public void callBackByFind ()
	{

		infos = FriendsManagerment.Instance.getRecommendFriends ();
		if (infos[0] != null) {
			content.gameObject.SetActive (true);
			if(infos[0].getUid() == UserManager.Instance.self.uid)
				inviteButton.disableButton(true);
			else
				inviteButton.disableButton(false);
			content.reLoad (3);
		} else {
			UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("Friend_noRecommend"), null);
			content.gameObject.SetActive (false);
			inviteButton.disableButton(true);
		}
	}

	private void callBackByFind2()
	{
		content.gameObject.SetActive (false);
		inviteButton.disableButton(true);
		searchId.value = "";
	}
}
