using UnityEngine;
using System.Collections;

public class ContentLuckyDraw : dynamicContent
{
	LuckyDraw[] drawList;

	public void	reLoad ()
	{ 
		if(!GuideManager.Instance.isGuideComplete())
		{
			drawList = new LuckyDraw[1];
			drawList[0] = new LuckyDraw(GuideGlobal.LUCKY_SID);
				base.reLoad (drawList.Length);
			return;
		}
		LuckyDraw[] arr = LuckyDrawManagerment.Instance.getLuckyDrawArr ();
		if (arr == null || arr.Length <= 0)
			return;
		
		
		//按策划要求排序
		LuckyDraw temp;
		for (int i = 0; i < arr.Length-1; i++) {
			for (int j = 0; j < arr.Length-i-1; j++) {
				if (arr [j].getLuckyIndex () > arr [j + 1].getLuckyIndex ()) {
					temp = arr [j];
					arr [j] = arr [j + 1];
					arr [j + 1] = temp;
				}
			}
		}
		drawList = arr;
		base.reLoad (arr.Length);
	}
	
 	public override void  updateItem (GameObject item, int index)
	{
		ButtonLuckyDraw button = item.GetComponent<ButtonLuckyDraw> ();				 
 		button.updateLuckyDraw (drawList [index]);
		
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as LuckyDrawWindow).luckyDrawBarPrefab);
		}
		ButtonLuckyDraw button = nodeList [i].GetComponent<ButtonLuckyDraw> ();
		button.fatherWindow = fatherWindow;
		button.updateLuckyDraw (drawList [i]);
	}
	public override void jumpToPage(int index)
	{
		base.jumpToPage(index);
		if (GuideManager.Instance.isEqualStep(7003000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
}
