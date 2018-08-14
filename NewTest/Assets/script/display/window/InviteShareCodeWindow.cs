using UnityEngine;
using System.Collections;

public class InviteShareCodeWindow : WindowBase {
	//public UIInput playerCode;
	//public GameObject getButton;
	//public GameObject noGetButton;
	//-----
	public GameObject shareToGroup;
	public GameObject shareToMessageBoard;
	public UILabel shareCodeContent;//分享内容
	Json_ServerInfo latelyServer = null;
	private string serverName;//获取服务器名

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		latelyServer = ServerManagerment.Instance.lastServer;
		if (latelyServer != null) {
			serverName= latelyServer.name;
		} 
		shareCodeContent.text =LanguageConfigManager.Instance.getLanguage("shareContent01",serverName,UserManager.Instance.self.nickname,StringKit.serverIdToFrontId(UserManager.Instance.self.uid));
	}
	
	public void initWindow(int _invtiteType)
	{
		shareToGroup.SetActive (true);
		shareToMessageBoard.SetActive (true);
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);	
		if (gameObj.name == "shareToGroup") {
			this.finishWindow ();
		} else if (gameObj.name == "kuang_back3") {
			this.finishWindow ();
		}else if (gameObj.name == "close") {
			finishWindow();
		}
	}
}
