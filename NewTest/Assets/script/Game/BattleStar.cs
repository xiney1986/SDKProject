using UnityEngine;
using System.Collections;

public class BattleStar : MonoBase
{
	[HideInInspector]
	public int
		addValue;

	public void destory ()
	{
		if (UiManager.Instance.missionMainWindow == null)
			return;
		EffectCtrl ctrl = EffectManager.Instance.CreateEffect (UiManager.Instance.UIEffectRoot.transform, "Effect/UiEffect/luckybox_effect");
		if (ctrl != null) {
			ctrl.transform.position = transform.position;
			ctrl.transform.localScale = Vector3.one;
			if (FuBenManagerment.Instance.checkStarMultipleTime ()) {
				Transform doubleTran = ctrl.gameObject.transform.FindChild ("double");
				doubleTran.GetComponent<UILabel> ().text = "X" + FuBenManagerment.Instance.getStarHit();
				TweenPosition.Begin (doubleTran.gameObject, 0.5f, doubleTran.localPosition + new Vector3 (Random.Range (10, 50), Random.Range (10, 50), 0)).PlayForward ();
			}
		}
		Destroy (gameObject);
	}

	void OnDisable ()
	{
		UserManager.Instance.self.addStar (addValue);
	}
}
