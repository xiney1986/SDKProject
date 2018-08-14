
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公会祭坛窗口
/// </summary>
public class GuildAltarWindow : WindowBase {
	/** BOSSICON */
	public UITexture bossTexture;
	/** BOSS血条 */
	public UISlider bossBloodSlider;
	/** BOSS血量 */
	public UILabel bossBloodNum;
	/** BOSS名称 */
	public UISprite bossNameSprite;
	/** 弱点描述 */
	public UILabel weakDes;
	/** 祝福 */
	public UILabel blessDes;
	/** 出战队伍 */
	public UILabel teamDes;
	/** 我的输出伤害 */
	public UILabel myHurtNum;
	/** 排行榜节点 */
	public GameObject rankRoot;
	/** 排行榜 */
	public GuildInRankItem[] items;
	/** 挑战按钮 */
	public ButtonBase buttonChallenge;

	protected override void begin () {
		base.begin ();
		if (GuildManagerment.Instance.isGuildBattle == true) {
			initWindow();
			GuildManagerment.Instance.isGuildBattle = false;
		}
		MaskWindow.UnlockUI ();
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		initWindow ();
	}
	/** 激活 */
	protected override void DoEnable () {
		base.DoEnable ();
		if (fatherWindow is GuildMainWindow) {
			GuildMainWindow win=fatherWindow as GuildMainWindow;
			UiManager.Instance.backGround.switchSynToDynamicBackground (win.launcherPanel, "gangBG", BackGroundCtrl.gangSize);
		}
	}
	public void initWindow () {
		GuildGetAltarFPort fport = FPortManager.Instance.getFPort ("GuildGetAltarFPort") as GuildGetAltarFPort;
		fport.access (updateUI);
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "ButtonReward") {
			//UiManager.Instance.openDialogWindow<GuildAltarRewardWindow> ();
			UiManager.Instance.openDialogWindow<GuildBossRaskAwardWindow> ();
		}
		else if (gameObj.name == "ButtonChallenge") {
			challengeGuildBoss ();
		}
		else if (gameObj.name == "ButtonEdit") {
			openTeamEditWindow ();
		}
		else if (gameObj.name == "close") {
			this.finishWindow ();
		}
	}
	/// <summary>
	/// 挑战祭坛BOSS
	/// </summary>
	private void challengeGuildBoss () {
		GuildChallegeForFPort fport = FPortManager.Instance.getFPort ("GuildChallegeForFPort") as GuildChallegeForFPort;
		fport.access (battlePrepare);
	}

	/// <summary>
	/// 打开队伍编辑窗口
	/// </summary>
	private void openTeamEditWindow () {
		UiManager.Instance.openWindow <TeamEditWindow> (
			(win) => {
			win.setComeFrom (TeamEditWindow.FROM_PVE);
		});
	}
	/// <summary>
	/// 刷新窗口
	/// </summary>
	private void updateUI () {
		GuildAltar altar = GuildManagerment.Instance.getGuildAltar ();
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + GuildManagerment.Instance.getGuildBossIcon (), bossTexture);
		weakDes.text = GuildManagerment.Instance.getGuildWeakness ().Replace (Language ("guildAltar15"), "");
		blessDes.text = GuildManagerment.Instance.getGuildBossBless ();
		myHurtNum.text = Language ("guildAltar04") + GuildManagerment.Instance.getMyHurt ().ToString ();
		long currentHurtSum = GuildManagerment.Instance.getGuildAltar ().hurtSum;
		long bossSum = getBossPrize (currentHurtSum).hurt;
		float sliderValue = (float)currentHurtSum / bossSum;
		sliderValue = Mathf.Min (sliderValue ,1);
		bossBloodSlider.value = sliderValue;//boss血量
		bossBloodNum.text = currentHurtSum + "/" + bossSum;
		teamDes.text = Language ("s0440") + ArmyManager.Instance.getActiveArmyName ();
		InitRankContent ();
		InitButton ();
		InitBossName ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 初始化排行榜
	/// </summary>
	private void InitRankContent () {
		List<GuildAltarRank> list = GuildManagerment.Instance.filterRank ();
		for (int i = 0; i < list.Count; i++) {
			items [i].names.text = list [i].playerName;
			items [i].values.text = list [i].hurtValue.ToString ();
			items [i].gameObject.SetActive (true);
		}
	}
	/// <summary>
	/// 公会boss挑战端口
	/// </summary>
	private void battlePrepare () {
		GuildCallegeFPort fport = FPortManager.Instance.getFPort ("GuildCallegeFPort") as GuildCallegeFPort;
		fport.access (ArmyManager.PVE_TEAMID, () => {	
			MaskWindow.instance.setServerReportWait (true);
			GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
		});
	}
	/// <summary>
	///  初始化按钮
	/// </summary>
	public void InitButton () {
		if((ServerTimeKit.getDateTime().Hour >= 0 && ServerTimeKit.getDateTime().Second > 0) && (ServerTimeKit.getDateTime().Hour < 6 && ServerTimeKit.getDateTime().Second <= 59))
		{
		    buttonChallenge.textLabel.text = Language("guildAltar10");
			buttonChallenge.disableButton(true);
		} else {
			int challengeCount = 3 - GuildManagerment.Instance.getGuildAltar ().count;
			if (challengeCount <= 0) {
				buttonChallenge.textLabel.text = Language ("guildAltar10") + "(" + challengeCount.ToString () + "/3" + ")";
				buttonChallenge.disableButton (true);
			}
			else {
				buttonChallenge.textLabel.text = Language ("guildAltar10") + "(" + challengeCount.ToString () + "/3" + ")";
			}
		}
	}
	/// <summary>
	/// 初始化BOSS名称
	/// </summary>
	public void InitBossName () {
		string bossName = GuildManagerment.Instance.getGuildBossName ();
		if (bossName == Language ("guildAltarBoss01")) {
			bossNameSprite.spriteName = "boss_bxnw";
		}
		if (bossName == Language ("guildAltarBoss02")) {
			bossNameSprite.spriteName = "boss_ryjr";
		}
		if (bossName == Language ("guildAltarBoss03")) {
			bossNameSprite.spriteName = "boss_lmnw";
		}
		if (bossName == Language ("guildAltarBoss04")) {
			bossNameSprite.spriteName = "boss_swqs";
		}
		if (bossName == Language ("guildAltarBoss05")) {
			bossNameSprite.spriteName = "boss_yzsh";
		}
	}
	public GuildBossPrizeSample getBossPrize (long currentHurtSum) {
		List<GuildBossPrizeSample> bossPrizes = GuildPrizeSampleManager.Instance.getPrizes ();
		foreach (GuildBossPrizeSample s in bossPrizes) {
			if (currentHurtSum < s.hurt) {
				return s;
			}
		}
		return bossPrizes[bossPrizes.Count - 1];
	}
}
