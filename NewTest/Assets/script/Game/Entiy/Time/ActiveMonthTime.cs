using UnityEngine;
using System;
using System.Collections;

public class ActiveMonthTime:ActiveTime
{
	//周几  必须从小到大
	private int[] monthDay;

	public ActiveMonthTime (TimeInfoSample sample):base(sample)
	{
		monthDay = sample.mainTimeInfo;
	}
	//计算出当前最近的一次活动时间(即将开启，或者已经在活动时间内)
	public override void computeTime (int time)
	{
		base.computeTime (time);
		DateTime dt = TimeKit.getDateTime (time);
		int currTime = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
		int morning = time - currTime;//当天凌晨
		int nowDay = dt.Day;//当前是几号
		if (nowDay > monthDay [monthDay.Length - 1]) //如果已经大于最大号，大活动过期
			isFinish = true;
		else {
			foreach (int day in monthDay) {
				if (day >= nowDay) {
					startTime = morning + (day - nowDay) * 86400;
					endTime = startTime + 86400;
					return;
				}
			}
		}
	}
	
	//计算出当前最近的一次具体活动时间
	public override void computeDetailTime (int time)
	{
		base.computeDetailTime (time);
		//重新检测一次
		if (time >= endTime && endTime != 0) {
			computeTime(time);
		}
		if (isFinish)
			return;
		//走到这里，表示可能在大活动时间内，但是有可能小活动时间耗完了
		TimeDescript[] timeDes = timeSample.smallTimeDescript;
		DateTime dt = TimeKit.getDateTime (time);
		int currTime = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
		int morning = time - currTime;//当天凌晨
		if (timeDes == null) {
			detailStartTime = startTime;
			detailEndTime = endTime;
		} else {
			//如果有小条目的循环时间为0，那么必定所有小条目都是不循环的
			if (timeDes [0].cycleTime == 0) {
				//直接取时间最近的一次
				TimeDescript temp = null, small = null;
				foreach (TimeDescript des in timeDes) {
					if (des.startTime <= currTime && currTime <= des.startTime + des.durationTime) {
						detailStartTime = morning + des.startTime;
						detailEndTime = morning + des.startTime + des.durationTime;
						return;
					} else if (currTime < des.startTime && (temp == null || des.startTime < temp.startTime))
						temp = des;
					//记录最小的备用，因为会纯在
					if (small == null || des.startTime < small.startTime)
						small = des;
				}
				//等于null 说明小时间已过期 就要取下一次的 这里返回下次时间可能大活动进入休息了
				if (temp == null) {
					morning = getNextMonthDay (morning);
					detailStartTime = morning + small.startTime;
					detailEndTime = morning + small.startTime + small.durationTime;
				} else {
					detailStartTime = morning + temp.startTime;
					detailEndTime = morning + temp.startTime + temp.durationTime;
				}
			} else {
				int nextTime;
				TimeDescript temp = null, small = null;
				foreach (TimeDescript des in timeDes) {
					nextTime = des.startTime + (currTime - des.startTime) / (des.durationTime + des.cycleTime) * (des.durationTime + des.cycleTime);
					//小活动时间已经进入循环时间
					if (nextTime <= currTime && currTime <= nextTime + des.durationTime) {
						detailStartTime =morning + nextTime;
						detailEndTime =morning + nextTime + des.durationTime;
						return;
					} else if (currTime < nextTime && (temp == null || des.startTime < temp.startTime))
						temp = des;
					//记录最小的备用，因为会纯在
					if (small == null || des.startTime < small.startTime)
						small = des;
				}
				//等于null 说明小时间已过期 就要取下一次的
				if (temp == null) {
					morning = getNextMonthDay (morning);
					detailStartTime = morning + small.startTime;
					detailEndTime = morning + small.startTime + small.durationTime;
				} else {
					nextTime = temp.startTime + (currTime - temp.startTime) / (temp.durationTime + temp.cycleTime) * (temp.durationTime + temp.cycleTime);
					detailStartTime = morning + nextTime;
					detailEndTime = morning + nextTime + temp.durationTime;
				}
			}
		}
	}
	
	//获得下个具体星期时间
	private int getNextMonthDay (int morning)
	{
		DateTime dt = TimeKit.getDateTime (morning);
		int nowDay = dt.Day; //当前是几号
		if (nowDay > monthDay [monthDay.Length - 1]) { //如果已经大于最大周，大活动过期
			isFinish = true;
			return morning;
		} else {
			foreach (int day in monthDay) {
				if (day > nowDay) {
					return morning + (day - nowDay) * 86400;
				}
			}
			isFinish = true;
			return morning;
		}
	}
}

