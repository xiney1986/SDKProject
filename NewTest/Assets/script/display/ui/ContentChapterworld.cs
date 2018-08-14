using UnityEngine;
using System.Collections;

public class ContentChapterworld : ContentBase
{
	public override void CreateButton (int index, GameObject page, int buttonIndex)
	{
		print (page.name);
		ButtonBase button = page.GetComponent<ButtonBase> ();
		int num = (StringKit.toInt (page.name) - 1);
		num = num < 0 ? 0 : num;
		int sid =   FuBenManagerment.getAllShowStoryChapter (1)[num]; 
		button.textLabel.text = ChapterSampleManager.Instance.getChapterSampleBySid (sid).name;
	}
	
	public override void initAllButton (GameObject each)
	{
//		print (each.name);
		base.initAllButton (each);
		ButtonBase button = each.GetComponent<ButtonBase> ();
		int num = (StringKit.toInt (each.name) - 1);
		num = num < 0 ? 0 : num;
		int sid =   FuBenManagerment.getAllShowStoryChapter (1)[num];
		button.textLabel.text = ChapterSampleManager.Instance.getChapterSampleBySid (sid).name;
	}
	
	public override void updateActive (GameObject obj, int pageNUm)
	{
		base.updateActive (obj, pageNUm);
		/* 
		if (pageNUm == 1) {
			if(pageNUm >= maxPage) {
				(fatherWindow as ChapterSelectWindow).arrowLeft.gameObject.SetActive (false);
				(fatherWindow as ChapterSelectWindow).arrowRight.gameObject.SetActive (false);
			}
			else {
				(fatherWindow as ChapterSelectWindow).arrowLeft.gameObject.SetActive (false);
				(fatherWindow as ChapterSelectWindow).arrowRight.gameObject.SetActive (true);
			}
		} else if (pageNUm >= maxPage) {
			(fatherWindow as ChapterSelectWindow).arrowLeft.gameObject.SetActive (true);
			(fatherWindow as ChapterSelectWindow).arrowRight.gameObject.SetActive (false);	
		} else {
			(fatherWindow as ChapterSelectWindow).arrowLeft.gameObject.SetActive (true);
			(fatherWindow as ChapterSelectWindow).arrowRight.gameObject.SetActive (true);		
		}
		(fatherWindow as ChapterSelectWindow).changeScene ( FuBenManagerment.getAllShowStoryChapter (1) [pageNUm - 1]);
		*/
	}


}
