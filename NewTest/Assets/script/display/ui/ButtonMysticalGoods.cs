using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class ButtonMysticalGoods : ButtonBase
{

	/** 碎片标签 */
	public UISprite spriteScrap;
	public UILabel topLabel;
	public UILabel bottomLabel;
	public UISprite costIcon;
	public UISprite icon_backGround;
	public UITexture itemIcon;
	public UILabel buyTime;
	public UILabel goodsNum;
	//public ButtonGoods buyButton;
	public Goods goods;
    public UISprite sell;
	public GameObject costPoint;
	/**关于物品的文字描述 */
	public UILabel infoDec;
	private CallBack shopUpdate;
	private Timer timer;//倒计时
	public UISprite timeBg;
	public const int RMBSHOP = 1;//RMB商店
	public const int GUILDSHOP = 3;//公会商店
	public const int MERIT_SHOP = 4;//功勋商店
	public const int STARSOUL_DEBRIS_SHOP = 5;//星魂碎片商店
	private int intoType = 0;//进入类型
	public const int MYSTICAL_SHOP = 6;
	public UISprite zhekou_sprite;//折扣标签
	public UILabel newConstNum;//打折的价格
	public UISprite red_line;//打折红线显示
	public UILabel propNum;//商品的数量
	public bool isNameColor=true;//是否修改文字颜色
	private const int STRING_LENGTH=17;//截取字符串长度
    public GameObject stars;//星星
	public void buy (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogCancel)
			return;
		goods = msg.msgInfo as Goods;
		if (intoType == MYSTICAL_SHOP) {
			MysticalShopBuyFPort fport = FPortManager.Instance.getFPort ("MysticalShopBuyFPort") as MysticalShopBuyFPort;
			fport.buyGoods ((msg.msgInfo as Goods).sid,(msg.msgInfo as Goods).showIndex+1, buyCallBack);
		}
	}
	
	private void buyCallBack (int sid, int num)
	{ 
		goods.nowBuyNum += num;       
        UiManager.Instance.openDialogWindow<MessageLineWindow>(
                        (win) =>
                        {
                            win.Initialize(LanguageConfigManager.Instance.getLanguage("s0056", goods.getName(), (num * goods.getGoodsShowNum()).ToString()));
                        });
        update(null);
        
	}
    private void showStar(int level) {
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
	public void updateGoods (Goods goods, CallBack callback, int intoType)
	{
		this.intoType = intoType;
		this.goods = goods;  
		this.shopUpdate = callback;
        if (stars != null) {
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
		updateConstIcon ();
		if (goods.getShowTime () != "") {
			buyTime.text = goods.getShowTime ();
			buyTime.gameObject.SetActive (false);
			timeBg.gameObject.SetActive (false);
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
			timer.addOnTimer (showTime);
			timer.start ();
		} else {
			buyTime.gameObject.SetActive (false);
			timeBg.gameObject.SetActive (false);
		}
		//buyButton.disableButton (false);
		//buyButton.goods = this.goods;
		//buyButton.callback = buy;
		//buyButton.fatherWindow = fatherWindow;
		updateByButtonState ();
		updateIsBuyState ();
		string des="";
		string nameColor="";
		if (goods.getGoodsType () == PrizeType.PRIZE_EQUIPMENT) {
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
			des=EquipmentSampleManager.Instance.getEquipSampleBySid(goods.getGoodsSid()).desc;
			nameColor=QualityManagerment.getQualityColor(EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
		} else if (goods.getGoodsType () == PrizeType.PRIZE_PROP) {
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (PropSampleManager .Instance.getPropSampleBySid (goods.getGoodsSid ()).qualityId);
			des=PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).describe;
			nameColor=QualityManagerment.getQualityColor(PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).qualityId);
            if (goods.isCardScrap() && stars != null) {//如果是卡片碎片，显示星星
                stars.transform.localPosition = new Vector3(0, -30, 0);
                int propSid = PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).sid;
                Card card = CardScrapManagerment.Instance.getCardByScrapSid(propSid);//根据卡片碎片id获取对应卡片
                if (card != null) {
                    int cardStarLevel = CardSampleManager.Instance.getStarLevel(card.sid);//卡片星级
                    showStar(cardStarLevel);
                }
            } else if (goods.isMagicScrap() && stars != null) {
                stars.transform.localPosition = new Vector3(0, -30, 0);
                int propSid = PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).sid;
                MagicWeapon magic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(propSid);
                if (magic != null && MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid) != null) {
                    int starLevel = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid).starLevel;
                    showStar(starLevel);
                }
            }
		}else if(goods.getGoodsType()==PrizeType.PRIZE_CARD){
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (CardSampleManager.Instance.getRoleSampleBySid (goods.getGoodsSid ()).qualityId);
            if (stars != null) {//如果是卡片，显示星星
                stars.transform.localPosition = new Vector3(0, -30, 0);
                CardSample card = CardSampleManager.Instance.getRoleSampleBySid(goods.getGoodsSid());//根据id获取对应卡片
                if (card != null) {
                    int cardStarLevel = CardSampleManager.Instance.getStarLevel(card.sid);//卡片星级
                    showStar(cardStarLevel);
                }
            }
			//des=CardSampleManager.Instance.getRoleSampleBySid (goods.getGoodsSid ()).d;
		} else if(goods.getGoodsType () == PrizeType.PRIZE_STARSOUL) {
			//CreateStarSoulItem(goods.getGoodsSid());
			des=StarSoulManager.Instance.createStarSoul(goods.getGoodsSid()).getDescribe();
			nameColor=QualityManagerment.getQualityColor(StarSoulManager.Instance.createStarSoul(goods.getGoodsSid()).getQualityId());
		}
		if (spriteScrap != null) {
			if (goods.isScrap ()&&goods.getOfferNum()<=0) {
				spriteScrap.gameObject.SetActive (true);
			} else {
				spriteScrap.gameObject.SetActive (false);
			}
		}
		showGoodsTex ();
		if(nameColor!=""&&isNameColor){
			topLabel.text = nameColor+goods.getName ();
		}else
			topLabel.text = goods.getName ();
		des=des.Split('~')[0];
		Regex r=new Regex("\\[.+?\\]");
		MatchCollection mc=r.Matches(des);
		int index=0;
		for(int i=0;i<mc.Count;i++){
			if(mc[i].Index>STRING_LENGTH-1){
				break;
			}else if(mc[i].Index<=STRING_LENGTH-1&&mc[i].Index+mc[i].Value.Length>STRING_LENGTH){
				index+=mc[i].Value.Length;
				break;
			}else{
				index+=mc[i].Value.Length;
			}
			
		}
		if(des.Length>(STRING_LENGTH+index)){
			des=des.Substring(0,STRING_LENGTH-1+index)+"...";
		}		
		infoDec.text=des;
		propNum.text = "x " + goods.getGoodsShowNum ();

	}

	private void updateIsBuyState ()
	{
		if (goods.getIsBuy () == 1) {
			//buyButton.disableButton (true);
			sell.gameObject.SetActive(true);
			costPoint.SetActive(false);
		}else{
			sell.gameObject.SetActive(false);
			costPoint.SetActive(true);
		}
	}

	private void updateByButtonState ()
	{
		switch (goods.getCostType ()) {
		case PrizeType.PRIZE_MONEY:
			if (goods.getOfferNum () > 0) {
				updateState (true);
				if (UserManager.Instance.self.getMoney () >= goods.getCostPrice ()) {
					//buyButton.disableButton (false);
					updateMoney (true);
				} else {
					//buyButton.disableButton (true);
					updateMoney (false);
				}
			} else {
				updateState (false);
				if (UserManager.Instance.self.getMoney () >= goods.getCostPrice ()) {
					//buyButton.disableButton (false);
					updateMoneyy (true);
				} else {
					//buyButton.disableButton (true);
					updateMoneyy (false);
				}
			}
			break;
		case PrizeType.PRIZE_RMB:
			if (goods.getOfferNum () > 0) {
				updateState (true);
				if (UserManager.Instance.self.getRMB () >= goods.getCostPrice ()) {
					//buyButton.disableButton (false);
					updateMoney (true);
				} else {
					//buyButton.disableButton (true);
					updateMoney (false);
				}
			} else {
				updateState (false);
				if (UserManager.Instance.self.getRMB () >= goods.getCostPrice ()) {
					//buyButton.disableButton (false);
					updateMoneyy (true);
				} else {
					//buyButton.disableButton (true);
					updateMoneyy (false);
				}
			}
			break;
		case PrizeType.PRIZE_PROP:
			Prop  pp=StorageManagerment.Instance.getProp(goods.getCostToolSid ());
			if (goods.getOfferNum () > 0) {
				updateState (true);
				if (pp!=null&&pp.getNum() >= goods.getCostPrice ()) {
					//buyButton.disableButton (false);
					updateMoney (true);
				} else {
					//buyButton.disableButton (true);
					updateMoney (false);
				}
			} else {
				updateState (false);
				if (pp!=null&&pp.getNum() >= goods.getCostPrice ()) {
					//buyButton.disableButton (false);
					updateMoneyy (true);
				} else {
					//buyButton.disableButton (true);
					updateMoneyy (false);
				}
			}
			break;
		}
	}

	private void updateConstIcon ()
	{
		switch (goods.getCostType ()) {
		case PrizeType.PRIZE_MONEY:
			costIcon.spriteName = "icon_money";
			break;
		case PrizeType.PRIZE_RMB:
			costIcon.spriteName = "icon_Addrmb";
			break;
		case PrizeType.PRIZE_PROP:
			costIcon.spriteName = StringKit.intToFixString (goods.getCostToolSid ()) + "icon";
			break;
		}
	}

	private void updateMoney (bool bl)
	{
		if (bl) {
			bottomLabel.text = "[FFFFFF]" + goods.getOfferNum ().ToString () + "[-]";
			newConstNum.transform.localPosition = bottomLabel.transform.localPosition + new Vector3 (bottomLabel.width + 10, 0f, 0f);
			red_line.transform.localPosition = bottomLabel.transform.localPosition;
			red_line.width = bottomLabel.width;
			newConstNum.text = "[FFFFFF]" + goods.getCostPrice ().ToString () + "[-]";
		} else {
			bottomLabel.text = "[FFFFFF]" + goods.getOfferNum ().ToString () + "[-]";
			newConstNum.transform.localPosition = bottomLabel.transform.localPosition + new Vector3 (bottomLabel.width + 10, 0f, 0f);
			red_line.transform.localPosition = bottomLabel.transform.localPosition;
			red_line.width = bottomLabel.width;
			newConstNum.text = "[ff0000]" + goods.getCostPrice ().ToString () + "[-]";
		}
	}

	private void updateMoneyy (bool bl)
	{
		if (bl) {
			bottomLabel.text = "[FFFFFF]" + goods.getCostPrice ().ToString () + "[-]";
		} else {
			bottomLabel.text = "[ff0000]" + goods.getCostPrice ().ToString () + "[-]";
		}
	}

	private void updateState (bool bl)
	{
		if (bl) {
			zhekou_sprite.gameObject.SetActive (true);
			newConstNum.gameObject.SetActive (true);
			red_line.gameObject.SetActive (true);
		} else {
			zhekou_sprite.gameObject.SetActive (false);
			newConstNum.gameObject.SetActive (false);
			red_line.gameObject.SetActive (false);
		}

	}

	private void showGoodsTex ()
	{
		if (itemIcon != null) {
			itemIcon.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + goods.getIconId (), itemIcon);
		}
	}

	private void showTime ()
	{
		buyTime.text = goods.getShowTime ();
	}
	
	public void update (MessageHandle msg)
	{ 
		ShopManagerment.Instance.updateMysticalGood (goods.sid,goods.showIndex);
		(fatherWindow as ShopWindow).UpdateContent ();
		updateGoods (goods, shopUpdate, intoType);
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();  
		if (goods.getGoodsType () == GoodsType.CARD) {
			UiManager.Instance.openWindow <CardBookWindow> ((win) => {
				Card card = CardManagerment.Instance.createCard (goods.getGoodsSid ());
				win.init (card, CardBookWindow.OTHER, null);
			});
		} else if (goods.getGoodsType () == GoodsType.EQUIP) {
			UiManager.Instance.openWindow <EquipAttrWindow> ((win) => {
				Equip eq = EquipManagerment.Instance.createEquip ("", goods.getGoodsSid (), 0, 0,0);
				win.Initialize (eq, EquipAttrWindow.OTHER, null);
			});
		} else if (goods.getGoodsType () == GoodsType.TOOL) {
			Prop prop = PropManagerment.Instance.createProp (goods.getGoodsSid (), goods.getGoodsShowNum ()); 
			UiManager.Instance.openDialogWindow<PropBuyWindow> ((win) => {
                win.Initialize(prop, goods, () => {
                    MysticalShopBuyFPort fport = FPortManager.Instance.getFPort("MysticalShopBuyFPort") as MysticalShopBuyFPort;
                    fport.buyGoods(goods.sid, goods.showIndex + 1, buyCallBack);
                });
			});
		} else if (goods.getGoodsType () == GoodsType.STARSOUL) {		
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (goods.getGoodsSid ()); 
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> ((win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
		} 
	}
}
