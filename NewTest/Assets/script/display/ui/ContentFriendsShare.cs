using UnityEngine;
using System.Collections;

public class ContentFriendsShare : dynamicContent {

	ShareInfo[] info;
	int fawinTapType;//属于哪个标签。0好友信息，1待批准，2查找申请
	
	public void InitUI (int _type)
	{
		fawinTapType = _type;
		switch(fawinTapType)
		{
		case 0:
			info = FriendsShareManagerment.Instance.getShareInfo();
			break;
		case 1:
			info = FriendsShareManagerment.Instance.getPraiseInfo();
			break;
		}
		if(info != null)
			base.reLoad (info.Length);
	}
	
	public void reLoadUI (int _type)
	{
		fawinTapType = _type;
		switch(fawinTapType)
		{
		case 0:
			info = FriendsShareManagerment.Instance.getShareInfo();
			break;
		case 1:
			info = FriendsShareManagerment.Instance.getPraiseInfo();
			break;
		}
		if(info != null)
			base.reLoad(info.Length);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		FriendsShareItemButton button=item.GetComponent<FriendsShareItemButton> ();
		button.initUI(fawinTapType,info[index]);
	}
	
	public override void initButton (int i)
	{
		if(nodeList [i] ==null)
			nodeList [i] =NGUITools.AddChild(gameObject, ( fatherWindow as FriendsShareWindow).shareButtonPrefab);

		//nodeList [i].name = StringKit. intToFixString (i + 1);
		FriendsShareItemButton button=nodeList [i].GetComponent<FriendsShareItemButton> ();
		button.fatherWindow=fatherWindow;
		button.initUI(fawinTapType,info[i]);
	}
}
