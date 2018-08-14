using UnityEngine;
using System.Collections;

public class ActiveTime
{
	public const int TYPEOF_ABSOLUTE = 1;//绝对时间
	public const int TYPEOF_ONLINE = 2;//自上线后的时间
	public const int TYPEOF_WEEK = 3;//按周来循环
	public const int TYPEOF_MONTH = 4;//按月来循环

	//大活动开始时间
	protected int startTime;
	//大活动结束时间
	protected int endTime;
	//预热时间，相对于大活动时间
	protected int preheatingTime;
	//具体开始时间
	protected int detailStartTime = -1;
	//具体结束时间
	protected int detailEndTime = -1;
	//时间条目样本
	protected TimeInfoSample timeSample;
	//是否已经完成所有可能的时间(true=不满足显示条件)
	protected bool isFinish;

	public ActiveTime (TimeInfoSample sample)
	{
		timeSample = sample;
		preheatingTime = sample.preheatingTime;
	}

	public virtual void initTime (int time)
	{
		computeOnLineTime (time);
        if (Log.useUnityLog)
        {
            if (isFinish)
                return;
        }
        computeTime (time);
		computeDetailTime (time);
        if(computeOnLineEndTime(time))isFinish=true;
	}
    public bool computeOnLineEndTime(int time) {//time 当前时间
        int openServerTime = ServerTimeKit.onlineTime;//服务器的开服时间
        if (timeSample.onLineSecondTime[1] <= 0) {//没有开服时间多少天不开启的限制
            if(!(timeSample.onLineSecondTime[0]==0)){//没有开服时间多少天开始的限制(如果没有配置 ==0 不能判断是否关闭了)对有开服时间限制的处理
                if (time < timeSample.onLineSecondTime[0] + openServerTime) return true;//如果当前时间比开服时间+开服多少天还小 还没有开启 
                else {//现在的时间在开服多少天以后开启之后（有开启的绝对时间）
                    if (startTime != 0) {
                        if (time < startTime)return true;
                        if (endTime != 0) {
                            if (time > endTime) return true;
                        }
                    }
                }
            }
        } else {//有开服时间多少天不开启的时间限制（有这个情况就必然会配置绝对开启时间和关闭时间）
            if (!(openServerTime + timeSample.onLineSecondTime[0] <= endTime && openServerTime + timeSample.onLineSecondTime[1] >= startTime)) return true;
        }
        return false;
    }

	//计算活动条目时间
	public virtual void computeTime (int time)
	{

	}

	// 计算开服时间
	public void computeOnLineTime(int time)
	{
		bool isCheck=checkOnLineTime (time);
		if (!isCheck)
			isFinish = true;
	}

    /** 校验开服时间是否满足(false不满足) */
    public bool checkOnLineTime(int time) {
        if (timeSample.onLineSecondTime[0] == 0)
            return true;
        if (time < timeSample.onLineSecondTime[0] + ServerTimeKit.onlineTime)
            return false;
        return true;
    }


	//计算活动具体小条目时间
	public virtual void computeDetailTime (int time)
	{
		
	}

	public int getStartTime ()
	{
		return startTime;
	}

	public int getEndTime ()
	{
		return endTime;
	}

	public int getDetailStartTime ()
	{
		return detailStartTime;
	}
	
	public int getDetailEndTime ()
	{
		return detailEndTime;
	}

	public bool getIsFinish ()
	{
		return isFinish;
	}

	public int getPreShowTime ()
	{
		return startTime - preheatingTime;
	}

	public void doRefresh ()
	{
		int now = ServerTimeKit.getSecondTime ();
		if ((now > detailEndTime && detailEndTime > 0) || (now > endTime && endTime > 0)||checkOnLineTime(now))
			initTime (now);
	}

	//根据时间条目类型取活动时间类型
	public static ActiveTime getActiveTimeByType (TimeInfoSample sample)
	{
		if (sample == null)
			return null;
		int type = sample.type;
		if (type == ActiveTime.TYPEOF_ABSOLUTE)
			return new ActiveAbsTime (sample);
		else if (type == ActiveTime.TYPEOF_ONLINE)
			return new ActiveOnlineTime (sample);
		else if (type == ActiveTime.TYPEOF_WEEK)
			return new ActiveWeekTime (sample);
		else if (type == ActiveTime.TYPEOF_MONTH)
			return new ActiveMonthTime (sample);
		return null;
	}

	public static ActiveTime getActiveTimeByID (int activeID)
	{
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (activeID);
		if (tsample == null)
			return null;
		ActiveTime activeTime = getActiveTimeByType (tsample);
		activeTime.initTime (ServerTimeKit.getSecondTime ());
		return activeTime;
	}

    public string GetTime(int second)
    {
        int day = second / 86400;
        int hour = second % 86400 / 3600;
        int minute = second % 86400 % 3600 / 60;
        return day + "天" + hour + "小时" + minute + "分钟";
    }
}

