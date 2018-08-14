using System;
using System.Collections.Generic;
 
/**
 * 时间限制管理器
 * */
public class TimeLimitManagerment
{

	//获得绝对时间限制
	public static TimeLimit updateTimeLimit (TimeLimit tls, long[] times)
	{
		long now = ServerTimeKit.getSecondTime ();
		DateTime dt=TimeKit.getDateTimeMillis(now*1000);
        tls.today = (int)dt.DayOfWeek == 0 ? 7 : (int)dt.DayOfWeek;
		if (now < times [0]) {
			tls.type = TimeLimit.FRONT;
			tls.time = times [0] - now;
		} else if (now >= times [0] && now <= times [1]) {
			tls.type = TimeLimit.MIDDLE;
			tls.time = times [1] - now;
		} 
		return tls;
	}
}

//时间限制显示对象
public class TimeLimit
{
	public const int FRONT = 1;//前
	public const int MIDDLE = 2;//中
	CallBack _onOver;
	//超时回调
	public TimeLimit (CallBack onOver)
	{
		_onOver = onOver;
	}
	//默认找出下个时间点算开启时间
	public int type = TimeLimit.FRONT;
	public long time = 0;//时间差  
	public int today=0;

	public void timeOver ()
	{
		if (_onOver != null)
			_onOver ();
	}

	public string toString ()
	{ 
		if (type == TimeLimit.FRONT) { 
			return "[F85C5C]"+LanguageConfigManager.Instance.getLanguage ("s0169", timeTransform (time));
		} else if (type == TimeLimit.MIDDLE) {
			if (time < 0)
				return LanguageConfigManager.Instance.getLanguage ("s0040");
			return "[64ED6E]"+LanguageConfigManager.Instance.getLanguage ("s0170", timeTransform (time));
		} else {
			return "";
		}	
	}
	
	public string timeTransform (long time)
	{
		if (time > TimeKit.DAY_SECONDS * 3) {
			int day = (int)(time / TimeKit.DAY_SECONDS);
			return day + LanguageConfigManager.Instance.getLanguage ("s0018");
		} else if (time > TimeKit.DAY_SECONDS) { 
			int day = (int)(time / TimeKit.DAY_SECONDS);
			int hour = (int)((time - day * TimeKit.DAY_SECONDS) / (60 * 60));
			return day + LanguageConfigManager.Instance.getLanguage ("s0018") + hour + LanguageConfigManager.Instance.getLanguage ("s0019"); 
		} else {
			int hour = (int)(time / (60 * 60));
			int m = (int)((time - hour * 60 * 60) / 60);
			int s = (int)(time - hour * 60 * 60 - m * 60);
			return   hour + LanguageConfigManager.Instance.getLanguage ("s0019") + m + LanguageConfigManager.Instance.getLanguage ("s0020") + s + LanguageConfigManager.Instance.getLanguage ("s0021"); 
		}
	}
}

