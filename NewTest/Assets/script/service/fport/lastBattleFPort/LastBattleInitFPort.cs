using UnityEngine;
using System.Collections;
using System;

public class LastBattleInitFPort : BaseFPort
{
	private CallBack callBack;
	DateTime dt;
	string yearStr;
	string monthStr;
	string dayStr;
	string timeStr;
	string[] strArr;

	public void lastBattleInitAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LASTBATTLEINIT);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlArray info = message.getValue ("msg") as ErlArray;
		ErlArray donations;// 捐献列表//
		ErlArray processPrizes;// 进度奖励列表//
		if(info != null)
		{
			LastBattleManagement.Instance.lastBattlePhase = StringKit.toInt(info.Value[0].getValueString());
			// 未开启//
			if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.NOT_OPEN)
			{
				LastBattleManagement.Instance.clearData();
			}
			else
			{
				if(!compareTime())
				{
					if(PlayerPrefs.HasKey(LastBattleManagement.lastBattleOldProcessKey + UserManager.Instance.self.uid))
					{
						PlayerPrefs.DeleteKey(LastBattleManagement.lastBattleOldProcessKey + UserManager.Instance.self.uid);
					}
					if(PlayerPrefs.HasKey(LastBattleManagement.lastbattleDonationKey + UserManager.Instance.self.uid))
					{
						PlayerPrefs.DeleteKey(LastBattleManagement.lastbattleDonationKey + UserManager.Instance.self.uid);
					}
					if(PlayerPrefs.HasKey(LastBattleManagement.lastBattleBossIDKey + UserManager.Instance.self.uid))
					{
						PlayerPrefs.DeleteKey(LastBattleManagement.lastBattleBossIDKey + UserManager.Instance.self.uid);
					}
					if(PlayerPrefs.HasKey(LastBattleManagement.lastBattleBossHpKey + UserManager.Instance.self.uid))
					{
						PlayerPrefs.DeleteKey(LastBattleManagement.lastBattleBossHpKey + UserManager.Instance.self.uid);
					}
				}

				// 备战阶段//
				if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.PREPARE)
				{
					// 最新进度//
					LastBattleManagement.Instance.newProcess = StringKit.toInt(info.Value[1].getValueString());
					// 已领取备战奖励条目//
					processPrizes = info.Value[2] as ErlArray;
					setProcessPrize(processPrizes);
					// 女神鼓舞等级//
					LastBattleManagement.Instance.nvBlessLv = StringKit.toInt(info.Value[3].getValueString());
					// 上次挑战小怪关卡id//
					LastBattleManagement.Instance.battleID = StringKit.toInt(info.Value[4].getValueString());
					// 剩余小怪挑战次数//
					LastBattleManagement.Instance.battleCount = StringKit.toInt(info.Value[5].getValueString());
					// 挑战小怪连胜次数//
					LastBattleManagement.Instance.battleWinCount = StringKit.toInt(info.Value[6].getValueString());
					// 捐献商店下次刷新时间//
					LastBattleManagement.Instance.donationNextUpdateTime = StringKit.toInt(info.Value[7].getValueString());
					// 捐献列表//
					donations = info.Value[8] as ErlArray;
					updateDonationList(donations);
				}
				// boss战阶段//
				else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.BOSS)
				{
					// 最新进度//
					LastBattleManagement.Instance.newProcess = StringKit.toInt(info.Value[1].getValueString());
					// 已领取备战奖励条目//
					processPrizes = info.Value[2] as ErlArray;
					setProcessPrize(processPrizes);
					// 女神鼓舞等级//
					LastBattleManagement.Instance.nvBlessLv = StringKit.toInt(info.Value[3].getValueString());
					// 击杀boss个数 //
					LastBattleManagement.Instance.killBossCount = StringKit.toInt(info.Value[4].getValueString());
					// 挑战boss//
					LastBattleManagement.Instance.bossID = StringKit.toInt(info.Value[5].getValueString());
					// 当前血量//
					LastBattleManagement.Instance.currentBossHP = StringKit.toLong(info.Value[6].getValueString());
					// 最大血量//
					LastBattleManagement.Instance.bossTotalHP = StringKit.toLong(info.Value[7].getValueString());
					// 剩余boss挑战次数//
					LastBattleManagement.Instance.bossCount = StringKit.toInt(info.Value[8].getValueString());
					// 下次boss挑战次数刷新时间//
					if(info.Value[9].getValueString() == "0")
					{
						LastBattleManagement.Instance.nextBossCountUpdateTime = 0;
					}
					else
					{
						LastBattleManagement.Instance.nextBossCountUpdateTime = StringKit.toInt(info.Value[9].getValueString()) + CommandConfigManager.Instance.lastBattleData.bossBattleCountUpdateTime;
					}
				}
				//  领奖阶段//
				else if(LastBattleManagement.Instance.lastBattlePhase == LastBattlePhase.AWARD)
				{
					// 物资进度//
					LastBattleManagement.Instance.newProcess = StringKit.toInt(info.Value[1].getValueString());
					// 已领取备战奖励条目//
					processPrizes = info.Value[2] as ErlArray;
					setProcessPrize(processPrizes);
				}
			}

			if(callBack != null)
			{
				callBack();
				callBack = null;
			}
		}
	}

	void setProcessPrize(ErlArray arr)
	{
		for(int i=0;i<LastBattleProcessPrizeConfigManager.Instance.processPrize.Count;i++)
		{
			if(i + 1 <= LastBattleManagement.Instance.getCurrentPrecessCount())
			{
				LastBattleProcessPrizeConfigManager.Instance.processPrize[i].state = LastBattleProcessPrizeState.CAN_RECEVIE;
			}
		}
		if(arr.Value.Length > 0)
		{
			for(int i=0;i<arr.Value.Length;i++)
			{
				for(int j=0;j<LastBattleProcessPrizeConfigManager.Instance.processPrize.Count;j++)
				{
					if(StringKit.toInt(arr .Value[i].getValueString()) == LastBattleProcessPrizeConfigManager.Instance.processPrize[j].id)
					{
						LastBattleProcessPrizeConfigManager.Instance.processPrize[j].state = LastBattleProcessPrizeState.RECEVIED;
					}
				}
			}
		}
	}

	public void updateDonationList(ErlArray donations)
	{
		LastBattleManagement.Instance.donationFromServer.Clear();
		int donationIndex;
		int donationID;
		int donationState;
		ErlArray donationItem;
		for(int i=0;i<donations.Value.Length;i++)
		{
			donationItem = donations.Value[i] as ErlArray;
			donationIndex = StringKit.toInt(donationItem.Value[0].getValueString());
			donationID = StringKit.toInt(donationItem.Value[1].getValueString());
			donationState = StringKit.toInt(donationItem.Value[2].getValueString());
			LastBattleManagement.Instance.donationFromServer.Add(new LastBattleDonationFromServer(donationIndex,donationID,donationState));
		}
		// 刷新捐献列表//
		LastBattleManagement.Instance.updateDonationList(LastBattleManagement.Instance.donationFromServer);
		// 捐献列表排序//
		if(LastBattleManagement.Instance.currentDonationList != null && LastBattleManagement.Instance.currentDonationList.Count > 0)
		{
			LastBattleManagement.Instance.currentDonationList.Sort(LastBattleManagement.Instance.compareDonation);
		}
	}

	bool compareTime()
	{
		dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		yearStr = dt.Year.ToString();
		monthStr = dt.Month.ToString();
		dayStr = dt.Day.ToString();
		timeStr = yearStr + "," + monthStr + "," + dayStr;
		if(PlayerPrefs.HasKey(LastBattleManagement.lastBattleTimeKey + UserManager.Instance.self.uid))
		{
			strArr = PlayerPrefs.GetString(LastBattleManagement.lastBattleTimeKey + UserManager.Instance.self.uid).Split(',');
			if(strArr[0]==yearStr && strArr[1]==monthStr && strArr[2]==dayStr)
				return true;
		}
		PlayerPrefs.SetString(LastBattleManagement.lastBattleTimeKey + UserManager.Instance.self.uid,timeStr);
		return false;
	}
}

// 活动阶段//
public class LastBattlePhase
{
	public const int NOT_OPEN = 0;// 为开启//
	public const int PREPARE = 1;// 备战阶段//
	public const int BOSS = 2;// boss战阶段//
	public const int AWARD = 3;// 领奖阶段//
}
