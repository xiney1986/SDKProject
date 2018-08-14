using UnityEngine;
using System.Collections;

/// <summary>
/// 节日礼花点燃窗口
/// </summary>
public class FireworksFireWindow : WindowBase {
	/** 礼花sid*/
	private int sid;
	/** 点燃按钮*/
	public ButtonBase wishButton;
	/** 制作数量滑条*/
	public UISlider slider;
	/** 道具描述*/
	public UILabel descLabel;
	/** 点燃数量*/
	public UILabel numberText;
	/** 消耗道具icon*/
	public GoodsView prop;
	/**允许使用道具最大数量*/
	private int max = 0;//
	/**最小值*/
	private int min = 0;
	/**每点一次的+-数量*/
	private int setp = 1;
	/**当前的设置数*/
	private int now = 1;
	private CallBack<int,Prop> callback;
	/**兑换提示*/
	private string[] exchangeNames;
	private int[]	 exchangeValues;

	Prop propPrize;
	FestivalFireworksSample sample;
	ExchangeSample exchange;

	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	//有大小限制的选择
	public void init (FestivalFireworksSample sample,CallBack<int,Prop> callback)
	{ 
		this.callback = callback;
		this.sid = sample.prizes.pSid;
		this.descLabel.text = sample.exchangeSample.describe;
		this.max = getNumberMax();
		this.min = getNumberMin();
		initIcon();
	}

	/// <summary>
	/// 获取最大可点燃的值
	/// </summary>
	private int getNumberMax()
	{
		int max = 50;

		int _max = getPropsNum(sid);
	    max = _max > max ? max : _max;
		return max;
	}

	/// <summary>
	/// 得到拥有道具的数量
	/// </summary>
	public int getPropsNum(int _sid)
	{
		Prop s;
		int num = 0;
		ArrayList list = StorageManagerment.Instance.getPropsBySid(_sid);
		for(int j=0;j<list.Count;j++)
		{
			s = list[j] as Prop;
			this.propPrize = s;
			num = s.getNum();
		}
		return num;
	}
	/// <summary>
	/// 获取最小可点燃的值
	/// </summary>
	private int getNumberMin()
	{
		int min = 0;
		int _max = getPropsNum(sid);
		min = _max == 0 ?  0 : 1;
		return min;	
	}

	/// <summary>
	/// 获取消耗商品类型
	/// </summary>
	private int getCostType()
	{
		return exchange.type;
	}
	#region sliderChange
	/// <summary>
	/// 范围缩放到0,1空间为了slider
	/// </summary>
	void coverDistanceToOne ()
	{
		if (max - min == 0) {
			//slider.gameObject.SetActive (false);
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
	#endregion

	/// <summary>
	/// 初始化icon(GoodsView)
	/// </summary>
	private void initIcon ()
	{
		Prop s;
		ArrayList list = StorageManagerment.Instance.getPropsBySid(sid);
		if(list.Count==0)
		{
			s = PropManagerment.Instance.createProp(sid);
			prop.init (s);
			prop.rightBottomText.text = "x"+getPropsNum(sid).ToString(); 
			return;
		}
		for(int j=0;j<list.Count;j++)
		{
			s = list[j] as Prop;
			if(s!=null)
			{
				prop.init (s);
				prop.rightBottomText.text = "x"+getPropsNum(sid).ToString(); 
			}				
		}
	}

	/// <summary>
	/// 更新显示数量
	/// </summary>
	private void updateDisplayeNumber ()
	{
		numberText.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFF_12",now.ToString());
		showExchangeTips (now);
	}

	///<summary>
	/// 根据兑换数量设置提示值
	/// </summary>
	public void showExchangeTips(int value){
		if (!(fatherWindow is NoticeWindow))
			return;
	}

	/// <summary>
	/// 点击事件
	/// </summary>
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close")
		{
			this.dialogCloseUnlockUI=true;
		}
		else if(gameObj.name=="buttonFire")
		{
			PropSample sample = PropSampleManager.Instance.getPropSampleBySid (propPrize.sid);	
			if (sample.type == PropType.PROP_TYPE_CHEST) {
				//若果临时仓库有东西时，不能打开宝箱，并飘字提示玩家
				if (StorageManagerment.Instance.getAllTemp ().Count > 0) {
					UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull_temp_tip"));
					});
					return;
				}
				//如果数量只有一个 则直接使用,不用去选择数量
				if (now > 0) {
					if(callback!=null)
					{
						this.dialogCloseUnlockUI=false;
						callback (now,propPrize);
					}
				}
				
			} else if (sample.type == PropType.PROP_TYPE_LOCK_CHEST) {//带锁的宝箱打开界面
				UiManager.Instance.openWindow<TreasureChestWindow> ((win) => {
					win.init (propPrize.sid);
				});
			}
		}
		finishWindow();
	}
}
