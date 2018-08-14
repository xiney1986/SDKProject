using UnityEngine;
using System.Collections;

public class ButtonTotalLoginAward : ButtonBase {

	public UITexture Image;
	public UISprite icon_back;
	public GameObject starSoulPoint;
	public GameObject scrapSprite;
    public GameObject stars;
	private PrizeSample prize;

	private Vector3 iconPic = new Vector3 (85, 85, 0);
	
	public override void DoClickEvent () {
		base.DoClickEvent ();

		switch (prize.type) {
		case PrizeType.PRIZE_BEAST:
			Card beast = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show (beast, CardBookWindow.OTHER, null);
			break;
		case PrizeType.PRIZE_CARD:
			Card card = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show (card, CardBookWindow.OTHER, null);
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);	
			UiManager.Instance.openWindow <EquipAttrWindow> ((win) => {
				win.Initialize (equip, EquipAttrWindow.OTHER, null);
			});
			break;
		case PrizeType.PRIZE_MONEY:
			//暂时处理，游戏币也需要显示详情
			MaskWindow.UnlockUI ();
			break;
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.pSid);
			UiManager.Instance.openDialogWindow<PropAttrWindow> ((win) => {
				win.Initialize (prop);
			});
			break;
		case PrizeType.PRIZE_RMB:
			//暂时处理，软妹币也需要显示详情
			MaskWindow.UnlockUI ();
			break;
		case PrizeType.PRIZE_STARSOUL:
			StarSoul star = StarSoulManager.Instance.createStarSoul (prize.pSid);
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> ((win) => {
				win.Initialize (star, StarSoulAttrWindow.AttrWindowType.None);
			});
			break;
            case PrizeType.PRIZE_MAGIC_WEAPON://点击后显示神器属性
                MagicWeapon mw=MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
             if (fatherWindow != null && fatherWindow is MissionAwardWindow) {
                 fatherWindow.finishWindow();
             }
             UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                 win.init(mw, MagicWeaponType.FORM_OTHER);
             });
            break;

		}
		
//		if (item == null){
//			MaskWindow.UnlockUI();
//			return;
//		}
//	
//		if (item.GetType () == typeof(Equip)) {
//			UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
//				win.Initialize (item as Equip, EquipAttrWindow.OTHER, null);
//			});
//
//			return;
//		}
//		if (item.GetType () == typeof(Prop)) {
//			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
//				win.Initialize (item as Prop);
//			});
//
//			return;
//		}
//		if (item.GetType () == typeof(Card)) {
//			CardBookWindow.Show(item as Card,CardBookWindow.OTHER,null);
//		}		
	}
	public void cleanData () {
		Image.alpha = 0;
		icon_back.alpha = 0;
		textLabel.alpha = 0;
		prize = null; 
	}

	public void setTextNum (int num) {
		if (textLabel == null)
			return;
		textLabel.alpha = 1;
		textLabel.text = "x" + num;
	}

	public void updateButton (PrizeSample _prize, WindowBase father) {
		fatherWindow = father;
		updateButton (_prize);
	}
    /// <summary>
    /// 显示星星
    /// </summary>
    void showStar(int level) {
        stars.transform.localPosition = new Vector3(0, -35, 0);
        for (int i = 0; i < level; i++) {
            stars.transform.GetChild(i).gameObject.SetActive(true);
        }
        if (level == CardSampleManager.ONESTAR) {
            stars.transform.localPosition = new Vector3(33, -40, 0);
        } else if (level == CardSampleManager.TWOSTAR) {
            stars.transform.localPosition = new Vector3(22, -40, 0);
        } else if (level == CardSampleManager.THREESTAR) {
            stars.transform.localPosition = new Vector3(11, -40, 0);
        } else if (level == CardSampleManager.FOURSTAR) {
            stars.transform.localPosition = new Vector3(0, -40, 0);
        }
    }
	public void updateButton (PrizeSample _prize) {
        if (stars != null) {
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
		if (starSoulPoint != null)
			starSoulPoint.SetActive (false);
		Image.alpha = 1;
		icon_back.alpha = 1;
		prize = _prize;
		
		if (_prize.type == PrizeType.PRIZE_CARD) {		  
			Card showCard = CardManagerment.Instance.createCard (_prize.pSid);
            showStar(CardSampleManager.Instance.getRoleSampleBySid(showCard.sid).sFlagLevel);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showCard.getIconID (), Image);			
			Image.width = (int)iconPic .x;
			Image.height = (int)iconPic.y;
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (showCard.getQualityId ());
			ArrayList list = StorageManagerment.Instance.getNoUseRolesBySid (_prize.pSid);
			scrapSprite.SetActive (false);
			setTextNum (_prize.getPrizeNumByInt ());
			
		}
		else if (_prize.type == PrizeType.PRIZE_EQUIPMENT) {
			Equip showEquip = EquipManagerment.Instance.createEquip (_prize.pSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showEquip.getIconId (), Image);	
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (showEquip.getQualityId ());
			Image.width = (int)iconPic .x;
			Image.height = (int)iconPic.y;
			scrapSprite.SetActive (false);
			setTextNum (_prize.getPrizeNumByInt ());

		}
		else if (_prize.type == PrizeType.PRIZE_PROP) {
			Prop showProp = PropManagerment.Instance.createProp (_prize.pSid);
            if (showProp.isCardScrap() && stars != null) {
                Card card = CardScrapManagerment.Instance.getCardByScrapSid(showProp.sid);//根据卡片碎片id获取对应卡片
                showStar(CardSampleManager.Instance.getRoleSampleBySid(card.sid).sFlagLevel);
            }
            if (showProp.isMagicScrap() && stars != null) {
                MagicWeapon magic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(showProp.sid);
                showStar(MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid).starLevel);
            }
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showProp.getIconId (), Image);	
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (showProp.getQualityId ());
			Image.width = (int)iconPic .x;
			Image.height = (int)iconPic.y;
			if (showProp.isScrap ())
				scrapSprite.SetActive (true);
			else
				scrapSprite.SetActive (false);
			setTextNum (_prize.getPrizeNumByInt ());

		}
		else if (_prize.type == PrizeType.PRIZE_MONEY) {
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, Image);	
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (5);
			Image.width = (int)iconPic .x;
			Image.height = (int)iconPic.y;
			scrapSprite.SetActive (false);
			setTextNum (_prize.getPrizeNumByInt ());
		}
		else if (_prize.type == PrizeType.PRIZE_RMB) {
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, Image);	
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (5);
			Image.width = (int)iconPic .x;
			Image.height = (int)iconPic.y;
			scrapSprite.SetActive (false);
			setTextNum (_prize.getPrizeNumByInt ());
		}
		else if (_prize.type == PrizeType.PRIZE_STARSOUL) {
			StarSoul star = StarSoulManager.Instance.createStarSoul (_prize.pSid);
			UpdateStarSoulView (star);
			scrapSprite.SetActive (false);
			setTextNum (_prize.getPrizeNumByInt ());
		}
        else if (_prize .type==PrizeType.PRIZE_MAGIC_WEAPON)
        {
            MagicWeapon magicweapon = MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
            MagicWeaponSample magic = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(prize.pSid);
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicweapon.getIconId(), Image);
            icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName(magicweapon.getMagicWeaponQuality());
            Image.width = (int)iconPic.x;
            Image.height = (int)iconPic.y;
            scrapSprite.SetActive(false);
            setTextNum(_prize.getPrizeNumByInt());
            if (magic != null)
            {
                if (stars != null)
                    showStar(CardSampleManager.USEDBYCARD);
            }
        }

		
	}
	/**  更新星魂视图 */
	private void UpdateStarSoulView (StarSoul starsoulView) {
		if (starSoulPoint == null)
			return;
		icon_back.spriteName = "iconback_3";
		Image.gameObject.SetActive (false);
		starSoulPoint.SetActive (true);
		if (starSoulPoint.transform.childCount > 0)
			Utils.RemoveAllChild (starSoulPoint.transform);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.STARSOUL_ICONPREFAB_PATH + starsoulView.getIconId (), starSoulPoint.transform, (obj) => {
			GameObject gameObj = obj as GameObject;
			if(gameObj!=null) {
				Transform childTrans = gameObj.transform;
				if (childTrans != null) {
					StarSoulEffectCtrl effectCtrl = childTrans.gameObject.GetComponent<StarSoulEffectCtrl> ();
					effectCtrl.setColor (starsoulView.getQualityId ());
				}
			}
		});
	}
}
