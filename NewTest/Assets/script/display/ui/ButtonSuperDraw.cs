using UnityEngine;
using System.Collections;

public class ButtonSuperDraw : ButtonBase
{
	public UISprite back;
	public UISprite select;
	public UITexture icon;
	public UILabel num;
	public UILabel value;
	public GameObject spObj;
	public PrizeSample prize;
	public int drawNum = 0;

	public void clearDate()
	{
		drawNum =0;
		value.text = "";
		value.gameObject.SetActive(false);
	}

	public void initInfo(PrizeSample prize)
	{
		this.prize = prize;
		setCreatButton(prize);
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (prize != null) {
			clickButton (prize);
		}
	}

	private void clickButton(PrizeSample prize)
	{
		switch (prize.type) {
		case PrizeType.PRIZE_BEAST:
			Card beast = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show (beast, CardBookWindow.OTHER, null);
			if (fatherWindow != null && (fatherWindow is AllAwardViewWindow || fatherWindow is WarAwardWindow)) {
				fatherWindow.finishWindow();
			}
			break;
		case PrizeType.PRIZE_CARD:
			Card card = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show (card, CardBookWindow.OTHER, null);
			if (fatherWindow != null && (fatherWindow is AllAwardViewWindow || fatherWindow is WarAwardWindow)) {
				fatherWindow.finishWindow();
			}
			break;
		case PrizeType.PRIZE_MOUNT:
			UiManager.Instance.openWindow<MountShowWindow>((win) => {
				win.init(prize.pSid, MountStoreItem.IS_CAN_UNACTIVE);
			});
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
			UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
				win.Initialize (equip, EquipAttrWindow.OTHER, null);
			});
			if (fatherWindow != null && (fatherWindow is AllAwardViewWindow || fatherWindow is WarAwardWindow)) {
				fatherWindow.finishWindow();
			}
			break;
		case PrizeType.PRIZE_STARSOUL:
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (prize.pSid);
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> (
				(win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
			break;
		case PrizeType.PRIZE_STARSOUL_DEBRIS:
			//暂时处理，星魂碎片
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_MONEY:
			//暂时处理，有可能游戏币也显示详情
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.pSid);
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
			break;
		case PrizeType.PRIZE_RMB:
			//暂时处理，有可能软妹币也显示详情
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_PRESTIGE:
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_MAGIC_WEAPON:
			MagicWeapon mw=MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
			UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
				win.init(mw, MagicWeaponType.FORM_OTHER);
			});
			break;
		default:
			MaskWindow.UnlockUI();
			break;
		}
	}

	public PrizeSample getPrize()
	{
		return prize;
	}

	//设置创建按钮信息
	private void setCreatButton (PrizeSample _prize)
	{
		if(_prize == null)
		{
			return;
		}
		else
		{
			prize = _prize;
			icon.gameObject.SetActive(false);
			back.spriteName = QualityManagerment.qualityIDToIconSpriteName(_prize.getQuality());
			spObj.SetActive(false);
			switch (prize.type) {
			case PrizeType.PRIZE_MONEY:
				ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, icon);
				num.text = "x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_PROP:
				Prop prop = PropManagerment.Instance.createProp(prize.pSid);
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), icon);
				if (prop.isCardScrap() || prop.isEquipScrap() || prop.isScrap()) spObj.SetActive(true);
				num.text ="x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_RMB:
				ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, icon);
				num.text ="x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_CARD:
				Card card = CardManagerment.Instance.createCard (prize.pSid);
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + card.getIconID(), icon);
				num.text ="x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_EQUIPMENT:
				Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId(), icon);
				num.text ="x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_STARSOUL:
				StarSoul starsoulView = StarSoulManager.Instance.createStarSoul(prize.pSid);
				Utils.DestoryChilds(icon.gameObject);
				ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.STARSOUL_ICONPREFAB_PATH + starsoulView.getIconId(), icon.transform, (obj) => {
					GameObject gameObj = obj as GameObject;
					if (gameObj != null) {
						Transform childTrans = gameObj.transform;
						childTrans.localScale = new Vector3(0.7f,0.7f,1);
						if (childTrans != null) {
							StarSoulEffectCtrl _effectCtrl = childTrans.gameObject.GetComponent<StarSoulEffectCtrl>();
							_effectCtrl.setColor(starsoulView.getQualityId());
						}
					}
				});
				num.text ="x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_MAGIC_WEAPON:
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(prize.pSid).iconId, icon);
				num.text ="x"+prize.num.ToString();
				break;
			}
			icon.gameObject.SetActive(true);
		}
	}
}
