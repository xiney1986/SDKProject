using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 限时装备抽奖
/// </summary>
public class LuckyEquipContent : MonoBase {

	/* show fields */
	/** 积分标签 */
	public UILabel integralValue;
	/** 排名标签 */
	public UILabel rankValue;
	/** 抽奖icon1消耗图标 */
	public UISprite icon1Cost;
	/** 抽奖icon2消耗图标 */
	public UISprite icon2Cost;
	/** 查看广播按钮 */
	//public ButtonBase buttonFind;
	/** 显示奖励按钮 */
	public ButtonBase showAwardButton;
	/** 显示奖励按钮 */
	public ButtonBase showDetailButton;
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
	/** 描述文本 */
	public UILabel descText;
	//*展示物品*/
	public GoodsView showGoods;
	//*前往按钮*/
	public ButtonBase gotoButton;
	//**抽装备title*/
	public GameObject equipTitle;
	//**抽道具title*/
	public UISprite propTitle;
	//**广播*/
	public GameObject showRadio;
	//**广播类型*/
	public RadioCtrl radioCtrl;
	/* fields */
	/** 活动窗口 */
	NoticeWindow win;
	/** 抽奖条目对象 */
	LuckyDraw lucky;
	/** 活动 */
	LuckyDrawNotice notice;
	/** 活动开启时间 */
	int noticeOpenTime;
	/** 活动关闭时间 */
	int noticeCloseTime;
	/** 活动积分 */
	int integral;
	/** 活动排名 */
	[HideInInspector]
	public int rank;
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
	/** 限时炼金标题 */
	public GameObject xs_lianjin;
	/** 限时猎魂标题 */
	public GameObject xs_liehun;
	/** 显示排名按钮 */
	public ButtonBase rankViewButton;

	//*展示奖励*/
	private Equip showEquip;
	private Prop showProp;
	private StarSoul showSoul;
	private LuckyDrawSample luckySample;
	public static int needReload = 0;
	/* methods */
	public void initContent (NoticeWindow win, Notice notice) {
		this.notice = notice as LuckyDrawNotice;
		SidNoticeContent content = notice.getSample ().content as SidNoticeContent;
		this.lucky = LuckyDrawManagerment.Instance.getLuckyDrawBySid (content.sids [0]);
		this.luckySample = lucky.getSample ();
		this.win = win;
		this.openTimeNoticeText = LanguageConfigManager.Instance.getLanguage ("s0503");
		this.closeTimeNoticeText = LanguageConfigManager.Instance.getLanguage ("s0570");
		if (this.luckySample.idsType == "2") {  //限时抽装备
			showEquip = EquipManagerment.Instance.createEquip (luckySample.ids [0]);
			showGoods.setFatherWindow (this.win);
			showGoods.init (showEquip);
			drawButton1.gameObject.SetActive (true);
			drawButton2.gameObject.SetActive (true);
			gotoButton.gameObject.SetActive (false);
			equipTitle.SetActive (true);
			drawInfoDesc.gameObject.SetActive(true);
			radioCtrl.radioType = 2;
		}
		else {
			StarSoulSample tmpSample = StarSoulSampleManager.Instance.getStarSoulSampleBySid(luckySample.ids[0]);
			if(tmpSample != null){
				showSoul = StarSoulManager.Instance.createStarSoul(luckySample.ids [0]);
				showGoods.init(showSoul);
			}
			else{
				showProp = PropManagerment.Instance.createProp (luckySample.ids [0]);
				showGoods.init (showProp);
			}
			showGoods.setFatherWindow (this.win);
            setNoticeOpenTime();
			gotoButton.gameObject.SetActive (true);
            gotoButton.disableButton(noticeOpenTime - ServerTimeKit.getSecondTime() > 0);
			drawButton1.gameObject.SetActive (false);
			drawButton2.gameObject.SetActive (false);
			propTitle.gameObject.SetActive (true);
			drawInfoDesc.gameObject.SetActive(false);
			if (luckySample.idsType == "3"){   //luckysample isdType 3 猎魂 4 炼金
				radioCtrl.radioType = 3;
				if(luckySample.name.EndsWith("1")){
                    propTitle.spriteName = "kf_xs_xinghun";
                    propTitle.gameObject.SetActive(true);
                    xs_liehun.gameObject.SetActive(false);
                }
				else
				{
					propTitle.gameObject.SetActive(false);
					xs_liehun.gameObject.SetActive(true);
				}	
				gotoButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("StarSoulWindow_Hunt_go");
			}
			if (luckySample.idsType == "4"){
				showRadio.SetActive(false);
				if(luckySample.name.EndsWith("1"))
				{
					propTitle.spriteName = "kf_xs_lianjin";
                    propTitle.gameObject.SetActive(true);
                    xs_lianjin.gameObject.SetActive(false);
				}
				else
				{
					propTitle.gameObject.SetActive(false);
					xs_lianjin.gameObject.SetActive(true);
				}
				gotoButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("go_get_money");
			}
		}
		// 与服务器通讯
		if(luckySample.idsType == "2")
			(FPortManager.Instance.getFPort ("LuckyEquipFPort") as LuckyEquipFPort).access (notice.sid, OnLoadDataResault);
		else
			(FPortManager.Instance.getFPort ("LuckyXianshiFPort") as LuckyXianshiFPort).access (notice.sid, OnLoadDataResault,StringKit.toInt(luckySample.idsType));

	}
	/** 初始化button */
	private void initButton () {
		drawButton1.fatherWindow = win;
		drawButton2.fatherWindow = win;
		freeDrawButton.fatherWindow = win;
		//buttonFind.fatherWindow = win;
		showAwardButton.fatherWindow = win;
		showDetailButton.fatherWindow = win;
		gotoButton.fatherWindow = win;
		rankViewButton.fatherWindow = win;
		//buttonFind.onClickEvent = HandleShowRadio;
		showDetailButton.onClickEvent = HandleShowRadio;
		showAwardButton.onClickEvent = HandleShowAward;
		drawButton1.onClickEvent = HandleDrawButton1;
		drawButton2.onClickEvent = HandleDrawButton2;
		freeDrawButton.onClickEvent = HandleFreeDrawButton;
		gotoButton.onClickEvent = HandleGotoButton;
		rankViewButton.onClickEvent = doViewRankEvent;
	}
	public void doViewRankEvent(GameObject obj)
	{
		if(luckySample.idsType == "2")
			UiManager.Instance.openDialogWindow<LuckyRankWindow>((win)=>{
				win.initWindow(RankManagerment.TYPE_LUCKY_EQUIP,RankManagerment.Instance.luckyEquipList,win);
			});
		else if(luckySample.idsType == "3")
			UiManager.Instance.openDialogWindow<LuckyRankWindow>((win)=>{
				win.initWindow(RankManagerment.TYPE_LUCKY_LIEHUN,RankManagerment.Instance.luckyLiehunList,win);
			});
		else if(luckySample.idsType == "4")
			UiManager.Instance.openDialogWindow<LuckyRankWindow>((win)=>{
				win.initWindow(RankManagerment.TYPE_LUCKY_LIANJIN,RankManagerment.Instance.luckyLianjinList,win);
			});
	}
	/// <summary>
	/// 加载排行数据信息后回调 
	/// </summary>
	/// <param name="integral">活动积分</param>
	/// <param name="rank">活动排名</param>
	void OnLoadDataResault (int integral, int rank) {
		this.integral = integral;
		this.rank = rank;
		initButton ();
		UpdateUI ();
		OpenPopIntegralAward ();
	}
	/** 弹出积分奖励 */
	void OpenPopIntegralAward () {
		AwardCache cache = null;
		if (luckySample.idsType == "2")
			cache = AwardsCacheManager.getAwardCache (AwardManagerment.AWARDS_LUCKY_CARD);
		else 
			return;
		if (cache != null) {
			Award[] awards = cache.getAwards ();
			cache.clear ();
			if (awards != null && awards.Length > 0) {
				UiManager.Instance.openDialogWindow<AllAwardViewWindow> ((win) => {	
					win.Initialize (awards, LanguageConfigManager.Instance.getLanguage ("s0562"));
				});
			}
		}
	}
	/** 设置活动开启时间 */
	public void setNoticeOpenTime () {
		noticeOpenTime = notice.activeTime.getDetailStartTime ();
		noticeCloseTime = notice.activeTime.getDetailEndTime ();
	}
	/** 刷新活动时间 */
	private void refreshNoticeTime () {
		long remainTime = noticeOpenTime - ServerTimeKit.getSecondTime ();
		if (remainTime <= 0) {
			long remainCloseTime = noticeCloseTime - ServerTimeKit.getSecondTime ();
			if (remainCloseTime >= 0) {
				if(luckySample.idsType == "2"){
				freeDrawButton.disableButton (false);
				drawButton1.disableButton (false);
				drawButton2.disableButton (false);
				}
				timeText.gameObject.SetActive (true);
				timeText.text = closeTimeNoticeText.Replace ("%1", TimeKit.timeTransformDHMS (remainCloseTime));
			}
			else {
				if(luckySample.idsType == "2"){
				freeDrawButton.disableButton (true);
				drawButton1.disableButton (true);
				drawButton2.disableButton (true);
				}
				timeText.gameObject.SetActive (false);
			}
		}
		else {
			if(luckySample.idsType == "2"){
			freeDrawButton.disableButton (true);
			drawButton1.disableButton (true);
			drawButton2.disableButton (true);
			}
			timeText.gameObject.SetActive (true);
			timeText.text = openTimeNoticeText.Replace ("%1", TimeKit.timeTransformDHMS (remainTime));
            gotoButton.disableButton(remainTime > 0);
		}
	}
	/** 更新ui */
	public void UpdateUI () {
		if (timer == null) {
			setNoticeOpenTime ();
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
			timer.addOnTimer (refreshNoticeTime);
			timer.start (true);
		}
		else {
			refreshNoticeTime ();
		}
		UpdateLabelText ();
		updateCostIcon ();
		updateCostButton ();
		UpdateButtonFind ();

		if(luckySample.idsType == "2")
		{
			List<RankItemLuckyEquip> list = new List<RankItemLuckyEquip>();
			if(RankManagerment.Instance.luckyEquipList.Count == 0)
				rankViewButton.disableButton(true);
			for(int i=0;i<RankManagerment.Instance.luckyEquipList.Count;i++)
			{
				if(i<3)
				{
					list.Add(RankManagerment.Instance.luckyEquipList[i]);
				}
			}
			rankContent.init (RankManagerment.TYPE_LUCKY_EQUIP, list, win);
			rankContent.reLoad(list.Count);
		}
			
		else if(luckySample.idsType == "3")
		{
			List<RankItemLuckyLiehun> list = new List<RankItemLuckyLiehun>();
			if(RankManagerment.Instance.luckyLiehunList.Count == 0)
				rankViewButton.disableButton(true);
			for(int i=0;i<RankManagerment.Instance.luckyLiehunList.Count;i++)
			{
				if(i<3)
				{
					list.Add(RankManagerment.Instance.luckyLiehunList[i]);
				}
			}
			rankContent.init (RankManagerment.TYPE_LUCKY_LIEHUN, list, win);
			rankContent.reLoad(list.Count);
		}
			
		else if(luckySample.idsType == "4")
		{
			List<RankItemLuckyLianjin> list = new List<RankItemLuckyLianjin>();
			if(RankManagerment.Instance.luckyLianjinList.Count == 0)
				rankViewButton.disableButton(true);
			for(int i=0;i<RankManagerment.Instance.luckyLianjinList.Count;i++)
			{
				if(i<3)
				{
					list.Add(RankManagerment.Instance.luckyLianjinList[i]);
				}
			}
			rankContent.init (RankManagerment.TYPE_LUCKY_LIANJIN, list, win);
			rankContent.reLoad(list.Count);
		}
			
	}
	private void UpdateButtonFind () {
		string[] radioMessage = null;
		if(luckySample.idsType == "2")
			radioMessage = RadioManager.Instance.getCacheListByType (RadioManager.RADIO_LUCKY_EQUIP_TYPE);
		else if(luckySample.idsType == "3")
			radioMessage = RadioManager.Instance.getCacheListByType (RadioManager.RADIO_LUCKY_STARSOUL_TYPE);
		if (radioMessage == null || radioMessage.Length == 0)
			showDetailButton.disableButton (true);
		else
			showDetailButton.disableButton (false);
	}
	private void UpdateLabelText () {
		integralValue.text = integral.ToString ();
		if (integral == 0||rank < 0||rank==0) {
			rankValue.text = LanguageConfigManager.Instance.getLanguage ("s0560");
		}
		else {
			rankValue.text = rank.ToString ();
		}
		updateDrawInfoDesc ();
		updateDescText ();
	}
	/** 更新描述 */
	private void updateDescText () {
		int minIntegral = 0;
		RankAward sourceAward = LucklyActivityAwardConfigManager.Instance.getFirstSource (notice.sid);
		if (sourceAward != null) {
			minIntegral = sourceAward.needSource;
		}
		if (luckySample.idsType == "2")
			descText.text = LanguageConfigManager.Instance.getLanguage ("luckyEquipContent_desc", Convert.ToString (minIntegral));
		else if (luckySample.idsType == "3" || luckySample.idsType == "4")
			descText.text = luckySample.luckyPoints [0].Replace('~','\n');
	}
	/** 更新花费图标 */
	private void updateCostIcon () {
		LuckyCostIcon.setToolCostIconName (lucky.ways [0], costIcon);
		LuckyCostIcon.setToolCostIconName (lucky.ways [0], icon1Cost);
		LuckyCostIcon.setToolCostIconName (lucky.ways [0], icon2Cost);
	}
	/** 更新当前货币按钮 */
	private void updateCostButton () {
		if (lucky.ways [0].getCostType () == PrizeType.PRIZE_RMB && luckySample.idsType == "2") {
			int num = UserManager.Instance.self.getRMB ();
			currentCostValue.text = "x" + num;
			if (lucky.isFreeDraw ()) {
				freeDrawButton.gameObject.SetActive (true);
				freeDrawButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0026", lucky.getFreeNum ().ToString ());
				drawButton1.gameObject.SetActive (false);
				drawButton2.gameObject.SetActive (false);
				button1CostText.gameObject.SetActive (false);
				button2CostText.gameObject.SetActive (false);
				freeDrawCostText.gameObject.SetActive (true);
				freeDrawCostText.text = "X " + lucky.ways [0].getCostPrice (lucky.getFreeNum ());
			}
			else {
				freeDrawButton.gameObject.SetActive (false);
				drawButton1.gameObject.SetActive (true);
				drawButton2.gameObject.SetActive (true);
				drawButton1.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [0].getDrawTimes ().ToString ()); 
				drawButton2.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s00026", lucky.ways [1].getDrawTimes ().ToString ()); 
				button1CostText.gameObject.SetActive (true);
				button2CostText.gameObject.SetActive (true);
				freeDrawCostText.gameObject.SetActive (false);
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
				}
				else if (StringKit.toInt (luckyPoints [i]) > quality) {
					quality = StringKit.toInt (luckyPoints [i]);
					des = luckyPoints [i + 2];
				}
			}
			string str = "";
			//本次不是必出
			if (count > 0) {
				str = LanguageConfigManager.Instance.getLanguage ("luckdraw11", count.ToString (), des);
			}
			else {
				str = LanguageConfigManager.Instance.getLanguage ("luckdraw12", des);
			}
			drawInfoDesc.text = str;
		}
		else {
			drawInfoDesc.text = lucky.getDescribe ();
		}
	}
	/** 显示广播消息 */
	private void HandleShowRadio (GameObject gameObj) {
		UiManager.Instance.openDialogWindow<LuckyRadioWindow> ((win) => {
			if(luckySample.idsType == "2")
				win.initUI (RadioManager.Instance.getCacheListByType (RadioManager.RADIO_LUCKY_EQUIP_TYPE));
			else if(luckySample.idsType == "3")
				win.initUI (RadioManager.Instance.getCacheListByType (RadioManager.RADIO_LUCKY_STARSOUL_TYPE));
		});
	}
	/** 显示奖励信息 */
	private void HandleShowAward (GameObject gameObj) {
		UiManager.Instance.openWindow<LucklyActivityAwardWindow> ((win) => {
			win.init (notice.sid, integral,StringKit.toInt(luckySample.idsType));
			win.descInfo = luckySample.luckyPoints[1].Replace('~','\n');
		});
	}
	/** 单次抽奖 */
	private void HandleDrawButton1 (GameObject gameObj) {
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
	private void HandleDrawButton2 (GameObject gameObj) {
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
	private void HandleFreeDrawButton (GameObject gameObj) {
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
	//*前往*/
	private void HandleGotoButton (GameObject obj) {
		needReload = 1;
		if (obj.name == "gotoButton") {
			if (luckySample.idsType == "3") {
//				UiManager.Instance.openWindow<StarSoulWindow> ((win) => {
//					win.init (1);
//					win.integral = integral;
//					win.hountSid = notice.sid;
//				}); 

				UiManager.Instance.openWindow<SoulHuntWindow> ((win) => {
					win.init (1);
					win.integral = integral;
					win.hountSid = notice.sid;
					win.rank = rank;
				}); 
			}
			if (luckySample.idsType == "4") {
				win.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.ALCHEMY_SID).entranceId;
				win.updateSelectButton (NoticeType.ALCHEMY_SID);
				win.initTopButton ();
				MaskWindow.UnlockUI ();
			}
		}
	}
	/** 抽奖特效完成后 */
	void callbackAfterEffect (LuckyDrawResults results) {
		lucky.drawLucky (drawIndex);
		UiManager.Instance.switchWindow<LuckyDrawShowWindow> ((windown) => {
			windown.init (results, lucky, LuckyDrawShowWindow.SKIP_NOTICE_TYPE, notice.sid); 
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
		}
		else if (lucky.ways [index].getCostType () == PrizeType.PRIZE_MONEY) {
			int money = lucky.ways [index].getCostPrice (lucky.getFreeNum ());
			return  money <= UserManager.Instance.self.getMoney ();
		}
		else if (lucky.ways [index].getCostType () == PrizeType.PRIZE_PROP) {  
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
		}
		else if (StorageManagerment.Instance.isEquipStorageFull (1)) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("luckdraw14"));
			return true;
		}
		else if (StorageManagerment.Instance.isTempStorageFull (20)) {
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
	void OnDestroy () {
		clear ();
	}
	/** 清理 */
	private void clear () {
		if (timer != null)
			timer.stop ();
		timer = null;
		if (luckySample != null && luckySample.idsType == "2") {
			RadioManager.Instance.clearByType (RadioManager.RADIO_LUCKY_EQUIP_TYPE);
			RankManagerment.Instance.luckyEquipList.Clear ();
		}else if (luckySample != null && luckySample.idsType == "3") {
			RadioManager.Instance.clearByType (RadioManager.RADIO_LUCKY_STARSOUL_TYPE);
			RankManagerment.Instance.luckyLiehunList.Clear ();
		}
	}
	void Update(){
		if (needReload == 2) {
			initContent(this.win,this.notice);
			needReload = 0;
		}
	}
}
