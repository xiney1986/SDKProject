using UnityEngine;
using System.Collections;

//物品属性窗口
//李程
public class PropBuyWindow : WindowBase
{
	
	public Prop chooseProp;
	public PrizeSample prize;
	public UILabel propName;
	public UILabel propHaveNumber;
	public UITexture propImage;
	public UILabel  propDescript;
	public UISprite quality;
	public UISprite scrapIcon;
    public UISprite costIcon;//货币图标
    public UILabel bottomLabel;//价格
    //public UILabel newConstNum;//打折的价格
    public ButtonBase buyButton;
	//public const int TEMPSTORE = 2;//临时仓库
    public GameObject stars;//星星
	private Vector3 arrowPosition;
    private Goods goods;
    private CallBack callback;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void setPosition(Vector3 v3)
	{
		arrowPosition = v3;
	}
	public void Initialize (Prop chooseItem,Goods good,CallBack callback)
	{
        this.callback = callback;
        this.goods = good;
		chooseProp = chooseItem; 
		if(chooseProp != null){
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + chooseProp.getIconId (), propImage);
			propName.text = QualityManagerment.getQualityColor(chooseProp.getQualityId()) + chooseProp .getName () + "";				
			Prop pp=StorageManagerment.Instance.getProp(chooseItem.sid);
            if (chooseItem.isCardScrap() && stars != null) {
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
            updateConstIcon();
            updateByButtonState();
		}

	}
    /// <summary>
    /// 显示星星
    /// </summary>
    void showStar(Card showCard) {
        stars.transform.localPosition = new Vector3(0, -35, 0);
        int cardStarLevel = CardSampleManager.Instance.getStarLevel(showCard.sid);//卡片星级
        for (int i = 0; i < cardStarLevel; i++) {
            stars.transform.GetChild(i).gameObject.SetActive(true);
        }
        if (cardStarLevel == CardSampleManager.ONESTAR) {
            stars.transform.localPosition = new Vector3(33, -40, 0);
        } else if (cardStarLevel == CardSampleManager.TWOSTAR) {
            stars.transform.localPosition = new Vector3(22, -40, 0);
        } else if (cardStarLevel == CardSampleManager.THREESTAR) {
            stars.transform.localPosition = new Vector3(11, -40, 0);
        } else if (cardStarLevel == CardSampleManager.FOURSTAR) {
            stars.transform.localPosition = new Vector3(0, -40, 0);
        }
    }
    private void updateState(bool bl) {
        if (bl) {
            //newConstNum.gameObject.SetActive(true);
        } else {
            //newConstNum.gameObject.SetActive(false);
        }
    }
    private void updateByButtonState() {
        switch (goods.getCostType()) {
            case PrizeType.PRIZE_MONEY:
                if (goods.getOfferNum() > 0) {
                    updateState(true);
                    if (UserManager.Instance.self.getMoney() >= goods.getCostPrice()) {
                        buyButton.disableButton(false);
                        updateMoney(true);
                    } else {
                        buyButton.disableButton(true);
                        updateMoney(false);
                    }
                } else {
                    updateState(false);
                    if (UserManager.Instance.self.getMoney() >= goods.getCostPrice()) {
                        buyButton.disableButton(false);
                        updateMoneyy(true);
                    } else {
                        buyButton.disableButton(true);
                        updateMoneyy(false);
                    }
                }
                break;
            case PrizeType.PRIZE_RMB:
                if (goods.getOfferNum() > 0) {
                    updateState(true);
                    if (UserManager.Instance.self.getRMB() >= goods.getCostPrice()) {
                        buyButton.disableButton(false);
                        updateMoney(true);
                    } else {
                        buyButton.disableButton(true);
                        updateMoney(false);
                    }
                } else {
                    updateState(false);
                    if (UserManager.Instance.self.getRMB() >= goods.getCostPrice()) {
                        buyButton.disableButton(false);
                        updateMoneyy(true);
                    } else {
                        buyButton.disableButton(true);
                        updateMoneyy(false);
                    }
                }
                break;
            case PrizeType.PRIZE_PROP:
                if (goods.getIsBuy() == 1) {
                    buyButton.gameObject.SetActive(false);
                    costIcon.gameObject.SetActive(false);
                    return;
                }
                Prop pp = StorageManagerment.Instance.getProp(goods.getCostToolSid());
                if (goods.getOfferNum() > 0) {
                    updateState(true);
                    if (pp != null && pp.getNum() >= goods.getCostPrice()) {
                        buyButton.disableButton(false);
                        updateMoney(true);
                    } else {
                        buyButton.disableButton(true);
                        updateMoney(false);
                    }
                } else {
                    updateState(false);
                    if (pp != null && pp.getNum() >= goods.getCostPrice()) {
                        buyButton.disableButton(false);
                        updateMoneyy(true);
                    } else {
                        buyButton.disableButton(true);
                        updateMoneyy(false);
                    }
                }
                break;
        }
    }
    private void updateMoneyy(bool bl) {
        if (bl) {
            bottomLabel.text = "[FFFFFF]" + goods.getCostPrice().ToString() + "[-]";
        } else {
            bottomLabel.text = "[ff0000]" + goods.getCostPrice().ToString() + "[-]";
        }
    }
    private void updateMoney(bool bl) {
        if (bl) {
            bottomLabel.text = "[FFFFFF]" + goods.getOfferNum().ToString() + "[-]";
           // newConstNum.transform.localPosition = bottomLabel.transform.localPosition + new Vector3(bottomLabel.width + 10, 0f, 0f);
           // newConstNum.text = "[FFFFFF]" + goods.getCostPrice().ToString() + "[-]";
        } else {
            bottomLabel.text = "[FFFFFF]" + goods.getOfferNum().ToString() + "[-]";
           // newConstNum.transform.localPosition = bottomLabel.transform.localPosition + new Vector3(bottomLabel.width + 10, 0f, 0f);
           // newConstNum.text = "[ff0000]" + goods.getCostPrice().ToString() + "[-]";
        }
    }
    private void updateConstIcon() {
        switch (goods.getCostType()) {
            case PrizeType.PRIZE_MONEY:
                costIcon.spriteName = "icon_money";
                break;
            case PrizeType.PRIZE_RMB:
                costIcon.spriteName = "icon_Addrmb";
                break;
            case PrizeType.PRIZE_PROP:
                costIcon.spriteName = StringKit.intToFixString(goods.getCostToolSid()) + "icon";
                break;
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
        if (gameObj.name == "buttonClose") {
			finishWindow ();
		}
        if (gameObj.name == "buttonOk") {
            dialogCloseUnlockUI = false;
            finishWindow();
            callback();
        } else if (gameObj.name == "imageButton") {
			if(fatherWindow == UiManager.Instance.getWindow<ShopWindow>())
			{
				MaskWindow.UnlockUI();
				return;
			}
            if (chooseProp != null && chooseProp.isScrap() && prize == null) {
                if (chooseProp.isCardScrap()) {
                    Card tmpCard = CardScrapManagerment.Instance.getCardByScrapSid(chooseProp.sid);
                    if (tmpCard != null) {
                        CardBookWindow.Show(tmpCard, CardBookWindow.OTHER, null);
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                } else if(chooseProp.isEquipScrap()) {
                    Equip tempEquip = EquipScrapManagerment.Instance.getEquipByScrapSid(chooseProp.sid);
                    if (tempEquip != null) {
                        UiManager.Instance.openWindow<EquipAttrWindow>((win) => {
                            win.Initialize(tempEquip, EquipAttrWindow.OTHER, null);
                        });
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                }else if (chooseProp.isMagicScrap()){
                    MagicWeapon tempMagic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(chooseProp.sid);
                    if (tempMagic != null) {
                        UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                            win.init(tempMagic, MagicWeaponType.FORM_OTHER);
                        });
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                }
                 } else if (chooseProp == null && prize != null && prize.type == PrizeType.PRIZE_PROP) {
                Prop propTemp = PropManagerment.Instance.createProp(prize.pSid);
                if (propTemp.isCardScrap()) {
                    Card tmpCard = CardScrapManagerment.Instance.getCardByScrapSid(propTemp.sid);
                    if (tmpCard != null) {
                        CardBookWindow.Show(tmpCard, CardBookWindow.OTHER, null);
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                } else if (chooseProp.isMagicScrap()) {
                    MagicWeapon tempMagic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(chooseProp.sid);
                    if (tempMagic != null) {
                        UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                            win.init(tempMagic, MagicWeaponType.FORM_OTHER);
                        });
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                }
                else if (propTemp.isEquipScrap()) {
                    Equip tempEquip = EquipScrapManagerment.Instance.getEquipByScrapSid(propTemp.sid);
                    if (tempEquip != null) {
                        UiManager.Instance.openWindow<EquipAttrWindow>((win) => {
                            win.Initialize(tempEquip, EquipAttrWindow.OTHER, null);
                        });
                        closeWin();
                    } else {
                        MaskWindow.UnlockUI();
                    }
                } else {
                    MaskWindow.UnlockUI();
                }
            } else {
                MaskWindow.UnlockUI();
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
