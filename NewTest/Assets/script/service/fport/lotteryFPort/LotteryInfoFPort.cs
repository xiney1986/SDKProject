using UnityEngine;
using System.Collections;
using System;

public class LotteryInfoFPort : BaseFPort
{
	private CallBack callBack;

	public void lotteryInfoAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LOTTERY_INFO);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlArray infos = message.getValue("msg") as ErlArray;
		if(infos != null)
		{
			int index = 0;
			// 开奖结果 -1则未开奖//
			LotteryManagement.Instance.awardResult = infos.Value[index++].getValueString();
			// 奖池奖金//
			LotteryManagement.Instance.moneyAward = StringKit.toLong(infos.Value[index++].getValueString());
			// 中奖日志//
			ErlList awardInfos = infos.Value[index++] as ErlList;
			if(awardInfos != null)
			{
				LotteryManagement.Instance.awardList.Clear();
				AwardLottery al;
				ErlArray eArr;
				ErlArray times;
				for(int i=0;i<awardInfos.Value.Length;i++)
				{
					al = new AwardLottery();
					eArr = awardInfos.Value[i] as ErlArray;
					if(eArr != null)
					{
						times = eArr.Value[0] as ErlArray;
						if(times != null)
						{
							al.time = string.Format(LanguageConfigManager.Instance.getLanguage("lottery_ymd"),times.Value[0].getValueString(),times.Value[1].getValueString(),times.Value[2].getValueString());
						}
						al.serName = eArr.Value[1].getValueString();
						al.playerName = eArr.Value[2].getValueString();
						al.money = eArr.Value[3].getValueString();
						al.awardNum = eArr.Value[4].getValueString();
						LotteryManagement.Instance.awardList.Add(al);
					}
				}
			}
			// 玩家购买信息//
			LotteryManagement.Instance.currentDayBuyCount = 0;
			LotteryManagement.Instance.lastBuyCount = 0;
			ErlList buyInfos = infos.Value[index++] as ErlList;
			DateTime dt = ServerTimeKit.getDateTime();
			if(buyInfos != null)
			{
				Lottery lottery;
				ErlArray eArr;
				ErlList eArr2;// [{号码 数量 状态},...]// 
				ErlArray eArr3;// [号码 数量 状态,...]//
				ErlArray times;// 年月日//
				string timeStr = "";
				LotteryManagement.Instance.playerLotteryList.Clear();
				for(int i=0;i<buyInfos.Value.Length;i++)
				{
					eArr = buyInfos.Value[i] as ErlArray;
					if(eArr != null)
					{
						times = eArr.Value[0] as ErlArray;
						if(times != null)
						{
							timeStr = string.Format(LanguageConfigManager.Instance.getLanguage("lottery_ymd"),times.Value[0].getValueString(),times.Value[1].getValueString(),times.Value[2].getValueString());
						}
						eArr2 = eArr.Value[1] as ErlList;
						if(eArr2 != null)
						{
							for(int j=0;j<eArr2.Value.Length;j++)
							{
								eArr3 = eArr2.Value[j] as ErlArray;
								if(eArr3 != null)
								{
									if(dt.Year == StringKit.toInt(times.Value[0].getValueString()) && dt.Month == StringKit.toInt(times.Value[1].getValueString()) && dt.Day == StringKit.toInt(times.Value[2].getValueString()))
									{
										LotteryManagement.Instance.currentDayBuyCount +=  StringKit.toInt(eArr3.Value[1].getValueString());
									}
									for(int k=0;k<StringKit.toInt(eArr3.Value[1].getValueString());k++)
									{
										lottery = new Lottery(timeStr,eArr3.Value[0].getValueString(),StringKit.toInt(eArr3.Value[2].getValueString()));
										LotteryManagement.Instance.playerLotteryList.Add(lottery);
									}
								}
							}
						}
					}
				}
				LotteryManagement.Instance.playerLotteryList.Reverse();
			}
			// 上次活动日期//
			ErlArray lastActivity = infos.Value[index++] as ErlArray;
			if(lastActivity != null)
			{
				string lastTimeStr = string.Format(LanguageConfigManager.Instance.getLanguage("lottery_ymd"),lastActivity.Value[0].getValueString(),lastActivity.Value[1].getValueString(),lastActivity.Value[2].getValueString());
				for(int i=0;i<LotteryManagement.Instance.playerLotteryList.Count;i++)
				{
					if(LotteryManagement.Instance.playerLotteryList[i].time == lastTimeStr)
					{
						LotteryManagement.Instance.lastBuyCount++;
					}
				}
			}
			// 已领取奖励条目//
			LotteryManagement.Instance.selectedAwardCount = 0;
			LotteryNotice notice = NoticeManagerment.Instance.getNoticeByType(NoticeType.LOTTERY) as LotteryNotice;
			for(int i=0;i<LotterySelectPrizeConfigManager.Instance.prizes.Count;i++)
			{
				// 在活动日 看当天购买次数//
				if(isActivityOpen())
				{
					// 达到条件可领取//
					if(LotteryManagement.Instance.currentDayBuyCount >= LotterySelectPrizeConfigManager.Instance.prizes[i].condition)
					{
						LotterySelectPrizeConfigManager.Instance.prizes[i].state = LotterySelectPrizeState.CanRecive;
						LotteryManagement.Instance.selectedAwardCount++;
					}
					else 
					{
						LotterySelectPrizeConfigManager.Instance.prizes[i].state = LotterySelectPrizeState.CantRecive;
					}
				}
				// 不在活动日 看上次活动日购买次数//
				else 
				{
					if(LotteryManagement.Instance.lastBuyCount >= LotterySelectPrizeConfigManager.Instance.prizes[i].condition)
					{
						LotterySelectPrizeConfigManager.Instance.prizes[i].state = LotterySelectPrizeState.CanRecive;
						LotteryManagement.Instance.selectedAwardCount++;
					}
					else 
					{
						LotterySelectPrizeConfigManager.Instance.prizes[i].state = LotterySelectPrizeState.CantRecive;
					}
				}
			}
			ErlArray receviedAward = infos.Value[index++] as ErlArray;
			if(receviedAward != null)
			{
				for(int i=0;i<receviedAward.Value.Length;i++)
				{
					for(int j=0;j<LotterySelectPrizeConfigManager.Instance.prizes.Count;j++)
					{
						if(StringKit.toInt(receviedAward.Value[i].getValueString()) == LotterySelectPrizeConfigManager.Instance.prizes[j].id)
						{
							LotterySelectPrizeConfigManager.Instance.prizes[j].state = LotterySelectPrizeState.Recived;
						}
					}
				}
				LotteryManagement.Instance.selectedAwardCount -= receviedAward.Value.Length;
			}

			if(callBack != null)
			{
				callBack();
				callBack = null;
			}
		}
	}

	public bool isActivityOpen()
	{
		int day = TimeKit.getWeekCHA(TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime()).DayOfWeek);
		for(int i=0;i<CommandConfigManager.Instance.getLotteryData().openTime.Length;i++)
		{
			if(day == CommandConfigManager.Instance.getLotteryData().openTime[i])
			{
				return true;
			}
		}
		return false;
	}
}
