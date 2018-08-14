using UnityEngine;
using System.Collections;

/// <summary>
/// 节日许愿条目
/// </summary>
public class FestivalWishItemUI : MonoBase {
	/**  许愿按钮 */
	public ButtonBase wishButton;
	/** 剩余时间 */
	public UILabel timeLabel;
	/** 奖励道具 */
	public GoodsView goodsView;
	/** 当前许愿人数 */
	public UILabel currentPersonNum;
	/** 最大许愿人数 */
	//public UILabel maxPersonNum;
	/** 原价 */
	public UILabel oldCostLabel;
	/** 许愿价格 */
	public UILabel wishCostLabel;
	/** 许愿进度条 */
	public HpbarCtrl wishProgress;
	/** 许愿实体 */
	private FestivalWish festivalwish;

	private WindowBase win;
	private Timer timer;

	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initItemUI(FestivalWish wish,WindowBase fatherwindow)
	{
		this.festivalwish = wish;
		this.win = fatherwindow;
		this.goodsView.init(wish.sample.prizes);
		this.goodsView.fatherWindow = fatherwindow;
		this.currentPersonNum.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_07",wish.currentWishNum.ToString(),wish.sample.maxWishsNum.ToString());
		wishProgress.updateValue(wish.currentWishNum,wish.sample.maxWishsNum);
		this.oldCostLabel.text  = wish.sample.oldCost.ToString();
		this.wishCostLabel.text = wish.sample.wishCost.ToString();
		initButton();
		startTimer();
		updateTime();
	}

	/// <summary>
	/// 初始化按钮信息
	/// </summary>
	public void initButton()
	{
		//许愿按钮事件委托
		wishButton.fatherWindow = this.win;
		wishButton.onClickEvent = buttonEvent;
		if(festivalwish.state == 1||festivalwish.currentWishNum>=festivalwish.sample.maxWishsNum)
		{
			wishButton.disableButton(true);
			if(festivalwish.state == 1)
				wishButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_09").ToString ();
			else 
				wishButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_12").ToString ();
		}
		else{
			wishButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_03").ToString ();
			wishButton.disableButton(false);
		}
			
	}

	/// <summary>
	/// 点击事件
	/// </summary>
	private void buttonEvent(GameObject obj)
	{
		UiManager.Instance.openDialogWindow<FestivalWishWindow>((win)=>{
			win.initWindow(festivalwish.sample.wishCost,festivalwish.sid,updateUI);
		});
	}

	/// <summary>
	/// 许愿后更新界面
	/// </summary>
	public void updateUI()
	{
		wishProgress.updateValue(festivalwish.currentWishNum+1,festivalwish.sample.maxWishsNum);
		currentPersonNum.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_07",(festivalwish.currentWishNum+1).ToString(),festivalwish.sample.maxWishsNum.ToString());
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("noticeActivityFW_08"));
		wishButton.disableButton(true);
		wishButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_09");
	}

	private void startTimer ()
	{
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start ();
	}
	private void updateTime()
	{
		timeLabel.text = TimeKit.timeTransformDHMS(festivalwish.endTime-ServerTimeKit.getSecondTime())+LanguageConfigManager.Instance.getLanguage("s0086");
		if(festivalwish.endTime-ServerTimeKit.getSecondTime()<=0)
		{
			timeLabel.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_12");
			wishButton.disableButton(true);
			wishButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("noticeActivityFW_12");
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
		}
	}
	public void OnDisable ()
	{
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
}
