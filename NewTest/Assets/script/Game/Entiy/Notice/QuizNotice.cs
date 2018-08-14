using UnityEngine;
using System;
using System.Collections;

public class QuizNotice : Notice
{
	private ActiveTime activeTime;

	public QuizNotice (int sid):base(sid)
	{
		this.sid = sid;
	}
	
	public override bool isValid ()
	{
		activeTime = ActiveTime.getActiveTimeByID (getSample().timeID);
		if (activeTime.getIsFinish ())
			return false;
		return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
	}

	/// <summary>
	/// 获得答题活动开放时间
	/// </summary>
	public string getOpenTimeDesc ()
	{
		SidNoticeContent content = getSample ().content as SidNoticeContent;
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (getSample().timeID);
		if (activeTime == null) {
			activeTime = ActiveTime.getActiveTimeByType (tsample);
			activeTime.initTime (ServerTimeKit.getSecondTime ());
		} else {
			activeTime.doRefresh();
		}
		//具体开始时间
		int startTime = activeTime.getStartTime ();
		//具体结束时间
		int endTime = activeTime.getEndTime ();
		DateTime dt = TimeKit.getDateTime (endTime);
		int currTime = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
		if (currTime == 0) {
			endTime -= 1;
		}

		if (DateKit.isInSameDay(startTime,endTime)) {
			return LanguageConfigManager.Instance.getLanguage ("notice11",TimeKit.dateToFormat (startTime, LanguageConfigManager.Instance.getLanguage ("notice04")));
		} else {
			return LanguageConfigManager.Instance.getLanguage ("notice02", TimeKit.dateToFormat (startTime, LanguageConfigManager.Instance.getLanguage ("notice04")),
		                                                   TimeKit.dateToFormat (endTime, LanguageConfigManager.Instance.getLanguage ("notice04")));
		}
	}
	
	
	/// <summary>
	/// 获得时间描述
	/// </summary>
	public string getTimeDesc ()
	{
		int nowTime = ServerTimeKit.getSecondTime ();
		ExamSample examSample = QuizManagerment.Instance.getExamSampleBySid (this);

		if (examSample == null) {
			return LanguageConfigManager.Instance.getLanguage ("notice_quiz01");//活动已经结束
		}
		SidNoticeContent content = getSample ().content as SidNoticeContent;
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (getSample().timeID);
		if (activeTime == null) {
			activeTime = ActiveTime.getActiveTimeByType (tsample);
			activeTime.initTime (ServerTimeKit.getSecondTime ());
		} else {
			activeTime.doRefresh();
		}
		//具体开始时间
		int detailStartTime = activeTime.getDetailStartTime ();
		//具体结束时间
		int detailEndTime = activeTime.getDetailEndTime ();
		
		if (detailStartTime < nowTime && detailEndTime > nowTime && examSample.getAwardType == 1) {
			return LanguageConfigManager.Instance.getLanguage ("notice09");//可以答题
		} else if (DateKit.isInSameDay (detailStartTime,nowTime) && detailStartTime > nowTime) {
			return LanguageConfigManager.Instance.getLanguage ("notice08", TimeKit.dateToFormat (detailStartTime, LanguageConfigManager.Instance.getLanguage ("notice05")));//尚未开启
		} else if (activeTime.getIsFinish ()) {
			return LanguageConfigManager.Instance.getLanguage ("notice_quiz01");//活动已经结束
		} else if (detailEndTime < nowTime) {
			return LanguageConfigManager.Instance.getLanguage ("notice_quiz01");//活动已经结束
		} else {
			return LanguageConfigManager.Instance.getLanguage ("notice_quiz01");//活动已经结束
		}
	}

	/// <summary>
	/// 能否答题
	/// </summary>
	public bool isCanAnswer ()
	{
		int nowTime = ServerTimeKit.getSecondTime ();
		ExamSample examSample = QuizManagerment.Instance.getExamSampleBySid (this);
		SidNoticeContent content = getSample ().content as SidNoticeContent;
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (getSample().timeID);
		if (activeTime == null) {
			activeTime = ActiveTime.getActiveTimeByType (tsample);
			activeTime.initTime (ServerTimeKit.getSecondTime ());
		} else {
			activeTime.doRefresh();
		}
		//具体开始时间
		int detailStartTime = activeTime.getDetailStartTime ();
		//具体结束时间
		int detailEndTime = activeTime.getDetailEndTime ();

		if (examSample == null) {
			if (detailStartTime < nowTime && detailEndTime > nowTime) {
				return true;//可以答题
			} else {
				return false;
			}
		} else {
			if (detailStartTime < nowTime && detailEndTime > nowTime && examSample.getAwardType == 1) {
				return true;//可以答题
			} else {
				return false;
			}
		}
	}
}
