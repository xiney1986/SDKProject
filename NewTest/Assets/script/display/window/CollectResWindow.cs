using UnityEngine;
using System.Collections;

public class CollectResWindow : WindowBase
{
	
	public UITexture resImage;
	public UILabel  label;
	public int   equipShowIndex = 0;
	public int  propsShowIndex = 0;
	public int   cardShowIndex = 0;
	//宝箱类型
	public int   treasureType;
	//是否宝箱模式
	public bool   treasureMode;
	public	AwardDisplayCtrl awardCtrl;
	public UILabel close;

	
	void Update ()
	{
		close.alpha = sin();
	}

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	public void setOtherId ()
	{
		
	}
	
	public void Initialize (int _treasureType, AwardDisplayCtrl ctrl)
	{
		awardCtrl = ctrl;
		treasureType = _treasureType;
 
		if (ctrl.award == null) { 
			treasureMode = true;
			resImage.gameObject.SetActive(true);
			if (treasureType == TreasureType.TREASURE_SILVER) {
				ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.TREASURE_SILVER, resImage);
				label.text = LanguageConfigManager.Instance.getLanguage ("s0064");
			}
			if (treasureType == TreasureType.TREASURE_GOLD) {
				ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.TREASURE_GOLD, resImage);
				label.text = LanguageConfigManager.Instance.getLanguage ("s0063");
			}
		} 

	}

	public void Initialize (AwardDisplayCtrl ctrl, string str)
	{
		awardCtrl = ctrl;
		resImage.gameObject.SetActive(true);
		if (str == "money") {
			label.text = LanguageConfigManager.Instance.getLanguage ("s0050") + (ctrl.activeAward.moneyGap);
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, resImage);
		}
		if (str == "exp") {
			label.text = LanguageConfigManager.Instance.getLanguage ("s0051") + (ctrl.activeAward.expGap);
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.EXP_ICONPATH, resImage);
		}
		if (str == "rmb") {
			label.text = LanguageConfigManager.Instance.getLanguage ("s0057") + (ctrl.activeAward.rmbGap );
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, resImage);
		}
 
	}
	
	public void Initialize (PropAward item, AwardDisplayCtrl ctrl)
	{
		label.text = LanguageConfigManager.Instance.getLanguage ("s0065") + item.sid; 
		awardCtrl = ctrl; 
	}
	
	public void Initialize (EquipAward item, AwardDisplayCtrl ctrl)
	{
		label.text = LanguageConfigManager.Instance.getLanguage ("s0058") + item.sid; 
		awardCtrl = ctrl; 
	}
	
	public void Initialize (CardAward item, AwardDisplayCtrl ctrl)
	{
		label.text = LanguageConfigManager.Instance.getLanguage ("s0062") + item.sid; 
		awardCtrl = ctrl; 
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
		awardCtrl.openNextWindow ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		hideWindow ();
	}
	
 
}
