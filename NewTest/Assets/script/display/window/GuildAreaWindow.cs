using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公会战领地界面(敌方，我方)
/// </summary>
public class GuildAreaWindow : WindowBase
{
	/** 领地各防守点 */
	public ButtonGuildAreaPoint[] points;
	/** 开启时间 */
	public UILabel timeLabel;
	/** 祝福，鼓舞显示 */
	public UILabel buffLabel;
	/** 已击杀 */
	public UILabel hasKilled;
	/** 自身公会领地 */
	public const int SELF = 1;
	/** 敌方公会领地 */
	public const int OTHER = 2;
	/** 公会血量 */
	public barCtrl bloodCtrl;
	/** 公会血量标签 */
	public UILabel bloddLabel;
	/** 领地类型 */
	[HideInInspector]
	public int
		type = 0;
	/** 自身领地Button组 */
	public GameObject selfButtonGroup;
	/** 别人领地Button */
	public GameObject otherButtonGroup;
	/** 挑战按钮 */
	// public ButtonBase challengeButton;
	/** 祝福按钮 */
	//  public ButtonBase wishButton;
	/** 鼓舞按钮 */
	//  public ButtonBase inspireButton;
	/** 行军值 */
	public UILabel powerLabel;
	/** 行军值 */
	public barCtrl power;
	/** 领地数据 */
	public GuildArea data;
	/** 公会uid */
	[HideInInspector]
	public string
		uid;
	/** 公会服务器id */
	[HideInInspector]
	public string
		server;
	/** 公会名 */
	[HideInInspector]
	public string
		guildName;
	/** 祈福模版 */
	public GuildBuffSample inspireSample;
	/** 鼓舞模版 */
	public GuildBuffSample wishSample;
	/** 挑战模版 */
	public GuildFightChallengeSample challengeSample;
	/** 是否是从战斗回来 */
	[HideInInspector]
	public bool
		isFightBack = false;
	/** 是否打赢 */
	[HideInInspector]
	public bool
		isWin = false;
	/** 战斗造成的伤害值 */
	private string hurtNum = "";
	/** 本次获得的战争点 */
	private string fight_score = "";
	private Timer timer;
	private float selfCurbo = 0;
	private float selfMaxBo = 0;

	protected override void begin ()
	{
		base.begin ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		if (isFightBack) {
			fightBackShow ();
			if (!isWin)
				return;
		}
		GuildGetFightInfoFport port = FPortManager.Instance.getFPort ("GuildGetFightInfoFport") as GuildGetFightInfoFport;
		port.access (updateInfo);
        
	}

	private void updateInfo ()
	{
		GuildFightInfo data = GuildManagerment.Instance.guildFightInfo;
		selfCurbo = data.curBlood;
		selfMaxBo = data.maxBlood;
		if (isAwakeformHide) {
			updateUI (uid, server, guildName, selfCurbo, selfMaxBo);
		}
		MaskWindow.UnlockUI ();
	}

	private void startTimer ()
	{
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start ();
	}

	private void updateTime ()
	{
		if (GuildFightSampleManager.Instance ().getLastFightOpenTime () == 0) {
			timer.stop ();
			timer = null;
			timeLabel.text = "";
			updateUI (uid, server, guildName, selfCurbo, selfMaxBo);
		} else {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_36", GuildFightSampleManager.Instance ().getLastFightOpenTimeString ());
		}
	}

	public void updateUI (string uid, string server, string guildName, float curBoll, float MaxBoll)
	{
		this.uid = uid;
		this.server = server;
		this.guildName = guildName;
		this.selfCurbo = curBoll;
		this.selfMaxBo = MaxBoll;
		setTitle (Language ("GuildArea_56", guildName + "." + server));
		inspireSample = GuildFightSampleManager.Instance ().getSampleBySid<GuildBuffSample> (GuildFightSampleManager.INSPIRE_SID);
		wishSample = GuildFightSampleManager.Instance ().getSampleBySid<GuildBuffSample> (GuildFightSampleManager.WISH_SID);
		challengeSample = GuildFightSampleManager.Instance ().getSampleBySid<GuildFightChallengeSample> (GuildFightSampleManager.CHALLENGE_SID);
		if (uid == GuildManagerment.Instance.getGuild ().uid) {
			type = SELF;
		} else {
			type = OTHER;
			bloodCtrl.updateValue (selfCurbo, selfMaxBo);
		}
		getAreaInfo (uid, server);

	}

	private void getAreaInfo (string uid, string server)
	{
		GuildGetAreaFPort port = FPortManager.Instance.getFPort ("GuildGetAreaFPort") as GuildGetAreaFPort;
		port.access (uid, server, getAreaInfoCallBack);
	}

	private void getAreaInfoCallBack (GuildArea data)
	{
		if (data == null) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_32"));
			this.finishWindow ();
			return;
		}
		this.data = data;
		initUI ();
	}

	private void initUI ()
	{
		initPoint ();
		initButton ();
		initBuffs ();
		initKilled ();
		initTimer ();
		initPower ();
	}

	/// <summary>
	/// 初始化计时器
	/// </summary>
	private void initTimer ()
	{
		if (!GuildFightSampleManager.Instance ().isFightTime () && !GuildFightSampleManager.Instance ().isGuildFightOver ())
			startTimer ();
	}
	/// <summary>
	/// 初始化已击杀
	/// </summary>
	private void initKilled ()
	{
		hasKilled.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_20", data.hasKilled.ToString () + "/" + data.pointList.Count);
	}

	/// <summary>
	/// 初始化界面
	/// </summary>
	private void initPoint ()
	{
		for (int i = 0; i < data.pointList.Count; i++) {
			GuildAreaPoint each = data.pointList [i];
			points [i].updateUI (each);
			points [i].onClickEvent = clickPoint;
		}
	}

	private void clickPoint (GameObject go)
	{
		int index = StringKit.toInt (go.name);
		openPvpPlayerWindow (index);
	}

	/// <summary>
	/// 根据index打开玩家预览
	/// </summary>
	private void openPvpPlayerWindow (int index)
	{
		/** 如果点击的是当前能够挑战的对象 */
		bool isCanChallenge = false;
		/** 在别人的领地且当前点击的为可挑战对象 */
		if (data.getCurrentIndex () == index && type == OTHER)
			isCanChallenge = true;    
		/** 如果是NPC */
		if (data.pointList [index - 1].isNpc) {
			if (isCanChallenge)
				challenge ();
			else {
				MaskWindow.UnlockUI ();
			}
			return;
		}
    
		GuildFightGetPlayerInfoFPort port = FPortManager.Instance.getFPort ("GuildFightGetPlayerInfoFPort") as GuildFightGetPlayerInfoFPort;
		port.access (server, uid, index, (oppInfo) =>
		{
			UiManager.Instance.openWindow<PvpPlayerWindow> ((win) =>
			{
				if (isCanChallenge) {
					PvpPlayerWindow.comeFrom = PvpPlayerWindow.FROM_GUILD_AREA_CHALLENGE;
				} else {
					PvpPlayerWindow.comeFrom = PvpPlayerWindow.FROM_GUILD_AREA;
				}
				win.teamType = PvpPlayerWindow.PVP_TEAM_TYPE;
				win.initInfo (oppInfo);
			});
		});
	}

	/// <summary>
	/// 初始化按钮
	/// </summary>
	private void initButton ()
	{
		selfButtonGroup.gameObject.SetActive (type == SELF);
		otherButtonGroup.gameObject.SetActive (type == OTHER);
		/** 不在挑战时间 */
		//if (!GuildFightSampleManager.Instance().isFightTime()) {
		//    challengeButton.disableButton(true);
		//} else {
		//    challengeButton.disableButton(false);
		//}
	}

	/// <summary>
	/// 初始化BUFF显示
	/// </summary>
	private void initBuffs ()
	{
		buffLabel.text = "";
		// if (data.inspireNum != 0)
		buffLabel.text = inspireSample.getName () + "Lv." + data.inspireNum + ":  " + inspireSample.getGuildBuffDes (data.inspireNum) + "\n";
		//  if (data.wishNum != 0)
		buffLabel.text += wishSample.getName () + "Lv." + data.wishNum + ":  " + wishSample.getGuildBuffDes (data.wishNum);

	}

	/// <summary>
	/// 更新行动力
	/// </summary>
	public void initPower ()
	{
		power.updateValue (UserManager.Instance.self.guildFightPower, UserManager.Instance.self.guildFightPowerMax);
		powerLabel.text = UserManager.Instance.self.guildFightPower + "/" + UserManager.Instance.self.guildFightPowerMax;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		/** 祝福 */
		if (gameObj.name == "ButtonWish") {
			wish ();
		}
            /** 鼓舞 */
        else if (gameObj.name == "ButtonInspire") {
			inspire ();
		}
            /** 队伍编辑 */
        else if (gameObj.name == "ButtonTeamEdit") {
			teamEdit ();
		} else if (gameObj.name == "close") {
			this.finishWindow ();
		} else if (gameObj.name == "ButtonTeam") {
			if (!GuideManager.Instance.isGuideComplete ()) {
				ArmyManager.Instance.cleanAllEditArmy ();
			}
			GuideManager.Instance.doGuide ();
			//判断队伍6（公会战队伍）是否存在
			if(ArmyManager.Instance.getArmy(6)==null)
			{
				GetGuildFightTeamFPort fport = FPortManager.Instance.getFPort<GetGuildFightTeamFPort> ();
				fport.access (openTeamEmtpyWindow);
			}
			else
				openTeamEmtpyWindow();
		}

	}
	/// <summary>
	/// 获取替补席位信息
	/// </summary>
	private void openTeamEmtpyWindow()
	{
		TeamEmtpyInfoFPort _fport = FPortManager.Instance.getFPort<TeamEmtpyInfoFPort> ();
		_fport.access (openGuildFightWindow);
	}
	/// <summary>
	/// 打开公会战队伍编辑窗口
	/// </summary>
	/// <param name="ids">Identifiers.</param>
	private void openGuildFightWindow (List<int> ids)
	{
		UiManager.Instance.openWindow<TeamEditWindow> ((win) =>
		                                               {
			win.initInfo (ids);
			
			win.setComeFrom (TeamEditWindow.FROM_GUILD);
		});
	}

	/** 前往任务获取行动力 */
	private void goToGetPower ()
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) =>
		{
			win.dialogCloseUnlockUI = false;
			win.initWindow (2, Language ("s0094"), Language ("GuildArea_50"), Language ("GuildArea_06"), (msg) =>
			{
				if (msg.msgEvent == msg_event.dialogOK) {
					UiManager.Instance.openWindow<TaskWindow> ((tWin) =>
					{
						tWin.initTap (0);
					});
				} else {
					MaskWindow.UnlockUI ();
				}
			});
		});

	}

	/// <summary>
	/// 祝福
	/// </summary>
	private void wish ()
	{
		/** 不在时间范围内 */
		if (!GuildFightSampleManager.Instance ().isActivityBuffTime ()) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_35"));
			return;
		}
		/** 行动力不足 */
		if (UserManager.Instance.self.guildFightPower < wishSample.getExpends ()) {
			goToGetPower ();
			return;
		}
		string des = wishSample.getUseBuffDes (1);
		MessageWindow.ShowConfirm (des, (msg) =>
		{
			if (msg.msgEvent == msg_event.dialogOK) {
				GuildActiveAreaBuffFport port = FPortManager.Instance.getFPort ("GuildActiveAreaBuffFport") as GuildActiveAreaBuffFport;
				port.access (1, wishSample.getExpends (), wishCallBack);
			}
		});
	}

	private void wishCallBack (int expend)
	{
		UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) =>
		{
			win.dialogCloseUnlockUI = false;
			foreach (string s in wishSample.getRewardDes()) {
				win.Initialize (s, false);
			}
		});
		UserManager.Instance.self.guildFightPower -= expend;
		data.wishNum++;
		initUI ();
		MaskWindow.UnlockUI (true);
	}

	/// <summary>
	/// 鼓舞
	/// </summary>
	private void inspire ()
	{
		/** 不在时间范围内 */
		if (!GuildFightSampleManager.Instance ().isActivityBuffTime ()) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_35"));
			return;
		}
		/** 行动力不足 */
		if (UserManager.Instance.self.guildFightPower < inspireSample.getExpends ()) {
			goToGetPower ();
			return;
		}
		string des = inspireSample.getUseBuffDes (1);
		MessageWindow.ShowConfirm (des, (msg) =>
		{
			if (msg.msgEvent == msg_event.dialogOK) {
				GuildActiveAreaBuffFport port = FPortManager.Instance.getFPort ("GuildActiveAreaBuffFport") as GuildActiveAreaBuffFport;
				port.access (2, inspireSample.getExpends (), inspireCallBack);
			}
		});
	}

	private void inspireCallBack (int expend)
	{
		UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) =>
		{
			win.dialogCloseUnlockUI = false;
			foreach (string s in inspireSample.getRewardDes()) {
				win.Initialize (s, false);
			}
		});
		UserManager.Instance.self.guildFightPower -= expend;
		data.inspireNum++;
		initUI ();
		MaskWindow.UnlockUI ();

	}
	/** 队伍编辑 */
	private void teamEdit ()
	{
		UiManager.Instance.openWindow<TeamEditWindow> (
            (win) =>
		{
			win.comeFrom = TeamEditWindow.FROM_PVP;
		});
	}
	/** 挑战 */
	public void challenge ()
	{
		/** 不在公会战战斗期间 */
		if (!GuildFightSampleManager.Instance ().isFightTime ()) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_39"));
			UiManager.Instance.BackToWindow<GuildFightMainWindow> ();
			return;
		}
		/** 行动力不足 */
		if (UserManager.Instance.self.guildFightPower < challengeSample.getExpends ()) {
			goToGetPower ();
			return;
		}
		//判断队伍6（公会战队伍）是否存在
		if(ArmyManager.Instance.getArmy(6)==null)
		{
			GetGuildFightTeamFPort fport = FPortManager.Instance.getFPort<GetGuildFightTeamFPort> ();
			fport.access (sendChallengeMsg);
		}
		else
			sendChallengeMsg();
	}

	private void sendChallengeMsg ()
	{
		MaskWindow.instance.setServerReportWait (true);
		GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
		GuildAreaChallengeFPort port = FPortManager.Instance.getFPort ("GuildAreaChallengeFPort") as GuildAreaChallengeFPort;
		port.access (server, uid, (result) =>
		{
			UserManager.Instance.self.guildFightPower -= challengeSample.getExpends ();
			EffectManager.Instance.CreateActionCast (challengeSample.getExpendsDes (), ActionCastCtrl.GUILD_FIGHT_TYPE);
			this.hurtNum = result [0];
			this.fight_score = result [1];
			initUI ();
			ArmyManager.Instance.setArmyState(ArmyManager.PVP_GUILD,1);
		});
	}

	/// <summary>
	/// 战斗会有奖励 结束完后应该展示
	/// </summary>
	private void fightBackShow ()
	{
		if (!isFightBack)
			return;
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_GUILD_WAR, showCallBack);
		isFightBack = false;
	}

	/// <summary>
	/// 战斗结束后 回来需要显示战斗中获得的奖励，战斗成功和失败 显示的提示不一样 需要区分
	/// </summary>
	/// <param name="awards">Awards.</param>
	private void showCallBack (Award[] awards)
	{
		/** 失败就挂掉了,会返回到公会战主界面 */
		if (!isWin) {
			GuildManagerment.Instance.guildFightInfo.isDead = true;
			UiManager.Instance.BackToWindow<GuildFightMainWindow> ();
		}
		UiManager.Instance.openDialogWindow<AllAwardViewWindow> ((win) =>
		{
			string topDes = LanguageConfigManager.Instance.getLanguage ("GuildArea_52", hurtNum, fight_score);
			string downDes = LanguageConfigManager.Instance.getLanguage ("GuildArea_53");
			win.Initialize (awards [0], null, topDes, downDes);
			win.showComfireButton (true, Language ("ladderButton"));
		});


	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		MaskWindow.instance.setServerReportWait (false);
		updateUI (uid, server, guildName, selfCurbo, selfMaxBo);
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
