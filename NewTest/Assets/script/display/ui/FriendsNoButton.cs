using UnityEngine;
using System.Collections;

public class FriendsNoButton : ButtonBase {

	public FriendInfo info;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(info == null)
			return;
		refuseApply();
	}
	
	//拒绝好友申请
	private void refuseApply()
	{
		FriendsFPort fport = FPortManager.Instance.getFPort ("FriendsFPort") as FriendsFPort;
		fport.refuseFriend(info.getUid(),sendMsgBack);
	}
	
	private void sendMsgBack()
	{
		if(fatherWindow  is FriendsWindow)
			(fatherWindow as FriendsWindow).updatepage(1);
		if(fatherWindow is FriendApplyWindow)
			(fatherWindow as FriendApplyWindow).reload();
		MaskWindow.UnlockUI ();
	}
}
