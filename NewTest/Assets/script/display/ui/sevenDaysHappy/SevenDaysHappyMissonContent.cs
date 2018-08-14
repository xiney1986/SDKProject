using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SevenDaysHappyMissonContent : dynamicContent
{
	public GameObject missonPrefab;// 任务预设//
	List<SevenDaysHappyMisson> missonList;// 任务//
	SevenDaysHappyDetail detail;
	WindowBase fatherWin;
	SevenDaysHappyContent content;
	SevenDaysHappyDetailBtn detailBtn;

	public void reLoad(SevenDaysHappyDetail detail,WindowBase win,SevenDaysHappyContent content,SevenDaysHappyDetailBtn detailBtn)
	{
		this.content = content;
		this.fatherWin = win;
		this.detail = detail;
		this.detailBtn = detailBtn;
		missonList = detail.missonList;
		base.reLoad (missonList.Count);
	}
	
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, missonPrefab);
		}
	}
	
	public override void updateItem (GameObject item, int index)
	{
		SevenDaysHappyMissonItem mItem = item.GetComponent<SevenDaysHappyMissonItem>();
		mItem.updateItem(missonList[index],fatherWin,this,content,detailBtn);
	}

	// 根据条件删选任务//
	public List<SevenDaysHappyMisson> filterMissonList(List<SevenDaysHappyMisson> list)
	{
		List<SevenDaysHappyMisson> missonList = new List<SevenDaysHappyMisson>();

		return missonList;
	}

	public void destroyMissons()
	{
		for(int i=0;i<gameObject.transform.childCount;i++)
		{
			Destroy(gameObject.transform.GetChild(i).gameObject);
		}
	}
}
