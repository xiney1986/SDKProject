using UnityEngine;
using System.Collections;

/**
 * 公会邀请窗口
 * @author 汤琦
 * */
public class GuildInviteWindow : WindowBase {
	public GuildInviteContent content;
	public UILabel noInvitation;
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	
	//更新容器
	public void updateContent () {
		if (GuildManagerment.Instance.getGuildInviteList () != null)
			content.reLoad ();
		else {
			noInvitation.gameObject.SetActive (true);
		}
	}
}
