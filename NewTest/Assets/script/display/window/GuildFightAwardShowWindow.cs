using UnityEngine;
using System.Collections;

/// <summary>
/// 工会战奖励选择窗口
/// </summary>
public class GuildFightAwardShowWindow : WindowBase {

	protected override void begin ()
	{
		base.begin ();
        MaskWindow.UnlockUI();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
        //if (gameObj.name == "ButtonClose")
        //{
        //    finishWindow();
        //}
        if (gameObj.name == "ButtonAwardBox")
        {
            UiManager.Instance.openWindow<GuildFightAwardWindow>();
        }
        else if (gameObj.name == "ButtonAwardPersonal")
        {
            UiManager.Instance.openDialogWindow<PersonalAwardDescriptionWindow>();
        }
        finishWindow();
	}
}
