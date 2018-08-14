using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentChapterSelect:dynamicContent
{
	public GameObject itemPrefab;	
	private List<ChapterSample> data;
	private int type;
	
	public void init(List<ChapterSample> list)
	{
		this.data = list;
		base.reLoad(Mathf.Max(1,list.Count));
	}
	
	public override void initButton (int i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, itemPrefab);
			nodeList [i].SetActive(true);
		}
		//nodeList [i].name = StringKit. intToFixString (i + 1);
		ChapterSelectItemView item = nodeList [i].GetComponent<ChapterSelectItemView> ();

		item.fatherWindow = fatherWindow;
		if(i >= data.Count)
			item.init (null);
		else
			item.init (data [i]);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		if(data==null || index>=data.Count || data [index]==null)
			return;		
		ChapterSelectItemView itemC = item.GetComponent<ChapterSelectItemView> ();
		itemC.init (data [index]);
	}
	public override void jumpToPage (int index)
	{
		base.jumpToPage(index);
		if (GuideManager.Instance.isEqualStep(9004000)||GuideManager.Instance.isEqualStep(13003000)||GuideManager.Instance.isEqualStep(20003000)
		    ||GuideManager.Instance.isEqualStep(103003000)||GuideManager.Instance.isEqualStep(126003000)||GuideManager.Instance.isEqualStep(120003000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
}


