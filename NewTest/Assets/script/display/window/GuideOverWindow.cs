using UnityEngine;
using System.Collections;

/**
 * 冒险提示窗口
 * @authro 陈世惟  
 * */
public class GuideOverWindow : WindowBase {

	public UILabel blinkLabel;
	CallBack callback;


	public void initWindow(CallBack call)
	{
		callback = call;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "screenButton") {
			finishWindow ();
		}
	}

	public override void DoDisable ()
	{
		base.DoDisable (); 
		GuideManager.Instance.isOpenMsgWin = false;
		if (callback != null) {
			callback ();
			callback = null;
		} 
	}
	
	// Update is called once per frame
	void Update () {
		blinkLabel.alpha = sin ();
	}
}
