using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公会战主界面 
/// </summary>
public class GuildFightMainWindow : WindowBase
{

	/* static fields */
	/** 公会战状态常量 */
	public const int FIGHTING = 1, //战斗中
		GROUPING = 2, //分组中
		NOTOPEN = 0, //未开启
		NOTJOIN = 3, //未参加
		CLEARING = 4; //清理数据

	/* fields */
	//**倒计时*/
	public UILabel titleTimeLabel;
	//**black bg*/
	public GameObject blackObj;
	/**  聊天按钮 */
	public ButtonChat chatButton;
	/** 动态的消息动画控制 */
	public UIPlayTween tweenMessage;
	/** 条目消息对象(15条数据的) */
	public GameObject multiMessageItem;
	/** 任意地方关闭标签 */
	public UILabel closeInfoLabel;
	//**公会名字*/
	public UILabel guildName;
	//**公会评级*/
	public UILabel guildClass;
	/**  领地UI */
	public GuildAreaItem[] areaItems;
	/** 小人 */
	public GameObject smallMan;
	public GameObject areaGroup;
	/**  规则按钮 */
	public ButtonBase buttonRule;
	/** 排行榜按钮 */
	public ButtonBase buttonRank;
	/** 复活按钮 */
	//public ButtonBase buttonRevive;
	/** 复活按钮 */
	public ButtonBase buttonTeam;
	/** 获取行动力按钮 */
	public ButtonBase buttonGetPower;
	/** 未开启时的UI元素 */
	public GameObject notOpenGroup;
	/** 时间标签 */
	public UILabel timeLabel;
	/** 公会战数据 */
	private GuildFightInfo data;
	/** 行动力 */
	public barCtrl warPowerCtrl;
	/** 行动力标签 */
	public UILabel warPowerLabel;
	/** 公会血量 */
	public barCtrl bloodCtrl;
	/** 公会血量标签 */
	public UILabel bloddLabel;
	/** 公会战消息 */
	public UITextList messageLabel;
	/** 公会战长消息列表 */
	public UITextList msgList;
	/** 公会战活跃度需求 */
	public UILabel activityRequire;
	/**  分组中UI组 */
	public GameObject groupingGroup;
	/** 分组中的描述 */
	public UILabel groupingLabel;
	/** 提示信息 */
	public GameObject tips;
	/** 提示信息 */
	public UILabel tipsLabel;
	/** 结束提示 */
	public UILabel overTipsLabel;
	/** 小提示 */
	public UILabel downTipsLabel;
	/** 公会战加入条件模版 */
	private GuildFightJoinConditionSample conditionSample;
	/** 公会战复活消耗模版 */
	private GuildFightReviveSample reviveSample;
	/** 公会战攻击小人 */
	private List<GuildSpriteCtrl> smallManList = new List<GuildSpriteCtrl> ();
	private bool tweenerMessageState ;
	/** 蓝色 */
	private Color blue = new Color (40f / 255, 90f / 255, 255f / 255);
	/** 红色 */	
	private Color red = new Color (255f / 255, 60f / 255, 80f / 255);
	/** 绿色 */	
	private Color green = new Color (110f / 255, 240f / 255, 250f / 255);
	/** 紫色 */
	private Color violet = new Color (190f / 255, 30f / 255, 250f / 255);
	/* methods */

	private Timer timer;

	public override void OnAwake ()
	{
		base.OnAwake ();
		chatButton.setShowTips (ChatManagerment.Instance.IsHaveNewHaveMsg ());
		UiManager.Instance.guildFightMainWindow = this;
	}

	protected override void begin ()
	{
		base.begin ();
		if(GuildManagerment.Instance.isReviveBack&&data!=null){
			updateReviveUI();
		}
		if(!GuildManagerment.Instance.isCanJoinGuildFight())
		{
			buttonTeam.gameObject.SetActive(false);
		}
		guildName.text = LanguageConfigManager.Instance.getLanguage ("Guild_112") + UserManager.Instance.self.guildName;
		reviveSample = GuildFightSampleManager.Instance ().getSampleBySid<GuildFightReviveSample> (GuildFightSampleManager.REVIVE_COST);
		conditionSample = GuildFightSampleManager.Instance ().getSampleBySid<GuildFightJoinConditionSample> (GuildFightSampleManager.JOIN_CONDITION);
		getFightInfo ();
		updateTime ();
		UiManager.Instance.backGround.switchBackGround ("guildFight");
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
		if (GuildFightSampleManager.Instance ().getFightTimeDelay () == 0) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			setTitle (Language ("GuildArea_13"));
			titleTimeLabel.gameObject.SetActive (false);
		} else {
			System.DateTime date = System.DateTime.Parse ("00:00:00");
			date = date.AddSeconds (GuildFightSampleManager.Instance ().getFightTimeDelay ());
			string time = date.ToString ("HH:mm:ss");
			setTitle (Language ("GuildArea_57", time));
			titleTimeLabel.text = Language ("GuildArea_84", time);
			titleTimeLabel.gameObject.SetActive (true);
		}
	}
	   
	/** 获得战斗信息 */
	public void getFightInfo ()
	{
		GuildGetFightInfoFport port = FPortManager.Instance.getFPort ("GuildGetFightInfoFport") as GuildGetFightInfoFport;
		port.access (initUI);
	}
	/** 初始化ui */
	private void initUI ()
	{
		updateUI ();
		int score = getGuildScore ();
		if (score > 0)
			guildClass.text = "[u]" + LanguageConfigManager.Instance.getLanguage ("GuildArea_95") + GuildFightSampleManager.Instance ().getJudgeString (score) + LanguageConfigManager.Instance.getLanguage ("GuildArea_96") + "[/u]";
		else
			guildClass.text = "[u]" + LanguageConfigManager.Instance.getLanguage ("GuildArea_71") + "[/u]";
	}
	/// <summary>
	/// 更新公会基础信息
	/// </summary>
	public void updateBaseInfo ()
	{
		if (GuildManagerment.Instance.guildFightInfo.messageList == null || GuildManagerment.Instance.guildFightInfo.messageList.Count == 0)
			return;
		messageLabel.Clear ();
		msgList.Clear ();
		int count = GuildManagerment.Instance.guildFightInfo.messageList.Count;
		messageLabel.Add (GuildManagerment.Instance.guildFightInfo.messageList [0]);								
		for (int i= count-1; i>-1; i--) {
			msgList.Add (GuildManagerment.Instance.guildFightInfo.messageList [i]);
		}
	}

	private void updateUI ()
	{
		data = GuildManagerment.Instance.guildFightInfo;
		updateBaseInfo ();
		updatePower ();
		switch (data.state) {
		case NOTOPEN:
			showNoOpenUI ();
			break;
		case GROUPING:
			showGroupingUI ();
			break;
		case FIGHTING:
			showFightingUI ();
			break;
		case NOTJOIN:
			showNoJoinUI ();
			break;
		case CLEARING:
			showClearingUI ();
			break;
		}
		if (GuildFightSampleManager.Instance ().isFightTime ())
			startTimer ();
	}
	/** 显示战斗中的界面 */
	private void showFightingUI ()
	{
		if (!isAwakeformHide && data.areas.Count == 1 && !GuildFightSampleManager.Instance ().isGuildFightOver ()) {
			MessageWindow.ShowAlert (Language ("GuildArea_46"));
		}
		//buttonRevive.gameObject.SetActive (data.isDead);
		//buttonRevive.disableButton (!data.isDead);
//		if (!data.isDead) {
//			buttonRevive.textLabel.effectColor = new Color (0.22f, 0.22f, 0.22f);
//		} else {
//			buttonRevive.textLabel.effectColor = new Color (0.67f, 0.05f, 0.075f);
//		}
		buttonRank.gameObject.SetActive (true);
		buttonGetPower.disableButton (!data.get_power);
		buttonRule.gameObject.SetActive (true);
		notOpenGroup.SetActive (false);
		areaGroup.SetActive (true);
		GuildAreaPreInfo [] preArray = new GuildAreaPreInfo[data.areas.Count];
		data.areas.CopyTo (preArray);
		SetKit.sort (preArray, new GuildAreaPreCompareWithWarNum ());
		List<GuildAreaPreInfo> list = new List<GuildAreaPreInfo> ();
		list.AddRange (preArray);
		for (int i=0; i<data.areas.Count; i++) {
			data.areas [i].rank = list.IndexOf (data.areas [i]) + 1;
			areaItems [i].initUI (data.areas [i], data.curBlood, data.maxBlood);
		}
		initSmallMan ();
		showTips ();
	}


	/** 显示未开启界面 */
	private void showNoOpenUI ()
	{
		buttonRank.gameObject.SetActive (true);
		buttonGetPower.disableButton (true);
		buttonRule.gameObject.SetActive (true);
		notOpenGroup.SetActive (true);
		areaGroup.SetActive (false);
		timeLabel.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_17", data.openTime.ToString ());
		activityRequire.text = LanguageConfigManager.Instance.getLanguage ("guildBuildWin02") + GuildManagerment.Instance.guildFightInfo.weekActivi + "/" + conditionSample.condition.ToString () + "\n" + conditionSample.des;

	}
	/** 显示未加入的界面  */
	private void showNoJoinUI ()
	{
		buttonRank.gameObject.SetActive (true);
		buttonGetPower.disableButton (true);
		buttonRule.gameObject.SetActive (true);
		notOpenGroup.SetActive (true);
		areaGroup.SetActive (false);
		timeLabel.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_33");
		activityRequire.transform.parent.gameObject.SetActive (false);
	}
	/** 显示分组中的界面 */
	private void showGroupingUI ()
	{
		buttonRank.gameObject.SetActive (true);
		buttonGetPower.disableButton (true);
		buttonRule.gameObject.SetActive (true);
		notOpenGroup.SetActive (false);
		areaGroup.SetActive (true);
		groupingGroup.SetActive (true);
		groupingLabel.text = Language ("GuildArea_43", GuildFightSampleManager.Instance ().getGuildFightOpenTime ());
	}

	/** 显示未加入的界面  */
	private void showClearingUI ()
	{
		buttonRank.gameObject.SetActive (true);
		buttonGetPower.disableButton (true);
		buttonRule.gameObject.SetActive (true);
		notOpenGroup.SetActive (true);
		areaGroup.SetActive (false);
		timeLabel.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_49");
		activityRequire.transform.parent.gameObject.SetActive (false);
	}

	private void showTips ()
	{
		if (!GuildFightSampleManager.Instance ().isGuildFightOpen ()) {
			tips.gameObject.SetActive (false);
			return;
		}
		if (!GuildFightSampleManager.Instance ().isShowTipsTime ()) {
			tips.gameObject.SetActive (false);
			return;
		}
		tips.gameObject.SetActive (true);
		tipsLabel.text = GuildFightSampleManager.Instance ().getTips ();
		if (GuildFightSampleManager.Instance ().isGuildFightOver ()) {
			overTipsLabel.text = Language ("GuildArea_63");
			downTipsLabel.text = "";
			tipsLabel.text = tipsLabel.text.Replace ("%1", data.getMyRank ().ToString ());
			tipsLabel.text = tipsLabel.text.Replace ("%2", getGuildScore ().ToString ());
			tipsLabel.text = tipsLabel.text.Replace ("%3", GuildFightSampleManager.Instance ().getJudgeString (getGuildScore ()));

		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonHelp") {
			UiManager.Instance.openDialogWindow<GeneralDesWindow> ((win) => {
				win.initialize (LanguageConfigManager.Instance.getLanguage ("GuildArea_15"), LanguageConfigManager.Instance.getLanguage ("GuildArea_11"), null);
			});

		} else if (gameObj.name == "ButtonRank") {
			UiManager.Instance.openWindow<GuildAreaHurtRankWindow> ();
		} else if (gameObj.name == "close") {
			this.finishWindow ();
		} else if (gameObj.name == "ButtonGetPower") {  
			/** 行动力已满 */
			if (UserManager.Instance.self.isGuildFightPowerMax ()) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_23"));
				MaskWindow.UnlockUI ();
				return;
			}
			/** 领取后行动力超出 */
			else if (UserManager.Instance.self.guildFightPower + GuildFightSampleManager.Instance ().getPowerNum () > UserManager.Instance.self.guildFightPowerMax) {
				string des = Language ("GuildArea_47");
				MessageWindow.ShowConfirm (des, (msg) => {
					if (msg.msgEvent == msg_event.dialogOK) {
						GuildGetWarPowerFport port = FPortManager.Instance.getFPort ("GuildGetWarPowerFport") as GuildGetWarPowerFport;
						port.access ((power) => {
							int addPower = power - UserManager.Instance.self.guildFightPower;
							UserManager.Instance.self.guildFightPower = power;
							data.get_power = false;
							buttonGetPower.disableButton (!data.get_power);
							updatePower ();
							UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_34", addPower.ToString ()));
						});
					}
				});
				MaskWindow.UnlockUI ();
				return;
			}
			/** 领取行动值 */
			else {
				GuildGetWarPowerFport port = FPortManager.Instance.getFPort ("GuildGetWarPowerFport") as GuildGetWarPowerFport;
				port.access ((power) => {
					int addPower = power - UserManager.Instance.self.guildFightPower;
					UserManager.Instance.self.guildFightPower = power;
					data.get_power = false;
					buttonGetPower.disableButton (!data.get_power);
					updatePower ();
					UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_34", addPower.ToString ()));
				});
			}
		} else if (gameObj.name == "ButtonAward") {
			UiManager.Instance.openDialogWindow<GuildFightAwardShowWindow> ();
		} else if (gameObj.name == "noticeButton") {
			if (GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_VICE_PRESIDENT)
				UiManager.Instance.openDialogWindow<GuildNoticeEditWindow> ((win) => {
					win.updateInput ();
				});
			else
				MaskWindow.UnlockUI ();
		} else if (gameObj.name == "chatButton") {
			UiManager.Instance.openWindow<ChatWindow> ((win) => {
				win.initChatWindow (ChatManagerment.Instance.sendType - 1);
			});
			if (MainWindow.sort > ChatManagerment.Instance.getAllChat ().Count) {
				++MainWindow.sort;
			} else {
				MainWindow.sort = ChatManagerment.Instance.getAllChat ().Count;
			}
		} else if (gameObj.name == "buttonInfo") { // 详情
			tweenerMessageState = false;			
			tweenMessage.gameObject.SetActive (true);
			tweenerMessageState = true;
			tweenerMessageGroupIn (tweenMessage);
		} else if (gameObj.name == "buttonCloseInfo") { // 关闭详情
			tweenerMessageState = false;
			tweenerMessageGroupOut (tweenMessage);
		}
        /** 复活 */
        else if (gameObj.name == "ButtonRevive") {
			//SendRivive ();
		} else if (gameObj.name == "ButtonCourage") {
			if (data.state == NOTOPEN) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("GuildArea_35"));
			} else {
				/** 入会不满1天 */
				if (!GuildManagerment.Instance.isCanJoinGuildFight ()) {
					UiManager.Instance.createMessageLintWindow(Language("GuildArea_41"));
					return;
				}
				if (data.areas.Count > 0) {
					foreach (GuildAreaPreInfo info in data.areas) {
						if (info.uid == UserManager.Instance.self.guildId) {
							GuildGetAreaFPort port = FPortManager.Instance.getFPort ("GuildGetAreaFPort") as GuildGetAreaFPort;
							port.access (info.uid, info.server, (guildAreaData) => {
								UiManager.Instance.openDialogWindow<GuildFightCourageWindow> ((win) => {
									win.initializeInfo (guildAreaData);
								});
							});
							return;
						}
					}
				} else
					MaskWindow.UnlockUI ();
			}
		} else if (gameObj.name == "ButtonIntegralRank") {
			UiManager.Instance.openDialogWindow<GuildFightClassWindow> ((win) => {
				win.Intialize ();
			});
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
	/// <summary>
	/// 复活信息
	/// </summary>
	public void updateReviveUI()
	{
		//buttonRevive.disableButton (false);
		data.curBlood = 1;
		data.maxBlood = 1;
		bloodCtrl.updateValue (data.curBlood, data.maxBlood);
		updatePower ();
	}

	/// <summary>
	/// 更新行动力
	/// </summary>
	public void updatePower ()
	{
		warPowerCtrl.updateValue (UserManager.Instance.self.guildFightPower, UserManager.Instance.self.guildFightPowerMax);
		warPowerLabel.text = UserManager.Instance.self.guildFightPower + "/" + UserManager.Instance.self.guildFightPowerMax;
		bloodCtrl.updateValue (data.curBlood, data.maxBlood);
	}

	private void clearSmallMan ()
	{
		foreach (GuildSpriteCtrl smallMan in smallManList) {
			Destroy (smallMan.gameObject);
		}
		smallManList.Clear ();
	}

	public void initSmallMan ()
	{
		clearSmallMan ();
		Vector3 enemyPoint = Vector3.zero;
		Vector3 myPoint = Vector3.zero;
		GuildAreaPreInfo area;
		for (int i =0; i< data.areas.Count; i++) {
			area = data.areas [i];
			if (area.uid == GuildManagerment.Instance.getGuild ().uid)
				continue;
			else {
				myPoint = getMyPoint (i - 1);
				enemyPoint = areaItems [i].AttackPoint [0].localPosition;
				if (area.attack != 0) {
					StartCoroutine (creatRobot (enemyPoint, myPoint, area.attack, Random.Range (0, 1f), getColor (i)));
				}

				if (area.defense != 0) {
					StartCoroutine (creatRobot (myPoint, enemyPoint, area.defense, Random.Range (0, 1f), getColor (0)));
				}
			}
		}
	}

	public Vector3 getMyPoint (int index)
	{
		Vector3 myPoint = Vector3.zero;
		GuildAreaPreInfo area;
		int myIndex = 0;
		for (int i =0; i< data.areas.Count; i++) {
			area = data.areas [i];
			if (area.uid == GuildManagerment.Instance.getGuild ().uid) {
				myIndex = i;
			}
		}
		return areaItems [myIndex].AttackPoint [index].localPosition;
	}

	/// <summary>
	/// 获取小人颜色
	/// </summary>
	private Color getColor (int index)
	{
		Color color = new Color ();
		switch (index) {
		case 0:
			color = red;
			break;
		case 1:
			color = green;
			break;
		case 2:
			color = blue;
			break;
		case 3:
			color = violet;
			break;
		}
		return color;
	}

	private IEnumerator creatRobot (Vector3 start, Vector3 target, int num, float waitTime, Color color)
	{	
		yield return new WaitForSeconds (waitTime);
		GameObject tempSmallMan = NGUITools.AddChild (areaGroup, smallMan);
		tempSmallMan.transform.localPosition = start;
		Utils.SetMaterialRenderQueueByAll (tempSmallMan, 800, color);		
		GuildSpriteCtrl smallManCtrl = tempSmallMan.GetComponent<GuildSpriteCtrl> ();
		smallManList.Add (smallManCtrl);
		/** 向左跑,向左攻击(默认为向右跑,向右攻击) */
		if (target.x < start.x && Mathf.Abs (target.x - start.x) > 100)
			smallManCtrl.trunLeft ();
		smallManCtrl.setAttackNum (num);
		smallManCtrl.init (start, target);
		smallManCtrl.moveStart ();
	}

	void Update ()
	{
		UpdateAlpha ();
	}

	/// <summary>
	/// 更新聊天按钮好友聊天显示状态
	/// </summary>
	public void UpdateChatMsgTips ()
	{
		chatButton.setShowTips (ChatManagerment.Instance.IsHaveNewHaveMsg ());
	}

	/** 闪烁任意地方关闭文字 */
	public void UpdateAlpha ()
	{
		if (tweenMessage.gameObject.activeSelf)
			closeInfoLabel.alpha = sin ();
	}

	/** 动态消息进入动画  */
	public void tweenerMessageGroupIn (UIPlayTween tween)
	{
		tween.playDirection = AnimationOrTween.Direction.Forward;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0.4f;
		}
		tween.Play (true);
	}
	/** 动态消息退出动画  */
	public void tweenerMessageGroupOut (UIPlayTween tween)
	{
		tween.playDirection = AnimationOrTween.Direction.Reverse;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0;
		}
		tween.Play (true);
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		getFightInfo ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		clearSmallMan ();
		if (timer != null) {
			timer.stop ();
			timer = null;
		}

	}
	/// <summary>
	/// 获得公会得分
	/// </summary>
	/// <returns></returns>
	private int getGuildScore ()
	{
		GuildGetInfoFPort fport = FPortManager.Instance.getFPort ("GuildGetInfoFPort") as GuildGetInfoFPort;
		fport.access (null);
		//foreach (GuildAreaPreInfo info in GuildManagerment.Instance.guildFightInfo.areas)
		//{
		//    if (info.uid == UserManager.Instance.self.guildId)
		//        score = info.judgeScore;
		//}
		return GuildManagerment.Instance.selfScore;
	}
}

public class GuildAreaPreCompareWithWarNum : Comparator
{
	public int compare (object a, object b)
	{
		GuildAreaPreInfo itemA = a as GuildAreaPreInfo;
		GuildAreaPreInfo itemB = b as GuildAreaPreInfo;
		if (itemA.warNum > itemB.warNum)
			return -1;
		else if (itemA.warNum < itemB.warNum)
			return 1;
		else {
			if (itemA.judgeScore < itemB.judgeScore)
				return -1;
			else if (itemA.judgeScore > itemB.judgeScore)
				return 1;
			else {
				long uidA = long.Parse (itemA.uid);
				long uidB = long.Parse (itemB.uid);
				if (uidA > uidB)
					return 1;
				else if (uidA < uidB)
					return  -1;
				else 
					return 0;
			}
		} 
	}
}

