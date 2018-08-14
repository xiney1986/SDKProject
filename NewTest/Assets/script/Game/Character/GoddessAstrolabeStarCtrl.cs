using UnityEngine;
using System.Collections;

public class GoddessAstrolabeStarCtrl : MonoBase
{

	private GoddessAstrolabeStarButton button;
	private GoddessAstrolabeWindow fawin;
	private GoddessAstrolabeSample info;
	private goddessAstrolabeMoveItem item;

	public void init (GoddessAstrolabeSample _data, GoddessAstrolabeWindow _fawin)
	{
		//相互关联
		this.info = _data;
		fawin = _fawin;
		
//		passObj obj = Create3Dobj ("Goddess/ClickButton/ClickButton");
		GameObject objs = Instantiate (_fawin.titleLabelPrefab) as GameObject;
		objs.transform.parent = fawin.UIEffectRoot;
		
		objs.transform.localScale = Vector3.one;
		objs.transform.localPosition = Vector3.zero;
		button = objs.GetComponent<GoddessAstrolabeStarButton> ();
		item = button.GetComponent<goddessAstrolabeMoveItem> ();
		item.fawin = fawin;
		button.name = info.id.ToString ();
		button.init (info);
		button.setFatherWindow (fawin);
		if (info.isOpen)
			hideUI ();
		else
			showUI ();
	}
	//FixedUpdate
	void Update ()
	{
		if (button != null && (fawin.lookType == 2 || !isInsideScreen ())) {
			button.gameObject.SetActive (false);
		} else if (button != null && fawin.lookType == 1 && !info.isOpen && isInsideScreen ()) {
			showUI ();
		} else if (button != null && fawin.lookType == 1 && info.isOpen && isInsideScreen ()) {
			hideUI ();
		}
		if (button != null && button.gameObject.activeSelf) {
			Vector3 pos = fawin.gaCamera.WorldToScreenPoint (transform.position);
			pos += new Vector3 (0, -50, 0);
			button.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
			button.transform.position = new Vector3 (button.transform.position.x, button.transform.position.y, 0);
		}
	}

	public void showUI ()
	{
		if (button == null || info == null)
			return;
		if (info.father == 0) {
			button.gameObject.SetActive (true);
			button.spriteBg.gameObject.SetActive (true);
			button.textLabel.text = info.awardDesc;
			if (GuideManager.Instance.isMoreThanStep (101007000)) {
				button.setArrow (true);
			} else {
				button.setArrow (false);
			}
			return;
		}
		if (!GoddessAstrolabeManagerment.Instance.getFatherStarIsOpen (info)) {
			button.gameObject.SetActive (false);
			button.setArrow (false);
		} else {
			button.gameObject.SetActive (true);
			button.setArrow (true);
			button.spriteBg.gameObject.SetActive (true);
			button.textLabel.text = info.awardDesc;
		}
	}

	public void hideUI ()
	{
		button.gameObject.SetActive (true);
		button.textLabel.text = "";
		button.spriteBg.gameObject.SetActive (false);
	}

	/// <summary>
	/// 我是不是在屏幕范围内呀
	/// </summary>
	public bool isInsideScreen ()
	{
		Vector3 pos = fawin.gaCamera.WorldToScreenPoint (this.transform.position);
		float minX = 0 * UiManager.Instance.screenScaleX;
		float maxX = Screen.width * UiManager.Instance.screenScaleX;
		float minY = 0 * UiManager.Instance.screenScaleY;
		float maxY = Screen.height * UiManager.Instance.screenScaleY;
		return pos.x > minX && pos.x < maxX && pos.y > minY && pos.y < maxY;
	}

	public Vector3 getPos ()
	{
		return this.transform.position;
	}

	public GoddessAstrolabeSample getInfo ()
	{
		return info;
	}
}
