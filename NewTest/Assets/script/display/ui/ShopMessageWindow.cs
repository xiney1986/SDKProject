using UnityEngine;
using System.Collections;

public class ShopMessageWindow : MessageWindow {

	public UIToggle toggle;

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "button_1") {
			msg.buttonID = MessageHandle.BUTTON_LEFT;
			msg.msgEvent = msg_event.dialogCancel;
			
		} else if (gameObj.name == "button3") { 
			if (GuideManager.Instance.isEqualStep (22011000)) {
				GuideManager.Instance.doGuide ();
				GuideManager.Instance.guideEvent (); 
			}
			msg.buttonID = MessageHandle.BUTTON_MIDDLE;
			msg.msgEvent = msg_event.dialogOK;
			ShopManagerment.Instance.isOpenOneKey = toggle.value;
			
		} else {
			msg.buttonID = MessageHandle.BUTTON_RIGHT;
			msg.msgEvent = msg_event.dialogOK;
			ShopManagerment.Instance.isOpenOneKey = toggle.value;
		}
		finishWindow ();
		
	}
}
