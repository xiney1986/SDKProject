using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RebateDayInfo
{
	public const int RebateNoticeID = 40000000;
	public RebateDayInfo()
	{

	}

	public RebateDayInfo(int _dayID)
	{
		//NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(RebateNoticeID);
		NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(RebateInfoManagement.Instance.rebateNoticeID);
		int delayTime = sample.order;
		this.dayID = _dayID;
		startTime = getStartTime(_dayID,TimeConfigManager.Instance.getTimeInfoSampleBySid (sample.timeID).mainTimeInfo[0]);
		endTime = getEndTime(_dayID,startTime,delayTime);
	}
	public int rebateState = RebateState.NOT_JOIN;// 状态大分类//
	public long costDiamond;
	public long costGold;
	public long getDiamond;
	public long getGold;
	public int dayID;// 哪一天//
	public int startTime;// 开始时间//
	public int endTime;// 领奖时间//
	public int s_rebateState = S_RebateState.NONE;// 状态小分类,只有未领取才有此分类//

	public RebateDayInfo creatRebateDayInfo(ErlArray arr)
	{
		RebateDayInfo rdi=new RebateDayInfo ();
		rdi.bytesRead (0,arr);
		return rdi;
	}

	/** 序列化读取可变属性数据 */
	public void bytesRead (int j, ErlArray ea)
	{
		this.dayID = StringKit.toInt (ea.Value [j++].getValueString ());
		this.costDiamond = StringKit.toInt (ea.Value [j++].getValueString ());
		this.costGold = StringKit.toInt (ea.Value [j++].getValueString ());
		this.rebateState = StringKit.toInt (ea.Value [j++].getValueString ());
	}

	public int getEndTime(int dayID,int startTime,int delayTime)
	{
		return delayTime * 24 * 60 * 60 + startTime;
	}
	public int getStartTime(int dayID,int startTime)
	{
		return (dayID - 1) * 24 * 60 * 60 + startTime;
	}

}
public class RebateState
{
	public static int NOT_JOIN = -1;// 未参加//
	public static int RECEIVED = 1;// 已领取//
	public static int UN_RECEIVE = 0;// 未领取//
	public static int NOT_ON_TIME = 2;// 未到活动时间//
}
public class S_RebateState
{
	public static int NONE = 0; 
	public static int CAN_RECEIVE = 1;// 可领取//
	public static int WAIT_RECEIVE = 2;// 等待领取//
	public static int COLLECTING = 3;// 收集中//
}
 
