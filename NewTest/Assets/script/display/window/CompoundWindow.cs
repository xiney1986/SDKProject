using UnityEngine;
using System.Collections;

/**
 * 红色万能卡合成窗口
 * @author longlingquan
 * */
public class CompoundWindow : WindowBase
{
	public GameObject goodsViewProfab;
	public GameObject goodsPoint;
    public GameObject goodsPoint1;
    public GameObject goodsPoint2;
	//public UILabel titleText;//标题
    public UILabel numberText;//合成数量
	public UISlider slider;
    public UILabel goodsName;
    public UILabel goods1Name;
    public UILabel goods2Name;
//	private int consume = 0;//消耗值 
	private int max = 0;//允许使用道具最大数量
	private int min = 0;//允许使用道具最小数量
	private int  setp = 1;//每点一次的+-数量
	private int  now = 1;//当前的设置数
	private CallBackMsg callback;
	MessageHandle msg;
	private object item;
    private string str;
    private Prop costProp;

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
		MaskWindow.UnlockUI ();
		//	ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "backGround_2",bg);
	}
	
	//有大小限制的选择
	public void init (object obj, int numberMax, int numberMin, CallBackMsg callback)
	{ 
		init (obj, numberMax, numberMin, 1, 1, callback); 
	}

	//最脑残的选择
	public void init (object obj, CallBackMsg callback)
	{ 
		init (obj, 100, 1, 1, 1, callback); 
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
	public void init (object obj, int numberMax, int numberMin, int numberNow, int numberSetp, CallBackMsg callback)
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
		updateDisplayeNumber ();
		GoodsView tmpGoodsView = CreateGoodsView ();
        Prop prop = item as Prop;
        tmpGoodsView.init(prop);
        switch (prop.sid) { 
            case 71197:
                costProp = PropManagerment.Instance.createProp(71196);
                break;
            case 71198:
                costProp = PropManagerment.Instance.createProp(71197);
                break;
            case 71199:
                costProp = PropManagerment.Instance.createProp(71198);
                break;
            case 71200:
                costProp = PropManagerment.Instance.createProp(71199);
                break;
        }
        goodsName.text = prop.getName();
        goods1Name.text = costProp.getName();
        goods2Name.text = costProp.getName();
        Utils.DestoryChilds (goodsPoint1);
        GameObject obj1 = NGUITools.AddChild(goodsPoint1, goodsViewProfab) as GameObject;
        GoodsView obj1View = obj1.transform.GetComponent<GoodsView>();
        if (StorageManagerment.Instance.getProp(costProp.sid) != null)
        obj1View.init(costProp, StorageManagerment.Instance.getProp(costProp.sid).getNum());

        Utils.DestoryChilds(goodsPoint2);
        GameObject obj2 = NGUITools.AddChild(goodsPoint2, goodsViewProfab) as GameObject;
        GoodsView obj2View = obj2.transform.GetComponent<GoodsView>();
        if (StorageManagerment.Instance.getProp(costProp.sid) != null)
        obj2View.init(costProp, StorageManagerment.Instance.getProp(costProp.sid).getNum());
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

	//数字滚动响应
	public void numberFly (bool isAdd)
	{
		if (isAdd) {
			addNumber ();
		} else {
			reduceNumber ();
		}
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
		//选定确认
		if (gameObj.name == "buttonOk") {
            msg.msgEvent = msg_event.dialogOK;
			finishWindow ();
			EventDelegate.Add (OnHide, () => {
				callback (msg);
			});
		} 
		//MaskWindow.UnlockUI ();
	}
}
