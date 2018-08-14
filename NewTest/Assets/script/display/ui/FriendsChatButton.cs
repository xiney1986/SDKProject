using UnityEngine;
using System.Collections;

public class FriendsChatButton : ButtonBase
{

	public FriendInfo info;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
//		fatherWindow.hideWindow();
		//激活的tap页清空,不然查看后返回好友窗口会因为激活的tapButton相同而return;
//		(fatherWindow as FriendsWindow).tapBase.resetTap();
        //getPlayerInfoFPort (info.getUid ());


		if(fatherWindow.GetFatherWindow() is ChatWindow){
			ChatWindow fatherWin = fatherWindow.GetFatherWindow() as ChatWindow;
			fatherWin.clear();
			EventDelegate.Add (fatherWin.OnStartAnimFinish, () => {
				fatherWin.initChatWindow(ChatManagerment.CHANNEL_FRIEND-1);
				UiManager.Instance.openDialogWindow<ChatSendMsgWindow>((win1) => {
					win1.initWindow(ChatManagerment.CHANNEL_FRIEND);
					win1.setFriendValue(info);
					fatherWindow.GetFatherWindow().OnStartAnimFinish.Clear();
				});
			});
			fatherWindow.finishWindow();
		}else{
			UiManager.Instance.switchWindow<ChatWindow>((win)=>{
				win.initChatWindow(ChatManagerment.CHANNEL_FRIEND-1);
				EventDelegate.Add (win.OnStartAnimFinish, () => {
					UiManager.Instance.openDialogWindow<ChatSendMsgWindow>((win1) => {
						win1.initWindow(ChatManagerment.CHANNEL_FRIEND);
						win1.setFriendValue(info);
						win.OnStartAnimFinish.Clear();
					});
				});
			});
		}
		}

		


    //private void getPlayerInfoFPort (string _uid)
    //{
    //    ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort ("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
    //    fport.access (_uid, reLoadFriendWin, PvpPlayerWindow.FROM_MISSION_NPC);
    //}
	
    //private void reLoadFriendWin (bool bl)
    //{
    //    if (bl)
    //        (fatherWindow as FriendsWindow).clickFriends ();
    //}
}
