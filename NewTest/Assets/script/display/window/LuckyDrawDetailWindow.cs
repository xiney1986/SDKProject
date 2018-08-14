using UnityEngine;
using System;
using System.Collections.Generic;
 
/**
 * 抽奖细节窗口
 * @author longlingquan
 * */
public class LuckyDrawDetailWindow:WindowBase
{  
	public ButtonBase button1;//左边按钮(一个按钮时 在中间)
	public ButtonBase button2;//右边按钮
	public UILabel button1CostText;
	public UILabel button2CostText;
	public UISprite button1CostIcon;
	public UISprite button2CostIcon;

	//info

	public UILabel costNum;//当前货币数量

	public UILabel topinfo;//抽奖描述信息
	public Transform point1;//按钮坐标 左
	public Transform point2;//按钮坐标 右
	public Transform point3;//按钮坐标 中
	public Transform infopoint1;//信息描述坐标 左
	public Transform infopoint2;//信息描述坐标 右
	public Transform infopoint3;//信息描述坐标 中
	
	public UISprite costIcon;
	public carouselCtrl content;
	private int drawIndex = -1;//抽奖方式索引
	private bool isSend = false;
	private int drawTimes = 0;
	private bool isShowPromt = false;
	private LuckyDraw lucky;//抽奖条目对象

	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}
	protected override void DoEnable () {
		UiManager.Instance.backGround.switchBackGround("luckyDraw");
		if (MissionManager.instance != null)//隐藏掉3D场景
			MissionManager.instance.hideAll ();
	}

	//初始化花费图标
	private void setCostIcon ()
	{
        LuckyCostIcon.setToolCostIconName(lucky.ways[0], costIcon);
		LuckyCostIcon.setToolCostIconName (lucky.ways [0], button1CostIcon);
		LuckyCostIcon.setToolCostIconName (lucky.ways [0], button2CostIcon);
	}
	
	//设置抽奖条目
	public void setLuckyDraw (LuckyDraw lucky)
	{
		this.lucky = lucky;
		showInfo ();
	}
	
	//显示信息
	private void showInfo ()
	{ 
		setCostIcon ();
		setTitle (lucky.getTitle ());
		showByCostTyep ();
		showButtonAndInfo ();
		showContentInfo ();
		showTopInfo ();
	}

	private void showTopInfo ()
	{
		LuckyDrawSample luckySample = lucky.getSample ();
		string[] luckyPoints = luckySample.luckyPoints;
		//大于0表示要显示
		if (StringKit.toInt (luckyPoints [0]) > 0) {
			//大条目已抽奖总次数（多加一次抽奖信息）
			int allCount = 1;
			for (int i=0; i<lucky.ways.Length; i++) {
				allCount += lucky.ways [i].getNum ();
			}

			int quality = 0, count = int.MaxValue;
			string des = "";
			int temp;
			for (int i=1; i<luckyPoints.Length; i += 3) {
				temp = allCount % StringKit.toInt (luckyPoints [i + 1]);
				if (temp > 0)
					temp = StringKit.toInt (luckyPoints [i + 1]) - temp;
				if (temp > count)
					continue;
				if (temp < count) {
					count = temp;
					quality = StringKit.toInt (luckyPoints [i]);
					des = luckyPoints [i + 2];
				} else if (StringKit.toInt (luckyPoints [i]) > quality) {
					quality = StringKit.toInt (luckyPoints [i]);
					des = luckyPoints [i + 2];
				}
			}
            count += 1;
			string str = "";
			//本次不是必出
			if (count > 1) {
				str = LanguageConfigManager.Instance.getLanguage ("luckdraw11", count.ToString (), des);
			} else {
				str = LanguageConfigManager.Instance.getLanguage ("luckdraw12", des);
			}
			topinfo.text = str;
		} else {
			topinfo.text = lucky.getDescribe ();
		}
	}
	
	private void showContentInfo ()
	{ 
		content.setTextureInfo (lucky.getPrizesInfo (), lucky.getPrizesType ());
	}
	
	//显示当前货币数量
	public void showByCostTyep ()
	{
		if (lucky.ways [0].getCostType () == PrizeType.PRIZE_RMB) {
			int num = UserManager.Instance.self.getRMB ();
			costNum.text = "x" + num;
			if (lucky.isFreeDraw ()) {
				button1.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0026", lucky.getFreeNum ().ToString ());
			} else {
				button1.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [0].getDrawTimes ().ToString ()); 
				button2.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [1].getDrawTimes ().ToString ()); 
			}
			isShowPromt = true;
		}
		if (lucky.ways [0].getCostType () == PrizeType.PRIZE_MONEY) {
			costNum.text = "x" + UserManager.Instance.self.getMoney ();
			buttonTextInfo (UserManager.Instance.self.getMoney ());
		}
		if (lucky.ways [0].getCostType () == PrizeType.PRIZE_PROP) {
			Prop prop = StorageManagerment.Instance.getProp (lucky.ways [0].getCostToolSid ());
			if (prop == null) {
				costNum.text = "0";
				buttonTextInfo (0);
			} else {
				costNum.text = "x" + prop .getNum ();
				buttonTextInfo (prop.getNum ());
			}
				
		}
	}

	private void buttonTextInfo (int num)
	{
		if (lucky.isFreeDraw ()) {
			button1.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0026", lucky.getFreeNum ().ToString ());
		} else {
			button1.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [0].getDrawTimes ().ToString ()); 
			if (num >= lucky.ways [1].getCostPrice (lucky.getFreeNum ()) || num < (lucky.ways [0].getCostPrice (lucky.getFreeNum ())) * 2) {
				button2.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [1].getDrawTimes ().ToString ()); 
				isSend = false;
				isShowPromt = true;
			} else {
				drawTimes = num / lucky.ways [0].getCostPrice (lucky.getFreeNum ());
				button2.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", drawTimes.ToString ()); 
				isShowPromt = false;
				isSend = true;
			}
		}
	}
	//以后再者修改图标名字
	public void setIcon (UISprite icon)
	{
		icon.spriteName = getIconName ();
	}
	
	//获得图标地址
	private string getIconName ()
	{	
		return "money";
	}
				
	//显示抽奖按钮信息
	private void showButtonAndInfo ()
	{
		//先隐藏按钮
		button1.gameObject.SetActive (false);
		button2.gameObject.SetActive (false);
 
		//只有一个抽奖方式
		if (lucky.isFreeDraw ()) { 	
			button1.transform.position = point3.transform.position;
			if (!lucky.isCost (0)) {
				button1.disableButton (true);
			}
			button1.gameObject.SetActive (true);
			if (lucky.ways [0].getCostPrice (lucky.getFreeNum ()) == 0 || lucky.canFreeCD()) {
				button1CostIcon.enabled = false;
				button1CostText.text = "";
			} else {
				button1CostIcon.enabled = true;
				button1CostText.text = "X " + lucky.ways [0].getCostPrice (lucky.getFreeNum ());
			}
		} else { 
			button1.transform.position = point1.transform.position;
			if (!lucky.isCost (0)) {
				button1.disableButton (true);
			}
			button1.gameObject.SetActive (true);
			button1CostIcon.enabled = true;
			button1CostText.text = "X " + lucky.ways [0].getCostPrice (lucky.getFreeNum ());

			button2.transform.position = point2.transform.position;
			if (!lucky.isCost (1)) {
				button2.disableButton (true);
			}
			button2.gameObject.SetActive (true);
			button2CostIcon.enabled = true;
			if (isSend) {
				button2CostText.text = "X " + drawTimes * lucky.ways [0].getCostPrice (lucky.getFreeNum ());
			} else {
				button2CostText.text = "X " + lucky.ways [1].getCostPrice (lucky.getFreeNum ());
			}

		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{  
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name == "drawButton1") { 
			if (isStorageFulls ()) {
				return;
			}
			if (!isDrawCost (0)) {
				string str = getMSG (0);
				if (lucky.ways [0].getCostType () == PrizeType.PRIZE_RMB)
					MessageWindow.ShowRecharge (str);
				else
					MessageWindow.ShowAlert (str);
				return;
			}
						
			drawIndex = 0;
			LuckyDrawFPort port = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
			port.luckyDraw (1, lucky.sid, 1, lucky.ways [0], luckyDrawCallBack);
		} else if (gameObj.name == "drawButton2") {
			if (isStorageFulls ()) {
				return;
			}
						
			if (isShowPromt && !isDrawCost (1)) {
				string str = getMSG (1);
				if (lucky.ways [1].getCostType () == PrizeType.PRIZE_RMB)
					MessageWindow.ShowRecharge (str);
				else
					MessageWindow.ShowAlert (str);
				return;
			}
			if (isSend) {
				drawIndex = 1;
				LuckyDrawFPort port = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
				port.luckyDraw (drawTimes, lucky.sid, 1, lucky.ways [0], luckyDrawMultipleCallBack);
				UiManager.Instance.applyMask ();
			} else {
				drawIndex = 1;
				LuckyDrawFPort ports = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
				ports.luckyDraw (10, lucky.sid, 2, lucky.ways [1], luckyDrawMultipleCallBack);
			}
						
						
		}
	}
	
	private bool isStorageFulls ()
	{
		//忽略除了卡片之外的抽奖获得
		if (StorageManagerment.Instance.isRoleStorageFull (1)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0172"));
			return true;
		} else if (StorageManagerment.Instance.isEquipStorageFull (1)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("luckdraw14"));
			return true;
		} else if (StorageManagerment.Instance.isTempStorageFull (20)) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("luckdraw10")
				                , LanguageConfigManager.Instance.getLanguage ("s0173"), gotoTempMail); 
			});
			return true;
		}
		return false;
	}

	//临时仓库已满，去领取再来
	private void gotoTempMail (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			return;
		}
		if (fatherWindow is LuckyDrawWindow) {
			fatherWindow.destoryWindow ();
		}
		//finishWindow ();
		UiManager.Instance.openWindow<MailWindow> ((win) => {
			win.Initialize (1);
		});
	}

	private string getMSG (int index)
	{
		string str = string.Empty;
		switch (lucky.ways [index].getCostType ()) {
		case PrizeType.PRIZE_RMB:
			str = LanguageConfigManager.Instance.getLanguage ("s0048");
			break;
		case PrizeType.PRIZE_MONEY:
			str = LanguageConfigManager.Instance.getLanguage ("s0049");
			break;
		case PrizeType.PRIZE_PROP:
			str = PropSampleManager.Instance.getPropSampleBySid (lucky.ways [index].getCostToolSid ()).name;
			break;
		}
		return str + LanguageConfigManager.Instance.getLanguage ("s0053");
	}
	
	//是否能够抽奖 放回true (表明扣除消耗) 优化有，扣除由后台控制
	private bool isDrawCost (int index)
	{
		if (lucky.getFreeNum () > 0 || lucky.canFreeCD())
			return true;
		if (lucky.ways [index].getCostType () == PrizeType.PRIZE_RMB) {
			int rmb = lucky.ways [index].getCostPrice (lucky.getFreeNum ());
			return rmb <= UserManager.Instance.self.getRMB ();
		} else if (lucky.ways [index].getCostType () == PrizeType.PRIZE_MONEY) {
			int money = lucky.ways [index].getCostPrice (lucky.getFreeNum ());
			return  money <= UserManager.Instance.self.getMoney ();
		} else if (lucky.ways [index].getCostType () == PrizeType.PRIZE_PROP) {
			return StorageManagerment.Instance.checkProp (lucky.ways [index].getCostToolSid (), lucky.ways [index].getCostPrice (lucky.getFreeNum ()));
		} 
		return false;
	} 
	//抽奖回调方法
	private void luckyDrawCallBack (LuckyDrawResults results)
	{
		UiManager.Instance.cancelMask ();
		callbackAfterEffect (results);
		return;
	}

	private void luckyDrawMultipleCallBack (LuckyDrawResults results)
	{ 
		drawTimes = 0;
		isSend = false;
		UiManager.Instance.cancelMask ();
		callbackAfterEffect (results);
		return;
	}
	//抽奖特效完成后
	void callbackAfterEffect (LuckyDrawResults results)
	{
		lucky.drawLucky (drawIndex);
		UiManager.Instance.switchWindow<LuckyDrawShowWindow> ((win) => {
			win.init (results, lucky); 
		});
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		showInfo ();
	}
	public override void DoDisable ()
	{
		base.DoDisable ();
		if (MissionManager.instance != null)
		{
			MissionManager.instance.showAll ();
			MissionManager.instance.setBackGround();
		}
	}
} 

