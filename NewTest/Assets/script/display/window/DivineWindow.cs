using UnityEngine;
using System.Collections.Generic;

public class DivineWindow : WindowBase
{
	/** 满运势 */
	private const int MAX_DIVINE = 50;
	/** 女神形象 */
	public UITexture texNvShen;
	/** 星座标志 */
	public UISprite usXingZuo;
	/** 星座名称 */
	public UILabel lblName;	
	/** 星座日期 */
	public UILabel lblDate;
	/** 运势标题 */
	public UILabel lblYunShiTitle;
	/** 运势解说 */
	public UILabel lblYunShiContent;
	/** 我的运势 */
	public UILabel lblMyInfo;
	/** 奖励列表 */
	public GoodsView[] awards;
	/** 已领取标志 */
	public GameObject received;
	/** 占卜按钮 */
	public GameObject btnDivine;
	/** 占分享卜按钮 */
	public GameObject btnDivineShare;
	/** 关闭按钮 */
	public GameObject btnClose;
	/** 奖励介绍 */
	public UILabel awardText01;
	/** 奖励介绍02 */
	public UILabel awardText02;
	/** 特效 */
	public GameObject effect;
	/** 经验条 */
	public barCtrl expbar;
	/** 经验值 */
	public UILabel expLabel;
	public GoodsView awardTips01;
	public GoodsView awardTips02;
	public GameObject nvshen;
	public GameObject yunshi;
	public UILabel xzname_ys;
	/** 奖励类型 */
	private const int AWARD_TYPE = 4;

	protected override void begin () {
		base.begin ();
		Horoscopes hs = HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star);
		usXingZuo.spriteName = hs.getSpriteName ();
		lblName.text = hs.getName ();
		xzname_ys.text = hs.getName();
		lblDate.text = hs.getDate ();
		lblMyInfo.text = LanguageConfigManager.Instance.getLanguage ("divine_05", UserManager.Instance.self.divineFortune.ToString ());
//		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + hs.getImageID () + "_head", texNvShen);
		updateExp ();
		/* 奖励预览，满5(取5)，满50(取50) */
		List<ArenaAwardSample> awardSamples = ArenaAwardSampleManager.Instance.getSamplesByType (AWARD_TYPE);
		PrizeSample ps1 = null ;
		PrizeSample ps2 = null;
		foreach (ArenaAwardSample a in awardSamples) {
			if (a.condition == 5) {
				if (a.prizes.Length != 0) {
					ps1 = a.prizes[0];
				}
			}
			if (a.condition == 50) {
				if (a.prizes.Length != 0) {
					ps2 = a.prizes[0];
				}
			}
		}
		if(ps1 != null && ps2 != null){
			awardText01.text = LanguageConfigManager.Instance.getLanguage("divine_08",ps1.getPrizeName(),ps2.getPrizeName());
		}
		MaskWindow.UnlockUI ();

	}

	protected override void DoEnable () {
		base.DoEnable ();
		MaskWindow.LockUI ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		if (gameObj.name == "close" || gameObj.name == "buttonClose") {
			finishWindow ();
		} else if (gameObj.name == "buttonDivine") {
			FPortManager.Instance.getFPort<DivineSendFPort> ().access (OnDivineSendBack);
		}
	}

	/// <summary>
	/// 占卜后回调
	/// </summary>
	/// <param name="num">增加运势.</param>
	/// <param name="awardSid">基础奖励sid.</param>
	/// <param name="shareAward">分享额外增加运势.</param>
	private void OnDivineSendBack (int num, int awardSid, int shareAward)
	{
		UserManager.Instance.self.canDivine = false;
		nvshen.SetActive(false);

		int add = num;//num - UserManager.Instance.self.divineFortune;
		if (add <= 0) {
			finishWindow ();
			return;
		}
		Destroy (effect);
		if (shareAward > 0) {
			lblYunShiTitle.text = LanguageConfigManager.Instance.getLanguage ("divine_01", UserManager.Instance.self.divineFortune.ToString (), add.ToString (), shareAward.ToString ());
		}
		else {
			lblYunShiTitle.text = LanguageConfigManager.Instance.getLanguage ("divine_010", UserManager.Instance.self.divineFortune.ToString (), add.ToString ());
		}
		int max = StringKit.toInt (LanguageConfigManager.Instance.getLanguage ("divine_yunshi_" + add + "_count"));
		int random = Random.Range (0, max);
		lblYunShiContent.text = LanguageConfigManager.Instance.getLanguage ("divine_yunshi_" + add + "_" + random);
		lblMyInfo.text = LanguageConfigManager.Instance.getLanguage ("divine_06", UserManager.Instance.self.divineFortune.ToString (), add.ToString ());
		if (shareAward > 0) {
			lblMyInfo.text += string.Format (Colors.RED + LanguageConfigManager.Instance.getLanguage ("divine_07") + "[-]", shareAward);
		}

		//刷新我的运势
		//int oldDivineFortune = UserManager.Instance.self.divineFortune;

		int addTotalValue=num + shareAward;

		UserManager.Instance.self.addDivineFortune (addTotalValue);

		//隐藏占卜前的奖励提示
		awards [0].transform.parent.parent.gameObject.SetActive (true);
//		awardText01.gameObject.SetActive (false);
		awardText02.gameObject.SetActive (false);
		awardTips01.gameObject.SetActive (false);
		awardTips02.gameObject.SetActive (false);
		//获得奖励样本
		ArenaAwardSample sample = ArenaAwardSampleManager.Instance.getArenaAwardSampleBySid (awardSid);
		int awardConut = 0;
		if (sample != null) {
			PrizeSample[] ps = sample.prizes;
			for (int i = 0; i < awards.Length && ps != null && i < ps.Length; i++) {
				awards [i].gameObject.SetActive (true);
				awards [i].init (ps [i]);
				awards [i].fatherWindow = this;
				awardConut = i;
			}
		}

		//根据运势值获得的奖励
		int divineFortune = UserManager.Instance.self.divineFortune;
		int oldDivineFortune = divineFortune - addTotalValue;
		List<ArenaAwardSample> infoList = ArenaAwardSampleManager.Instance.getSamplesByType (AWARD_TYPE);
		ArenaAwardSample a;
		PrizeSample[] sampleAward = null;
		for (int i=0 ; i<infoList.Count ; ++i) {
			a = infoList[i];
			if(a.condition == 0)
				continue;
			if(oldDivineFortune <a.condition &&divineFortune>=a.condition)
			{
				sampleAward = a.prizes;
			}
		}
//		List<ArenaAwardSample> infoList = ArenaAwardSampleManager.Instance.getSamplesByType (AWARD_TYPE);
//		PrizeSample[] sampleAward = null;
//		for (int i = 0; i < infoList.Count; i++) {
//			if (infoList [i].condition != 0 && divineFortune == infoList [i].condition && i + 1 < infoList.Count && divineFortune < infoList [i + 1].condition) {
//				sampleAward = infoList [i].prizes;
//			} else if (infoList [i].condition != 0 && divineFortune >= infoList [i].condition && i + 1 >= infoList.Count) {
//				sampleAward = infoList [i].prizes;
//			}
//		}
		if (sampleAward != null) {
			for (int j = 0; j < sampleAward.Length; j++) {
				awards [3 + j].gameObject.SetActive (true);
				awards [3 + j].init (sampleAward [j]);
				awards [3 + j].fatherWindow = this;
			}
		}
		btnDivine.SetActive (false);
		btnDivineShare.SetActive (false);
		btnClose.SetActive (true);
		updateExp ();
		yunshi.SetActive(true);
		MaskWindow.UnlockUI ();
		//TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("divine_03"));
	}
	void updateExp () {
		if (UserManager.Instance.self.divineFortune >= 50) {
			expbar.updateValue (MAX_DIVINE, MAX_DIVINE);
			expLabel.text = LanguageConfigManager.Instance.getLanguage("divine_05",( MAX_DIVINE + "/" + MAX_DIVINE));
		}
		else {
			expbar.updateValue (UserManager.Instance.self.divineFortune, MAX_DIVINE);
			expLabel.text = LanguageConfigManager.Instance.getLanguage("divine_05", UserManager.Instance.self.divineFortune + "/" + MAX_DIVINE);
		}
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		//	FPortManager.Instance.getFPort<StorageFPort> ().init (null);
	}
	/// <summary>
	/// 初始奖励
	/// </summary>
	private void initAwardContent(){

	}
}
