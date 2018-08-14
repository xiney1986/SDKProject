using UnityEngine;
using System.Collections;

public class ContentActivity : dynamicContent
{
	ActivityChapter[] chapterList;
    ActivityChapter chapter;
	private WindowBase fatherw;

	public void	reLoad ()
	{
		chapterList = FuBenManagerment.Instance.getSortByTimeChapter ();
		 

		if (chapterList == null || chapterList.Length <= 0)
			return; 
		base.reLoad (chapterList.Length);
	}
    public void reload(int sid) {
        chapter = FuBenManagerment.Instance.getActivityChapterBySid(sid);
        chapterList[0] = chapter;
        base.reLoad(1);
    }
	
	public override void  updateItem (GameObject item, int index)
	{
		ButtonActivityChapter button = item.GetComponent<ButtonActivityChapter> ();	
		button.buyButton.fatherWindow=fatherWindow;
		button.updateActive (chapterList [index]);
		
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as ActivityChapterWindow).activityChapterButtonPrefab);
		}
			
 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonActivityChapter button = nodeList [i].GetComponent<ButtonActivityChapter> ();
		button.fatherWindow = fatherWindow;
		button.buyButton.fatherWindow=fatherWindow;
		button.updateActive (chapterList [i]);

	}
}
