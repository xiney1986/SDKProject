using System;
 
public class Exchange
{
	private int[] time;

	public Exchange (int sid, int num, int exType)
	{
		this.sid = sid;
		this.num = num;
		this.exType = exType;
	}

	public Exchange (int sid, int num)
	{
		this.sid = sid;
		this.num = num;
		this.exType = getExchangeSample ().exType;
	}
	
	public int sid = 0;//兑换条目sid
	protected int num = 0;//兑换过的次数
	public int exType = 0;//兑换条目类型 


	public void setTime (int[] time)
	{
		this.time = time;
	}

	public virtual int getStartTime ()
	{
		if (time != null && time.Length == 2)
			return time [0];
		return 0;
	}

	public virtual int getEndTime ()
	{
		if (time != null) {
			if (time.Length == 2)
				return time [1];
			else
				return time [0];
		}
		return 0;
	}

	//是否显示
	//只有兑换次数用完才不需要显示
	public bool isShow ()
	{
		int num = getExchangeSample ().times;
		if (num == 0)
			return true;
		if (this.num >= num)
			return false;
		return true;
	}
	
	public void setNum (int num)
	{
		this.num = num;
	}
	
	public void addNum (int count)
	{
		num += count;
		
	}

	public int getNum ()
	{
		return num;
	}

	public int  getLastNum ()
	{
		if (getExchangeSample ().times <= 0)
			return int.MaxValue;
		int count = getExchangeSample ().times - num;
		
		if (count < 0)
			count = 0;
		return count;
		
	}
	
	public ExchangeSample getExchangeSample ()
	{
		return ExchangeSampleManager.Instance.getExchangeSampleBySid (sid);
	}

	/// <summary>
	/// 是否过期
	/// </summary>
	/// <param name="now">当前时间</param>
	public bool checkTimeOut (int now)
	{
		if (getStartTime () == 0 && getEndTime () == 0)
			return false;
		if (getStartTime () == 0 && getEndTime () > 0 && now < getEndTime ())
			return false;
		if (getStartTime () > 0 && getEndTime () == 0 && now > getStartTime ())
			return false;
		if (getStartTime () > 0 && getEndTime () > 0 && now > getStartTime () && now < getEndTime ())
			return false;
		return true;
	}

	//是否是限制兑换
	public bool isTimeLimit ()
	{
		time = NoticeManagerment.Instance.getExchangeTime (sid);
		if (time == null || time.Length < 2)
			return false;
		return true;
	}

	//是否已经过期 true表示过期
	public bool isTimeout (int now)
	{
		int[] time = NoticeManagerment.Instance.getExchangeTime (sid);
		if (time == null)
			return false;
		return now > time [1];
	}
}

public class ExchangeType
{
	public const int COMMON = 1;
	public const int NOTICE = 2;
	public const int BEAST = 3;
	public const int CARDSCRAP = 4;//卡片碎片
	public const int EQUIPSCRAP = 5;//装备碎片
	public const int MOUNT=6;//普通坐骑
	public const int BACTICMOUNT=7;//活动坐骑
    public const int MAGICWEAPON = 8;//秘宝碎片
    public const int WNCARD = 9;//万能卡兑换
}

