using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankContent_1 : dynamicContent
{
	/** 3列用 */
	public GameObject itemPrefab3;
	private IList list;
	private int type;
	public WindowBase fatherwindow;

	public void init (int type, IList list, WindowBase wb)
	{
		this.type = type;
		this.list = list;
		this.fatherwindow =wb;
	}
	public override void initButton (int i)
	{
		if (nodeList [i] == null)
		{
			nodeList [i] = NGUITools.AddChild(gameObject, itemPrefab3);
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		RankItemView button= nodeList [i].GetComponent<RankItemView> ();
		button.setFatherWindow(this.fatherWindow);
		if (i >= list.Count)
			button.init (null, type, i);
		else
			button.init (list [i], type, i);
	}

	public override void updateItem (GameObject item, int index)
	{
		if (list == null || index >= list.Count || list [index] == null)
			return;
		RankItemView sc = item.GetComponent<RankItemView> ();
		sc.init (list [index], type, index);
	}
}
