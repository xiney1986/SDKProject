using UnityEngine;
using System.Collections.Generic;

public class ContentFriends : dynamicContent {

	public FriendInfo[] ic;
	int fawinTapType;//属于哪个标签。0好友信息，1待批准，2查找申请，3公会查找,4天梯求助 , 5好友聊天

	public new void reLoad ( int _type ) {
		cleanAll();
		fawinTapType = _type;
		switch (fawinTapType) {
			case 0:
				ic = FriendsManagerment.Instance.getFriendList ();
				break;
			case 1:
				ic = FriendsManagerment.Instance.getRequestFriendList ();
				break;
			case 2:
				ic = FriendsManagerment.Instance.getRecommendFriends ();
				break;
			case 3:
				ic = FriendsManagerment.Instance.getRecommendFriends ();
				break;
			case 4:
				ic = FriendsManagerment.Instance.getFriendList ();
				break;
			case 5:
				//List<FriendInfo> tempList = new List<FriendInfo>();
				ic = FriendsManagerment.Instance.getFriendList ();
				if (ic != null) {
					System.Array.Sort (ic, ( a, b ) => {
						return a.getIsOnline () ? -1 : 1;
					});
				}
				//for (int i = 0; i < ic.Length; i++)
				//{
				//	if (ic[i].getIsOnline())
				//		tempList.Add(ic[i]);
				//}
				//ic = tempList.ToArray();
				break;
		}
		if (ic != null)
			base.reLoad (ic.Length);
	}

	public void reLoad (int type, FriendInfo[] ic) {
		fawinTapType = type;
		this.ic = ic;
		if (ic != null)
			base.reLoad (ic.Length);
	}


	public override void updateItem ( GameObject item, int index ) {
		if (fawinTapType == 4) {
			Ladders_FriendItem button=item.GetComponent<Ladders_FriendItem> ();
			button.M_update (ic[index]);
		}
		else {
			friendsItem button=item.GetComponent<friendsItem> ();
			button.initInfo (fawinTapType, ic[index], fawinTapType == 5);
		}
	}
	public override void OnDisable () {
		base.OnDisable ();
		//		cleanAll();
	}
	public override void initButton ( int i ) {


		if (fawinTapType == 4) {
			if (nodeList[i] == null) {
				nodeList[i] = NGUITools.AddChild (gameObject, (fatherWindow as LaddersFriendsWindow).friendsBarPrefab);
			}
			nodeList[i].SetActive (true);
			nodeList[i].name = StringKit.intToFixString (i + 1);
			Ladders_FriendItem button_4 = nodeList[i].GetComponent<Ladders_FriendItem> ();
			button_4.M_update (ic[i]);
			return;
		}



		if (fawinTapType == 3) {
			if (nodeList[i] == null) {
				nodeList[i] = NGUITools.AddChild (gameObject, (fatherWindow as GuildInviteMinWindow).friendsBarPrefab);
			}
		}
		else {
			if (nodeList[i] == null) {
				nodeList[i] = NGUITools.AddChild (gameObject, (fatherWindow as IFriendsWindow).FriendsBarPrefab);
			}
		}

		nodeList[i].name = StringKit.intToFixString (i + 1);
		friendsItem button = nodeList[i].GetComponent<friendsItem> ();
		button.fatherWindow = fatherWindow;
		button.initInfo (fawinTapType, ic[i], false);
	}





}
