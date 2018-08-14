using UnityEngine;
using System.Collections;

public class SdkPrizeContent : dynamicContent {
	
	
	 InvitePrize[] prizeInfo;
	  
	
	public  new void reLoad (int _type)
	{
		cleanAll();
		prizeInfo = InvitePrizeManager.Instance.InvitePrizes;
		
		if (prizeInfo != null && prizeInfo.Length > 0)
		{
			base.reLoad(prizeInfo.Length);
		}
	}
	
	public override void updateItem (GameObject item, int index)
	{
		
		InvitePrizeItem button=item.GetComponent<InvitePrizeItem> ();
		button.initItem(prizeInfo[index],fatherWindow);
	}
	
	public override void OnDisable ()
	{
		base.OnDisable ();
	}
	
	public override void initButton (int i)
	{
		
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as SdkFriendWindow).awardsBarPrefab);
		nodeList[i].gameObject.SetActive(true);
		nodeList[i].name = StringKit.intToFixString(i + 1);
		InvitePrizeItem button = nodeList[i].GetComponent<InvitePrizeItem>();
		button.initItem(prizeInfo[i],fatherWindow);
	}
}