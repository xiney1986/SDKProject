using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentLaddersFriends : dynamicContent {
	
	protected List<FriendInviteInfo> friends;

	public void reLoad (List<FriendInviteInfo> list)
	{
		friends = new List<FriendInviteInfo>();
		//List<FriendInviteInfo> friends_temp = list;//FriendsManagerment.Instance.getFriendList();
		if(list.Count<=0)
		{
			return;
		}

		string currentOppUid=LaddersManagement.Instance.CurrentOppPlayer.uid;
		//如果对手是自己好友，则屏蔽掉
		for(int i=0,length=list.Count;i<length;i++)
		{
			if(list[i].getUid().Equals(currentOppUid))
			{
				continue;
			}
			friends.Add(list[i]);
		}

		if(friends != null)
		{
			base.reLoad(friends.Count);
		}
	}
	
	public override void updateItem (GameObject item, int index)
	{
		Ladders_FriendItem button=item.GetComponent<Ladders_FriendItem> ();
		button.M_update(friends[index]);
	}
	public override void OnDisable ()
	{
		base.OnDisable ();
	}
	public override void initButton (int i)
	{
		nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as LaddersFriendsWindow).friendsBarPrefab);
		nodeList [i].SetActive(true);
		nodeList [i].name = StringKit. intToFixString (i + 1);
		Ladders_FriendItem button=nodeList [i].GetComponent<Ladders_FriendItem> ();
		button.M_update(friends[i]);
	}
}