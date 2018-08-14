using UnityEngine;
using System.Collections;

/**
 * 神器碎片分解窗口
 * @author longlingquan
 * */
public class MagicWeapdecompositionWindow : WindowBase
{
	public GameObject goodsViewProfab;
	public GameObject goodsPoint;
	public UILabel titleText;//标题
	public UILabel numberText;//数量
    public UILabel currentHaveNum;//当前持有的数量
	public UISlider slider;
    public GoodsView getPropView;//分解获得的物品
    public UILabel getPropNum;//分解得到的数量
	private int max = 0;//允许使用道具最大数量
	private int min = 0;//允许使用道具最小数量
	private int  setp = 1;//每点一次的+-数量
	private int  now = 1;//当前的设置数
	private CallBackMsg callback;
	MessageHandle msg;
	private Prop item;
	private string str;
	private int costType;
	private string costStr;
    private int perNumForProp=0;//每个碎片分解得到的物品数量
	//兑换提示
	private string[] exchangeNames;
	private int[]	exchangeValues;

    //public class BuyStruct {
    //    public string titleTextName;
    //    public int goodsBgId;
    //    public string iconId;
    //    public int unitPrice;
    //}

	protected override void begin ()
	{
		base.begin (); 
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
		//	ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "backGround_2",bg);
	}
	
	//有大小限制的选择
	public void init (object obj, int numberMax, int numberMin, CallBackMsg callback)
	{ 
		init (obj, numberMax, numberMin, 1, 1,callback); 
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
    public void init(object obj, int numberMax, int numberMin, int numberNow, int numberSetp,CallBackMsg callback)
	{
		msg = new MessageHandle ();
		msg.msgInfo = obj;
		item = obj as Prop;
		this.callback = callback;
		setp = numberSetp;
		max = numberMax;
		min = numberMin;
		now = numberNow;
		coverDistanceToOne ();
        updateGetNum();
		updateDisplayeNumber ();
		calculateTotal ();
		GoodsView tmpGoodsView = CreateGoodsView ();
        currentHaveNum.text = numberMax.ToString();
        tmpGoodsView.init(item);
        tmpGoodsView.rightBottomText.text = "";
        titleText.text = item.getName();
	}
    //更新碎片
    void updateGetNum() {
        PrizeSample[] ps = PropSampleManager.Instance.getPropSampleBySid(item.sid).prizes;
        int propSid = ps[0].pSid;
        int perNum =StringKit.toInt(ps[0].num);
        getPropView.fatherWindow = this;
        getPropView.init(ps[0]);
        perNumForProp = perNum;
        getPropNum.text = perNumForProp.ToString();
    }

	/// <summary>
	/// 创建GoodsView
	/// </summary>
	private GoodsView CreateGoodsView ()
	{
		Utils.DestoryChilds (goodsPoint);
		GameObject obj = NGUITools.AddChild (goodsPoint, goodsViewProfab) as GameObject;
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
        view.fatherWindow = this;
        view.rightBottomText.text = "";
		return view;
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
        getPropNum.text = (now * perNumForProp)+"";
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
			GuideManager.Instance.doGuide ();;
            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(item.sid);
            msg.msgEvent = msg_event.dialogOK;
			finishWindow ();
			EventDelegate.Add (OnHide, () => {
				callback (msg);
			});
		} 
		//MaskWindow.UnlockUI ();
	}
	private bool checkSotreFull ()
	{
		
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
           // totalMoney.text = getTotalCost(good, now, good.getGoodsShopType());
		}
		//这里添加多样性 
        else if (msg.msgInfo.GetType () == typeof(Prop)) {
			//使用道具不显示cost
		} else if (msg.msgInfo.GetType () == typeof(ArenaChallengePrice)) {
			ArenaChallengePrice are = msg.msgInfo as ArenaChallengePrice;
			//totalMoney.text = are.getPrice (now).ToString ();
		}  else if (msg.msgInfo.GetType () == typeof(LaddersChallengePrice)) {
			LaddersChallengePrice are = msg.msgInfo as LaddersChallengePrice;
			//totalMoney.text = are.getPrice (now).ToString ();
		}
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
}
