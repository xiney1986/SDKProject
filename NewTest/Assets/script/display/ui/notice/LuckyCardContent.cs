using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 限时卡片抽奖
/// </summary>
public class LuckyCardContent : MonoBase {

	/* show fields */
	/** 卡片视图预制件 */
	public GameObject roleViewPrefab;
	//**goodsview*/
	public GameObject goodsViewPrefab;
	/** 卡片视图点 */
	public GameObject roleViewPoint;
	/** 限时召唤卡片名标签 */
	public UILabel cardName;
	/** 积分标签 */
	public UILabel integralValue;
	/** 排名标签 */
	public UILabel rankValue;
	/** 描述标签 */
	public UILabel descTextLabel;
	/** 抽奖icon1消耗图标 */
	public UISprite icon1Cost;
	/** 抽奖icon2消耗图标 */
	public UISprite icon2Cost;
	/** 查看广播按钮 */
	public ButtonBase buttonFind;
	/** 显示奖励按钮 */
	public ButtonBase showAwardButton;
	/** 显示排名按钮 */
	public ButtonBase rankViewButton;
	/** 免费抽奖button */
	public ButtonBase freeDrawButton;
	/** 免费抽奖button的货币消耗文本 */
	public UILabel freeDrawCostText;
	/** 抽奖button1 */
	public ButtonBase drawButton1;
	/** button1头上的货币消耗文本  */
	public UILabel button1CostText;
	/** 抽奖button2 */
	public ButtonBase drawButton2;
	/** button2头上的货币消耗文本 */
	public UILabel button2CostText;
	/** 抽奖信息描述 */
	public UILabel drawInfoDesc;
	/** 当前图标 */
	public UISprite costIcon;
	/** 当前货币值 */
	public UILabel currentCostValue;
	/** 排行榜容器 */
	public RankContent_1 rankContent;
	/** 剩余活动开启时间文本 */
	public UILabel timeText;
	/* fields */
	/** 活动窗口 */
	WindowBase win;
	/** 抽奖条目对象 */
	LuckyDraw lucky;
	/** 活动 */
    [HideInInspector]
	public LuckyDrawNotice notice;
	/** 当前活动卡片 */
	Card card;
	/** 活动开启时间 */
	int noticeOpenTime;
	/** 活动结束时间 */
	int noticeCloseTime;
	/** 活动积分 */
	int integral;
	/** 活动排名 */
	[HideInInspector] public int rank;
	/**抽奖方式索引 */
	private int drawIndex = -1;
	/** 定时器 */
	private Timer timer;
	/** 激活时间 */
	ActiveTime activeTime;
	/** 活动开启倒计时文本 */
	string openTimeNoticeText;
	/** 活动结束倒计时文本 */
	string closeTimeNoticeText;
    //活动模板
    private NoticeSample sample;
	/** 限时活动标题 */
	public UISprite titleTexture;
	/** 跨服限时活动标题 */
	public UISprite xs_titleTexture;
	private List<RankItemLuckyCard> list;

	/* methods */
	public void initContent (WindowBase win,Notice notice) {
		this.notice = notice as LuckyDrawNotice;
        sample = NoticeSampleManager.Instance.getNoticeSampleBySid(this.notice.sid);
        SidNoticeContent content = sample.content as SidNoticeContent;
		this.lucky=LuckyDrawManagerment.Instance.getLuckyDrawBySid(content.sids[0]);
		this.card=CardManagerment.Instance.createCard(lucky.getSample().ids[0]);
		this.win = win;
		this.openTimeNoticeText=LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOpen");
		this.closeTimeNoticeText=LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOver");
		// 与服务器通讯
        if(sample.activiteDesc == "KUAFU")
		{
			titleTexture.gameObject.SetActive(true);
			(FPortManager.Instance.getFPort ("LuckyCardFPort") as LuckyCardFPort).access (notice.sid,OnLoadDataResault);
		}	    
        else if(sample.activiteDesc == "LOCAL")
		{
			xs_titleTexture.gameObject.SetActive(true);
			(FPortManager.Instance.getFPort("LuckyCardLocalFPort") as LuckyCardLocalFPort).access(notice.sid, OnLoadDataResault);
		}
            
	}

    void OnEnable()
    {
        if(notice!=null)
        {
            sample = NoticeSampleManager.Instance.getNoticeSampleBySid(this.notice.sid);
            // 与服务器通讯
            if (sample.activiteDesc == "KUAFU")
                (FPortManager.Instance.getFPort("LuckyCardFPort") as LuckyCardFPort).access(notice.sid, OnLoadDataResault);
            else if (sample.activiteDesc == "LOCAL")
                (FPortManager.Instance.getFPort("LuckyCardLocalFPort") as LuckyCardLocalFPort).access(notice.sid, OnLoadDataResault);
        }
    }

	/** 初始化button */
	private void initButton() {
		drawButton1.fatherWindow = win;
		drawButton2.fatherWindow = win;
		rankViewButton.fatherWindow = win;
		freeDrawButton.fatherWindow = win;
		buttonFind.fatherWindow = win;
		showAwardButton.fatherWindow = win;
		buttonFind.onClickEvent = HandleShowRadio;
		showAwardButton.onClickEvent = HandleShowAward;
		drawButton1.onClickEvent = HandleDrawButton1;
		drawButton2.onClickEvent = HandleDrawButton2;
		freeDrawButton.onClickEvent = HandleFreeDrawButton;
		rankViewButton.onClickEvent = doViewRankEvent;
	}
	public void doViewRankEvent(GameObject obj)
	{
		UiManager.Instance.openDialogWindow<LuckyRankWindow>((win)=>{
			win.initWindow(RankManagerment.TYPE_LUCKY_CARD,RankManagerment.Instance.luckyCardList,win);
		});
	}

	/// <summary>
	/// 加载排行数据信息后回调 
	/// </summary>
	/// <param name="integral">活动积分</param>
	/// <param name="rank">活动排名</param>
	void OnLoadDataResault(int integral,int rank) {
		this.integral = integral;
		this.rank = rank;
		initButton ();
		UpdateUI ();
		OpenPopIntegralAward ();
	}
	/** 弹出积分奖励 */
	void OpenPopIntegralAward() {
		AwardCache cache = AwardsCacheManager.getAwardCache(AwardManagerment.AWARDS_LUCKY_CARD);
		if (cache != null) {
			Award[] awards=cache.getAwards ();
			cache.clear();
			if(awards!=null&&awards.Length>0) {
				PrizeSample[] prizes = AllAwardViewManagerment.Instance.exchangeAwards(awards);
				bool isOpen=HeroRoadManagerment.Instance.isOpenHeroRoad (prizes);
				if(isOpen)
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("HeroRoad_open"));
				UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{	
					win.Initialize(prizes,LanguageConfigManager.Instance.getLanguage("s0562"));
				});
			}
		}
	}
	/** 设置活动开启时间 */
	public void setNoticeOpenTime () {
		noticeOpenTime = notice.activeTime.getDetailStartTime();
		noticeCloseTime = notice.activeTime.getDetailEndTime();
	}
	/** 刷新活动时间 */
	private void refreshNoticeTime() {
		long remainTime=noticeOpenTime-ServerTimeKit.getSecondTime();
		if (remainTime <= 0) {
			long remainCloseTime=noticeCloseTime-ServerTimeKit.getSecondTime();
			if(remainCloseTime>=0) {
				freeDrawButton.disableButton(false);
				drawButton1.disableButton(false);
				drawButton2.disableButton(false);
				timeText.gameObject.SetActive(true);
				timeText.text=closeTimeNoticeText.Replace ("%1", TimeKit.timeTransformDHMS (remainCloseTime));
			} else {
				freeDrawButton.disableButton(true);
				drawButton1.disableButton(false);
				drawButton2.disableButton(false);
				timeText.gameObject.SetActive(false);
				drawButton1.disableButton(true);
				drawButton2.disableButton(true);
			}
		} else{
			freeDrawButton.disableButton(true);
			drawButton1.disableButton(true);
			drawButton2.disableButton(true);
			timeText.gameObject.SetActive(true);
			timeText.text=openTimeNoticeText.Replace ("%1", TimeKit.timeTransformDHMS (remainTime));
		}
	}

	/** 更新ui */
	public void UpdateUI() {
		if(timer==null) {
			setNoticeOpenTime ();
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
			timer.addOnTimer (refreshNoticeTime);
			timer.start (true);
		} else {
			refreshNoticeTime();
		}
		UpdateRoleView ();
		UpdateLabelText ();
		updateCostIcon ();
		updateCostButton ();
		UpdateButtonFind();
		list = new List<RankItemLuckyCard>();
		if(RankManagerment.Instance.luckyCardList.Count == 0)
			rankViewButton.disableButton(true);
		for(int i=0;i<RankManagerment.Instance.luckyCardList.Count;i++)
		{
			if(i<3)
			{
				list.Add(RankManagerment.Instance.luckyCardList[i]);
			}
		}
		rankContent.init(RankManagerment.TYPE_LUCKY_CARD,list,win);
		rankContent.reLoad(list.Count);
	}
	private void UpdateButtonFind() {
		string[] radioMessage=RadioManager.Instance.getCacheListByType(RadioManager.RADIO_LUCKY_CARD_TYPE);
		if(radioMessage==null||radioMessage.Length==0)
			buttonFind.disableButton(true);
		else
			buttonFind.disableButton(false);
	}
	public void UpdateLabelText() {
		integralValue.text = integral.ToString();
		if(integral==0||rank<=0||rank==0) {
			rankValue.text= LanguageConfigManager.Instance.getLanguage("s0560");
		} else{
			rankValue.text = rank.ToString ();
		}
		string desc = QualityManagerment.getQualityNameByNone (card.getQualityId()) + card.getName ();
		descTextLabel.text = LanguageConfigManager.Instance.getLanguage ("LuckyCardContent_desc",desc);
		cardName.text=QualityManagerment.getQualityColor(card.getQualityId()) + card.getName ();
		updateDrawInfoDesc ();
	}
	/** 更新卡片显示 */
	public void UpdateRoleView() {
		GameObject roleObj;
		if (roleViewPoint.transform.childCount>0) {
			roleObj=roleViewPoint.transform.GetChild(0).gameObject;
		} else {
			roleObj=NGUITools.AddChild (roleViewPoint, roleViewPrefab);
		}
		roleObj.SetActive (true);
		RoleView view = roleObj.GetComponent<RoleView> ();
		view.showType = CardBookWindow.SHOW;
		view.hideInBattle = false;
		view.init (card, win, (roleView)=>{
			CardBookWindow.Show(card,view.showType,null);
		});
	}
	//* 更新Goods显示 */
	public void UpdateGoodsView(){
		GameObject obj;
		if (roleViewPoint.transform.childCount > 0) {
			obj = roleViewPoint.transform.GetChild(0).gameObject;
		}
		else {
			obj = NGUITools.AddChild(roleViewPoint,goodsViewPrefab);
			obj.transform.localScale=new Vector3(0.85f,0.85f,1);
		}
		GoodsView gv = obj.GetComponent<GoodsView>();
		gv.setFatherWindow (this.win);
		//gv.init(starSoul);
	}
	/** 更新花费图标 */
	private void updateCostIcon () {
		LuckyCostIcon.setToolCostIconName(lucky.ways[0], costIcon);
		LuckyCostIcon.setToolCostIconName (lucky.ways [0], icon1Cost);
		LuckyCostIcon.setToolCostIconName (lucky.ways [0], icon2Cost);
	}
	/** 更新当前货币按钮 */
	public void updateCostButton () {
		if (lucky.ways [0].getCostType () == PrizeType.PRIZE_RMB) {
			int num = UserManager.Instance.self.getRMB ();
			currentCostValue.text = "x" + num;
			if (lucky.isFreeDraw ()) {
				freeDrawButton.gameObject.SetActive(true);
				freeDrawButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0026", lucky.getFreeNum ().ToString ());
				drawButton1.gameObject.SetActive(false);
				drawButton2.gameObject.SetActive(false);
				button1CostText.gameObject.SetActive(false);
				button2CostText.gameObject.SetActive(false);
				freeDrawCostText.gameObject.SetActive(true);
				freeDrawCostText.text = "X " + lucky.ways [0].getCostPrice (lucky.getFreeNum ());
			} else {
				freeDrawButton.gameObject.SetActive(false);
				drawButton1.gameObject.SetActive(true);
				drawButton2.gameObject.SetActive(true);
				drawButton1.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [0].getDrawTimes ().ToString ()); 
				drawButton2.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [1].getDrawTimes ().ToString ()); 
				button1CostText.gameObject.SetActive(true);
				button2CostText.gameObject.SetActive(true);
				freeDrawCostText.gameObject.SetActive(false);
				button1CostText.text = "X " + lucky.ways [0].getCostPrice (lucky.getFreeNum ());
				button2CostText.text = "X " + lucky.ways [1].getCostPrice (lucky.getFreeNum ());
			}
		}
	}
	/** 更新抽奖描述信息 */
	private void updateDrawInfoDesc () {
		LuckyDrawSample luckySample = lucky.getSample ();
		string[] luckyPoints = luckySample.luckyPoints;
		//大于0表示要显示
		if (StringKit.toInt (luckyPoints [0]) > 0) {
			//大条目已抽奖总次数（多加一次抽奖信息）
//			int allCount = 1;
//			for (int i=0; i<lucky.ways.Length; i++) {
//				allCount += lucky.ways [i].getNum ();
//			}
//			int quality = 0, count = int.MaxValue;
//			string des = "";
//			int temp;
//			for (int i=1; i<luckyPoints.Length; i += 3) {
//				temp = allCount % StringKit.toInt (luckyPoints [i + 1]);
//				if (temp > 0)
//					temp = StringKit.toInt (luckyPoints [i + 1]) - temp;
//				if (temp > count)
//					continue;
//				if (temp < count) {
//					count = temp;
//					quality = StringKit.toInt (luckyPoints [i]);
//					des = luckyPoints [i + 2];
//				} else if (StringKit.toInt (luckyPoints [i]) > quality) {
//					quality = StringKit.toInt (luckyPoints [i]);
//					des = luckyPoints [i + 2];
//				}
//			}
//			string str = "";
//			//本次不是必出
//			if (count > 0) {
//				str = LanguageConfigManager.Instance.getLanguage ("luckdraw11", count.ToString (), des);
//			} else {
//				str = LanguageConfigManager.Instance.getLanguage ("luckdraw12", des);
//			}
//			drawInfoDesc.text = str;

			int allCount = 1;
			for (int i=0; i<lucky.ways.Length; i++) {
				allCount += lucky.ways [i].getNum ();
			}
			
			int quality = 0, count = int.MaxValue;
			string des = "";
			int temp;
			for (int i=1; i<luckyPoints.Length; i += 3) {
				temp = allCount % StringKit.toInt (luckyPoints [i + 1]);
				if (temp > 0)
					temp = StringKit.toInt (luckyPoints [i + 1]) - temp;
				if (temp > count)
					continue;
				if (temp < count) {
					count = temp;
					quality = StringKit.toInt (luckyPoints [i]);
					des = luckyPoints [i + 2];
				} else if (StringKit.toInt (luckyPoints [i]) > quality) {
					quality = StringKit.toInt (luckyPoints [i]);
					des = luckyPoints [i + 2];
				}
			}
			count += 1;
			string str = "";
			//本次不是必出
			if (count > 1) {
				str = LanguageConfigManager.Instance.getLanguage ("luckdraw11", count.ToString (), des);
			} else {
				str = LanguageConfigManager.Instance.getLanguage ("luckdraw12", des);
			}
			drawInfoDesc.text = str;
		} else {
			drawInfoDesc.text = lucky.getDescribe ();
		}
	}
	/** 显示广播消息 */
	private void HandleShowRadio(GameObject gameObj) {
		UiManager.Instance.openDialogWindow<LuckyRadioWindow> ((win)=>{
			win.initUI(RadioManager.Instance.getCacheListByType(RadioManager.RADIO_LUCKY_CARD_TYPE));
		});
	}
	/** 显示奖励信息 */
	private void HandleShowAward(GameObject gameObj) {
		UiManager.Instance.openWindow<LucklyActivityAwardWindow>((win)=>{
			win.init(notice.sid,integral,1);
		});
	}
	/** 单次抽奖 */
	private void HandleDrawButton1(GameObject gameObj) {
		if (isStorageFulls ()) {
			return;
		}
		if (!isDrawCost (0)) {
			string str = getMSG (0);
			if (lucky.ways [0].getCostType () == PrizeType.PRIZE_RMB)
				MessageWindow.ShowRecharge (str);
			else
				MessageWindow.ShowAlert (str);
			return;
		}
		drawIndex = 0;
		LuckyDrawFPort port = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
		port.luckyDrawByNotice (1, lucky.sid, 1, notice.sid, lucky.ways [drawIndex], callbackAfterEffect);
	}
	/** N单次抽奖 */
	private void HandleDrawButton2(GameObject gameObj) {
		if (isStorageFulls ()) {
			return;
		}
		if (!isDrawCost (1)) {
			string str = getMSG (1);
			if (lucky.ways [1].getCostType () == PrizeType.PRIZE_RMB)
				MessageWindow.ShowRecharge (str);
			else
				MessageWindow.ShowAlert (str);
			return;
		}
		LuckyDrawFPort ports = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
		drawIndex = 1;
		ports.luckyDrawByNotice (10, lucky.sid, 2, notice.sid, lucky.ways [drawIndex], callbackAfterEffect);
	}
	/** 免费抽奖 */
	private void HandleFreeDrawButton(GameObject gameObj) {
		if (isStorageFulls ()) {
			return;
		}
		if (!isDrawCost (0)) {
			string str = getMSG (1);
			if (lucky.ways [1].getCostType () == PrizeType.PRIZE_RMB)
				MessageWindow.ShowRecharge (str);
			else
				MessageWindow.ShowAlert (str);
			return;
		}
		drawIndex = 0;
		LuckyDrawFPort port = FPortManager.Instance.getFPort ("LuckyDrawFPort") as LuckyDrawFPort;
		port.luckyDrawByNotice (1, lucky.sid, 1, notice.sid, lucky.ways [drawIndex], callbackAfterEffect);
	}
	/** 抽奖特效完成后 */
	void callbackAfterEffect (LuckyDrawResults results) {
		lucky.drawLucky (drawIndex);
		UiManager.Instance.openWindow<LuckyDrawShowWindow> ((windown) => {
			windown.init (results, lucky,LuckyDrawShowWindow.SKIP_NOTICE_TYPE,notice.sid); 
		});
	}
	/** 获取无法抽奖的提示信息 */
	private string getMSG (int index) {
		string str = string.Empty;
		switch (lucky.ways [index].getCostType ()) {
		case PrizeType.PRIZE_RMB:
			str = LanguageConfigManager.Instance.getLanguage ("s0048");
			break;
		case PrizeType.PRIZE_MONEY:
			str = LanguageConfigManager.Instance.getLanguage ("s0049");
			break;
		case PrizeType.PRIZE_PROP:
			str = PropSampleManager.Instance.getPropSampleBySid (lucky.ways [index].getCostToolSid ()).name;
			break;
		}
		return str + LanguageConfigManager.Instance.getLanguage ("s0053");
	}
	/** 是否能够抽奖 */
	private bool isDrawCost (int index) {
		if (lucky.getFreeNum () > 0)
			return true;
		if (lucky.ways [index].getCostType () == PrizeType.PRIZE_RMB) {
			int rmb = lucky.ways [index].getCostPrice (lucky.getFreeNum ());
			return rmb <= UserManager.Instance.self.getRMB ();
		} else if (lucky.ways [index].getCostType () == PrizeType.PRIZE_MONEY) {
			int money = lucky.ways [index].getCostPrice (lucky.getFreeNum ());
			return  money <= UserManager.Instance.self.getMoney ();
		} else if (lucky.ways [index].getCostType () == PrizeType.PRIZE_PROP) {  
			return StorageManagerment.Instance.checkProp (lucky.ways [index].getCostToolSid (), lucky.ways [index].getCostPrice (lucky.getFreeNum ()));
		} 
		return false;
	} 
	/** 仓库是否已满 */
	private bool isStorageFulls () {
		//忽略除了卡片之外的抽奖获得
		if (StorageManagerment.Instance.isRoleStorageFull (1)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0172"));
			return true;
		} else if (StorageManagerment.Instance.isEquipStorageFull (1)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("luckdraw14"));
			return true;
		} else if (StorageManagerment.Instance.isTempStorageFull (20)) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("luckdraw10")
				                , LanguageConfigManager.Instance.getLanguage ("s0173"), gotoTempMail); 
			});
			return true;
		}
		return false;
	}
	//临时仓库已满，去领取再来
	private void gotoTempMail (MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			return;
		}
		UiManager.Instance.openWindow<MailWindow> ((win) => {
			win.Initialize (1);
		});
	}
	/** 消耗 */
	void OnDestroy(){
		clear ();
	}
	/** 清理 */
	private void clear () {
		if (timer != null)
			timer.stop ();
		timer = null;
		RadioManager.Instance.clearByType (RadioManager.RADIO_LUCKY_CARD_TYPE);
		RankManagerment.Instance.luckyCardList.Clear();
	}
}
