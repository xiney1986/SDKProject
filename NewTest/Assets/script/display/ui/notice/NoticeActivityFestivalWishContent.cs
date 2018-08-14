using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 节日许愿活动
/// </summary>
public class NoticeActivityFestivalWishContent : MonoBase {

	public ContentFestivalWishItem content;
	private Notice notice;
	public UILabel desLabel;
	public UILabel timeLabel;
	public WindowBase win;//活动窗口
	private Timer timer;
	private Timer timer1;
	public GameObject[] effects;
    public GameObject noObj;
	private ActiveTime activeTime;

	private NoticeSample sample;
	/** 活动开启时间 */
	int noticeOpenTime;
	/** 活动结束时间 */
	int noticeCloseTime;
	/** 活动开启倒计时文本 */
	string openTimeNoticeText;
	/** 活动结束倒计时文本 */
	string closeTimeNoticeText;
	/**每条活动对应条目sid集合*/
	private string sids;
	int [] allSids;

	public void initContent (Notice notice, WindowBase win)
	{
		this.notice = notice;
		this.win = win;
		initContent();
		startTimer();
		setNoticeOpenTime();
		refreshNoticeTime();
	}
	public void initContent()
	{
		List<FestivalWish> list = new List<FestivalWish>();
        noObj.SetActive(false);
		this.sample = NoticeSampleManager.Instance.getNoticeSampleBySid(this.notice.getSample().sid);
		allSids = (sample.content as SidNoticeContent).sids;
		sids = "";
		if(allSids.Length >0)
		{
			for(int i=0;i<allSids.Length;i++)
				sids += allSids[i]+",";
		}
		FestivalWishManagerment.Instance.getAllFestivalWishInfo(()=>{
			
			list = FestivalWishManagerment.Instance.getAllFestivalWish();
            if (list == null || list.Count == 0) noObj.SetActive(true);
            else noObj.SetActive(false);
			content.Initialize(list,updateUI,this.win);
			content.reLoad(list.Count);
		},sids);
	}
	#region 暗箱操作人数
	//5分钟增加一次人数
	private void startTimer ()
	{
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_MINUTE*5);
		if(timer1==null )
			timer1 = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer1.addOnTimer(refreshNoticeTime);
		timer1.start();
		timer.addOnTimer (updateTime);
		timer.start ();
	}
	public void OnDisable ()
	{
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
		if (timer1 != null) {
			timer1.stop ();
			timer1 = null;
		}
		foreach(GameObject a in effects)
			a.gameObject.SetActive(false);
	}
	private void updateTime()
	{
		initContent(notice,win);
	}

	#endregion

	/** 设置活动开启时间 */
	public void setNoticeOpenTime()
	{
		this.openTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOpen");
		this.closeTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOver");
		activeTime = ActiveTime.getActiveTimeByID(this.notice.getSample().timeID);
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
				timeLabel.gameObject.SetActive(true);
				timeLabel.text = "("+ closeTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainCloseTime))+")";
			}
			else
			{
				timeLabel.gameObject.SetActive(false);
				if(timer1!=null)
				{
					timer1.stop();
					timer1 = null;
				}
			}
		}
		/** 还没开启 */
		else
		{
			timeLabel.gameObject.SetActive(true);
			timeLabel.text = openTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainTime));
		}
	}

	public void updateUI()
	{
		//desLabel.text = notice.getSample().activiteDesc;
		//timeLabel.text = notice.getTimeLimit();
	}
	private void initDesTime ()
	{
		if (notice.isTimeLimit ()) {
			int[] time = notice.getShowTimeLimit ();
			if(time == null){
				timeLabel.gameObject.SetActive(true);
				timeLabel.text = LanguageConfigManager.Instance.getLanguage("s0138");
				return;
			}
			timeLabel.gameObject.SetActive(true);
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice02", TimeKit.dateToFormat (time [0], LanguageConfigManager.Instance.getLanguage ("notice04")),	                                                             TimeKit.dateToFormat (time [1] - 1, LanguageConfigManager.Instance.getLanguage ("notice04")));
		} else
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice03");
	}
}

