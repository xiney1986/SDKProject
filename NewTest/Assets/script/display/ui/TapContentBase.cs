using UnityEngine;
using System.Collections;

//button工具导航栏
public class TapContentBase : MonoBehaviour
{
	public static Color activeLabelColor = new Color (0.9803f,0.9765f,0.9686f,1f);
	public static  Color normalLabelColor = new Color (0.6313f, 0.4549f, 0.3373f, 1f);
	public static Color activeSpriteColor = Color.white;
	public static Color normalSpriteColor = Color.white;
	public Vector3 activeSize = Vector3.one;
	public Vector3 normalSize = Vector3.one;
	public string activeImageName = "page_up";
	public string normalImageName = "page_down";
	public  TapButtonBase[] tapButtonList;
	[HideInInspector]
	TapButtonBase
		activeButton;
	public GameObject tapWindowStartPoint;
	public WindowBase fatherwindow;

	public TapButtonBase getTapButtonByIndex(int index) {
		if(index>tapButtonList.Length-1)
			return null;
		if(index<0)
			return null;
		return tapButtonList[index];
	}

	public int getActiveIndex ()
	{
		if (activeButton == null)
			return -1;
		for (int i=0; i< tapButtonList.Length; i++) {
			if (tapButtonList [i] == activeButton)
				return  i;
		}
		return -1;
	}

	public void resetTap ()
	{
		activeButton = null;
	}

	public void changeTapPage (TapButtonBase  tapButton)
	{
		try {
			changeTapPage (tapButton, true);
		} catch (System.Exception e) {
			Debug.LogWarning (e);
			MaskWindow.UnlockUI ();
		}
	}

	public void changeTapPage (TapButtonBase  tapButton, bool sendEvent)
	{
		//忽略已经激活的tap
		if (activeButton == tapButton)
			return;
		//如果点击的按钮要求只走事件不激活
		if (tapButton.doEventButNoActive) {
			fatherwindow.tapButtonEventBase (tapButton.gameObject, true);
			return;
		}

		activeButton = tapButton;
		changeAllTapState (sendEvent);
		if (fatherwindow != null)
			fatherwindow.OnTapUpdate ();
	}

	public virtual void changeAllTapState (bool sendEvent)
	{	
		foreach (TapButtonBase each in tapButtonList) {
			if (each != activeButton) {
				if (sendEvent) {
					fatherwindow.tapButtonEventBase (each.gameObject, false);
				}
				if (each.tapWindowObj != null) {
					each.tapWindowObj.SetActive (false);	
				}

				each.IsActiveTapPage = false;
			}
		}
		foreach (TapButtonBase each in tapButtonList) {
			if (each == activeButton) {

				if (each.tapWindowObj != null) {
					each.tapWindowObj.SetActive (true);
				}
				if (sendEvent) {
					fatherwindow.tapButtonEventBase (each.gameObject, true);
				}


				each.IsActiveTapPage = true;
			} 
			each.setTapPageState ();
		}
	}

	public void resetActiveTapPageState ()
	{
		if (activeButton != null) {
			activeButton.IsActiveTapPage = false;
			activeButton.setTapPageState ();
			activeButton = null;
		}
	}
}
