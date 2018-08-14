using UnityEngine;
using System.Collections.Generic;

/**
 * 新的充值公告
 * @author hzh
 * */
public class NewRechargeNotice:Notice
{

	public NewRechargeNotice (int sid):base(sid)
	{
		this.sid = sid;
	}

	private ActiveTime activeTime;

	public override int[] getShowTimeLimit ()
	{
		activeTime = getActiveTime ();
		if (activeTime.getIsFinish ())
			return null;
		return new int[] {
			activeTime.getStartTime (),
			activeTime.getEndTime ()
		};
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
    public bool isEnding()
    {
        return getActiveTime().getIsFinish();
    }
	
	public override bool isInTimeLimit ()
	{
		activeTime = getActiveTime ();
		if (activeTime.getIsFinish ())
			return false;
		int now = ServerTimeKit.getSecondTime ();
		return activeTime.getDetailStartTime () < now && now < activeTime.getDetailEndTime ();
	}
	
	public ActiveTime getActiveTime ()
	{
		if (activeTime == null) {
			activeTime = ActiveTime.getActiveTimeByID (getSample ().timeID);
		} else {
			activeTime.doRefresh ();
		}
		return activeTime;
	}

	public override bool isValid ()
	{
		//显示规则策划定
		activeTime = getActiveTime ();
		if (activeTime.getIsFinish ())
			return false;
		if (activeTime.getEndTime () == 0)
			return true;
		int now = ServerTimeKit.getSecondTime ();
		return activeTime.getPreShowTime () < now && (now < activeTime.getEndTime ());
	}
}

