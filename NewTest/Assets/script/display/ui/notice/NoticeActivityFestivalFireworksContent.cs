using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 节日礼花活动
/// </summary>
public class NoticeActivityFestivalFireworksContent : MonoBase {

	private Notice notice;
	public UILabel desLabel;
	public UILabel timeLabel;
	public WindowBase win;//活动窗口
	private Timer timer;
	public FestivalFireworksItemUI[] item;
	private ActiveTime activeTime;
	public ButtonBase buttonBuyMatrial;
	public GameObject[] effects;

	private NoticeSample sample;
	/** 活动开启时间 */
	int noticeOpenTime;
	/** 活动结束时间 */
	int noticeCloseTime;
	/** 活动开启倒计时文本 */
	string openTimeNoticeText;
	/** 活动结束倒计时文本 */
	string closeTimeNoticeText;
	int [] allSids;

	List<FestivalFireworksSample> fireworksSamples ;

	public void initContent (Notice notice, WindowBase win)
	{
		this.notice = notice;
		this.win = win;
		buttonBuyMatrial.fatherWindow = win;
		initItems();
		setNoticeOpenTime();
		refreshNoticeTime();
		startTimer();
	}
	/// <summary>
	/// 初始化各个条目内容
	/// </summary>
	public void initItems()
	{
		this.sample = NoticeSampleManager.Instance.getNoticeSampleBySid(this.notice.getSample().sid);
		allSids = (sample.content as SidNoticeContent).sids;
		fireworksSamples = new List<FestivalFireworksSample>(); 
		for(int i=0;i<allSids.Length;i++)
		{
			fireworksSamples.Add(FestivalFireworksSampleManager.Instance.getFestivalFireworksSampleBySid(allSids[i]));
		}
		if(fireworksSamples.Count<item.Length)
			return;
		for(int i=0;i<item.Length;i++)
		{
			//当前只有三个条目，只会取配置的前三条数据
			item[i].initItemUI(fireworksSamples[i],win);

		}
	}
	/// <summary>
	/// 开启计时器
	/// </summary>
	private void startTimer ()
	{
		if(timer==null )
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer(refreshNoticeTime);
		timer.start();
	}
	/// <summary>
	/// 结束时销毁计时器
	/// </summary>
	public void OnDisable ()
	{
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
//		foreach(GameObject a in effects)
//			a.gameObject.SetActive(false);
	}

	/// <summary>
	/// 设置活动开启时间
	/// </summary>
	public void setNoticeOpenTime()
	{
		this.openTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOpen");
		this.closeTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOver");
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
				timeLabel.gameObject.SetActive(true);
				timeLabel.text = sample.activiteDesc +"("+ closeTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainCloseTime))+")";
			}
			else
			{
				timeLabel.gameObject.SetActive(false);
				timer.stop();
				timer = null;
			}
		}
		/** 还没开启 */
		else
		{
			timeLabel.gameObject.SetActive(true);
			timeLabel.text = openTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainTime));
		}
	}
}

