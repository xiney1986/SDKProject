using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 * 充值管理器
 * 所有充值信息
 * @author 汤琦
 * */
public class RechargeManagerment
{
	public const int ONERMB_STATE_VALID = 0, // 充钱未领取
		ONERMB_STATE_INVALID = 1, // 没充钱
		ONERMB_STATE_FINISHED = 2; // 已领取

	private List<Recharge> list;
	private Recharge oneRmb;//首冲条目
	private string showLabel;
	private string rechargeLabel;
	private bool m_canFirst;

	public bool canFirst {
		get {
			return m_canFirst;
		}
		set {
			m_canFirst = value;
		}
	}

//首充机会是否还在
	
	public RechargeManagerment ()
	{
		initList ();
	}

	public Recharge getOneRmb ()
	{
		return oneRmb == null ? new Recharge (NoticeType.ONERMB_RECHARGE_SID, 0, 0) : oneRmb;
	}

	public static RechargeManagerment Instance {
		get{ return SingleManager.Instance.getObj ("RechargeManagerment") as RechargeManagerment;}
	}

	/// <summary>
	/// 首冲状态(1=没充钱,2=已领取)
	/// </summary>
	/// <returns>The rmb.</returns>
	public int getOneRmbState ()
	{
		// 异常
		if (RechargeManagerment.Instance.getOneRmb () == null) {
			return ONERMB_STATE_INVALID;
		}
		//已经领过不能领了
		if (RechargeManagerment.Instance.getOneRmb ().count > 0) {
			return ONERMB_STATE_FINISHED;
		}
		//没冲过钱不能领
		if (RechargeManagerment.Instance.getOneRmb ().num < 1) {
			return ONERMB_STATE_INVALID;
		}
		return ONERMB_STATE_VALID;
	}

	//初始化兑换条目集合
	private void initList ()
	{
		list = new List<Recharge> ();
		int[] sids = RechargeSampleManager.Instance.getAllRecharge ();
		int max = sids.Length;
		for (int i = 0; i < max; i++) {
			Recharge re = new Recharge (sids [i], 0);
			list.Add (re);
		}
	}
	//更新充值条目
	public void updateRecharge ()
	{
		int max = list.Count;
		for (int i = 0; i < max; i++) {
			Recharge recharge=NoticeActiveManagerment.Instance.getActiveInfoBySid (list [i].sid) as Recharge;
			if(recharge!=null) {
				list [i].setCount (recharge.count);
				list [i].setNum (recharge.num); 
				//首冲条目，写死不能改 
				if (list [i].sid == NoticeType.ONERMB_RECHARGE_SID && oneRmb == null)
					oneRmb = list [i];
			}
		}
	}
	//获得所有充值条目
	public List<Recharge> getAllRecharges ()
	{
		return list;
	}
	
	//获得所有可用的充值条目
	public List<Recharge> getCanUseRecharges ()
	{
		List<Recharge> tmps = new List<Recharge> ();
		foreach (Recharge each in list) {			
			tmps.Add (each);
			RechargeSample rechargeSample = each.getRechargeSample ();
		}
		return tmps;
	}

	/// <summary>
	/// 获得指定时间有效的Recharge
	/// </summary>
	/// <param name="sids">sids</param>
	/// <param name="now">时间</param>
	public List<Recharge> getValidRechargesByTime (int[] sids, int now)
	{
		List<Recharge> temps = getCanUseRecharges (sids);
		Recharge rg;
		for (int i=0; i<temps.Count; i++) {
			rg = temps [i];
			if (rg == null)
				continue;
			if (rg.checkTimeOut (now) || rg.isComplete () == false || rg.isRecharge () == false) {
				temps.RemoveAt (i);
				i--;
			}
		}
		return temps;
	}

	/// <summary>
	/// 获得指定时间内的Recharge
	/// </summary>
	/// <param name="sids">sids</param>
	/// <param name="now">时间</param>
	public List<Recharge> getAllRechargesByTime (int[] sids, int now)
	{
		List<Recharge> temps = getCanUseRecharges (sids);
		Recharge rg;
		for (int i=0; i<temps.Count; i++) {
			rg = temps [i];
			if (rg == null)
				continue;
			//不是限时栏目就移除
			if (!rg.isInTimeLimit (now)) {
				temps.RemoveAt (i);
				i--;
			}
		}
		return temps;
	}

	public List<Recharge> getCanUseRecharges (int[] sids)
	{
		List<Recharge> temps = new List<Recharge> ();
		int now = ServerTimeKit.getSecondTime ();
		RechargeSample sample;
		for (int i = 0; i < sids.Length; i++) {
			foreach (Recharge item in list) {
				if (sids [i] == item.sid) {
					sample = item.getRechargeSample ();
					item.setTime (NoticeManagerment.Instance.getRechargeTime (item.sid));//暂时写这里
					if (item.isTimeLimit ()) { //是限时充值
						if (!item.isTimeout (now))
							temps.Add (item);
					} else {
						//不是限时充值，领取要隐藏
						if (item.count < sample.count)
							temps.Add (item);
					}
				}
			}
		}
		return temps;
	}
    public List<Recharge> getOneManyRecharges(int[] sids)
    {
        List<Recharge> temps = new List<Recharge>();
        for (int i = 0; i < sids.Length; i++)
        {
            foreach (Recharge item in list)
            {
                if (sids[i] == item.sid)
                {
                    item.setTime(NoticeManagerment.Instance.getRechargeTime(item.sid));//暂时写这里
                    if(item.isOneManyRechargeShow())
                       temps.Add(item);
                }
            }
        }
        return temps;
    }

	public bool testTime (Recharge recharge, int currentTime)
	{
		bool isOk = false;
		if (recharge.getStartTime () == 0 && recharge.getEndTime () == 0) {
			isOk = true;
		}
		//差多久开始
		if (recharge.getStartTime () > currentTime && recharge.getStartTime () > 0) {	
			isOk = false;
		}
		//过期移除
		if (recharge.getEndTime () < currentTime && recharge.getEndTime () > 0) {
			isOk = false;
		}		
		if (recharge.getStartTime () == 0 && recharge.getEndTime () > 0 && currentTime < recharge.getEndTime ()) {	
			isOk = true;
		}
		if (recharge.getStartTime () > 0 && recharge.getEndTime () == 0 && currentTime > recharge.getStartTime ()) {
			isOk = true;
		}	
		
		if (recharge.getStartTime () > 0 && recharge.getEndTime () > 0 && currentTime > recharge.getStartTime () && currentTime < recharge.getEndTime ()) {	
			isOk = true;
		}
		return isOk;
	}
}
