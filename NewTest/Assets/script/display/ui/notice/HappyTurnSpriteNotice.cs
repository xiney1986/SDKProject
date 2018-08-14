using UnityEngine;
using System.Collections;

public class HappyTurnSpriteNotice : Notice {

	public ActiveTime activeTime;
	
	public HappyTurnSpriteNotice (int sid):base(sid) {
		this.sid = sid;
	}
	/** 是否有效 */
	public override bool isValid () {
		activeTime = ActiveTime.getActiveTimeByID (getSample ().timeID);
		if (activeTime.getIsFinish ())
			return false;
		return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
	}
	
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
}
