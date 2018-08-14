using UnityEngine;
using System.Collections;

/**
 * 好友相关推送服务
 * @authro 陈世惟  
 * */
public class FriendsService : BaseFPort {

	public FriendsService () {

	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);
		string str = (message.getValue ("type") as ErlAtom).Value;

		ErlType msg = message.getValue ("value") as ErlType;
		ErlArray array = msg as ErlArray;
		GameObject win = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/FriendsWindow");
		GameObject pvpWin = GameObject.Find("/NGUI_manager/GameCamera/UIScaleRoot/PvpPlayerWindow");

		switch(str)
		{
		//别人申请你
		case "apply":
			if(FriendsManagerment.Instance == null)
				return;
			FriendsManagerment.Instance.addRefuseFriend(array);
			GameObject mainWin = GameObject.Find("/NGUI_manager/GameCamera/UIScaleRoot/MainWindow");

			if (win != null){
				if((win.GetComponent<FriendsWindow> ()).getTapType() == 1)
					(win.GetComponent<FriendsWindow> ()).content2.reLoad(1);
				else
					(win.GetComponent<FriendsWindow> ()).showNewApply();
			}
//新改版后没有实际意义
//			else if (pvpWin != null)
//				(pvpWin.GetComponent<PvpPlayerWindow>()).initCallBack(null);
			else if (mainWin != null)
				(mainWin.GetComponent<MainWindow>()).showFriendNum();
			break;

		//别人同意你的申请
		case "agree":
			if(FriendsManagerment.Instance == null)
				return;
			FriendsManagerment.Instance.addFriend(array,true);

			if (win != null)
				(win.GetComponent<FriendsWindow> ()).initWin(0);
//新改版后没有实际意义
//			else if (pvpWin != null)
//				(pvpWin.GetComponent<PvpPlayerWindow>()).initCallBack(null);
			break;


			//你自动同意增加好友
		case "agree_1":
			if(FriendsManagerment.Instance == null)
				return;
			FriendsManagerment.Instance.addFriend(array,false);
			
			if (win != null)
				(win.GetComponent<FriendsWindow> ()).initWin(0);
			//新改版后没有实际意义
			//			else if (pvpWin != null)
			//				(pvpWin.GetComponent<PvpPlayerWindow>()).initCallBack(null);
			break;
		//别人解除和你的好友关系
		case "delete":
			if(FriendsManagerment.Instance == null)
				return;
			FriendsManagerment.Instance.deleteFriend(array);
			
			if (win != null)
				(win.GetComponent<FriendsWindow> ()).initWin(0);
//新改版后没有实际意义
//			else if (pvpWin != null)
//				(pvpWin.GetComponent<PvpPlayerWindow>()).initCallBack(null);
			break;
        case "update" :
            FriendsManagerment.Instance.updateFriend(array);
            break;
		default:
			MonoBase.print (GetType () + "==error:"+str);
			break;
		}
	}

	//浏览玩家信息时删除和同意申请后回调到好友列表
	public void delGoback()
	{
		UiManager.Instance.openWindow<FriendsWindow>((win)=>{
			win.initWin(0);
		});
	}
	//浏览玩家信息时别人申请你后回调到好友列表
	public void applyGoback()
	{
		UiManager.Instance.openWindow<FriendsWindow>((win)=>{
			win.initWin(1);
		});
	}

}
