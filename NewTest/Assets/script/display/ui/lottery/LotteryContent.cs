using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LotteryContent : MonoBase 
{
	public GameObject mainPanel;// 主界面//
	public GameObject selectedAward;// 选注奖励界面//
	public GameObject noAwardShowNum;// 未开奖号码显示//
	public GameObject awardShowNum;// 开奖结果号码显示//
	public GameObject infoTmp;// 号码信息模板 //
	public GameObject noticeTips;// 预告提示//
	public UILabel noticeTipsLabel;// 预告文字// 
	public GameObject timeCount;// 倒计时提示//
	public UILabel timeCountLabel;// 倒计时文字//
	public GameObject sendAwardTips;// 发放奖励提示//
	public GameObject moneyTips;// 奖池提示//
	public UILabel moneyTipsLabel;// 奖池奖金文字//
	public UIGrid infoGrid;// 号码信息grid//
	public ButtonBase selectNumBtn;// 选号按钮//
	public ButtonBase selectAwardBtn;// 选注奖励按钮//
	public UISprite[] numSprite;// 中奖号码展示//
	public UILabel activityTimeTips;// 活动时间label//
	public ButtonBase closeBtn;// 关闭选注奖励界面按钮//
	//计时器
	private Timer timer;
	bool isUpdateFinishPhase = true;// 是否刷新结束阶段//
	bool isUpdateSelectNumPhase = true; // 是否刷新选号阶段//
	bool isUpdateAwardNumPhase = true;// 是否刷新开奖阶段//
	bool isUpdateSendAwardPhase = true;// 是否刷新发奖阶段//
	bool isUpdatePreViewPhase = true;// 预告阶段//
	GameObject[] lotteryObjs;// 存放玩家购买彩票列表中的obj//

	public LotteryAwardContent awardContent;
	public LotterySelfListContent selfListContent;

	public GameObject awardList;
	public UIGrid awardListGrid;
	public GameObject awardListTmp;
	GameObject[] awardListObjs;
	public ButtonBase clickAwardListBg;
	public ButtonBase clickCloseAwardListBtn;
	public LotteryAwardListContent listContent;

	public GameObject radioTmp;// 跑马灯文字模板 //
	public Transform tmpFromTrans;
	public Transform tmpToTrans;
	int radioLabelCount = 0;

	public UIPlayTween tweenHelpPanel;
	public ButtonBase buttonHelp;
	public ButtonBase buttonHelpClose;
	public GameObject helpMask;

	public GameObject selectAwardsNumObj;
	public UILabel selectAwardsNumCount;
	public GameObject selectNumObj;
	public UILabel selectNumCount;

	public int moneyTimeCount;// 奖池奖金计时器//

	public WindowBase win;
	Notice m_notice;



	public void initContent(WindowBase win,Notice notice)
	{
		this.m_notice = notice;
		this.win = win;
		initRadioLabels();
		initMainPanel();
		setTimer();
	}

	public void setTimer()
	{
		if(timer == null)
		{
			timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
			timer.addOnTimer(updateTime);
			timer.start();
		}
	}

	// 初始化主界面//
	public void initMainPanel()
	{
		selfListContent.reLoad();
		selectNumBtn.fatherWindow = win;
		selectAwardBtn.fatherWindow = win;
		closeBtn.fatherWindow = win;
		selectNumBtn.onClickEvent = ClickSelectNumBtn;
		selectAwardBtn.onClickEvent = ClickSelectAwardBtn;
		closeBtn.onClickEvent = clickCloseSelectAwardBtn;
		clickCloseAwardListBtn.fatherWindow = win;
		clickAwardListBg.fatherWindow = win;
		clickCloseAwardListBtn.onClickEvent = clickCloseAwardList;
		clickAwardListBg.onClickEvent = clickAwardList;
		buttonHelp.fatherWindow = win;
		buttonHelp.onClickEvent = clickHelpButton;
		buttonHelpClose.fatherWindow = win;
		buttonHelpClose.onClickEvent = clickHelpButtonClose;

		setSelectAwardsCountTips();
		setSelectNumCountTips();
	}

	void setSelectAwardsCountTips()
	{
		if(LotteryManagement.Instance.selectedAwardCount > 0)
		{
			selectAwardsNumObj.SetActive(true);
			selectAwardsNumCount.text = LotteryManagement.Instance.selectedAwardCount.ToString();
		}
		else 
		{
			selectAwardsNumObj.SetActive(false);
		}
	}

	void setSelectNumCountTips()
	{
		if(LotteryManagement.Instance.getLotteryCount() > 0 && (m_notice as LotteryNotice).isActivityOpen())
		{
			selectNumObj.SetActive(true);
			selectNumCount.text = LotteryManagement.Instance.getLotteryCount().ToString();
		}
		else 
		{
			selectNumObj.SetActive(false);
		}
	}

	// 关闭中奖列表//
	void clickCloseAwardList(GameObject obj)
	{
		awardList.SetActive(false);
	}
	// 打开中奖列表//
	void clickAwardList(GameObject obj)
	{
		awardList.SetActive(true);
		listContent.reLoad();
	}

	// 更新结束 预告阶段ui//
	void updateFinishPhase()
	{
		reSetTips();
		showAwardNum(true);
		setSelectNumBtnState(true);
		// 下次活动将在周几开启//
		noticeTips.SetActive(true);
		noticeTipsLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("lottery_nextOpenTips"),getNextOpenTimeLabel());
	}
	// 选号阶段//
	void updateSelectNumPhase()
	{
		reSetTips();
		showAwardNum(false);
		if(LotteryManagement.Instance.getLotteryCount() > 0)
		{
			selectNumBtn.disableButton(false);
			selectNumBtn.gameObject.GetComponent<UIButton>().normalSprite = "button_red";
			selectNumBtn.gameObject.GetComponent<UIButton>().pressedSprite = "button_red_clickOn";
		}
		else
		{
			selectNumBtn.disableButton(true);
			selectNumBtn.gameObject.GetComponent<UIButton>().normalSprite = selectNumBtn.gameObject.GetComponent<UIButton>().disabledSprite;
			selectNumBtn.gameObject.GetComponent<UIButton>().pressedSprite = "";
			selectNumBtn.gameObject.GetComponent<BoxCollider>().enabled = true;
		}
		// 奖池金额//
		moneyTips.SetActive(true);
		moneyTipsLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("lottery_gold"),LotteryManagement.Instance.moneyAward.ToString());
	}
	// 开奖阶段//
	void updateAwardNumPhase()
	{
		reSetTips();
		showAwardNum(false);
		setSelectNumBtnState(true);
		//  开奖倒计时//
		timeCount.SetActive(true);
	}
	// 发奖阶段//
	void updateSendAwardPhase()
	{
		reSetTips();
		showAwardNum(true);
		setSelectNumBtnState(true);
		sendAwardTips.SetActive(true);
	}

	public void reSetTips()
	{
		noticeTips.SetActive(false);
		timeCount.SetActive(false);
		sendAwardTips.SetActive(false);
		moneyTips.SetActive(false);
	}
	// 显示开奖结果//
	public void showAwardNum(bool isShow)
	{
		if(isShow)
		{
			setShowAwardNumByNum(LotteryManagement.Instance.awardResult);
		}
		else
		{
			awardShowNum.SetActive(false);
		}
	}
	public void setShowAwardNumByNum(string num)
	{
		if(num != "-1" || string.IsNullOrEmpty(num))
		{
			Char[] strs = num.ToCharArray();
			for(int i=0;i<strs.Length;i++)
			{
				switch (strs[i]) {
				case '0':
					numSprite[i].spriteName = "0";
					break;
				case '1':
					numSprite[i].spriteName = "1";
					break;
				case '2':
					numSprite[i].spriteName = "2";
					break;
				case '3':
					numSprite[i].spriteName = "3";
					break;
				case '4':
					numSprite[i].spriteName = "4";
					break;
				case '5':
					numSprite[i].spriteName = "5";
					break;
				case '6':
					numSprite[i].spriteName = "6";
					break;
				case '7':
					numSprite[i].spriteName = "7";
					break;
				case '8':
					numSprite[i].spriteName = "8";
					break;
				case '9':
					numSprite[i].spriteName = "9";
					break;
				}
			}
			awardShowNum.SetActive(true);
		}
		else 
		{
			noAwardShowNum.SetActive(true);
			awardShowNum.SetActive(false);
		}

	}

	string getNextOpenTimeLabel()
	{
		int currentDay = TimeKit.getWeekCHA(TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime()).DayOfWeek);
		int[] openTimes = CommandConfigManager.Instance.getLotteryData().openTime;
		for(int i=0;i<openTimes.Length;i++)
		{
			if(currentDay < openTimes[i])
				return LanguageConfigManager.Instance.getLanguage("lottery_" + openTimes[i]);
		}

		return LanguageConfigManager.Instance.getLanguage("lottery_" + openTimes[0]);
	}

	// 开奖倒计时//
	public void updateAwardNumCountTime()
	{
		if(CommandConfigManager.Instance.getLotteryData().awardNumEndTime >= ServerTimeKit.getCurrentSecond())
		{
			timeCountLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("lottery_timeCount"),TimeKit.timeTransformDHMS (CommandConfigManager.Instance.getLotteryData().awardNumEndTime - ServerTimeKit.getCurrentSecond()));
		}
		else 
		{
			timeCount.SetActive(false);
		}
	}
	// 活动是否开启 //
	bool isActivityOpen()
	{
		int day = TimeKit.getWeekCHA(TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime()).DayOfWeek);
		for(int i=0;i<CommandConfigManager.Instance.getLotteryData().openTime.Length;i++)
		{
			if(day == CommandConfigManager.Instance.getLotteryData().openTime[i])
			{
				return true;
			}
		}
		return false;
	}
	// 每一秒执行//
	void updateTime()
	{
		//  活动开启中//
		if(isActivityOpen())
		{
			// 开奖后刷新//
			if(ServerTimeKit.getCurrentSecond() == CommandConfigManager.Instance.getLotteryData().awardNumEndTime)
			{
				LotteryInfoFPort fPort = FPortManager.Instance.getFPort("LotteryInfoFPort") as LotteryInfoFPort;
				fPort.lotteryInfoAccess(()=>{
					initRadioLabels();
					selfListContent.reLoad();
					showAwardNum(true);
				});
			}
			// 选号阶段//
			if(ServerTimeKit.getCurrentSecond() <= CommandConfigManager.Instance.getLotteryData().selectNumEndTime && ServerTimeKit.getCurrentSecond() > 0)
			{
				if(isUpdateSelectNumPhase)
				{
					setUpdateSelectNumFalse();
					// 处于界面跨天刷新//
					if(LotteryManagement.Instance.canGetInitFPort)
					{
						LotteryInfoFPort fPort = FPortManager.Instance.getFPort("LotteryInfoFPort") as LotteryInfoFPort;
						fPort.lotteryInfoAccess(()=>{
							updateSelectNumPhase();
							setSelectAwardsCountTips();
							setSelectNumCountTips();
						});
					}
					else 
					{
						updateSelectNumPhase();
					}
				}
			}
			// 开奖倒计时阶段//
			else if(ServerTimeKit.getCurrentSecond() > CommandConfigManager.Instance.getLotteryData().selectNumEndTime && ServerTimeKit.getCurrentSecond() <= CommandConfigManager.Instance.getLotteryData().awardNumEndTime)
			{
				if(isUpdateAwardNumPhase)
				{
					if(LotteryManagement.Instance.awardResult != "-1")
					{
						if(isUpdateSendAwardPhase)
						{
							setUpdateSendAwardFalse();
							updateSendAwardPhase();
						}
					}
					else 
					{
						setUpdateAwardNumFalse();
						updateAwardNumPhase();
					}
				}
			}
			// 发奖阶段//
			else if(ServerTimeKit.getCurrentSecond() > CommandConfigManager.Instance.getLotteryData().awardNumEndTime && ServerTimeKit.getCurrentSecond() <= CommandConfigManager.Instance.getLotteryData().sendAwardEndTime)
			{
				if(isUpdateSendAwardPhase)
				{
					setUpdateSendAwardFalse();
					updateSendAwardPhase();
				}
			}
			else
			{
				if(isUpdateFinishPhase)
				{
					setUpdateFinishFalse();
					updateFinishPhase();
				}
			}
		}
		else 
		{
			if(!LotteryManagement.Instance.canGetInitFPort)
			{
				LotteryManagement.Instance.canGetInitFPort = true;
			}
			if(isUpdatePreViewPhase)
			{
				setUpdatePreViewFalse();
				updateFinishPhase();
			}
		}

		if(moneyTips.activeSelf)
		{
			if(moneyTimeCount >= CommandConfigManager.Instance.getLotteryData().updateLotteryMoneyTime)
			{
				moneyTimeCount = 0;
				LotteryInfoFPort fPort = FPortManager.Instance.getFPort("LotteryInfoFPort") as LotteryInfoFPort;
				fPort.lotteryInfoAccess(()=>{
					moneyTipsLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("lottery_gold"),LotteryManagement.Instance.moneyAward.ToString());
				});
			}
			else 
			{
				moneyTimeCount++;
			}
		}

		if(timeCount.activeSelf)
		{
			updateAwardNumCountTime();
		}
	}
	
	void OnDisable()
	{
		cleanRadioLabels();
		if(timer != null)
		{
			moneyTimeCount = 0;
			timer.stop();
			timer = null;
		}

	}
	//  点击选号按钮//
	void ClickSelectNumBtn(GameObject obj)
	{
		// 不在选注时间//
		if(!(m_notice as LotteryNotice).isActivityOpen())
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_tips2"));
			});
			return;
		}
		// 选注次数不足//
		if(LotteryManagement.Instance.getLotteryCount() <= 0)
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_tips"));
			});
			return;
		}
		//  打开选号界面//
		UiManager.Instance.openWindow<LotteryWindow>();
	}
	//  点击选注奖励按钮//
	void ClickSelectAwardBtn(GameObject obj)
	{
		MaskWindow.UnlockUI();
		selectedAward.SetActive(true);
		awardContent.initContent(win);
	}
	// 点击关闭选注奖励界面//
	void clickCloseSelectAwardBtn(GameObject obj)
	{
		setSelectAwardsCountTips();
		awardContent.destroyAward();
		selectedAward.SetActive(false);
	}

	// 初始化跑马灯文字信息//
	public void initRadioLabels()
	{
		cleanRadioLabels();
//		for(int i=0;i<3;i++)
//		{
//			LotteryManagement.Instance.awardList.Add(new AwardLottery("2015-12-31","fasdfas///" + i,"50000000","1234"));
//		}
		if(LotteryManagement.Instance.awardList != null && LotteryManagement.Instance.awardList.Count > 0)
		{
			AwardLottery award = LotteryManagement.Instance.awardList[radioLabelCount++];
			radioTmp.GetComponent<UILabel>().text = string.Format(LanguageConfigManager.Instance.getLanguage("Lottery_awardTips"),award.time,award.serName + award.playerName,award.money);
			radioTmp.GetComponent<TweenTransform>().from = tmpFromTrans;
			radioTmp.GetComponent<TweenTransform>().to = tmpToTrans;
			EventDelegate.Add(radioTmp.GetComponent<TweenTransform>().onFinished, transFinished);
			radioTmp.SetActive(true);
		}
	}
	void cleanRadioLabels()
	{
		radioLabelCount = 0;
		radioTmp.SetActive(false);
	}
	void transFinished()
	{
		if(radioLabelCount >= LotteryManagement.Instance.awardList.Count)
		{
			radioLabelCount = 0;
		}
		AwardLottery award = LotteryManagement.Instance.awardList[radioLabelCount++];
		radioTmp.GetComponent<UILabel>().text = string.Format(LanguageConfigManager.Instance.getLanguage("Lottery_awardTips"),award.time,award.serName + award.playerName,award.money);
		radioTmp.GetComponent<TweenTransform>().ResetToBeginning();
		radioTmp.GetComponent<TweenTransform>().enabled = true;
	}

	public void tweenerMessageGroupIn (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Forward;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0.2f;
		}
		tween.Play (true);
	}
	
	public void tweenerMessageGroupOut (UIPlayTween tween) {
		tween.playDirection = AnimationOrTween.Direction.Reverse;
		UITweener[] tws = tween.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0;
		}
		tween.Play (true);
	}

	void clickHelpButton(GameObject obj)
	{
		tweenerMessageGroupIn(tweenHelpPanel);
		helpMask.SetActive(true);
	}
	void clickHelpButtonClose(GameObject obj)
	{
		tweenerMessageGroupOut(tweenHelpPanel);
		helpMask.SetActive(false);
	}
	public void setBoolTrue()
	{
		isUpdateFinishPhase = true;
		isUpdateSelectNumPhase = true;
		isUpdateAwardNumPhase = true;
		isUpdateSendAwardPhase = true;
		isUpdatePreViewPhase =  true;
	}
	public void reSetMoneyTimeCount()
	{
		moneyTimeCount = 0;
	}

	void setSelectNumBtnState(bool isDisable)
	{
		if(!isDisable)
		{
			selectNumBtn.disableButton(false);
			selectNumBtn.gameObject.GetComponent<UIButton>().normalSprite = "button_red";
			selectNumBtn.gameObject.GetComponent<UIButton>().pressedSprite = "button_red_clickOn";
		}
		else
		{
			selectNumBtn.disableButton(true);
			selectNumBtn.gameObject.GetComponent<UIButton>().normalSprite = selectNumBtn.gameObject.GetComponent<UIButton>().disabledSprite;
			selectNumBtn.gameObject.GetComponent<UIButton>().pressedSprite = "";
			selectNumBtn.gameObject.GetComponent<BoxCollider>().enabled = true;
		}
	}
	void setUpdatePreViewFalse()
	{
		setBoolTrue();
		isUpdatePreViewPhase = false;
	}
	void setUpdateSelectNumFalse()
	{
		setBoolTrue();
		isUpdateSelectNumPhase = false;
	}
	void setUpdateAwardNumFalse()
	{
		setBoolTrue();
		isUpdateAwardNumPhase = false;
	}
	void setUpdateSendAwardFalse()
	{
		setBoolTrue();
		isUpdateSendAwardPhase = false;
	}
	void setUpdateFinishFalse()
	{
		setBoolTrue();
		isUpdateFinishPhase = false;
	}
}
