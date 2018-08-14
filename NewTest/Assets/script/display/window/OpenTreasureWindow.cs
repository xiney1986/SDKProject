using UnityEngine;
using System.Collections;

public class OpenTreasureWindow : WindowBase
{

	public Transform pointOne;
	public Transform pointLeft;
	public Transform pointRight;
	public Award award;
	public CallBack closeCallback;

	protected override void begin ()
	{
		base.begin ();
		
		//只有金宝箱
		if (UiManager.Instance.missionMainWindow.goldBoxCount == 0) {
			EffectManager.Instance.CreateEffect (pointOne, "Effect/UiEffect/OpenTheTreasureChest2");

		}
		//只有银宝箱
		if (UiManager.Instance.missionMainWindow.sliverBoxCount == 0) {
			EffectManager.Instance.CreateEffect (pointOne, "Effect/UiEffect/OpenTheTreasureChest1");

		}	
		//都有
		if( UiManager.Instance.missionMainWindow.goldBoxCount != 0 && UiManager.Instance.missionMainWindow.sliverBoxCount != 0){
			EffectManager.Instance.CreateEffect (pointLeft, "Effect/UiEffect/OpenTheTreasureChest1");		
		EffectManager.Instance.CreateEffect (pointRight, "Effect/UiEffect/OpenTheTreasureChest2");		
		}
		StartCoroutine (closeDelay ());
		MaskWindow.LockUI ();
	}
	
	IEnumerator closeDelay ()
	{
		yield return new WaitForSeconds(2f);
		hideWindow ();
		if(closeCallback != null)
			closeCallback();

		//AwardDisplayCtrl ctrl = MissionManager.instance.gameObject.GetComponent<AwardDisplayCtrl> ();
		//ctrl .StartCoroutine(	ctrl.openNextWindow());
	}
}
