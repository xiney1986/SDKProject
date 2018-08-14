using UnityEngine;
using System.Collections;

/**
 * 好友相关推送服务
 * @authro 陈世惟  
 * */
public class FriendsShareService : BaseFPort {

	public FriendsShareService () {
		
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message);

		string str = (message.getValue ("type") as ErlAtom).Value;
//		Debug.LogError ("------------------->>>>>>" + str);
		
		ErlType msg = message.getValue ("value") as ErlType;
		ErlArray array = msg as ErlArray;

		GameObject win = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/FriendsShareWindow");
		GameObject mainShareWin = GameObject.Find("/NGUI_manager/GameCamera/UIScaleRoot/IncreaseWayWindow");

		switch(str)
		{
			//我的待分享列表
		case "share_list":
			if(FriendsShareManagerment.Instance == null)
				return;
			FriendsShareManagerment.Instance.addShareInfoByErlArray(array);
			if (win != null) {
				(win.GetComponent<FriendsShareWindow> ()).initWin(true,0);
				(win.GetComponent<FriendsShareWindow> ()).changeShareButton(true);
			}
			if (mainShareWin != null)
				(mainShareWin.GetComponent<IncreaseWayWindow> ()).showShare();
			break;

			//好友可点赞分享列表
		case "shared_list":
			if(FriendsShareManagerment.Instance == null)
				return;
			FriendsShareManagerment.Instance.addPraiseInfoByErlArray(array);
			if (win != null) {
				(win.GetComponent<FriendsShareWindow> ()).initWin(true,1);
				(win.GetComponent<FriendsShareWindow> ()).changePraiseButton(true);
			}
			if (mainShareWin != null)
				(mainShareWin.GetComponent<IncreaseWayWindow> ()).showShare();
			break;
		}
	}
}
