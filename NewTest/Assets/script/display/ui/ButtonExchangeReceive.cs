using UnityEngine;
using System.Collections;

public class ButtonExchangeReceive : ButtonBase
{
	
	public Exchange exchange;
	private string exchangeNames; //兑换消耗条件名
	private string exchangeValues;//兑换消耗数量
	public void updateButton (Exchange exchange)
	{
		this.exchange = exchange;
		
	}

	bool checkStoreFull ()
	{
		bool full = false;
		switch (exchange.getExchangeSample ().type) {
		case PrizeType.PRIZE_CARD:
			if (StorageManagerment.Instance.isRoleStorageFull (1))
				full = true;
			return full;
		case PrizeType.PRIZE_EQUIPMENT:
			if (StorageManagerment.Instance.isEquipStorageFull (1))
				full = true;
			return full;
		case PrizeType.PRIZE_PROP:
			if (StorageManagerment.Instance.isPropStorageFull (1))
				full = true;
			return full;
        case PrizeType.PRIZE_MAGIC_WEAPON:
            if (StorageManagerment.Instance.isMagicWeaponStorageFull(1))
            full = true;
            return full;
		}
		
		return full;
		
		
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		int count = ExchangeManagerment.Instance.getCanExchangeNum (exchange);
		if (count <= 0) {
			MaskWindow.UnlockUI();
			return;
		}
		if (exchange != null)
        {
			if (checkStoreFull ()) {
//				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage ("s0393"));
				UiManager.Instance.openDialogWindow<MessageWindow>((msgwin)=>{
					msgwin.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("s0393"), null);
				});
				return;
			}
			GuideManager.Instance.doGuide(); 
			if (fatherWindow is ExChangeWindow) {
				getExchangeTipContent(exchange);
				UiManager.Instance.openDialogWindow<BuyWindow>((win)=>{
					win.setExchangeTipsContent(exchangeNames,exchangeValues);
					win.init (exchange, getPracticalNum (exchange.getExchangeSample (), count),1,constResourcesPath.NONE, (fatherWindow as ExChangeWindow) .sureExchange);
					win.dialogCloseUnlockUI=false;
				});

			} else if (fatherWindow is NoticeWindow) {
				UiManager.Instance.openDialogWindow<BuyWindow>((win)=>{
					win.init (exchange, getPracticalNum (exchange.getExchangeSample (), count),1,constResourcesPath.NONE,  (fatherWindow as NoticeWindow).show.GetComponent<NoticeActivityExchangeContent>().receiveGift);
					win.dialogCloseUnlockUI=false;
				});

			} else if (fatherWindow is CardStoreWindow) {
				bool isNew = false;
				ExchangeSample sample = exchange.getExchangeSample ();
				Card c = null;
				if (sample.type == PrizeType.PRIZE_CARD) {
					c = CardManagerment.Instance.createCard (sample.exchangeSid);
				}
				(fatherWindow as CardStoreWindow).exchange(exchange.sid,c);

            } 
            else if (fatherWindow is StoreWindow) {
                if (exchange.getExchangeSample().type == PrizeType.PRIZE_MAGIC_WEAPON) {
                    ExchangeFPort exf = FPortManager.Instance.getFPort("ExchangeFPort") as ExchangeFPort;
                    exf.exchange(exchange.sid, 1, showLuckEffect);
                }else
				(fatherWindow as StoreWindow).exchange(exchange.sid);
			}else if(fatherWindow is MagicWeaponStoreWindow){
                ExchangeFPort exf = FPortManager.Instance.getFPort("ExchangeFPort") as ExchangeFPort;
                exf.exchange(exchange.sid, 1, showLuckEffect);
            }
			
		}
	}
    /// <summary>
    /// 这里开始播放抽卡得特效
    /// </summary>
    private void showLuckEffect(int sid,int num) {
        //做一个假的数据
        LuckyDrawPrize ld = new LuckyDrawPrize();
        ld.sourceType = "";
        ld.type = LuckyDrawPrize.TYPE_MAGIC_WEAPON;
        ld.num = 1;
        ld.sid = exchange.getExchangeSample().exchangeSid;
        ld.uid = "";
        LuckyDrawResults ldr = new LuckyDrawResults();
        ldr.setPrizes(ld);
        UiManager.Instance.openWindow<LuckyDrawShowWindow>(
            (windown) => {
            windown.init(ldr, null);
        });
    }
	//得到最终数量
	private int getPracticalNum (ExchangeSample sample, int num)
	{
		switch (sample.type) {
		case PrizeType.PRIZE_CARD:
			if (StorageManagerment.Instance.isRoleStorageFull (num)) {
				return StorageManagerment.Instance.getRoleStorageMaxSpace () - StorageManagerment.Instance.getAllRole().Count;
			} else {
				return num;
			}
		case PrizeType.PRIZE_EQUIPMENT:
			if (StorageManagerment.Instance.isEquipStorageFull (num)) {
				return StorageManagerment.Instance.getEquipStorageMaxSpace () - StorageManagerment.Instance.getAllEquip().Count;
			} else {
				return num;
			}
		case PrizeType.PRIZE_PROP:
			if (StorageManagerment.Instance.isPropStorageFull (sample.exchangeSid)) {
				return StorageManagerment.Instance.getPropStorageMaxSpace () - StorageManagerment.Instance.getAllProp().Count;
			} else {
				return num;
			}
		default:
			return num;
		}
		
			
	}
	/// <summary>
	/// 获取兑换消耗条件
	/// </summary>
	public void getExchangeTipContent(Exchange exchange){
		ExchangeSample sample = exchange.getExchangeSample ();
		exchangeNames = "";
		exchangeValues = "";
		for (int i=0; i<sample.conditions[0].Length; i++) {
			exchangeNames += sample.conditions[0] [i].getName () + "#";
			exchangeValues += sample.conditions[0] [i].num + "#";
		}
		exchangeNames = exchangeNames.Substring (0,exchangeNames.Length-1);
		exchangeValues = exchangeValues.Substring (0,exchangeValues.Length-1);
	}
}
