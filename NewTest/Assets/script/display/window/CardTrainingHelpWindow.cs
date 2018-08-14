using UnityEngine;
using System.Collections;

public class CardTrainingHelpWindow : WindowBase {
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			this.finishWindow();
		}
	}
}
