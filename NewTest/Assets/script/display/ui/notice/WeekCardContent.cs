using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WeekCardContent : MonoBehaviour
{

	public GameObject go_rewardListContent;
	public ButtonBase btn_buy;
	public ButtonBase btn_receive;
	//public UILabel lab_cardInfo;
	public UILabel lab_timeInfo;
	public WindowBase fatherWindow;
	public UISprite sprite_next;
	public UISprite sprite_pre;
	public UIScrollView scrollView;
	public UILabel lab_receive;
	public GameObject go_goodsPrefab;

//	PrizeSample[] prizeSample;// 周卡奖励对象//
	PrizeSample selectPrizeSample = null;// 已选中的奖励对象//
	Dictionary<int,PrizeSample[]> prizeDic;
	int selectID;

	bool canRecevie = false;// 可以领取//

	public GameObject selectIcon;// 选择标示//

	bool isOnPressing = false;
	float mTime = 0.0f;
	//GameObject pressedPrize;
	GoodsView pressedPrizeView;

	void Awake()
	{
		btn_receive.disableButton (true);

		btn_buy.onClickEvent = M_onClickBuy;

		btn_receive.onClickEvent = M_onClickReceive;
	}

	//更新周卡领取按钮
	private void updateReceiveButton()
	{
		MaskWindow.UnlockUI ();
		int recevieState = WeekCardInfo.Instance.recevieState;
		int weekCardState = WeekCardInfo.Instance.weekCardState;
		if(weekCardState == WeekCardState.not_open || weekCardState == WeekCardState.over)// 没有买过 或者已过期//
		{
			btn_receive.disableButton(true);
			lab_receive.text=LanguageConfigManager.Instance.getLanguage("s0309");
		}
		else// 买过//
		{
			if(recevieState == WeekCardRecevieState.recevie)// 可领取//
			{
				btn_receive.disableButton(false);
				lab_receive.text=LanguageConfigManager.Instance.getLanguage("s0309");
			}
			else// 已领取//
			{
				btn_receive.disableButton(true);
				lab_receive.text=LanguageConfigManager.Instance.getLanguage("recharge02");
			}
		}
	}
	//更新购买的周卡信息
	private void updateWeekCardInfo()
	{
		updateReceiveButton();
		if(WeekCardInfo.Instance.weekCardState == WeekCardState.not_open)// 没有买过//
		{
			lab_timeInfo.text=LanguageConfigManager.Instance.getLanguage("monthCardNoBuy");
		}
		else if(WeekCardInfo.Instance.weekCardState == WeekCardState.over)// 已过期//
		{
			lab_timeInfo.text=LanguageConfigManager.Instance.getLanguage("s0138");
		}
		else
		{
			DateTime dt = TimeKit.getDateTime(WeekCardInfo.Instance.endTime);
			string[] strArr = new string[3];
			strArr[0] = dt.Year.ToString();
			strArr[1] = dt.Month.ToString();
			strArr[2] = dt.Day.ToString();
			lab_timeInfo.text = string.Format(LanguageConfigManager.Instance.getLanguage ("weekCardTimeInfo"),strArr);
		}
	}
	public void initContent(WindowBase win)
	{

		fatherWindow=win;
		btn_receive.fatherWindow = fatherWindow;
		btn_buy.fatherWindow = fatherWindow;
		M_creatDayReward();
		WeekCardInfoFPort fPort = FPortManager.Instance.getFPort ("WeekCardInfoFPort") as WeekCardInfoFPort;
		fPort.WeekCardInfoAccess(updateWeekCardInfo);
	}
	private void M_creatDayReward()
	{
		prizeDic = WeekCardSampleManager.Instance.prizesDic;
		PrizeSample prize;
		GameObject newItem;
		GoodsView goodsButton;

		foreach (KeyValuePair<int,PrizeSample[]> kv in prizeDic)
		{
			prize = kv.Value[0];
			newItem= NGUITools.AddChild(go_rewardListContent,go_goodsPrefab);
			newItem.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
			newItem.name = kv.Key.ToString();
			goodsButton = newItem.GetComponent<GoodsView>();
			goodsButton.fatherWindow = fatherWindow;
			goodsButton.onClickCallback = clickPrize2;
			UIEventListener.Get(newItem).onClick = clickPrize;
			UIEventListener.Get(newItem).onPress = onPressPrize;

			if (prize == null) {
				return;
			} else {
				switch (prize.type) {
				case PrizeType.PRIZE_BEAST:
					goodsButton.gameObject.SetActive (true);
					Card beast = CardManagerment.Instance.createCard (prize.pSid);
					goodsButton.init(beast,prize.getPrizeNumByInt ());
					break;
				case PrizeType.PRIZE_CARD:
					goodsButton.gameObject.SetActive (true);
					Card card = CardManagerment.Instance.createCard (prize.pSid);
					goodsButton.init(card,prize.getPrizeNumByInt ());
					break;
				case PrizeType.PRIZE_EQUIPMENT:
					goodsButton.gameObject.SetActive (true);
					Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
					goodsButton.init(equip,prize.getPrizeNumByInt ());
					break;
				case PrizeType.PRIZE_MONEY:
					goodsButton.gameObject.SetActive (true);
					goodsButton.init(prize);
					break;
				case PrizeType.PRIZE_PROP:
					goodsButton.gameObject.SetActive (true);
					Prop prop = PropManagerment.Instance.createProp (prize.pSid);
					goodsButton.init(prop,prize.getPrizeNumByInt ());
					break;
				case PrizeType.PRIZE_RMB:
					goodsButton.gameObject.SetActive (true);
					goodsButton.init(prize);
					break;
                 case PrizeType.PRIZE_MAGIC_WEAPON:
                        goodsButton.gameObject.SetActive (true);
                        MagicWeapon magicweapon = MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
                        goodsButton.init(magicweapon);
                    break;

				}
				goodsButton.prize = prize;
			}	
		go_rewardListContent.GetComponent<UIGrid>().Reposition();	
		sprite_pre.gameObject.SetActive(prizeDic.Count>4);
	    sprite_next.gameObject.SetActive(prizeDic.Count>4);
		}
	}
	private void M_onClickBuy(GameObject obj)
	{
		MaskWindow.LockUI ();
//		if (AndroidSDKManager.Instance.plantformID == PlantformID.MonoAndroid || AndroidSDKManager.Instance.plantformID == PlantformID.MonoIOS) {
////			UiManager.Instance.openDialogWindow<WeekCardBuyWindow> (( win ) => {
////				win.init (updateWeekCardInfo);
////			});
//			UiManager.Instance.openWindow<rechargeWindow> ();
//		}
//		else {
//			int limitLv = 0;
//			foreach (KeyValuePair<int,WeekCardSample> kv in WeekCardSampleManager.Instance.weekCards)
//			{
//				limitLv = kv.Value.limitLv;
//				break;
//			}
//			if(UserManager.Instance.self.getUserLevel () >= limitLv)
//			{
//				UiManager.Instance.openWindow<rechargeWindow> ();
//			}
//			else
//			{
//				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
//					win.Initialize (string.Format(LanguageConfigManager.Instance.getLanguage ("weekCardIntro4"),limitLv.ToString()));
//				});
//			}
//		}

		int limitLv = 0;
		foreach (KeyValuePair<int,WeekCardSample> kv in WeekCardSampleManager.Instance.weekCards)
		{
			limitLv = kv.Value.limitLv;
			break;
		}
		if(UserManager.Instance.self.getUserLevel () >= limitLv)
		{
			UiManager.Instance.openWindow<rechargeWindow> ();
		}
		else
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (string.Format(LanguageConfigManager.Instance.getLanguage ("weekCardIntro4"),limitLv.ToString()));
			});
		}
	}
	private void M_onClickReceive(GameObject obj)
	{
		if(canRecevie && selectPrizeSample != null && selectID != 0)
		{
			PrizeSample[] ps = prizeDic[selectID];
			if(!isPropStorageFull(ps))// 仓库有容量//
			{
				MaskWindow.LockUI ();
				WeekCardAwardFPort fPort=FPortManager.Instance.getFPort ("WeekCardAwardFPort") as WeekCardAwardFPort;
				fPort.access(selectID,awardCallBack);
				return;
			}
			if(isPropStorageFull(ps) && StorageManagerment.Instance.getAllTemp ().Count == 0)// 仓库满，临时仓库没东西//
			{
				MaskWindow.LockUI ();
				WeekCardAwardFPort fPort=FPortManager.Instance.getFPort ("WeekCardAwardFPort") as WeekCardAwardFPort;
				fPort.access(selectID,awardCallBack2);
				return;
			}
			if(isPropStorageFull(ps) && StorageManagerment.Instance.getAllTemp ().Count > 0)// 仓库满，临时仓库有东西//
			{
				// 提示临时仓库未清空，请清理后再来//
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("weekCard_msg2"));
				});
				return;
			}

		}
		else
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("weekCard_msg3"));
			});
		}
	}

	public void awardCallBack()
	{
		UiManager.Instance.createPrizeMessageLintWindow(prizeDic[selectID]);
		WeekCardInfo.Instance.recevieState = WeekCardRecevieState.recevied;
		updateReceiveButton();
		for(int i=0;i<prizeDic[selectID].Length;i++)
		{
			if(prizeDic[selectID][i].type == PrizeType.PRIZE_CARD)
			{
				Card card = CardManagerment.Instance.createCard(prizeDic[selectID][i].pSid);
				if(card != null)
				{
					if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed(card)) {
						StartCoroutine(Utils.DelayRun(() => {
							UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
								win.init(LanguageConfigManager.Instance.getLanguage("s0418"), 0.8f);
							});
						},0.7f));
					}
				}
			}
		}

		selectIcon.SetActive(false);
		selectID = 0;
		selectPrizeSample = null;
	}

	public void awardCallBack2()// 放临时仓库的回调//
	{
		awardCallBack();
		StartCoroutine (showMsg ("weekCard_msg1",2));
//		UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
//			win.Initialize (LanguageConfigManager.Instance.getLanguage ("weekCard_msg1"));
//		});
	}

	IEnumerator showMsg (string msg,int delayTime)
	{
		yield return new WaitForSeconds (delayTime);
		UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
			win.Initialize (LanguageConfigManager.Instance.getLanguage (msg));
		});
	}

	public void clickPrize(GameObject obj)
	{
		canRecevie = true;
		selectPrizeSample = obj.GetComponent<GoodsView>().prize;
		selectID = StringKit.toInt(obj.name);
		setSelectIcon(obj.transform);

	}
	public void onPressPrize(GameObject obj,bool isPress)
	{
		if(isPress)
		{
			clickPrize(obj);
			pressedPrizeView = obj.GetComponent<GoodsView>();
			isOnPressing = true;
		}
		else
		{
			isOnPressing = false;
			mTime = 0;
			pressedPrizeView = null;
		}
	}
	public void clickPrize2()
	{
		MaskWindow.UnlockUI();
	}
	public void setSelectIcon(Transform parent)
	{
		selectIcon.transform.parent = parent;
		selectIcon.transform.localScale = Vector3.one;
		selectIcon.transform.localPosition = new Vector3(0,4,0);
		selectIcon.SetActive(true);
	}

	//验证相关仓库是否满
	private bool isPropStorageFull (PrizeSample[] propArr)
	{
		PrizeSample prop;
		bool isfull = false;
		for(int i=0;i<propArr.Length;i++)
		{
			prop = propArr[i];
			if (prop == null)
				return false;
			switch (prop.type) {
			case PrizeType.PRIZE_CARD:
				if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllRole ().Count > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_BEAST:
				if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllBeast ().Count > StorageManagerment.Instance.getBeastStorageMaxSpace ()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_EQUIPMENT:
				if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllEquip ().Count > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_MAGIC_WEAPON:
				if (prop.getPrizeNumByInt() + StorageManagerment.Instance.getAllMagicWeapon().Count > StorageManagerment.Instance.getMagicWeaponStorageMaxSpace()) {
					isfull = true;
				} else {
					isfull = false;
				}
				break;
			case PrizeType.PRIZE_PROP:
				if (StorageManagerment.Instance.getProp (prop.pSid) != null) {
					isfull = false;
				} else {
					if (1 + StorageManagerment.Instance.getAllProp ().Count > StorageManagerment.Instance.getPropStorageMaxSpace ()) {
						isfull = true;
					} else {
						isfull = false;
					}
				}
				break;
			}
			return isfull;
		}
		return isfull;		
	}

	void Update()
	{
		if(WeekCardInfo.Instance.loginTime == 0)
		{
			WeekCardInfo.Instance.loginTime = ServerTimeKit.getLoginTime();
		}
		if(ServerTimeKit.getMillisTime() >= BackPrizeLoginInfo.Instance.getSecondDayTime(WeekCardInfo.Instance.loginTime))// 跨天//
		{
			WeekCardInfo.Instance.loginTime = ServerTimeKit.getMillisTime();

			WeekCardInfoFPort fPort = FPortManager.Instance.getFPort ("WeekCardInfoFPort") as WeekCardInfoFPort;
			fPort.WeekCardInfoAccess(updateWeekCardInfo);
		}
		if(isOnPressing)
		{
			mTime += 0.1f;
			if(mTime >= 3)
			{
				mTime = 0;
				isOnPressing = false;
				if(pressedPrizeView != null)
				{
					pressedPrizeView.DefaultClickEvent();
				}
			}
		}
	}

}

