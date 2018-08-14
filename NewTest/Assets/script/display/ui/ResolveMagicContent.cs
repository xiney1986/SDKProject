using UnityEngine;
using System.Collections;

/// <summary>
/// 秘宝晶炼
/// </summary>
public class ResolveMagicContent : dynamicContent {
	public ResolveWindow window;
	public ResolveCardChooseWindow chooseWindow;
	ArrayList equips;
	 
	public void Initialize (ArrayList magic, ResolveWindow _resolveWin, ResolveCardChooseWindow _cardChooseWin) {
        equips = magic;
		window = _resolveWin;
		chooseWindow = _cardChooseWin;
        base.reLoad (equips.Count); 
	}
	public void reLoad (ArrayList _equips) {
		equips = _equips;
		base.reLoad (equips.Count);
	}
	public override void updateItem (GameObject item, int index) {
		//	base.updateItem (item, index);
		GoodsView button = item.GetComponent<GoodsView> ();
		button.init (equips [index] as MagicWeapon); 
		button.fatherWindow = chooseWindow;
		button.onClickCallback = () => {
			OnButtonClick (button);
		};
		button.tempGameObj.SetActive (window.isSelect (button.magicWeapon));
	}
	public override void initButton (int  i) {
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as ResolveCardChooseWindow).equipButtonPrefab);
		}
		nodeList [i].name = StringKit.intToFixString (i + 1);
		GoodsView view = nodeList [i].GetComponent<GoodsView> ();
		view.transform.localScale = new Vector3 (1.1f, 1.1f, 1);
        view.init(equips[i] as MagicWeapon);
		view.fatherWindow = chooseWindow;
		view.onClickCallback = () => {
			OnButtonClick (view);
		};
		if (view.tempGameObj == null) {
			UISprite us = NGUITools.AddChild<UISprite> (view.gameObject);
			us.depth = 300;
			us.atlas = view.rightBottomSprite.atlas;
			us.spriteName = "gou_3";
			us.MakePixelPerfect ();
            us.gameObject.SetActive(window.isSelect(view.magicWeapon));
			
			view.tempGameObj = us.gameObject;
		}
	}
	void OnButtonClick (GoodsView view) {
        if (window.isSelect(view.magicWeapon)) {
            window.offSelectmagic(view.magicWeapon);
			view.tempGameObj.SetActive (false);
		} else if (window.selectMagicList.Count + window.selectedCardList.Count+window.selectedEquipList.Count < 8) {
            window.onSelectMagic(view.magicWeapon);
			view.tempGameObj.SetActive (true);
        } else if (window.selectMagicList.Count + window.selectedCardList.Count + window.selectedEquipList.Count >= 8) {
			TextTipWindow.ShowNotUnlock (Language("resolveChooseMax_2"));
		}
		MaskWindow.UnlockUI ();
	}
}