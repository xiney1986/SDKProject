using System;

public class DateKit
{
	public static string toString (int seconds)
	{
		int hour = (int)(seconds / 3600);
		int minute = ((int)(seconds / 60)) % 60;
		int second = seconds % 60;
		if (hour > 0)
			return (hour > 10 ? hour.ToString () : "0" + hour) + (minute > 10 ? minute.ToString () : "0" + minute);
		else
			return (minute > 10 ? minute.ToString () : "0" + minute) + (second > 10 ? second.ToString () : "0" + second);
	}
	
	/** 是否在同一天 */
	public static bool isInSameDay(int secondTime1,int secondTime2)
	{
		DateTime dateTime1=TimeKit.getDateTimeMin(secondTime1);
		int year1=dateTime1.Year;
		int day1=dateTime1.DayOfYear;
		DateTime dateTime2=TimeKit.getDateTimeMin(secondTime2);
		int year2=dateTime2.Year;
		int day2=dateTime2.DayOfYear;
		return year1==year2&&day1==day2;
	}
	
	/** 距离上次关键时间长度 */
	public static string getLastTime (int sendTime, int nowTime)
	{
		string dateDiff = "";
		try {
			TimeSpan ts1 = new TimeSpan (TimeKit.getDateTimeMin (nowTime).Ticks);
			TimeSpan ts2 = new TimeSpan (TimeKit.getDateTimeMin (sendTime).Ticks);
			TimeSpan ts = ts1.Subtract (ts2).Duration ();

			if (ts.Days >= 365) {
				return (ts.Days / 365).ToString () + "Y";
			} else if (ts.Days >= 30 && ts.Days < 365) {
				return (ts.Days / 30).ToString () + "M";
			} else if (ts.Days >= 1 && ts.Days < 30) {
				return (ts.Days / 1).ToString () + "D";
			} else if (ts.Days < 1 && ts.Hours >= 1) {
				return (ts.Hours / 1).ToString () + "H";
			} else if (ts.Hours < 1 && ts.Minutes >= 1) {
				return (ts.Minutes / 1).ToString () + "M";
			} else if (ts.Minutes < 1) {
				return (ts.Seconds / 1).ToString () + "S";
			}
		} catch {
		}
		return dateDiff;
	}
}

