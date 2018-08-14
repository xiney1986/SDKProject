using UnityEngine;
using System.Collections;

public class ContentLaddersAwards : dynamicContent {
	
	protected LaddersAwardSample[] awards;
	private UIScrollView scrollView;
	public void init ()
	{
		scrollView=GetComponent<UIScrollView>();
		awards=LaddersConfigManager.Instance.config_Award.M_getAwards().ToArray();
		if(awards != null)
			base.reLoad(awards.Length);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		Ladders_AwardItem button=item.GetComponent<Ladders_AwardItem> ();
		button.M_update(awards[index]);
	}

	public override void initButton (int i)
	{
		nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as LaddersAwardWindow).prefab_awardItem);
		nodeList [i].SetActive(true);
		nodeList [i].name = StringKit. intToFixString (i + 1);
		Ladders_AwardItem button=nodeList [i].GetComponent<Ladders_AwardItem> ();
		button.parentScrollView=scrollView;
		button.M_update(awards[i]);

	}
}
