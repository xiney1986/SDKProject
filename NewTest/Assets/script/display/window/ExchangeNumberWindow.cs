using UnityEngine;
using System.Collections;

//兑换数量选择窗口
public class ExchangeNumberWindow : WindowBase
{
	public UITexture itemImage;//物品图标
	public UILabel titleText;//标题
	public UILabel numberText;//数量
//	private int consume = 0;//消耗值 
	private int max = 0;//允许使用道具最大数量
	private int min = 0;//允许使用道具最大数量
	private int  setp = 1;//每点一次的+-数量
	private int  now = 1;//当前的设置数
	private CallBackMsg callback;
	public ButtonBase buttonOk;
 	MessageHandle msg;
 
	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent();
		MaskWindow.UnlockUI ();
	}
	
	//最原始的初始化
	public void Initialize (Exchange exc, int numberMax, CallBackMsg callback)
	{
		msg=new MessageHandle();
		msg.msgInfo = exc;
		ExchangeSample sample=exc.getExchangeSample();
		this.callback = callback;
		setp = 1;
		max = numberMax;
		min = 1;
		if(GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID26)
			now = 1;
		else
			now = numberMax;

		updateDisplayeNumber();
		
		
		//	setNumber (goods);
		if (sample.type == PrizeType.PRIZE_CARD) {
			Card showCard = CardManagerment.Instance.createCard (sample.exchangeSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + showCard.getImageID () , itemImage);
		//	itemImage.transform.localScale = bigPic;
			titleText.text = showCard.getName();

			
		} else if ( sample.type  == PrizeType.PRIZE_EQUIPMENT) {
			Equip showEquip = EquipManagerment.Instance.createEquip (sample.exchangeSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showEquip.getIconId (), itemImage);	
			//itemImage.transform.localScale = new Vector3 (90, 90, 0);
			titleText.text = showEquip.getName();
			 
		} else if (sample.type == PrizeType.PRIZE_PROP) {
			Prop showProp = PropManagerment.Instance.createProp (sample.exchangeSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showProp.getIconId (), itemImage);	
		//	itemImage.transform.localScale = new Vector3 (90, 90, 0);
			titleText.text = showProp.getName();
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
 
	}
	
	public override void DoDisable ()
	{
		base.DoDisable (); 
		if (callback != null)
			callback (msg);
	}

	void addNumber ()
	{
		if (now >= max) {
			updateDisplayeNumber();	
		} else {
			now += setp;
			updateDisplayeNumber();			
		}
	}

	void reduceNumber ()
	{
		if (now <= min) {
			updateDisplayeNumber();
		} else {
			now -= setp;
			updateDisplayeNumber();			
		}
	}
	void updateDisplayeNumber()
	{
		numberText.text = now + "";
		msg.msgNum=now;
	}
 
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase(gameObj);
		if (gameObj.name == "close") {
 			msg.msgEvent=msg_event.dialogCancel;
			hideWindow (); 
		}
		//最小值
		if (gameObj.name == "min") {
			now = min;
			updateDisplayeNumber();

		}
		//最大值
		if (gameObj.name == "max") {
			now = max;
			updateDisplayeNumber();
		}
		//加
		if (gameObj.name == "add") {
			addNumber ();
		}
		//减
		if (gameObj.name == "reduce") {
			reduceNumber ();
		}
		
 
		//选定确认
		if (gameObj.name == "buttonOk") { 
			GuideManager.Instance.doGuide(); 
			msg.msgEvent=msg_event.dialogOK;
			finishWindow();
		}
		
	}

 
	
}
