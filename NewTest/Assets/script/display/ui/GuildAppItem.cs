using UnityEngine;
using System.Collections;

/**
 * 公会审批单项
 * @author 汤琦
 * */
public class GuildAppItem : MonoBehaviour 
{
	public UITexture headIcon;
	public UILabel playerName;
	public UILabel id;
	public UILabel level;
	public UISprite vipLv;
	private GuildApprovalInfo app;
	private GuildAppWindow win;
	public GuildAppCanelButton canelButton;
	public GuildAppAcceptButton acceptButton;

	public void updateInfo(GuildApprovalInfo app,GuildAppWindow win)
	{
		this.win = win;
		this.app = app; 
		level.text = "LV " + app.level;
		if (app.vipLevel > 0) {
			vipLv.gameObject.SetActive (true);
			vipLv.spriteName = "vip" + app.vipLevel;
		} else {
			vipLv.gameObject.SetActive (false);
		}
		playerName.text = app.name;
		id.text =  StringKit.serverIdToFrontId(app.uid);
		canelButton.fatherWindow = win;
		canelButton.uid = app.uid;
		acceptButton.fatherWindow = win;
        acceptButton.uid = app.uid;
        ResourcesManager.Instance.LoadAssetBundleTexture(UserManager.Instance.getIconPath(StringKit.toInt( app.headIcon)),headIcon);
	}

	void OnClick()
	{
		MaskWindow.LockUI ();
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		fport.access(app.uid,null,null,PvpPlayerWindow.FROM_OTHER);
	}
}

