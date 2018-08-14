using UnityEngine;
using System.Collections;

/**
 * 行动力不足窗
 * @author 汤琦
 * */
public class PveUseWindow : WindowBase
{
	public barCtrl bar;
	public UILabel barValue;
	public UILabel timeValue;
	public UITexture restoreAll;
	public UITexture restoreOne;
	public UILabel restoreAllNum;
	public UILabel restoreOneNum;
	public UILabel useCount;
	private Timer timer;//刷新PVP倒计时
	private const int PVE = 71001;
	private const int PVES = 76005;
	private const int restoreAllIconId = 13;
	private  int restorePveAll;//行动力药水值
	private CallBack callback;
	private Goods goods;
	private bool cantUsePVES; //如果为真,则是达到每日使用上限
	private int maxUseCount; //玩家能使用的最大次数;
	public ButtonBase buttonAll;
	public ButtonBase buttonOne;
	public ButtonBase restoreAllProp;
	public UILabel pveLimitTip;//行动力可超上限提示
	public UILabel totalRMB;
	public UILabel costRMB;
	public UISprite RMB_1;
	public UISprite RMB_2;
	private int PveUseCount;//每日大行动力药水已经使用次数
	public UILabel label_tip;
	public UILabel pveUse_tip;//行动力药剂增加值提示

	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateWindow ();
	}
	
	protected override void begin ()
	{
		base.begin ();
        UiManager.Instance.pveUseWindow = this;
        getBaseData();

	}

	public void getBaseData ()
	{
		//显示限用次数
		restorePveAll= PropSampleManager.Instance.getPropSampleBySid (PropType.PROP_PVES).effectValue;
		maxUseCount = PropSampleManager.Instance.getPropSampleBySid (PropType.PROP_PVES).maxUseCount;
		if (UserManager.Instance.self.vipLevel > 0) {
			maxUseCount += VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.vipLevel).privilege.pvePropUseCountAdd;
		}
		GetPropUseInfoFPort port = FPortManager.Instance.getFPort ("GetPropUseInfoFPort") as GetPropUseInfoFPort;
		port.access (PropType.PROP_PVES, (count) =>
		{
			useCount.gameObject.SetActive (true);
			useCount.text = string.Format (LanguageConfigManager.Instance.getLanguage ("s0365"), count, maxUseCount);
			cantUsePVES = count >= maxUseCount;
			PveUseCount = count;
            if (timer != null) {
                timer.stop();
                timer = null;
            }
			updateWindow ();
			MaskWindow.UnlockUI();
		});
	}
    public void init(int count) {
        restorePveAll = PropSampleManager.Instance.getPropSampleBySid(PropType.PROP_PVES).effectValue;
        maxUseCount = PropSampleManager.Instance.getPropSampleBySid(PropType.PROP_PVES).maxUseCount;
        if (UserManager.Instance.self.vipLevel > 0) {
            maxUseCount += VipManagerment.Instance.getVipbyLevel(UserManager.Instance.self.vipLevel).privilege.pvePropUseCountAdd;
        }
        useCount.gameObject.SetActive(true);
        useCount.text = string.Format(LanguageConfigManager.Instance.getLanguage("s0365"), count, maxUseCount);
        cantUsePVES = count >= maxUseCount;
        PveUseCount = count;
        updateWindow();
        MaskWindow.UnlockUI();
    }

	public void updateTipInfo (string _info)
	{
		label_tip.text = _info; 
	}
	
	public void updateWindow ()
	{
		updatePVENum ();
		initTexture ();
		initPower ();
		startTimer ();
	}

	public void startTimer ()
	{
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
		timer.addOnTimer (showPlayerInfo);
		timer.start ();
		timeValue.gameObject.SetActive (true);
	}

	/// <summary>
	/// 更新药水的信息
	/// </summary>
	private void updatePVENum ()
	{
		Prop p = StorageManagerment.Instance.getProp (PropType.PROP_PVES);
		if (p != null && p.getNum () > 0) {
			restoreAllNum.text = p.getNum ().ToString () + "/1";
			buttonAll.textLabel.text = LanguageConfigManager.Instance.getLanguage ("pveUse04");
			buttonAll.disableButton (false);
		} else {
			restoreAllNum.text = "[FF0000]0/1";
			buttonAll.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0233");
			buttonAll.disableButton (false);
		}
		p = StorageManagerment.Instance.getProp (PropType.PROP_PVE);
		if (p != null && p.getNum () > 0) {
			restoreOneNum.text = p.getNum ().ToString () + "/1";
			buttonOne.disableButton (false);

		} else {
			restoreOneNum.text = "[FF0000]0/1";
			buttonOne.disableButton (true);
		}
		

	}
	
	private void initTexture ()
	{
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + PropSampleManager.Instance.getPropSampleBySid (PVE).iconId, restoreOne);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + restoreAllIconId, restoreAll);
		restoreOne.gameObject.SetActive (true);
		restoreAll.gameObject.SetActive (true);

	}
	
	private void showPlayerInfo ()
	{ 
		timeValue.text = UserManager.Instance.getNextPveTime ();
		initPower ();
	}
	
	private void initPower ()
	{
		pveLimitTip.text = LanguageConfigManager.Instance.getLanguage ("pveUse05", CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample> (CommonConfigSampleManager.PvePowerMax_SID).pvePowerMax.ToString ());
		bar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
		barValue.text = UserManager.Instance.self.getTotalPvEPoint () + "/" + UserManager.Instance.self.getPvEPointMax ();
		pveUse_tip.text= LanguageConfigManager.Instance.getLanguage("pveUse02", restorePveAll.ToString ());
		//检查包裹里是否有大行动力药剂，没有就显示购买界面
		if (!StorageManagerment.Instance.checkProp (PropType.PROP_PVES, 1)) {
			costRMB.gameObject.SetActive (true);
			totalRMB.gameObject.SetActive (true);
			RMB_1.gameObject.SetActive (true);
			RMB_2.gameObject.SetActive (true);
			restoreAllProp.gameObject.SetActive (false);
			totalRMB.text = LanguageConfigManager.Instance.getLanguage ("pveUse08", UserManager.Instance.self.getRMB ().ToString ());
			//获取次数对应的钻石消耗
			int costRmb = CommonConfigSampleManager.Instance.getSampleBySid<PveBuyCostRMB> (CommonConfigSampleManager.PveBuyCostRMB_SID).getCostByCount (PveUseCount);
			costRMB.text = LanguageConfigManager.Instance.getLanguage ("pveUse07", costRmb.ToString ());
			//更新主界面的RMB显示
			if (UiManager.Instance.mainWindow != null) {
				UiManager.Instance.mainWindow.update_RMB ();
			}

		}
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		int totalPveMax = CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample> (CommonConfigSampleManager.PvePowerMax_SID).pvePowerMax;//总的存储行动力上限，默认为600
		base.buttonEventBase (gameObj);
		// 使用或购买大行动力药剂 
		if (gameObj.name == "restoreAll") {

			if (StorageManagerment.Instance.checkProp (PropType.PROP_PVES, 1)) {
                //当前行动力大于等于600时不能使用 
				if (UserManager.Instance.self.getPvEPoint () >= totalPveMax) {
					UiManager.Instance.openDialogWindow<MessageWindow> (
						(win) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("s0361"), null);
					});
					return;
				}
				// 判断购买或使用次数是否用尽 
				if (cantUsePVES) {
					CanNotUsePves ();	
					return;
				}
				// 使用大行动力药剂 
				Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVES);
				PropManagerment.Instance.useProp (prop, 1, (num) => {
					if (num == -1) {
						UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
							win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, getNumEnoughMessage (), null);
						});
					} else {
						UiManager.Instance.openDialogWindow<MessageLineWindow> (( win1 ) => {
							win1.Initialize (Language ("s0581", prop.getName (), num));
						});
						if (UserManager.Instance.self.getPvEPoint () + restorePveAll > totalPveMax) {
							UserManager.Instance.self.addPvEPoint (totalPveMax - UserManager.Instance.self.getPvEPoint ());
						} else 
							UserManager.Instance.self.addPvEPoint (restorePveAll);
						refresh ();
						if (this.GetFatherWindow () is MissionChooseWindow) {
							MissionChooseWindow chooseWin = this.GetFatherWindow () as MissionChooseWindow;
							chooseWin.refreshData ();
						}
					}
					MaskWindow.UnlockUI ();
					finishWindow ();
				});
				return;
			} else {
				// 购买行动力 
				if (UserManager.Instance.self.getRMB () < CommonConfigSampleManager.Instance.getSampleBySid<PveBuyCostRMB> (CommonConfigSampleManager.PveBuyCostRMB_SID).getCostByCount (PveUseCount)) {
					UiManager.Instance.openDialogWindow<MessageWindow> (
                    (win) =>
					{
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("s0158"), null);
					});
					return;
				}
				if (cantUsePVES) {
					CanNotUsePves ();	
					return;
				}
				if (UserManager.Instance.self.getPvEPoint () >= totalPveMax) {
					UiManager.Instance.openDialogWindow<MessageWindow> (
						(win) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("s0361"), null);
					});
					return;
				}
				PveBuyFPort fport = FPortManager.Instance.getFPort ("PveBuyFPort") as PveBuyFPort;
				fport.access (() =>
				{
					UserManager.Instance.self.addPvEPoint (restorePveAll);
					UiManager.Instance.createMessageLintWindow (Language ("s0581", restorePveAll));
					if (UiManager.Instance.mainWindow != null) {
						UiManager.Instance.mainWindow.update_RMB ();
					}
					finishWindow ();
				});
				
			}
		} else if (gameObj.name == "restoreOne") {
			//行动力超过上限不能使用
			if (UserManager.Instance.self.getPvEPoint () >= totalPveMax) {
				UiManager.Instance.openDialogWindow <MessageWindow> (
					(win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("s0361"), null);
				});
				return;
			}
			buttonOne.disableButton (true);
			UiManager.Instance.openDialogWindow<BuyWindow> (
				(window) => {
				Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVE);
				int max = Mathf.Min (prop.getNum (), totalPveMax - UserManager.Instance.self.getPvEPoint ());
				window.init ((object)prop, max, 1, PrizeType.PRIZE_PROP, (msg) => {
					if (msg.msgEvent != msg_event.dialogCancel)
						PropManagerment.Instance.useProp (prop, msg.msgNum, ( i ) => {
							usePropOne (prop.getName (), i); });
					else
						buttonOne.disableButton (false);
				});
			});
		} else if (gameObj.name == "close") {
			finishWindow ();
		}
	}

	public void CanNotUsePves ()
	{
		string msg = "";
		int vipLevel = UserManager.Instance.self.vipLevel;
		if (vipLevel > 0) {
			msg = string.Format (LanguageConfigManager.Instance.getLanguage ("s0367"), vipLevel, maxUseCount);
		} else {
			msg = string.Format (LanguageConfigManager.Instance.getLanguage ("s0366"), maxUseCount);
		}
		UiManager.Instance.openDialogWindow<MessageWindow> (
				(win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("recharge01"), LanguageConfigManager.Instance.getLanguage ("s0093"), msg, (msgHandle) => {
				if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
						
					this.finishWindow ();
					UiManager.Instance.openWindow<VipWindow> ();
				}
			});

		});
	}

	/** 获取PVE使用次数超上限提示 */
	private string getNumEnoughMessage ()
	{
		//显示限用次数
		int maxUseCount = PropSampleManager.Instance.getPropSampleBySid (PropType.PROP_PVES).maxUseCount;
		if (maxUseCount > 0) {
			if (UserManager.Instance.self.vipLevel > 0) {
				maxUseCount += VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.vipLevel).privilege.pvePropUseCountAdd;
			}
		}
		string dialogMessage = "";
		int vipLevel = UserManager.Instance.self.vipLevel;
		if (vipLevel > 0) {
			dialogMessage = string.Format (LanguageConfigManager.Instance.getLanguage ("s0367"), vipLevel, maxUseCount);
		} else {
			dialogMessage = string.Format (LanguageConfigManager.Instance.getLanguage ("s0366"), maxUseCount);
		}
		return dialogMessage;
	}
	
	//购买物品回调
	public void buy (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogCancel) {
			UiManager.Instance.openDialogWindow<PveUseWindow> ();
			return;
		}
		goods = msg.msgInfo as Goods;
		BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
		fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, buyCallBuck);
	}
	//购买物品成功后回调
	private void buyCallBuck (int sid, int number)
	{ 
		goods.nowBuyNum += number;
		UiManager.Instance.openDialogWindow<MessageLineWindow> (( messwin ) => {
			messwin.Initialize (LanguageConfigManager.Instance.getLanguage ("s0056", goods.getName (), (number * goods.getGoodsShowNum ()).ToString ()));
			Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVES);
			PropManagerment.Instance.useProp (prop, 1, ( num ) => {
				if (num == -1) {
					UiManager.Instance.openDialogWindow<MessageWindow> (( win ) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, getNumEnoughMessage (), null);
					});
				} else {
					UiManager.Instance.openDialogWindow<MessageLineWindow> (( win1 ) => {
						win1.Initialize (Language ("s0581", prop.getName (), num));
					});

					UserManager.Instance.self.addPvEPoint (UserManager.Instance.self.getPvEPointMax ());
					if (messwin.GetFatherWindow () is MissionChooseWindow) {
						MissionChooseWindow chooseWin = messwin.GetFatherWindow () as MissionChooseWindow;
						chooseWin.refreshData ();
					}
				}
				MaskWindow.UnlockUI ();
			});
		});

		//UiManager.Instance.openDialogWindow <MessageWindow> ((messwin) => {
		//	messwin.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("s0056", goods.getName (), (number * goods.getGoodsShowNum ()).ToString ()), (msg) => {
		//		Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVES);
		//		PropManagerment.Instance.useProp (prop, 1, (num)=>{
		//			if (num == -1) {
		//				UiManager.Instance.openDialogWindow<MessageWindow> ( (win) => {
		//					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, getNumEnoughMessage(), null);
		//				});
		//			} else {
		//				UserManager.Instance.self.addPvEPoint (UserManager.Instance.self.getPvEPointMax ());
		//				if(messwin.GetFatherWindow() is MissionChooseWindow){
		//					MissionChooseWindow chooseWin=messwin.GetFatherWindow() as MissionChooseWindow;
		//					chooseWin.refreshData();
		//				}
		//			}
		//			MaskWindow.UnlockUI ();
		//		});
		//	});
		//});
	}
	
	//使用加1点PVP道具
	private void usePropOne (string propName, int i)
	{
		UiManager.Instance.openDialogWindow<MessageLineWindow> (( win1 ) => {
			win1.Initialize (Language ("s0580", propName, i, i));
		});
		UserManager.Instance.self.addPvEPoint (i);
		refresh ();
		if (this.GetFatherWindow () is MissionChooseWindow) {
			MissionChooseWindow chooseWin = this.GetFatherWindow () as MissionChooseWindow;
			chooseWin.refreshData ();
		}
		MaskWindow.UnlockUI ();
		if (UserManager.Instance.self.getPvEPoint () == UserManager.Instance.self.getPvEPointMax ())
			finishWindow ();
	}

	void refresh ()
	{
		updatePVENum ();
		initPower ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
        UiManager.Instance.pveUseWindow = null;
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
}
