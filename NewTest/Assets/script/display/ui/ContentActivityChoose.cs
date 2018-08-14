using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentActivityChoose : dynamicContent {

	public List<Mission> missonList;
	private ActivityChapter chapter;
	
	public void Initialize(int[] msList,ActivityChapter _chapter)
	{
		chapter = _chapter;
		missonList=new List<Mission>();
		
		foreach(int each in msList){
			missonList.Add(  MissionInfoManager.Instance .getMissionBySid (each));
		}
		
		reLoad(missonList.Count);

		
	}
	
	
 public override void initButton (int i)
	{
 
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as ActivityChooseWindow ).activityChoosePrefab);
		}
			
		nodeList [i].name = StringKit. intToFixString (i + 1);
		ActivityChooseButton button = nodeList [i].GetComponent<ActivityChooseButton> ();
		button.fatherWindow = fatherWindow;
		button.updateButton (missonList[i],chapter);
		
 
		
	}
}
