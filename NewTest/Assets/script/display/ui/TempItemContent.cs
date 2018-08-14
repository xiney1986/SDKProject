using UnityEngine;
using System.Collections;

public class TempItemContent : dynamicContent 
{
	ArrayList temps;
	 
	public void Initialize (ArrayList _temps)
	{
		temps = _temps;
		base.reLoad (temps.Count); 
	}
	public void reLoad(ArrayList _temps)
	{
		temps = _temps;
		base.reLoad(temps.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//base.updateItem (item, index);
		
		ButtonTempProp button = item.GetComponent<ButtonTempProp> ();
		button.updateButton (temps [index] as TempProp); 
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as MailWindow).templtemPrefab);
		}

		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonTempProp button = nodeList [i].GetComponent<ButtonTempProp> ();
		button.fatherWindow = fatherWindow; 
		button.initialize (temps [i] as TempProp);
	}
	
}
