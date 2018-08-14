using UnityEngine;
using System.Collections;

public class TowerInfoShowWindow : WindowBase
{
	/* fields */
	/**关闭按钮 */
	public UILabel closeLabel;

	/* methods */
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close") {
			finishWindow ();
		} 
	}
	/***/
	void Update () {
		UpdateCloseLable ();
	}
	private void UpdateCloseLable() {
		if(closeLabel.gameObject.activeSelf)
			closeLabel.alpha = sin ();
	}
}
