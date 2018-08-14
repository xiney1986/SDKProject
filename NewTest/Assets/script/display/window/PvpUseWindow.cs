using UnityEngine;
using System.Collections;

/**
 * 体力不足窗
 * @author 汤琦
 * */
public class PvpUseWindow : WindowBase
{

	public PvpPowerBar bar;
	public UILabel timeValue;
	public UITexture restoreAll;
	public UITexture restoreOne;
	private int atract;//1普通战斗，2全力战斗
	private string opp;
	private Timer timer;//刷新PVP倒计时
	private CallBack callback;
	private Goods goods;
	private const int PVP = 76006;//体力剂
	private const int PVPS = 76007;//体力药水
	public UILabel restoreAllNum;
	public UILabel restoreOneNum;
	public ButtonBase buttonOne;
	public ButtonBase buttonAll;
	public UILabel buttonAll_lable;
	public UILabel buttonOne_lable;
	private string timeLabel;
	 
	protected override void begin ()
	{
		base.begin ();
		initPower ();
		initTexture ();
		updatePvpTime ();
		startTimer ();
		timeValue.gameObject.SetActive (true);
		MaskWindow.UnlockUI ();
	}

	private void startTimer ()
	{
		if (timer != null)
			return;
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updatePvpTime);
		timer.start ();
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		buttonAll.disableButton (false);
		buttonOne.disableButton (false);
	}

	public void updateWindow ()
	{
		initPower ();
		initTexture ();
	}
	
	private void updatePVPNum ()
	{
		Prop p = StorageManagerment.Instance.getProp (PropType.PROP_PVPS); 
		if (p != null && p.getNum () > 0) {
			restoreAllNum.text = p.getNum ().ToString () + "/1";
			buttonAll_lable.text = LanguageConfigManager.Instance.getLanguage ("s0232");
		} else {
			restoreAllNum.text = "[FF0000]0/1";
			buttonAll_lable.text = LanguageConfigManager.Instance.getLanguage ("s0233");

		}
		int needNum = atract == 1 ? 1 - UserManager.Instance.self.getPvPPoint () : UserManager.Instance.self.getPvPPointMax () - UserManager.Instance.self.getPvPPoint ();
		p = StorageManagerment.Instance.getProp (PropType.PROP_PVP);
		if (p != null && p.getNum () >= needNum) {
			restoreOneNum.text = p.getNum ().ToString () + "/" + needNum;
			buttonOne_lable.text = LanguageConfigManager.Instance.getLanguage ("s0232");
		} else {
			restoreOneNum.text = "[FF0000]" + (p != null ? p.getNum () : 0) + "/" + needNum;
			buttonOne_lable.text = LanguageConfigManager.Instance.getLanguage ("s0233");
		}
	}
	 
	private void initTexture ()
	{
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + GoodsSampleManager.Instance.getGoodsSampleBySid (PVP).iconId, restoreOne);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + GoodsSampleManager.Instance.getGoodsSampleBySid (PVPS).iconId, restoreAll);
		restoreOne.gameObject.SetActive (true);
		restoreAll.gameObject.SetActive (true);
		updatePVPNum ();
	}

	private void updatePvpTime ()
	{
		initPower ();
		timeValue.text = UserManager.Instance.getNextPvpTime ();
//		int pvpTime = PvpInfoManagerment.Instance.getPvpTime (null);
//		if (PvpInfoManagerment.Instance.getPvpInfo () != null) {
//			if (pvpTime > 0) {
//				int minute = pvpTime / 60;
//				string minuteStr;
//				int second = pvpTime % 60;
//				string secondStr;
//				if (minute < 10) {
//					minuteStr = "0" + minute;
//				} else {
//					minuteStr = minute.ToString ();
//				}
//				if (second < 10) {
//					secondStr = "0" + second;
//				} else {
//					secondStr = second.ToString ();
//				}
//				timeLabel = minuteStr + " : " + secondStr;
//			} else {
//				timeLabel = LanguageConfigManager.Instance.getLanguage ("s0215");
//				timer.stop ();
//				timer = null;
//			}	
//			timeValue.text = timeLabel;
//		} 
	}
	
	private void initPower ()
	{
		bar.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
	}
	//PVP三选一
	public void initInfo (int atract, string opp, CallBack callback)
	{
		this.atract = atract;
		this.opp = opp;
		this.callback = callback;
	}
	//PVP杯赛
	public void initInfo (int atract, CallBack callback)
	{
		this.atract = atract;
		this.callback = callback;
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "restoreAll") {
			if (StorageManagerment.Instance.checkProp (PropType.PROP_PVPS, 1)) {
				Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVPS);
				buttonAll.disableButton (true);
				buttonOne.disableButton (true);
				PropManagerment.Instance.useProp (prop, 1, (num) => {
					UiManager.Instance.openDialogWindow<MessageLineWindow> (( win1 ) => {
						win1.dialogCloseUnlockUI=false;
						win1.Initialize (Language ("s0583", prop.getName (), num));
					});
					UserManager.Instance.self.addPvPPoint (UserManager.Instance.self.getPvPPointMax ());
					updatePVPNum ();
					initPower ();
					StartCoroutine (createMessageLine (num, PVPS));
					updatePower ();
				});
				return;
			} else {
				finishWindow ();
				Goods goods = new Goods (PVPS);
				UiManager.Instance.openDialogWindow<BuyWindow> ((win) => {
					win.init (goods, PrizeType.PRIZE_RMB, buy);
				});
			}
		} else if (gameObj.name == "restoreOne") {
			//得到我还差几点PVP就满
			int num = UserManager.Instance.self.getPvPPointMax () - UserManager.Instance.self.getPvPPoint ();
			if (atract == 2) {
				if (StorageManagerment.Instance.checkProp (PropType.PROP_PVP, num)) {
					Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVP);
					buttonAll.disableButton (true);
					buttonOne.disableButton (true);
					PropManagerment.Instance.useProp (prop, num, ( n ) => { usePropOne (prop.getName (), n); });
				} else {
					finishWindow ();
					Goods goods = new Goods (PVP);
					UiManager.Instance.openDialogWindow<BuyWindow> ((win) => {
						win.init (goods, PrizeType.PRIZE_RMB, buy);
					});
				}
			} else {
				if (StorageManagerment.Instance.checkProp (PropType.PROP_PVP, 1)) {
					Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVP);
					buttonAll.disableButton (true);
					buttonOne.disableButton (true);
					PropManagerment.Instance.useProp (prop, 1, ( n ) => { usePropOne (prop.getName (), n); });
				} else {
					finishWindow ();
					Goods goods = new Goods (PVP);
					UiManager.Instance.openDialogWindow<BuyWindow> ((win) => {
						win.init (goods, PrizeType.PRIZE_RMB, buy);
					});
				}
			}
		} else if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	
	private void windowBack (MessageHandle msg)
	{
		UiManager.Instance.openDialogWindow<PvpUseWindow> ((win) => {
			win.initInfo (atract, null);
		});
//		updateWindow ();
	}
	//购买物品回调
	public void buy (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogCancel) {
			UiManager.Instance.openDialogWindow<PvpUseWindow> ((win) => {
				win.initInfo (atract, null);
			});
			return;
		}
		goods = msg.msgInfo as Goods;
		BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
		fport.buyGoods ((msg.msgInfo as Goods).sid, msg.msgNum, buyCallBuck);
	}
	//购买物品成功后回调
	private void buyCallBuck (int sid, int num)
	{ 
		goods.nowBuyNum += num;
		UiManager.Instance.openDialogWindow<MessageLineWindow> (( win ) => {

			win.Initialize (Language ("s0056", goods.getName (), (num * goods.getGoodsShowNum ()).ToString ()));
			if (sid == PVP) {
				int lackNum = UserManager.Instance.self.getPvPPointMax () - UserManager.Instance.self.getPvPPoint ();
				int useNum = num;
				if (useNum > lackNum)
					useNum = lackNum;
				Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVP);
				PropManagerment.Instance.useProp (prop, useNum, (number) => {
					UiManager.Instance.openDialogWindow<MessageLineWindow> (( win1 ) => {
						win1.Initialize (Language ("s0582", prop.getName (), number, number));
					});
					UserManager.Instance.self.addPvPPoint (number);
					updatePower ();
				});
			} else if (sid == PVPS) {
				Prop prop = StorageManagerment.Instance.getProp (PropType.PROP_PVPS);
				PropManagerment.Instance.useProp (prop, 1, (number) => {
					UiManager.Instance.openDialogWindow<MessageLineWindow> (( win1 ) => {
						win1.Initialize (Language ("s0583", prop.getName (), 1));
					});
					UserManager.Instance.self.addPvPPoint (UserManager.Instance.self.getPvPPointMax ());
					updatePower ();
				});
			}
			MaskWindow.UnlockUI ();
		});
	}	
	//使用加1点PVP道具
	private void usePropOne (string propName, int num)
	{
		UiManager.Instance.openDialogWindow<MessageLineWindow> (( win1 ) => {
			win1.dialogCloseUnlockUI=false;
			win1.Initialize (Language ("s0582", propName, num, num));
		});
		UserManager.Instance.self.addPvPPoint (num);
		updatePVPNum ();
		initPower ();
		StartCoroutine (createMessageLine (num, PVP));
		updatePower ();
	}

	private void updatePower ()
	{
		if (fatherWindow is PvpCupFightWindow)
			(fatherWindow as PvpCupFightWindow).power.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
		else if (fatherWindow is PvpMainWindow)
			(fatherWindow as PvpMainWindow).power.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
	}

	IEnumerator createMessageLine (int num, int id)
	{
		string str = "-" + num.ToString ();
		UiManager.Instance.createMessageTextureWindow (ResourcesManager.ICONIMAGEPATH + GoodsSampleManager.Instance.getGoodsSampleBySid (id).iconId, str,false);
		yield return new WaitForSeconds (0.1f);
		if (atract == 2 && UserManager.Instance.self.getPvPPoint () < 3) {
			buttonAll.disableButton (false);
			buttonOne.disableButton (false);
		} else {
			finishWindow ();
//			UiManager.Instance.switchWindow<BattlePrepareWindow> (
//				(win) => {
//				win.Initialize (BattleType.BATTLE_FIVE, true, false, () => {
//					PvpInfoManagerment.Instance.sendFight (atract);
//
//				});
//			});
            if (!(fatherWindow is MiningWindow)) {
                EventDelegate.Add(OnHide, () => {
                    UiManager.Instance.switchWindow<BattlePrepareWindowNew>(
                        (win) => {
                            win.Initialize(BattleType.BATTLE_FIVE, true, false, () => {
                                PvpInfoManagerment.Instance.sendFight(atract);

                            });
                        });
                });
            }
			
		}
	}

	//战斗后消耗PVP
	private void fightUsePvp ()
	{
		if (atract == 1) {
			UserManager.Instance.self.expendPvPPoint (1);
		} else if (atract == 2) {
			UserManager.Instance.self.expendPvPPoint (3);
		}
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		initTexture ();
		initPower ();
		updatePower ();
	}
}
