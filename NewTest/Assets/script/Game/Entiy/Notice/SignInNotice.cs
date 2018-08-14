using UnityEngine;
using System.Collections.Generic;

/**
 * 签到活动
 * @author hzh
 * */
public class SignInNotice:Notice
{
    public SignInNotice(int sid) : base(sid)
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
        return now >= activeTime.getStartTime() && now <= activeTime.getDetailEndTime()&& hasInfo();
    }
    private bool hasInfo() {
        int ssid = StringKit.toInt(sid + "" + ServerTimeKit.getCurrentMonth());
        SignInSample sample = SignInSampleManager.Instance.getSignInSampleBySid(ssid);
        if (sample == null)
            return false;
        return true;
    }
}

