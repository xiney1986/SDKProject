using UnityEngine;
using System.Collections;

public class ExitWindow : WindowBase {

	/** 是否有新手遮罩 */
	private bool isHaveGuide = false;

	protected override void DoEnable ()
	{
		base.DoEnable ();
		if(GuideManager.Instance.guideUI != null && GuideManager.Instance.guideUI.gameObject.activeSelf) {
			isHaveGuide = true;
			GuideManager.Instance.guideUI.gameObject.SetActive (false);
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "button_yes") {
            Application.Quit();
        } else  {
			finishWindow();
		}
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		GameManager.Instance.isOpenExitWin = false;
		if(isHaveGuide && GuideManager.Instance.guideUI != null) {
			isHaveGuide = false;
			GuideManager.Instance.guideUI.gameObject.SetActive (true);
		}
	}
}
