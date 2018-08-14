using UnityEngine;
using System.Collections;

public class PersonalAwardDescriptionWindow : WindowBase
{
	public ButtonBase screenButton;
	private float time;
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "screenButton") {
			finishWindow ();
		}
	}
}
