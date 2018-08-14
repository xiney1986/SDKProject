using System;
using UnityEngine;
using System.Collections;

/**
 * 装备图标节点按钮 ,应用于equipStore equipChoose 
 * */
public class ButtonStoreEquip : ButtonBase
{ 
	public UILabel buttonName;
	public UILabel state;
    public UISprite isstate;
	public UILabel equipName;
	public UILabel equipLv;
	public UILabel equipAttribute;
	public UILabel equipAttributeValue;
	public UITexture itemIcon;
	public UISprite quality;
	public ButtonStoreResult intensifyButton;
	public Equip equip;
	public UISprite isNew;
 	public UILabel starLevel;
	private int type;
	private const string FIRSTEQUIPNAME = "001";//第一个加载的装备的对象名字

	public override void DoUpdate ()
	{
        if(equip!=null&&equip.equpStarState>0&&!starLevel.gameObject.activeInHierarchy)starLevel.gameObject.SetActive(true);
		if (starLevel != null && starLevel.gameObject != null && starLevel.gameObject.activeSelf)
			starLevel.alpha = sin ();
	}
	public void UpdateEquip (Equip _equip, int type)
	{
		this.type = type;
		equip = _equip;
		if (equip.equpStarState > 0) {
			starLevel.gameObject.SetActive (true);
			starLevel.text = "+" + equip.equpStarState.ToString();
		}
		else {
			starLevel.gameObject.SetActive (false);
		}
		intensifyButton.UpdateEquip (_equip);
		
		if (type == ContentEquipChoose .INTENSIFY) {
			intensifyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0012");
			if (equip.getLevel () >= equip.getMaxLevel ()) {
				intensifyButton.gameObject.SetActive (false);
			} else {
				//新手引导等级指引开始前不开放
				if (GuideManager.Instance.isLessThanStep (124002000)) {
					intensifyButton.gameObject.SetActive (false);
				} else {
					intensifyButton.gameObject.SetActive (true);
				}
			}
		} else if (type == ContentEquipChoose.PUT_ON) {
			intensifyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0013");
		} else if (type == ContentEquipChoose.CHATSHOW) {
			intensifyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0308");
		} else if(type == ContentEquipChoose.FROM_TO_UPSTAR)
		{
			intensifyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("equipStar02");
		}else
			throw new Exception (GetType () + "UpdateEquip type error! type=" + type);
		
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), itemIcon);
		EquipManagerment.Instance.stateIDToString (equip.getState ());
		equipName.text = QualityManagerment.getQualityColor (equip.getQualityId ()) + equip.getName ();
		if(equipLv!=null)
			equipLv.text = "Lv." + equip.getLevel ();
		AttrChange[] attrs = equip .getAttrChanges ();
		if (attrs != null && attrs.Length > 0 && attrs [0] != null) {
			equipAttribute.text = attrs [0].typeToString ();
			if(equipAttributeValue!=null)
				equipAttributeValue.text = "+" + attrs [0].num;
		}
		quality.spriteName = QualityManagerment.qualityIDToIconSpriteName (equip.getQualityId ());
        int isstateId;
        isstateId = EquipManagerment.Instance.stateIDToString(equip.getState());
        //state.text = EquipManagerment.Instance.stateIDToString (equip.getState ());
        if (isstateId == 1)
        {
            isstate.gameObject.SetActive(true);
        }
        else 
        {
            isstate.gameObject.SetActive(false);
        }
        int refinelevel=equip.getrefineLevel();
        if(refinelevel==0)
        {
            state.gameObject.SetActive(false);
        }
        else
        {
            state.gameObject.SetActive(true);
            state.text = LanguageConfigManager.Instance.getLanguage("refine_031",refinelevel.ToString());
        }
		if (equip.isNew) {
			if (isNew != null && !isNew.gameObject.activeSelf)
				isNew.gameObject.SetActive (true);
		} else if (isNew != null && isNew.gameObject.activeSelf) {
			isNew.gameObject.SetActive (false);
		}
	}


	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		starLevel.gameObject.SetActive (false);

		UiManager.Instance.openWindow <EquipAttrWindow> ((win) => {


			if (fatherWindow.GetType () == typeof(StoreWindow)) {
				win.Initialize (equip, EquipAttrWindow.STOREVIEW, null);
			}
			else {
				//其他窗口
				win.Initialize (equip, EquipAttrWindow.OTHER, null);
			}

		});

	}
	
	private void showEquipChooseWindow ()
	{
		UiManager.Instance.openWindow<EquipChooseWindow> ();
	}
}
