/**
 * Coypright 2013 by 刘耀鑫<xiney@youkia.com>.
 */
using System;


/**
 * @author 刘耀鑫
 */
public class TimeKit
{
 
	public const int DAY_SECONDS = 24 * 60 * 60;
	//目前后台时间都按1970,1,1 8点开始
//	private static readonly DateTime initTime = new DateTime (1970, 1, 1);
	private static readonly DateTime initTime2 = new DateTime (1970, 1, 1, 8, 0, 0);

	/** 时间的修正值 */
	private static long timeFix;

	/* static methods */
	/** 得到校正后时间，毫秒为单位 */
	public static long getMillisTime ()
	{
		return currentTimeMillis () - timeFix;
	}
	/** 得到校正后时间，秒为单位 */
	public static int getSecondTime ()
	{
		return (int)((currentTimeMillis () - timeFix) / 1000);
	} 
	
	/** 校正时间 */
	public static void resetTime (long time)
	{
		timeFix = currentTimeMillis () - time;
	}
	/** 将指定的毫秒数转换成秒数，毫秒数除1000 */
	public static int timeSecond (long timeMillis)
	{
		return (int)(timeMillis / 1000);
	}
	/** 将指定的秒数转换成毫秒数，秒数乘1000 */
	public static long timeMillis (long timeSecond)
	{
		return timeSecond * 1000;
	}

	
	//得到DateTime   从1970.1.1 8:00 开始 秒
	public static DateTime getDateTime (int time)
	{
		return initTime2.AddSeconds (time);
	}

	//得到DateTime   从1970.1.1 8:00 开始 毫秒
	public static DateTime getDateTimeMillis (long time)
	{
		return initTime2.AddMilliseconds (time);
	}
	
	//得到DateTime   从1970.1.1 8:00 开始 毫秒
	public static DateTime getDateTimeMin (int time)
	{
		return initTime2.AddMinutes (time / 60);
	}



	/// <summary>
	/// 通过指定datatime得到当前时间的时间戳
	/// </summary>
	public static long getTimeMillis (DateTime date)
	{
		return Convert.ToInt64 (date.Subtract (initTime2).TotalMilliseconds);
	}

	/// <summary>
	/// 得到当前时间的时间戳
	/// </summary>
	/// <returns>长整型时间</returns>
	private static long currentTimeMillis ()
	{
		return Convert.ToInt64 (DateTime.UtcNow.Subtract (initTime2).TotalMilliseconds);

	}  
	 
	//转换时间格式 单位:毫秒
	public static string timeTransform (double time)
	{
		time = time / 1000;
		string hours = ((int)(time / 3600)).ToString ();
		if (hours.Length == 1) {
			hours = "0" + hours;
		}
		string minutes = ((int)(time % 3600 / 60)).ToString ();
		if (minutes.Length == 1) {
			minutes = "0" + minutes;
		}
		string seconds = ((int)(time % 3600 % 60)).ToString ();
		if (seconds.Length == 1) {
			seconds = "0" + seconds;
		}
		return  hours + ":" + minutes + ":" + seconds;
	}

	//转换时间格式 单位:秒  
	public static string timeTransformDHMS (double time)
	{  
		int days = (int)(time / (3600 * 24));
		string dStr = "";
		if (days != 0)
			dStr = days + LanguageConfigManager.Instance.getLanguage ("s0018");	
		int hours = (int)(time % (3600 * 24) / 3600);
		string hStr = "";
		if (hours != 0)
			hStr = hours + LanguageConfigManager.Instance.getLanguage ("s0019");
		int minutes = (int)(time % (3600 * 24) % 3600 / 60);
		string mStr = "";
		if (minutes != 0)
			mStr = minutes + LanguageConfigManager.Instance.getLanguage ("s0020");
		
		int seconds = (int)(time % (3600 * 24) % 3600 % 60);
		string sStr = "";
		if (seconds != 0)
			sStr = seconds + LanguageConfigManager.Instance.getLanguage ("s0021");
		return dStr + hStr + mStr + sStr;
	}

    //转换时间格式 单位:分 
    public static string timeTransformDHM(double time) {
        int days = (int)(time / (3600 * 24));
        string dStr = "";
        if (days != 0)
            dStr = days + LanguageConfigManager.Instance.getLanguage("s0018");
        int hours = (int)(time % (3600 * 24) / 3600);
        string hStr = "";
        if (hours != 0)
            hStr = hours + LanguageConfigManager.Instance.getLanguage("s0019");
        int minutes = (int)(time % (3600 * 24) % 3600 / 60);
        string mStr = "";
        if (minutes != 0)
            mStr = minutes + LanguageConfigManager.Instance.getLanguage("s0020");

        int seconds = (int)(time % (3600 * 24) % 3600 % 60);
        return dStr + hStr + mStr;
    }

	public static string dateToFormat (int time, string format)
	{
		return getDateTime (time).ToString (format);
	}


	/// <summary>
	/// 算出该月的第一天
	/// </summary>
	public static int  firstDayOfMonth (DateTime date)
	{
		DateTime _date = date.AddDays (1 - date.Day);  
		return _date.Day;
	}

	public static int getWeekCHA (DayOfWeek dw)
	{   
		int week = 1;
		string str = dw.ToString ();
		switch (str) {   
		case "Monday":
			return 1;
		case "Tuesday":
			return 2;
		case "Wednesday":
			return 3;
		case "Thursday":
			return 4;
		case "Friday":
			return 5;   
		case "Saturday":
			return 6;
		case "Sunday":
			return 7;  
		}
		return 1;
	}
	/* constructors */
	private TimeKit ()
	{
	}
}
