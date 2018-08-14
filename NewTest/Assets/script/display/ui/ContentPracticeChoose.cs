using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentPracticeChoose : dynamicContent
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

		PracticeChooseButton button = item.GetComponent<PracticeChooseButton> ();
		button.updateButton (missonList [index]);
	}
	
	public override void initButton (int i)
	{
 
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as PracticeWindow).practiceBarPrefab);
		}
 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		PracticeChooseButton button = nodeList [i].GetComponent<PracticeChooseButton> ();
		button.fatherWindow = fatherWindow;
		button.updateButton (missonList [i]);
	}
}
