using UnityEngine;
using System.Collections;

public class EquipAttrDesWindow : WindowBase
{
	public ButtonBase screenButton;
	public UILabel    buttonLabel;
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
	void Update ()
	{
		if (buttonLabel.gameObject.activeSelf) {
			float offset = Mathf.Sin (time * 6); 
			buttonLabel.alpha = sin ();
		}
	}
}
