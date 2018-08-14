using UnityEngine;
using System.Collections;

/**
 * 公会成员头像按钮
 * @author 汤琦
 * */
public class GuildMemberHeadButton : ButtonBase
{
	private string uid;

	public void initInfo(string _uid)
	{
		this.uid = _uid;
	}


	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (fatherWindow  is GuildInfoWindow) {
			ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
			fport.access(uid,()=>{
				UiManager.Instance.hideWindowByName("guildInfoWindow");
			},()=>{
				UiManager.Instance.openWindow<GuildInfoWindow>();
			},PvpPlayerWindow.FROM_UNION);
		} else {
			ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort ("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
			fport.access (uid, null, PvpPlayerWindow.FROM_UNION);
		}
	}

}
