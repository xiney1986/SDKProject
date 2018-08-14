using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentIntensifyCardChoose : dynamicContent
{
	private ArrayList roleList;
	private int selectType = 0;
	private int intoType = 0;
	public GameObject CardChooseButtonPrefab;

	public ArrayList getRoleList()
	{
		return roleList;
	}

	public void Initialize (ArrayList roles,int selectType,int intoType)
	{
		this.intoType = intoType;
		this.selectType = selectType;
		roleList = roles;
		base.reLoad (roleList.Count);
	}
	
	public void reLoad (ArrayList Roles,int selectType)
	{
		this.selectType = selectType;
		roleList = Roles;
		base.reLoad (Roles.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//	base.updateItem (item, index);
		ButtonIntensifyCard button = item.GetComponent<ButtonIntensifyCard> ();
		button.updateButton (roleList [index] as Card);
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, CardChooseButtonPrefab);
		}
		ButtonIntensifyCard button = nodeList [i].GetComponent<ButtonIntensifyCard> ();
		button.fatherWindow = fatherWindow;
		button.Initialize (roleList [i] as Card,selectType,intoType);
	}
	public override void jumpToPage (int index)
	{
		base.jumpToPage(index);
		if (GuideManager.Instance.isEqualStep(12006000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
}
