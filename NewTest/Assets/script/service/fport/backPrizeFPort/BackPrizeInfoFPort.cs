using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BackPrizeInfoFPort : BaseFPort
{

	private CallBack callBack;
	public const int tottalLoginDays = 7;// 总共登录天数//

	public void BackPrizeLoginInfoAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.BACKPRIZE_LOGININFO);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType msgInfo = message.getValue ("msg") as ErlType;
		if(msgInfo is ErlArray)// 回归登录信息，活动已开启//
		{
			BackPrizeLoginInfo.Instance.backPrizeIsActive = true;
			ErlArray arr = msgInfo as ErlArray;
			ErlArray arrDays;

			BackPrizeLoginInfo.Instance.loginDays = StringKit.toInt(arr.Value[0].getValueString());
			arrDays = arr.Value[1] as ErlArray;
			if(BackPrizeLoginInfo.Instance.receivedDays == null)
			{
				BackPrizeLoginInfo.Instance.receivedDays = new List<int>();
			}
			int day = 0;
			for(int i=0;i<arrDays.Value.Length;i++)
			{
				day = StringKit.toInt(arrDays.Value[i].getValueString());
				if(!BackPrizeLoginInfo.Instance.receivedDays.Contains(day))
				{
					BackPrizeLoginInfo.Instance.receivedDays.Add(day);
				}
			}
			BackPrizeLoginInfo.Instance.endTimes = StringKit.toInt(arr.Value[2].getValueString());

			BackPrizeLoginInfo.Instance.prizeList = setBackPrizeList();

			if(BackPrizeLoginInfo.Instance.receivedDays.Count == tottalLoginDays)// 所有奖励都领完入口关闭//
			{
				BackPrizeLoginInfo.Instance.backPrizeIsActive = false;
			}
		}
		else// 回归活动未开启//
		{
			BackPrizeLoginInfo.Instance.backPrizeIsActive = false;
		}
		if(callBack != null)
		{
			callBack();
		}
	}

	public List<BackPrize> setBackPrizeList()
	{
		List<BackPrize> list = BackPrizeConfigManager.Instance.list;
		if(BackPrizeLoginInfo.Instance.loginDays <= tottalLoginDays)
		{
			for(int i=0;i<BackPrizeLoginInfo.Instance.loginDays;i++)
			{
				if(BackPrizeLoginInfo.Instance.receivedDays.Contains(i+1))
				{
					list[i].isRecevied = BackPrizeRecevieType.RECEVIED;
				}
				else
				{
					list[i].isRecevied = BackPrizeRecevieType.RECEVIE;
				}
			}
		}
		else
		{
			for(int i=0;i<list.Count;i++)
			{
				if(BackPrizeLoginInfo.Instance.receivedDays.Contains(i+1))
				{
					list[i].isRecevied = BackPrizeRecevieType.RECEVIED;
				}
				else
				{
					list[i].isRecevied = BackPrizeRecevieType.RECEVIE;
				}
			}
		}

		return list;
	}

}

// 回归玩家登录信息//
public class BackPrizeLoginInfo
{
	private BackPrizeLoginInfo loginInfo;
	public int loginDays;// 登录天数//
	public List<int> receivedDays = new List<int>();// 已领取的天数//
	public int endTimes;// 双倍经验剩余时间//
	public bool backPrizeIsActive = false;// 回归活动开启标志//
	public List<BackPrize> prizeList = new List<BackPrize>();// 奖励列表//
	public long secondDayTime = 0;// 处理跨天用//
	public long loginTime = ServerTimeKit.getLoginTime();// 登录时间//

	public static BackPrizeLoginInfo Instance
	{		
		get{return SingleManager.Instance.getObj("BackPrizeLoginInfo") as BackPrizeLoginInfo;}
	}

	public long getSecondDayTime(long loginTime)
	{
		DateTime dt_loginTime = TimeKit.getDateTimeMillis(loginTime);
		DateTime dt_secondDayTime = new DateTime(dt_loginTime.Year,dt_loginTime.Month,dt_loginTime.Day,23,59,59);
		secondDayTime = TimeKit.getTimeMillis(dt_secondDayTime) + 1000;
		DateTime dt = TimeKit.getDateTimeMillis(secondDayTime);

		return secondDayTime;
	}
}
