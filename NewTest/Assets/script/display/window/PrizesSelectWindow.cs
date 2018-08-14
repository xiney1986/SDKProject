using UnityEngine;
using System.Collections.Generic;

public class PrizesSelectWindow : WindowBase
{
	public GameObject twoPrizesPanel;// 显示两个奖励//
	public GameObject threePrizesPanel;// 显示三个奖励//
	public GoodsView[] twoPrizes;
	public GoodsView[] threePrizes;

	public UILabel twoPrizes_goods_1_label;
	public UILabel twoPrizes_goods_2_label;

	public UILabel threePrizes_goods_1_label;
	public UILabel threePrizes_goods_2_label;
	public UILabel threePrizes_goods_3_label;

	public PrizeSelectBtn old_prizeSelectBtn;

	SevenDaysHappyMisson misson;

	SevenDaysHappyMissonContent missonContent;
	SevenDaysHappyContent content;
	SevenDaysHappyDetailBtn detailBtn;

	public UILabel tittle;

	public ButtonBase awardBtn;

	protected override void begin ()
	{
		base.begin ();
		UIEventListener.Get(gameObject).onClick = closeWin;
		MaskWindow.UnlockUI();
	}

	void closeWin(GameObject obj)
	{
		finishWindow ();
	}

	public void initWin(SevenDaysHappyMisson _misson,SevenDaysHappyMissonContent missonContent,SevenDaysHappyContent content,SevenDaysHappyDetailBtn detailBtn)
	{
		this.detailBtn = detailBtn;
		this.misson = _misson;
		this.missonContent = missonContent;
		this.content = content;
		string str = "";
		if(misson.prizes.Length == 2)// 两个奖励//
		{
			str = "2";
			twoPrizesPanel.SetActive(true);
			threePrizesPanel.SetActive(false);
			for(int i=0;i<2;i++)
			{
				twoPrizes[i].init(misson.prizes[i]);
				twoPrizes[i].fatherWindow = this;
			}
			twoPrizes_goods_1_label.text = twoPrizes[0].showName;
			twoPrizes_goods_2_label.text = twoPrizes[1].showName;
		}
		else if(misson.prizes.Length == 3)//  三个奖励//
		{
			str = "3";
			twoPrizesPanel.SetActive(false);
			threePrizesPanel.SetActive(true);
			for(int i=0;i<3;i++)
			{
				threePrizes[i].init(misson.prizes[i]);
				threePrizes[i].fatherWindow = this;
			}
			threePrizes_goods_1_label.text = threePrizes[0].showName;
			threePrizes_goods_2_label.text = threePrizes[1].showName;
			threePrizes_goods_3_label.text = threePrizes[2].showName;
		}
	
		tittle.text = string.Format(LanguageConfigManager.Instance.getLanguage("sevenDaysHappy_awardTittle"),str);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if(gameObj.name == "button_1")// 点击领奖//
		{
			if(old_prizeSelectBtn == null)// 飘字提示请选择一种奖励//
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("weekCard_msg3"));
				});
			}
			else
			{
				if(!isPropStorageFull(old_prizeSelectBtn.good.prize))// 仓库未满//
				{
					MaskWindow.LockUI ();
					SevenDaysHappyAwardFPort fPort=FPortManager.Instance.getFPort ("SevenDaysHappyAwardFPort") as SevenDaysHappyAwardFPort;
					fPort.access(misson.missonID,old_prizeSelectBtn.good.prize.pSid,awardCallBack); 
				}
				else
				{
					// 飘字提示，仓库已满请清理//
					UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull"));
					});
				}
			}
		}
	}

	public void awardCallBack()
	{
		SevenDaysHappyManagement.Instance.setIsPrizesSelectWin(true);
		// 切换任务为已领取状态//
		misson.missonState = SevenDaysHappyMissonState.Recevied;
		SevenDaysHappyManagement.Instance.sortMisson();
		if(SevenDaysHappyManagement.Instance.canReceviedCount > 0)
		{
			SevenDaysHappyManagement.Instance.canReceviedCount--;
		}
		if(SevenDaysHappyManagement.Instance.dayIDAndCount[misson.dayID] > 0)
		{
			SevenDaysHappyManagement.Instance.dayIDAndCount[misson.dayID]--;
		}
		if(detailBtn.canReceivedCount > 0)
		{
			detailBtn.canReceivedCount--;
		}

		awardBtn.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
		awardBtn.disableButton(true);
		UiManager.Instance.createPrizeMessageLintWindow(old_prizeSelectBtn.good.prize);

		// 英雄之章//
		if(old_prizeSelectBtn.good.prize.type == PrizeType.PRIZE_CARD)
		{
			Card card = CardManagerment.Instance.createCard(old_prizeSelectBtn.good.prize.pSid);
			if(card != null)
			{
				if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed(card)) {
					StartCoroutine(Utils.DelayRun(() => {
						UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
							win.init(LanguageConfigManager.Instance.getLanguage("s0418"), 0.8f);
							finishWindow ();
						});
					},0.7f));
				}
			}
		}

//		if(missonContent != null && content != null)
//		{
//			if(content.selectedDetailBtn != null)
//			{
//				missonContent.destroyMissons();
//				content.selectedDetailBtn.showMisson();
//			}
//		}
	}

	//验证相关仓库是否满
	private bool isPropStorageFull (PrizeSample prop)
	{
		bool isfull = false;
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

}
