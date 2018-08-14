using UnityEngine;
using System.Collections;

public class FriendFindWindow : WindowBase {

	private CallBack callback1;
	private CallBack callback2;
	private CallBack<int> callbackI;
	private string inpuString;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void initWin(string inpuStr,CallBack _callback1,CallBack _callback2)
	{
		callback1 = _callback1;
		callback2 = _callback2;
		inpuString = inpuStr;
	}

	public void chooseWho(CallBack<int> i)
	{
		callbackI = i;
	}

	//根据类型发送查找内容
	public bool checkInput(string str)
	{
		if(str.Replace(" ","") == "")
			return false;
		if(str == null)
			return false;
		return true;
	}

	public void getFindFriend(int _type,string _str)
	{
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.findFriend(_type,_str,callback1);
		destoryWindow();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		switch(gameObj.name)
		{
		case "close":
			destoryWindow();
			break;
		case "button_id":
			if(!checkInput(inpuString))
				return;
			if(!StringKit.isNum(inpuString))
			{
				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("FriendErrorId"),null);
				callback2();
				finishWindow();
				return;
			}
			string _uid = StringKit.frontIdToServerId(inpuString);
			if (_uid == "error") {
				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("FriendErrorId"),null);
				callback2();
				finishWindow();
				return;
			}
			getFindFriend(0,_uid);
			if(callbackI != null)
				callbackI(1);
			break;
		case "button_name":
			checkInput(inpuString);
			getFindFriend(1,inpuString);
			if(callbackI != null)
				callbackI(2);
			break;
		}
	}

}
