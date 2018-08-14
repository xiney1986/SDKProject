using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

//特惠小组件
public class ButtonTeHuiGoods : ButtonBase
{

	/** 碎片标签 */
	public UISprite spriteScrap;
	public UILabel topLabel;//商品的名字
	public UILabel bottomLabel;//实际卖价
	public UISprite costIcon;//花费的道具图标
	public UISprite icon_backGround;//品质背景
	public UITexture itemIcon;//物品图标
	public ButtonGoods buyButton;//购买按钮
	public Goods goods;
	public GameObject costPoint;
	private CallBack shopUpdate;
	public const int RMBSHOP = 1;//RMB商店
	public const int GUILDSHOP = 3;//公会商店
	public const int MERIT_SHOP = 4;//功勋商店
	public const int STARSOUL_DEBRIS_SHOP = 5;//星魂碎片商店
	private int intoType = 0;//进入类型
	public const int MYSTICAL_SHOP = 20;
	public UILabel OldConst;//原始价格
	public UISprite red_line;//红线显示
	public UILabel propNum;//商品的数量
	public bool isNameColor=true;//是否修改文字颜色
	public UILabel buyCount;//购买次数
	public UISprite oldCOnstIcion;
	private const int STRING_LENGTH=17;//截取字符串长度
    public GameObject stars;//星星
	
	public void buy (MessageHandle msg)
	{
//		if (msg.msgEvent == msg_event.dialogCancel)
//			return;
//		goods = msg.msgInfo as Goods;
		if (intoType ==  ContentShopGoods.TEHUI_SHOP) {
			if(UserManager.Instance.self.getRMB()< goods.getCostPrice())
			{
				UiManager.Instance.openDialogWindow<MessageWindow>((win) =>{
					win.dialogCloseUnlockUI = false;
					win.initWindow(2, Language("s0094"), Language("s0324"), LanguageConfigManager.Instance.getLanguage("s0158"), (msg1) =>{
						if (msg1.msgEvent == msg_event.dialogOK)
						{
							UiManager.Instance.openWindow<rechargeWindow>();
						}
						else
						{
							MaskWindow.UnlockUI();
						}
					});
				});
			}
			else
			{
				BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
				fport.buyGoods (goods.sid,1,buyCallBack);//(msg.msgInfo as Goods).sid, msg.msgNum, buyCallBack);
			}

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
	
	public void updateGoods (Goods goods, CallBack callback, int intoType)
	{
		this.intoType = intoType;
		this.goods = goods;  
		this.shopUpdate = callback;
		updateConstIcon ();//更新花费类型图标
		buyButton.disableButton (false);
		buyButton.goods = this.goods;
		buyButton.callback = buy;
		buyButton.fatherWindow = fatherWindow;
		updateIsBuyState ();
		updateByButtonState ();
		string nameColor="";
		if (goods.getGoodsType () == PrizeType.PRIZE_EQUIPMENT) {
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
			nameColor=QualityManagerment.getQualityColor(EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
		} else if (goods.getGoodsType () == PrizeType.PRIZE_PROP) {
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (PropSampleManager .Instance.getPropSampleBySid (goods.getGoodsSid ()).qualityId);
			nameColor=QualityManagerment.getQualityColor(PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).qualityId);
            if (goods.isCardScrap() && stars != null) {//如果是卡片碎片，显示星星
                stars.transform.localPosition = new Vector3(0, -30, 0);
                int propSid = PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).sid;
                Card card = CardScrapManagerment.Instance.getCardByScrapSid(propSid);//根据卡片碎片id获取对应卡片
                int cardStarLevel = CardSampleManager.Instance.getStarLevel(card.sid);//卡片星级
                for (int i = 0; i < cardStarLevel; i++) {
                    stars.transform.GetChild(i).gameObject.SetActive(true);
                }
                if (cardStarLevel == CardSampleManager.ONESTAR) {
                    stars.transform.localPosition = new Vector3(33, -30, 0);
                } else if (cardStarLevel == CardSampleManager.TWOSTAR) {
                    stars.transform.localPosition = new Vector3(22, -30, 0);
                } else if (cardStarLevel == CardSampleManager.THREESTAR) {
                    stars.transform.localPosition = new Vector3(11, -30, 0);
                } else if (cardStarLevel == CardSampleManager.FOURSTAR) {
                    stars.transform.localPosition = new Vector3(0, -30, 0);
                }
            }
		}else if(goods.getGoodsType()==PrizeType.PRIZE_CARD){
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (CardSampleManager.Instance.getRoleSampleBySid (goods.getGoodsSid ()).qualityId);
            if (stars != null) {//如果是卡片，显示星星
                stars.transform.localPosition = new Vector3(0, -30, 0);
                CardSample card = CardSampleManager.Instance.getRoleSampleBySid(goods.getGoodsSid());//根据id获取对应卡片
                int cardStarLevel = CardSampleManager.Instance.getStarLevel(card.sid);//卡片星级
                for (int i = 0; i < cardStarLevel; i++) {
                    stars.transform.GetChild(i).gameObject.SetActive(true);
                }
                if (cardStarLevel == CardSampleManager.ONESTAR) {
                    stars.transform.localPosition = new Vector3(33, -30, 0);
                } else if (cardStarLevel == CardSampleManager.TWOSTAR) {
                    stars.transform.localPosition = new Vector3(22, -30, 0);
                } else if (cardStarLevel == CardSampleManager.THREESTAR) {
                    stars.transform.localPosition = new Vector3(11, -30, 0);
                } else if (cardStarLevel == CardSampleManager.FOURSTAR) {
                    stars.transform.localPosition = new Vector3(0, -30, 0);
                }
            }
			//des=CardSampleManager.Instance.getRoleSampleBySid (goods.getGoodsSid ()).d;
		} else if(goods.getGoodsType () == PrizeType.PRIZE_STARSOUL) {
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
			topLabel.text =goods.getName ();
		}else
			topLabel.text = goods.getName ();
		propNum.text = "x " + goods.getGoodsShowNum ();
		int maxNumm=GoodsBuyCountManager.Instance.getMaxNum(goods.sid);
        buyCount.text = LanguageConfigManager.Instance.getLanguage("s0151") + ":" + (maxNumm-(goods.nowBuyNum)) + "/" + maxNumm.ToString();
	}
	/// <summary>
	/// 是否超过了购买上限
	/// </summary>
	private void updateIsBuyState ()
	{
		//nowBuyNum 
		if(GoodsBuyCountManager.Instance.getMaxNum(goods.sid)<=goods.nowBuyNum)buyButton.disableButton (true);
		else buyButton.disableButton (false);
	}
	/// <summary>
	/// 更新物品可以不可以买的状态 达到了购买限制不可以买
	/// </summary>
	private void updateByButtonState ()
	{
		switch (goods.getCostType ()) {//类型区分
		case PrizeType.PRIZE_MONEY:
			if (UserManager.Instance.self.getMoney () >= goods.getCostPrice ()) {
                if (!buyButton.isDisable()) buyButton.disableButton(false);
				updateMoney (true);
			} else {
				buyButton.disableButton (true);
				updateMoney (false);
			}
			break;
		case PrizeType.PRIZE_RMB:
			if (UserManager.Instance.self.getRMB () >= goods.getCostPrice ()) {
				if(!buyButton.isDisable())buyButton.disableButton (false);
				updateMoney (true);
			} else {
				//buyButton.disableButton (true);
				updateMoney (false);
			}
			break;
		case PrizeType.PRIZE_PROP:
			Prop  pp=StorageManagerment.Instance.getProp(goods.getCostToolSid ());
			if (pp!=null&&pp.getNum() >= goods.getCostPrice ()) {
                if (!buyButton.isDisable()) buyButton.disableButton(false);
				updateMoney (true);
			} else {
                if (!buyButton.isDisable()) buyButton.disableButton(true);
				updateMoney (false);
			}
			break;
		}
	}

	private void updateConstIcon ()
	{
		switch (goods.getCostType ()) {
		case PrizeType.PRIZE_MONEY:
			costIcon.spriteName = "icon_money";
			oldCOnstIcion.spriteName="icon_money";
			break;
		case PrizeType.PRIZE_RMB:
			costIcon.spriteName = "rmb";
			oldCOnstIcion.spriteName="rmb";
			break;
		case PrizeType.PRIZE_PROP:
			costIcon.spriteName = StringKit.intToFixString (goods.getCostToolSid ()) + "icon";
			oldCOnstIcion.spriteName=StringKit.intToFixString (goods.getCostToolSid ()) + "icon";
			break;
		}
	}

	private void updateMoney (bool bl)
	{
		if (bl) {
            bottomLabel.text = "[FFFFFF]" + goods.getCostPrice().ToString() + "[-]";
			OldConst.text =goods.getOfferNum ().ToString ();
			red_line.transform.localPosition = OldConst.transform.localPosition;
			red_line.width = bottomLabel.width;
		} else {
            bottomLabel.text = "[FFFFFF]" + goods.getCostPrice().ToString() + "[-]";
			OldConst.text =goods.getOfferNum ().ToString ();
			red_line.transform.localPosition = OldConst.transform.localPosition;
			red_line.width = bottomLabel.width;
		}
	}

	private void showGoodsTex ()
	{
		if (itemIcon != null) {
			itemIcon.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + goods.getIconId (), itemIcon);
		}
	}

	
	public void update (MessageHandle msg)
	{ 
		ShopManagerment.Instance.updateTeHuiGood (goods.sid,goods.showIndex);
		(fatherWindow as ShopWindow).UpdateContent ();
		//updateGoods (goods, shopUpdate, intoType);
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
			UiManager.Instance.openDialogWindow<PropAttrWindow> ((win) => {
				win.Initialize (prop);
			});
		} else if (goods.getGoodsType () == GoodsType.STARSOUL) {		
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (goods.getGoodsSid ()); 
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> ((win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
		} 
	}
}
