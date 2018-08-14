using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildBossPrizeContent : SampleDynamicContent
{
	public UISprite leftArrow;
	public UISprite rightArrow; 
	private List<GuildBossPrizeSample> list;
 
	public void 	 initInfo(){
		list=GuildPrizeSampleManager.Instance.getPrizes ();
		maxCount=list.Count;
	}


	public  void updateButton (GameObject page)
	{
		int index=StringKit.toInt(page.name)-1;
		GuildBossPrizeButton button = page.transform.GetChild(0).GetComponent<GuildBossPrizeButton>();
		button.initInfo(list[index]);
	}



	public  void updateActive (GameObject obj)
	{
		int pageNUm=StringKit.toInt(obj.name);

		if(pageNUm == 1)
		{
			leftArrow.gameObject.SetActive(false);
			rightArrow.gameObject.SetActive(true);
		}
		else if(pageNUm == list.Count)
		{
			leftArrow.gameObject.SetActive(true);
			rightArrow.gameObject.SetActive(false);
		}
		else
		{
			leftArrow.gameObject.SetActive(true);
			rightArrow.gameObject.SetActive(true);
		}

	}
}
