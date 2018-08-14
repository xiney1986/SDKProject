using UnityEngine;
using System.Collections;

public class SevenDaysHappyAwardReceve : ButtonBase
{
	public SevenDaysHappyMisson misson;
	public SevenDaysHappyMissonContent missonContent;
	public SevenDaysHappyContent content;
	public SevenDaysHappyDetailBtn detailBtn;

	public override void DoClickEvent ()
	{
		if(misson != null)
		{
			if(misson.missonState == SevenDaysHappyMissonState.Doing)
			{
				if(misson.missonType == SevenDaysHappyMissonType.Recharge)// 充值弹出充值界面//
				{
					UiManager.Instance.openWindow<rechargeWindow> ();
				}
			}
			else
			{
				if(misson.prizeType == SevenDaysHappyPrizeType.Not_MoreChooseOne)// 非多选一奖励直接领取//
				{
					if(!isPropStorageFull(misson.prizes))// 仓库未满//
					{
						MaskWindow.LockUI ();
						SevenDaysHappyAwardFPort fPort=FPortManager.Instance.getFPort ("SevenDaysHappyAwardFPort") as SevenDaysHappyAwardFPort;
						fPort.access(misson.missonID,0,awardCallBack); 
					}
					else
					{
						// 飘字提示，仓库已满请清理//
						UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
							win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull"));
						});
					}
				}
				else// 多个奖励，多选一//
				{
//					UiManager.Instance.openDialogWindow<PrizesSelectWindow>((win)=>{
//						MaskWindow.UnlockUI ();
//						win.initWin(misson,missonContent,content,detailBtn);
//					});
					UiManager.Instance.openWindow<PrizesSelectWindow>((win)=>{
						//MaskWindow.UnlockUI ();
						win.initWin(misson,missonContent,content,detailBtn);
					},true);
				}
			}
		}
	}

	public void awardCallBack()
	{
		UiManager.Instance.createPrizeMessageLintWindow(misson.prizes);
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
		// 英雄之章//
		for(int i=0;i<misson.prizes.Length;i++)
		{
			if(misson.prizes[i].type == PrizeType.PRIZE_CARD)
			{
				Card card = CardManagerment.Instance.createCard(misson.prizes[i].pSid);
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
		// 切换任务为已领取状态//
		misson.missonState = SevenDaysHappyMissonState.Recevied;
		SevenDaysHappyManagement.Instance.sortMisson();
		if(missonContent != null && content != null)
		{
			if(content.selectedDetailBtn != null)
			{
				missonContent.destroyMissons();
				content.selectedDetailBtn.showMisson();
			}
		}
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
}
