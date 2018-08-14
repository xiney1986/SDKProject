using UnityEngine;
using System.Collections;

public class FriendsLookButton : ButtonBase
{
    public int type;
	public bool isLookMsg = false;
	public FriendInfo info;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
//      fatherWindow.hideWindow();
//		激活的tap页清空,不然查看后返回好友窗口会因为激活的tapButton相同而return;
//		(fatherWindow as FriendsWindow).tapBase.resetTap();
//	    getPlayerInfoFPort (info.getUid ());
		//添加sdk好友
		if (isLookMsg) {
			getPlayerInfoFPort (info.getUid ());
		    if (type == 2) {
			   fatherWindow.finishWindow ();
			}
			return;	
		}
		
		SdkFriendManager.Instance.getsendSdkidFriend (info.getUid (),
		                                              () => {
			SdkFriendManager.Instance.addFriendUidRecord(info.getUid());
			this.disableButton(true);
		});
		
	}
	
	public void setLookButton(FriendInfo infoc)
	{
		bool isfriend = SdkFriendManager.Instance.isSdkFriend (infoc.getplatUid());
		isfriend = SdkFriendManager.Instance.IsContentFriendUid(infoc.getUid()) == true ? true : isfriend;
		disableButton (isfriend);
	}
		
		
	private void getPlayerInfoFPort (string _uid)
	{
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort ("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		fport.access (_uid, reLoadFriendWin, PvpPlayerWindow.FROM_MISSION_NPC);
	}
	
	private void reLoadFriendWin (bool bl)
	{
		if (bl)
			(fatherWindow as FriendsWindow).clickFriends ();
	}




}
