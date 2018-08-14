using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公会主界面
/// </summary>
public class GuildMainWindow : WindowBase {
	/** 幸运女神开放的大厅等级 */
	public const int LuckyNvShenOpenLevel = 3;
	/* gameobj fields */
	/** 建筑预制体 */
	public GameObject guildViewPre;
	/** 拖拽相关 */
	public UIScrollView launcher;
	public UIPanel launcherPanel;
	/** 公会属性 */
	public GuildMainGuildContent guildContent;
	/** 箭头按钮 */
	public ButtonBase arrowButton;
	/** 公告条目动画控制 */
	public UIPlayTween tweenerArrow;
	/** 动态的消息动画控制 */
	public UIPlayTween tweenMessage;
	public UIPlayTween tweenHelp;
	/** 条目消息对象(2条数据的) */
	public GameObject messageItem;
	/** 条目消息对象(15条数据的) */
	public GameObject multiMessageItem;
	/** 建筑按钮组 */
	public GameObject guildItem;
	/** 建筑对象 */
	public GameObject[] guildsObjs;
	/** 公会幸运星 */
	public GuildBuildView luckyNvshenBuild;
	/** 任意地方关闭标签 */
	public UILabel closeInfoLabel;
	/** 待审批提示 */
	public GameObject applyNum;
	/** 动画Delay */
	public float delayTime = 3f;
	/** 重命名按钮 */
	public GameObject RenameButton;
	/* fields */
	/** 建筑按钮编号 */
	private int[] guildsIds = new int[5]{1,2,3,4,5};
	/** 消息(15条数据的)条目状态(false=收拢,true=拉伸) */
	bool tweenerMessageState;

	/* methods */
	public override void OnAwake () {
		launcherPanel.clipOffset = new Vector2 (0, 0);
		launcherPanel.transform.localPosition = new Vector3 (0, 0, 0);
	}
	protected override void begin () {
		base.begin ();
		UpdateUI ();
		MaskWindow.UnlockUI (true);
	}
	/** 激活 */
	protected override void DoEnable () {
		base.DoEnable ();
//		UiManager.Instance.backGround.setOffsetValue (-0.3023983f, 0.3023983f);
		UiManager.Instance.backGround.switchSynToDynamicBackground (launcherPanel, "gangBG", BackGroundCtrl.gangSize);
	}
	/** 隐藏 */
	public override void DoDisable () { 
		base.DoDisable ();
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		initWindow ();
	}
	public void initWindow () {
		GuildManagerment.Instance.clearUpdateMsg ();
		GuildBuildLevelGetFPort fport = FPortManager.Instance.getFPort ("GuildBuildLevelGetFPort") as GuildBuildLevelGetFPort;
		fport.access (UpdateUI);
	}
	/** 更新UI */
	public void UpdateUI () {
		Guild guild = GuildManagerment.Instance.getGuild ();
		if (guild == null)
			return;
		UpdateGuildContent ();
		UpdateGuildsButton ();
		updateTweenerArrowGroup ();
		showApplyNum ();
		showRenameButton ();
	}
	/** 更新公会属性 */
	public void UpdateGuildContent() {
		guildContent.initInfo ();
	}
	/** 更新建筑按钮 */
	public void UpdateGuildsButton() {
		GuildBuildView guildBuildView;
		for (int i = 0; i < guildsObjs.Length; i++) {
			if (guildsObjs[i].transform.childCount == 0) {
				guildBuildView = NGUITools.AddChild (guildsObjs[i], guildViewPre).GetComponent<GuildBuildView> (); 
			} else {
				guildBuildView = guildsObjs[i].transform.GetChild(0).GetComponent<GuildBuildView>();
			}
			guildBuildView.fatherWindow = this;
			guildBuildView.name = "button_" + guildsIds[i];
			guildBuildView.updateInfo (guildsIds[i]);
		}
		if (GuildManagerment.Instance.getBuildLevel ("1") >= GuildManagerment.LUCK_GODDESS) {
			luckyNvshenBuild.spriteLock.gameObject.SetActive (false);
		} else {
			luckyNvshenBuild.spriteLock.gameObject.SetActive (true);
		}
	}
	/** 隐藏建筑按钮 */
	public void HideBuildButton() {
		guildItem.SetActive (false);
	}
	/** 激活建筑按钮 */
	public void ActiveBuildButton() {
		guildItem.SetActive (true);
	}
	/***/
	void Update () {
		CorrectBackground ();
		UpdateAlpha ();
		CorrectBuildPos ();
	}
	
	/** 闪烁任意地方关闭文字 */
	public void UpdateAlpha() {
		if(tweenMessage.gameObject.activeSelf)
			closeInfoLabel.alpha =sin();
	}

	/** 纠正建筑坐标 */
	private void CorrectBuildPos () {
		Vector3 pos;
		for (int i = 0; i < guildsObjs.Length; i++) {
			pos = UiManager.Instance.backGround.getBuildPosById (i + 1);
			guildsObjs[i].transform.position = pos;
			guildsObjs[i].transform.position = new Vector3 (guildsObjs[i].transform.position.x, guildsObjs[i].transform.position.y, 0);
		}
		Vector3 pos2 = UiManager.Instance.backGround.getBuildPosById (0);
		luckyNvshenBuild.transform.position = pos2;
		luckyNvshenBuild.transform.position = new Vector3 (luckyNvshenBuild.transform.position.x, luckyNvshenBuild.transform.position.y, 0);

//		luckyNvshenBuild.transform.parent.position = UiManager.Instance.backGround.getPosBySid (5);
	}
	
	/** 纠正动态背景 */
	public void CorrectBackground () {
		//这里是绝对算边界
//		float maxX = 8000f * UiManager.Instance.screenScaleX;
//		float minX = 1650f * UiManager.Instance.screenScaleX;
//		if (launcherPanel.clipOffset.x >= maxX) {
//			launcherPanel.clipOffset = new Vector2 (maxX,0);
//			launcherPanel.transform.localPosition = new Vector2 (-maxX,0);
//		} else if (launcherPanel.clipOffset.x <= minX) {
//			launcherPanel.clipOffset = new Vector2 (minX,0);
//			launcherPanel.transform.localPosition = new Vector2 (-minX,0);
//		}
//		float a = launcherPanel.clipOffset.x;
//		UiManager.Instance.backGround.UpdateGuildBG (a);
		float orgX = launcherPanel.clipOffset.x;
		float x = orgX;
		bool isCorrect = false;

//		float ss = (float)Screen.width / 640;
		float scale = (float)Screen.width / 640;
//		if (ss != ((float)640/960) && Screen.width != 640 && Screen.height != 960) {
//			scale = ss < ((float)640/960) ? (float)Screen.width / Screen.height + 1 : (float)Screen.width / Screen.height;
//		}

		if (scale < 1) {
			scale = (1 - scale) + 1;
		}
		else if (scale > 1) {
			scale = (1 - (scale - 1)) / 1;
		}

		float minScale = -157 * scale;
		float maxScale = 157 * scale;
		if (x > maxScale) {
			x = maxScale;
			isCorrect=true;
		}			
		if (x < minScale) {
			x = minScale;
			isCorrect=true;
		}

		if (orgX != x) {
			launcherPanel.clipOffset = new Vector2 (x, launcherPanel.clipOffset.y);
			if (gameObject.activeInHierarchy) {
				Vector3 tempVector3 = new Vector3 ();
				tempVector3.x = -x;
				tempVector3.y = launcherPanel.transform.localPosition.y;
				tempVector3.z = launcherPanel.transform.localPosition.z;
				launcherPanel.transform.localPosition = tempVector3;
				if(isCorrect) {
					float positionX=launcherPanel.transform.position.x;
					positionX=Mathf.Abs(positionX);
					UiManager.Instance.backGround.setOffsetValue (-positionX, positionX);
				}
			}
		}
	}
	/** button点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			UiManager.Instance.switchWindow<MainWindow> ();
			EventDelegate.Add (onDestroy, () => {
				if (UiManager.Instance.backGround != null) {
					UiManager.Instance.backGround.resetLastSynPosition ();
				}
			});
		} else if (gameObj.name == "buttonMember") { // 成员管理
			UiManager.Instance.openWindow<GuildMemberWindow> ((win) => {
				win.updateMember ();
			});
		} else if (gameObj.name == "noticeButton") {
			if (GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuild ().job == GuildJobType.JOB_VICE_PRESIDENT)
				UiManager.Instance.openDialogWindow<GuildNoticeEditWindow> ((win) => {
					win.updateInput ();
				});
			else
				MaskWindow.UnlockUI ();
		} else if (gameObj.name == "buttonGuildRank") { // 排行榜
			GuildGetMembersFPort fport = FPortManager.Instance.getFPort ("GuildGetMembersFPort") as GuildGetMembersFPort;
			fport.access (getRastRank);
		} else if (gameObj.name == "buttonGuildDonate") { // 捐款
			UiManager.Instance.openWindow<GuildDonateWindow> ();
		} else if (gameObj.name == "arrowButton") {
			bool b = PlayerPrefs.GetInt (UserManager.Instance.self.uid + PlayerPrefsComm.GUILD_INFO_HIDE) == 0 ? true : false;
			delayTime = 0.4f;
			if (b) {
				tweenerArrowGroupOut ();
				PlayerPrefs.SetInt (UserManager.Instance.self.uid + PlayerPrefsComm.GUILD_INFO_HIDE, 1);
			} else {
				tweenerArrowGroupIn ();
				PlayerPrefs.SetInt (UserManager.Instance.self.uid + PlayerPrefsComm.GUILD_INFO_HIDE, 0);
			}
			PlayerPrefs.Save ();
			MaskWindow.UnlockUI ();
		} else if (gameObj.name == "button_1") {//大厅
			if (GuildManagerment.Instance.getBuildLevel (GuildManagerment.HALL) <= 0)
				return;
			UiManager.Instance.openWindow<GuildBuildWindow> ((win) => {
				win.init (GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (gameObj.GetComponent<GuildBuildView> ().buildSid));
			});
		} else if (gameObj.name == "button_2") {//学院
			//解锁逻辑
			if (GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE) <= 0) {
				//判断是否有权限解锁
				if (GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_VICE_PRESIDENT) {

					UiManager.Instance.openWindow<GuildBuildWindow> ((win) => {   //进入解锁界面
						win.init (GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (gameObj.GetComponent<GuildBuildView> ().buildSid));
					});
				} else {
					UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_1115"));   //没有权限就飘字提示
				}
			} else {
				UiManager.Instance.openWindow<GuildCollegeWindow> ();  //可以使用，进入
			}
		} else if (gameObj.name == "button_3") {//商店
			if (GuildManagerment.Instance.getBuildLevel (GuildManagerment.SHOP) <= 0) {
				//判断是否有权限解锁
				if (GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_VICE_PRESIDENT) {
					UiManager.Instance.openWindow<GuildBuildWindow> ((win) => {   //进入解锁界面
						win.init (GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (gameObj.GetComponent<GuildBuildView> ().buildSid));
					});
				} else {
					UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_1115"));   //没有权限就飘字提示
				}
			} else {
				UiManager.Instance.openWindow<GuildShopWindow> ();  //可以使用，进入
			}

		} else if (gameObj.name == "button_4") {//祭坛
			if (GuildManagerment.Instance.getBuildLevel (GuildManagerment.ALTAR) <= 0) {
				//判断是否有权限解锁
				if (GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_VICE_PRESIDENT) {
					UiManager.Instance.openWindow<GuildBuildWindow> ((win) => {   //进入解锁界面
						win.init (GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (gameObj.GetComponent<GuildBuildView> ().buildSid));
					});
				} else {
					UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_1115"));   //没有权限就飘字提示
				}
			} else {
				UiManager.Instance.openWindow<GuildAltarWindow> ((win) => {
					win.initWindow ();
				}); //可以使用，进入
			}

//			if(GuildManagerment.Instance.getBuildLevel (GuildManagerment.HALL) < GuildManagerment.LUCK_GODDESS ||
//			   GuildManagerment.Instance.getBuildLevel (GuildManagerment.ALTAR) <= 0)
//				return;
//			UiManager.Instance.openWindow<GuildAltarWindow> ((win)=>{
//				win.initWindow ();
//			});
		} else if (gameObj.name == "buttonInfo") { // 详情
			tweenerMessageState = false;
			tweenerMessageGroupOut (tweenHelp);

			tweenMessage.gameObject.SetActive (true);
			tweenerArrow.gameObject.SetActive (false);
			tweenerMessageState = true;
			tweenerMessageGroupIn (tweenMessage);
		} else if (gameObj.name == "buttonCloseInfo") { // 关闭详情
			tweenerMessageState = false;
			tweenerMessageGroupOut (tweenMessage);
		} else if (gameObj.name == "buttonShake") {
			UiManager.Instance.openWindow<GuildLuckyNvShenWindow> ();
		} else if (gameObj.name == "buttonRename") {
			UiManager.Instance.openDialogWindow<GuildRenameWindow> ();
		} else if (gameObj.name == "button_5") {//领地
			if (GuildManagerment.Instance.getBuildLevel (GuildManagerment.AREA) <= 0) {
				//判断是否有权限解锁
				if (GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_VICE_PRESIDENT) {
					UiManager.Instance.openWindow<GuildBuildWindow> ((win) => {   //进入解锁界面
						win.init (GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (gameObj.GetComponent<GuildBuildView> ().buildSid));
					});
				} else {
					UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_1115"));   //没有权限就飘字提示
				}
			} 
			else {
				UiManager.Instance.openWindow<GuildFightMainWindow>();  //可以使用，进入
			}

		}else if(gameObj.name=="buttonHelp"){
			tweenerMessageState = false;
			tweenerMessageGroupOut (tweenMessage);
			tweenHelp.gameObject.SetActive(true);
			tweenerArrow.gameObject.SetActive (false);
			tweenerMessageState = true;
			tweenerMessageGroupIn (tweenHelp);
		}else if (gameObj.name == "buttonCloseHelp") { // 关闭详情
			tweenerMessageState = false;
			tweenerMessageGroupOut (tweenHelp);
		}
	}
	/** 动态消息进入动画  */
	public void tweenerMessageGroupIn (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Forward;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0.4f;
		}
		tween.Play (true);
	}
	/** 动态消息退出动画  */
	public void tweenerMessageGroupOut (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Reverse;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0;
		}
		tween.Play (true);
	}
	/** 动态消息完成做的事情  */
	public void tweenerMessageGroupFinsh () {
		if (tweenerMessageState) { // 展开
			tweenerArrow.gameObject.SetActive(false);
			tweenMessage.gameObject.SetActive(true);
		}
		else { // 收拢
			tweenerArrow.gameObject.SetActive(true);
			tweenMessage.gameObject.SetActive(false);
		}
		MaskWindow.UnlockUI();
	}
	/** 公告条目进入动画  */
	public void tweenerArrowGroupIn () {
		tweenerArrow.playDirection = AnimationOrTween.Direction.Forward;
		UITweener[] tws = tweenerArrow.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = delayTime;
		}
		//第一次进入为3秒停留，之后为0.4f
		delayTime = 0.4f;
		tweenerArrow.Play (true);
	}
	/** 公告条目退出动画  */
	public void tweenerArrowGroupOut () {
		tweenerArrow.playDirection = AnimationOrTween.Direction.Reverse;
		UITweener[] tws = tweenerArrow.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0;
		}
		tweenerArrow.Play (true);
	}
	/** 公告条目完成做的事情  */
	public void tweenerArrowGroupFinsh () {
		updateTweenerArrowGroup();
		MaskWindow.UnlockUI ();
	}
	/** 更新箭头方向 */
	private void updateTweenerArrowGroup() {
		bool b=PlayerPrefs.GetInt(UserManager.Instance.self.uid + PlayerPrefsComm.GUILD_INFO_HIDE)==0?true:false;
		if (b) {
			tweenerArrowGroupIn();
			arrowButton.transform.localRotation = Quaternion.identity;
		}
		else {
			tweenerArrowGroupOut();
			arrowButton.transform.localRotation = Quaternion.AngleAxis (180, Vector3.forward);
		}
	}
	private void getRastRank () {
		GuildGetRastRankFPort fport = FPortManager.Instance.getFPort ("GuildGetRastRankFPort") as GuildGetRastRankFPort;
		fport.access (openGuildInRankWindow);
	}
	private void openGuildInRankWindow () {
		UiManager.Instance.openWindow<GuildInsideRankWindow> ((win) => {});
	}


	public void showApplyNum ()
	{
		Guild guild = GuildManagerment.Instance.getGuild ();
		List<GuildApprovalInfo> applyList = GuildManagerment.Instance.getGuildApprovalList ();
		/** 只有会长和副会长才能看到待审批提示 */
		if (guild != null && (guild.job == GuildJobType.JOB_PRESIDENT || guild.job == GuildJobType.JOB_VICE_PRESIDENT)) {
			if(applyList == null || applyList.Count == 0)
			{
				applyNum.SetActive(false);
			}
			else
			{
				applyNum.SetActive(true);
			}
		} else {
			applyNum.SetActive(false);
		}
	}

	/// <summary>
	/// 显示公会重命名按钮
	/// </summary>
	public void showRenameButton()
	{
		Guild guild = GuildManagerment.Instance.getGuild ();
		if (guild != null && guild.job == GuildJobType.JOB_PRESIDENT && guild.isCanRename) {
			RenameButton.SetActive (true);
		} else {
			RenameButton.SetActive(false);
		}
	}
}