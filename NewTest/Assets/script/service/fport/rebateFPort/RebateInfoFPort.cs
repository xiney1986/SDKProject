using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RebateInfoFPort : BaseFPort
{
	private CallBack callBack;
	public Dictionary<int,RebateDayInfo> infos = new Dictionary<int,RebateDayInfo>();
	List<int> dayIDs = new List<int>();
	public List<RebateSample> diamondSample;
	public List<RebateSample> goldSample;

	public void RebateInfoAccess(CallBack _callBack)
	{
		this.callBack = _callBack;

		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_REBATE_INFO);
		access (message);
	}


	public override void read (ErlKVMessage message)
	{
		dayIDs.Clear();
		for(int i=1;i<=9;i++)
		{
			dayIDs.Add (i);
		}
		infos.Clear();
		RebateInfoManagement.Instance.canRecevieCount = 0;
		ErlType msgInfo = message.getValue ("msg") as ErlType;
		if(msgInfo is ErlArray)
		{
			ErlArray arr = msgInfo as ErlArray;
			ErlArray arr1;
			for(int i=0;i<arr.Value.Length;i++)
			{
				arr1 = arr.Value[i] as ErlArray;
				int j = 0;
				int _dayID = StringKit.toInt (arr1.Value [j++].getValueString ());
				RebateDayInfo rdi = new RebateDayInfo(_dayID);
				
				rdi.costDiamond = StringKit.toLong (arr1.Value [j++].getValueString ());
				rdi.getDiamond = calculateGetDiamond(rdi.costDiamond);
				
				rdi.costGold = StringKit.toLong (arr1.Value [j++].getValueString ());
				rdi.getGold = calculateGetGold(rdi.costGold);
				
				rdi.rebateState = StringKit.toInt (arr1.Value [j++].getValueString ());
				
				if(rdi.rebateState == RebateState.UN_RECEIVE)// 未领取//
				{
					int serverTime = ServerTimeKit.getSecondTime();// 服务器时间//
					setState(serverTime,rdi);
				}
				else
				{
					rdi.s_rebateState = S_RebateState.NONE;
				}
				
				infos.Add(rdi.dayID,rdi);
				dayIDs.Remove(rdi.dayID);
			}
			// 未参加活动//
			for(int i=0;i<dayIDs.Count;i++)
			{
				RebateDayInfo rdi = new RebateDayInfo(dayIDs[i]);
				int serverTime = ServerTimeKit.getSecondTime();// 服务器时间//
				int _startTime = rdi.startTime;// 该天活动开始时间//

				if(serverTime >= _startTime && serverTime < (_startTime+24*60*60))// 收集中//
				{
					rdi.rebateState = RebateState.UN_RECEIVE;
					rdi.s_rebateState = S_RebateState.COLLECTING;
				}
				else if(serverTime < _startTime)// 未到活动时间//
				{
					rdi.rebateState = RebateState.NOT_ON_TIME;
				}
				infos.Add(dayIDs[i],rdi);
			}
		}
		else// 所有活动未开启//
		{
			string result = msgInfo.getValueString();
			if(result == "not_open")
			{
				for(int i=1;i<=9;i++)
				{
					RebateDayInfo rdi = new RebateDayInfo();
					infos.Add(i,rdi);
				}
			}
		}

		if(callBack != null)
		{
			callBack();
		}
	}

	public void setState(int serverTime,RebateDayInfo _rdi)
	{
		int _startTime = _rdi.startTime;// 该天活动开始时间//
		int _endTime = _rdi.endTime;// 该天活动可领奖时间//

		if(serverTime >= _endTime)// 可领取//
		{
			RebateInfoManagement.Instance.canRecevieCount++;
			_rdi.s_rebateState = S_RebateState.CAN_RECEIVE;
		}
		else if(serverTime >= (_startTime+24*60*60) && serverTime < _endTime)// 收集完成等待领奖//
		{
			_rdi.s_rebateState = S_RebateState.WAIT_RECEIVE;
		}
		else if(serverTime >= _startTime && serverTime < (_startTime+24*60*60))// 收集中//
		{
			_rdi.s_rebateState = S_RebateState.COLLECTING;
		}
	}

	public Dictionary<int,RebateDayInfo> getInfos()
	{
		return infos;
	}

	public void setInfos(Dictionary<int,RebateDayInfo> _infos)
	{
		infos = _infos;
	}

	// 计算返利金币数//
	public long calculateGetGold(long costGold)
	{
		// 小于等于//
		for(int i=0;i<goldSample.Count;i++)
		{
			if(costGold <= goldSample[i].compareVal)
			{
				if(costGold <= 0)
				{
					return 0;
				}
				else
				{
					// 返利不足1，取1//
					if(costGold * goldSample[i].lessRate / 100 == 0)
					{
						return 1;
					}
					return costGold * goldSample[i].lessRate / 100;
				}
			}
		}
		// 大于//
		for(int j=0;j<goldSample.Count;j++)
		{
			if(goldSample[j].moreRate != 0)
			{
				if(costGold <= 0)
				{
					return 0;
				}
				else
				{
					// 返利不足1，取1//
					if(costGold * goldSample[j].moreRate / 100 == 0)
					{
						return 1;
					}
					return costGold * goldSample[j].moreRate / 100;
				}
			}
		}
		return 0;
	}

	// 计算返利钻币数//
	public long calculateGetDiamond(long costDiamod)
	{
		// 小于等于//
		for(int i=0;i<diamondSample.Count;i++)
		{
			if(costDiamod <= diamondSample[i].compareVal)
			{
				if(costDiamod <= 0)
				{
					return 0;
				}
				else
				{
					// 返利不足1，取1//
					if(costDiamod * diamondSample[i].lessRate / 100 == 0)
					{
						return 1;
					}
					return costDiamod * diamondSample[i].lessRate / 100;
				}
			}
		}
		// 大于//
		for(int j=0;j<diamondSample.Count;j++)
		{
			if(diamondSample[j].moreRate != 0)
			{
				if(costDiamod <= 0)
				{
					return 0;
				}
				else
				{
					// 返利不足1，取1//
					if(costDiamod * diamondSample[j].moreRate / 100 == 0)
					{
						return 1;
					}
					return costDiamod * diamondSample[j].moreRate / 100;
				}
			}
		}
		return 0;
	}

	public void setDiamondSample(List<RebateSample> _diamondSample)
	{
		diamondSample = _diamondSample;
	}
	public void setGoldSample(List<RebateSample> _goldSample)
	{
		goldSample = _goldSample;
	}
}

public class RebateInfoManagement
{
	public int canRecevieCount;// 可以领奖个数//
	public int rebateNoticeID;// 返福利活动id//

	public long loginTime = ServerTimeKit.getLoginTime();// 登录时间//

	public static RebateInfoManagement Instance
	{		
		get{return SingleManager.Instance.getObj("RebateInfoManagement") as RebateInfoManagement;}
	}
}
