using UnityEngine;
using System.Collections;

/// <summary>
/// 节日礼花制造窗口
/// </summary>
public class FireworksMakeWindow : WindowBase {

	private int sid;
	/** 制造按钮*/
	public ButtonBase wishButton;
	/** 红纸消耗*/
	public UILabel redpaperLabel;
	/** 火药消耗*/
	public UILabel powderLabel;
	/** 五彩消耗*/
	public UILabel wucaiLabel;
	/** 制作数量滑条*/
	public UISlider slider;
	/** 制作数量*/
	public UILabel numberText;
	/** 消耗道具icon*/
	public GoodsView[] props;
	/** 红纸消耗*/
	private int redpaperNum;
	/** 火药消耗*/
	private int powderNum;
	/** 五彩消耗*/
	private int wucaiNum;
	/**允许使用道具最大数量*/
	private int max = 0;//
	/**最小值*/
	private int min = 0;
	/**每点一次的+-数量*/
	private int setp = 1;
	/**当前的设置数*/
	private int now = 1;
	private CallBack<int,int> callback;

	private int materialNum;

	FestivalFireworksSample sample;
	ExchangeSample exchange;

	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	//有大小限制的选择
	public void init (FestivalFireworksSample sample,CallBack<int,int> callback)
	{ 
		this.sample = sample;
		this.callback = callback;
		this.exchange = sample.exchangeSample;
		this.sid = sample.noitceItemSid;
		int numberMax = getNumberMax ();
		int numberMin = getNumberMin ();
		int costType  = getCostType  ();
		materialNum = exchange.conditions[0].Length;
		setCurrentPropNum();
		initIcon();
		init (numberMax, numberMin, 1, 1, costType); 
	}

	/// <summary>
	/// 获取最大可制造的值
	/// </summary>
	private int getNumberMax()
	{
		int max = 99999;
		for(int i=0;i<exchange.conditions[0].Length;i++)
		{
			int _max = getPropsNum(exchange.conditions[0][i].costSid)/exchange.conditions[0][i].num;
			max = _max > max ? max : _max;
		}
		return max;
	}

	/// <summary>
	/// 得到拥有道具的数量
	/// </summary>
	/// <returns>The properties number.</returns>
	public int getPropsNum(int sid)
	{
		Prop s;
		int num = 0;
		ArrayList list = StorageManagerment.Instance.getPropsBySid(sid);
		for(int j=0;j<list.Count;j++)
		{
			s = list[j] as Prop;
			num = s.getNum();
		}
		return num;
	}
	/// <summary>
	/// 获取最小可制造的值
	/// </summary>
	private int getNumberMin()
	{
		int min = 0;
		for(int i=0;i<exchange.conditions[0].Length;i++)
		{
			int _max = getPropsNum(exchange.conditions[0][i].costSid)/exchange.conditions[0][i].num;
			min = _max == 0 ?  0 : 1;
			if(min == 0)
				return min;
		}
		return min;	
	}

	/// <summary>
	/// 获取消耗商品类型
	/// </summary>
	private int getCostType()
	{
		return exchange.type;
	}

	/// <summary>
	/// 最原始的初始化
	/// </summary>
	public void init (int numberMax, int numberMin, int numberNow, int numberSetp, int costType)
	{
		setp = numberSetp;
		max = numberMax;
		min = numberMin;
		now = numberNow;
		coverDistanceToOne ();
		updateDisplayeNumber ();
		calculateTotal ();
	}

	/// <summary>
	/// 范围缩放到0,1空间为了slider
	/// </summary>
	void coverDistanceToOne ()
	{
		if (max - min == 0 && (min==0||max==0)) {
			//slider.gameObject.SetActive (false);
			wishButton.disableButton(true);
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

	/// <summary>
	/// 计算对应物品的消耗
	/// </summary>
	public void calculateTotal ()
	{

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

	/// <summary>
	/// 初始化icon(GoodsView)
	/// </summary>
	private void initIcon ()
	{

		Prop s;
		if(materialNum ==1)
		{
			s = PropManagerment.Instance.createProp(exchange.conditions[0][0].costSid);
			props[1].init (s);
			props[1].rightBottomText.text = "x"+getPropsNum(sid).ToString();

			props[0].gameObject.SetActive(false);
			redpaperLabel.gameObject.SetActive(false);
			props[2].gameObject.SetActive(false);
			wucaiLabel.gameObject.SetActive(false);
		}
		else if(materialNum ==2)
		{
			s = PropManagerment.Instance.createProp(exchange.conditions[0][0].costSid);
			props[0].init (s);
			props[0].rightBottomText.text = "x"+getPropsNum(sid).ToString();
			props[0].transform.localPosition = new Vector3(47.0f,props[0].transform.localPosition.y,0);
			redpaperLabel.transform.localPosition = new Vector3(47.0f,redpaperLabel.transform.localPosition.y,0);

			s = PropManagerment.Instance.createProp(exchange.conditions[0][1].costSid);
			props[2].init (s);
			props[2].rightBottomText.text = "x"+getPropsNum(sid).ToString();
			props[2].transform.localPosition = new Vector3(-50.0f,props[2].transform.localPosition.y,0);
			wucaiLabel.transform.localPosition = new Vector3(-50.0f,redpaperLabel.transform.localPosition.y,0);

			props[1].gameObject.SetActive(false);
			powderLabel.gameObject.SetActive(false);
		}
		else if(materialNum >= 3)
		{
			for(int i= 0;i<props.Length;i++)
			{
				s = PropManagerment.Instance.createProp(exchange.conditions[0][i].costSid);
				props[i].init (s);
				props[i].rightBottomText.text = "x"+getPropsNum(sid).ToString();
			}
		}
	}

	/// <summary>
	/// 更新显示数量
	/// </summary>
	private void updateDisplayeNumber ()
	{
		numberText.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFF_11",now.ToString());
		showExchangeTips (now);
	}

	/// <summary>
	/// 设置当前仓库材料拥有量
	/// </summary>
	private void setCurrentPropNum()
	{
		if(materialNum == 1)
			powderNum   = getPropsNum(exchange.conditions[0][0].costSid);
		else if(materialNum == 2)
		{
			redpaperNum = getPropsNum(exchange.conditions[0][0].costSid);
			wucaiNum    = getPropsNum(exchange.conditions[0][1].costSid);
		}
		else
		{
			redpaperNum = getPropsNum(exchange.conditions[0][0].costSid);
			powderNum   = getPropsNum(exchange.conditions[0][1].costSid);
			wucaiNum    = getPropsNum(exchange.conditions[0][2].costSid);
		}
	}

	///<summary>
	/// 根据兑换数量设置提示值
	/// </summary>
	public void showExchangeTips(int value){
		if (!(fatherWindow is NoticeWindow))
			return;
		if(materialNum == 1)
		{
			if(powderNum<exchange.conditions[0][0].num)
				powderLabel.text   = "[FF0000]"+powderNum+"/"+exchange.conditions[0][0].num+"[-]";
			else
				powderLabel.text   = powderNum+"/"+exchange.conditions[0][0].num*(value==0?1:value);
		}
		else if(materialNum == 2)
		{
			if(redpaperNum < exchange.conditions[0][0].num)
				redpaperLabel.text = "[FF0000]"+redpaperNum+"/"+exchange.conditions[0][0].num+"[-]";
			else
				redpaperLabel.text = redpaperNum+"/"+exchange.conditions[0][0].num*(value==0?1:value);
			if(wucaiNum<exchange.conditions[0][1].num)
				wucaiLabel.text    = "[FF0000]"+wucaiNum+"/"+exchange.conditions[0][1].num+"[-]";
			else
				wucaiLabel.text    = wucaiNum+"/"+exchange.conditions[0][1].num*(value==0?1:value);
		}
		else
		{		
			if(redpaperNum < exchange.conditions[0][0].num)
				redpaperLabel.text = "[FF0000]"+redpaperNum+"/"+exchange.conditions[0][0].num+"[-]";
			else
				redpaperLabel.text = redpaperNum+"/"+exchange.conditions[0][0].num*(value==0?1:value);
			if(powderNum<exchange.conditions[0][1].num)
				powderLabel.text   = "[FF0000]"+powderNum+"/"+exchange.conditions[0][1].num+"[-]";
			else
				powderLabel.text   = powderNum+"/"+exchange.conditions[0][1].num*(value==0?1:value);
			if(wucaiNum<exchange.conditions[0][2].num)
				wucaiLabel.text    = "[FF0000]"+wucaiNum+"/"+exchange.conditions[0][2].num+"[-]";
			else
				wucaiLabel.text    = wucaiNum+"/"+exchange.conditions[0][2].num*(value==0?1:value);
		}		
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
		else if(gameObj.name=="buttonMake")
		{
			ExchangeFPort exf = FPortManager.Instance.getFPort ("ExchangeFPort") as ExchangeFPort;
			exf.exchange (sample.noitceItemSid, now, receiveOK);
		}
		finishWindow();
	}
	private void receiveOK(int sid,int num)
	{
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("noticeActivityFF_10",sample.prizes.getPrizeName(),now.ToString()));
		if(callback!=null)
			callback(sid,num);
	}
	/// <summary>
	/// 检查道具是否够
	/// </summary>
	private bool checkCondition()
	{
		return true;
	}

	/// <summary>
	/// 更新UI
	/// </summary>
	public void updateUI()
	{

	}

}
