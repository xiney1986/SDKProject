using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LastBattleManagement
{
	private static LastBattleManagement instance;
	// 完成备战新进度//
	public int newProcess;
	// 完成备战老进度//
	public int oldProcess;
	// 当前时间段需要显示的捐献列表列表//
	public List<LastBattleDonationSample> currentDonationList;

	public bool isUpdateDonationList = false;

	public int battleCount;// 剩余小怪挑战次数//

	public int killBossCount;// 全服刺杀boss个数//

	public int bossID;//  挑战bossID//

	public long currentBossHP;// 当前boss血量//

	public long bossTotalHP;// 当前boss最大血量//

	public int bossCount;// 剩余boss挑战次数//

	public int nextBossCountUpdateTime;// 下次boss挑战次数刷新时间 (在次数满时用掉一次记得把该值赋成当前时间)//

	public int battleID;// 小怪挑战关卡id//

	public int battleWinCount;// 小怪挑战连胜次数//

	public int lastBattlePhase;// 末日挑战阶段 0未开启 1备战阶段 2boss战阶段//

	public int nvBlessLv;// 女神鼓舞等级//

	public int donationNextUpdateTime;// 捐献下次刷新时间 //

	public List<LastBattleDonationFromServer> donationFromServer = new List<LastBattleDonationFromServer>();// 从服务器拿到捐献列表//

	// 击杀boss数据//
	public List<LastBattleKillBossData> killBossDatas = new List<LastBattleKillBossData>();

	public List<LastBattleProcessPrizeSample> stateWeakList = new List<LastBattleProcessPrizeSample>();// 类型弱点//
	public List<LastBattleProcessPrizeSample> physicalHarmList = new List<LastBattleProcessPrizeSample>();// 物理易伤//
	public List<LastBattleProcessPrizeSample> magicHarmList = new List<LastBattleProcessPrizeSample>();// 魔法易伤//

	public List<LastBattleRank> rank = new List<LastBattleRank>();// 活动得分排名//

	public int battleScore;// 战斗获得排名积分//

	public int myRank;// 积分排名中自己的名次//

	public const string lastbattleDonationKey = "lastbattleDonation";
	public const string lastBattleOldProcessKey = "lastbattleOldProcess";
	public const string lastBattleBossIDKey = "lastbattleBossIDKey";
	public const string lastBattleBossHpKey = "lastbattleBossHp";
	public const string lastBattleTimeKey = "lastBattleTime";

	public static LastBattleManagement Instance {
		get {
			if (instance == null)
				instance = new LastBattleManagement ();
			return instance;
		}
	}

	// 计算进度//
	public string caculateProcess()
	{
		if(newProcess >= CommandConfigManager.Instance.lastBattleData.totalProcess)
		{
			return "100%";
		}
		return Mathf.Floor(((float)newProcess / CommandConfigManager.Instance.lastBattleData.totalProcess) * 100) + "%";
	}

	// 根据后台推的捐献条目进行当前时间段需要显示的捐献列表列表的刷新//
	public void updateDonationList(List<LastBattleDonationFromServer> donations)
	{
		//isUpdateDonationList = true;
		if(currentDonationList == null)
			currentDonationList = new List<LastBattleDonationSample>();
		List<LastBattleDonationSample> totalDonation = LastBattleDonationConfigManager.Instance.totalDonation;
		currentDonationList.Clear();
		LastBattleDonationSample sample;
		for(int i=0;i<donations.Count;i++)
		{
			for(int j=0;j<totalDonation.Count;j++)
			{
				if(donations[i].id == totalDonation[j].id)
				{
					sample = new LastBattleDonationSample(totalDonation[j].id,totalDonation[j].donation,totalDonation[j].scores,totalDonation[j].junGong,totalDonation[j].nvShenBlessLV,donations[i].index,donations[i].state,totalDonation[j].process,totalDonation[j].donationType);
					currentDonationList.Add(sample);
				}
			}
		}
	}

	// 解析末日决战在command表中的数据//
	public LastBattleData parseLastBattleData(string str)
	{
		LastBattleData data = new LastBattleData();
		string[] strs = str.Split('#');
		data.dayOfWeek = StringKit.toInt(strs[0]);
		data.startTime = StringKit.toInt(strs[1]);
		data.battlePrepareEndTime = StringKit.toInt(strs[2]);
		data.endTime = StringKit.toInt(strs[3]);
		data.openLevel = StringKit.toInt(strs[4]);
		data.totalProcess = StringKit.toInt(strs[5]);
		data.junGongSid = StringKit.toInt(strs[6]);
		data.junGongMaxNum = StringKit.toInt(strs[7]);
		data.battleWinCount = StringKit.toInt(strs[8]);
		data.bossBattleCountUpdateTime = StringKit.toInt(strs[9]);
		data.battleCountUpdateTimes = parseTime(strs[10]);
		data.battleTotalCount = StringKit.toInt(strs[11]);
		data.bossBattleTotalCount = StringKit.toInt(strs[12]);
		data.battleSkipCount = StringKit.toInt(strs[13]);
		data.battleTottalFubenCount = StringKit.toInt(strs[14]); 
		data.nvShenAdd = StringKit.toFloat(strs[15]);
		data.bossBattleFinalKillAardJunGong = StringKit.toInt(strs[16]);
		data.bossHpUpdateTime = StringKit.toInt(strs[17]);
		data.processUpdateTime = StringKit.toInt(strs[18]);
		data.battleAddTime = StringKit.toInt(strs[19]);

		return data;
	}
	int[] parseTime(string str)
	{
		int[] times;
		string[] strs = str.Split(',');
		times = new int[strs.Length];
		for(int i=0;i<times.Length;i++)
		{
			times[i] = StringKit.toInt(strs[i]);
		}
		return times;
	}
	// 通过当前时间得到下次更新小怪次数时间//
	public int getNextUpdateBattleCountTime(int currentTime)
	{
		int[] times = CommandConfigManager.Instance.lastBattleData.battleCountUpdateTimes;
		for(int i=0;i<times.Length;i++)
		{
			if(currentTime < times[i])
			{
				return times[i];
			}
		}
		return 0;
	}

	// 得到当前完成几个进度//
	public int getCurrentPrecessCount()
	{
		float i = (float)newProcess / CommandConfigManager.Instance.lastBattleData.totalProcess;
		if(i >= 0.1f && i < 0.2f)
		{
			return 1;
		}
		else if(i >= 0.2f && i < 0.3f)
		{
			return 2;
		}
		else if(i >= 0.3f && i < 0.4f)
		{
			return 3;
		}
		else if(i >= 0.4f && i < 0.5f)
		{
			return 4;
		}
		else if(i >= 0.5f && i < 0.6f)
		{
			return 5;
		}
		else if(i >= 0.6f && i < 0.7f)
		{
			return 6;
		}
		else if(i >= 0.7f && i < 0.8f)
		{
			return 7;
		}
		else if(i >= 0.8f && i < 0.9f)
		{
			return 8;
		}
		else if(i >= 0.9f && i < 1f)
		{
			return 9;
		}
		else if(i >= 1f)
		{
			return 10;
		}

		return 0;
	}

	// 判断该物资够不够捐献//
	public bool isEnoughDonation(LastBattleDonationSample sample)
	{
		// 钻石//
		if(sample.donation.type == PrizeType.PRIZE_RMB)
		{
			if(UserManager.Instance.self.getRMB () >= StringKit.toInt(sample.donation.num))
			{
				return true;
			}
		}
		// 游戏币//
		else if(sample.donation.type == PrizeType.PRIZE_MONEY)
		{
			if(UserManager.Instance.self.getMoney () >= StringKit.toInt(sample.donation.num))
			{
				return true;
			}
		}
		// 道具//
		else if(sample.donation.type == PrizeType.PRIZE_PROP)
		{
			Prop prop = StorageManagerment.Instance.getProp(sample.donation.pSid);
			if(prop != null && prop.getNum() >= StringKit.toInt(sample.donation.num))
			{
				return true;
			}
		}
		// 功勋//
		else if(sample.donation.type == PrizeType.PRIZE_MERIT)
		{
			if(UserManager.Instance.self.merit >= StringKit.toInt(sample.donation.num))
			{
				return true;
			}
		}
		// 星魂碎片//
		else if(sample.donation.type == PrizeType.PRIZE_STARSOUL_DEBRIS)
		{
			if(StarSoulManager.Instance.getDebrisNumber () >= StringKit.toInt(sample.donation.num))
			{
				return true;
			}
		}
		// 星屑//
		else if(sample.donation.type == PrizeType.PRIZE_STAR_SCORE)
		{
			if(GoddessAstrolabeManagerment.Instance.getStarScore() >= StringKit.toInt(sample.donation.num))
			{
				return true;
			}
		}
		// 卡片//
		else if(sample.donation.type == PrizeType.PRIZE_CARD)
		{
			Card card = StorageManagerment.Instance.getCardBySid(sample.donation.pSid);
			// 祭品卡//
			if(card != null && ChooseTypeSampleManager.Instance.isToEat(card,ChooseTypeSampleManager.TYPE_SKILL_EXP))
			{
				if(StorageManagerment.Instance.getAllRoleByEatByQuiltyID(card.getQualityId()).Count >= StringKit.toInt(sample.donation.num))
				{
					return true;
				}
			}
		}
		// 装备//
		else if(sample.donation.type == PrizeType.PRIZE_EQUIPMENT)
		{
			Equip equip = StorageManagerment.Instance.getEquipBySid(sample.donation.pSid);
			// 祭品装备//
			if(equip != null && ChooseTypeSampleManager.Instance.isToEat(equip,ChooseTypeSampleManager.TYPE_EQUIP_EXP))
			{
				if(StorageManagerment.Instance.getAllEquipByEatByQuiltyID(equip.getQualityId()).Count >= StringKit.toInt(sample.donation.num))
				{
					return true;
				}
			}
		}
		return false;
	}

	public string getDonationName(LastBattleDonationSample sample)
	{
		string donationName = "";
		// 钻石//
		if(sample.donation.type == PrizeType.PRIZE_RMB)
		{
			donationName = LanguageConfigManager.Instance.getLanguage("s0048");
		}
		// 游戏币//
		else if(sample.donation.type == PrizeType.PRIZE_MONEY)
		{
			donationName = LanguageConfigManager.Instance.getLanguage("s0049");
		}
		// 道具//
		else if(sample.donation.type == PrizeType.PRIZE_PROP)
		{
			donationName = PropSampleManager.Instance.getPropSampleBySid (sample.donation.pSid).name;
		}
		// 功勋//
		else if(sample.donation.type == PrizeType.PRIZE_MERIT)
		{
			donationName = LanguageConfigManager.Instance.getLanguage("Arena06");
		}
		// 星魂碎片//
		else if(sample.donation.type == PrizeType.PRIZE_STARSOUL_DEBRIS)
		{
			donationName = LanguageConfigManager.Instance.getLanguage("s0466");
		}
		// 星屑//
		else if(sample.donation.type == PrizeType.PRIZE_STAR_SCORE)
		{
			donationName = LanguageConfigManager.Instance.getLanguage("resources_star_score_name");
		}
		// 卡片//
		else if(sample.donation.type == PrizeType.PRIZE_CARD)
		{
			donationName = CardSampleManager.Instance.getRoleSampleBySid (sample.donation.pSid).name;
		}
		// 装备//
		else if(sample.donation.type == PrizeType.PRIZE_EQUIPMENT)
		{
			donationName = EquipmentSampleManager.Instance.getEquipSampleBySid (sample.donation.pSid).name;
		}

		return donationName;
	}

	public string getHaveGoodsCount(LastBattleDonationSample sample)
	{
		string count = "0";
		// 钻石//
		if(sample.donation.type == PrizeType.PRIZE_RMB)
		{
			count = UserManager.Instance.self.getRMB ().ToString();
		}
		// 游戏币//
		else if(sample.donation.type == PrizeType.PRIZE_MONEY)
		{
			count = UserManager.Instance.self.getMoney ().ToString();
		}
		// 道具//
		else if(sample.donation.type == PrizeType.PRIZE_PROP)
		{
			Prop prop = StorageManagerment.Instance.getProp(sample.donation.pSid);
			if(prop != null)
			{
				count = prop.getNum().ToString();
			}
		}
		// 功勋//
		else if(sample.donation.type == PrizeType.PRIZE_MERIT)
		{
			count = UserManager.Instance.self.merit.ToString();
		}
		// 星魂碎片//
		else if(sample.donation.type == PrizeType.PRIZE_STARSOUL_DEBRIS)
		{
			count = StarSoulManager.Instance.getDebrisNumber ().ToString();
		}
		// 星屑//
		else if(sample.donation.type == PrizeType.PRIZE_STAR_SCORE)
		{
			count = GoddessAstrolabeManagerment.Instance.getStarScore().ToString();
		}
		// 卡片//
		else if(sample.donation.type == PrizeType.PRIZE_CARD)
		{
			Card card = StorageManagerment.Instance.getCardBySid(sample.donation.pSid);
			// 祭品卡//
			if(card != null && ChooseTypeSampleManager.Instance.isToEat(card,ChooseTypeSampleManager.TYPE_SKILL_EXP))
			{
				count = StorageManagerment.Instance.getAllRoleByEatByQuiltyID(card.getQualityId()).Count.ToString();
			}
		}
		// 装备//
		else if(sample.donation.type == PrizeType.PRIZE_EQUIPMENT)
		{
			Equip equip = StorageManagerment.Instance.getEquipBySid(sample.donation.pSid);
			// 祭品装备//
			if(equip != null && ChooseTypeSampleManager.Instance.isToEat(equip,ChooseTypeSampleManager.TYPE_EQUIP_EXP))
			{
				count = StorageManagerment.Instance.getAllEquipByEatByQuiltyID(equip.getQualityId()).Count.ToString();
			}
		}
		return count;
	}

	// 更新击杀boss数据//
	public void updateKillBossData()
	{
		killBossDatas.Clear();
	}

	public LastBattleBossSample getBossInfo(int bossSid)
	{
		return LastBattleBossSampleManager.Instance.getBossInfoSampleBySid(bossSid);
	}

	// 军功已满提示//
	public bool creatJunGongMaxTip()
	{
		// 军功超上限//
		if(StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid) != null)
		{
			if(StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid).getNum() >= CommandConfigManager.Instance.lastBattleData.junGongMaxNum)
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("LastBattle_junGongWarrning"));
				});
				MaskWindow.UnlockUI();
				return true;
			}
		}
		return false;
	}
	public bool creatJunGongMaxTipByGetCount(int getCount)
	{
		// 军功超上限//
		if(StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid) != null)
		{
			if(StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid).getNum() + getCount > CommandConfigManager.Instance.lastBattleData.junGongMaxNum)
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("LastBattle_junGongWarrning"));
				});
				MaskWindow.UnlockUI();
				return true;
			}
		}
		return false;
	}
	public void clearData()
	{
		if(PlayerPrefs.HasKey(lastBattleOldProcessKey + UserManager.Instance.self.uid))
		{
			PlayerPrefs.DeleteKey(lastBattleOldProcessKey + UserManager.Instance.self.uid);
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
		newProcess = 0;
		oldProcess = 0;
		if(LastBattleProcessPrizeConfigManager.Instance.processPrize != null)
		{
			for(int i=0;i<LastBattleProcessPrizeConfigManager.Instance.processPrize.Count;i++)
			{
				LastBattleProcessPrizeConfigManager.Instance.processPrize[i].state = LastBattleProcessPrizeState.CANNOT_RECEVIE;
			}
		}
	}

	public int compareDonation(LastBattleDonationSample sample1,LastBattleDonationSample sample2)
	{
		int result = sample1.donationType.CompareTo(sample2.donationType);
		if(result == 0)
		{
			int resultID = sample1.id.CompareTo(sample2.id);
			if(resultID == 0)
			{
				return 1;
			}
			return resultID;
		}
		
		return result;
	}
}

public class LastBattleData
{
	// 开放星期几#活动开启时间#备战结束时间#活动关闭时间#开放等级#备战进度上限//
	public int dayOfWeek;// 活动是星期几开启//
	public int startTime;// /活动开启时间/
	public int battlePrepareEndTime;// 备战结束时间//
	public int endTime;// 活动结束时间//
	public int openLevel;// 活动开放等级//
	public int totalProcess;// 备战进度上限//
	public int junGongSid;// 军功sid//
	public int junGongMaxNum;// 军功上限个数//
	public int battleWinCount;// 小怪跳关连胜次数标准//
	public int bossBattleCountUpdateTime;// boss挑战次数刷新间隔时间//
	public int[] battleCountUpdateTimes;// 小怪挑战次数刷新时间节点//
	public int battleTotalCount;// 挑战小怪次数上限//
	public int bossBattleTotalCount;// boss挑战次数上限//
	public int battleSkipCount;// 挑战小怪跳关数//
	public int battleTottalFubenCount;// 挑战小怪总副本数//
	public float nvShenAdd;// 女神加成倍数//
	public int bossBattleFinalKillAardJunGong;// boss战最后一击奖励军功数//
	public int bossHpUpdateTime;// boss血量刷新间隔时间//
	public int processUpdateTime;// 备战物资刷新间隔时间//
	public int battleAddTime;// 挑战小怪增加次数//

	public LastBattleData()
	{

	}
}

public class LastBattleKillBossData
{
	public string killBossTime;// 击杀boss的时间//
	public string playerName;// 击杀boss的玩家名字//
	public string serverName;// 该玩家所在服务器名字//
	public string bossName;// 被杀boss的名字//
	public string killBossCount;//  当时刺杀boss的数量//
}

public class LastBattleRank
{
	public string serverName;
	public string uid;
	public string name;
	public int score;
	public int vipLV;

	public LastBattleRank()
	{

	}

	public LastBattleRank(string serverName,string uid,string name,int score)
	{
		this.serverName = serverName;
		this.uid = uid;
		this.name = name;
		this.score = score;
	} 
}

public class LastBattleDonationFromServer
{
	public int index;// 下标//
	public int id;// 条目id//
	public int state;// 状态//

	public LastBattleDonationFromServer(int index,int id,int state)
	{
		this.index = index;
		this.id = id;
		this.state = state;
	}
}


