using UnityEngine;
using System.Collections;

//物品属性窗口
//李程
public class PropAttrWindow : WindowBase
{
	
	public Prop chooseProp;
	public PrizeSample prize;
	public UILabel propName;
	public UILabel propHaveNumber;
	public UITexture propImage;
	public UILabel  propDescript;
	public UISprite quality;
	public UISprite scrapIcon;
    public GameObject starPrefab;
	//public const int TEMPSTORE = 2;//临时仓库
	private Vector3 arrowPosition;
    private MessageHandle msg;
    private CallBackMsg callback;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void setPosition(Vector3 v3)
	{
		arrowPosition = v3;
	}
	public void Initialize (PrizeSample _prize)
	{
		this.prize = _prize;
		if(prize != null){
			ResourcesManager.Instance.LoadAssetBundleTexture(prize.getIconPath(),propImage);
			propName.text = prize.getPrizeName() + "";
            if (prize.type == PrizeType.PRIZE_MERIT) {
                propHaveNumber.text=LanguageConfigManager.Instance.getLanguage("intensifyEquip04")+UserManager.Instance.self.merit.ToString();
            } else {
                Prop pp =StorageManagerment.Instance.getProp(prize.pSid);
                if (pp == null) {
                    propHaveNumber.text = LanguageConfigManager.Instance.getLanguage("intensifyEquip04") + "0";
                } else {
                    propHaveNumber.text = LanguageConfigManager.Instance.getLanguage("intensifyEquip04") + pp.getNum().ToString();
                }
            }
			
			propDescript.text = prize.getPrizeDes();
			quality.spriteName = QualityManagerment.qualityIDToIconSpriteName  (prize.getQuality());
			if (prize.type == PrizeType.PRIZE_PROP) {
				Prop propTemp = PropManagerment.Instance.createProp(prize.pSid);
                if (propTemp.isCardScrap() && starPrefab != null) {
                    Card card = CardScrapManagerment.Instance.getCardByScrapSid(propTemp.sid);//根据卡片碎片id获取对应卡片
                    showStar(card);
                }
				if (scrapIcon != null) {
					if (propTemp.isScrap()) {
						scrapIcon.gameObject.SetActive (true);
					} else {
						scrapIcon.gameObject.SetActive (false);
					}
				}
			}
		}
	}
	public void Initialize (Prop chooseItem )
	{ 
		chooseProp = chooseItem; 
		if(chooseProp != null){
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + chooseProp.getIconId (), propImage);
			propName.text = QualityManagerment.getQualityColor(chooseProp.getQualityId()) + chooseProp .getName () + "";				
			Prop pp=StorageManagerment.Instance.getProp(chooseItem.sid);
            if (chooseItem.isCardScrap() && starPrefab != null) {
                Card card = CardScrapManagerment.Instance.getCardByScrapSid(chooseItem.sid);//根据卡片碎片id获取对应卡片
                showStar(card);
            }
			if(pp==null){
				propHaveNumber.text=LanguageConfigManager.Instance.getLanguage("intensifyEquip04")+"0";
			}else{
				propHaveNumber.text=LanguageConfigManager.Instance.getLanguage("intensifyEquip04")+pp.getNum().ToString();
			}
			propDescript.text = chooseProp.getDescribe ();
			quality.spriteName = QualityManagerment.qualityIDToIconSpriteName  (chooseProp.getQualityId ());
			if (scrapIcon != null) {
				if (chooseProp.isScrap()) {
					scrapIcon.gameObject.SetActive (true);
				} else {
					scrapIcon.gameObject.SetActive (false);
				}
			}
		}

	}
    /// <summary>
    /// 显示星星
    /// </summary>
    void showStar(Card showCard) {
        starPrefab.transform.localPosition = new Vector3(0, -35, 0);
        int cardStarLevel = CardSampleManager.Instance.getStarLevel(showCard.sid);//卡片星级
        for (int i = 0; i < cardStarLevel; i++) {
            starPrefab.transform.GetChild(i).gameObject.SetActive(true);
        }
        if (cardStarLevel == CardSampleManager.ONESTAR) {
            starPrefab.transform.localPosition = new Vector3(33, -40, 0);
        } else if (cardStarLevel == CardSampleManager.TWOSTAR) {
            starPrefab.transform.localPosition = new Vector3(22, -40, 0);
        } else if (cardStarLevel == CardSampleManager.THREESTAR) {
            starPrefab.transform.localPosition = new Vector3(11, -40, 0);
        } else if (cardStarLevel == CardSampleManager.FOURSTAR) {
            starPrefab.transform.localPosition = new Vector3(0, -40, 0);
        }
    }
	public override void DoDisable ()
	{
		base.DoDisable ();
//		if(GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID5 || GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID17 || GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID18)
//			GuideManager.Instance.guideEvent();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonOk") {
			finishWindow ();
		}
		else if (gameObj.name == "imageButton") {
			if (chooseProp != null && chooseProp.isScrap () && prize == null){
				if (chooseProp.isCardScrap ()) {
					Card tmpCard = CardScrapManagerment.Instance.getCardByScrapSid (chooseProp.sid);
					if (tmpCard != null) {
						CardBookWindow.Show (tmpCard,CardBookWindow.OTHER,null);
						closeWin ();
					} else {
						MaskWindow.UnlockUI ();
					}
                } else if (chooseProp.getType() == PropType.PROP_MAGIC_SCRAP) {
                    MagicWeapon tempMagic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(chooseProp.sid);
                    if (tempMagic != null) {
                        UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                            win.init(tempMagic, MagicWeaponType.FORM_OTHER);
                        });
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                } else {
                    Equip tempEquip = EquipScrapManagerment.Instance.getEquipByScrapSid(chooseProp.sid);
                    if (tempEquip != null) {
                        UiManager.Instance.openWindow<EquipAttrWindow>((win) => {
                            win.Initialize(tempEquip, EquipAttrWindow.OTHER, null);
                        });
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                }
			}
			else if (chooseProp == null && prize != null && prize.type == PrizeType.PRIZE_PROP){
				Prop propTemp = PropManagerment.Instance.createProp(prize.pSid);
				if (propTemp.isCardScrap ()) {
					Card tmpCard = CardScrapManagerment.Instance.getCardByScrapSid (propTemp.sid);
					if (tmpCard != null) {
						CardBookWindow.Show (tmpCard,CardBookWindow.OTHER,null);
						closeWin ();
					} else {
						MaskWindow.UnlockUI ();
					}
                } else if (prize.type == PropType.PROP_MAGIC_SCRAP) {
                    MagicWeapon tempMagic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(chooseProp.sid);
                    if (tempMagic != null) {
                        UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                            win.init(tempMagic, MagicWeaponType.FORM_OTHER);
                        });
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                } else if (propTemp.isEquipScrap()) {
					Equip tempEquip = EquipScrapManagerment.Instance.getEquipByScrapSid (propTemp.sid);
					if (tempEquip != null) {
						UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
							win.Initialize (tempEquip, EquipAttrWindow.OTHER, null);
						});
						closeWin ();
					} else {
						MaskWindow.UnlockUI ();
					}
				} else {
					MaskWindow.UnlockUI ();
				}
			} else {
				MaskWindow.UnlockUI ();
			}
		}
	}

	void closeWin ()
	{
		if (UiManager.Instance.getWindow<AllAwardViewWindow> () != null && UiManager.Instance.getWindow<AllAwardViewWindow> ().gameObject.activeSelf) {
			UiManager.Instance.getWindow<AllAwardViewWindow> ().finishWindow ();
		}
		if (UiManager.Instance.getWindow<WarAwardWindow> () != null) {
			UiManager.Instance.getWindow<WarAwardWindow> ().finishWindow ();
		}
		finishWindow ();
	}
}
