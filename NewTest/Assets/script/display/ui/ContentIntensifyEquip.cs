using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentIntensifyEquip : dynamicContent
{
	ArrayList RoleList;
	public List<ButtonIntensifyEquip> buttonList;
	public GameObject equipButtonPrefab;

	public  void Initialize (ArrayList Roles)
	{
		RoleList = Roles;
		buttonList = new List<ButtonIntensifyEquip> ();
		base.reLoad (Roles.Count);

	}

	public  void reLoad (ArrayList Roles)
	{
		RoleList = Roles;
		buttonList = new List<ButtonIntensifyEquip> ();
		base.reLoad (Roles.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//	base.updateItem (item, index);
		ButtonIntensifyEquip button = item.GetComponent<ButtonIntensifyEquip> ();
		button.updateButton (RoleList [index] as  Equip);
 
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, equipButtonPrefab);
		}
			
		nodeList [i].name = StringKit.intToFixString (i + 1);
		ButtonIntensifyEquip button = nodeList [i].GetComponent<ButtonIntensifyEquip> ();
		button.LockOnClick = false;
		buttonList.Add (button);
		button.fatherWindow = fatherWindow;
		button.Initialize (RoleList [i] as Equip);
 
	}

	//主卡和食物卡点击处理函数
	public void updateButton (ButtonIntensifyEquip buttonEquip)
	{
		if (buttonEquip.equip == null)
			return;

		ButtonIntensifyEquip button;
		for (int i = 0; i < nodeList.Count; i++) {
			if(nodeList[i]!=null){
			button = nodeList [i].GetComponent<ButtonIntensifyEquip> ();
			if (button.equip.uid == buttonEquip.equip.uid) {
				button.DoClickEvent ();
				return;
				}
			}
		}
		//点击食物卡，食物卡不在容器中显示
		buttonEquip.putOff ();
	}
	public override void jumpToPage (int index)
	{
		base.jumpToPage(index);
		if (GuideManager.Instance.isEqualStep(124006000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
}

