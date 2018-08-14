using UnityEngine;
using System.Collections;

public class InviteSendCodeWindow : WindowBase {
	
	public UIInput playerCode;
	public GameObject getButton;
	public GameObject noGetButton;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	public void initWindow(int _invtiteType)
	{
		getButton.SetActive (false);
		noGetButton.SetActive (false);
		isGet(_invtiteType);
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "buttonToGet") {
            if (playerCode.label.text=="") {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("notice_jihuomalw0"));
            });
            }else{
                (fatherWindow as InviteCodeWindow).invtiteCodeFport (playerCode.label.text);
                this.finishWindow();
            }
		} else if (gameObj.name == "kuang_back3") {
			this.finishWindow ();
		} else if (gameObj.name == "close") {
			finishWindow();
		}
	}
	
	public void isGet(int _type)
	{
		if(_type == 0)
		{
			getButton.SetActive (false);
			noGetButton.SetActive (true);
		}
		if(_type == 1)
		{
			getButton.SetActive (true);
			noGetButton.SetActive (false);
		}
	}
}
