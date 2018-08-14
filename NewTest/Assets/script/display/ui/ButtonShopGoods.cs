using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/**
 *  商店按钮节点
 * */
public class ButtonShopGoods : ButtonBase {
	public GameObject goodsViewProfab;
	public GameObject goodsPoint;
	public UISprite spriteScrap;
	public UILabel topLabel;
	public UILabel bottomLabel;
	public UISprite costIcon;
	public UISprite icon_backGround;

	[System.Serializable]
	public class LadderHegoMoney {
		public UISprite hideSprite;
		public UISprite scrapSprite;
		public UILabel showPointLabel;
		public UILabel showLimitLabel;
	}

	public LadderHegoMoney ladderHeSetting;

	/**关于物品的文字描述 */
	public UILabel infoDec;
	public UITexture itemIcon;
	public UILabel buyTime;
	public UILabel goodsNum;
    public UILabel showNum;
	public ButtonGoods buyButton;
	public Goods goods;
	private CallBack shopUpdate;
	private Timer timer;//倒计时
	public const int RMBSHOP = 1;//RMB商店
	public const int GUILDSHOP = 3;//公会商店
	public const int SUPERDRAWSHOP = 23;//超级奖池商店
	public const int GODSWARSHOP = 24;//诸神战商店
    public const int HEROSYMBOL_SHOP = 25;//恶魔挑战商店
	public const int MERIT_SHOP = 4;//功勋商店
	public const int STARSOUL_DEBRIS_SHOP = 5;//星魂碎片商店
	public const int LADDER_HEGOMENOY = 7;//天梯积分商店
	private int intoType = 0;//进入类型
	private const int STRING_LENGTH = 17;//截取字符串长度
	public GameObject timePoint;
    public GameObject stars;//星星
	public const int LASTBATTLESHOP = 26;// 末日决战军功商店//
	public const int STARSHOP = 27;// 星屑商店//
	
	public void buy (MessageHandle msg) {
		if (msg.msgEvent == msg_event.dialogCancel)
			return;
		goods = msg.msgInfo as Goods;
		if (intoType == RMBSHOP) {
			BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, buyCallBack);
		}
		else if (intoType == GUILDSHOP) {
			GuildShopBuyFPort fport = FPortManager.Instance.getFPort ("GuildShopBuyFPort") as GuildShopBuyFPort;
			fport.access ((msg.msgInfo as Goods).sid, msg.msgNum, buyCallBack);
		}
		else if (intoType == MERIT_SHOP) {
			BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, buyCallBack);
		}
		else if (intoType == STARSOUL_DEBRIS_SHOP) {
			BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, ShopType.STARSOUL_DEBRIS, buyCallBack);
		}
		else if (intoType == LADDER_HEGOMENOY) {
			BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, ShopType.LADDER_HEGOMONEY, buyLadderBack);
		}
		else if (intoType == SUPERDRAWSHOP) {
			BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, ShopType.SUPERDRAW_SHOP, buyCallBack);
		}
		else if (intoType == GODSWARSHOP) {
			BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, ShopType.GODSWAR_SHOP, buyCallBack);
        } else if (intoType == HEROSYMBOL_SHOP) {
            BuyGoodsFPort fport = FPortManager.Instance.getFPort("BuyGoodsFPort") as BuyGoodsFPort;
            fport.buyGoods((msg.msgInfo as Goods).sid, msg.msgNum, ShopType.HEROSYMBOL_SHOP, buyCallBack);
		}
		else if (intoType == LASTBATTLESHOP)
		{
			BuyGoodsFPort fport = FPortManager.Instance.getFPort("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods((msg.msgInfo as Goods).sid, msg.msgNum, ShopType.JUNGONG_SHOP, buyCallBack);
		}
		else if(intoType == STARSHOP)
		{
			BuyGoodsFPort fport = FPortManager.Instance.getFPort("BuyGoodsFPort") as BuyGoodsFPort;
			fport.buyGoods((msg.msgInfo as Goods).sid, msg.msgNum, ShopType.STAR_SHOP, buyCallBack);
		}
	}

	private void buyLadderBack (int sid, int num) { 
		goods.nowBuyNum += num;
		int ladderpoint = LadderHegeMoneyManager.Instance.myPort - num * goods.getCostPrice ();
		LadderHegeMoneyManager.Instance.myPort = ladderpoint >= 0 ? ladderpoint : 0;
		if (fatherWindow is NoticeWindow) {
			(fatherWindow as NoticeWindow).show.GetComponent<NoticeLadderHegeMoneyContent> ().updatePoint ();
		}
		UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
			win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0056", goods.getName (), (num * goods.getGoodsShowNum ()).ToString ()));
			update (null);
		});
	}

    private void showStar(int level){
        for (int i = 0; i < level; i++) {
            stars.transform.GetChild(i).gameObject.SetActive(true);
        }
        if (level == CardSampleManager.ONESTAR) {
            stars.transform.localPosition = new Vector3(33, -35, 0);
        } else if (level == CardSampleManager.TWOSTAR) {
            stars.transform.localPosition = new Vector3(22, -35, 0);
        } else if (level == CardSampleManager.THREESTAR) {
            stars.transform.localPosition = new Vector3(11, -35, 0);
        } else if (level == CardSampleManager.FOURSTAR) {
            stars.transform.localPosition = new Vector3(0, -35, 0);
        }
        }
	private void buyCallBack (int sid, int num) { 
		goods.nowBuyNum += num;
		UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
			win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0056", goods.getName (), (num * goods.getGoodsShowNum ()).ToString ()));
			update (null);
		});
	}
	
	public void updateGoods (Goods goods, CallBack callback, int intoType) {
		this.intoType = intoType;
		this.goods = goods;  
		this.shopUpdate = callback;
        if (stars != null) {
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
		if (goods.getShowTime () != "") {
			buyTime.text = goods.getShowTime ();
			buyTime.gameObject.SetActive (true);
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
			timer.addOnTimer (showTime);
			timer.start ();
		}
		else {
			buyTime.gameObject.SetActive (false);
		}
		timePoint.SetActive(true);
		if (intoType == MERIT_SHOP) {
			costIcon.spriteName = "icon_gongxun";
			timePoint.SetActive(true);
		}
		else if(intoType == LASTBATTLESHOP)
		{
			costIcon.spriteName = "icon_junGong";
			//timePoint.SetActive(false);
		}
		else if (intoType == GUILDSHOP) {
			costIcon.spriteName = "Contribution";
            showNum.text = "";//把数据清零
            if (goods.getGoodsShowNum() > 1) {
                showNum.gameObject.SetActive(true);
                showNum.text = "x" + goods.getGoodsShowNum();
            }
		}
		else if (intoType == SUPERDRAWSHOP) {
			buyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("superDraw_09");
			costIcon.spriteName = "jiangjuan";
			costIcon.gameObject.SetActive(true);
			costIcon.transform.localScale = new Vector3(0.7f,0.7f,1f);
			costIcon.transform.localPosition =new Vector3(34.0f,0,0);
            showNum.text = "";//把数据清零
            if (goods.getGoodsShowNum() > 1) {
                showNum.gameObject.SetActive(true);
                showNum.text = "x" + goods.getGoodsShowNum();
            }
		}
		else if (intoType == LADDER_HEGOMENOY) {

			ladderHeSetting.showPointLabel.gameObject.SetActive (true);
			ladderHeSetting.hideSprite.gameObject.SetActive (false);
			ladderHeSetting.showLimitLabel.gameObject.SetActive (true);
			ladderHeSetting.showLimitLabel.text = "X" + goods.getGoodsShowNum ();
		}

		if (goods.getGoodsMaxBuyCount () > 0 && goods.getNowBuyNum () >= goods.getGoodsMaxBuyCount ()) {
			buyButton.disableButton (true);
			goodsNum.text = LanguageConfigManager.Instance.getLanguage ("Guild_96");
		}
		else {
			buyButton.disableButton (false);
		    if (intoType == MERIT_SHOP)
		    {
		        if (UserManager.Instance.self.getUserLevel() < goods.getLimitLevel &&
		            UserManager.Instance.self.vipLevel < goods.getVipLimitLevel)
		        {
		            buyButton.disableButton(true);
		            buyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_028",
		                goods.getLimitLevel + "",goods.getVipLimitLevel+ "");
		        }
		        else
		        {
                    buyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0014");
		        }
		    }
		    buyButton.goods = this.goods;
			buyButton.callback = buy;
			buyButton.fatherWindow = fatherWindow;
			if (goods.getGoodsMaxBuyCount () == 0) {
				if(intoType == SUPERDRAWSHOP)
					goodsNum.text = LanguageConfigManager.Instance.getLanguage("superDraw_11");
				else
					goodsNum.text = LanguageConfigManager.Instance.getLanguage("s0280l1");
			}
			else {
				if(intoType == SUPERDRAWSHOP)
				{
					int lastNum = Math.Max (goods.getGoodsMaxBuyCount () - goods.getNowBuyNum (), 0);
					goodsNum.text = LanguageConfigManager.Instance.getLanguage ("superDraw_10", lastNum.ToString (), goods.getGoodsMaxBuyCount ().ToString ());
				}
				else
				{
					int lastNum = Math.Max (goods.getGoodsMaxBuyCount () - goods.getNowBuyNum (), 0);
					goodsNum.text = LanguageConfigManager.Instance.getLanguage ("s0280l0", lastNum.ToString (), goods.getGoodsMaxBuyCount ().ToString ());
				}
			}
		}
		string des = "";
		string nameColor = "";
		if ((goods.isScrap ()) && 
			goods.getGoodsShopType () == ShopType.LADDER_HEGOMONEY) {
			ladderHeSetting.scrapSprite.gameObject.SetActive (true);
		}
		if (goods.getGoodsType () == PrizeType.PRIZE_EQUIPMENT) {
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
			des = EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).desc;
			nameColor = QualityManagerment.getQualityColor (EquipmentSampleManager.Instance.getEquipSampleBySid (goods.getGoodsSid ()).qualityId);
		}
		else if (goods.getGoodsType () == PrizeType.PRIZE_PROP) {
			icon_backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (PropSampleManager .Instance.getPropSampleBySid (goods.getGoodsSid ()).qualityId);
			des = PropSampleManager.Instance.getPropSampleBySid (goods.getGoodsSid ()).describe;
			nameColor = QualityManagerment.getQualityColor (PropSampleManager.Instance.getPropSampleBySid (goods.getGoodsSid ()).qualityId);

            if (goods.isCardScrap() && stars != null) {//如果是卡片碎片，显示星星
                stars.transform.localPosition = new Vector3(0, -30, 0);
                int propSid = PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).sid;
                Card card = CardScrapManagerment.Instance.getCardByScrapSid(propSid);//根据卡片碎片id获取对应卡片
                if (card != null && CardSampleManager.Instance.getStarLevel(card.sid) != null) {
                    int cardStarLevel = CardSampleManager.Instance.getStarLevel(card.sid);//卡片星级
                    showStar(cardStarLevel);
                }
            } else if (goods.isMagicScrap() && stars != null) {
                stars.transform.localPosition = new Vector3(0, -30, 0);
                int propSid = PropSampleManager.Instance.getPropSampleBySid(goods.getGoodsSid()).sid;
                MagicWeapon magic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(propSid);
                if (magic != null && MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid) != null) {
                    int level = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid).starLevel;
                    showStar(level);
                }
            }
		}
		else if (goods.getGoodsType () == PrizeType.PRIZE_STARSOUL) {
			CreateStarSoulItem (goods.getGoodsSid ());
			des = StarSoulManager.Instance.createStarSoul (goods.getGoodsSid ()).getDescribe ();
			nameColor = QualityManagerment.getQualityColor (StarSoulManager.Instance.createStarSoul (goods.getGoodsSid ()).getQualityId ());
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
		if (goods.getGoodsShopType () == ShopType.LADDER_HEGOMONEY) {
			topLabel.text = nameColor + goods.getName ();
		}
		else {
			topLabel.text = goods.getName ();
		}

		des = des.Split ('~') [0];
		Regex r = new Regex ("\\[.+?\\]");
		MatchCollection mc = r.Matches (des);
		int index = 0;
		for (int i=0; i<mc.Count; i++) {
			if (mc [i].Index > STRING_LENGTH - 1) {
				break;
			}
			else if (mc [i].Index <= STRING_LENGTH - 1 && mc [i].Index + mc [i].Value.Length > STRING_LENGTH) {
				index += mc [i].Value.Length;
				break;
			}
			else {
				index += mc [i].Value.Length;
			}

		}
		if (des.Length > (STRING_LENGTH + index)) {
			des = des.Substring (0, STRING_LENGTH - 1 + index) + "...";
		}		
		if (infoDec != null)
			infoDec.text = des;
		if (intoType == LADDER_HEGOMENOY) {
			bottomLabel.text = goods.getCostPrice ().ToString ();
		} else if(intoType == SUPERDRAWSHOP)
		{
			bottomLabel.text = goods.getCostPrice ().ToString ();
			bottomLabel.transform.localPosition = new Vector3(71.0f,-3.0f,0);
		}
		else {
			if (fatherWindow is GuildShopWindow)
				bottomLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_1114", goods.getCostPrice ().ToString ());
			else
				bottomLabel.text = goods.getCostPrice ().ToString ();
		}
	}

	/// <summary>
	/// 创建星魂条目
	/// </summary>
	/// <param name="starSoul">星魂</param>
	private GameObject CreateStarSoulItem (int sid) {
		if (goodsPoint.transform.childCount > 0) {
			Utils.RemoveAllChild (goodsPoint.transform);		
		}
		StarSoul starSoul = StarSoulManager.Instance.createStarSoul (sid);
		GameObject obj = NGUITools.AddChild (goodsPoint, goodsViewProfab) as GameObject;
		obj.transform.localScale = new Vector3 (0.85f, 0.85f, 1);
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		view.init (starSoul, GoodsView.BOTTOM_TEXT_NONE);
		view.fatherWindow = fatherWindow;
		view.onClickCallback = () => {
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> (
				(win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
		};
		return obj;
	}
	
	private void showGoodsTex () {
		if (itemIcon != null) {
			itemIcon.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + goods.getIconId (), itemIcon);
		}
	}
	
	private void showTime () {
		buyTime.text = goods.getShowTime ();
	}
	
	public void update (MessageHandle msg) { 
		if (intoType == RMBSHOP) {
			//如果 达到可买的最大值 需要刷新商品列表
			(fatherWindow as ShopWindow).UpdateContent ();
			if (goods.getNowBuyNum () >= goods.getGoodsMaxBuyCount ()) {
				(fatherWindow as ShopWindow).UpdateContent ();
				return;
			}
		}
		else if (intoType == GUILDSHOP) {
			//刷新工会商品列表
			(fatherWindow as GuildShopWindow).updateShop ();
		}
		else if (intoType == SUPERDRAWSHOP) {
			//刷新超级奖池兑换列表
			(fatherWindow as SuperDrawShopWindow).updateShop ();
		}
		else if (intoType == GODSWARSHOP) {
			//刷新战争商店兑换列表
			(fatherWindow as GodsWarShopWindow).updateShop ();
        } else if (intoType == HEROSYMBOL_SHOP) {
            (fatherWindow as HuiJiShopWindow).updateShop();
        } else if (intoType == MERIT_SHOP) {
            (fatherWindow as MeritShopWindow).updateInfo();
            if (goods.getNowBuyNum() >= goods.getGoodsMaxBuyCount()) {
                (fatherWindow as MeritShopWindow).reloadShop();
                return;
            }
        } else if (intoType == STARSOUL_DEBRIS_SHOP) {
            (fatherWindow as StarSoulDebrisShopWindow).updateInfo();
            if (goods.getNowBuyNum() >= goods.getGoodsMaxBuyCount()) {
                (fatherWindow as StarSoulDebrisShopWindow).reloadShop();
                return;
            }
        }
		else if(intoType == LASTBATTLESHOP)
		{
			(fatherWindow as LastBattleShopWindow).updateInfo();
        } else if (intoType == STARSHOP)
        {
            (fatherWindow as StarShopWindow).updateInfo();   
        }
		updateGoods (goods, shopUpdate, intoType);
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
