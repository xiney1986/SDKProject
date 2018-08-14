using UnityEngine;
using System.Collections;

/**
 * 公会审批批准按钮
 * @author 汤琦
 * */
public class GuildAppAcceptButton : ButtonBase
{
	public string uid;
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(GuildManagerment.Instance.getGuild().membership >= GuildManagerment.Instance.getGuild().membershipMax)
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("Guild_18"),null);
			});
			return;
		}
		GuildApproveFPort fport = FPortManager.Instance.getFPort("GuildApproveFPort") as GuildApproveFPort;
		fport.access(uid,fportBack);
	}

	private void fportBack()
	{
		(fatherWindow as GuildAppWindow).UpdateContent();
		MaskWindow.UnlockUI();
	}


}
