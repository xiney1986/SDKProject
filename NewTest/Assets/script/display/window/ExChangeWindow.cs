using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExChangeWindow : WindowBase
{
	public UIDragScrollView dragObj;//拖动体
	public TapContentBase tapBase;//分页按钮
	public ContentCardExchange cardContent;//卡片分页
	public ContentEquipExchange equipContent;//装备分页
	public ContentPropExchange propContent;//道具分页
	public MessageHandle tmpMsg;
	public GameObject  exChangeBarPrefab;
	public GameObject[] exchangeCount; //可兑换数量 0为card分页，1为装备分页，2为道具分页
	private int index = 2;//0为card分页，1为装备分页，2为道具分页

	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide) {
			tapBase.changeTapPage (tapBase.tapButtonList [index]);
			updateExchangeCount ();
		}
		if (GuideManager.Instance.isEqualStep(106004000)) {
			GuideManager.Instance.guideEvent ();
		}	
		MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 设置默认的页签
	/// </summary>
	public void setDefaultIndex(int index){
		this.index = index;
	}

	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		tapBase.changeTapPage (tapBase.tapButtonList [index]);
		updateExchangeCount ();
	}

	//更新可兑换数量显示
	public void updateExchangeCount () {
		for (int i = 0; i < 3; ++i) {
			UILabel label = exchangeCount [i].transform.FindChild ("num").GetComponent<UILabel> ();
			int count = ExchangeManagerment.Instance.getExchangeCount (i, ExchangeType.COMMON);	
			if (count > 0) {
				label.text = count.ToString ();
				exchangeCount [i].SetActive (true);
			}
			else
				exchangeCount [i].SetActive (false);
		}
	}

//	private void changeCollider()
//	{
//		TapButtonBase[] list = tapBase.tapButtonList;
//		for (int i = 0; i < list.Length; i++) {
//			if(i == index)
//			{
////				list[i].GetComponent<BoxCollider>().enabled = false;
//			}
//			else
//			{
////				list[i].GetComponent<BoxCollider>().enabled = true;
//			}
//		}
//	}

	//页面按钮
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonCard" && enable == false) {
			cardContent.cleanAll ();
		} else if (gameObj.name == "buttonEquip" && enable == false) {
			equipContent .cleanAll ();
		} else if (gameObj.name == "buttonProp" && enable == false) {
			propContent.cleanAll ();
		} else if (gameObj.name == "buttonCard" && enable == true) {
			updateCardContent ();
		} else if (gameObj.name == "buttonEquip" && enable == true) {
			updateEquipContent ();
		} else if (gameObj.name == "buttonProp" && enable == true) {
			if (GuideManager.Instance.isEqualStep(106004000)) {
				GuideManager.Instance.doGuide ();
			}
			updatePropContent ();
		}
	}
	//更新卡片兑换分页
	private void updateCardContent ()
	{
		index = 0;
		//changeCollider();
		dragObj.scrollView = cardContent.GetComponent<UIScrollView>();
		cardContent.setExchangeType (ExchangeType.COMMON);
		float y = cardContent.transform.localPosition.y;
		cardContent.cleanAll();
		cardContent.reLoad ();
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			cardContent.jumpToPos(y);
		}));
	}
	//更新装备兑换分页
	private void updateEquipContent ()
	{
		index = 1;
		//	changeCollider();
		dragObj.scrollView = equipContent.GetComponent<UIScrollView>();
		equipContent.setExchangeType (ExchangeType.COMMON);
		float y = equipContent.transform.localPosition.y;
		cardContent.cleanAll();
		equipContent.reLoad ();
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			equipContent.jumpToPos(y);
		}));
	}
	//更新道具兑换分页
	private void updatePropContent ()
	{
		index = 2;
		//changeCollider();
		dragObj.scrollView = propContent.GetComponent<UIScrollView>();
		propContent.setExchangeType (ExchangeType.COMMON);
		float y = propContent.transform.localPosition.y;
		cardContent.cleanAll();
		propContent.reLoad ();
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			propContent.jumpToPos(y);
		}));
	}


	//兑换回调
	public void receiveGift (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogOK) {
			ExchangeSample sample = (msg.msgInfo as Exchange).getExchangeSample ();
			
			string need = "";
			int max=sample.conditions[0].Length;
			if(max>0){
				for (int i=0; i<sample.conditions[0].Length; i++) {
					need += sample.conditions[0] [i].getName () + "x" + msg.msgNum * sample.conditions[0] [i].num + "\n";
				}
			}
			string target = sample.getExhangeItemName () + "x" + msg.msgNum * sample.num;
			string info = LanguageConfigManager.Instance.getLanguage ("s0130", need, target);

			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), info, sureExchange);
			});

			tmpMsg = msg;
		}
	}
	public void sureExchange (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogOK) {
			tmpMsg = msg;
			ExchangeFPort exf = FPortManager.Instance.getFPort ("ExchangeFPort") as ExchangeFPort;
			exf.exchange ((msg.msgInfo as Exchange).sid, msg.msgNum, receiveOK);
			//	UiManager.Instance.applyMask();
		}
		else {
			tmpMsg = null;
		}
		
	}
	
	public void receiveOK (int sid,int num)
	{
		ExchangeManagerment.Instance.addExchange (sid, num);
		bool needShowEffect=false;
		ExchangeSample sample = (tmpMsg.msgInfo as Exchange).getExchangeSample ();
		if (sample.type == PrizeType.PRIZE_CARD) {
			Card c = CardManagerment.Instance.createCard (sample.exchangeSid);
			for (int i = 0; i < tmpMsg.msgNum; i++) {
				if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (c)) {
					needShowEffect=true;
				}
			}
		}
		//TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0132"));
		exchangeFinish (null);
		if(needShowEffect)//如果需要显示特效 则不需要在TextTipWindow消失时 解除MaskWindow.UnlockUI,应该在特效消失时调用，因为特效的时间更长
		{
			TextTipWindow.ShowNotUnlock (LanguageConfigManager.Instance.getLanguage ("s0132"));
			showEffect ();
		}else
		{
			TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0132"));
		}
	}
	void showEffect ()
	{
		MaskWindow.LockUI();
		EffectManager.Instance.CreateEffectCtrlByCache(UiManager.Instance.UIEffectRoot.transform,"Effect/UiEffect/hero_RoadEnable",(obj,ec)=>{
			ec.transform.localPosition = new Vector3 (0, -100, 0);
			StartCoroutine (Utils.DelayRun (() => {
				Destroy (ec.gameObject);
				MaskWindow.UnlockUI ();
			}, 2.6f));
		});
	}

	public void exchangeFinish (MessageHandle msg)
	{
		updateExchangeCount ();
		tapBase.resetTap ();
		tapBase.changeTapPage (tapBase.tapButtonList [index]);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			GuideManager.Instance.doGuide ();
			if (GuideManager.Instance.isEqualStep(107001000)) {
				UiManager.Instance.openMainWindow ();
				return;
			}
			finishWindow ();
		}
	}
}