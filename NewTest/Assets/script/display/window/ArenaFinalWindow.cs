using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaFinalWindow : WindowBase
{
	const int LOOK_POSITION_MY_TYPE=1, // 我的位置类型
	LOOK_POSITION_CUESS_TYPE=2; // 竞猜定位类型
	
	public GameObject arenaFinalMapPrefab;
	public GameObject rolePointPrefab;
	public GameObject pointRoot;
	public TapButtonBase[] tabButtons;
	public TapContentBase tapContent;
	public UILabel[] guessNumbers;
	public UILabel lblNextRound;
	public UILabel lblNextCD;
	public UILabel lblRank;
	public GameObject effectRoot;
	public GameObject focusCuessButton;
	public ArenaAwardNum numFinal;//积分
	public ArenaAwardNum numGuess;//可竞猜
	public ArenaAwardNum numGuessAward;//竞猜奖励
	[HideInInspector]
	public ArenaFinalMap map;
	Timer cdTimer;
	Timer checkStateTimer;
	ArenaManager am;
	int tapIndex;
	List<ArenaFinalInfo>[] allInfoList;
	[HideInInspector] public ArenaFocusGuess focusGuess;
	/** 点击竞猜定位需要重新tap才能获取数据时临时存储用 */
	ArenaFocusGuess.FocusPointInfo focusPointInfo;
	int myTeamIndex = -1; //我的分组
	/** 初始竞猜分组下标 */
	int guessTapIndex= -1;
	int myRank;
	/** 当前定位类型 */
	int currentLookPositionType;
	ArenaFinalPoint myPoint;
	/**冠军point*/
	ArenaFinalPoint chanPoint;
	/**寻找竞猜结束Label*/
	public UILabel EndLabel;
	
	public override void OnBeginCloseWindow ()
	{
	//	UiManager.Instance.gameCamera.clearFlags = CameraClearFlags.SolidColor;
	//	UiManager.Instance.backGround.restoreBackGround ();
		if (map != null)
			map.gameObject.SetActive (false);
	}
	// Use this for initialization
	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		if (am == null) {
			am = ArenaManager.instance;
			map = (Instantiate (arenaFinalMapPrefab) as GameObject).GetComponent<ArenaFinalMap> ();
			EffectManager.Instance.CreateEffectCtrlByCache (effectRoot.transform, "Effect/UiEffect/MeteorShower", null);
			checkStateTimer = TimerManager.Instance.getTimer (1000);
			checkStateTimer.addOnTimer (CheckState);
			checkStateTimer.start ();
		}
	}
	
	protected override void begin ()
	{
		if (map != null)
			map.gameObject.SetActive (true);
	//	UiManager.Instance.gameCamera.clearFlags = CameraClearFlags.Depth;
	//	UiManager.Instance.backGround.hideAllBackGround (false);
		if (isAwakeformHide) {
			numFinal.loadData ();
			numGuess.loadData (); 
			//注释这段代码，由于上面两句加载有通讯回调，会导致窗口销毁，现在把解锁操作放在numGuess.loadData ()完成后执行，执行顺序不能变
//			MaskWindow.UnlockUI ();
			return;
		}
		
		cacheModel();
	}
	void cacheModel()
	{
		string[] paths=new string[]{
			
			"mission/ez",
			"mission/girl",
			"mission/mage",
			"mission/maleMage",
			"mission/point_start",
			"mission/swordsman",
			"mission/archer",
		};
		ResourcesManager.Instance.cacheData(paths,(list)=>{
			cacheFinish();
		},"other");
	}
	void 	cacheFinish(){
		
		if (ArenaManager.instance.finalInfoList != null) 
		{
			FPortManager.Instance.getFPort<ArenaFinalFPort> ().access (OnDataLoad);
		}
		else
			MaskWindow.UnlockUI ();
	}
	void OnDataLoad ()
	{
		if (am.state >= ArenaManager.STATE_64_32 && am.state <= ArenaManager.STATE_CHAMPION) {
			string name = LanguageConfigManager.Instance.getLanguage ("Arena14_" + am.state);
			string str;
			if (am.state <= ArenaManager.STATE_8_4 && am.finalRound > 0)
				str = LanguageConfigManager.Instance.getLanguage ("Arena15", name, am.finalRound + "");
			else
				str = LanguageConfigManager.Instance.getLanguage ("Arena16", name);
			lblNextRound.text = str;
			if (cdTimer == null && am.finalCD > 0) {
				cdTimer = TimerManager.Instance.getTimer (1000);
				cdTimer.addOnTimer (UpdateCD);
				cdTimer.start ();
			}
			UpdateCD ();
		} else {
			System.DateTime dt = TimeKit.getDateTime (ArenaManager.instance.stateEndTime);
			lblNextCD.transform.localPosition = new Vector3(-85,821,0);
			lblNextCD.effectStyle = UILabel.Effect.None;
			lblNextCD.text = LanguageConfigManager.Instance.getLanguage ("Arena55", dt.Month.ToString (), dt.Day.ToString ());
			lblNextRound.text = LanguageConfigManager.Instance.getLanguage ("Arena24");
			lblRank.text = "";
			if (cdTimer != null) {
				cdTimer.stop ();
				cdTimer = null;
			}
		}
		initAllInfoList ();
		tapContent.changeTapPage ( tabButtons[am.tapIndex]);
//		tapButtonEventBase (tabButtons [tapIndex].gameObject, true);
		numFinal.loadData ();
		numGuess.loadData (); 
		//由于小标签的加载是用的协程，解锁早了，关了窗口，协程里面组件操作会报错，让解锁操作在协程完成后
		//MaskWindow.UnlockUI ();
	}
	
	void Update ()
	{
		
	}
	
	void UpdateCD ()
	{
		string str = TimeKit.timeTransform (am.finalCD * 1000);
		lblNextCD.text = LanguageConfigManager.Instance.getLanguage ("Arena17", str);
		if (am.finalCD > 0) {
			am.finalCD--;
		} else {
			if (cdTimer != null) {
				cdTimer.stop ();
				cdTimer = null;
				FPortManager.Instance.getFPort<ArenaFinalFPort> ().access (OnDataLoad);
			}
		}
	}
	
	void CheckState ()
	{
		if (ArenaManager.instance.state == ArenaManager.STATE_RESET && !ArenaManager.instance.isStateCorrect () && gameObject.activeInHierarchy) {
			checkStateTimer.stop ();
			checkStateTimer = null;
			
			map.gameObject.SetActive (false);
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Arena43"), (msg) => {
				UiManager.Instance.clearWindowsName("MainWindow");
				UiManager.Instance.BackToWindow<MainWindow>();
			});
		}
	}
	void addArenaFinalInfo(List<ArenaFinalInfo> infoList,ArenaFinalInfo[] info){
		foreach(ArenaFinalInfo each in info){
			infoList.Add(each);
		}

	}
	private void initAllInfoList ()
	{
		bool findMe = false;
		allInfoList = new List<ArenaFinalInfo>[5];
		for (int i = 0; i < 5; i++) {
			List<ArenaFinalInfo> infoList = new List<ArenaFinalInfo> ();
			ArenaFinalInfo[][] infoListArr;
			if (i <= 3) {
				infoListArr = new ArenaFinalInfo[5][];
				addArenaFinalInfo(infoList,infoListArr [0] = am.getFinalInfo (i * 16, 16));
				addArenaFinalInfo(infoList,infoListArr [1] = am.getFinalInfo (i * 8 + 64, 8));
				addArenaFinalInfo(infoList,infoListArr [2] = am.getFinalInfo (i * 4 + 96, 4));
				addArenaFinalInfo(infoList,infoListArr [3] = am.getFinalInfo (i * 2 + 112, 2));
				addArenaFinalInfo(infoList,infoListArr [4] = am.getFinalInfo (i + 120, 1));
				
			} else {
				infoListArr = new ArenaFinalInfo[3][];
				addArenaFinalInfo(infoList,infoListArr [0] = am.getFinalInfo (120, 4));
				addArenaFinalInfo(infoList,infoListArr [1] = am.getFinalInfo (124, 2));
				addArenaFinalInfo(infoList,infoListArr [2] = am.getFinalInfo (126, 1));
			}
			allInfoList [i] = infoList;
			
			for (int j = 0; j < infoListArr.Length; j++) {
				foreach (ArenaFinalInfo info in infoListArr[j]) {
					if (i <= 3) {
						info.finalState = ArenaManager.STATE_64_32 + j;
					} else {
						info.finalState = ArenaManager.STATE_4_2 + j;
					}
					
					if (!findMe && info.uid == UserManager.Instance.self.uid) {
						findMe = true;
						myRank = info.finalState - 1;
						myTeamIndex = i;
					}
				}
			}
		}
		lblRank.text = LanguageConfigManager.Instance.getLanguage ("Arena45") + " : " + LanguageConfigManager.Instance.getLanguage ("Arena14_" + myRank + "_2");
	}
	
	public int getMyRank ()
	{
		return myRank;
	}
	
	public void updateGuessNumbers ()
	{
		int now = ServerTimeKit.getSecondTime ();
		for (int i = 0; i < 5; i++) {
			List<ArenaFinalInfo> infoList = allInfoList [i];
			int count = 0;
			for (int j = 0; j < infoList.Count - 1; j+= 2) {
				ArenaFinalInfo info = infoList [j].hasUser () ? infoList [j] : infoList [j + 1];
				if (info.guessStartTime + 60 <= now && now <= info.guessEndTime) {
					guessTapIndex=i;
					if (!(infoList [j].guessed || infoList [j+1].guessed)) {
						count++;
					}
				}
			}
			guessNumbers [i].transform.parent.gameObject.SetActive (count > 0);
			if (count > 0) {
				guessNumbers [i].text = count.ToString ();
			}
		}
	}
	
	private IEnumerator initData (int index)
	{
		pointRoot.GetComponent<UIScrollView> ().ResetPosition ();
		yield return 1;
		map.clear ();
		if(focusGuess==null) focusGuess = new ArenaFocusGuess ();
		else focusGuess.Clear ();
		Utils.DestoryChilds (pointRoot);
		ArenaFinalInfo[][] infos;
		if (index <= 3) {
			infos = new ArenaFinalInfo[5][];
			infos [0] = am.getFinalInfo (index * 16, 16);
			infos [1] = am.getFinalInfo (index * 8 + 64, 8);
			infos [2] = am.getFinalInfo (index * 4 + 96, 4);
			infos [3] = am.getFinalInfo (index * 2 + 112, 2);
			infos [4] = am.getFinalInfo (index + 120, 1);
		} else {
			infos = new ArenaFinalInfo[3][];
			infos [0] = am.getFinalInfo (120, 4);
			infos [1] = am.getFinalInfo (124, 2);
			infos [2] = am.getFinalInfo (126, 1);
		}
		ArenaFinalPoint[][] points = new ArenaFinalPoint[infos.Length][];
		for (int i = 0; i < points.Length; i++) {
			points [i] = new ArenaFinalPoint[infos [i].Length];
		}
		Vector3 postion = new Vector3 (-263, 62, 0);
		//bool isFocusCuess = false;
		for (int i = 0; i < infos.Length; i++) {
			for (int j = 0; j < infos[i].Length; j++) {
				ArenaFinalInfo info = infos [i] [j];
				GameObject obj = NGUITools.AddChild (pointRoot, rolePointPrefab);
				obj.name="rolepoint_"+(i*100)+j;
				obj.SetActive (true);
				if (i == 0) {
					obj.transform.localPosition = postion + new Vector3 (0, j * -233, 0);
				} else {
					ArenaFinalPoint paren1 = points [i - 1] [j * 2];
					ArenaFinalPoint paren2 = points [i - 1] [j * 2 + 1];
					float len = (paren2.transform.localPosition.y - paren1.transform.localPosition.y) / 2;
					obj.transform.localPosition = paren1.transform.localPosition + new Vector3 (200, len, 0); 
				} 
				ArenaFinalPoint point = obj.GetComponent<ArenaFinalPoint> ();
				point.window = this;
				point.init (info);
				points [i] [j] = point;
				if (i > 0) {
					ArenaFinalPoint paren1 = points [i - 1] [j * 2];
					ArenaFinalPoint paren2 = points [i - 1] [j * 2 + 1];
					point.initParent (paren1, paren2);
					if(point.buttonGuess.activeInHierarchy)
					{
						focusGuess.AddFocusCuessPoint(point.gameObject.transform.localPosition,i,j,point.guessNum.activeInHierarchy,index,obj.name);
					}
				}
				chanPoint = point;
				if (info.uid == UserManager.Instance.self.uid) {
					myPoint = point;
				}
				yield return 1;
			}
		}
		focusGuess.SortFocusCuess ();
		ResetSpringPanel ();
		updateGuessNumbers ();
		if(am.state==ArenaManager.STATE_RESET&&am.tapIndex==4)
		{
			lookChanPosition();
			chanPoint.chanName.text = LanguageConfigManager.Instance.getLanguage("Arena80",chanPoint.lblRoleName.text);
			chanPoint.chanName.gameObject.SetActive(true);
			chanPoint.timecd.text = lblNextCD.text;
			chanPoint.nextTime.gameObject.SetActive(true);
			EndLabel.text = LanguageConfigManager.Instance.getLanguage("Arena73");
			EndLabel.gameObject.SetActive(true);
		}
		MaskWindow.UnlockUI();
	}
	
	private void ResetSpringPanel()
	{
		if (currentLookPositionType == LOOK_POSITION_MY_TYPE)
		{
			if (myPoint != null) {
				StartCoroutine (Utils.DelayRunNextFrame (() => {
					SpringPanel.Begin (pointRoot, -myPoint.transform.localPosition, 9);
				}));
			}
		}
		else if(currentLookPositionType == LOOK_POSITION_CUESS_TYPE)
		{
			if(focusPointInfo!=null)
			{
				StartCoroutine (Utils.DelayRunNextFrame (() => {
					SpringPanel.Begin (pointRoot, -focusPointInfo.getFocusPoint(), 9);
					focusPointInfo=null;
				}));
			}
			else
			{
				ArenaFocusGuess.FocusPointInfo pointInfo=focusGuess.focusCuessPoint();
				if(pointInfo!=null)
				{
					StartCoroutine (Utils.DelayRunNextFrame (() => {
						SpringPanel.Begin (pointRoot, -pointInfo.getFocusPoint(), 9);
					}));
				}
			}
		}
		currentLookPositionType = 0;
	}
	
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		if (!enable)
			return;
		int index = 0;
		for (int i = 0; i < tabButtons.Length; i++) {
			if (tabButtons [i].gameObject == gameObj) {
				index = i;
				break;
			}
		}
		am.tapIndex = index;
		MaskWindow.LockUI();
		GameManager.Instance.StartCoroutine (initData (index));
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			am.tapIndex = 0;
			UiManager.Instance.openMainWindow ();
		} else if (gameObj.name == "buttonFinalAward") {
			UiManager.Instance.openWindow<ArenaAwardWindow> ((win) => {
				win.init (ArenaAwardWindow.TYPE_FINAL);
			});
		} else if (gameObj.name == "buttonGuessAward") {
			UiManager.Instance.openWindow<ArenaAwardWindow> ((win) => {
				win.init (ArenaAwardWindow.TYPE_GUESS);
			});
		} else if (gameObj.name == "buttonIntegralAward") {
			UiManager.Instance.openDialogWindow<ArenaIntegralAwardWindow> ((win) => {
				win.initUI ();
			});
		} else if (gameObj.name == "buttonMeritShop") {
			UiManager.Instance.openWindow<MeritShopWindow> ();
		} else if (gameObj.name == "MyPosition") {
			if (myTeamIndex < 0) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, Language ("Arena70"), "", Language ("Arena71"), null);
				});
				return;
			}
			if (am.tapIndex == myTeamIndex) {
				if (myPoint == null) {
					MaskWindow.UnlockUI ();
					return;
				}
				SpringPanel.Begin (pointRoot, -myPoint.transform.localPosition, 9);
			} else {
				currentLookPositionType = LOOK_POSITION_MY_TYPE;
				tapContent.changeTapPage (tabButtons [myTeamIndex]);
			}
			MaskWindow.UnlockUI ();
		} else if (gameObj.name == "focusCuess") {
			ArenaFocusGuess.FocusPointInfo pointInfo = focusGuess.focusCuessPoint ();
			if (pointInfo == null) {
				if (guessTapIndex != -1) {
					currentLookPositionType = LOOK_POSITION_CUESS_TYPE;
					tapContent.changeTapPage (tabButtons [guessTapIndex]);
				}
			} else {
				if (am.tapIndex == pointInfo.getTapIndex ()) {
					SpringPanel.Begin (pointRoot, -pointInfo.getFocusPoint (), 9);
				} else {
					currentLookPositionType = LOOK_POSITION_CUESS_TYPE;
					focusPointInfo = pointInfo;	
					tapContent.changeTapPage (tabButtons [pointInfo.getTapIndex ()]);
				}
			}
			MaskWindow.UnlockUI ();
		} else if (gameObj.name == "buttonHelp") {
			UiManager.Instance.openDialogWindow<GeneralDesWindow>((win)=>{
				string massTimeStr = ArenaTimeSampleManager.Instance.getMassTimeString(ArenaManager.instance.state,ArenaManager.instance.stateEndTime);
				string finalTimeStr = ArenaTimeSampleManager.Instance.getFinalTimeString(ArenaManager.instance.state,ArenaManager.instance.stateEndTime);
				string rule = LanguageConfigManager.Instance.getLanguage("Arena68",massTimeStr,finalTimeStr);
				win.initialize(rule,LanguageConfigManager.Instance.getLanguage("Arena69"),"");
			});
		} else if (gameObj.name == "next") {
            if (ArenaManager.instance.state == ArenaManager.STATE_RESET) {
                MaskWindow.UnlockUI();
                return;
            }
				
			UiManager.Instance.openDialogWindow<ArenaFinalPreduceTimeWindow>((win)=>{
				win.initUI(ArenaManager.instance.getFinalPreduceDes());
			});
		}
	}
	public void lookChanPosition()
	{
		if (chanPoint != null) {
			StartCoroutine (Utils.DelayRunNextFrame (() => {
				SpringPanel.Begin (pointRoot, -chanPoint.transform.localPosition, 9);
			}));
		}
	}
	public void setArenaManagerTapIndex(int index)
	{
		am.tapIndex = index;
	}
	void OnDestroy ()
	{
		if (cdTimer != null)
			cdTimer.stop ();
		if (checkStateTimer != null)
			checkStateTimer.stop ();
		if(map!=null)
			Destroy (map.gameObject);
		focusGuess = null;
	}
}
