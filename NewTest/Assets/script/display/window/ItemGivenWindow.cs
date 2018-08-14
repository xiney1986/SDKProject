using UnityEngine;
using System.Collections;

//副本中物品获得窗口
//李程
public class ItemGivenWindow :WindowBase
{ 
	public UITexture  itemImage;
	public UILabel  itemName;
	public UILabel buttonNext;
	public AwardDisplayCtrl awardCtrl;
    public UISprite qualityBg;
    public GameObject starPerfab;//神器星星

	protected override void begin ()
	{
		base.begin ();
		if(MissionInfoManager.Instance.autoGuaji){
			awardCtrl.openNextWindow ();
		}
		MaskWindow.UnlockUI ();
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		//UiManager.Instance.backGroundWindow.switchToDark();//2014.7.5 modified
	}

	public void Initialize (PropAward item, AwardDisplayCtrl ctrl)
	{
		
		PropSample sample = PropSampleManager .Instance.getPropSampleBySid (item.sid);
        Prop tmp = PropManagerment.Instance.createProp(sample.sid);
        if (tmp.isMagicScrap()) {
            Utils.RemoveAllChild(itemImage.transform);
            MagicWeapon magic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(tmp.sid);
            GameObject obj = NGUITools.AddChild(itemImage.gameObject, starPerfab);
            ShowStars show = obj.GetComponent<ShowStars>();
            show.init(magic, MagicWeaponManagerment.USEDBUMAGIC_AWARD);
        }
		itemName.text = LanguageConfigManager.Instance.getLanguage ("s0058") + sample.name;
		string count;
		if (item.num == 0)
			count = "x1";
		else
			count = "x" + item.num;
		itemName.text += count;

		itemImage.width = 256;
		itemImage.height = 256;
        if(item.sid == 71181){
            itemImage.width = 180;
            itemImage.height = 180;
        }
        qualityBg.spriteName = QualityManagerment.qualityIDToIconSpriteName(sample.qualityId);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + sample.iconId, itemImage);
		Initialize (ctrl);
	}

	public void Initialize (AwardDisplayCtrl ctrl, string str)
	{
		awardCtrl = ctrl;
		if (str == "money") {
			itemName.text = LanguageConfigManager.Instance.getLanguage ("s0050") + " " + (ctrl.activeAward.moneyGap).ToString ();
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, itemImage);
		}
		if (str == "exp") {
			itemName.text = LanguageConfigManager.Instance.getLanguage ("s0051") + " " + (ctrl.activeAward.expGap).ToString ();
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.EXP_ICONPATH, itemImage);

		}
		if (str == "rmb") {
			itemName.text = LanguageConfigManager.Instance.getLanguage ("s0057") + " " + (ctrl.activeAward.rmbGap).ToString ();
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, itemImage);

		}
		Initialize (ctrl);
	}
	
	public void Initialize (EquipAward item, AwardDisplayCtrl ctrl)
	{

		EquipSample sample = EquipmentSampleManager .Instance.getEquipSampleBySid (item.sid);
		itemName.text = LanguageConfigManager.Instance.getLanguage ("s0058") + sample.name;
		string count;
		if (item.num == 0)
			count = "x1";
		else
			count = "x" + item.num;

		itemName.text += count;
		itemImage.width = 256;
		itemImage.height = 256;
        qualityBg.spriteName = QualityManagerment.qualityIDToIconSpriteName(sample.qualityId);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + sample.iconId, itemImage);
		Initialize (ctrl);
	}
	
	public void Initialize (CardAward item, AwardDisplayCtrl ctrl)
	{
	
		CardSample sample = CardSampleManager.Instance.getRoleSampleBySid (item.sid);
		itemName.text = sample.name;

		string count;
		if (item.num == 0)
			count = "x1";
		else
			count = "x" + item.num;

		itemName.text += count;
		itemImage.width = 512;
		itemImage.height = 512;
        qualityBg.spriteName = QualityManagerment.qualityIDToIconSpriteName(sample.qualityId);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + sample.imageID, itemImage);
		Initialize (ctrl);
		 
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		awardCtrl.openNextWindow ();
	}

	public void Initialize (AwardDisplayCtrl ctrl)
	{
		awardCtrl = ctrl;
		if (ctrl.activeAward.type == AwardManagerment .FIRST)
			setTitle (LanguageConfigManager.Instance.getLanguage ("s0059"));
		if (ctrl.activeAward.type == AwardManagerment.MNGV)
			setTitle (LanguageConfigManager.Instance.getLanguage ("s0060"));
	}

	void Update ()
	{
		buttonNext.alpha = sin ();
	}
}
