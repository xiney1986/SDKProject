using System;
using UnityEngine;

/**
 * 后台服务器时间工具
 * @author huangzhenghan
 * */
public class ServerTimeKit
{
    //服务器时间
    private static long loginTime;
    //登录成功消耗时间
    private static long costTime;
    //服务器开服时间
    public static int onlineTime;

    private ServerTimeKit()
    {

    }

    /** 初始化服务器时间 
		 * 前台延迟1000毫秒,确保后台数据刷新后
	 	*/
    public static void initTime(long _loginTime, float _costTime, int _onlineTime)
    {
        loginTime = _loginTime - 1000;
        costTime = (long)(_costTime * 1000);
        onlineTime = _onlineTime;
    }

    /** 获得当前服务器毫秒时间 */
    public static long getMillisTime()
    {
        if (Log.useUnityLog)
        {
            return loginTime + (long)(Time.realtimeSinceStartup * 1000) - costTime;
        }
        else
        {
            return TimeKit.getMillisTime();
        }
    }

    /** 获得当前服务器秒时间 */
    public static int getSecondTime()
    {
        if (Log.useUnityLog)
        {
            return (int)(getMillisTime() / 1000);
        }
        else
        {
            return TimeKit.getSecondTime();
        }
    }

    /** 获得服务器时间,即登录时间 */
    public static long getLoginTime()
    {
        return loginTime;
    }

    /**获得当前天数*/
    public static int getDayOfYear()
    {
        return TimeKit.getDateTimeMillis(getMillisTime()).DayOfYear;
    }
    /**获得当前月分的第几日*/
    public static int getDayOfMonth()
    {
        return TimeKit.getDateTimeMillis(getMillisTime()).Day;
    }
    public static int getYear()
    {
        return TimeKit.getDateTimeMillis(getMillisTime()).Year;
    }
    public static int getCurrentMonth()
    {
        return TimeKit.getDateTimeMillis(getMillisTime()).Month;
    }
    /** 得到DateTime */
    public static DateTime getDateTime()
    {
        return TimeKit.getDateTime(getSecondTime());
    }

    public static int getDataBySeconds(int seconds)
    {
        return TimeKit.getDateTime(seconds).Day;
    }
    /** 获得当天秒数,0点开始 */
    public static int getCurrentSecond()
    {
        return (getSecondTime() + (8 * 60 * 60)) % 86400;
    }
}


public class ServerWeekInfo
{
    public int dayOfWeek;
    public int[] startTime;
    public int[] endTime;
}