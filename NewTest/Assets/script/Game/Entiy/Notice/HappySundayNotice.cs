using UnityEngine;
using System.Collections.Generic;

public class HappySundayNotice : Notice
{
	public ActiveTime activeTime;

    public HappySundayNotice(int sid) : base(sid)
    { 
        
    }

    public override bool isValid()
    {
        //bool isShow = false;
 //       System.Collections.Hashtable table = HappySundaySampleManager.Instance.samples;
//        System.DateTime date = TimeKit.getDateTime(ServerTimeKit.getSecondTime());
//        foreach (System.Collections.DictionaryEntry item in table)
//        {
//            HappySundaySample sample = item.Value as HappySundaySample;
//            if (sample.Week == (int)date.DayOfWeek)
//            {
//                isShow = true;
//                break;
//            }
//        }
        //if (!isShow) return false;
		//TODO   取时间
		activeTime = ActiveTime.getActiveTimeByID (getSample ().timeID);
		if (activeTime.getIsFinish ())
			return false;
		if(UserManager.Instance.self.getUserLevel() < getSample().levelLimit)
			return false;
		return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
		if(activeTime.getStartTime()>ServerTimeKit.getSecondTime())
			return false;
        int onlineDay = (ServerTimeKit.getSecondTime() - ServerTimeKit.onlineTime) / 3600 / 24;
        if (HappySundaySampleManager.Instance.getDataBySid(1).OnlineDay > onlineDay) return false;
        return true;
    }

	/// <summary>
	/// 得到开始时间
	/// </summary>
	public string getStartTime()
	{
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (getSample().timeID);
		if (activeTime == null) {
			activeTime = ActiveTime.getActiveTimeByType (tsample);
			//activeTime.initTime (ServerTimeKit.getSecondTime ());
		} else {
			activeTime.doRefresh();
		}
		//具体开始时间
		int startTime = activeTime.getStartTime ();//+ServerTimeKit.onlineTime;
		System.DateTime date =	TimeKit.getDateTime(startTime);
		return LanguageConfigManager.Instance.getLanguage("s0125",date.Month.ToString(),date.Day.ToString());

	}
	/// <summary>
	/// 得到结束时间
	/// </summary>
	public string getEndTime()
	{
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (getSample().timeID);
		if (activeTime == null) {
			activeTime = ActiveTime.getActiveTimeByType (tsample);
			//activeTime.initTime (ServerTimeKit.getSecondTime ());
		} else {
			activeTime.doRefresh();
		}
		//具体结束时间
		int endTime = activeTime.getEndTime ();//+ServerTimeKit.onlineTime;

	    System.DateTime date =	TimeKit.getDateTime(endTime);
		return LanguageConfigManager.Instance.getLanguage("s0125",date.Month.ToString(),date.Day.ToString());
	}

}

