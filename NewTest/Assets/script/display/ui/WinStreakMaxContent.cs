using UnityEngine;
using System.Collections;

public class WinStreakMaxContent : dynamicContent
{
	PvpPrizeSample[] samples;
	public GameObject pvpPrizeItemPrefab;
	public void Initialize (PvpPrizeSample[] _samples)
	{
		samples = _samples;
		base.reLoad (samples.Length); 
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//base.updateItem (item, index);
		PvpPrizeModule button = item.GetComponent<PvpPrizeModule> ();
		button.updateSample (samples [index]); 
		
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, pvpPrizeItemPrefab);
		}
 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		PvpPrizeModule button = nodeList [i].GetComponent<PvpPrizeModule> ();
		button.initialize (samples [i],fatherWindow);
	}
}
