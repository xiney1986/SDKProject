using UnityEngine;
using System.Collections;

public class ContentRuleLadder : dynamicContent {
	
	
	LadderHegeMoney[] prizeInfo;


	public  new void reLoad (int _type)
	{
		cleanAll();

		prizeInfo = LadderAwardSampleManager.Instance.getLadderHegeMoneys (_type);
		
		if (prizeInfo != null && prizeInfo.Length > 0)
		{
			base.reLoad(prizeInfo.Length);
		}
	}
	
	public override void updateItem (GameObject item, int index)
	{
		
		LadderHeItem button=item.GetComponent<LadderHeItem> ();
		button.initData(prizeInfo[index].rangeLevel,prizeInfo[index]);
	}
	
	public override void OnDisable ()
	{
		base.OnDisable ();
	}
	
	public override void initButton (int i)
	{
		
		nodeList[i] = NGUITools.AddChild(gameObject, (fatherWindow as LadderRuleWindow).ruleItem);
		nodeList[i].gameObject.SetActive(true);
		nodeList[i].name = StringKit.intToFixString(i + 1);
		LadderHeItem button = nodeList[i].GetComponent<LadderHeItem>();
		button.initData(prizeInfo[i].rangeLevel,prizeInfo[i]);
	}
}
