using UnityEngine;
using System.Collections;

/**
 * 公告实体类
 * @author 汤琦
 * */
public class Notice
{
	public Notice(int sid)
	{
		this.sid = sid;
	}

	public int sid = 0;//sid
	public int readed = 0;//0 未读 1 已读
	public int index = 0;//显示排序
	public void updateRead(int value)
	{
		this.readed = value;
	}

	public NoticeSample getSample()
	{
		return NoticeSampleManager.Instance.getNoticeSampleBySid(sid);
	}

	//true表示有时间限制
	public bool isTimeLimit()
	{
		return getSample().timeLimit[0] > 0;
	}

	/// <summary>
	/// 获取显示时间的开始时间与结束时间(与getTimeLimit区别是此方法不管中途活动开关情况 ) 
	/// 调用此方法前必须先判断空的情况
	/// </summary>
	/// <returns>The show time limit.</returns>
	public virtual int[] getShowTimeLimit()
	{
		if (getSample().timeLimit.Length == 3)
			return new int[] { getSample().timeLimit[1], getSample().timeLimit[2] };
		return null;
	}

	//获得开始时间和结束时间数组
	public virtual int[] getTimeLimit()
	{
		if (getSample().timeLimit.Length == 3)
			return new int[] { getSample().timeLimit[1], getSample().timeLimit[2] };
		return null;
	}

	//是否处于有效时间 true表示当前处于有效时间
	public virtual bool isInTimeLimit()
	{
		int[] time = getTimeLimit();
		//无时间限制
		if (time == null)
			return true;
		int now = ServerTimeKit.getSecondTime();
		if (time[0] == 0 && time[1] == 0) //这个情况也是无时间限时
			return true;
		return time[0] < now && now < time[1];
	}

	//true表示有效 具体活动子类实现
	public virtual bool isValid()
	{
		return true;
	}

	public override string ToString()
	{
		string timeInfo = "";
		ActiveTime activeTime = ActiveTime.getActiveTimeByID(getSample().timeID);
		if (activeTime != null)
		{
			timeInfo = activeTime.ToString();
            Log.info("下次开始" + TimeKit.getDateTime(activeTime.getDetailStartTime()).ToString("yyyy-MM-dd HH:mm:ss"));
        }
		else
		{
			timeInfo = "永久";
		}
		return base.ToString() + "\t" + getSample().sid + "\t" + getSample().name + "\t" + getSample().activiteDesc + "\t" + timeInfo + "\t" + getSample().content;
	}
}

