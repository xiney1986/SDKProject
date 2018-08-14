using UnityEngine;
using System.Collections.Generic;

/**
 * 分享抽奖活动
 * @author 
 * */
public class ShareDrawNotice:Notice
{
    public ShareDrawNotice(int sid)
        : base(sid)
	{
		this.sid = sid;
	}

	private ActiveTime activeTime;

	public override bool isValid ()
	{
        NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(sid);
        activeTime = ActiveTime.getActiveTimeByID(sample.timeID);
		if (activeTime.getIsFinish ())
			return false;
		if (activeTime.getEndTime () == 0)
			return true;
		int now = ServerTimeKit.getSecondTime ();
        return now >= activeTime.getPreShowTime() && now <= activeTime.getDetailEndTime();
	}
}

