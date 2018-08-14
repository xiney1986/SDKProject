using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//首冲奖励窗口
//李程

public class OneRmbContent : MonoBehaviour
{
	public GameObject content;
	public GameObject roleViewPrefab;
	public GameObject goodsViewPrefab;
	private RechargeSample sample;
	public NoticeWindow fatherWindow;
	public ButtonBase btn_receive;
	//public ButtonBase btn_recharge;
	public ButtonBase btn_vip;

	void Awake ()
	{
		btn_receive.onClickEvent = onButtonReceive;
		//btn_recharge.onClickEvent = onButtonRecharge;
		btn_vip.onClickEvent = onButtonVip;
	}

	public void initContent (NoticeWindow win)
	{
		fatherWindow = win;
		btn_receive.fatherWindow = win;
		//btn_recharge.fatherWindow = win;
		btn_vip.fatherWindow=win;
	}

	void Start ()
	{
		int oneRmbState=RechargeManagerment.Instance.getOneRmbState ();
		if (oneRmbState == RechargeManagerment.ONERMB_STATE_INVALID) {
						btn_receive.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0315");
						btn_receive.onClickEvent = onButtonRecharge;
				} else {
			btn_receive.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0323");
			btn_receive.onClickEvent = onButtonReceive;
		}
		//btn_receive.disableButton (oneRmbState != RechargeManagerment.ONERMB_STATE_VALID);
		if (oneRmbState == RechargeManagerment.ONERMB_STATE_FINISHED) {
			btn_receive.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
			btn_receive.disableButton (true);
		}
		this.sample = RechargeSampleManager.Instance.getRechargeSampleBySid (NoticeType.ONERMB_RECHARGE_SID);
		GameObject obj;
		GoodsView goodsButton;
		PrizeSample prize;
		for (int i = 0; i < this.sample.prizes.Length; i++) {
			prize = this.sample.prizes [i];
			if (prize == null) {
				continue;
			} 
			
			obj = NGUITools.AddChild (content, goodsViewPrefab);
			goodsButton = obj.GetComponent<GoodsView> ();
			goodsButton.fatherWindow = fatherWindow;
			goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
			
			switch (prize.type) {
			case PrizeType.PRIZE_BEAST:
				goodsButton.gameObject.SetActive (true);
				Card beast = CardManagerment.Instance.createCard (prize.pSid);
				goodsButton.init (beast,true);
				break;
			case PrizeType.PRIZE_CARD:
				goodsButton.gameObject.SetActive (true);
				Card card = CardManagerment.Instance.createCard (prize.pSid);
				goodsButton.init (card,true);
				break;
			case PrizeType.PRIZE_EQUIPMENT:
				goodsButton.gameObject.SetActive (true);
				Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
				goodsButton.init (equip,true);
				break;
			case PrizeType.PRIZE_MONEY:
				goodsButton.gameObject.SetActive (true);
				PrizeSample prizeMoney = new PrizeSample(PrizeType.PRIZE_MONEY,0,prize.num);
				goodsButton.init (prizeMoney,true);
				break;
			case PrizeType.PRIZE_PROP:
				goodsButton.gameObject.SetActive (true);
				Prop prop = PropManagerment.Instance.createProp (prize.pSid);
				goodsButton.init (prop, prize.getPrizeNumByInt (),true);
				break;
			case PrizeType.PRIZE_RMB:
				goodsButton.gameObject.SetActive (true);
				PrizeSample prizeRmb = new PrizeSample(PrizeType.PRIZE_RMB,0,prize.num);
				goodsButton.init (prizeRmb,true);
				break;
			}
		}
		content.GetComponent<UIGrid> ().Reposition ();
	}	
	/// <summary>
	/// 领取按钮
	/// </summary>
	private void onButtonReceive (GameObject obj)
	{
		int oneRmbState=RechargeManagerment.Instance.getOneRmbState ();
		if (oneRmbState==RechargeManagerment.ONERMB_STATE_INVALID) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0325"));
			return;
		}
		if (oneRmbState==RechargeManagerment.ONERMB_STATE_FINISHED) {
			btn_receive.disableButton(true);
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("recharge02"));
			btn_receive.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
			return;
		}
		string str = "";
		if (StorageManagerment.Instance.checkStoreFull (sample.prizes, out str)) {
			//仓库满提示
			MessageWindow.ShowAlert (str + "," + LanguageConfigManager.Instance.getLanguage ("s0203"));
			return;
			
		} else {
			NoticeGetActiveAwardFPort fport = FPortManager.Instance.getFPort ("NoticeGetActiveAwardFPort") as NoticeGetActiveAwardFPort;
			fport.access (RechargeManagerment.Instance.getOneRmb ().sid, getGiftSuccess);
		}

	}

	/// <summary>
	/// 去充值
	/// </summary>
	private void onButtonRecharge (GameObject obj)
	{
		UiManager.Instance.openWindow<rechargeWindow> ();		
	}

	/// <summary>
	/// 查看vip特权
	/// </summary>
	/// <param name="obj">Object.</param>
	private void onButtonVip(GameObject obj)
	{
		UiManager.Instance.openWindow<VipWindow> ();		
	}

	/// <summary>
	///	领取成功后
	/// </summary>
	void getGiftSuccess (bool b) {
		if (b) {
			RechargeManagerment.Instance.getOneRmb ().addCount (1);
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0326"), msgBack);
			btn_receive.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
			btn_receive.disableButton (true);
			fatherWindow.initTopButton (true, 0,NoticeEntranceType.DAILY_NOTICE);
		}
		else {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0203"));
		}
	}
	public void msgBack (MessageHandle msg)
	{
		if (HeroRoadManagerment.Instance.isOpenHeroRoad (sample.prizes)) {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("HeroRoad_open"));
		}
	}
}
