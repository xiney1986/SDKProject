using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 升级奖励展示窗口
/// </summary>
public class LevelupRewardWindow : WindowBase {
	public GameObject rewardItemPrefab;
	public UILabel lab_progressInfo;
	public UILabel lab_rewardTip;
	public UILabel lab_rewardLevel;
	public UILabel lab_curLevelInfo;
	public UISlider expBar;
	public GameObject go_content;
	public ButtonBase btn_reward;
	public GameObject intro_content;
	public GameObject introPrefab;
	private CallBack fatherWindowFun;
	public ButtonBase buttonReceive;
		
		
	public void init (CallBack oncloseCallBackFun) {
		fatherWindowFun = oncloseCallBackFun;
	}

	protected override void DoEnable () {
		base.DoEnable ();
		btn_reward.disableButton (true);
	}

	protected override void begin () {
		base.begin ();
	    UiManager.Instance.levelupRewardWindow = this;
		MaskWindow.UnlockUI ();
		//refreshStatus();
		updateView ();
	}
	/// <summary>
	/// 刷新当前升级奖励 取得最新的信息
	/// </summary>
	public void refreshStatus () {
		LevelupRewardFPort sp = FPortManager.Instance.getFPort ("LevelupRewardFPort") as LevelupRewardFPort;
		sp.access_get (updateView);
	}

	private int tempSid;
	/// <summary>
	/// 更新视图
	/// </summary>
	private void updateView () {		  
		MaskWindow.UnlockUI ();
		int sid = LevelupRewardManagerment.Instance.lastRewardSid;
        sid = sid > 0 ? sid + 1 : sid;
		int curLevel = UserManager.Instance.self.getUserLevel ();
		long curExp = UserManager.Instance.self.getEXP ();
		LevelupSample rewardSample = LevelupRewardSampleManager.Instance.getSampleBySid (sid);
		//如果没有奖励 则按照已经达到顶级等级处理
		if (rewardSample == null) {
			lab_rewardTip.text = LanguageConfigManager.Instance.getLanguage ("levelupReward04");
			lab_curLevelInfo.text = LanguageConfigManager.Instance.getLanguage ("levelupReward02", curLevel + "/" + curLevel);
			lab_progressInfo.text = curExp + "/" + curExp;
			expBar.value = 1;
			btn_reward.disableButton (true);
			lab_rewardLevel.text = "";
			if(fatherWindow is FubenAwardWindow)
				closeWindow();
			return;
		}

		int nextRewardLevel = rewardSample.level;
		long nextRewardExp = EXPSampleManager.Instance.getEXPDown (1, nextRewardLevel);

		lab_rewardTip.text = LanguageConfigManager.Instance.getLanguage ("levelupReward01");
		lab_rewardLevel.text = nextRewardLevel.ToString ();
		lab_curLevelInfo.text = LanguageConfigManager.Instance.getLanguage ("levelupReward02", curLevel + "/" + nextRewardLevel);
		lab_progressInfo.text = curExp + "/" + nextRewardExp;
		expBar.value = (float)curExp / nextRewardExp;

		tempSid = rewardSample.sid;
		btn_reward.disableButton (curLevel < rewardSample.level);

		if (!btn_reward.enabled && fatherWindow is FubenAwardWindow)
			closeWindow ();
		List<PrizeSample> rewardList = rewardSample.samples;
		PrizeSample prize;
		GameObject newItem;
		AllAwardViewButton goodsButton;
		int i = 0, length = rewardList.Count;

		Transform[] childs = go_content.GetComponentsInChildren<Transform> ();
		foreach (Transform item in childs) {
			if (item != go_content.transform) {
				MonoBehaviour.Destroy (item.gameObject);
			}
		}
		childs = null;
		//button.initPrize(prizes[index],fatherWindow,backClose,backOpen);

		for (; i < length; i++) {
			prize = rewardList [i];
			newItem = NGUITools.AddChild (go_content, rewardItemPrefab);
			goodsButton = newItem.GetComponent<AllAwardViewButton> ();
			goodsButton.initPrize (prize, this, null, null);
		}

		childs = intro_content.GetComponentsInChildren<Transform> ();
		foreach (Transform item in childs) {
			if (item != intro_content.transform) {
				MonoBehaviour.Destroy (item.gameObject);
			}
		}
		childs = null;

		string[] descs = rewardSample.descs;
		UILabel label;
		foreach (string intro in descs) {
			newItem = NGUITools.AddChild (intro_content, introPrefab);
			newItem.gameObject.SetActive (true);
			label = newItem.GetComponent<UILabel> ();
			label.text = intro;
		}

		StartCoroutine (Utils.DelayRun (() => {
			go_content.GetComponent<UIGrid> ().Reposition ();	
			intro_content.GetComponent<UIGrid> ().Reposition ();	
		}, 0.1f));

	}

	private void updateTipInfo () {
		lab_curLevelInfo.text = "leveup to";
	}

	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		string targetName = gameObj.name;
		if (targetName == "button_close") {
			closeWindow ();
		}
		else if (targetName == "button_receive") {
			LevelupRewardFPort fport = FPortManager.Instance.getFPort ("LevelupRewardFPort") as LevelupRewardFPort;
			fport.access_receive (onReceiveCmp, tempSid);
			buttonReceive.disableButton(true);
		}
	}
	/// <summary>
	/// 领取奖励后 回调 包括更新当前窗口视图和更新英雄之章
	/// </summary>
	/// <param name="_ok">If set to <c>true</c> _ok.</param>
	private void onReceiveCmp (bool _ok) {
		if (_ok) {
			if(fatherWindow is FubenAwardWindow ){
				closeWindow();
			}else{
				updateView ();
				StartCoroutine (updateHeroRoad ());				
			}

		    if (UiManager.Instance.getWindow<MainWindow>() != null)
		    {
		         UiManager.Instance.getWindow<MainWindow>().update_RMB();
		    }
		}			
	}
	/// <summary>
	/// 领取的奖励中 是否有开启英雄之章的卡片
	/// </summary>
	/// <returns>The hero road.</returns>
	private IEnumerator updateHeroRoad () {
		int sid = LevelupRewardManagerment.Instance.lastRewardSid;
		LevelupSample rewardSample = LevelupRewardSampleManager.Instance.getSampleBySid (sid);

		Card c;
		PrizeSample ps;
		for (int i = 0; i < rewardSample.samples.Count; i++) {
			ps = rewardSample.samples [i];	
			if (ps.type == PrizeType.PRIZE_CARD) {
				c = CardManagerment.Instance.createCard (ps.pSid);
				for (int j = 0; j < ps.getPrizeNumByInt (); j++) {
					if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (c)) {
						yield return new WaitForSeconds (1.5f);
						TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0418"));
					}
				}
			}
		}
	}
	///<summary>
	/// 关闭窗口
	/// </summary>
	private void closeWindow () {
		if (fatherWindowFun != null) {
			fatherWindowFun ();
		}
		fatherWindowFun = null;
		finishWindow ();
	}
    public override void DoDisable() {
        base.DoDisable();
        UiManager.Instance.levelupRewardWindow = null;
    }
}

