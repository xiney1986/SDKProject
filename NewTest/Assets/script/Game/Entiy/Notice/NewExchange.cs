using System;

public class NewExchange:Exchange
{

	public NewExchange (int sid, int num):base(sid,num)
	{
		this.sid = sid;
		this.num = num;
		this.exType = getExchangeSample ().exType;
	}

	public NewExchange (int sid, int num, int exType):base(sid,num,exType)
	{
		this.sid = sid;
		this.num = num;
		this.exType = exType;
	}

	public int timeID;
	private ActiveTime activeTime;

	public void initTime ()
	{
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (timeID);
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
