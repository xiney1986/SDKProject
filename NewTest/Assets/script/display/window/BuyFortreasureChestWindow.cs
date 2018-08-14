using UnityEngine;
using System.Collections;

/**
 * 购买钥匙窗口
 * @author longlingquan
 * */
public class BuyFortreasureChestWindow : WindowBase
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
	public UITexture bg;//背景
	public UILabel nameBg;//名称描述
	public UILabel nameDecc;//名称描述1
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
	
	public class BuyStruct
	{
		public string titleTextName;
		public int goodsBgId;
		public string iconId;
		public int unitPrice;
	}
	
	protected override void begin ()
	{
		base.begin (); 
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
		if (obj.GetType () == typeof(Goods)) {
			Goods good = item as Goods;
			Prop p=PropManagerment.Instance.createProp(good.getGoodsSid());
			if(p!=null){
				nameBg.text=QualityManagerment.getQualityColor( p.getQualityId ()) +p.getName ()+"[-]";
				nameDecc.text=LanguageConfigManager.Instance.getLanguage("s03l6",now.ToString(),p.getName());
			}
			titleText.text = (obj as Goods).getName ();
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + good.getIconId (), goodsTexture);
			goodsTexture.gameObject.SetActive (true);
			goodsBg.gameObject.SetActive (true);
			if (good.getGoodsType () == PrizeType.PRIZE_EQUIPMENT) {
				goodsBg.spriteName = QualityManagerment.qualityIDToIconSpriteName (EquipmentSampleManager.Instance.getEquipSampleBySid (good.getGoodsSid ()).qualityId);
			} else if (good.getGoodsType () == PrizeType.PRIZE_PROP) {
				goodsBg.spriteName = QualityManagerment.qualityIDToIconSpriteName (PropSampleManager.Instance.getPropSampleBySid (good.getGoodsSid ()).qualityId);
			}
		} 
	}
	
	void updateCoinIcon ()
	{
		//-1无消耗
		if (costType < 0) {
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
			costStr = "";
			break;
		case PrizeType.PRIZE_STARSOUL_DEBRIS:
			costStr = LanguageConfigManager.Instance.getLanguage ("s0466");
			costIcon.spriteName = constResourcesPath.STARSOUL_DEBRIS;
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
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		if (gameObj.name == "close") {
			msg.msgEvent = msg_event.dialogCancel;
			this.dialogCloseUnlockUI=true;
			finishWindow ();
			//callback (msg);
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
			if (item.GetType () == typeof(Goods)) {
				if (!canBuy ()) {
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
					msg.msgEvent = msg_event.dialogCancel;
					UiManager.Instance.openDialogWindow<MessageWindow> (
						(win) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, str + "," + LanguageConfigManager.Instance.getLanguage ("s0207"), null);
					});
				}
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
		}
		return false;	
	}
	
	private bool checkSotreFull ()
	{
		if (item.GetType () == typeof(Goods)) {
			Goods good = item as Goods;
			GoodsSample sample = GoodsSampleManager.Instance.getGoodsSampleBySid (good.sid);
			if (sample.goodsType == GoodsType.TOOL) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0196"));
				//若果临时仓库有东西时，不能打开宝箱，并飘字提示玩家
				if (StorageManagerment.Instance.getAllTemp ().Count > 0)
				{
					UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull_temp_tip"));
					});
					return true;
				}
				return StorageManagerment.Instance.isPropStorageFull (good.getGoodsSid ());
			} 
		}
		return false;
	}
	/// <summary>
	/// justShowNum 是否显示消耗为:几个物品,而不是对应物品的价格
	/// </summary>
	public void calculateTotal ()
	{
		if (msg.msgInfo.GetType () == typeof(Goods) || item.GetType () == typeof(NoticeActiveGoods)) {
			Goods good = msg.msgInfo as Goods;
			totalMoney.text = (now * good.getCostPrice ()).ToString ();
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
		} else if (msg.msgInfo.GetType () == typeof(LaddersChallengePrice)) {
			LaddersChallengePrice are = msg.msgInfo as LaddersChallengePrice;
			totalMoney.text = are.getPrice (now).ToString ();
		}
	}
}
