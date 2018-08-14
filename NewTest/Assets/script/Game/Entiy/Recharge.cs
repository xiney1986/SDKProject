using UnityEngine;
using System.Collections;

/**
 * 充值实体类
 * @author 汤琦
 * */
public class Recharge
{
	public Recharge (int sid, int num)
	{
		this.sid = sid;
		this.num = num;
	}

	public Recharge (int sid, int count, int num)
	{
		this.sid = sid;
		this.num = num;
		this.count = count;
	}
	
	public int sid = 0;//sid
	public int num = 0;//累积充值的充值金额-单笔充值的可领奖次数  - 单笔倍数返利的可领取次数
	public int count = 0;//累积充值已领取次数-单笔充值的已领取次数 - 单笔倍数返利的已领取次数
	private int[] time;

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
	public RechargeSample getRechargeSample() {
		return RechargeSampleManager.Instance.getRechargeSampleBySid (sid);
	}
	/// <summary>
	/// 修改充值实体数据
	/// </summary>
	/// <param name="count">添加的领取次数</param>
	/// <param name="delNum">扣除的可领奖次数</param>
	public void modifyRecharge(int count,int delNum) {
		addCount (count);
		RechargeSample sample = getRechargeSample();
		if(sample.reType==RechargeSample.RECHARGE) {
			deductNum (delNum);
		}
	}
	public void setNum (int num) {
		this.num = num;
	}
	public void deductNum (int value) {
		if (value > num)
			value = num;
		num -= value;
	}
	public void addCount (int count)
	{
		this.count += count;
	}
	public void setCount (int count)
	{
		this.count = count;
	}

	/** 累计充值的已领取次数是否满足 */
	public bool isRecharge () {
		return count < getRechargeSample ().count;
	}
	/** 累计充值金额是否满足 */
	public bool isComplete () {
		if (num >= getRechargeSample ().condition)
			return true;
		else 
			return false;
	}
	/** 单笔充值的已领取次数是否满足 */
	public bool isOneRecharge () {
		return count < getRechargeSample ().count;
	}
	/** 单笔充值的可领奖次数是否满足 */
	public bool isOneComplete () {
		return num > 0;
	}
    /** 获取单笔充值的可领奖次数 */
    public int getOneCompleteNum()
    {
        return num;
    }
    /// <summary>
    /// 单笔倍数返利是否可领取
    /// </summary>
    public bool isOneManyRechargeComplete()
    {
        return num > 0;
    }
    /// <summary>
    /// 单笔倍数返利是否已经领取
    /// </summary>
    public bool isOneManyRechargeReceived()
    {
        return count >= getOneManyRechargeMaxNum();
    }
    /// <summary>
    /// 单笔倍数返利是否可显示
    /// </summary>
    public bool isOneManyRechargeShow()
    {
        return count < getRechargeSample().count;
    }
    /// <summary>
    /// 单笔倍数返利最大可领取次数
    /// </summary>
    public int getOneManyRechargeMaxNum()
    {
        return getRechargeSample().count;
    }
	public int  getLastNum ()
	{
		if (getRechargeSample ().count <= 0)
			return 999;
		int currentCount = getRechargeSample ().count - count;
		
		if (currentCount < 0)
			currentCount = 0;
		return currentCount;
		
	}
	/// <summary>
	/// 是否过期
	/// </summary>
	/// <param name="now">当前时间</param>
	public bool checkTimeOut (int now)
	{
		int[] time = NoticeManagerment.Instance.getRechargeTime (sid);
		if (time != null) {
			if (time.Length == 1) {
				//约定时间值不足2个视为不限时间,永不过期
				return false;
			} else if (time.Length == 2 && now < time [0] && now > time [1]) {
				return true;
			}
		}

		//时间为空用不过期
		return false;
	}

	//是否是限制充值
	public bool isTimeLimit ()
	{
		int[] time = NoticeManagerment.Instance.getRechargeTime (sid);
		if (time == null || time.Length < 2)
			return false;
		return true;
	}

	//是否在该限时活动内
	public bool isInTimeLimit (int now)
	{
		int[] time = NoticeManagerment.Instance.getRechargeTime (sid);
		//时间不规格不符合视为永不限时
		if (time == null || time.Length < 2)
			return false;
		else if (now >= time [0] && now <= time [1]) 
			return true;
		else 
			return false;//不在时间段内视为过期
	}

	//是否已经过期 true表示过期
	public bool isTimeout (int now)
	{
		int[] time = NoticeManagerment.Instance.getRechargeTime (sid);
		if (time == null)
			return false;
		return now > time [1];
	}
}
