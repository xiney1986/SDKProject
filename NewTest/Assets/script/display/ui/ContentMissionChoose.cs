using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentMissionChoose : dynamicContent
{
	public	List<Mission> missonList;
	
	public void Initialize (int[] msList)
	{
		missonList = new List<Mission> ();
		
		foreach (int each in msList) {
			missonList.Add (MissionInfoManager.Instance .getMissionBySid (each));
		}
		
		reLoad (missonList.Count);

		
	}

	public override void updateItem (GameObject item, int index)
	{
		
		if (item == null)
			return;

		MissionChooseButton button = item.GetComponent<MissionChooseButton> ();
		button.updateButton (missonList [index]);
		
	}
	
	public override void initButton (int i)
	{
 
		if (nodeList [i] == null){
//			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as MissionChooseWindow ).missionChooseBarPrefab);
		}
			
 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		
		MissionChooseButton button = nodeList [i].GetComponent<MissionChooseButton> ();
		button.fatherWindow = fatherWindow;
		button.updateButton (missonList [i]);
	}
}
