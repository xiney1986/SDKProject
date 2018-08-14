using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentInviteCode : dynamicContent {
	
	List<InviteCode> ic;

	public void Initialize (List<InviteCode> _ic)
	{
		ic = _ic;
		base.reLoad (ic.Count);
	}
	
	public void reLoad (List<InviteCode> _ic)
	{
		ic = _ic;
		base.reLoad(ic.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		InviteCodeButton button=item.GetComponent<InviteCodeButton> ();
		button.initUI(ic [index],index);
	}
	
	public override void initButton (int i)
	{
		if(nodeList [i] ==null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as InviteCodeWindow).inviteCodeButtonPrefab);
		}

		nodeList [i].name = StringKit. intToFixString (i + 1);
		InviteCodeButton button=nodeList [i].GetComponent<InviteCodeButton> ();
		button.fatherWindow=fatherWindow;
		button.initUI(ic [i],i);
	}
}
