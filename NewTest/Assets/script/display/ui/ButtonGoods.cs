using System;
using UnityEngine;
public class ButtonGoods:ButtonBase
{
	public Goods goods;
	public CallBackMsg callback;
 
	public override void OnAwake ()
	{
		this.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0014");  
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		int sid = GoodsSampleManager.Instance.getGoodsSampleBySid(goods.sid).goodsSID;
		if (BeastEvolveManagerment.Instance.isSameBeastGoods(sid)) {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Arena57"));
			return;
		}
		if(goods.getGoodsShopType()==ShopType.MYSTICAL_SHOP){
			string money;
			money=goods.getCostPrice().ToString();
			string moneyName="";
			switch(goods.getCostType())
			{
			case PrizeType.PRIZE_MONEY:
				moneyName=LanguageConfigManager.Instance.getLanguage("resources_money_name");
				break;
			case PrizeType.PRIZE_RMB:
				moneyName=LanguageConfigManager.Instance.getLanguage("s0131");
				break;
			case PrizeType.PRIZE_PROP:
				moneyName=PropSampleManager.Instance.getPropSampleBySid (goods.getCostToolSid()).name;
				break;
			default:
				break;
			}
			if(!ShopManagerment.Instance.isOpenOneKey){
				UiManager.Instance.openDialogWindow<ShopMessageWindow> ((win) => {
					win.msg.msgInfo=goods;
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("shop_message_for_buy",money,moneyName,goods.getName(),goods.getGoodsShowNum().ToString()), callback);
				});
			}else{
				MessageHandle msg = new MessageHandle();
				msg.msgInfo = goods;
				callback(msg);
			}
		
		} else if (goods.getGoodsShopType()==ShopType.LADDER_HEGOMONEY) {

			Notice notice = NoticeManagerment.Instance.getNoticeListByType (NoticeType.LADDER_HEGEMONY,NoticeEntranceType.LIMIT_NOTICE) ;
			LadderHegemoneyNotice ladderNotice = notice as LadderHegemoneyNotice;
			
			if (ladderNotice != null &&  !ladderNotice.isInTimeLimit ()) {
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("ladderruleprize5"));
				return;
			}
			
			if (ladderNotice != null && ladderNotice.isValid()) {
			} else {
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0171"));
				return;
			}

			int count=goods.getNum();
			int max0 = (goods.getGoodsMaxBuyCount() - goods.getNowBuyNum());
			int singelPrice = Mathf.RoundToInt(goods.getCostPrice()/goods.getGoodsShowNum());
			int max = Mathf.RoundToInt(LadderHegeMoneyManager.Instance.myPort/singelPrice);
			max = max0 < max? max0:max;
			if(count<0)
			{

				UiManager.Instance.openDialogWindow<BuyWindow>((win)=>{
					win.init (goods,max,1,goods.getCostType(), callback); 
				});
			}
			else
				UiManager.Instance.openDialogWindow<BuyWindow>((win)=>{
					win.init (goods,max,1,goods.getCostType(), callback); 
				});
		}else if(goods.getGoodsShopType()==ShopType.TEHUI_SHOP){
			MessageHandle msg = new MessageHandle();
			callback(msg);
		}else if(goods.getGoodsShopType()==ShopType.NVSHEN_SHOP){
            int count = goods.getNum();
            if (count < 0)
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow40"));
                });
            else {
                int typee = goods.getCostType();
                if (typee == PrizeType.PRIZE_PROP) {
                    int propSid = goods.getCostToolSid();
                    Prop p = StorageManagerment.Instance.getProp(propSid);
                    if(p==null||p.getNum()<goods.getCostPrice()){

                        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                            win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow41",PropManagerment.Instance.createProp(propSid).getName()));
                        });
                        return;
                    }
                    UiManager.Instance.openDialogWindow<BuyWindow>((win) => {
                        win.init(goods, count >= p.getNum() / goods.getCostPrice() ? p.getNum() / goods.getCostPrice() : count, 1, goods.getCostType(), callback);
                    });
                }
               
            }
        } else if (goods.getGoodsShopType() == ShopType.HEROSYMBOL_SHOP) {
            int count = goods.getNum();
            if (count < 0)
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow40"));
                });
            else {
                int typee = goods.getCostType();
                if (typee == PrizeType.PRIZE_PROP) {
                    int propSid = goods.getCostToolSid();
                    Prop p = StorageManagerment.Instance.getProp(propSid);
                    if (p == null || p.getNum() < goods.getCostPrice()) {

                        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                            win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow41", PropManagerment.Instance.createProp(propSid).getName()));
                        });
                        return;
                    }
                    UiManager.Instance.openDialogWindow<BuyWindow>((win) => {
                        win.init(goods, count >= p.getNum() / goods.getCostPrice() ? p.getNum() / goods.getCostPrice() : count, 1, goods.getCostType(), callback);
                    });
                }
            }
		}else if (goods.getGoodsShopType() == ShopType.JUNGONG_SHOP) {
			int count = goods.getNum();
			if (count < 0)
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
					win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow40"));
				});
			else {
				int typee = goods.getCostType();
				if (typee == PrizeType.PRIZE_PROP) {
					int propSid = goods.getCostToolSid();
					Prop p = StorageManagerment.Instance.getProp(propSid);
					if (p == null || p.getNum() < goods.getCostPrice()) {
						
						UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
							win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow41", PropManagerment.Instance.createProp(propSid).getName()));
						});
						return;
					}
					UiManager.Instance.openDialogWindow<BuyWindow>((win) => {
						win.init(goods, count >= p.getNum() / goods.getCostPrice() ? p.getNum() / goods.getCostPrice() : count, 1, goods.getCostType(), callback);
					});
				}
			}
		}else {
            int count = goods.getNum();
            if (count < 0)
                UiManager.Instance.openDialogWindow<BuyWindow>((win) => {
                    win.init(goods, goods.getCostType(), callback);
                });

            else
                UiManager.Instance.openDialogWindow<BuyWindow>((win) => {
                    win.init(goods, count, 1, goods.getCostType(), callback);
                });
        }

			
	}
} 

