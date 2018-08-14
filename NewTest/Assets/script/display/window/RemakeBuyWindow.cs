using UnityEngine;
using System.Collections;

/// <summary>
/// 重塑购买窗口
/// </summary>
public class RemakeBuyWindow : WindowBase {

	/** 预制体 */
	public GameObject goodsViewProfab;
	/** 预制体创建点位 */
	public GameObject goodsPoint;
	/** 拖动条 */
	public UISlider slider;
	/** 道具拖动条数量 */
	public UILabel goodCostCountLabel;
	/** 需求rmb数量 */
	public UILabel needRmbNumValue;
	/** 持有rmb数量  */
	public UILabel rmbNumValue;
	/** 道具抵扣数量 */
	public UILabel goodsNeedValue;

	/** 提升需求RMB */
	private int needValue = 0;
	/** 道具抵扣价值 */
	private int exValue = 0;
	/** 允许使用道具最大数量 */
	private int max = 0;
	/** 允许使用道具最小数量 */
	private int min = 0;
	/** 每点一次的+-数量 */
	private int setp = 1;
	/** 当前的设置数 */
	private int now = 0;

	private GoodsView tmpPropView;
	private Prop tmpProp;
	private CallBackMsg callback;
	/** message确认框 */
	public MessageHandle msg;

	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 初始化数据
	/// </summary>
	/// <param name="_tmpProp">需求抵扣道具.</param>
	/// <param name="_exValue">道具抵扣价值.</param>
	/// <param name="_needValue">提升需求RMB.</param>
	/// <param name="callback">回调.</param>
	public void init (Prop _tmpProp, int _exValue, int _needValue, CallBackMsg _callBack) {
		needValue = _needValue;
		tmpProp = _tmpProp;
		exValue = _exValue;
		msg = new MessageHandle ();
		msg.msgInfo = _tmpProp;
		this.callback = _callBack;
		tmpPropView = CreateItem ();
		max = getMaxValue ();
		showUI ();
		coverDistanceToOne ();
	}
	/// <summary>
	/// 面板显示
	/// </summary>
	private void showUI () {
		rmbNumValue.text = UserManager.Instance.self.getRMB().ToString();
		needRmbNumValue.text =  getNeedRmb ().ToString();
		goodsNeedValue.text = LanguageConfigManager.Instance.getLanguage("RemakeBuyWindow_goodsNeedValue",exValue.ToString ());
	}
	/// <summary>
	/// 当前抵扣后的需求RMB数量
	/// </summary>
	private int getNeedRmb () {
		return Mathf.Max ((needValue - now * exValue),0);
	}
	/// <summary>
	/// 获得最多能使用的道具数量
	/// </summary>
	private int getMaxValue () {
		//玩家拥有的指定道具>=所需要道具的最大值，则max为所需要道具的最大值
		//玩家拥有的指定道具<所需要道具的最大值，则max为玩家拥有的道具值
		if (tmpProp == null) {
			return 0;
		} else {
			Prop prop = StorageManagerment.Instance.getProp (tmpProp.sid);
			int num = prop == null ? 0 : prop.getNum ();
			return Mathf.Min ((int)(needValue / exValue),num);
		}
	}
	/// <summary>
	/// 创建需求物品
	/// </summary>
	private GoodsView CreateItem () {
		if (tmpPropView == null && tmpProp != null) {
			GameObject obj = NGUITools.AddChild (goodsPoint, goodsViewProfab) as GameObject;
			obj.transform.localScale = new Vector3(0.55f,0.55f,1);
			GoodsView view = obj.transform.GetComponent<GoodsView> ();
			view.init (tmpProp, 0);
			view.fatherWindow = fatherWindow;
			return view;
		} else {
			return tmpPropView;
		}
	}

	/// <summary>
	/// 有钱没,没钱滚蛋
	/// </summary>
	private bool isRmbEnough () {
		return UserManager.Instance.self.getRMB () >= getNeedRmb ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			msg.msgEvent = msg_event.dialogCancel;
			finishWindow ();
			callback (msg);
		}
		//最小值
		else if (gameObj.name == "min") {
			now = min;
			updateDisplayeNumber ();
			coverDistanceToOne ();
		}
		//最大值
		else if (gameObj.name == "max") {
			now = max;
			updateDisplayeNumber ();
			coverDistanceToOne ();
		}
		//加
		else if (gameObj.name == "add") {
			addNumber ();
		}
		//减
		else if (gameObj.name == "reduce") {
			reduceNumber ();
		}
		//确定提升
		else if (gameObj.name == "buttonOk") {
			if (!isRmbEnough ()) {
				UiManager.Instance.openDialogWindow<MessageWindow>( (win)=>{
					win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0040"),LanguageConfigManager.Instance.getLanguage("s0315"),LanguageConfigManager.Instance.getLanguage("Guild_85"),recharge);
				});
				return;
			}
			callback (msg);
			finishWindow ();
		}
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 没钱？去充值！！！
	/// </summary>
	private void recharge(MessageHandle msg) {
		if(msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		finishWindow();
		EventDelegate.Add(OnHide,()=>{
			UiManager.Instance.openWindow<rechargeWindow>();
		});
	}
	//范围缩放到0,1空间为了slider
	void coverDistanceToOne () {
		if (max - min == 0) {
			slider.gameObject.SetActive (false);
			return;
		}
		float a = (float)now / max;
		if (slider == null)
			return;
		slider.value = a;
	}
	/** 改变数量 */
	void changeNum (int newValue) {
		if (newValue > max)
			newValue = max;
		if (newValue < min)
			newValue = min;
		now = newValue;
		updateDisplayeNumber ();	
	}
	/** 拖拽滑块 */
	public void onSliderChange () {
		int a = Mathf.CeilToInt (slider.value * max);
		changeNum (a);
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
		goodCostCountLabel.text = now + "";
		msg.msgNum = now;
		showUI ();
	}
}
