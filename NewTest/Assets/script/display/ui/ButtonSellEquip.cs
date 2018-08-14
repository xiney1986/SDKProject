using UnityEngine;
using System.Collections;

public class ButtonSellEquip : ButtonBase
{
	public UITexture equipImage;
	public UISprite bg;
	public Equip equip;//关联的card对象
	public UILabel level;//等级不解释
	public UISprite selectPic;//选中标志
	public bool isChoose;//是否选中;
	public SellWindow win;

	public override void DoLongPass ()
	{
		if (equip == null)
			return;
	
		fatherWindow.finishWindow ();
		UiManager.Instance.openWindow<EquipAttrWindow>((win)=>{
			win.Initialize (equip, EquipAttrWindow.OTHER, openFatherWindow);
		});

	}

	void openFatherWindow ()
	{
		UiManager.Instance.openWindow<SellWindow> ();
	}

	public  void initialize (Equip _equip)
	{
		win = fatherWindow as SellWindow;
		updateButton (_equip);
	}

	public  void changeBlack ()
	{
		equipImage.color = Color.gray;
	}

	public  void changeLight ()
	{
		equipImage.color = Color.white;
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (isChoose == false) {
			win.onSelectEquip (equip);
			selectOn ();
		} else {
			win.offSelectEquip (equip);
			selectOff ();
		}
		
		win.changeButton ();
	}

	void selectOn ()
	{
		isChoose = true;
		setShower ();
	}

	void selectOff ()
	{
		isChoose = false;
		cleanShower ();
	}

	void setShower ()
	{
		selectPic.alpha = 1;
	}

	void cleanShower ()
	{
		selectPic.alpha = 0;
	}

	void cleanButton ()
	{
		equipImage.alpha = 0;
		equipImage.mainTexture = null;
		level.text = "";
		equip = null;
		selectPic.alpha = 0;
	
	}

	void resetCard ()
	{
		equipImage.alpha = 1;
		equipImage.color = Color.white;
		selectPic.alpha = 0;
	}

	public void updateButton (Equip newEquip)
	{
		if (newEquip == null) {
			cleanButton ();
			return;
		} else {
			equip = newEquip;
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), equipImage);
			level.text = "Lv." + equip.getLevel ().ToString ();
			bg.spriteName = QualityManagerment.qualityIDToIconSpriteName  (equip.getQualityId ());
			resetCard ();
			if (!win.isSelect (equip)) {
				isChoose = false;
				selectPic.alpha = 0;
			} else {
				isChoose = true;
				selectPic.alpha = 1;
			}
			
		}
	}
}
