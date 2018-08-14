using UnityEngine;
using System.Collections;

public class FriendsYesButton : ButtonBase {

	public FriendInfo info;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(info == null)
			return;
		agreeApply();
	}

	//同意好友申请
	private void agreeApply()
	{
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.agreeFriend(info,sendMsgBack);
	}
	
	private void sendMsgBack()
	{
        //(fatherWindow as FriendsWindow).updatepage(1);
        ((FriendApplyWindow)fatherWindow).reload();
		MaskWindow.UnlockUI ();
	}
}
