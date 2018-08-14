using UnityEngine;
using System.Collections;

/**
 * PVP主窗口
 * @author 汤琦
 * */
public class PvpMainWindow : WindowBase
{
	public PvpArmyContent content;
	private int startId = 1;//初始玩家编号
	public UILabel title;//窗口标题
	public UILabel title1;
	public UILabel title2;
	public barCtrl power;
	private string timeLabel;
	public ButtonBase teamButton;
	public UILabel myCombat;
	private CallBack callback;
	public GameObject cardPrefab;
	private Timer timer;

	protected override void begin ()
	{
		base.begin ();
		initPower ();
		updatePvpTime ();
		startTimer ();

		if (!isAwakeformHide) {
			PvpInfoManagerment.Instance.setOppIndex (0);
			content.callbackUpdateEach = content.updateButton;
			content.maxCount = 3;
			content.onCenterItem = content.updateActive;
			content.onLoadFinish = () => {

				myCombat.text = ArmyManager.Instance.getTeamCombat (3).ToString ();
				//引导完了才开启一次性引导
				if (GuideManager.Instance.isGuideComplete ()) {
					GuideManager.Instance.onceGuideEvent (GuideGlobal.ONCEGUIDE_PVP1);
				}
				//MaskWindow.UnlockUI ();
				
				armyButtonChange (ArmyManager.Instance.activeID);


			};
		}

		content.startIndex = startId - 1;
		content.init ();
		StartCoroutine (Utils.DelayRun (() => {
			MaskWindow.UnlockUI();
		}, 0.3f));

	}
	
	private void startTimer ()
	{
		if (timer != null)
			return;
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updatePvpTime);
		timer.start ();
	}

	private void updatePvpTime ()
	{
		int pvpTime = PvpInfoManagerment.Instance.getPvpTime (null);
		if (pvpTime > 0) {
			int minute = pvpTime / 60;
			string minuteStr;
			int second = pvpTime % 60;
			string secondStr;
			if (minute < 10) {
				minuteStr = "0" + minute;
			} else {
				minuteStr = minute.ToString ();
			}
			if (second < 10) {
				secondStr = "0" + second;
			} else {
				secondStr = second.ToString ();
			}
			timeLabel = minuteStr + " : " + secondStr;
		} else {
			timeLabel = LanguageConfigManager.Instance.getLanguage ("s0215");
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
		} 
		titleShow ();
	}
	
	private void initPower ()
	{
		power.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
	}
	
	private void titleShow ()
	{
		if (PvpInfoManagerment.Instance.getPvpInfo () != null) {
			int round = PvpInfoManagerment.Instance.getPvpInfo ().round;
			if (round == 1) {
				title.text = LanguageConfigManager.Instance.getLanguage ("s0212", timeLabel);
				title1.text = LanguageConfigManager.Instance.getLanguage ("s0212", timeLabel);
				title2.text = LanguageConfigManager.Instance.getLanguage ("s0212", timeLabel);
			} else if (round == 2) {
				title.text = LanguageConfigManager.Instance.getLanguage ("s0213", timeLabel);
				title1.text = LanguageConfigManager.Instance.getLanguage ("s0213", timeLabel);
				title2.text = LanguageConfigManager.Instance.getLanguage ("s0213", timeLabel);
			} else if (round == 3) {
				title.text = LanguageConfigManager.Instance.getLanguage ("s0214", timeLabel);
				title1.text = LanguageConfigManager.Instance.getLanguage ("s0214", timeLabel);
				title2.text = LanguageConfigManager.Instance.getLanguage ("s0214", timeLabel);
			}
		}
		else {
			title.text = LanguageConfigManager.Instance.getLanguage ("s0215");
			title1.text = LanguageConfigManager.Instance.getLanguage ("s0215");
			title2.text = LanguageConfigManager.Instance.getLanguage ("s0215");
		}
	}
	
	private void armyButtonChange (int index)
	{
		myCombat.text = ArmyManager.Instance.getTeamCombat (3).ToString ();
		teamButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0225"); 
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		//切换队伍
		if (gameObj.name == "changeArmy") {
			if (PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0215"));
				return;
			}
			UiManager.Instance.openWindow<TeamViewInMissionWindow1> ((wins)=>{
                wins.comeFrom = TeamViewInMissionWindow1.FROM_PVP;
			});
		}
		//进入普通战斗
		else if (gameObj.name == "commonFight") {
			if (PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0215"));
				return;
			}
			if (UserManager.Instance.self.getPvPPoint () <= 0) {
				UiManager.Instance.openDialogWindow<PvpUseWindow > (
					(win) => {
					win.initInfo (1, PvpInfoManagerment.Instance.getOpp ().uid, null);
				});
			} else {
				showBattleInfoWindow (1);
			}
		}
		//进入全力战斗
		else if (gameObj.name == "specialFight") {
			if (PvpInfoManagerment.Instance.getPvpTime (null) <= 0) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0215"));
				return;
			}
			if (UserManager.Instance.self.getPvPPoint () < 3) {
				UiManager.Instance.openDialogWindow<PvpUseWindow> (
					(win) => {
					win.initInfo (2, PvpInfoManagerment.Instance.getOpp ().uid, null);
				});
				//destoryWindow();
			} else {
				showBattleInfoWindow (2);
				//hideWindow();
			}
			
		} else if (gameObj.name == "close") {
			if (BattleManager.Instance != null)
				BattleManager.Instance.awardFinfish ();
			else
				finishWindow ();
			if (MissionManager.instance != null)
			{
				MissionManager.instance.showAll ();
				MissionManager.instance.setBackGround();
			}
		}
		
	}
	
	//显示战前队伍信息窗口
	private void showBattleInfoWindow (int atract)
	{ 
		UiManager.Instance.switchWindow<BattlePrepareWindowNew> (
			(win) => {
			win.Initialize (BattleType.BATTLE_SUBSTITUTE, true, false, () => {
				PvpInfoManagerment.Instance.sendFight (atract);});	
		});
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();  //2014.7.2 16:02 modified
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
}
