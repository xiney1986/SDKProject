using UnityEngine;
using System.Collections;

/// <summary>
/// worldboss展示窗口
/// </summary>
public class WorldbossAwardWindow : WindowBase {
	public GameObject prefabAwardItem;
	public ContentWorldbossAwards awardContent;
	
	public override void OnStart () {
		prefabAwardItem.SetActive (false);
		if (!isAwakeformHide) {
			awardContent.init ();
		}
	}
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close")
			finishWindow ();
	}
	
}