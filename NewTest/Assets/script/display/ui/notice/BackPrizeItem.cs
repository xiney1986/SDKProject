using UnityEngine;
using System.Collections;

public class BackPrizeItem : MonoBehaviour
{
	BackPrize bp;
	public UISprite awardButtonBg;// 领取按钮背景 //
	public UILabel awardButtonLabel;// 领取按钮文本//
	public UILabel todayTitle;// 回归第几天文字//
	public GoodsView[] goods;// 奖励物品//
	public ButtonBase awardButton;// 领取按钮//

	string recevied_spriteName = "button_big1_gray";// 已领取按钮背景priteName//
	string recevie_spriteName = "button_big1";// 领取按钮背景priteName//

	string str = "";
	WindowBase fatherWin;

	NoticeTopButton button;

	// item的更新和初始化//
	public void updateItem(BackPrize bp,WindowBase win,NoticeTopButton button)
	{
		if(button != null)
		{
			this.button = button;
		}
		this.fatherWin = win;
		this.bp = bp;
		awardButton.fatherWindow = win;
		awardButton.onClickEvent = receviedClick;

		if(bp != null)
		{
			if(bp.isRecevied == BackPrizeRecevieType.RECEVIE)// 领取//
			{
				awardButton.disableButton(false);
				//awardButtonBg.spriteName = recevie_spriteName;
				awardButtonLabel.text = LanguageConfigManager.Instance.getLanguage("s0309");
			}
			else if(bp.isRecevied == BackPrizeRecevieType.RECEVIED)// 已领取//
			{
				awardButton.disableButton(true);
				//awardButtonBg.spriteName = recevied_spriteName;
				awardButtonLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
			}
			else if(bp.isRecevied == BackPrizeRecevieType.CANT_RECEVIE)// 不能领取//
			{
				awardButton.disableButton(true);
				//awardButtonBg.spriteName = recevied_spriteName;
				awardButtonLabel.text = LanguageConfigManager.Instance.getLanguage("s0309");
			}

			todayTitle.text = bp.dayID.ToString();

			for(int i=0;i<bp.prizes.Length;i++)
			{
				goods[i].gameObject.SetActive(true);
				goods[i].init(bp.prizes[i]);
				goods[i].fatherWindow = win;
			}

		}
	}

	// 领取点击事件//
	public void receviedClick(GameObject obj)
	{
		if(bp != null)
		{
			if(bp.isRecevied == BackPrizeRecevieType.RECEVIED || bp.isRecevied == BackPrizeRecevieType.CANT_RECEVIE)
			{
				return;
			}
			//若果临时仓库有东西时，不能领取
			if (StorageManagerment.Instance.getAllTemp ().Count > 0) {
				UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull_temp_tip"));
				});
				return;
			}
			// 处理领取逻辑//
			(FPortManager.Instance.getFPort ("BackPrizeSendFPort") as BackPrizeSendFPort).access (bp.dayID, receiveCallBack);
		}
	}

	// 领奖成功回调//
	public void receiveCallBack()
	{
		// 飘字//
		showPrize();

		bp.isRecevied = BackPrizeRecevieType.RECEVIED;
		BackPrizeLoginInfo.Instance.prizeList[bp.dayID - 1] = bp;
		BackPrizeLoginInfo.Instance.receivedDays.Add(bp.dayID);
		updateItem(bp,fatherWin,null);
		if(button != null)
		{
			button.updateTime();
		}

		// 英雄之章//
		for(int i=0;i<bp.prizes.Length;i++)
		{
			if(bp.prizes[i].type == PrizeType.PRIZE_CARD)
			{
				Card card = CardManagerment.Instance.createCard(bp.prizes[i].pSid);
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
	}

	public void showPrize()
	{
		if(bp != null)
		{			
			if(bp.prizes.Length > 0)
			{
				UiManager.Instance.createPrizeMessageLintWindow(bp.prizes);
			}
		}
	}

	//验证相关仓库是否满
	private bool isPropStorageFull (PrizeSample prop)
	{
		if (prop == null)
			return false;
		bool isfull = false;
		switch (prop.type) {
		case PrizeType.PRIZE_CARD:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllRole ().Count > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0193"));
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_BEAST:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllBeast ().Count > StorageManagerment.Instance.getBeastStorageMaxSpace ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0194"));
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllEquip ().Count > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0195"));
				isfull = true;
			} else {
				isfull = false;
			}
			break;
        case PrizeType.PRIZE_MAGIC_WEAPON:
            if (prop.getPrizeNumByInt() + StorageManagerment.Instance.getAllMagicWeapon().Count > StorageManagerment.Instance.getMagicWeaponStorageMaxSpace()) {
                str = LanguageConfigManager.Instance.getLanguage("s0192", LanguageConfigManager.Instance.getLanguage("s0195"));
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
					str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0196"));
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
