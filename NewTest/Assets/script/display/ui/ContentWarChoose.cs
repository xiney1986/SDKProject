using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentWarChoose : dynamicContent
{
	public	List<Mission> missonList;
	
	public void Initialize (int[] msList)
	{
		missonList = new List<Mission> ();
		
		foreach (int each in msList) {
			if(FuBenManagerment.Instance.isCompleteLastMission (each))missonList.Insert (0,MissionInfoManager.Instance .getMissionBySid (each));
		}

		reLoad (missonList.Count);
	}

	public override void updateItem (GameObject item, int index)
	{
		
		if (item == null)
			return;

		WarChooseButton button = item.GetComponent<WarChooseButton> ();
		button.updateButton (missonList [index]);
	}
	
	public override void initButton (int i)
	{
 
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as WarChooseWindow).warChooseBarPrefab);
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		WarChooseButton button = nodeList [i].GetComponent<WarChooseButton> ();
		button.fatherWindow = fatherWindow;
		button.updateButton (missonList [i]);
	}
	public override void jumpToPage (int index)
	{
		base.jumpToPage(index);
		if (GuideManager.Instance.isEqualStep(126004000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
}
