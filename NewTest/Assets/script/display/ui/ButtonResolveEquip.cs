using UnityEngine;
using System.Collections;
 
public class ButtonResolveEquip : ButtonBase
{
	public UITexture equipImage;
	public Equip equip;//关联的card对象
	public UILabel level;//等级不解释
	public UISprite selectPic;//选中标志
	public UISprite qualityBg;//品质背景
	public bool isChoose;//是否选中;
	public ResolveWindow win;

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
		UiManager.Instance.openWindow<ResolveWindow> ();
	}

	public  void initialize (Equip _equip)
	{
		win = fatherWindow as ResolveWindow;
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
		selectPic.gameObject.SetActive (true);
	}

	void cleanShower ()
	{
		selectPic.gameObject.SetActive (false);
	}

	void cleanButton ()
	{
		equipImage.alpha = 0;
		equipImage.mainTexture = null;
		level.text = "";
		equip = null;
		selectPic.gameObject.SetActive (false);
	 
	}

	void resetCard ()
	{
		equipImage.alpha = 1;
		equipImage.color = Color.white;
	}

	public void updateButton (Equip newEquip)
	{
		if (newEquip == null) {
			cleanButton ();
			return;
		} else {
			equip = newEquip;
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), equipImage);
			level.text = equip.getLevel ().ToString ();
			qualityBg.spriteName = QualityManagerment.qualityIDToIconSpriteName  (equip.getQualityId ());
			resetCard ();
			if (!win.isSelect (equip)) {
				isChoose = false;
				selectPic.gameObject.SetActive (false);
			} else {
				isChoose = true;
				selectPic.gameObject.SetActive (true);
			}
		}
	}
}
