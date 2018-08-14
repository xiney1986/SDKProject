using UnityEngine;
using System.Collections;

//活动掉落查看窗口

public class ActivityPrizeViewWindow : WindowBase
{
	public ButtonActivityPrizeView[] viewButton;
 
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void Initialize (PrizeSample[] prize)
	{ 
		if (prize == null || prize.Length <= 0)
			return;
		
		for (int i = 0; i < viewButton.Length; i++) {
			if (i >= prize.Length || prize [i] == null) {
				viewButton [i].gameObject.SetActive (false);
				continue;
			} 
			viewButton [i].updateButton (prize [i]);
		} 
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
		} else {
			
			ButtonActivityPrizeView button = gameObj.GetComponent<ButtonActivityPrizeView> ();
			if (button != null) {
				if (button. item.GetType () == typeof(Equip) || button.item.GetType () == typeof(Card)) {
					UiManager.Instance.hideWindowByName("activityChooseWindow");
				}
				
			}
			
		}
	}
}
