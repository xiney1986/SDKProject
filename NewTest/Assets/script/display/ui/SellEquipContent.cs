using UnityEngine;
using System.Collections;

public class SellEquipContent : dynamicContent
{
	public SellWindow window;
	ArrayList equips;
	 
	public void Initialize (ArrayList _equips)
	{
		equips = _equips;
		base.reLoad (equips.Count); 
	}
	public void reLoad(ArrayList _equips)
	{
		equips = _equips;
		base.reLoad(equips.Count);
	}
	

	public override void OnDisable ()
	{
		base.OnDisable ();
		cleanAll();
	}

	public override void updateItem (GameObject item, int index)
	{
		//	base.updateItem (item, index);
		GoodsView button = item.GetComponent<GoodsView> ();
		button.init (equips [index] as Equip); 
		button.fatherWindow = window;
		button.rightBottomText.gameObject.SetActive (false);
		button.onClickCallback = () => {
			OnButtonClick(button);
		};
		button.tempGameObj.SetActive(window.isSelect(button.equip));
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as SellWindow).equipButtonPrefab);
			GoodsView view = nodeList [i].GetComponent<GoodsView> ();
			view.init (equips [i] as Equip);
			view.fatherWindow = window;
			view.rightBottomText.gameObject.SetActive (false);
			view.onClickCallback = () => {
				OnButtonClick(view);
			};
		
			if (view.tempGameObj == null) {
				UISprite us = NGUITools.AddChild<UISprite>(view.gameObject);
				us.depth = 500;
				us.atlas = view.rightBottomSprite.atlas;
				us.spriteName = "gou_3";
				us.MakePixelPerfect();
				us.gameObject.SetActive(window.isSelect(view.equip));
				
				view.tempGameObj = us.gameObject;
			}
		}
	}
	
	void OnButtonClick(GoodsView view)
	{
		if (window.isSelect (view.equip)) {
			window.offSelectEquip(view.equip);
			view.tempGameObj.SetActive(false);
		} else {
			window.onSelectEquip(view.equip);
			view.tempGameObj.SetActive(true);
		}
		MaskWindow.UnlockUI ();
	}
}
