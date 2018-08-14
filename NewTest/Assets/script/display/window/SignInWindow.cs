using UnityEngine;
using System.Collections;

//物品属性窗口
//李程
public class SignInWindow : WindowBase
{
	
	public PrizeSample prize;
	public UILabel haveRmb;
    public UILabel needRmb;
    public UILabel signTimes;
	public UILabel propHaveNumber;
	public UITexture propImage;
	public UISprite quality;
	public UISprite scrapIcon;
    public GameObject starPrefab;
    public ButtonBase signInButton;//补签按钮
    public UILabel showName;
    private CallBackMsg callback;
    private MessageHandle msg = new MessageHandle();

	protected override void begin ()
	{
		base.begin ();
        updateUI();
		MaskWindow.UnlockUI ();
	}
    public void updateUI(){
        //GetSignInInfoFport fport = FPortManager.Instance.getFPort("GetSignInInfoFport") as GetSignInInfoFport;
        //fport.getSignInInfo(null);
        int needMoney = CommandConfigManager.Instance.getSignInCost()[SignInManagerment.Instance.sign_inTimes];
        int haveMoney = UserManager.Instance.self.getRMB();
        haveRmb.text = haveMoney +"";
        signTimes.text = LanguageConfigManager.Instance.getLanguage("signInTips5", (SignInManagerment.Instance.sign_inTimes+ 1).ToString());
        needRmb.text = needMoney > haveMoney ? "[FF0000]" + needMoney : needMoney + "";
        if (needMoney > haveMoney) {
            signInButton.disableButton(true);
        } else signInButton.disableButton(false);
    }

    public void Initialize(PrizeSample _prize, CallBackMsg callback)
	{
		this.prize = _prize;
        this.callback = callback;
        if (prize != null) {
            showName.text = QualityManagerment.getQualityColor(prize.getQuality()) +  prize.getPrizeName();
            quality.spriteName = QualityManagerment.qualityIDToIconSpriteName(prize.getQuality());
			ResourcesManager.Instance.LoadAssetBundleTexture(prize.getIconPath(),propImage);
            if (prize.type == PrizeType.PRIZE_MERIT) {
                propHaveNumber.text=LanguageConfigManager.Instance.getLanguage("intensifyEquip04")+UserManager.Instance.self.merit.ToString();
            } else if (prize.type == PrizeType.PRIZE_STARSOUL) {
                StarSoul starSoul = StarSoulManager.Instance.createStarSoul(prize.pSid);
                if (starSoul == null) return;
                propImage.mainTexture = null;
                quality.spriteName = "iconback_3";
                showName.text = QualityManagerment.getQualityColor(starSoul.getQualityId()) + starSoul.getName();
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.STARSOUL_ICONPREFAB_PATH + starSoul.getIconId(), propImage.gameObject.transform, (obj) => {
                    GameObject gameObj = obj as GameObject;
                    if (gameObj != null) {
                        Transform childTrans = gameObj.transform;
                        if (childTrans != null) {
                            StarSoulEffectCtrl effectCtrl = childTrans.gameObject.GetComponent<StarSoulEffectCtrl>();
                            effectCtrl.setColor(starSoul.getQualityId());
                        }
                    }
                });
            } else if(prize.type == PrizeType.PRIZE_PROP){
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + PropManagerment.Instance.createProp(prize.pSid).getIconId(), propImage);
                Prop pp = StorageManagerment.Instance.getProp(prize.pSid);
                if (pp == null) {
                    propHaveNumber.text = LanguageConfigManager.Instance.getLanguage("intensifyEquip04") + "0";
                } else {
                    propHaveNumber.text = LanguageConfigManager.Instance.getLanguage("intensifyEquip04") + pp.getNum().ToString();
                }
            }
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
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonOk") {
            msg.msgEvent = msg_event.dialogOK;
            callback(msg);
			finishWindow ();
        } else if (gameObj.name == "buttonCancel") {
            msg.msgEvent = msg_event.dialogCancel;
            finishWindow();
        } else if (gameObj.name == "imageButton") {
            if (prize.type == PrizeType.PRIZE_STARSOUL) {
                StarSoul starSoul = StarSoulManager.Instance.createStarSoul(prize.pSid);
                if (starSoul == null) {
                    MaskWindow.UnlockUI();
                    return;
                }
                UiManager.Instance.openDialogWindow<StarSoulAttrWindow>((win) => {
                    win.Initialize(starSoul, StarSoulAttrWindow.AttrWindowType.None);
                });
                finishWindow();
            } else if (prize.type == PrizeType.PRIZE_CARD) {
                Card tmpCard = CardManagerment.Instance.createCard(prize.pSid);
                if (tmpCard == null) {
                    MaskWindow.UnlockUI();
                    return;
                }
                UiManager.Instance.openWindow<CardBookWindow>((win) => {
                    win.init(tmpCard, CardBookWindow.SHOW, null);
                });
                finishWindow();
            } else if (prize.type == PrizeType.PRIZE_EQUIPMENT) {
                Equip equip = EquipManagerment.Instance.createEquip(prize.pSid);
                if (equip == null) {
                    MaskWindow.UnlockUI();
                    return;
                }
                UiManager.Instance.openWindow<EquipAttrWindow>((win) => {
                    win.Initialize(equip, EquipAttrWindow.OTHER, null);
                });
                finishWindow();
            } else if (prize.type == PrizeType.PRIZE_MAGIC_WEAPON) {
                MagicWeapon magic = MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
                if (magic == null) {
                    MaskWindow.UnlockUI();
                    return;
                }
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(magic, MagicWeaponType.FORM_OTHER);
                });
                finishWindow();
            } else {
                if (prize != null) {
                    UiManager.Instance.openDialogWindow<PropAttrWindow>((win) => {
                        win.Initialize(prize);
                        finishWindow();
                    });
                } else {
                    MaskWindow.UnlockUI();
                }
            }
        }
	}
}
