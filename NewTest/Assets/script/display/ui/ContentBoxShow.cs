using UnityEngine;
using System.Collections;

public class ContentBoxShow : dynamicContent {
	public override void updateItem (GameObject item, int index) {
        BoxShowItem button = item.GetComponent<BoxShowItem>();
        button.BoxButton.fatherWindow = fatherWindow;
        button.fawin = fatherWindow;
		button.updateItem(index); 
	}

	public override void initButton (int  i) 
    {
        if (nodeList[i] == null) {
            nodeList[i] = NGUITools.AddChild(this.gameObject, (fatherWindow as BoxShowWindow).itemButtonPrefab);  
        }
		nodeList [i].name = StringKit.intToFixString (i + 1);
        BoxShowItem button = nodeList[i].GetComponent<BoxShowItem>();
        button.BoxButton.fatherWindow = fatherWindow;
        button.fawin = fatherWindow ;
		button.updateItem (i); 

	}
	void OnDisable () {
		cleanAll ();
	}
}

