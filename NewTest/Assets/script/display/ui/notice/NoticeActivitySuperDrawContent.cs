using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 超级奖池容器
/// </summary>
public class NoticeActivitySuperDrawContent : MonoBase {

	private Notice notice;
	public UILabel timeLabel;
	public WindowBase win;
	private Timer timer1;
	private ActiveTime activeTime;
	private NoticeSample sample;
	private int noticeOpenTime;//活动开启时间 
	private int noticeCloseTime;// 活动结束时间
	private string openTimeNoticeText;//活动开启倒计时文本
	private string closeTimeNoticeText;// 活动结束倒计时文本 
	public ButtonBase drawButton;// 抽奖按钮
	public ButtonBase rechargeButton;// 充值按钮
	public ButtonBase helpButton;// 帮助按钮
	public ButtonBase exchangeButton;// 兑换按钮
	public HpbarCtrl scoreBar;//积分条
	public UILabel lotterySum;//奖券总量文本
	public UILabel myLottery;//我的奖券
	public List<ButtonSuperDraw> prizeList;//所有奖励集
	public List<ButtonSuperDraw> prizeShowList;//需要展示的奖励
	private int index = 0;//当前激活的奖励索引
	public Timer timer;//计时器
	private int cycles = 0;//圈数
	private bool isSpeedDown = false;//是否减速
	private List<PrizeSample> psList;
	private EffectCtrl effectCtrl;
	public UILabel canDrawNumLabel;//可抽奖次数
	private int maxScore = 0;//最大积分
	public UILabel scoreLabel;//积分显示标签
	public HpbarCtrl lotteryScoreBar;
	private bool isEndDraw = false;//是否结束抽奖事件
	private int checkPoint = 0;//选中的位置
	SuperDraw superDraw;
	private bool isDrawing = false;//是否正在抽奖中
	public UITextList audioList;//广播信息
	string  cacheStr;
	public GameObject endTip;
	public GameObject endBottom;

	private Dictionary<int, EffectCtrl> mEffectDic;

	/// <summary>
	/// 初始化容器
	/// </summary>
	public void initContent (Notice notice, WindowBase win)
	{
		this.notice = notice;
		this.win = win;
		initButton();
		startTimer();
		setNoticeOpenTime();
		refreshNoticeTime();
		initDataContent();

	}
	/// <summary>
	/// 初始化抽奖按钮
	/// </summary>
	public void initButton()
	{
		drawButton.fatherWindow = win;
		helpButton.fatherWindow = win;
		rechargeButton.fatherWindow = win;
		exchangeButton.fatherWindow = win;
		drawButton.onClickEvent = doDrawEvent;
		helpButton.onClickEvent = doHelpEvent;
		rechargeButton.onClickEvent = doRechargeEvent;
		exchangeButton.onClickEvent = doExchangeEvent;
	}

	/// <summary>
	/// 初始化界面
	/// </summary>
	public void initDataContent()
	{
		mEffectDic = new Dictionary<int, EffectCtrl>();
		cacheRes ();
	}

	/// <summary>
	/// 预加载缓存
	/// </summary>
	void cacheRes ()
	{
		string[] _list = new string[]{	
			"Effect/UiEffect/battleDrawWindow_StarsDraw"
		};
		ResourcesManager.Instance.cacheData (_list, cacheWindowFinish, "base");
	}
	
	public void cacheWindowFinish (List<ResourcesData> _list)
	{
		init();
		MaskWindow.UnlockUI ();
		
	}
	public void init()
	{
		SuperDrawGetInfoFPort fPort = FPortManager.Instance.getFPort ("SuperDrawGetInfoFPort") as SuperDrawGetInfoFPort;
		fPort.access (notice.sid,loadData);
	}

	/// <summary>
	/// 加载数据
	/// </summary>
	private void loadData ()
	{
		superDraw = SuperDrawManagerment.Instance.superDraw;
		psList = getRandomList ();
		for (int i = 0; i < prizeList.Count; i++) {
			prizeList [i].initInfo (psList [i]);
			prizeList[i].fatherWindow = win;
			if(psList[i].pSid == CommonConfigSampleManager.Instance.getSampleBySid<SuperDrawMaxSample>(CommonConfigSampleManager.SuperDraw_SID).prizeSid)
				prizeList [i].num.text = "x"+prizeList [i].prize.num.ToString()+"%";
			if(psList[i].pSid == 0 && psList[i].type==1)
				prizeList [i].num.text = "x"+((StringKit.toInt(prizeList [i].prize.num)/10000) > 0 ? (StringKit.toInt(prizeList [i].prize.num)/10000):StringKit.toInt(prizeList [i].prize.num))+"W";
		}
		for (int i = 0; i < prizeShowList.Count; i++) {
			prizeShowList [i].clearDate ();
			prizeShowList [i].num.text = "";
			prizeShowList [i].icon.gameObject.SetActive (false);
		}
		List<SuperDrawAudio> list = superDraw.list;
		
		if (list != null)
		{   int count = list.Count;  
			for(int i=count-1;i>0;i--)
			//foreach(SuperDrawAudio c in list)
			{
				onAudio(list[i]);
			}
		}

		loadInfo ();
	}
	/// <summary>
	/// 添加广播
	/// </summary>
	public void onAudio(SuperDrawAudio audio)
	{
		if(audio.serverName=="")return;
		string  str = LanguageConfigManager.Instance.getLanguage("superDraw_12",audio.serverName,audio.playerName,audio.DrawNum.ToString());
		cacheStr = str + "\r\n";//每条自动换行加分隔符
		audioList.Add(cacheStr);
//		SuperDrawManagerment.Instance.superDraw.list.Add (SuperDrawManagerment.Instance.audio);
	}

	/// <summary>
	/// 获得随机列表
	/// </summary>
	private List<PrizeSample> getRandomList ()
	{
		List<PrizeSample> list = SuperDrawSampleManager.Instance.getSuperDrawSampleBySid (superDraw.poolSid).list;
		return list;
	}

	/// <summary>
	/// 加载界面
	/// </summary>
	private void loadInfo ()
	{
		lotterySum.text = "x " + superDraw.poolNum;
		myLottery.text ="x"+getPropSumBySid(SuperDrawManagerment.Instance.propSid);
		canDrawNumLabel.text =LanguageConfigManager.Instance.getLanguage("superDraw_08",superDraw.canUseNum.ToString());
		if(superDraw.canUseNum==0)drawButton.disableButton(true);
		maxScore = CommonConfigSampleManager.Instance.getSampleBySid<SuperDrawMaxSample>(CommonConfigSampleManager.SuperDraw_SID).max;
		scoreLabel.text = superDraw.score+"/"+maxScore;
		//audioLabel.text = LanguageConfigManager.Instance.getLanguage("superDraw_12",superDraw.list[0].serverName,superDraw.list[0].playerName,superDraw.list[0].DrawNum.ToString());
		lotteryScoreBar.updateValue(superDraw.score,maxScore);
	}
	/// <summary>
	/// 获取道具的总量
	/// </summary>
	public int getPropSumBySid(int sid)
	{

		Prop s;
		int num = 0;
		ArrayList list = StorageManagerment.Instance.getPropsBySid(sid);
		for(int j=0;j<list.Count;j++)
		{
			s = list[j] as Prop;
			num = s.getNum();
		}
		return num;

	}

	/// <summary>
	/// 执行抽奖事件
	/// </summary>
	public void doDrawEvent(GameObject obj)
	{
		this.isDrawing = true;
		setDrawbuttonState();
		if (isStorageFulls ()) {
			isDrawing = false;
			return;
		}
		if (!isEndDraw && SuperDrawManagerment.Instance.superDraw.canUseNum>0) {
			clearEffect();
			if(effectCtrl!=null)
				effectCtrl.destoryThis();
			SuperDrawFPort port = FPortManager.Instance.getFPort ("SuperDrawFPort") as SuperDrawFPort;
			port.access (notice.sid, drawOne);
		}
		else
		{
			isDrawing = false;
			drawButton.disableButton(true);
			MaskWindow.UnlockUI ();
		}
	}

	/// <summary>
	/// 执行兑换事件
	/// </summary>
	public void doExchangeEvent(GameObject obj)
	{
		UiManager.Instance.openWindow<SuperDrawShopWindow>((win)=>{
			win.initWindow (notice,getPropSumBySid(SuperDrawManagerment.Instance.propSid),updateMyLottery);
		});
	}
	public void updateMyLottery()
	{
		myLottery.text = "x" + getPropSumBySid(SuperDrawManagerment.Instance.propSid);
		startTimer();
	}
	/// <summary>
	/// 执行帮助事件
	/// </summary>
	public void doHelpEvent(GameObject obj)
	{
		MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("superDraw_16"),MessageAlignType.left);
	}
	/// <summary>
	/// 执行跳转充值窗口事件
	/// </summary>
	public void doRechargeEvent(GameObject obj)
	{
		UiManager.Instance.openWindow<rechargeWindow>();
	}

	/// <summary>
	///检查相关仓库是否满 
	/// </summary>
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
	/// <summary>
	///单次抽奖
	/// </summary>
	private void drawOne (int checkedPoit,int prizeSid,int prizeNum)
	{
		this.checkPoint = checkedPoit-1;
		for (int i = 0; i < prizeList.Count; i++) {
			prizeList [i].clearDate ();
		}
//		if (GameObject.FindObjectOfType<EffectCtrl> () != null) {
//			for (int i = 0; i < GameObject.FindObjectsOfType<EffectCtrl>().Length; i++) {
//				Destroy (GameObject.FindObjectsOfType<EffectCtrl> () [i].gameObject);
//			}
//		}
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (playResultOne);
		timer.start (); 
	}

	/// <summary>
	///播放效果单抽
	/// </summary>
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
		if (cycles == 4&& index == changeIndex ()) {
			isSpeedDown = true;
			timer.reset ();
		}
		if (isSpeedDown) {
			speedDown ();
		} else {
			speedUp ();
		}
	}

	/// <summary>
	/// 移动特效
	/// </summary>
	private void moveEffect (Transform trans)
	{
		if (effectCtrl == null)
			effectCtrl = EffectManager.Instance.CreateEffect (trans, "Effect/UiEffect/battleDrawWindow_StarsDraw");
		effectCtrl.transform.position = trans.position;
	}
	/// <summary>
	/// 清理特效
	/// </summary>
	private void clearEffect()
	{
		foreach (KeyValuePair<int, EffectCtrl> item in mEffectDic)
		{
			prizeList[item.Key].clearDate();
			EffectManager.Instance.removeEffect(item.Value);
		}
		mEffectDic.Clear();
	}
	/// <summary>
	/// 改变选择项
	/// </summary>
	private int changeIndex ()
	{
		int temp = checkPoint - 4;
		if (temp < 0) {
			temp = prizeList.Count + temp;
		}
		return temp;
	}
	
	/// <summary>
	/// 加速
	/// </summary>
	private void speedUp ()
	{
		timer.delayTime -= 300;
		if (timer.delayTime <= 100) {
			timer.delayTime = 1;
		}
	}

	/// <summary>
	/// 减速
	/// </summary>
	private void speedDown ()
	{
		timer.delayTime += 100;
		if (index == checkPoint) {
			StartCoroutine (playEffect (index));
			timer.stop ();
			timer = null;
			
			cycles = 0;
			isSpeedDown = false;
			
			//getAwardEffect (0, prizeList [index].getPrize ());
			string name = QualityManagerment.getQualityColor(prizeList [index].getPrize ().getQuality ()) + prizeList [index].getPrize ().getPrizeName ();
			int sid = CommonConfigSampleManager.Instance.getSampleBySid<SuperDrawMaxSample>(CommonConfigSampleManager.SuperDraw_SID).prizeSid;
			init ();
			UiManager.Instance.createMessageLintWindowNotUnLuck(LanguageConfigManager.Instance.getLanguage("superDraw_18",name,prizeList[index].getPrize().pSid==sid ? (prizeList[index].getPrize().getPrizeNumByInt()+"%"):prizeList[index].getPrize().getPrizeNumByInt().ToString()));
			isDrawing = false;
			setDrawbuttonState();
		}

	}

	public void setDrawbuttonState()
	{
		if(superDraw.canUseNum>0&&!isDrawing)
			drawButton.disableButton(false);
		else 
			drawButton.disableButton(true);
	}
	/// <summary>
	/// 得到奖励特效
	/// </summary>
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
	/// <summary>
	/// 播放特效
	/// </summary>
	private IEnumerator playEffect (int i)
	{
		yield return new WaitForSeconds (0.2f);
		if (mEffectDic.ContainsKey(i)) yield break;
	    effectCtrl = EffectManager.Instance.CreateEffect (prizeList [i].transform,"Effect/UiEffect/Surroundeffect", "Surroundeffect_y");
		effectCtrl.transform.localPosition = Vector3.zero;
		effectCtrl.transform.localScale = new Vector3(1.5f,1.5f,1);
		effectCtrl.transform.parent = prizeList [i].transform;
		mEffectDic.Add(i, effectCtrl);
	}

	/// <summary>
	/// 开启计时器
	/// </summary>
	private void startTimer ()
	{
		if (timer1 == null)
			timer1 = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer1.addOnTimer (refreshNoticeTime);
		timer1.start ();
	}

	/// <summary>
	/// 更新界面
	/// </summary>
	public void updateUI()
	{
		superDraw = SuperDrawManagerment.Instance.superDraw;
		if(superDraw.canUseNum==0)
			drawButton.disableButton(true);
		else
			drawButton.disableButton(false);
		lotterySum.text = "x " + superDraw.poolNum;
		canDrawNumLabel.text = LanguageConfigManager.Instance.getLanguage("superDraw_08",superDraw.canUseNum.ToString());
		maxScore = CommonConfigSampleManager.Instance.getSampleBySid<SuperDrawMaxSample>(CommonConfigSampleManager.SuperDraw_SID).max;
		scoreLabel.text = superDraw.score+"/"+maxScore;
		lotteryScoreBar.updateValue(superDraw.score,maxScore);
		if(SuperDrawManagerment.Instance.audio!=null)
		{
			onAudio(SuperDrawManagerment.Instance.audio);
		}
	}

	/// <summary>
	/// 终止计时器
	/// </summary>
	public void OnDisable ()
	{
		if (timer1 != null) {
			timer1.stop ();
			timer1 = null;
		}
		clearEffect();
	}

	/// <summary>
	/// 设置活动开启时间
	/// </summary>
	public void setNoticeOpenTime()
	{
		this.openTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOpen");
		this.closeTimeNoticeText = LanguageConfigManager.Instance.getLanguage("superDraw_14");
		activeTime = ActiveTime.getActiveTimeByID(this.notice.getSample().timeID);
		noticeOpenTime = activeTime.getDetailStartTime();
		noticeCloseTime = activeTime.getDetailEndTime();
	}

	/// <summary>
	/// 刷新活动时间
	/// </summary>
	private void refreshNoticeTime()
	{
		long remainTime = noticeOpenTime - ServerTimeKit.getSecondTime();
		if (remainTime <= 0)
		{
			long remainCloseTime = noticeCloseTime - ServerTimeKit.getSecondTime();
			if (remainCloseTime >= 0)
			{
				timeLabel.text = closeTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainCloseTime));
			}
			else
			{
				//抽奖时间结束，进入只有兑换的阶段
				timeLabel.gameObject.SetActive(true);
				this.closeTimeNoticeText = LanguageConfigManager.Instance.getLanguage("superDraw_15");
				int[] sids = (notice.getSample().content as SidNoticeContent).sids;//获取商店的时间sid
				activeTime = ActiveTime.getActiveTimeByID(sids[0]);
				int shopCloseTime = activeTime.getDetailEndTime();
				long remainShopClosetime = shopCloseTime - ServerTimeKit.getSecondTime();//获取兑换商店剩余持续时间
				timeLabel.text = closeTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainShopClosetime));

				endBottom.gameObject.SetActive(false);
				endTip.gameObject.SetActive(true);
				drawButton.disableButton(true);
				rechargeButton.gameObject.SetActive(false);

				if(remainShopClosetime<=0)
				{
					timeLabel.gameObject.SetActive(false);
					timer1.stop();
					timer1 = null;
				}
			}
		}
		//还没开启 
		else
		{
			timeLabel.gameObject.SetActive(true);
			timeLabel.text = openTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainTime));
		}
	}
	/// <summary>
	/// 监控是否有广播信息过来
	/// </summary>
	public void Update()
	{
		if(SuperDrawManagerment.Instance.isAudio)
		{
			if(!isDrawing)
			{
				updateUI();
				SuperDrawManagerment.Instance.audio = null;
			}
				
			SuperDrawManagerment.Instance.isAudio = false;

		}
	}
}

