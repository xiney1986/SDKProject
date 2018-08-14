using System.Text.RegularExpressions;
using UnityEngine;
using System;
using System.Collections;
using System.Text;

public class ActiveAbsTime:ActiveTime
{
	//开始时间
	private int sTime;
	//持续时间
	private int duration;
	//循环时间
	private int cycleTime;

	public ActiveAbsTime (TimeInfoSample sample):base(sample)
	{
		sTime = sample.mainTimeInfo [0];
		duration = sample.mainTimeInfo [1];
		cycleTime = sample.mainTimeInfo [2];
	}

	//计算出当前最近的一次活动时间(即将开启，或者已经在活动时间内)
	public override void computeTime (int time)
	{
		base.computeTime (time);
		if (duration == 0) {
			startTime = sTime;
		} else if (cycleTime == 0) {
			startTime = sTime;
			endTime = sTime + duration;
			if (time >= endTime)
				isFinish = true;
		} else if (time < sTime) {
			startTime = sTime;
			endTime = sTime + duration;
		} else {
			int nextTime = sTime + (time - sTime) / (duration + cycleTime) * (duration + cycleTime);
			startTime = nextTime;
			endTime = nextTime + duration;
			//如果有问题
			if (time >= endTime) {
				startTime += cycleTime + duration;
				endTime += cycleTime + duration;
			}
		}
	}

	//计算出当前最近的一次具体活动时间
	public override void computeDetailTime (int time)
	{
		base.computeDetailTime (time);
		//重新检测一次
		if (time >= endTime && endTime != 0) {
			isFinish = true;
		}
        if (isFinish)
			return;
	    time = Math.Max(time, startTime);
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
					morning = getNextBigTime (morning);
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
					morning = getNextBigTime (morning);
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

	//调用这个方法，肯定是永久或者循环的活动
	private int getNextBigTime (int morning)
	{
		int t = morning + 86400;
		if (duration == 0 || t < endTime) {
			return t;
		} else {
			isFinish = true;
			return morning;
		}
	}

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("\"");
        Log.warning(timeSample.sid+","+ startTime+","+ endTime);
        if (startTime > endTime)
        {
            sb.Append("永久");
        }
        else
        {
            if (timeSample.onLineSecondTime[0] != 0)
            {
                int time = timeSample.onLineSecondTime[0];
                sb.Append("开始时间:");
                sb.Append(TimeKit.getDateTime(sTime).ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append("\n");
                sb.Append("持续时间:");
                sb.Append(GetTime(duration));
                sb.Append("\n");
                sb.Append("开服" + (time < 0 ? "前" : "后") + ":" + GetTime(time)+"生效");
            }
            else
            {
                sb.Append("开始时间:");
                sb.Append(TimeKit.getDateTime(startTime).ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append("\n");
                sb.Append("结束时间:");
                sb.Append(TimeKit.getDateTime(endTime).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        if (cycleTime != 0)
        {
            sb.Append("\n");

            sb.Append("循环时间:" + GetTime(cycleTime));
        }

        

        
       
        sb.Append("\"");
        return sb.ToString();
    }
}

