using UnityEngine;
using System.Collections;

/**
 * 购买商品窗口
 * @author longlingquan
 * */
public class BuyWindow : WindowBase
{
	public GameObject goodsViewProfab;
	public GameObject goodsPoint;
	public UISprite costIcon;//花费类型图标
	public UITexture goodsTexture;//物品图标
	public UISprite scrapBg;//碎片标记
	public UISprite goodsBg;
	public UILabel totalMoney;//花费总价
	public UILabel titleText;//标题
	public UILabel numberText;//数量
	public UILabel[] exchangeLabel;//兑换提示
    public UILabel currentHaveNum;//当前持有的数量
	public UISlider slider;
//	private int consume = 0;//消耗值 
	private int max = 0;//允许使用道具最大数量
	private int min = 0;//允许使用道具最小数量
	private int  setp = 1;//每点一次的+-数量
	private int  now = 1;//当前的设置数
	private CallBackMsg callback;
	MessageHandle msg;
	private object item;
	private string str;
	private int costType;
	private string costStr;
	//兑换提示
	private string[] exchangeNames;
	private int[]	exchangeValues;

	public class BuyStruct
	{
		public string titleTextName;
		public int goodsBgId;
		public string iconId;
		public int unitPrice;
	}
    /// <summary>
    /// 用于恶魔挑战挑战次数购买
    /// </summary>
    public class BossAttackTimeBuyStruct
    {
        public string descTime;//购买次数提示
        public string descExtraGet;//徽记获得提示
        public int goodsBgId;
        public string iconId;
        public int unitPrice;
    }

    protected override void begin ()
	{
		base.begin ();
	    if (fatherWindow is GodsWarShopWindow) UiManager.Instance.godsBuyWind = this;
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
		//	ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "backGround_2",bg);
	}
	
	//有大小限制的选择
	public void init (object obj, int numberMax, int numberMin, int costType, CallBackMsg callback)
	{ 
		init (obj, numberMax, numberMin, 1, 1, costType, callback); 
	}

	//最脑残的选择
	public void init (object obj, int costType, CallBackMsg callback)
	{ 
		init (obj, 100, 1, 1, 1, costType, callback); 
	}

	//范围缩放到0,1空间为了slider
	void coverDistanceToOne ()
	{
		if (max - min == 0) {
			slider.gameObject.SetActive (false);
			return;
		}
		float a = (float)now / max;
		if (slider == null)
			return;
		slider.value = a;
	}

	public void onSliderChange ()
	{
		int a = Mathf.CeilToInt (slider.value * max);
		changeNum (a);
		calculateTotal ();
	}

	void changeNum (int newValue)
	{
		if (newValue > max)
			newValue = max;
		if (newValue < min)
			newValue = min;
		now = newValue;
		updateDisplayeNumber ();	
	}

	//最原始的初始化
	public void init (object obj, int numberMax, int numberMin, int numberNow, int numberSetp, int costType, CallBackMsg callback)
	{
		msg = new MessageHandle ();
		msg.msgInfo = obj;
		item = obj;
		this.callback = callback;
		setp = numberSetp;
		max = numberMax;
		min = numberMin;
		now = numberNow;
		coverDistanceToOne ();
		this.costType = costType;
		updateCoinIcon ();
		updateDisplayeNumber ();
		calculateTotal ();
		goodsTexture.gameObject.SetActive (false);
		goodsBg.gameObject.SetActive (false);
		GoodsView tmpGoodsView = CreateGoodsView ();
        titleText.color = Color.white;
        currentHaveNum.color = Color.white;
		if (obj.GetType () == typeof(Goods) || obj.GetType () == typeof(NoticeActiveGoods)) {
			Goods good = item as Goods;
			tmpGoodsView.init (good.getGoodsType (),good.getGoodsSid (),good.getGoodsShowNum ());
            titleText.text = (obj as Goods).getName();
            if (good.getGoodsType() == GoodsType.EQUIP) {
                currentHaveNum.text = LanguageConfigManager.Instance.getLanguage("pveUse09", StorageManagerment.Instance.getEquipsBySid(good.getGoodsSid()).Count.ToString());  
            } else if (good.getGoodsType() == GoodsType.TOOL) {
                currentHaveNum.text = LanguageConfigManager.Instance.getLanguage("pveUse09", StorageManagerment.Instance.getProp(good.getGoodsSid()) == null ? "0" : StorageManagerment.Instance.getProp(good.getGoodsSid()).getNum().ToString());            
            }
		} else if (obj.GetType () == typeof(Prop)) {
			Prop prop = item as Prop;
			tmpGoodsView.init (prop);
			titleText.text = prop.getName ();
		} else if (obj.GetType () == typeof(Exchange) || obj.GetType () == typeof(NewExchange)) {
			ExchangeSample sample = (obj as Exchange).getExchangeSample ();
			now = numberMax;
			tmpGoodsView.init (sample.type,sample.exchangeSid,sample.num);
            titleText.text = tmpGoodsView.showName;
		} else if (obj.GetType () == typeof(ArenaChallengePrice)) {
			ArenaChallengePrice are = obj as ArenaChallengePrice;
            titleText.text = are.getName();
			ResourcesManager.Instance.LoadAssetBundleTexture (are.getIconPath (), goodsTexture);
			goodsTexture.gameObject.SetActive (true);
			goodsBg.gameObject.SetActive (true);
			tmpGoodsView.gameObject.SetActive (false);
		} else if (obj.GetType () == typeof(BuyStruct)) {
			BuyStruct buyStruct = obj as BuyStruct;
            titleText.text = buyStruct.titleTextName;
			ResourcesManager.Instance.LoadAssetBundleTexture (buyStruct.iconId, goodsTexture);
			goodsTexture.gameObject.SetActive (true);
			if (buyStruct.goodsBgId != 0) {
				goodsBg.spriteName = QualityManagerment.qualityIDToIconSpriteName (buyStruct.goodsBgId);
			}
			goodsBg.gameObject.SetActive (true);
			tmpGoodsView.gameObject.SetActive (false);
        } else if (obj.GetType() == typeof(BossAttackTimeBuyStruct))
		{
            BossAttackTimeBuyStruct buyStruct = obj as BossAttackTimeBuyStruct;
            titleText.text = buyStruct.descExtraGet;
		    titleText.color = Color.red;
		    currentHaveNum.text = buyStruct.descTime;
		    currentHaveNum.color = Color.red;
            ResourcesManager.Instance.LoadAssetBundleTexture(buyStruct.iconId, goodsTexture);
            goodsTexture.gameObject.SetActive(true);
            if (buyStruct.goodsBgId != 0) {
                goodsBg.spriteName = QualityManagerment.qualityIDToIconSpriteName(buyStruct.goodsBgId);
            }
            goodsBg.gameObject.SetActive(true);
            tmpGoodsView.gameObject.SetActive(false);
		}
		else if (obj.GetType() == typeof (LaddersChallengePrice))
		{
		    LaddersChallengePrice are = obj as LaddersChallengePrice;
		    titleText.text = are.getName();
		    ResourcesManager.Instance.LoadAssetBundleTexture(are.getIconPath(), goodsTexture);
		    goodsTexture.gameObject.SetActive(true);
		    goodsBg.gameObject.SetActive(true);
		    tmpGoodsView.gameObject.SetActive(false);
		}
		else if (obj.GetType() == typeof (ActivityChapter))
		{
		    ActivityChapter chapter = obj as ActivityChapter;
		    ResourcesManager.Instance.LoadAssetBundleTexture(constResourcesPath.TIMES_ICONPATH, goodsTexture);
		    goodsTexture.gameObject.SetActive(true);
		    tmpGoodsView.gameObject.SetActive(false);
		}


	}

	/// <summary>
	/// 创建GoodsView
	/// </summary>
	private GoodsView CreateGoodsView ()
	{
		Utils.DestoryChilds (goodsPoint);
		GameObject obj = NGUITools.AddChild (goodsPoint, goodsViewProfab) as GameObject;
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		return view;
	}

	/// <summary>
	/// 创建星魂条目
	/// </summary>
	/// <param name="starSoul">星魂</param>
	private GameObject CreateStarSoulItem (int sid)
	{
		if (goodsPoint.transform.childCount == 0) {
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (sid);
			GameObject obj = NGUITools.AddChild (goodsPoint, goodsViewProfab) as GameObject;
			GoodsView view = obj.transform.GetComponent<GoodsView> ();
			view.init (starSoul, GoodsView.BOTTOM_TEXT_NAME);
			view.fatherWindow = fatherWindow;
			view.onClickCallback = () => {
				UiManager.Instance.openDialogWindow<StarSoulAttrWindow> (
					(win) => {
					win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
				});
			};
			return obj;
		}
		return null;
	}

	void updateCoinIcon ()
	{
		//-1无消耗
		if (costType < 0){ //|| costType == PrizeType.PRIZE_PVE) {
			costIcon.gameObject.SetActive (false);
			totalMoney.gameObject.SetActive (false);
			costStr = "";
			return;
		}
		costIcon.gameObject.SetActive (true);
		switch (costType) {

		case PrizeType.PRIZE_RMB:
			costStr = LanguageConfigManager.Instance.getLanguage ("s0048");
			costIcon.spriteName = constResourcesPath.RMBIMAGE;
			break;
		case PrizeType.PRIZE_MONEY:
			costStr = LanguageConfigManager.Instance.getLanguage ("s0049");
			costIcon.spriteName = constResourcesPath.MONEYIMAGE;
			break;			
		case PrizeType.PRIZE_CONTRIBUTION:
			costStr = LanguageConfigManager.Instance.getLanguage ("Guild_57");
			costIcon.gameObject.SetActive (false);
			break;
		case PrizeType.PRIZE_MERIT:
			costStr = LanguageConfigManager.Instance.getLanguage ("Arena06");
			costIcon.spriteName = constResourcesPath.MERITIMAGE;
			break;
		case PrizeType.PRIZE_PROP:
			costIcon.gameObject.SetActive (false);
			totalMoney.gameObject.SetActive (false);
            if (item.GetType() == typeof(Prop)) {//使用行动剂走这里
                Prop prop = item as Prop;
                if (prop.getType() == PropType.PROP_TYPE_PVE) {
                    costStr = "";
                    break;
                }
            }
			Goods goods = item as Goods;
			GoodsSample gs = GoodsSampleManager.Instance.getGoodsSampleBySid(goods.sid);
			Prop p = PropManagerment.Instance.createProp(gs.costToolSid);
			if(p.getType() == PropType.PROP_GODSWAR_MONEY){
				costIcon.gameObject.SetActive(true);
				totalMoney.gameObject.SetActive (true);
				costStr = LanguageConfigManager.Instance.getLanguage ("godsWar_129");
				costIcon.spriteName = constResourcesPath.GODSWARMONEY_IMAGE; 
			}
			else if(p.getType() == PropType.PROP_HUIJI)
			{
				costIcon.gameObject.SetActive(true);
				totalMoney.gameObject.SetActive (true);
				costStr = LanguageConfigManager.Instance.getLanguage ("OneOnOneBoss_HuiJi");
				costIcon.spriteName = constResourcesPath.HUIJI_ICON; 
			}
			else if(p.getType() == PropType.PROP_JUNGONG)
			{
				costIcon.gameObject.SetActive(true);
				totalMoney.gameObject.SetActive (true);
				costStr = LanguageConfigManager.Instance.getLanguage ("junGong");
				costIcon.spriteName = constResourcesPath.JUNGONG_ICON; 
			}
			costStr = ""+p.getName();
			break;
		case PrizeType.PRIZE_STARSOUL_DEBRIS:
			costStr = LanguageConfigManager.Instance.getLanguage ("s0466");
			costIcon.spriteName = constResourcesPath.STARSOUL_DEBRIS;
			break;
		case PrizeType.PRIZE_LEDDER_SCORE:
			costStr = LanguageConfigManager.Instance.getLanguage ("laddermoney");
			costIcon.spriteName = constResourcesPath.LADDER_MONEY;
			break;
		case PrizeType.PRIZE_STAR_SCORE:
			costStr = LanguageConfigManager.Instance.getLanguage ("star_star");
			costIcon.spriteName = constResourcesPath.STAR_STAR;
			break;
		}
	}
	//数字滚动响应
	public void numberFly (bool isAdd)
	{
		if (isAdd) {
			addNumber ();
		} else {
			reduceNumber ();
		}
		calculateTotal ();
	}
	
	public override void DoDisable ()
	{

		base.DoDisable (); 
//		if (callback != null)
//			callback (msg);
	}

	private void addNumber ()
	{
		if (now >= max) {
			updateDisplayeNumber ();	
		} else {
			now += setp;
			updateDisplayeNumber ();
		}
		coverDistanceToOne ();

	}

	private void reduceNumber ()
	{
		if (now <= min) {
			updateDisplayeNumber ();
		} else {
			now -= setp;
			updateDisplayeNumber ();
		}

		coverDistanceToOne ();

	}

	private void updateDisplayeNumber ()
	{
		numberText.text = now + "";
		msg.msgNum = now;
		showExchangeTips (now);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		if (gameObj.name == "close") {
			msg.msgEvent = msg_event.dialogCancel;
			this.dialogCloseUnlockUI=true;
			finishWindow ();
			callback (msg);
		}
		//最小值
		else if (gameObj.name == "min") {
			now = min;
			updateDisplayeNumber ();
			coverDistanceToOne ();
			MaskWindow.UnlockUI ();
		}
		//最大值
		else if (gameObj.name == "max") {
			now = max;
			updateDisplayeNumber ();
			coverDistanceToOne ();
			MaskWindow.UnlockUI ();
		}
		//加
		else if (gameObj.name == "add") {
			addNumber ();
			MaskWindow.UnlockUI ();
		}
		//减
		else if (gameObj.name == "reduce") {
			reduceNumber ();
			MaskWindow.UnlockUI ();
		}
		calculateTotal ();
		//选定确认
		if (gameObj.name == "buttonOk") { 
			GuideManager.Instance.doGuide (); 
			if (item.GetType () == typeof(Goods) || item.GetType () == typeof(NoticeActiveGoods)) {
				if (!canBuy ()) {
					//MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage ("s0355", costStr));
					if (costType == PrizeType.PRIZE_RMB) {
						finishWindow ();
						EventDelegate.Add (OnHide, () => {
							MessageWindow.ShowRecharge (LanguageConfigManager.Instance.getLanguage ("s0355", costStr));
						});
					} else
						UiManager.Instance.openDialogWindow<MessageWindow> (
					      (win) => {
							win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0355", costStr), null);
						});
					return;
				}
				if (!checkSotreFull ()) {
					msg.msgEvent = msg_event.dialogOK;
				} else {
					UiManager.Instance.openDialogWindow<MessageWindow> (
				 (win) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, str + "," + LanguageConfigManager.Instance.getLanguage ("s0207"), null);
					});

				}
			} else {
				if (item.GetType () == typeof(Prop)) {
					Prop prop = item as Prop;
					PropSample sample = PropSampleManager.Instance.getPropSampleBySid (prop.sid);
					//精灵宝箱
					if (sample.sid == 71070) {
						if (StorageManagerment.Instance.isRoleStorageFull (StringKit.toInt (numberText.text))) {
							str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("cardName"));
							MessageWindow.ShowAlert (str);
							return;
						}
					}
                } else if (item.GetType() == typeof (BuyStruct))
                {
                    msg.msgNum = now;
                } else if (item.GetType() == typeof (BossAttackTimeBuyStruct))
                    msg.msgNum = now;
			    msg.msgEvent = msg_event.dialogOK;
			}
			finishWindow ();
			EventDelegate.Add (OnHide, () => {
				callback (msg);
			});
		} 
		//MaskWindow.UnlockUI ();
	}
	
	private bool canBuy ()
	{
		switch (costType) {
		case PrizeType.PRIZE_RMB:
			if (UserManager.Instance.self.getRMB () >= StringKit.toInt (totalMoney.text))
				return true;
			break;
		case PrizeType.PRIZE_MONEY:
			if (UserManager.Instance.self.getMoney () >= StringKit.toInt (totalMoney.text))
				return true;
			break;	
		case PrizeType.PRIZE_CONTRIBUTION:
			if (GuildManagerment.Instance.getGuild ().contributioning >= StringKit.toInt (totalMoney.text))
				return true;
			break;
		case PrizeType.PRIZE_STARSOUL_DEBRIS:
			if (StarSoulManager.Instance.getDebrisNumber () >= StringKit.toInt (totalMoney.text))
				return true;
			break;
		case  PrizeType.PRIZE_MERIT:
			return UserManager.Instance.self.merit >= StringKit.toInt (totalMoney.text);
		case PrizeType.PRIZE_LEDDER_SCORE:
			return LadderHegeMoneyManager.Instance.myPort >= StringKit.toInt (totalMoney.text);
			break;
        case PrizeType.PRIZE_PROP:
            Goods goods = item as Goods;
            GoodsSample gs = GoodsSampleManager.Instance.getGoodsSampleBySid(goods.sid);
            Prop p = StorageManagerment.Instance.getProp(gs.costToolSid);
            if (p == null) return false;
            int propNum = p.getNum();
            return propNum >= StringKit.toInt(totalMoney.text);
            break;
            default:
            return false;
		case  PrizeType.PRIZE_STAR_SCORE:
			return GoddessAstrolabeManagerment.Instance.getStarScore() >= StringKit.toInt (totalMoney.text);

		}
		return false;	
	}
	
	private bool checkSotreFull ()
	{
		if (item.GetType () == typeof(Goods) || item.GetType () == typeof(NoticeActiveGoods)) {
			Goods good = item as Goods;
			GoodsSample sample = GoodsSampleManager.Instance.getGoodsSampleBySid (good.sid);
			if (sample.goodsType == GoodsType.CARD) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("cardName"));
				return StorageManagerment.Instance.isRoleStorageFull (StringKit.toInt (numberText.text));
			} else if (sample.goodsType == GoodsType.EQUIP) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0195"));
				return StorageManagerment.Instance.isEquipStorageFull (StringKit.toInt (numberText.text));
			} else if (sample.goodsType == GoodsType.TOOL) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0196"));
				return StorageManagerment.Instance.isPropStorageFull (good.getGoodsSid ());
			} else if (sample.goodsType == GoodsType.STARSOUL) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0467"));
				return StorageManagerment.Instance.isStarSoulStorageFull (StringKit.toInt (numberText.text));
			}
		}
		return false;
	}
    public string getTotalCost(Goods good,int num,int type)
    {
        int reCost = 0;
        for (int i = 0; i < num;i++ ) 
        {
            if (type == ShopType.LADDER_HEGOMONEY) reCost += good.getCostPriceForBuyWindow(i)/good.getGoodsShowNum();
            else reCost += good.getCostPriceForBuyWindow(i);
        }
        return reCost.ToString();
    }
	/// <summary>
	/// justShowNum 是否显示消耗为:几个物品,而不是对应物品的价格
	/// </summary>
	public void calculateTotal ()
	{
		if (msg.msgInfo.GetType () == typeof(Goods) || item.GetType () == typeof(NoticeActiveGoods)) {
			Goods good = msg.msgInfo as Goods;
            //if (good.getGoodsShopType() == ShopType.LADDER_HEGOMONEY)
            //{
            //    totalMoney.text = (now *  (good.getCostPrice ()/good.getGoodsShowNum())).ToString ();
            //} else {
            //   //totalMoney.text = (now * good.getCostPrice ()).ToString ();
            //    totalMoney.text = getTotalCost(good, now);
            //}
            totalMoney.text = getTotalCost(good, now, good.getGoodsShopType());
		}
		//这里添加多样性 
        else if (msg.msgInfo.GetType () == typeof(Prop)) {
			//使用道具不显示cost
		} else if (msg.msgInfo.GetType () == typeof(ArenaChallengePrice)) {
			ArenaChallengePrice are = msg.msgInfo as ArenaChallengePrice;
			totalMoney.text = are.getPrice (now).ToString ();
		} else if (msg.msgInfo.GetType () == typeof(BuyStruct)) {
			BuyStruct buyStruct = msg.msgInfo as BuyStruct;
			totalMoney.text = (buyStruct.unitPrice * now).ToString ();
        } else if (msg.msgInfo.GetType() == typeof(BossAttackTimeBuyStruct)) {
            BossAttackTimeBuyStruct buyStruct = msg.msgInfo as BossAttackTimeBuyStruct;
            totalMoney.text = (buyStruct.unitPrice * now).ToString();
        } else if (msg.msgInfo.GetType() == typeof(LaddersChallengePrice)) {
			LaddersChallengePrice are = msg.msgInfo as LaddersChallengePrice;
			totalMoney.text = are.getPrice (now).ToString ();
		} else if (msg.msgInfo.GetType() == typeof(ActivityChapter)) {
            ActivityChapter _chapter = msg.msgInfo as ActivityChapter;
            int[] prises = GoodsBuyCountManager.Instance.getSampleByGoodsSid(_chapter.sid).prise;
            int rmb = prises.Length <= _chapter.getReBuyNum() ? prises[prises.Length - 1] : prises[_chapter.getReBuyNum()];
            totalMoney.text = ((now * (rmb + (rmb + (now-1)*(prises[1] - prises[0])))))/2 + "";
            msg.costNum = StringKit.toInt(totalMoney.text);
            msg.msgNum = now;
        }
	}
	///<summary>
	/// 清除兑换提示
	/// </summary>
	public void cleanExchangeTips () {
		for (int i=0; i < exchangeLabel.Length; i++)
			exchangeLabel [i].text = "";
	}
	/// <summary>
	/// 设置兑换提示内容
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="value">Value.</param>
	public void setExchangeTipsContent(string name, string value){
		exchangeNames = name.Split ('#');
		string[] tmp = value.Split ('#');
		exchangeValues = new int[tmp.Length];
		for (int i=0; i<tmp.Length; i++)
			exchangeValues [i] = StringKit.toInt (tmp [i]);
	}
	///<summary>
	/// 根据兑换数量设置提示值
	/// </summary>
	public void showExchangeTips(int value){
		if (!(fatherWindow is ExChangeWindow))
			return;
		string prefix = LanguageConfigManager.Instance.getLanguage ("equipStar05");
		for (int i=0; i<exchangeValues.Length; i++) {
			exchangeLabel[i].text =  prefix + exchangeNames[i] + "x" + exchangeValues[i]*value;
		}
	}
    void OnDestroy() {
        UiManager.Instance.godsBuyWind = null;
    }
}
