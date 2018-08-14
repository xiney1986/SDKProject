using UnityEngine;
using System.Collections;

/**
 * 消费返利公告
 * @author hzh
 * */
public class ConsumeRebateNotice:Notice
{
	public ActiveTime activeTime;

	public ConsumeRebateNotice (int sid):base(sid)
	{
		this.sid = sid;
	}
	
	public override int[] getTimeLimit ()
	{
		activeTime = getActiveTime ();
		if (activeTime.getIsFinish ())
			return null;
		return new int[] {
			activeTime.getDetailStartTime (),
			activeTime.getDetailEndTime ()
		};
	}
	
	public override bool isInTimeLimit ()
	{
		activeTime = getActiveTime ();
		if (activeTime.getIsFinish ())
			return false;
		int now = ServerTimeKit.getSecondTime ();
		return activeTime.getDetailStartTime () < now && now < activeTime.getDetailEndTime ();
	}
	
	public void initTime ()
	{
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (sid);
		activeTime = ActiveTime.getActiveTimeByType (tsample);
		activeTime.initTime (ServerTimeKit.getSecondTime ());
	}
	
	public ActiveTime getActiveTime ()
	{
		if (activeTime == null) {
			initTime ();
		} else {
			activeTime.doRefresh ();
		}
		return activeTime;
	}

	public override bool isValid ()
	{
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (getSample ().timeID);
		if (activeTime == null) {
			activeTime = ActiveTime.getActiveTimeByType (tsample);
			activeTime.initTime (ServerTimeKit.getSecondTime ());
		}
		if (activeTime.getIsFinish ())
			return false;
		return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
	}
}

