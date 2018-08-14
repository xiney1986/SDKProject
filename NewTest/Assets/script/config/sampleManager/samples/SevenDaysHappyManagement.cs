using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SevenDaysHappyManagement
{
	private GameObject sevenDaysHappyHelpWindow;
	Dictionary<int,SevenDaysHappySample> sampleDic = new Dictionary<int, SevenDaysHappySample>();
	public List<SevenDaysHappyMisson> missonAfterFilter = new List<SevenDaysHappyMisson>();
	private bool isSevendayshappyHelpWin = false;
	private int activeEndTime;// 活动结束时间//
	private int activeMissonEndTime;// 活动任务结束时间 //
	private bool isPrizesSelectWin = false;

	public long loginTime = ServerTimeKit.getLoginTime();// 登录时间,跨天处理要用//

	public int canReceviedCount;// 总可领取个数//

	public Dictionary<int,int> dayIDAndCount = new Dictionary<int, int>();// 每一天及对应的可领奖个数,key为dayid,value为该天可领取的个数//

	public static SevenDaysHappyManagement Instance 
	{
		get{ return SingleManager.Instance.getObj ("SevenDaysHappyManagement") as SevenDaysHappyManagement;}
	}


	public int compareMisson(SevenDaysHappyMisson misson1,SevenDaysHappyMisson misson2)
	{
		// 先比较是否完成可领取//
		int result = misson1.missonState.CompareTo(misson2.missonState);
		if(result == 0)
		{
			int resultByOrderID = misson1.missonOrder.CompareTo(misson2.missonOrder);
			if(resultByOrderID == 0)
			{
				return 1;
			}
			return resultByOrderID;
		}

		return result;
	}

	// 任务排序//
	public void sortMisson()
	{
		foreach (KeyValuePair<int,SevenDaysHappySample> item in sampleDic)
		{
			foreach (KeyValuePair<int,SevenDaysHappyDetail> kv in item.Value.detailsDic)
			{
				kv.Value.missonList.Sort(compareMisson);
			}
		}
	}

	// 通过服务器发过来的任务进行筛选//
	public List<SevenDaysHappyMisson> filterMisson(Dictionary<int,MissonFromServer> missonDic)
	{
		missonAfterFilter.Clear();
		List<SevenDaysHappyMisson> listBefore = SevenDaysHappySampleManager.Instance.getMissonBeforeFilter();
		SevenDaysHappyMisson misson;
		for(int i=0;i<listBefore.Count;i++)
		{
			if(missonDic.ContainsKey(listBefore[i].missonID) || listBefore[i].detailType == SevenDaysHappyDetailType.banjiaqianggou)
			{
				misson = listBefore[i];
				if(missonDic.ContainsKey(listBefore[i].missonID))
				{
					misson.missonState = missonDic[misson.missonID].missonState;
					misson.missonProgress = missonDic[misson.missonID].missonProgress;
					//missonAfterFilter.Add(misson);
				}
				else//  半价抢购//
				{
					misson.missonState = SevenDaysHappyMissonState.Recevied;
					misson.missonProgress = new int[1];
					misson.missonProgress[0] = 1;

					for(int j=0;j<ShopManagerment.Instance.getAllBanJiaGoods().Count;j++)
					{
						if(misson.goodsID == (ShopManagerment.Instance.getAllBanJiaGoods()[j] as Goods).sid)
						{
							misson.missonState = SevenDaysHappyMissonState.Doing;
							misson.missonProgress[0] = 0;
						}
					}
				}
				missonAfterFilter.Add(misson);
			}
		}
		return missonAfterFilter;
	}


	// 初始化筛选后的模板//
	public void initSevenDaysHappySample(List<SevenDaysHappyMisson> missons)
	{
		SevenDaysHappyManagement.Instance.canReceviedCount = 0;
		dayIDAndCount.Clear();
		sampleDic.Clear();
		SevenDaysHappySample sample;
		SevenDaysHappyDetail detail;
		for(int i=0;i < missons.Count;i++)
		{
			if(!sampleDic.ContainsKey(missons[i].dayID))
			{
				sample = new SevenDaysHappySample(missons[i].dayID);
				detail = new SevenDaysHappyDetail(missons[i]);
				sample.detailsDic.Add(missons[i].detailType,detail);
				sampleDic.Add(missons[i].dayID,sample);
			}
			else
			{
				sample = sampleDic[missons[i].dayID];
				if(!sample.detailsDic.ContainsKey(missons[i].detailType))
				{
					detail = new SevenDaysHappyDetail(missons[i]);
					sample.detailsDic.Add(missons[i].detailType,detail);
				}
				else
				{
					detail = sample.detailsDic[missons[i].detailType];
					detail.missonList.Add(missons[i]);
				}
			}

			if(missons[i].dayID <= getDayIndex() && missons[i].missonState == SevenDaysHappyMissonState.Completed)
			{
				canReceviedCount++;
			}

			if(!dayIDAndCount.ContainsKey(missons[i].dayID))
			{
				if(missons[i].missonState == SevenDaysHappyMissonState.Completed)// 已完成可领取 //
				{
					dayIDAndCount.Add(missons[i].dayID,1);
				}
				else
				{
					dayIDAndCount.Add(missons[i].dayID,0);
				}
			}
			else
			{
				if(missons[i].missonState == SevenDaysHappyMissonState.Completed)// 已完成可领取 //
				{
					dayIDAndCount[missons[i].dayID] ++;
				}
			}
		}
	}

	public void initData(Dictionary<int,MissonFromServer> missonDic)
	{
		List<SevenDaysHappyMisson> listAfterFilter = filterMisson(missonDic);
		initSevenDaysHappySample(listAfterFilter);
	}

	// 根据任务id找任务// 
	public SevenDaysHappyMisson getMissonByMissonID(int missonID)
	{
		foreach (KeyValuePair<int,SevenDaysHappySample> item in sampleDic)
		{
			foreach (KeyValuePair<int,SevenDaysHappyDetail> kv in item.Value.detailsDic)
			{
				for(int i=0;i<kv.Value.missonList.Count;i++)
				{
					if(kv.Value.missonList[i].missonID == missonID)
					{
						return kv.Value.missonList[i];
					}
				}
			}
		}
		return null;
	}

	public Dictionary<int,SevenDaysHappySample> getSevenDaysHappySampleDic()
	{
		return sampleDic;
	}

	// 获得第七天登录奖励的三张橙卡//
	public Card[] getCards()
	{
		if(sampleDic.ContainsKey(7))
		{
			int meirifuliIndex = SevenDaysHappyDetailType.meirifuli;// 每日福利//
			if(sampleDic[7].detailsDic.ContainsKey(meirifuliIndex))
			{
				List<SevenDaysHappyMisson> missons = sampleDic[7].detailsDic[meirifuliIndex].missonList;
				for(int i=0;i<missons.Count;i++)
				{
					if(missons[i].missonType == SevenDaysHappyMissonType.Login_AwardCard)// 第七的登录送卡任务//
					{
						CardSample sample;
						Card[] cards = new Card[missons[i].prizes.Length];
						Card card;
						for(int j=0;j<missons[i].prizes.Length;j++)
						{
							foreach (Card item in PictureManagerment.Instance.cardList)
							{
								if(missons[i].prizes[j].pSid == item.sid)
								{
									cards[j] = item;
									break;
								}
							}
						}
						return cards;
					}
				}
			}
		}

		return null;
	}
	public void setIsSevendayshappyHelpWin(bool b)
	{
		isSevendayshappyHelpWin = b;
	}
	public bool getIsSevendayshappyHelpWin()
	{
		return isSevendayshappyHelpWin;
	}
	public void setHelpObj(GameObject obj)
	{
		sevenDaysHappyHelpWindow = obj;
	}
	public GameObject getHelpObj()
	{
		return sevenDaysHappyHelpWindow;
	}
	public void setIsPrizesSelectWin(bool b)
	{
		isPrizesSelectWin = b;
	}
	public bool getIsPrizesSelectWin()
	{
		return isPrizesSelectWin;
	}

	// 根据注册时间计算整个活动到期时间和七天任务结束时间//
	public void calculateActiveOverTime(int loginTime)
	{
		DateTime dt = TimeKit.getDateTime(loginTime);
		DateTime dt_start = new DateTime(dt.Year,dt.Month,dt.Day,0,0,0);
		this.activeEndTime = TimeKit.timeSecond(TimeKit.getTimeMillis(dt_start)) + 7 * 86400 + 2 * 86400;
		this.activeMissonEndTime = TimeKit.timeSecond(TimeKit.getTimeMillis(dt_start)) + 7 * 86400;
	}
	// 得到整个活动结束时间（包括七天任务和两天领奖延时）//
	public int getEndTime()
	{
		return activeEndTime;
	}
	// 得到活动所有任务结束时间//
	public int getActiveMissonEndTime()
	{
		return activeMissonEndTime;
	}
	// 得到今天是第几天的任务//
	public int getDayIndex()
	{
		if((activeMissonEndTime - ServerTimeKit.getSecondTime()) / 86400 <= 0)
		{
			return 7;
		}
		return 7 - (activeMissonEndTime - ServerTimeKit.getSecondTime()) / 86400;
	}

	public long getSecondDayTime(long loginTime)
	{
		DateTime dt_loginTime = TimeKit.getDateTimeMillis(loginTime);
		DateTime dt_secondDayTime = new DateTime(dt_loginTime.Year,dt_loginTime.Month,dt_loginTime.Day,23,59,59);

		long s_time = TimeKit.getTimeMillis(dt_secondDayTime) + 1000;
		DateTime s_dt = TimeKit.getDateTimeMillis(s_time);
		return TimeKit.getTimeMillis(dt_secondDayTime) + 1000;
	}
}

