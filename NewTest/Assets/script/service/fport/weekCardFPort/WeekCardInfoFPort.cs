using UnityEngine;
using System.Collections;

public class WeekCardInfoFPort : BaseFPort
{
	private CallBack callBack;


	public void WeekCardInfoAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.WEEKCARD_INFO);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType msgInfo = message.getValue ("msg") as ErlType;

		if(msgInfo is ErlArray)// 买过//
		{
			ErlArray infoArr = msgInfo as ErlArray;
			WeekCardInfo.Instance.endTime = StringKit.toInt(infoArr.Value[0].getValueString());
			if(ServerTimeKit.getSecondTime() > WeekCardInfo.Instance.endTime)// 已过期//
			{
				WeekCardInfo.Instance.weekCardState = WeekCardState.over;
			}
			else
			{
				WeekCardInfo.Instance.weekCardState = WeekCardState.open;
			}
			WeekCardInfo.Instance.recevieState = StringKit.toInt(infoArr.Value[1].getValueString());
		}
		else// 没有买过//
		{
			WeekCardInfo.Instance.weekCardState = WeekCardState.not_open;
		}

		if(callBack != null)
		{
			callBack();
		}
	}

}

public class WeekCardInfo
{
	private WeekCardInfo info;
	public int endTime;
	public int recevieState; 
	public int weekCardState;
	public long loginTime = ServerTimeKit.getLoginTime();// 登录时间//
	
	public static WeekCardInfo Instance
	{		
		get{return SingleManager.Instance.getObj("WeekCardInfo") as WeekCardInfo;}
	}
}

public class WeekCardRecevieState
{
	public static int recevie = 1;// 可领取//
	public static int recevied = 0;// 已领取//
}
public class WeekCardState
{
	public static int open = 1;// 买过//
	public static int not_open = 0;// 没买//
	public static int over = 2;// 过期//
}
