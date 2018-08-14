using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 限时收集活动
/// </summary>
public class NoticeActivityLimitCollectContent : MonoBase {
	/** 活动项预制 */
	public GameObject itemPrefab;
	/** 时间显示 */
	public UILabel timeText;
	/** 动态容器 */
	public UIGrid content;
	private LimitCollectNotice notice;
	private List<LimitCollectSample> datas;
	private WindowBase win;
    private int[] sids;
    private ActiveTime activeTime;
    /** 活动开启时间 */
    int noticeOpenTime;
    /** 活动结束时间 */
    int noticeCloseTime;
    /** 活动开启倒计时文本 */
    string openTimeNoticeText;
    /** 活动结束倒计时文本 */
    string closeTimeNoticeText;
    private Timer timer;

    private void startTimer()
    {
        if (timer == null)
            timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(refreshNoticeTime);
        timer.start();
    }

	public void initContent (Notice notice, WindowBase win)
	{
        this.notice = notice as LimitCollectNotice;
        this.win = win;
        this.openTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOpen");
        this.closeTimeNoticeText = LanguageConfigManager.Instance.getLanguage("LuckyCardContent_timeOver");
        datas = new List<LimitCollectSample>();
        NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(notice.sid);
        sids = (sample.content as SidNoticeContent).sids;
        activeTime = ActiveTime.getActiveTimeByID(this.notice.getSample().timeID);
        setNoticeOpenTime();
        updateData(sids);
    }

    private void updateData(int [] sids) {
        FPortManager.Instance.getFPort<NoticeActiveGetFPort>().access(sids, updateUI);
    }

	private void updateUI(){
        foreach (int sid in sids)
        {
            if (NoticeActiveManagerment.Instance.getActiveInfoBySid(sid) != null)
                datas.Add(NoticeActiveManagerment.Instance.getActiveInfoBySid(sid) as LimitCollectSample);
        }
		initContent ();
        refreshNoticeTime();
        startTimer();
	}

	private void initContent(){
		Utils.RemoveAllChild (content.transform);
        foreach (LimitCollectSample item in datas)
        {
			GameObject go = Instantiate(itemPrefab) as GameObject;
			go.transform.parent = content.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			LimitCollectItemUI view = go.GetComponent<LimitCollectItemUI>();
			view.initUI(item,win);
		}
		content.Reposition ();
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
                timeText.text = closeTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainCloseTime));
            }
            else
            {

                timeText.gameObject.SetActive(false);
                timer.stop();
                timer = null;
                //时间到后所有按钮置灰
                LimitCollectItemUI[] items = content.GetComponentsInChildren<LimitCollectItemUI>();
                for (int i = 0; i < items.Length; i++)
                {
                    items[i].receiveButton.disableButton(true);
                }
                UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("notice37"));
            }
        }
        /** 还没开启 */
        else
        {
            timeText.gameObject.SetActive(true);
            timeText.text = openTimeNoticeText.Replace("%1", TimeKit.timeTransformDHMS(remainTime));
        }
    }

    void OnDisable() {
        if (timer != null)
        {
            timer.stop();
            timer = null;
        }
    }
}

public class LimitCollectCondition {
    /** 需要的道具类型 */
    public PrizeSample prize;
    /** 需要的 */
    public int need;
    /** 已经收集到的 */
    public int collected;
}
