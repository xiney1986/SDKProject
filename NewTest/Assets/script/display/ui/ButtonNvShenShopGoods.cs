using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/**
 *  商店按钮节点
 * */
public class ButtonNvShenShopGoods : ButtonBase {
	public GameObject goodsViewProfab;
	public GameObject goodsPoint;
	public UISprite spriteScrap;
	public UILabel goodsName;
	public UILabel contributeNum;
    public UILabel times;
    public UILabel desc;
    public Transform locked;
	public UISprite icon_backGround;
    private const int STRING_LENGTH = 17;
    private CallBack shopUpdate;
    public GameObject stars;//星星
    public UILabel openCondition;//开放条件

	/**关于物品的文字描述 */
	public UITexture itemIcon;
	public ButtonGoods buyButton;
	public Goods goods;

    public void update(MessageHandle msg) 
    {
        (fatherWindow as NvshenShopWindow).updateContent();
        if (goods.getNowBuyNum() >= goods.getGoodsMaxBuyCount()) 
        {
            (fatherWindow as NvshenShopWindow).updateContent();
            return;
        }
        updateGoods(goods, shopUpdate);
    }

    public void buy(MessageHandle msg) 
    {
        if (msg.msgEvent == msg_event.dialogCancel)
            return;
        goods = msg.msgInfo as Goods;
        BuyGoodsFPort fport = FPortManager.Instance.getFPort("BuyGoodsFPort") as BuyGoodsFPort;
        fport.buyGoods((msg.msgInfo as Goods).sid, msg.msgNum, buyCallBack);
    }
    private void buyCallBack(int sid, int num) {
        goods.nowBuyNum += num;
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("s0056", goods.getName(), (num * goods.getGoodsShowNum()).ToString()));
            update(null);
        });
    }
    public void showStar(int level) {
        if (stars == null) return;
        for (int i = 0; i < level; i++) {
            stars.transform.GetChild(i).gameObject.SetActive(true);
        }
        if (level == CardSampleManager.ONESTAR) {
            stars.transform.localPosition = new Vector3(33, -30, 0);
        } else if (level == CardSampleManager.TWOSTAR) {
            stars.transform.localPosition = new Vector3(22, -30, 0);
        } else if (level == CardSampleManager.THREESTAR) {
            stars.transform.localPosition = new Vector3(11, -30, 0);
        } else if (level == CardSampleManager.FOURSTAR) {
            stars.transform.localPosition = new Vector3(0, -30, 0);
        }
    }
    //初始化商品条目信息
	public void updateGoods (Goods goods, CallBack callback) {
        //判断商品是否已解锁
        bool isLocked = false;
        buyButton.disableButton(true);
        buyButton.gameObject.SetActive(true);
		this.goods = goods;
        this.shopUpdate = callback;
		this.goodsName.text = goods.getName();
        this.contributeNum.text = goods.getCostPrice() + "";
	    openCondition.text = "";
        //还没有开放，显示解锁条件
	    if (!FuBenManagerment.Instance.isCanShow(goods.sid))
	    {
            string[] st = CommandConfigManager.Instance.getNvShenShopSid();
            int flagSid = 0;
            for (int i = 0; i < st.Length; i++) {
                string[] kk = st[i].Split('#');
                int tempMissionSid = StringKit.toInt(kk[0].Substring(kk[0].Length -2 ,2));
                int tempGoodSid = StringKit.toInt(kk[1]);
                if (goods.sid == tempGoodSid)
                {
                    openCondition.text = LanguageConfigManager.Instance.getLanguage("nvShenShop_openConditon", tempMissionSid + "");
                    buyButton.gameObject.SetActive(false);
                    break;
                }
            }
	    }
	    if (stars != null) {
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        if (goods.getGoodsMaxBuyCount() == 0) {
            this.times.text = LanguageConfigManager.Instance.getLanguage("s0280l1");
        } else {
            this.times.text = LanguageConfigManager.Instance.getLanguage("prefabzc20") + Math.Max(goods.getGoodsMaxBuyCount() - goods.getNowBuyNum(), 0).ToString() + "/" + goods.getGoodsMaxBuyCount() + "";
        }
        //该商品是否已解锁
        if (!isLocked) {
            locked.parent.gameObject.collider.enabled = true;
            //购买物品次数是否已经达到上限
            if (goods.getGoodsMaxBuyCount() > 0 && goods.getNowBuyNum() >= goods.getGoodsMaxBuyCount()) {
                buyButton.disableButton(true);
            } else {
                int propSid = goods.getCostToolSid();
                Prop p = StorageManagerment.Instance.getProp(propSid);
                if (p == null || p.getNum() < goods.getCostPrice()) {
                    buyButton.disableButton(true);
                } else {
                    buyButton.disableButton(false);
                    buyButton.goods = this.goods;
                    buyButton.callback = buy;
                    buyButton.fatherWindow = fatherWindow;
                }

            }
        } else {
            locked.gameObject.SetActive(true);//不可以购买，点击无效
            locked.parent.gameObject.collider.enabled = false;
        }
		string des = "";
		//string nameColor = "";
		if (goods.getGoodsType () == PrizeType.PRIZE_EQUIPMENT) {
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
			des = EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).desc;
			//nameColor = QualityManagerment.getQualityColor (EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
		}
		else if (goods.getGoodsType () == PrizeType.PRIZE_PROP) {
            PropSample ps=PropSampleManager .Instance.getPropSampleBySid (goods.getGoodsSid ());
            icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName(ps.qualityId);
			des = PropSampleManager.Instance.getPropSampleBySid (goods.getGoodsSid ()).describe;
            if (goods.isCardScrap() && stars != null) {//如果是卡片碎片，显示星星
                stars.transform.localPosition = new Vector3(0, -30, 0);
                int propSid = goods.getGoodsSid ();//碎片sid
                Card card = CardScrapManagerment.Instance.getCardByScrapSid(propSid);//根据卡片碎片sid获取对应的卡片
                if (card != null) {
                    int cardStarLevel = CardSampleManager.Instance.getStarLevel(card.sid);//卡片星级
                    showStar(cardStarLevel);
                }
            } else if (ps.type == PropType.PROP_MAGIC_SCRAP) {
                MagicWeapon magic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(ps.sid);
                if (magic != null && MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid) != null) {
                    int level = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid).starLevel;
                    showStar(level);
                }
            } 
            if (ps.type==PropType.PROP_TYPE_EQUIPSCRAP) {
            }
			//nameColor = QualityManagerment.getQualityColor (PropSampleManager.Instance.getPropSampleBySid (goods.getGoodsSid ()).qualityId);
		}
		else if (goods.getGoodsType () == PrizeType.PRIZE_STARSOUL) {
			des = StarSoulManager.Instance.createStarSoul (goods.getGoodsSid ()).getDescribe ();
			//nameColor = QualityManagerment.getQualityColor (StarSoulManager.Instance.createStarSoul (goods.getGoodsSid ()).getQualityId ());
		}else if(goods.getGoodsType()==PrizeType.PRIZE_MAGIC_WEAPON){
            icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName(MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(goods.getGoodsSid()).qualityId);
            des = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(goods.getGoodsSid()).desc;
            //nameColor = QualityManagerment.getQualityColor(MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(goods.getGoodsSid()).qualityId);
            if (MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(goods.getGoodsSid()) != null) {
                int level = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(goods.getGoodsSid()).starLevel;
                showStar(level);
            }
        }
		if (spriteScrap != null) {
			if (goods.isScrap ()) {
				spriteScrap.gameObject.SetActive (true);
			}
			else {
				spriteScrap.gameObject.SetActive (false);
			}
		}
		showGoodsTex ();
        des = des.Split('~')[0];
        Regex r = new Regex("\\[.+?\\]");
        MatchCollection mc = r.Matches(des);
        int index = 0;
        for (int i = 0; i < mc.Count; i++) {
            if (mc[i].Index > STRING_LENGTH - 1) {
                break;
            } else if (mc[i].Index <= STRING_LENGTH - 1 && mc[i].Index + mc[i].Value.Length > STRING_LENGTH) {
                index += mc[i].Value.Length;
                break;
            } else {
                index += mc[i].Value.Length;
            }
        }
        if (des.Length > (STRING_LENGTH + index)) {
            des = des.Substring(0, STRING_LENGTH - 1 + index) + "...";
        }
        if (desc != null)
            desc.text = des;
	}
	
    //显示商品图标
	private void showGoodsTex () {
		if (itemIcon != null) {
			itemIcon.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + goods.getIconId (), itemIcon);
		}
	}
	
	
	public override void DoClickEvent () {
		base.DoClickEvent ();  
		if (goods.getGoodsType () == GoodsType.CARD) {
			UiManager.Instance.openWindow <CardBookWindow> ((win) => {
				Card card = CardManagerment.Instance.createCard (goods.getGoodsSid ());
				win.init (card, CardBookWindow.OTHER, null);
			});
		}
		else if (goods.getGoodsType () == GoodsType.EQUIP) {
			UiManager.Instance.openWindow <EquipAttrWindow> ((win) => {
				Equip eq = EquipManagerment.Instance.createEquip ("", goods.getGoodsSid (), 0, 0,0);
				win.Initialize (eq, EquipAttrWindow.OTHER, null);
			});
		}
		else if (goods.getGoodsType () == GoodsType.TOOL) {
			Prop prop = PropManagerment.Instance.createProp (goods.getGoodsSid (), goods.getGoodsShowNum ()); 
			UiManager.Instance.openDialogWindow<PropAttrWindow> ((win) => {
				win.Initialize (prop);
			});
		}
		else if (goods.getGoodsType () == GoodsType.STARSOUL) {		
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (goods.getGoodsSid ()); 
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> ((win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
		} 
	}
}
