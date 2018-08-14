using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackPrizeRechargeInfoFPort : BaseFPort
{

	private CallBack callBack;
	private List<RechargeSample> list;

	public void BackPrizeRechargeInfoAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.BACKPRIZE_RECHARGEINFO);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType msgInfo = message.getValue ("msg") as ErlType;
		if(msgInfo is ErlArray)// 回归累计充值信息，活动已开启//
		{
			BackPrizeRechargeInfo.Instance.isActive = true;
			ErlArray arr = msgInfo as ErlArray;

			ErlArray recevedIds;

			BackPrizeRechargeInfo.Instance.rechargeNum = StringKit.toInt(arr.Value[0].getValueString());

			recevedIds = arr.Value[1] as ErlArray;
			BackPrizeRechargeInfo.Instance.receviedCount = recevedIds.Value.Length;

			if(BackPrizeRechargeInfo.Instance.rechargeIitems == null)
			{
				BackPrizeRechargeInfo.Instance.rechargeIitems = new Dictionary<int,BackRecharge>();
			}
			if(list == null)
			{
				list = BackRechargeConfigManager.Instance.getRechargeList();
				for(int i=0;i<list.Count;i++)
				{
					BackPrizeRechargeInfo.Instance.rechargeIitems.Add(list[i].sid,new BackRecharge(list[i].sid,BackPrizeRechargeInfo.Instance.rechargeNum));
				}
			}
			int mID =0;
			for(int i=0;i<recevedIds.Value.Length;i++)
			{
				mID = StringKit.toInt(recevedIds.Value[i].getValueString());
				BackPrizeRechargeInfo.Instance.rechargeIitems[mID].setState(BackRechargeState.recevied);
			}

			BackPrizeRechargeInfo.Instance.endTimes = StringKit.toInt(arr.Value[2].getValueString());

			if(recevedIds.Value.Length == list.Count)// 所有条目领取完，关闭窗口//
			{
				BackPrizeRechargeInfo.Instance.isActive = false;
			}
		}
		else// 未开启//
		{
			BackPrizeRechargeInfo.Instance.isActive = false;
		}
		if(callBack != null)
		{
			callBack();
		}
	}
}

public class BackPrizeRechargeInfo
{
	private BackPrizeRechargeInfo info;
	public bool isActive = false;// 活动是否激活//
	public int rechargeNum;// 该活动累计充值金额//
	public int endTimes;// 活动到期时间//
	public Dictionary<int,BackRecharge> rechargeIitems = new Dictionary<int,BackRecharge>();// 领奖条目//
	public int receviedCount;// 领取条目个数//

	public static BackPrizeRechargeInfo Instance
	{		
		get{return SingleManager.Instance.getObj("BackPrizeRechargeInfo") as BackPrizeRechargeInfo;}
	}

	public int getCanRecevieCount()
	{
		int i = 0;
		List<RechargeSample> list = BackRechargeConfigManager.Instance.getRechargeList();
		for(int j=0;j<list.Count;j++)
		{
			//if(rechargeNum > list[j].condition/10)
			if(rechargeNum > list[j].condition)
			{
				i++;
			}
		}

		return i;
	}
	public int getReceviedCount()
	{
		int i = 0;
		foreach (KeyValuePair<int,BackRecharge> kv in rechargeIitems)
		{
			if(kv.Value.state == BackRechargeState.recevied)
			{
				i++;
			}
		}
		return i;
	}
}
