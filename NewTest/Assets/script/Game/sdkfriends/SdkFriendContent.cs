using UnityEngine;
using System.Collections;

public class SdkFriendContent : dynamicContent {

	
	public SdkFriendsInfo[] sdkFriendInfos;
	int type = 0;
	
	public  new void reLoad (int _type)
	{
		cleanAll();
		type = _type;
		if (_type == 1)
		{
			sdkFriendInfos =  SdkFriendManager.Instance.SdkFriendsInfos;
			if (sdkFriendInfos != null && sdkFriendInfos.Length > 0)
			{
				base.reLoad(sdkFriendInfos.Length);
			}
		} else if (_type == 2) {
			sdkFriendInfos =  SdkFriendManager.Instance.difServerFriendsInfos;
			if (sdkFriendInfos != null && sdkFriendInfos.Length > 0)
			{
				base.reLoad(sdkFriendInfos.Length);
			}
		}
	}
	
	public override void updateItem (GameObject item, int index)
	{
		
		SdkFriendsItem button = item.GetComponent<SdkFriendsItem> ();
		button.initItem(sdkFriendInfos[index],fatherWindow);
		
	}
	
	public override void OnDisable ()
	{
		base.OnDisable ();
	}
	
	
	public override void initButton (int i)
	{
		if (nodeList == null || i >= nodeList.Count) return;
		if (nodeList[i] == null)
		{
			nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as SdkFriendWindow).friendsBarPrefab);
		}
		nodeList[i].name = StringKit.intToFixString(i + 1);
		nodeList[i].gameObject.SetActive(true);
		SdkFriendsItem button = nodeList[i].GetComponent<SdkFriendsItem	>();
		button.initItem(sdkFriendInfos[i],fatherWindow);
	}
}
