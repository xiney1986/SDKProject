using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentRecharge : dynamicContent
{
    public List<string> sidList=null;

	public override void  updateItem (GameObject item, int index)
	{

		ButtonRechargeItem button = item.GetComponent<ButtonRechargeItem> ();
        button.setCashSidList(sidList);		 
		button.updateButton (SdkManager.jsonGoodsList [index]);
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			if(SdkManager.jsonGoodsList [i].name=="banner")
			{
				//nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as rechargeWindow).bannerPrefab);
			}
			else
			{
				nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as rechargeWindow).rechargeButtonPrefab);		
			}
		}


		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonRechargeItem button = nodeList [i].GetComponent<ButtonRechargeItem> ();
        button.setCashSidList(sidList);	
		button.fatherWindow = fatherWindow;
		button.updateButton (SdkManager.jsonGoodsList [i]);

	}
}
