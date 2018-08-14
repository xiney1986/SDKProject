using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 幸运女神主窗口
/// @author 丁杰
/// </summary>
public class GuildLuckyNvShenWindow : WindowBase
{
	/** 自身积分 */
	public UILabel selfIntegral; 
	/** 公会积分 */
	public UILabel guildIntegral; 
	/** 本周最高积分 */
	public UILabel topIntegral; 
	/** 投掷次数 */
	public UILabel shakeCount; 
	/** 积分排行 */
	public GameObject integralRankContent; 
	/** 投掷奖励 */
	public GuildShakeRewardContent shakeRewardContent;
	/** 公会奖励 */
	public GameObject guildRewardContent;
	/** 排行榜根节点 */
	public GameObject rankRoot;
	/** 公会奖励根节点 */
	public GameObject guildRewardRoot;
	/** 公会排行预制 */
	public GameObject rankItemPrefab;
	/** 规则预制 */
	public GameObject ruleItemPrefab;
	/** 记录当前content的index,默认为积分排行 */
	private int index = 0;

	/** 骰子动画播放器 */
	public ShakeEblowsCtrl eblowsCtrl;
	/** 骰子初始化状态 */
	public GameObject eblowsStatic;
	/** 骰子加锁信息 */
	public static string lockString = "";
	public TapContentBase tapContentBase;
	private Vector3 oldV3;
	/** 排名贡献奖励 */
	private int[] awardContribution;

	/** 是否可以摇 */
	private bool isCanShake = false;
	protected override void begin ()
	{
		base.begin ();
		eblowsCtrl.Init ();
		initWindow ();
		MaskWindow.UnlockUI ();
	}

	/** 激活 */
	protected override void DoEnable () {
		base.DoEnable ();
		if (fatherWindow is GuildMainWindow) {
			GuildMainWindow win=fatherWindow as GuildMainWindow;
			UiManager.Instance.backGround.switchSynToDynamicBackground (win.launcherPanel, "gangBG", BackGroundCtrl.gangSize);
		}
	}

	private void initWindow ()
	{
		GuildGetLuckyNvShenInfoFport fport = FPortManager.Instance.getFPort ("GuildGetLuckyNvShenInfoFport") as GuildGetLuckyNvShenInfoFport;
		fport.getLuckyNvShenInfo (updateUI);
	}

	private void updateUI ()
	{
		UpdateWindow (GuildManagerment.Instance.getGuildLuckyNvShenInfo ());
		if (GuildManagerment.Instance.getGuildShakeResult () != null) {
			isCanShake = false;
			openResultWindow ();
		} else {
			isCanShake = true;
		}
	}
	/// <summary>
	/// 刷新界面
	/// </summary>
	private void UpdateWindow (GuildLuckyNvShenInfo info)
	{
		selfIntegral.text = "[FFF0C1]" + Language ("GuildLuckyNvShen_03") + "[53DD6A]" + info.selfIntegral;
		guildIntegral.text = "[FFF0C1]" + Language ("GuildLuckyNvShen_04") + "[53DD6A]" + info.guildIntegral;
		topIntegral.text = "[FFF0C1]" + Language ("GuildLuckyNvShen_05") + "[53DD6A]" + info.topIntegral;
		shakeCount.text = "[FFF0C1]" + Language ("GuildLuckyNvShen_06") + "[53DD6A]" + info.shakeCount;
		tapContentBase.changeTapPage (tapContentBase.getTapButtonByIndex (index));
	}

	/// <summary>
	/// 隐藏所有的Content
	/// </summary>
	private void hideAllContent ()
	{
		integralRankContent.gameObject.SetActive (false);
		shakeRewardContent.gameObject.SetActive (false);
		guildRewardContent.gameObject.SetActive (false);

	}
	/// <summary>
	/// 显示积分排行
	/// </summary>
	private void showIntegralRankContent ()
	{
		hideAllContent ();
		integralRankContent.SetActive (true);
		List<GuildShakeRankItem> rankInfos = RankManagerment.Instance.guildShakeList;
		int i = 0;
		/** 清空排行榜信息 */
		UIUtils.M_removeAllChildren (rankRoot);
		/** 初始化排行榜界面 */
		string contriStr = LanguageConfigManager.Instance.getLanguage ("guildMain12");
		foreach (GuildShakeRankItem info in rankInfos) {
			info.contribution = GuildManagerment.Instance.getContribution(i+1) +contriStr ;
			RankItemView item = NGUITools.AddChild (rankRoot, rankItemPrefab).GetComponent<RankItemView> ();
			item.fatherWindow = this;
			item.init (info, RankManagerment.TYPE_GUILD_SHAKE, i);
			i++;
		}
		rankRoot.GetComponent<UIGrid> ().Reposition ();

	}
	/// <summary>
	/// 显示投掷奖励
	/// </summary>
	private void showShakeRewardContent ()
	{
		hideAllContent ();
		shakeRewardContent.gameObject.SetActive (true);
		List<ShakeEblowsRewardSample> allSample = ShakeEblowsRewardSampleManager.Instance ().GetNormalShakeEblowRewardSamples ();
		shakeRewardContent.Init (allSample);
		shakeRewardContent.otherDes.text = Colors.RED + ShakeEblowsRewardSampleManager.Instance ().GetFiveDiffSample ().getPrizesDesc ();
	}
	/// <summary>
	/// 显示公会奖励
	/// </summary>
	private void showGuildRewardContent ()
	{
		hideAllContent ();
		guildRewardContent.SetActive (true);
		UIUtils.M_removeAllChildren (guildRewardRoot);
		string ruleDes = Language ("GuildLuckyNvShen_13");
		ruleDes = ruleDes.Replace ("\\n", "|");
		string [] rules = ruleDes.Split ('|');
		foreach (string rule in rules) {
			UILabel label = NGUITools.AddChild (guildRewardRoot, ruleItemPrefab).GetComponent<UILabel> ();
			label.text = rule;
		}
		guildRewardRoot.GetComponent<UIGrid> ().Reposition ();
	}

	public void getShakeReward ()
	{
		lockString = "";
		GuildGetShakeReward fport = FPortManager.Instance.getFPort ("GuildGetShakeReward") as GuildGetShakeReward;
		fport.getShakeReward (getRewardCallBack);
	}

	private void getRewardCallBack (int addValue)
	{
		UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
			win.dialogCloseUnlockUI=false;
			win.Initialize (LanguageConfigManager.Instance.getLanguage ("GuildLuckyNvShen_21"),false);
			if(addValue>0)
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("GuildLuckyNvShen_25",addValue.ToString()),false);			                                                                             
		});
		List<PrizeSample> prizes = ShakeEblowsRewardSampleManager.Instance ().GetPrizeByResult (GuildManagerment.Instance.getGuildShakeResult ());

		/** 前台更新面板数据 */
		/** 更新个人积分 */
		int addIntegral = 0;
		int lastIntegral = GuildManagerment.Instance.getGuildLuckyNvShenInfo ().selfIntegral;
		int nowIntegral = 0;
		foreach (PrizeSample p in prizes) {
			if (p.type == PrizeType.PRIZE_SHAKE_SCORE) {
				addIntegral += p.getPrizeNumByInt ();
				break;
			}
		}
		nowIntegral = lastIntegral + addIntegral;
		GuildManagerment.Instance.getGuildLuckyNvShenInfo ().selfIntegral = nowIntegral;
		/** 更新周最高积分 , 公会积分 */
		if (nowIntegral > GuildManagerment.Instance.getGuildLuckyNvShenInfo ().topIntegral) {
			GuildManagerment.Instance.getGuildLuckyNvShenInfo ().topIntegral = nowIntegral;
			GuildManagerment.Instance.getGuildLuckyNvShenInfo ().guildIntegral += (nowIntegral - lastIntegral);
			updateRankInfo (nowIntegral);
		}
		/** 清空结果 */
		GuildManagerment.Instance.clearGuildShakeResult ();
		/** 刷新面板 */
		updateUI ();
	}


	private void updateRankInfo (int integral)
	{
		bool isExist = false;
		foreach (GuildShakeRankItem item in RankManagerment.Instance.guildShakeList) {
			if (item.uid == UserManager.Instance.self.uid) {
				item.integral = integral;
				isExist = true;
				break;
			}
		}
		if (!isExist) {
			GuildShakeRankItem item = new GuildShakeRankItem (UserManager.Instance.self.uid, UserManager.Instance.self.nickname, integral);
			RankManagerment.Instance.guildShakeList.Add (item);
		}
		GuildShakeRankItem [] rankArray = RankManagerment.Instance.guildShakeList.ToArray ();
		SetKit.sort (rankArray, new ShakeRankInfoCompare ());
		RankManagerment.Instance.guildShakeList.Clear ();
		RankManagerment.Instance.guildShakeList.AddRange (rankArray);
		if (index == 0)
			showIntegralRankContent ();
	}


	/// <summary>
	/// 开始投掷骰子
	/// </summary>
	public void beginShake ()
	{
		if (GuildManagerment.Instance.getGuildLuckyNvShenInfo ().shakeCount <= 0) {
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (Language ("GuildLuckyNvShen_11"));
			});
		} else {
			GuildShakeElbowsFport fport = FPortManager.Instance.getFPort ("GuildShakeElbowsFport") as GuildShakeElbowsFport;
			fport.beginShakeElbows (shakeCallBack);
		}

	}
	/// <summary>
	/// 重新投掷
	/// </summary>
	public void beginReshake (string locks)
	{
		lockString = locks;
		GuildShakeElbowsFport fport = FPortManager.Instance.getFPort ("GuildShakeElbowsFport") as GuildShakeElbowsFport;
		fport.reShakeElbows (locks, shakeCallBack);
	}

	/// <summary>
	/// 摇骰子通讯回调
	/// </summary>
	private void shakeCallBack ()
	{
		MaskWindow.LockUI ();
		eblowsStatic.gameObject.SetActive (false);
		eblowsCtrl.gameObject.SetActive (true);
		eblowsCtrl.playAnim (5);
		StartCoroutine (Utils.DelayRun (eblowsCtrl.stopAnim, 1.5f));
		StartCoroutine (Utils.DelayRun (openResultWindow, 1.5f));
	}


	/// <summary>
	/// 打开结果窗口
	/// </summary>
	private void openResultWindow ()
	{
		UiManager.Instance.openDialogWindow<GuildShakeElbowsResultWindow> ((win) => {
			EventDelegate.Add (win.OnStartAnimFinish, () => {
				MaskWindow.UnlockUI();
			});
			win.SetFatherWindow (this);
			win.Init (GuildManagerment.Instance.getGuildShakeResult ());
			eblowsStatic.gameObject.SetActive (true);
			eblowsCtrl.gameObject.SetActive (false);
		});
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			this.finishWindow ();
		} else if (gameObj.name == "BeginShake") {
			beginShake ();
		}

	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		if (!enable)
			return;
		base.tapButtonEventBase (gameObj, enable);
		//积分排行
		if (gameObj.name == "ButtonIntegralRank") {
			showIntegralRankContent ();
			index = 0;
		}
		//投掷奖励
		else if (gameObj.name == "ButtonShakeReward") {
			showShakeRewardContent ();
			index = 1;
		} 
		//公会奖励
		else if (gameObj.name == "ButtonGuildReward") {
			showGuildRewardContent ();
			index = 2;
		}
	}

	void Update ()
	{
		if (isCanShake && checkPhoneShake () ) {
			beginShake ();
		}
	}

	private bool checkPhoneShake ()
	{
		Vector3 newV3 = Input.acceleration;
		bool bl = (Mathf.Abs (newV3.x - oldV3.x) > 1 || Mathf.Abs (newV3.y - oldV3.y) > 1 || Mathf.Abs (newV3.z - oldV3.z) > 1);
		oldV3 = newV3;
		return bl;
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		initWindow ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();

	}

}

public class ShakeRankInfoCompare : Comparator
{
	public int compare (object a, object b)
	{
		GuildShakeRankItem itemA = a as GuildShakeRankItem;
		GuildShakeRankItem itemB = b as GuildShakeRankItem;
		if (itemA.integral > itemB.integral)
			return -1;
		else if (itemA.integral < itemB.integral)
			return 1;
		else  
			return 0;
	}
}
