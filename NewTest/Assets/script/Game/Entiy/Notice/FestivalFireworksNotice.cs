using UnityEngine;
using System.Collections;

public class FestivalFireworksNotice : Notice {
	public ActiveTime activeTime;
	public FestivalFireworksNotice (int sid):base(sid)
	{
		this.sid = sid;
	}
	
	public override bool isValid () {
		//TODO   取时间
		activeTime = ActiveTime.getActiveTimeByID (getSample ().timeID);
		if (activeTime.getIsFinish ())
			return false;
		if(UserManager.Instance.self.getUserLevel() < getSample().levelLimit)
			return false;
		return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
	}
}
