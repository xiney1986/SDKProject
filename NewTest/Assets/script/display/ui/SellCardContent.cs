using UnityEngine;
using System.Collections;

public class SellCardContent : dynamicContent
{
	public SellWindow window;
	ArrayList cards;
	 
	public void Initialize (ArrayList _cards)
	{
		cards = _cards;
		base.reLoad (cards.Count); 
	}
	public void reLoad(ArrayList _cards)
	{
		cards = _cards;
		base.reLoad(cards.Count);
	}
	

	public override void OnDisable ()
	{
		base.OnDisable ();
		cleanAll();
	}

	public override void updateItem (GameObject item, int index)
	{
		RoleView button = item.GetComponent<RoleView> ();
		button.init (cards [index] as Card,window,(roleView)=>{
			OnButtonClick(roleView);
		}); 
		button.tempGameObj.SetActive (window.isSelect(button.card));
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as SellWindow).cardButtonPrefab);

		RoleView view = nodeList [i].GetComponent<RoleView> ();
			view.transform.localScale = new Vector3 (0.9f,0.9f,1);
		view.init (cards [i] as Card, window, (roleView)=>{
			OnButtonClick(roleView);
		});
		
		if (view.tempGameObj == null) {
			UISprite us = NGUITools.AddChild<UISprite>(view.gameObject);
			us.depth = 500;
			us.atlas = view.qualityBg.atlas;
			us.spriteName = "gou_3";
			us.MakePixelPerfect();
			us.transform.localScale = new Vector3(2,2,1);
			us.gameObject.SetActive(window.isSelect(view.card));
			
			view.tempGameObj = us.gameObject;
		}
		}
	}
	
	void OnButtonClick(RoleView view)
	{
		if (window.isSelect (view.card)) {
			window.offSelectCard(view.card);
			view.tempGameObj.SetActive(false);
		} else {
			window.onSelectCard(view.card);
			view.tempGameObj.SetActive(true);
		}
		MaskWindow.UnlockUI ();
	}
}
