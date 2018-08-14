using UnityEngine;
using System.Collections;

/**
 * PVP杯赛前窗口
 * @author 汤琦
 * */
public class PvpCupFightWindow : WindowBase
{
	private PvpOppInfo opp;
	public ButtonPvpInfo button;
	public UILabel title;
	public UILabel teamCombat;//队伍战斗力
	public PvpPowerBar power;
	private string timeLabel;
	private Timer timer;
	public ButtonBase teamButton;
	public ButtonBase commonButton;//普通战按钮
	public ButtonBase specialButton;//全力战按钮
	private CallBack callback;
	public GameObject cardPrefab;
	GameObject formationRoot;
 
	public void initWindow (CallBack callback)
	{
		this.callback = callback;
	}

	private void startTimer ()
	{
		if (timer != null)
			return;
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updatePvpTime);
		timer.start ();
	}

	protected override void begin ()
	{
		base.begin ();
		PvpInfoManagerment.Instance.setCurrentRound ();
		teamCombat.text = ArmyManager.Instance.getTeamCombat (3).ToString ();

		FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (opp.formation);

		button.initInfo (opp,this);
		loadFormationGB (button, sample.getLength (), button.gameObject);
		CreateFormation (button, opp);

		initPower ();
		startTimer ();
		updatePvpTime ();
		//引导完了才开启一次性引导
		if (GuideManager.Instance.isGuideComplete ()) {
			GuideManager.Instance.onceGuideEvent (GuideGlobal.ONCEGUIDE_PVP1);
		}
		MaskWindow.UnlockUI ();
	}
	
	public void initInfo (PvpOppInfo opp)
	{
		this.opp = opp;
		PvpInfoManagerment.Instance.setCupOpp (opp);
	}
	
	private void loadFormationGB (ButtonPvpInfo button, int formationLength, GameObject root)
	{

		if (formationRoot != null)
			Destroy (formationRoot);

		passObj go = FormationManagerment.Instance.getPlayerInfoFormationObj (formationLength);
		go.obj.transform.parent = root.transform;
		go.obj .transform.localPosition = Vector3.zero;
		go.obj .transform.localScale = Vector3.one;
		formationRoot = go.obj;

		if (go.obj != null) {
			button.formationRoot = go.obj;
			go.obj.transform.localPosition = new Vector3 (0, 235, 0);
		}
		
	}
	
	void CreateFormation (ButtonPvpInfo button, PvpOppInfo info)
	{ 
		for (int i = 0; i < info.opps.Length; i++) {

			TeamPrepareCardCtrl card = NGUITools.AddChild (button.formationRoot, cardPrefab).GetComponent<TeamPrepareCardCtrl> ();

			//找到对应的阵形点位
			Transform formationPoint = null;
			formationPoint = button.formationRoot.transform .FindChild (FormationManagerment.Instance.getLoctionByIndex (info.formation, info.opps [i].index).ToString ());
			card.transform.position = formationPoint.position;
			card.updateButton (info.opps [i]);
			card.initInfo (info.uid, info.opps [i].uid, null);
			card.fatherWindow = this;
		}
		
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
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
	}
	
	private void titleShow ()
	{
		if (PvpInfoManagerment.Instance.getPvpInfo () != null) {
			int round = PvpInfoManagerment.Instance.getPvpInfo ().round;
			if (round == 1) {
				title.text = LanguageConfigManager.Instance.getLanguage ("s0212", timeLabel);
			} else if (round == 2) {
				title.text = LanguageConfigManager.Instance.getLanguage ("s0213", timeLabel);
			} else if (round == 3) {
				title.text = LanguageConfigManager.Instance.getLanguage ("s0214", timeLabel);
			}
		} else {
			title.text = LanguageConfigManager.Instance.getLanguage ("s0215");
		}
	}

	private void teamIndexChange (int index)
	{
		teamButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0225");
		teamCombat.text = ArmyManager.Instance.getTeamCombat (3).ToString ();
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
            UiManager.Instance.openWindow<TeamViewInMissionWindow1>((wins) => {
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
				UiManager.Instance.openDialogWindow<PvpUseWindow> ((win) => {
					win.initInfo (1, null);
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
			if (UserManager.Instance.self.getPvPPoint () != 3) {
				UiManager.Instance.openDialogWindow<PvpUseWindow> ((win) => {
					win.initInfo (2, null);
				});

			} else {
				showBattleInfoWindow (2);
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
	private void showBattleInfoWindow (int  atract)
	{ 
		UiManager.Instance.switchWindow<BattlePrepareWindowNew> (
		  (win) => {
			win.Initialize (BattleType.BATTLE_SUBSTITUTE, true, false, () => {
				PvpInfoManagerment.Instance.sendFight (atract);});	
		}); 
	}
	
	void reOpenThisWindow ()
	{
		UiManager.Instance.openWindow<PvpCupFightWindow> ();
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
