using UnityEngine;
using System;
using System.Collections;

public class ActiveWeekTime:ActiveTime
{
	//周几  必须从小到大
	private int[] weekDay;

	public ActiveWeekTime (TimeInfoSample sample):base(sample)
	{
		weekDay = sample.mainTimeInfo;
	}

	public override void initTime (int time)
	{
//		base.initTime (time);
		computeTime (time);
		computeDetailTime (startTime);
		int nextTime = getNextTime (time);
		if (nextTime > 0)
			computeDetailTime2(nextTime);
	}

	//如果具体时间已经过期，取下一个星期几的时间
	private int getNextTime(int time)
	{
		int now = ServerTimeKit.getSecondTime ();
		//当前时间有效
		if (now < detailEndTime)
			return -1;
		DateTime dt = TimeKit.getDateTime (time);
		int currTime = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
		int morning = time - currTime;//当天凌晨
		int nowWeek = TimeKit.getWeekCHA (dt.DayOfWeek);//当前是周几
		if (nowWeek >= weekDay [weekDay.Length - 1]) { //如果已经大于最大周，大活动过期
			return morning + (7 - nowWeek + weekDay[0]) * 86400;
		}
		else {
			foreach (int day in weekDay) {
				if (day > nowWeek) {
					return morning + (day - nowWeek) * 86400;
				}
			}
		}
		return -1;
	}

	//计算出当前最近的一次活动时间(即将开启，或者已经在活动时间内)
	public override void computeTime (int time)
	{
		base.computeTime (time);
		DateTime dt = TimeKit.getDateTime (time);
		int currTime = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
		int morning = time - currTime;//当天凌晨
		int nowWeek = TimeKit.getWeekCHA (dt.DayOfWeek);//当前是周几
		if (nowWeek > weekDay [weekDay.Length - 1]) { //如果已经大于最大周，大活动过期
			startTime = morning + (7 - nowWeek + weekDay[0]) * 86400;
			endTime = startTime + 86400;
		}
		else {
			foreach (int day in weekDay) {
				if (day >= nowWeek) {
					startTime = morning + (day - nowWeek) * 86400;
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
		computeDetailTime2 (time);
	}

	private void computeDetailTime2(int time)
	{
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
					morning = getNextWeekDay (morning);
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
						detailStartTime = morning + nextTime;
						detailEndTime = morning + nextTime + des.durationTime;
						return;
					} else if (currTime < nextTime && (temp == null || des.startTime < temp.startTime))
						temp = des;
					//记录最小的备用，因为会纯在
					if (small == null || des.startTime < small.startTime)
						small = des;
				}
				//等于null 说明小时间已过期 就要取下一次的
				if (temp == null) {
					morning = getNextWeekDay (morning);
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
	private int getNextWeekDay (int morning)
	{
		DateTime dt = TimeKit.getDateTime (morning);
		int nowWeek = TimeKit.getWeekCHA (dt.DayOfWeek);//当前是周几
		if (nowWeek > weekDay [weekDay.Length - 1]) { //如果已经大于最大周，大活动过期
			isFinish = true;
			return morning;
		} else {
			foreach (int day in weekDay) {
				if (day > nowWeek) {
					return morning + (day - nowWeek) * 86400;
				}
			}
			isFinish = true;
			return morning;
		}
	}

}

