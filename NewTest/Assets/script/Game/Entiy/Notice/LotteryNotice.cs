using UnityEngine;
using System.Collections.Generic;

public class LotteryNotice : Notice
{
	public LotteryNotice (int sid):base(sid)
	{
		this.sid = sid;
	}
	public override bool isValid (){
		if (UserManager.Instance.self.getUserLevel () >= CommandConfigManager.Instance.getLotteryData().limitLv)
			return true;
		else
			return false;
	}
	public bool isActivityOpen()
	{
		int day = TimeKit.getWeekCHA(TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime()).DayOfWeek);
		for(int i=0;i<CommandConfigManager.Instance.getLotteryData().openTime.Length;i++)
		{
			if(day == CommandConfigManager.Instance.getLotteryData().openTime[i] && ServerTimeKit.getCurrentSecond()<CommandConfigManager.Instance.getLotteryData().selectNumEndTime)
			{
				return true;
			}
		}
		return false;
	}
}

