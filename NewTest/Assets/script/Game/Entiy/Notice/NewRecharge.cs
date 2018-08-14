using System;

public class NewRecharge:Recharge
{

	public NewRecharge (int sid, int num):base(sid,num)
	{
		this.sid = sid;
		this.num = num;
	}
	
	public NewRecharge (int sid, int count, int num):base(sid,count,num)
	{
		this.sid = sid;
		this.num = num;
		this.count = count;
	}


	private ActiveTime activeTime;

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

	public override int getStartTime ()
	{
		return getActiveTime ().getDetailStartTime ();
	}
	
	public override int getEndTime ()
	{
		return getActiveTime ().getDetailEndTime ();
	}
}
