using UnityEngine;
using System.Collections;

/**
 * 公会审批拒绝按钮
 * @author 汤琦
 * */
public class GuildAppCanelButton : ButtonBase
{
	public string uid;
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		GuildRejectFPort fport = FPortManager.Instance.getFPort("GuildRejectFPort") as GuildRejectFPort;
		fport.access(uid,fportBack);
	}

	
	private void fportBack()
	{
		(fatherWindow as GuildAppWindow).UpdateContent();
		MaskWindow.UnlockUI();
	}

}
