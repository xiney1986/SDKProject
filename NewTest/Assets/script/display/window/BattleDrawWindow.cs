using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 战斗抽奖窗口
 * @author 汤琦
 * */
public class BattleDrawWindow : WindowBase
{
	public ButtonBase drawButtonLeft;//抽奖左按钮
	public ButtonBase drawButtonRight;//抽奖右按钮
	public ButtonBase drawButtonGet;//领奖按钮
	public GameObject showtitle;//奖励提示

	public UILabel costLabelLeft;//左消费文本
	public UILabel costLabelRight;//右消费文本
	public UILabel starSum;//星星总量文本
	public UILabel ruleLabel;//规则描述
	public List<ButtonBattleDraw> prizeList;//所有奖励集
	public List<ButtonBattleDraw> prizeShowList;//需要展示的奖励
	private int index = 0;//当前激活的奖励索引
	public Timer timer;//计时器
	private int cycles = 0;//圈数
	private bool isSpeedDown = false;//是否减速
	private LuckyDraw lucky;//抽奖条目
	private LuckyDrawResults results;//抽奖结果
	private List<PrizeSample> psList;
	private List<int> indexList;//多抽中奖索引集
	private const int LEFT = 1;//点击左边按钮标识
	private const int RIGHT = 2;//点击左边按钮标识
	private EffectCtrl effectCtrl;
	private int drawTimes = 0;//可抽奖次数
	private bool isSend = false;//是否十连抽
	private bool isPlayAudio = false;//是否播放了十连抽音效

    private Dictionary<int, EffectCtrl> mEffectDic;

	protected override void begin ()
	{
		base.begin ();
        mEffectDic = new Dictionary<int, EffectCtrl>();
		cacheRes ();
	}

	void cacheRes ()
	{
		string[] _list = new string[]{	
		"Effect/UiEffect/battleDrawWindow_StarsDraw"
		};
		ResourcesManager.Instance.cacheData (_list, cacheWindowFinish, "base");
	}
	
	public void cacheWindowFinish (List<ResourcesData> _list)
	{
		loadData ();
		loadInfo ();
		MaskWindow.UnlockUI ();

	}

	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		loadInfo ();
		updateShowButton (false);
		updateButton (true);
	}

	//加载数据
	private void loadData ()
	{
		psList = getRandomList ();
		for (int i = 0; i < prizeList.Count; i++) {
			prizeList [i].initInfo (psList [i]);
		}
		for (int i = 0; i < prizeShowList.Count; i++) {
			prizeShowList [i].clearDate ();
			prizeShowList [i].num.text = "";
			prizeShowList [i].icon.gameObject.SetActive (false);
		}
		lucky = LuckyDrawManagerment.Instance.getStarLuckyDraw ();
	}
	//获得随机列表
	private List<PrizeSample> getRandomList ()
	{
		List<PrizeSample> list = BattleDrawSampleManager.Instance.getBattleDrawSampleBySid (10000).list;
		PrizeSample[] copyArray = new PrizeSample[list.Count];
		list.CopyTo (copyArray);

		List<PrizeSample> copyList = new List<PrizeSample> ();
		ListKit.AddRange (copyList, copyArray);
	
		List<PrizeSample> outputList = new List<PrizeSample> ();
		System.Random rd = new System.Random (DateTime.Now.Millisecond);
			
		while (copyList.Count > 0) {
			int rdIndex = rd.Next (0, copyList.Count - 1);
			PrizeSample remove = copyList [rdIndex];
				
			copyList.Remove (remove);
			outputList.Add (remove);
		}
		return outputList;
	}

	//加载界面
	private void loadInfo ()
	{
		starSum.text = "x " + UserManager.Instance.self.starSum;
		drawButtonLeft.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [0].getDrawTimes ().ToString ());
		costLabelLeft.text = "x " + lucky.ways [0].getCostPrice (lucky.getFreeNum ());


		int s = UserManager.Instance.self.starSum;
		if (s >= lucky.ways [1].getCostPrice (lucky.getFreeNum ()) || s < (lucky.ways [0].getCostPrice (lucky.getFreeNum ())) * 2) {
			drawButtonRight.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [1].getDrawTimes ().ToString ());
			costLabelRight.text = "x " + lucky.ways [1].getCostPrice (lucky.getFreeNum ());
			isSend = false;
		} else {
			drawTimes = s / lucky.ways [0].getCostPrice (lucky.getFreeNum ());
			drawButtonRight.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", drawTimes.ToString ());
			costLabelRight.text = "x " + (lucky.ways [0].getCostPrice (lucky.getFreeNum ()) * drawTimes);
			isSend = true;
		}
		//ruleLabel.text = LanguageConfigManager.Instance.getLanguage("s0378");
	}
	//按钮更新
	private void updateButton (bool isOk)
	{
		drawButtonLeft.gameObject.SetActive (isOk);
		drawButtonRight.gameObject.SetActive (isOk);
	}

	private void updateShowButton (bool isOk)
	{
		showtitle.SetActive (isOk);
		drawButtonGet.gameObject.SetActive (isOk);
	}

    private void clearEffect()
    {
        foreach (KeyValuePair<int, EffectCtrl> item in mEffectDic)
        {
            prizeList[item.Key].clearDate();
            EffectManager.Instance.removeEffect(item.Value);
        }
        mEffectDic.Clear();
    }

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "drawButtonLeft") {
			if (isStorageFulls ()) {
				return;
			}
			if (isDraw (lucky.ways [0])) {
				updateButton (false);
				updateShowButton (false);

				LuckyDrawFPort port = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
				port.luckyDraw (1, lucky.sid, 1, lucky.ways [0], drawOne);
			}
		} else if (gameObj.name == "drawButtonRight") {
			if (isStorageFulls ()) {
				return;
			}
			if (isDraw (lucky.ways [1])) {
				updateButton (false);
				updateShowButton (false);
				MaskWindow.LockUI ();

				LuckyDrawFPort port = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
				if (isSend) {
					port.luckyDraw (drawTimes, lucky.sid, 1, lucky.ways [0], drawTwo);
				} else {
					port.luckyDraw (10, lucky.sid, 2, lucky.ways [1], drawTwo);
				}
			}
		} else if (gameObj.name == "drawButtonGet") {
			for (int i = 0; i < prizeShowList.Count; i++) {
				prizeShowList [i].prize = null;
				prizeShowList [i].clearDate ();
				prizeShowList [i].num.text = "";
				prizeShowList [i].back.spriteName = "qualityIconBack_1";
				prizeShowList [i].icon.gameObject.SetActive (false);
			}
			updateButton (true);
			updateShowButton (false);
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0120"));
            clearEffect();
		} else if (gameObj.name == "close") {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			if (drawButtonGet.gameObject.activeSelf) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("awardSave"));
			}
            clearEffect();
			finishWindow ();
		} else if (gameObj.name == "titleline") {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0378"),MessageAlignType.left);
		} else {
			MaskWindow.UnlockUI ();
		}
	}

	//检查相关仓库是否满
	private bool isStorageFulls ()
	{
		if (StorageManagerment.Instance.isRoleStorageFull (1) || StorageManagerment.Instance.isBeastStorageFull (1) || StorageManagerment.Instance.isEquipStorageFull (1) || StorageManagerment.Instance.isPropStorageFull (0)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0172"));
			return true;
		} else if (StorageManagerment.Instance.isTempStorageFull (20)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0173"));
			return true;
		}
		return false;
	}

	//检查玩家的星星数是否达到消费数量
	private bool isDraw (DrawWay ways)
	{
		if (UserManager.Instance.self.starSum>= ways.getCostPrice (lucky.getFreeNum ())) {
			return true;
		} else {
			if (isSend) {
				if (UserManager.Instance.self.starSum>= (lucky.ways [0].getCostPrice (lucky.getFreeNum ()) * drawTimes)) {
					return true;
				}
			}
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0377"));
			return false;
		}
	}

	private void drawTwo (LuckyDrawResults results)
	{
		for (int i = 0; i < prizeList.Count; i++) {
			prizeList [i].clearDate ();
		}
		if (GameObject.FindObjectOfType<EffectCtrl> () != null) {
			for (int i = 0; i < GameObject.FindObjectsOfType<EffectCtrl>().Length; i++) {
				Destroy (GameObject.FindObjectsOfType<EffectCtrl> () [i].gameObject);
			}
		}

		//修正lastStarNum
		UserManager.Instance.self.setLastStarSum (UserManager.Instance.self.starSum);
		
		loadInfo ();
		isPlayAudio = false;
		this.results = results;
		timer = TimerManager.Instance.getTimer (100);
		timer.addOnTimer (playResultTwo);
		timer.start (); 
	}

	//单次抽奖
	private void drawOne (LuckyDrawResults results)
	{
		for (int i = 0; i < prizeList.Count; i++) {
			prizeList [i].clearDate ();
		}
		if (GameObject.FindObjectOfType<EffectCtrl> () != null) {
			for (int i = 0; i < GameObject.FindObjectsOfType<EffectCtrl>().Length; i++) {
				Destroy (GameObject.FindObjectsOfType<EffectCtrl> () [i].gameObject);
			}
		}
		UserManager.Instance.self.setLastStarSum (UserManager.Instance.self.starSum);
		loadInfo ();
		this.results = results;
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (playResultOne);
		timer.start (); 
	}

	private void moveEffect (Transform trans)
	{
		if (effectCtrl == null)
			effectCtrl = EffectManager.Instance.CreateEffect (trans, "Effect/UiEffect/battleDrawWindow_StarsDraw");
		effectCtrl.transform.position = trans.position;
	}

	//播放效果单抽
	private void playResultOne ()
	{
		index ++;
		if (index >= prizeList.Count) {
			cycles ++;
			index = 0;
		}
		for (int i = 0; i < prizeList.Count; i++) {
			if (i == index) {
				moveEffect (prizeList [i].transform);
			}
		}
		if (cycles == 3 && index == changeIndex ()) {
			isSpeedDown = true;
			timer.reset ();
		}
		if (isSpeedDown) {
			speedDown ();
		} else {
			speedUp ();
		}
	}
	//播放效果多抽
	private void playResultTwo ()
	{
		index ++;
		for (int i = 0; i < prizeList.Count; i++) {
			cycles ++;
		}
		EffectCtrl ctrl;
		UnityEngine.Object audioObj;
		for (int i = 0; i < prizeList.Count; i++) {
			if ((i + index) % 2 == 0) {
				ctrl = EffectManager.Instance.CreateEffect (prizeList [i].transform, "Effect/UiEffect/battleDrawWindow_StarsDraw");
				ctrl.life = 0.1f;
				if (isPlayAudio) {
					audioObj = ctrl.gameObject.GetComponent ("AudioPlayer");
					if (audioObj != null) {
						Destroy (audioObj);
					}
				}
				isPlayAudio = true;
			}
		}
		if (cycles >= 400) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			cycles = 0;
			index = 0;
			StartCoroutine (drawTwoResult ());
		}
	}

	private IEnumerator playEffect (int i)
	{
		yield return new WaitForSeconds (0.2f);
        if (mEffectDic.ContainsKey(i)) yield break;

		EffectCtrl effectCtrl = EffectManager.Instance.CreateEffect (prizeList [i].transform,"Effect/UiEffect/Surroundeffect", "Surroundeffect_y");
		effectCtrl.transform.localPosition = Vector3.zero;
		effectCtrl.transform.localScale = new Vector3(1.5f,1.5f,1);
		effectCtrl.transform.parent = prizeList [i].transform;
        mEffectDic.Add(i, effectCtrl);
	}
	
	private IEnumerator drawTwoResult ()
	{
		int _i = 0;
		for (int i = 0; i < getResultIndex().Count; i++) {
			ButtonBattleDraw draw = prizeList [getResultIndex () [i]];
			yield return new WaitForSeconds (0.5f);
			getAwardEffect (_i, draw.getPrize ());
			_i++;
			if (draw.select.isValid) {
				draw.drawNum ++;
				if (draw.drawNum >= 2) {
					draw.value.text = "x " + draw.drawNum;
					draw.value.gameObject.SetActive (true);
				}

			}
			StartCoroutine (playEffect (getResultIndex () [i]));
		}
		yield return new WaitForSeconds(1f);
		updateButton (false);
		updateShowButton (true);
		MaskWindow.UnlockUI ();
	}

	private void getAwardEffect (int _i, PrizeSample prize)
	{
		StartCoroutine (Utils.DelayRun (() => {
			iTween.ShakePosition (prizeShowList [_i].gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.35f));
			iTween.ShakePosition (prizeShowList [_i].gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.35f));
			
			StartCoroutine (Utils.DelayRun (() => {
				GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
				obj.transform.parent = prizeShowList [_i].gameObject.transform;
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = new Vector3 (0, 0, -600);
				;
				
				StartCoroutine (Utils.DelayRun (() => {
					
					prizeShowList [_i].initInfo (prize);
					
					TweenAlpha tp = TweenAlpha.Begin (prizeShowList [_i].icon.gameObject, 0.5f, 1);
					tp.from = 0;
					EventDelegate.Add (tp.onFinished, () => {
						
						
					});
					
				}, 0.3f));
				
			}, 0.1f));
			
		}, 0.5f));
	}

	private int changeIndex ()
	{
		int temp = getResultIndex () [0] - 4;
		if (temp < 0) {
			temp = prizeList.Count + temp;
		}
		return temp;
	}

	//加速
	private void speedUp ()
	{
		timer.delayTime -= 300;
		if (timer.delayTime <= 100) {
			timer.delayTime = 1;
		}
	}
	//减速
	private void speedDown ()
	{
		timer.delayTime += 100;
		if (index == getResultIndex () [0]) {
			StartCoroutine (playEffect (index));
			timer.stop ();
			timer = null;

			cycles = 0;
			isSpeedDown = false;

			getAwardEffect (0, prizeList [index].getPrize ());
			StartCoroutine (Utils.DelayRun (() => {
				updateButton (false);
				updateShowButton (true);
				MaskWindow.UnlockUI ();
			}, 2f));
		}
	}
	
	private List<int> getResultIndex ()
	{
		List<SinglePrize> list = results.getSinglePrizes ();
		indexList = new List<int> ();
		int temp = 0;
		for (int i = 0; i < list.Count; i++) {
			SinglePrize single = list [i];
			for (int j = 0; j < psList.Count; j++) {
				if (single.type == "money" || single.type == "rmb") {
					if (changeType (single.type) == psList [j].type) {
						temp = j;
					}
					if (changeType (single.type) == psList [j].type && single.num == psList [j].getPrizeNumByInt ()) {
						indexList.Add (j);
						break;
					} else if (j == psList.Count - 1) {
						indexList.Add (temp);
						break;
					}
				} else {
					if (single.sid == psList [j].pSid) {
						temp = j;
					}
					if (single.sid == psList [j].pSid && single.num == psList [j].getPrizeNumByInt ()) {
						indexList.Add (j);
						break;
					} else if (j == psList.Count - 1) {
						indexList.Add (temp);
						break;
					}
				}
			}
		}
		return indexList;
	}

	private int changeType (string type)
	{
		switch (type) {
		case "money":
			return 1;
		case "rmb":
			return 2;
		}
		return 0;
	}
}
