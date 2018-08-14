using UnityEngine;
using System.Collections;

public class ContentOneRmbGift : ContentBase
{

	private PrizeSample[] prizes;
	public UISprite leftArrow;
	public UISprite rightArrow;
	
	public void initGift (PrizeSample[] _prizes, WindowBase win)
	{
		fatherWindow = win;
		prizes = _prizes;
	}
	
	public override void CreateButton (int index, GameObject page, int buttonIndex)
	{
		base.CreateButton (index, page, buttonIndex);
		if (index == -1)
			return;
		PrizesModule button = page.transform.GetChild (buttonIndex).GetComponent<PrizesModule> ();
		button.initPrize (prizes [index], windowBack, fatherWindow);

		if (index == prizes.Length - 1) {
			if (index % 4 != 0 || index == 0) {
				setHide (page, index % 4 + 1);
			}
		}
	} 
	
	private void windowBack ()
	{
		UiManager.Instance.openMainWindow ();
		//UiManager.Instance.openWindow<OneRmbWindow>();
	}
	
	private void setHide (GameObject page, int index)
	{
		for (int i = 0; i < page.transform.childCount - index; i++) {
			page.transform.GetChild (index + i).gameObject.SetActive (false);
		}
		
	}
	
	public override void updateActive (GameObject obj, int pageNUm)
	{
		updateContent (activeGameObj, pageNUm);
		if (prizes.Length > 4) {
			if (pageNUm == 1) {
				leftArrow.gameObject.SetActive (false);
				rightArrow.gameObject.SetActive (true);
			} else {
				leftArrow.gameObject.SetActive (true);
				rightArrow.gameObject.SetActive (false);
			}
		}
		else
		{
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (false);
		}
		
	}
	
	
	
}
