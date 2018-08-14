using UnityEngine;
using System.Collections;
/// <summary>
/// 精灵翻翻乐
/// </summary>
public class HappyTurnSpriteContent:MonoBase
{
	[HideInInspector]
	public WindowBase
		win;//活动窗口
	private Notice notice;
	public SpriteCardCtrl[] ctrls;
	public Transform[] positions;
	/** 重新发牌按钮 */
	public ButtonBase ButtonRestart;
	/** 帮助按钮 */
	public ButtonBase ButtonHelp;
    /** 开始洗牌按钮 */
    public ButtonBase BeginPull;
	/** 剩余次数 */
	public UILabel countLabel;
	private Vector3 startPoint;
	private TurnSpriteData data;
	/** 洗牌后自动播放反转动画 */
	private bool needPlayPullAnimStep1 = false;
    /** 等待玩家点击后播放洗牌动画 */
    private bool needPlayPullAnimStep2 = false;
	private int clickIndex = 0;
	private Timer timer;
	private Timer timer1;
	/** 操作步骤提示 */
	public UILabel doTips;
	/** 翻翻乐title */
	public UISprite textureTitle;
	public UISprite xs_textureTitle;
	/** 限时翻翻乐时间 */
	public UILabel timeText;
	private ActiveTime activeTime;
	public UILabel costLabel;
	public UISprite rmb;
	public UILabel ownRMBLabel;

	private NoticeSample sample;
	private const int MAXTURN = 5;

	/** 活动开启时间 */
	int noticeOpenTime;
	/** 活动结束时间 */
	int noticeCloseTime;
	/** 活动开启倒计时文本 */
	string openTimeNoticeText;
	/** 活动结束倒计时文本 */
	string closeTimeNoticeText;


	private void startTimer ()
	{
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start ();
	}
	private void startTimer1 ()
	{
		if (timer1 == null)
			timer1 = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer1.addOnTimer (refreshNoticeTime);
		timer1.start ();
	}

	private void updateTime ()
	{
		if (getNextRefreshTime () > 0) {
			countLabel.text = LanguageConfigManager.Instance.getLanguage("notice26",data.num.ToString(),data.num_max.ToString(),getNextRefreshTimeString());
		} else {
			timer.stop();
			timer = null;
			countLabel.text = LanguageConfigManager.Instance.getLanguage("notice31",data.num.ToString(),data.num_max.ToString());
			getBaseData(notice.sid);
		}

	}

	private int getNextRefreshTime(){
		int serverTime = ServerTimeKit.getSecondTime ();
		int nextRefreshTime = data.lastTime + data.cd;
		if (nextRefreshTime > serverTime) {
			return nextRefreshTime - serverTime;
		} else {
			return 0;
		}
	}



	private string getNextRefreshTimeString(){
		int time = getNextRefreshTime ();
		System.DateTime date = System.DateTime.Parse ("00:00:00");
		date = date.AddSeconds (time);
		return date.ToString ("hh:mm:ss");
	}

	public void initContent (Notice notice, WindowBase win)
	{
		this.win = win;
		this.notice = notice;
		this.sample = NoticeSampleManager.Instance.getNoticeSampleBySid(this.notice.getSample().sid);
		foreach (SpriteCardCtrl ctrl in ctrls) {
			ctrl.fatherWindow = win;
		}
		getBaseData (notice.sid);
		ButtonRestart.setFatherWindow (win);
		ButtonRestart.onClickEvent = pullSprite;
		ButtonHelp.setFatherWindow (win);
		ButtonHelp.onClickEvent = openHelpWindow;
        BeginPull.setFatherWindow(win);
        BeginPull.onClickEvent = beginPull;

		setNoticeSetting();
	}
	public void setNoticeSetting()
	{
		if(notice.isTimeLimit())//如果是限时活动
		{
			xs_textureTitle.gameObject.SetActive(true);

			this.openTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOpen");
			this.closeTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOver");
			activeTime = ActiveTime.getActiveTimeByID(this.notice.getSample().timeID);
			setNoticeOpenTime();
			refreshNoticeTime();
			startTimer1();
		}
		else
			textureTitle.gameObject.SetActive(true);
	}
	/** 设置活动开启时间 */
	public void setNoticeOpenTime()
	{
		noticeOpenTime = activeTime.getDetailStartTime();
		noticeCloseTime = activeTime.getDetailEndTime();
	}
	/** 刷新活动时间 */
	private void refreshNoticeTime()
	{
		long remainTime = noticeOpenTime - ServerTimeKit.getSecondTime();
		if (remainTime <= 0)
		{
			long remainCloseTime = noticeCloseTime - ServerTimeKit.getSecondTime();
			if (remainCloseTime >= 0)
			{
				timeText.gameObject.SetActive(true);
				timeText.text = sample.activiteDesc +"("+ closeTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainCloseTime))+")";
			}
			else
			{
				timeText.gameObject.SetActive(false);
				timer1.stop();
				timer1 = null;
			}
		}
		/** 还没开启 */
		else
		{
			timeText.gameObject.SetActive(true);
			timeText.text = openTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainTime));
		}
	}

	/// <summary>
	/// 开始洗牌,先翻至反面，然后翻至正面
	/// </summary>
	private IEnumerator beginRuffleStep1 ()
	{
		MaskWindow.LockUI ();
		foreach (SpriteCardCtrl ctrl in ctrls) {
			StartCoroutine (ctrl.turnToBack (null));
		}
		yield return new WaitForSeconds (1f);
		foreach (SpriteCardCtrl ctrl in ctrls) {
			StartCoroutine (ctrl.turnToFront (null));
		}
		yield return new WaitForSeconds (2f); 
        needPlayPullAnimStep1 = false;
        BeginPull.gameObject.SetActive(true);
        MaskWindow.UnlockUI();
		
	}

    private IEnumerator beginRuffleStep2() {
        foreach (SpriteCardCtrl ctrl in ctrls)
        {
            StartCoroutine(ctrl.turnToBack(null));
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < ctrls.Length; i++)
        {
            StartCoroutine(ctrls[i].moveToPosition(Vector3.zero, 0.5f));
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < ctrls.Length; i++)
        {
            StartCoroutine(ctrls[i].moveToPosition(positions[i].position, 0.5f));
        }
        yield return new WaitForSeconds(0.5f);
        needPlayPullAnimStep2 = false;
        BeginPull.gameObject.SetActive(false);
		UpdateDoTips();
        MaskWindow.UnlockUI();

    }



	/// <summary>
	/// 获取数据
	/// </summary>
	private void getBaseData (int sid)
	{
		NoticeGetHappyTurnSpriteFPort port = FPortManager.Instance.getFPort ("NoticeGetHappyTurnSpriteFPort") as NoticeGetHappyTurnSpriteFPort;
		port.access (sid,()=>{
			getBaseDataCallBack(sid);
		});
	}

	/// <summary>
	/// 获取数据回调
	/// </summary>
	private void getBaseDataCallBack (int sid)
	{
		this.needPlayPullAnimStep1 = false;
		if(sid==0)
			this.data = NoticeManagerment.Instance.turnSpriteData;
		else
			this.data = NoticeManagerment.Instance.xs_turnSpriteData;
		updateUI ();
		UpdateDoTips();
	}

	/// <summary>
	/// 更新UI
	/// </summary>
	private void updateUI ()
	{

		if (data.num <= 0) {
			ButtonRestart.disableButton (true);
		} else {
			ButtonRestart.disableButton (false);
		}
		if (getNextRefreshTime () > 0) {
			startTimer ();
		} else {
			countLabel.text = LanguageConfigManager.Instance.getLanguage ("notice31", data.num.ToString (), data.num_max.ToString ());
		}
		if (data.rewardList != null && data.rewardList.Count != 0) {
			for (int i=0; i<ctrls.Length; i++) {
				if (needPlayPullAnimStep1) {
					ctrls[i].turnToBackDirect();
					ctrls [i].init (data.rewardList [i], false);
					ctrls [i].onClickEvent = turnSprite;
					ctrls [i].gameObject.SetActive (true);
				} else {
					TurnSpriteReward reward = getRewardByIndex(i+1);
					/** 已经翻过 */
					if(reward != null){
						ctrls [i].turnToFrontDirect ();
						ctrls [i].init (reward, true);
					}
					/** 没翻过 */
					else{
						ctrls [i].turnToBackDirect ();
						ctrls [i].init (data.rewardList [i], false);
					}
					ctrls [i].onClickEvent = turnSprite;
					ctrls [i].gameObject.SetActive (true);
				}

			}
		}
		if (needPlayPullAnimStep1) {
			StartCoroutine (beginRuffleStep1 ());
		} else {
			MaskWindow.UnlockUI ();
		}
	}
	/// <summary>
	/// 更新步骤提示
	/// </summary>
	private void UpdateDoTips()
	{
		if(needPlayPullAnimStep2)
		{
			doTips.text = LanguageConfigManager.Instance.getLanguage("notice38");
			costLabel.gameObject.SetActive(false);
			ownRMBLabel.gameObject.SetActive(false);
		}
		else if(data.rewardList.Count!=0)
		{
			doTips.text = LanguageConfigManager.Instance.getLanguage("notice39",(data.rewardList.Count-getAwardNum()).ToString(),data.rewardList.Count.ToString());
			if(getTurnCost ()==-1)
			{
				costLabel.gameObject.SetActive(false);
				ownRMBLabel.gameObject.SetActive(false);
			}
			else if(getTurnCost()==0)
			{
				costLabel.gameObject.SetActive(true);
				rmb.gameObject.SetActive(false);
				ownRMBLabel.gameObject.SetActive(false);
				costLabel.text = LanguageConfigManager.Instance.getLanguage ("notice41");
				//costLabel.transform.localPosition =new Vector3(-73.0f,-291.0f,0);
			}
			else
			{
				costLabel.gameObject.SetActive(true);
				rmb.gameObject.SetActive(true);
				ownRMBLabel.gameObject.SetActive(true);
				costLabel.text = LanguageConfigManager.Instance.getLanguage ("notice40",getTurnCost ().ToString());
			}
		}
		 else
		{
			costLabel.gameObject.SetActive(false);
			doTips.text = "";			
		}
		ownRMBLabel.text =LanguageConfigManager.Instance.getLanguage("notice42",UserManager.Instance.self.getRMB().ToString());
	}

	/// <summary>
	/// 根据Index初始化奖励,没有领取过的返回NULL
	/// </summary>
	private TurnSpriteReward getRewardByIndex (int index)
	{
		foreach (TurnSpriteAward award in data.awardList) {
			if (award.index == index){
				foreach(TurnSpriteReward reward in data.rewardList){
					if(reward.num == award.num)
						return reward;
				}
			}
		}
		return null;
	}


	/// <summary>
	/// 洗牌,需要播放洗牌动画
	/// </summary>
	private void pullSprite (GameObject go)
	{

		/** 一张卡都未翻 */
		if (getAwardNum () == 0 && data.rewardList.Count != 0) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("notice27"));
			return ;
		}
        /** 仓库满 */
        if (StorageManagerment.Instance.isRoleStorageFull(1) == true)
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0260"));
            return;
        }
		/** 还有卡未翻 */
		if (getAwardNum () < data.rewardList.Count) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI = false;
				string msg = LanguageConfigManager.Instance.getLanguage ("notice30");
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), msg, (msgEvent) => {
					if (msgEvent.msgEvent == msg_event.dialogOK) {
						sendPullSpriteRequest (notice.sid);
					} else {
						MaskWindow.UnlockUI ();
					}
				});
			});
		} else {
			sendPullSpriteRequest (notice.sid);
		}
	}

	private void sendPullSpriteRequest(int sid){
		NoticePullSpriteFPort port = FPortManager.Instance.getFPort ("NoticePullSpriteFPort") as NoticePullSpriteFPort;
		port.access (sid,()=>{
			pullSpriteCallBack(sid);
		});
	}

	/// <summary>
	/// 洗牌获取数据回调
	/// </summary>
	private void pullSpriteCallBack (int sid)
	{
		needPlayPullAnimStep1 = true;
        needPlayPullAnimStep2 = true;
		if(sid==0)
			this.data = NoticeManagerment.Instance.turnSpriteData;
		else
			this.data = NoticeManagerment.Instance.xs_turnSpriteData;
		updateUI ();
		UpdateDoTips();
	}

	/// <summary>
	/// 翻牌
	/// </summary>
	private void  turnSprite (GameObject go)
	{
		SpriteCardCtrl ctrl = go.GetComponent<SpriteCardCtrl> ();
		clickIndex = ctrl.index;
		int cost = getTurnCost ();
		if (cost != 0) {
			if (cost > UserManager.Instance.self.getRMB ()) {
				rmbNotEnough();
			} else {
				BeginTurnSprite (notice.sid,clickIndex);
				ctrl.updateQuan(false);
			}
		} else {
			BeginTurnSprite (notice.sid,clickIndex);
			ctrl.updateQuan(false);
		}

	}

	private void rmbNotEnough(){
		UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
			string msg = LanguageConfigManager.Instance.getLanguage("s0158");
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0315"),msg, (eventMsg) => {
				if (eventMsg.buttonID == MessageHandle.BUTTON_RIGHT) {
					win.finishWindow ();
					UiManager.Instance.openWindow<rechargeWindow> (); 
				}
			});
		});
	}
	
	private void BeginTurnSprite (int sid,int  index)
	{
		NoticeTurnSpriteFPort port = FPortManager.Instance.getFPort ("NoticeTurnSpriteFPort") as NoticeTurnSpriteFPort;
		port.access (sid,turnSpriteCallBack, index);
	}


	private void turnSpriteCallBack (TurnSpriteReward reward)
	{
		ctrls [clickIndex - 1].init (reward, true);
		UpdateDoTips();
		StartCoroutine (ctrls [clickIndex - 1].turnToFront (() => {
			if(reward.type == "goods")
			{
				PrizeSample sample = new PrizeSample(PrizeType.PRIZE_PROP,reward.sid,reward.num);
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("notice28", sample.getPrizeName(), reward.num.ToString ()));
			}
			if(reward.type == "card")
			{		
				CardSample sample_card = CardSampleManager.Instance.getRoleSampleBySid (reward.sid);
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("notice28", sample_card.name, reward.num.ToString ()));
			}
			MaskWindow.UnlockUI ();
		}));
	}

	/// <summary>
	/// 获取翻牌的消耗
	/// </summary>
	private int getTurnCost ()
	{
		int count = 0;
		foreach (SpriteCardCtrl ctrl in ctrls) {
			if (ctrl.getIsAward ())
				count++;
		}
		if(count>MAXTURN-1) return -1;
		return CommonConfigSampleManager.Instance.getSampleBySid<HappyTurnSpriteCostSample>(CommonConfigSampleManager.HappyTurnSpriteCost_SID).getCostByCount (count);
	}

	/// <summary>
	/// 获取已经翻过的张数
	/// </summary>
	private int getAwardNum(){
		int count = 0;
		foreach (SpriteCardCtrl ctrl in ctrls) {
			if (ctrl.getIsAward ())
				count++;
		}
		return count;
	}

	private void openHelpWindow(GameObject go){
		UiManager.Instance.openDialogWindow<GeneralDesWindow>((win)=>{
			string content = LanguageConfigManager.Instance.getLanguage("notice32");
			string title = LanguageConfigManager.Instance.getLanguage("notice33");
			win.initialize(content,title,"");
		});
	}

	public void OnDisable(){
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
		if (timer1 != null) {
			timer1.stop ();
			timer1 = null;
		}
	}

    public void beginPull(GameObject go) {
        if (!needPlayPullAnimStep2)
            return;
        StartCoroutine(beginRuffleStep2());
    }

}

