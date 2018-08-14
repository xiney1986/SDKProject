using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 * 公告窗口
 * @author 汤琦
 * */
public class NoticeWindow : WindowBase
{

	public UISprite left, right; //箭头
	public Transform showPos;//具体活动显示位置
	public GameObject content;//活动按钮显示容器
	public GameObject topButton;//活动按钮预设
	public GameObject topButtons;//滚动顶级
	[HideInInspector]
	public GameObject show;
	public GameObject rechargeShow;
	public GameObject exchangeShow;
	public GameObject heroEatShow;
	public GameObject dailyReateShow;
    public GameObject oneManyShow; //单笔充值多次领取
	public GameObject festivalShow;//节日许愿
	public GameObject fireworksShow;//节日礼花
	public GameObject superDrawShow;//超级奖池
	/** 限时卡片抽奖显示点 */
	public GameObject luckyCardShow;
	/** 限时装备抽奖显示点 */
	public GameObject luckyEquipShow;
	/** 装备提升显示点 */
	public GameObject equipRemakeShow;
	public GameObject alchemyShow;
	public GameObject inviteShow;
	public GameObject monthCardShow;
	public GameObject happyTurnSpriteShow;
	public GameObject oneRmbShow;
	public GameObject quizShow;
	public GameObject happySunday;
	public GameObject doubleRMB;
	public GameObject consumeRebateShow;
	public GameObject ladderHegemoney;
	public GameObject limitCollect;
    public GameObject signInShow;
    public GameObject shareContent;
	public Texture rechargeUI;
	public Texture timeRechargeUI;
	public Texture totalCostUI;
	public Texture exchangeUI;
	public Texture heroEatUI;
	public Texture alchemyUI;
	public Texture boxCodeUI;
	public Texture monthCardUI;
	public Texture happyTurnSpriteUI;
	public Texture oneRmbUI;
	public Texture quizbUI;
	public Texture happySumdayUI;
	public Texture doubleRMBUI;
	public Texture consumeRebateUI;
	public Texture luckyCardUI;
	public Texture remakeEquipUI;
	public Texture xianshiUI;
	public Texture superDrawUI;
	public Texture festivalWishUI;
	public Texture festivalFireworksUI;
    public Texture signInUI;
    public Texture shareDrawUI;
	public Texture fudaiUI;
	public Texture weekCardUI;
	public Texture lotteryUI;
	private NoticeTopButton lastSelect;//最后选择的按钮
	private Timer timer;
	int firstBootIndex = -1;
	private int defaultSelectSid = -1;
	private Hashtable tabButtons;
	private ArrayList dailyRateList;
	private bool flag;
	private int indexxx=6;
	public int entranceId = 1; //默认进入日常活动进入活动条目 index

	public GameObject rebateShow;//福袋返利
	public GameObject backPrizeShow;// 老玩家回归//
	public GameObject backRechargeShow;// 老玩家回归充值//
	public GameObject weekCardShow;// 周卡//
	public GameObject lotteryShow;// 大乐透//

	public override void OnAwake ()
	{
		base.OnAwake ();
		UiManager.Instance.noticeWindow = this;
	}

	public override void OnStart ()
	{
		startTimer ();
	}

	public void firstBoot (int index)
	{	
		firstBootIndex = index;
	}

	public void firstBootBySid (int sid)
	{
		firstBootIndex = 0;//在关闭窗口时 会依此值做判断
		defaultSelectSid = sid;
	}
	/** 获取当前选择的活动sid */
	public int getDefaultSelectSid ()
	{
		return this.defaultSelectSid;
	}



	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide) {
			if (firstBootIndex > 0)
				initTopButton (true, firstBootIndex,entranceId);
			else
				initTopButton ();
		} else {
			if (lastSelect != null) {
				Notice _notice = lastSelect.getNotice ();
				int type = _notice.getSample ().type;
				doSwitchBackGround (type);
				if(lastSelect.getNotice().getSample().type == NoticeType.BACK_RECHARGE)// 当返回的是回归充值界面//
				{
					lastSelect.updateTime();
					showDetail(lastSelect);
				}
			}
		}
		MaskWindow.UnlockUI ();
		GuideManager.Instance.guideEvent ();
	}
	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		if (lastSelect == null) {
			return;
		}
		updateDefaulSelect (lastSelect.getNotice ().sid);
	}

    protected override void DoEnable()
    {
        base.DoEnable();
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();
        if (Time.frameCount % 50 == 0)
            updateTime();
		if(flag){
			if(topButtons.transform.localPosition.x<-120)left.gameObject.SetActive(true);
			else left.gameObject.SetActive(false);
			if(topButtons.transform.localPosition.x<-(indexxx-6)*110)right.gameObject.SetActive(false);
			else right.gameObject.SetActive(true);
		}

		if(NoticeManagerment.Instance.loginTime == 0)
		{
			NoticeManagerment.Instance.loginTime = ServerTimeKit.getLoginTime();
		}
		if(ServerTimeKit.getMillisTime() >= BackPrizeLoginInfo.Instance.getSecondDayTime(NoticeManagerment.Instance.loginTime))// 跨天//
		{
			NoticeManagerment.Instance.loginTime = ServerTimeKit.getMillisTime();
			if(entranceId == NoticeEntranceType.BACK_NOTICE)// 回归//
			{
				BackPrizeInfoFPort bpif = FPortManager.Instance.getFPort ("BackPrizeInfoFPort") as BackPrizeInfoFPort;
				bpif.BackPrizeLoginInfoAccess(updateTime);
			}
			if(entranceId == NoticeEntranceType.ZHOUNIANQING_NOTICE)// 福袋、周卡//
			{
				// 福袋//
				RebateInfoFPort rif = FPortManager.Instance.getFPort ("RebateInfoFPort") as RebateInfoFPort;
				rif.RebateInfoAccess(updateTime);
				// 周卡//
				WeekCardInfoFPort wcif = FPortManager.Instance.getFPort ("WeekCardInfoFPort") as WeekCardInfoFPort;
				wcif.WeekCardInfoAccess(updateTime);
			}
		}

    }

	private void startTimer ()
	{
        //if (timer != null)
        //    return;
        //timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
        //timer.addOnTimer (updateTime);
        //timer.start ();
	}

	private void updateTime ()
	{
		if (content == null || content.transform.childCount == 0)
			return;
		Transform[] childs = content.GetComponentsInChildren<Transform> ();
		NoticeTopButton noticeTopButton;
		foreach (Transform item in childs) {
			noticeTopButton = item.gameObject.GetComponent<NoticeTopButton> ();
			if (noticeTopButton != null) {
				//if(noticeTopButton.getNotice().getSample().sid == 30000019)
				//Debug.LogError(">>>>111>>>notice name : " + noticeTopButton.getNotice().getSample().name + "   ss "+ noticeTopButton.getNotice().getSample().sid);
				noticeTopButton.updateTime ();
			}
		}
	}

	public void updateSelectButton (int _type)
	{
		defaultSelectSid = _type;
	}
	/// <summary>
	/// 更新默认选项，断线重连特殊处理
	/// </summary>
	/// <param name="type">Type.</param>
	public void updateDefaulSelect(int type)
	{
		defaultSelectSid = type;
		NoticeTopButton selectBtn = tabButtons [defaultSelectSid] as NoticeTopButton;
		showDetail (selectBtn);
	}

	public void NextFrameInitTopButton (bool resetSelectSid, int positionIndex)
	{
		if (content != null && content.transform.childCount > 0)
			Utils.RemoveAllChild (content.transform);
		StartCoroutine (Utils.DelayRunNextFrame (() => {
			initTopButton (resetSelectSid, positionIndex,entranceId);
		}));
	}

	public void initTopButton ()
	{
		initTopButton (false, 0,entranceId);
	}

	public void initTopButton (bool resetSelectSid, int positionIndex, int entranceId)
	{
		flag=false;
		indexxx=0;
		tabButtons = new Hashtable ();
		NoticeTopButton itemButton;
		int itemSid;
		if (content.transform.childCount > 0)
			Utils.RemoveAllChild (content.transform);
		UIGrid grid = content.GetComponent<UIGrid> ();
		List<Notice> array = NoticeManagerment.Instance.getValidNoticeList (entranceId);
		UIScrollView scrollView = topButtons.transform.GetComponent<UIScrollView> ();
		if (resetSelectSid) {
				defaultSelectSid = -1;
		}
		scrollView.transform.localPosition = Vector3.zero;
		scrollView.panel.clipOffset = Vector2.zero;
		for (int i=0; i<array.Count&&array[i]!=null; i++) {
			if (UserManager.Instance.self.getUserLevel () >= array [i].getSample ().levelLimit) {
                if (array[i].getSample().type == NoticeType.LADDER_ACTION_TIME)
                    continue;         
				GameObject obj = Instantiate (topButton) as GameObject;
				obj.transform.parent = content.transform;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				obj.GetComponent<UIDragScrollView> ().scrollView = scrollView;
				obj.GetComponent<ButtonBase> ().fatherWindow = this;
				obj.name = "notice_" + array [i].getSample ().order;
				itemSid = array [i].sid;
                if (defaultSelectSid == -1)
                {//保证至少选中第一个
                   	 	defaultSelectSid = itemSid;
                }
				itemButton = obj.GetComponent<NoticeTopButton> ();
				itemButton.init (array [i]);
				itemButton.icon.mainTexture = setIcon (StringKit.toInt (array [i].getSample ().icon));
				tabButtons.Add (array [i].sid, itemButton);
				itemButton.local_x = grid.cellWidth * i;
				indexxx++;
				if (i == positionIndex&& defaultSelectSid == -1) {
						defaultSelectSid = itemSid;
				}
			}
		}
		//if (defaultSelectSid == 0) {//保证至少选中第一个
		//	defaultSelectSid = array [0].sid;
		//}
		if (array.Count > 0) {
			grid.onReposition = updateTabButtonView;
			grid.repositionNow = true;
		}
		if (GuideManager.Instance.isEqualStep (127003000)) {
			updateSelectButton (NoticeType.GODDNESS_SHAKE_SID);
			GuideManager.Instance.guideEvent ();
		}
		flag=true;
	}

	private void updateTabButtonView ()
	{
		NoticeTopButton selectBtn = tabButtons [defaultSelectSid] as NoticeTopButton;
		UIGrid grid = content.GetComponent<UIGrid> ();
		UIScrollView scrollView = topButtons.transform.GetComponent<UIScrollView> ();
		float toX = selectBtn.local_x;
		float min_X = grid.cellWidth * tabButtons.Keys.Count;

		Vector4 clip = scrollView.panel.finalClipRegion;
		if (min_X - toX < clip.z) {
			toX = min_X - clip.z + scrollView.panel.clipSoftness.x;
		}
		scrollView.MoveRelative (new Vector3 (-toX, 0, 0));
		showDetail (selectBtn);
        topButtons.transform.localPosition = Vector3.zero;
        topButtons.GetComponent<UIPanel>().clipOffset = Vector2.zero;
	}

	private Texture setIcon (int type)
	{
        if (type == NoticeType.TOPUPNOTICE || type == NoticeType.NEW_RECHARGE || type == NoticeType.BACK_RECHARGE)
            return rechargeUI;
        else if (type == NoticeType.TIME_RECHARGE)
            return timeRechargeUI;
        else if (type == NoticeType.COSTNOTICE || type == NoticeType.NEW_CONSUME || type == NoticeType.BACK_PRIZE)
            return totalCostUI;
        else if (type == NoticeType.EXCHANGENOTICE || type == NoticeType.NEW_EXCHANGE)
            return exchangeUI;
        else if (type == NoticeType.HEROEAT)
            return heroEatUI;
        else if (type == NoticeType.ALCHEMY)
            return alchemyUI;
        else if (type == NoticeType.CDKEY)
            return boxCodeUI;
        else if (type == NoticeType.MONTHCARD)
            return monthCardUI;
        else if (type == NoticeType.HAPPY_TURN_SPRITE)
            return happyTurnSpriteUI;
        else if (type == NoticeType.ONERMB)
            return oneRmbUI;
        else if (type == NoticeType.QUIZ_EXAM)
            return quizbUI;
        else if (type == NoticeType.QUIZ_SURVEY)
            return quizbUI;
        else if (type == NoticeType.HAPPY_SUNDAY)
            return happySumdayUI;
        else if (type == NoticeType.CONSUME_REBATE)
            return consumeRebateUI;
        else if (type == NoticeType.DOUBLE_RMB)
            return doubleRMBUI;
        else if (type == NoticeType.LUCKY_CARD)
            return xianshiUI;
        else if (type == NoticeType.REMAKE_EQUIP)
            return remakeEquipUI;
        else if (type == NoticeType.LADDER_HEGEMONY)
            return luckyCardUI;
        else if (type == NoticeType.XIANSHI_HUODONG || type == NoticeType.LUCKY_EQUIP || type == NoticeType.LIMIT_COLLECT)
            return xianshiUI;
        else if (type == NoticeType.SUPERDRAW)
            return superDrawUI;
        else if (type == NoticeType.FESTIVAL_WISH)
            return festivalWishUI;
        else if (type == NoticeType.FESTIVAL_FIREWORKS)
            return festivalFireworksUI;
        else if (type == NoticeType.SIGNIN)
            return signInUI;
        else if (type == NoticeType.SHAREDRAW)
            return shareDrawUI;
		else if(type == NoticeType.XIANSHI_FANLI)
			return fudaiUI;
		else if(type == NoticeType.WEEKCARD)
			return weekCardUI;
		else if(type == NoticeType.LOTTERY)
			return lotteryUI;
		return rechargeUI;

	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
		UiManager.Instance.noticeWindow = this;
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {

			//如果是登录弹窗,关闭后开mainwindow
			if (firstBootIndex >= 0) {
				UiManager.Instance.openMainWindow ();
				firstBootIndex = -1;
			} else if(fatherWindow!=null&&fatherWindow is TaskWindow){
				UiManager.Instance.backGround.switchBackGround ("backGround_1");
			}
				finishWindow ();

		} else if (gameObj.name.StartsWith ("notice")) {
			NoticeTopButton button = gameObj.GetComponent<NoticeTopButton> ();
			//点击的不是当前显示的活动
			if (lastSelect.getNotice ().sid != button.getNotice ().sid)
				showDetail (button);
			MaskWindow.UnlockUI ();
		} else if (gameObj.name.StartsWith ("Quiz_")) {
			int type = lastSelect.getNotice ().getSample ().type;
			if (type == NoticeType.QUIZ_EXAM || type == NoticeType.QUIZ_SURVEY) {
				show.GetComponent<NoticeQuizContent> ().clickButton (gameObj);
			}
		}
		else if (gameObj.name == "buttonRecharge") {  
			UiManager.Instance.openWindow<rechargeWindow> ();
		}	
		else if (gameObj.name == "ButtonBuymaterial") {  
			UiManager.Instance.openWindow<ShopWindow> ((win)=>{win.setCallBack(null);});
		}	
	}

	private void doSwitchBackGround(int type) {
		if (type == NoticeType.HAPPY_TURN_SPRITE) {
			UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		} else if (type == NoticeType.HEROEAT) {
			UiManager.Instance.backGround.switchBackGround ("noticeHeroEatBg");
		} else if (type == NoticeType.MONTHCARD) {
			UiManager.Instance.backGround.switchBackGround ("monthCardBg");
		}else if (type == NoticeType.WEEKCARD) {
			UiManager.Instance.backGround.switchBackGround ("weekCardBg");
		}  else {
			UiManager.Instance.backGround.switchBackGround ("backGround_1");
		}
	}

	private void showDetail (NoticeTopButton button)
	{
		Notice notice = button.getNotice ();
		int type = notice.getSample ().type;
		doSwitchBackGround (type);
		if (type == NoticeType.TOPUPNOTICE || type == NoticeType.COSTNOTICE || type == NoticeType.TIME_RECHARGE ||
			type == NoticeType.NEW_RECHARGE || type == NoticeType.NEW_CONSUME) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, rechargeShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeActivityRechargeContent> ().initContent (notice, this);
		} else if (type == NoticeType.EXCHANGENOTICE || type == NoticeType.NEW_EXCHANGE) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, exchangeShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeActivityExchangeContent> ().initContent (notice, this);
		} else if (type == NoticeType.HEROEAT) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, heroEatShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<HeroEatContent> ().initContent (this);
		} else if (type == NoticeType.ALCHEMY) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, alchemyShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<AlchemyContent> ().initContent (this);
		} else if (type == NoticeType.CDKEY) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, inviteShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<InviteContent> ().initWindow (0, this);
		} else if (type == NoticeType.MONTHCARD) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, monthCardShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<MonthCardContent> ().initContent (this);
		} else if (type == NoticeType.HAPPY_TURN_SPRITE || type == NoticeType.XIANSHI_HAPPY_TURN) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, happyTurnSpriteShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<HappyTurnSpriteContent> ().initContent (notice, this);
		} else if (type == NoticeType.ONERMB) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, oneRmbShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<OneRmbContent> ().initContent (this);
		} else if (type == NoticeType.QUIZ_EXAM) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, quizShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeQuizContent> ().initData (this, notice);
		} else if (type == NoticeType.QUIZ_SURVEY) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, quizShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeQuizContent> ().initData (this, notice);
		} else if (type == NoticeType.HAPPY_SUNDAY) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, happySunday);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<HappySundayContent> ().initContent (this,notice);
		} else if (type == NoticeType.CONSUME_REBATE) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, consumeRebateShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeConsumeRebateContent> ().initContent (notice, this);
		} else if (type == NoticeType.LUCKY_CARD) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, luckyCardShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<LuckyCardContent> ().initContent (this, notice);
		} else if (type == NoticeType.LUCKY_EQUIP || type == NoticeType.XIANSHI_HUODONG) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, luckyEquipShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<LuckyEquipContent> ().initContent (this, notice);
		} else if (type == NoticeType.DOUBLE_RMB) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, doubleRMB);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<DoubleRMBContent> ().initContent (this, notice);
		} else if (type == NoticeType.REMAKE_EQUIP) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, equipRemakeShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<EquipRemakeContent> ().initContent (this, notice);
		} else if (type == NoticeType.DAILY_REBATE) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, dailyReateShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			//(FPortManager.Instance.getFPort ("InitLuckyDrawFPort") as InitLuckyDrawFPort).init (null);
			dailyRateList = TaskManagerment.Instance.getDailyRebateTask ();
			show.GetComponent<NoticeActivityDailyRebateContent> ().initContent (notice, this, dailyRateList);
		} else if (type == NoticeType.LADDER_HEGEMONY) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, ladderHegemoney);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
            NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(notice.sid);
			Notice activenotice = NoticeManagerment.Instance.getNoticeListByType (NoticeType.LADDER_ACTION_TIME, notice.sid,sample.entranceId);
			show.GetComponent<NoticeLadderHegeMoneyContent> ().initContent (notice, this);
			show.GetComponent<NoticeLadderHegeMoneyContent> ().initActiveNotice (activenotice);
		} else if (type == NoticeType.LIMIT_COLLECT) {
			Destroy (show);
			show = NGUITools.AddChild (showPos.gameObject, limitCollect);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeActivityLimitCollectContent> ().initContent (notice, this);
        } else if (type == NoticeType.ONE_MANY_RECHARGE){
            Destroy(show);
            show = NGUITools.AddChild(showPos.gameObject, oneManyShow);
            show.transform.localPosition = Vector3.zero;
            show.transform.localScale = Vector3.one;
            show.GetComponent<NoticeOneManyRechargeContent>().initContent(notice, this);
		} else if(type == NoticeType.FESTIVAL_WISH)
		{
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject,festivalShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeActivityFestivalWishContent>().initContent(notice, this);
		} else if(type == NoticeType.FESTIVAL_FIREWORKS)
		{
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject,fireworksShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeActivityFestivalFireworksContent>().initContent(notice, this);
		}
		else if(type == NoticeType.SUPERDRAW)
		{
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject,superDrawShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<NoticeActivitySuperDrawContent>().initContent(notice, this);
		}
		else if(type == NoticeType.XIANSHI_FANLI)
		{
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject,rebateShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<RebateContent>().initContent(notice, this);
        } else if (type == NoticeType.SIGNIN) {
            Destroy(show);
            show = NGUITools.AddChild(showPos.gameObject, signInShow);
            show.transform.localPosition = Vector3.zero;
            show.transform.localScale = Vector3.one;
            show.GetComponent<NoticeActivitySignInContent>().initContent(notice, this);
        } else if (type == NoticeType.SHAREDRAW) {
            Destroy(show);
            show = NGUITools.AddChild(showPos.gameObject, shareContent);
            show.transform.localPosition = Vector3.zero;
            show.transform.localScale = Vector3.one;
            show.GetComponent<ShareDrawContent>().initContent(notice, this);
		} else if (type == NoticeType.BACK_PRIZE) {
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject, backPrizeShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<OldPlayerBackContent>().initContent(notice, this,button);
		}else if (type == NoticeType.BACK_RECHARGE) {
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject, backRechargeShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<BackRechargeContent>().initContent(notice, this);
		}else if (type == NoticeType.WEEKCARD) {
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject, weekCardShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			show.GetComponent<WeekCardContent>().initContent(this);
		}else if (type == NoticeType.LOTTERY) {
			Destroy(show);
			show = NGUITools.AddChild(showPos.gameObject, lotteryShow);
			show.transform.localPosition = Vector3.zero;
			show.transform.localScale = Vector3.one;
			LotteryInfoFPort fPort = FPortManager.Instance.getFPort ("LotteryInfoFPort") as LotteryInfoFPort;
			fPort.lotteryInfoAccess(()=>{
				LotteryManagement.Instance.canGetInitFPort = false;
				show.GetComponent<LotteryContent>().initContent(this,notice);
			});
		}else {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0358"));
            return;
        }
		if (lastSelect != null)
			lastSelect.selelct.gameObject.SetActive (false);
		button.selelct.gameObject.SetActive (true);
		
		lastSelect = button;
	}
}
