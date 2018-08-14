using UnityEngine;
using System.Collections;

public class MessageWithContentCardContent : dynamicContent
{
	public ResolveWindow window;
	ArrayList cards;
	
	public void Initialize (ArrayList _cards)
	{
		cards = _cards;
		base.reLoad (cards.Count); 
	}
	
	public void reLoad (ArrayList _cards)
	{
		cards = _cards;
		base.reLoad (cards.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		RoleView button = item.GetComponent<RoleView> ();
		button.init (cards [index] as Card, window,null); 
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as MessageWithContentWindow).cardPrefab);
			RoleView view = nodeList [i].GetComponent<RoleView> ();
			view.transform.localScale = new Vector3 (0.98f, 0.98f, 0.98f);
			
			view.init (cards [i] as Card, window,null);
		}
	}

	public void updateAllItems()
	{
		for (int i=0; i<nodeList.Count; i++)
			updateItem (nodeList [i], i);
	}
}
