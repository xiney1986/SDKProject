using UnityEngine;
using System;
using System.Collections;

public class ButtonUseProp : ButtonBase
{
	public Prop prop;
	private const int LEVEL = 8;//等级8级
	int useCount;
	private int type;
	public void initButton (Prop prop)
	{
		this.prop = prop;
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		GuideManager.Instance.doFriendlyGuideEvent ();

        PropSample sample = PropSampleManager.Instance.getPropSampleBySid(prop.sid);
        if (UserManager.Instance.self.getUserLevel() < prop.getUseLv() && sample.type != PropType.PROP_COMBAT_CHEST) {
            UiManager.Instance.openDialogWindow<MessageWindow>((window) => {
                window.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"), null, LanguageConfigManager.Instance.getLanguage("s0331", prop.getUseLv().ToString()), null);
            });
            return;
        }
		if (sample.type == PropType.PROP_TYPE_CHEST) {
			//若果临时仓库有东西时，不能打开宝箱，并飘字提示玩家
			if (StorageManagerment.Instance.getAllTemp ().Count > 0) {
				UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull_temp_tip"));
				});
				return;
			}
			//如果数量只有一个 则直接使用,不用去选择数量
			if (prop.getNum () == 1) {
				useProp (1);
				return;
			}

			GuideManager.Instance.doGuide (); 
			UiManager.Instance.openDialogWindow<BuyWindow> ((win) => {
				win.init (prop, prop.getNum (), 1, prop.getNum () > 50 ? 50 : prop.getNum (), 1, constResourcesPath.NONE, useBack); 
			});

		} else if (sample.type == PropType.PROP_TYPE_LOCK_CHEST) {//带锁的宝箱打开界面
			UiManager.Instance.openWindow<TreasureChestWindow> ((win) => {
				win.init (prop.sid);
			});
		}
		/** 使用改名卡 */
		else if (sample.type == PropType.PROP_RENAME) {
			UiManager.Instance.openDialogWindow<RoleRenameWindow>((win)=>{
				win.sample = sample;
			});
		}else if (sample.type==PropType.PROP_HAFE_MONTH)
		{
            OpenGiftBagFport fport = FPortManager.Instance.getFPort("OpenGiftBagFport") as OpenGiftBagFport;
            fport.access(1,prop, () =>
            {
                NoticeMonthCardFPort sp = FPortManager.Instance.getFPort("NoticeMonthCardFPort") as NoticeMonthCardFPort;
                sp.access_get(() =>
                {
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((winn) => {
                    winn.Initialize(LanguageConfigManager.Instance.getLanguage("month_hafe_add"));
                    StoreWindow store = UiManager.Instance.getWindow<StoreWindow>();
                    if (store != null) {
                        store.updateContent();
                    }
                });
                });
                

                //int[] monthCardDueDate=NoticeManagerment.Instance.monthCardDueDate;
                //if (monthCardDueDate == null || monthCardDueDate.Length == 0)
                //{
                //    int currentTime = ServerTimeKit.getSecondTime();
                //    DateTime time = TimeKit.getDateTime(currentTime + 1296000);
                //    NoticeManagerment.Instance.monthCardDueDate = new int[3]
                //    {
                //        time.Year,
                //        time.Month,
                //        time.Day
                //    };
                //    GameManager.Instance.monthTime = currentTime + 1296000;
                //}
                //else
                //{
                //    DateTime t = TimeKit.getDateTime(GameManager.Instance.monthTime + 1296000);
                //    NoticeManagerment.Instance.monthCardDueDate = new int[3]
                //    {
                //        t.Year,
                //        t.Month,
                //        t.Day
                //    };
                //}
                
            });
        } else if (sample.type == PropType.PROP_COMBAT_CHEST) {
            if (StorageManagerment.Instance.getAllTemp().Count > 0) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("storeFull_temp_tip"));
                });
                return;
            }
            useChest();
        }
	}
	
	private void sendInfoBack (Award[] award)
	{
	    UiManager.Instance.openDialogWindow<AllAwardViewWindow>(win => {
			win.Initialize (award, LanguageConfigManager.Instance.getLanguage ("s0206", prop.getName (),useCount.ToString()));
			openHeroRoad(award);
		});
	}
	
	private void addAward ()
	{
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_BOX, sendInfoBack);
		
	}

	void useBack (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogOK) 
		{
			useProp(msg.msgNum);
		}
	}
	private void  useProp(int count)
	{
		useCount=count;
		if (prop.getUseLv () <= UserManager.Instance.self.getUserLevel () && !StorageManagerment.Instance.isPropStorageFull (prop)) {
			OpenGiftBagFport fport = FPortManager.Instance.getFPort ("OpenGiftBagFport") as OpenGiftBagFport;
			fport.access (useCount, prop, addAward);
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, "user level limit", null);
			});
		}
	}
    private void useChest() {

        useCount = 1;
        ArmyManager.Instance.EditArmy1 = ArmyManager.Instance.DeepClone(ArmyManager.Instance.getArmy(1));
        ArmyManager.Instance.EditArmy3 = ArmyManager.Instance.DeepClone(ArmyManager.Instance.getArmy(3));
        int maoXian = ArmyManager.Instance.EditArmy1.getAllCombat();
        int jinJi = ArmyManager.Instance.EditArmy3.getAllCombat();
        if (Mathf.Max(maoXian, jinJi) >= prop.getUseCombat()) {
            OpenGiftBagFport fport = FPortManager.Instance.getFPort("OpenGiftBagFport") as OpenGiftBagFport;
            fport.access(1, prop, addAward);
        } else {
            UiManager.Instance.openDialogWindow<MessageWindow>((window) => {
                window.initWindow(1, LanguageConfigManager.Instance.getLanguage("s0093"), null, LanguageConfigManager.Instance.getLanguage("s0331s", prop.getUseCombat().ToString()), null);
            });
            return;
        }
    }

	private void openHeroRoad (Award[] award){
		PrizeSample[] prizes = AllAwardViewManagerment.Instance.exchangeAwards(award);
		bool isOpen=HeroRoadManagerment.Instance.isOpenHeroRoad (prizes);
		if (isOpen) {
			TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0418"));
		}
	}
}
