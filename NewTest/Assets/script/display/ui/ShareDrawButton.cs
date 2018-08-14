using UnityEngine;
using System.Collections;

public class ShareDrawButton : ButtonBase
{
	public UISprite back;
    public GameObject customIconPoint;
	public UISprite select;
	public UITexture icon;
    public UISprite spriteScrap;
	public UILabel num;
	public UILabel value;
	public PrizeSample prize;
	private int drawNum = 0;
    Card card;
    Prop prop;
    Equip equip;
    StarSoul starSoul;
    MagicWeapon magicWeapon;
    int count;

	public void clearDate()
	{
		drawNum =0;
		value.text = "";
		select.enabled = false;
		value.gameObject.SetActive(false);
//		GetComponent<BoxCollider>().enabled = false;
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
        if (equip != null) {
            UiManager.Instance.openWindow<EquipAttrWindow>(
                (winEquip) => {
                    winEquip.Initialize(equip, EquipAttrWindow.OTHER, null);
                });
        } else if (starSoul != null) {
            UiManager.Instance.openDialogWindow<StarSoulAttrWindow>(
                (win) => {
                    win.Initialize(starSoul, StarSoulAttrWindow.AttrWindowType.None);
                });
        } else if (prop != null) {
            UiManager.Instance.openDialogWindow<PropAttrWindow>(
                (winProp) => {
                    winProp.Initialize(prop);
                });
        } else if (card != null) {
            CardBookWindow.Show(card, CardBookWindow.SHOW, null);
        } else if (magicWeapon != null) {
            UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                win.init(magicWeapon, MagicWeaponType.FORM_OTHER);
            });
        } else {
            switch (prize.type) {
                case PrizeType.PRIZE_MONEY:
                    break;
                case PrizeType.PRIZE_RMB:
                    break;
            }
        }
		MaskWindow.UnlockUI();
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
            count = StringKit.toInt(prize.num);
            if (prize.type == PrizeType.PRIZE_CARD) {
                card = CardManagerment.Instance.createCard(prize.pSid);
            } else if (prize.type == PrizeType.PRIZE_EQUIPMENT) {
                equip = EquipManagerment.Instance.createEquip(prize.pSid);
            } else if (prize.type == PrizeType.PRIZE_MAGIC_WEAPON) {
                magicWeapon = MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
            } else if (prize.type == PrizeType.PRIZE_PROP) {
                prop = PropManagerment.Instance.createProp(prize.pSid);
            } else if (prize.type == PrizeType.PRIZE_STARSOUL) {
                starSoul = StarSoulManager.Instance.createStarSoul(prize.pSid);
            }
            updateInfo();
		}
	}
    private void updateInfo() {
        if (equip != null) {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + equip.getIconId(), icon);
            if (spriteScrap != null)
                spriteScrap.spriteName = "sign_scrap";
            if (count / 1000000 > 0)
                num.text = "x" + count / 10000 + "W";
            else
                num.text = "x" + count;
        } else if (starSoul != null) {
            UpdateStarSoulView(starSoul);
        } else if (magicWeapon != null) {
            UpdateMagicWeapon();
        } else if (prop != null) {
            if (spriteScrap != null) {
                if (prop.isScrap()) {
                    spriteScrap.gameObject.SetActive(true);
                    spriteScrap.spriteName = "sign_scrap";
                } else {
                    spriteScrap.gameObject.SetActive(false);
                }
            }
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + prop.getIconId(), icon);
            if (count / 1000000 > 0)
                num.text = "x" + count / 10000 + "W";
            else
                num.text = "x" + count;
        } else if (card != null) {
            if (card.sid <= 10)
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.MAINCARD_ICONIMAGEPATH + card.getMainCardImageIDBysid(card.sid).ToString(), icon);
            else
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + card.getIconID().ToString(), icon);
            if (spriteScrap != null)
                spriteScrap.spriteName = "sign_scrap";
            if (count == 0)
                count = 1;
            num.text = "x" + count;
        } else if (prize != null) {
            ResourcesManager.Instance.LoadAssetBundleTexture(prize.getIconPath(), icon);
            if (spriteScrap != null)
                spriteScrap.spriteName = "sign_scrap";
            if (prize.type == PrizeType.PRIZE_MONEY) spriteScrap.gameObject.SetActive(false);
            if (count / 1000000 > 0)
                num.text = "x" + count / 10000 + "W";
            else
                num.text = "x" + count;
        }
        icon.gameObject.SetActive(true);
    }
    private void UpdateStarSoulView(StarSoul starsoulView) {
        if (customIconPoint == null)
            return;
        back.spriteName = "iconback_3";
        icon.gameObject.SetActive(false);
        customIconPoint.SetActive(true);
        if (customIconPoint.transform.childCount > 0)
            Utils.RemoveAllChild(customIconPoint.transform);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.STARSOUL_ICONPREFAB_PATH + starsoulView.getIconId(), customIconPoint.transform, (obj) => {
            GameObject gameObj = obj as GameObject;
            if (gameObj != null) {
                Transform childTrans = gameObj.transform;
                if (childTrans != null) {
                    StarSoulEffectCtrl effectCtrl = childTrans.gameObject.GetComponent<StarSoulEffectCtrl>();
                    effectCtrl.setColor(starsoulView.getQualityId());
                }
            }
        });
        num.text = "x" + count;
    }
    /**更新秘宝的基本信息 */
    private void UpdateMagicWeapon() {
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicWeapon.getIconId(), icon);
        back.spriteName = QualityManagerment.qualityIDtoMagicWeapon(magicWeapon.getMagicWeaponQuality());
        icon.SetRect(0f, 0f, 80, 80);
        icon.transform.localPosition = new Vector3(0f, 6f, 1f);
        if (count / 1000000 > 0)
            num.text = "x" + count / 10000 + "W";
        else
            num.text = "x" + count;
    }
}
