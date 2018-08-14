using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 公告管理器
 * 所有公告信息
 * @author 汤琦
 * */
public class NoticeManagerment
{

	public const int MONTHCARD_STATE_VALID = 0, // 购买月卡奖励未领取
		MONTHCARD_STATE_INVALID = 1, // 没有购买月卡
		MONTHCARD_STATE_FINISHED = 2; // 购买月卡奖励已领取

	private List<Notice> array;
	private IntKeyHashMap exchangeTime, rechargeTime;//活动面板兑换、充值消费的有效时间
	private int alchemyNum;//已使用次数
	private int[] heroEatInfo;//[sid,start,end,count]
	public List<int> CloseNoticeSidList; //服务端gm关闭活动的列表
	public bool alchemyNeverTip;//炼金是否提示
	public bool huntNeverTip = false;
	public List<NoticeLimitAwardInfo> noticeLimit;// = new List<int>();
												  /** 限时翻翻乐数据缓存 */
	public TurnSpriteData xs_turnSpriteData;
	/** 精灵翻翻乐数据缓存 */
	public TurnSpriteData turnSpriteData;

	public long loginTime = ServerTimeKit.getLoginTime();// 登录时间//

	public NoticeManagerment()
	{
		CloseNoticeSidList = new List<int>();
		noticeLimit = new List<NoticeLimitAwardInfo>();
	}

	public static NoticeManagerment Instance
	{
		get { return SingleManager.Instance.getObj("NoticeManagerment") as NoticeManagerment; }
	}

	public int[] getHeroEatInfo()
	{
		return heroEatInfo;
	}

	public void setHeroEatInfo(int[] info)
	{
		this.heroEatInfo = info;
	}

	public void setAlchemyNum(int num)
	{
		alchemyNum = num;
	}

	//	每次消耗钻石=max（int（次数/2)*2-VIP等级对应减免值，0）
	public int getAlchemyConsume(int vipFactor)
	{
		return Mathf.Max(((int)(alchemyNum / 2)) * 16 - vipFactor, 0);
	}
	//	每次消耗钻石=max（int（次数/2)*2-VIP等级对应减免值，0）
	public int getAlchemyConsume()
	{
		int level = UserManager.Instance.self.getVipLevel();
		int vipFactor = level > 0 ? VipManagerment.Instance.getVipbyLevel(level).privilege.alchemyFactor : 0;
		return Mathf.Max(((int)(alchemyNum / 2)) * 16 - vipFactor, 0);
	}
	//	每次消耗钻石=max（int（次数/2)*2-VIP等级对应减免值，0）给定次数需要的钻石综合
	public int getAlchemyConsumeAll(int num)
	{
		int level = UserManager.Instance.self.getVipLevel();
		int vipFactor = level > 0 ? VipManagerment.Instance.getVipbyLevel(level).privilege.alchemyFactor : 0;
		int rmb = 0;
		int index = alchemyNum;
		for (int i = 0; i < num; i++)
		{
			rmb += Mathf.Max(((int)(index / 2)) * 16, 0);
			index = index + 1;
		}
		return rmb;
	}
	public int getAlchemyConsumeAllForVip(int num)
	{
		int level = UserManager.Instance.self.getVipLevel();
		int vipFactor = level > 0 ? VipManagerment.Instance.getVipbyLevel(level).privilege.alchemyFactor : 0;
		int rmb = 0;
		int index = alchemyNum;
		for (int i = 0; i < num; i++)
		{
			rmb += Mathf.Max(((int)(index / 2)) * 16 - vipFactor, 0);
			index = index + 1;
		}
		return rmb;
	}
	//		每次获得游戏币=9000+次数*500 给定次数能得到金币的最小数量
	public long getAlchemyMoneyAll(int num)
	{
		int lv = UserManager.Instance.self.getUserLevel();
		int index = alchemyNum + 1;
		long money = 0;
		for (int i = 0; i < num; i++)
		{
			//money+=Mathf.Max (3000+400*lv, 8660)+(300+40*lv)*index;
			money += Mathf.Max(3000 + 400 * lv, 8660) + (100 + 10 * lv) * index;
			index++;
		}
		return money;
	}

	public void updateCloseNoticeList(int sid, bool state)
	{

		if (CloseNoticeSidList.Contains(sid))
		{
			if (state) CloseNoticeSidList.Remove(sid);
		}
		else if (!state)
		{
			CloseNoticeSidList.Add(sid);
		}

		if (UiManager.Instance.getWindow<NoticeWindow>() != null)
		{
			UiManager.Instance.getWindow<NoticeWindow>().initTopButton();
		}

	}
	public void addNoticeLimitInfo(NoticeLimitAwardInfo awardInfo)
	{
		noticeLimit.Add(awardInfo);
		//		for (int i=0; i< noticeLimit.Count; i++)
		//			Debug.LogError (">>>" + noticeLimit [i]);
	}
	/// <summary>
	/// 获得指定时间内激活的公告,用来做第一次登录时候的显示
	/// </summary>
	/// <param name="now">时间</param>
	/// <param name="noticeType">获得类型</param>
	public int getFirstBootReceiveByTime(int entranceId)
	{
		int now = ServerTimeKit.getSecondTime();
		List<Notice> array = getValidNoticeList(entranceId);
		int noticeSid = 0;
		Notice notice;
		for (int i = 0; i < array.Count; i++)
		{
			notice = array[i];
			if (notice.isTimeLimit())
			{
				noticeSid = notice.sid;
				break;
			}
		}
		return noticeSid;
	}
	/// 获得指定时间指定公告类型Recharge的可领取次数
	/// </summary>
	/// <param name="now">时间</param>
	/// <param name="noticeType">获得类型</param>
	public int getCanReceiveRechargeByTime(int now, int noticeType, int entranceId)
	{
		List<Notice> array = getNoticeList();
		int count = 0;
		if (array == null)
			return count;
		Notice notice;
		for (int i = 0; i < array.Count; i++)
		{
			notice = array[i];
			if (notice == null)
				continue;
			int type = notice.getSample().type;
			if (type != noticeType)
				continue;
			NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(notice.sid);
			if (sample.entranceId != entranceId)
				continue;
			List<Recharge> temps = RechargeManagerment.Instance.getValidRechargesByTime((sample.content as SidNoticeContent).sids, now);
			if (temps != null)
			{
				count = temps.Count;
			}
			return count;
		}
		return count;
	}
	/// 获得指定时间指定公告类型Exchange的可领取次数
	/// </summary>
	/// <param name="now">时间</param>
	/// <param name="noticeType">获得类型</param>
	public int getCanReceiveExchangeByTime(int now, int noticeType, int entranceId)
	{
		List<Notice> array = getNoticeList();
		int count = 0;
		if (array == null)
			return count;
		Notice notice;
		for (int i = 0; i < array.Count; i++)
		{
			notice = array[i];
			if (notice == null)
				continue;
			int type = notice.getSample().type;
			if (type != noticeType)
				continue;
			NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(notice.sid);
			if (sample.entranceId != entranceId)
				continue;
			List<Exchange> temps = ExchangeManagerment.Instance.getValidExchangesByTime((sample.content as SidNoticeContent).sids, sample.type, now);
			//vip兑换特殊处理
			if (sample.sid == 4)
			{
				if (temps != null && temps.Count > 0 && PlayerPrefs.GetString(PlayerPrefsComm.VIP_EXCHANGE_TIP) == "ok")
					count++;
				continue;
			}
			if (temps != null)
			{
				count += temps.Count;
			}
			return count;
		}
		return count;
	}



	//		每次获得游戏币=9000+次数*500
	public int getAlchemyMoney()
	{
		int lv = UserManager.Instance.self.getUserLevel();
		//return Mathf.Max (3000+400*lv, 8660)+(300+40*lv)*(alchemyNum+1);
		//每次获得游戏币=MAX(3000+400*玩家等级,8660)+(100+10*玩家等级)*购买后次数
		return Mathf.Max(3000 + 400 * lv, 8660) + (100 + 10 * lv) * (alchemyNum + 1);
	}

	public int getAlchemyNum()
	{
		return alchemyNum;
	}

	public void addExchangeTime(int sid, int[] time)
	{
		if (exchangeTime == null)
			exchangeTime = new IntKeyHashMap();
		exchangeTime.put(sid, time);
	}

	public void addRechargeTime(int sid, int[] time)
	{
		if (rechargeTime == null)
			rechargeTime = new IntKeyHashMap();
		rechargeTime.put(sid, time);
	}

	public int[] getExchangeTime(int sid)
	{
		if (exchangeTime == null)
			return null;
		return exchangeTime.get(sid) as int[];
	}

	public int[] getRechargeTime(int sid)
	{
		if (rechargeTime == null)
			return null;
		return rechargeTime.get(sid) as int[];
	}

	//初始化所有公告信息
	public void createAllNotice()
	{
		int[] sids = NoticeSampleManager.Instance.getAllNotice();
		array = new List<Notice>();
		Notice tempNotice;
		for (int i = 0; i < sids.Length; i++)
		{
			tempNotice = createByType(sids[i]);//把每一个活动分配到对应的子类去
			if (tempNotice == null) continue;
			array.Add(tempNotice);
		}
		NoticeSample ns1, ns2;
		for (int i = 0; i < array.Count; i++)
		{
			for (int j = 0; j < array.Count - 1 - i; j++)
			{
				ns1 = array[j].getSample();
				ns2 = array[j + 1].getSample();
				if (ns1.order > ns2.order)
					swap(array, j, j + 1);
			}
		}
		for (int i = 0, length = array.Count; i < length; i++)
		{
			array[i].index = i;
		}
	}

	private Notice createByType(int sid)
	{
		NoticeSample noticeSample = NoticeSampleManager.Instance.getNoticeSampleBySid(sid);
		if (noticeSample == null)
			return null;
		int type = noticeSample.type;
		if (type == NoticeType.EXCHANGENOTICE)
			return new ExchangeNotice(sid);
		else if (type == NoticeType.ONERMB)
			return new OneRmbNotice(sid);
		else if (type == NoticeType.TOPUPNOTICE || type == NoticeType.TIME_RECHARGE || type == NoticeType.COSTNOTICE)
			return new RechargeNotice(sid);
		else if (type == NoticeType.QUIZ_EXAM || type == NoticeType.QUIZ_SURVEY)
			return new QuizNotice(sid);
		else if (type == NoticeType.CONSUME_REBATE)
			return new ConsumeRebateNotice(sid);
		else if (type == NoticeType.LUCKY_CARD || type == NoticeType.LUCKY_EQUIP || type == NoticeType.XIANSHI_HUODONG || type == NoticeType.XIANSHI_FANLI)
			return new LuckyDrawNotice(sid);
		else if (type == NoticeType.HAPPY_SUNDAY)
			return new HappySundayNotice(sid);
		else if (type == NoticeType.DOUBLE_RMB)
			return new DoubleRMBNotice(sid);
		else if (type == NoticeType.NEW_RECHARGE || type == NoticeType.NEW_CONSUME || type == NoticeType.ONE_MANY_RECHARGE)
			return new NewRechargeNotice(sid);
		else if (type == NoticeType.NEW_EXCHANGE)
			return new NewExchangeNotice(sid);
		else if (type == NoticeType.REMAKE_EQUIP)
			return new EquipRemakeNotice(sid);
		else if (type == NoticeType.DAILY_REBATE)
			return new DailyRebateNotice(sid);
		else if (type == NoticeType.LADDER_HEGEMONY)
			return new LadderHegemoneyNotice(sid);
		else if (type == NoticeType.LADDER_ACTION_TIME)
			return new LadderHegemoneyActiveNotice(sid);
		else if (type == NoticeType.LIMIT_COLLECT)
			return new LimitCollectNotice(sid);
		else if (type == NoticeType.XIANSHI_HAPPY_TURN)
			return new HappyTurnSpriteNotice(sid);
		else if (type == NoticeType.FESTIVAL_WISH)
			return new FestivalWishNotice(sid);
		else if (type == NoticeType.FESTIVAL_FIREWORKS)
			return new FestivalFireworksNotice(sid);
		else if (type == NoticeType.SUPERDRAW)
			return new SuperDrawNotice(sid);
		else if (type == NoticeType.SIGNIN)
			return new SignInNotice(sid);
		else if (type == NoticeType.SHAREDRAW)
		{
			return new ShareDrawNotice(sid);
		}
		else if (type == NoticeType.BACK_PRIZE)
			return new BackPrizeNotice(sid);
		else if (type == NoticeType.BACK_RECHARGE)
			return new BackPrizeRechargeNotice(sid);
		else if (type == NoticeType.LOTTERY)
			return new LotteryNotice(sid);
		return new Notice(sid);
	}

	// 交换
	private void swap(List<Notice> list, int left, int right)
	{
		Notice temp;
		temp = list[left];
		list[left] = list[right];
		list[right] = temp;
	}

	/// <summary>
	/// 获取指定公告sid的有效公告
	/// </summary>
	/// <param name="noticeSid">公告sid</param>
	public Notice getValidNoticeBySid(int noticeSid, int entranceId)
	{
		List<Notice> array = getValidNoticeList(entranceId);
		Notice notice;
		for (int i = 0; i < array.Count; i++)
		{
			notice = array[i];
			if (notice == null)
				continue;
			if (notice.sid == noticeSid)
				return notice;
		}
		return null;
	}
	/// <summary>
	/// 获取指定公告sid的公告
	/// </summary>
	/// <param name="noticeSid">公告sid</param>
	public Notice getNoticeBySid(int noticeSid)
	{
		List<Notice> array = getNoticeList();
		Notice notice;
		for (int i = 0; i < array.Count; i++)
		{
			notice = array[i];
			if (notice == null)
				continue;
			if (notice.sid == noticeSid)
				return notice;
		}
		return null;
	}
	/// <summary>
	/// 获取有效的公告列表
	/// </summary>
	public List<Notice> getValidNoticeList(int _entranceId)
	{
		List<Notice> array = getNoticeList();
		List<Notice> temp = new List<Notice>();
		NoticeSample tmpSample;
		for (int i = 0; i < array.Count; i++)
		{
			if (array[i] == null)
				continue;
            tmpSample = NoticeSampleManager.Instance.getNoticeSampleBySid(array[i].sid);
            bool isValid = array[i].isValid();
            //Log.info(array[i].sid + "," + array[i].getSample().name + "," + isValid + "," + array[i].GetType());
            if (isValid && tmpSample.entranceId == _entranceId && !CloseNoticeSidList.Contains(array[i].sid))
			{
                //Log.info(array[i].sid+","+ array[i].getSample().name);
				temp.Add(array[i]);
			}
		}
		return temp;
	}


	/// <summary>
	/// 获取所有公告列表
	/// </summary>
	/// <returns>The notice list.</returns>
	public List<Notice> getNoticeList()
	{
		if (array == null || array.Count < 1)
			createAllNotice();
		return array;
	}
	/// <summary>
	/// 获取有效的类型活动
	/// </summary>
	public Notice getNoticeListByType(int _type, int entranceId)
	{
		List<Notice> tempNotice = getValidNoticeList(entranceId);
		foreach (Notice item in tempNotice)
		{
			if (item.getSample().type == _type)
			{
				return item;
			}
		}
		return null;
	}
	public List<Notice> getNoticesByType(int type, int entranceId)
	{
		List<Notice> tempNotice = getValidNoticeList(entranceId);
		List<Notice> newList = null;
		foreach (Notice item in tempNotice)
		{
			if (item.getSample().type == type)
			{
				if (newList == null) newList = new List<Notice>();
				newList.Add(item);
			}
		}
		return newList;
	}
	// 根据类型和当前天梯争霸活动的sid，获取天梯争霸开启竞技场活动时间的notice信息
	public Notice getNoticeListByType(int _type, int sid, int entranceId)
	{
		List<Notice> tempNotice = getValidNoticeList(entranceId);
		foreach (Notice item in tempNotice)
		{
			if (item.getSample().type == _type && item.getSample().order == sid)
			{
				return item;
			}
		}
		return null;
	}

	//更新公告信息
	public void updateAllNotice(ErlArray erlarr)
	{
		if (array == null || array.Count < 1)
			createAllNotice();
		for (int i = 0; i < erlarr.Value.Length; i++)
		{
			updateNotice(erlarr.Value[i] as ErlArray);
		}
	}

	//更新公告信息
	private void updateNotice(ErlArray erlarr)
	{
		if (array == null || array.Count < 1)
			throw new Exception(GetType() + "updateLuckyDraw error! array is null");
		if (erlarr == null || erlarr.Value.Length < 1)
			return;
		for (int i = 0; i < array.Count; i++)
		{
			int sid = array[i].sid;
			int _sid = StringKit.toInt(erlarr.Value[0].getValueString());
			if (sid == _sid)
			{
				array[i].updateRead(StringKit.toInt(erlarr.Value[1].getValueString()));
			}
		}
	}
	//设置某个公告为已读
	public void setNoticeReaded(int sid)
	{
		for (int i = 0; i < array.Count; i++)
		{
			if (sid == array[i].sid)
			{
				array[i].readed = 1;
			}
		}
	}

	//获得公告信息
	public List<Notice> getNoticeArr()
	{
		return array;
	}

	public void clearNoticeArray()
	{
		if (array != null)
			array.Clear();
	}

	public string getNoticeSidToString()
	{
		createAllNotice();
		if (array == null)
			return null;
		string str = "";
		for (int i = 0; i < array.Count; i++)
		{
			if (i == 0)
				str = array[i].sid.ToString();
			else
				str += "," + array[i].sid;
		}
		return str;
	}

	//判断公告条目是否读取
	public bool isNoticeRead(Notice notice)
	{
		if (notice.readed == 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	//得到置顶公告数量
	public int getTopNoticeNum()
	{
		List<Notice> list = new List<Notice>();
		for (int i = 0; i < array.Count; i++)
		{
			NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(array[i].sid);
			if (sample.type == NoticeType.STICKNOTICE)
			{
				list.Add(array[i]);
				continue;
			}
		}
		return list.Count;
	}

	//获得符合条件的公告信息
	public Notice[] getNotices(int time)
	{
		List<Notice> list = new List<Notice>();
		for (int i = 0; i < array.Count; i++)
		{
			NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(array[i].sid);
			if (sample.type == NoticeType.STICKNOTICE)
			{
				list.Add(array[i]);
				continue;
			}
			list.Add(array[i]);
		}
		return list.ToArray();
	}

	//获得未读取公告数量
	public int getNoReadNoticeCount()
	{
		int count = 0;
		int currentTime = ServerTimeKit.getSecondTime();
		for (int i = 0; i < array.Count; i++)
		{
			if (array[i].readed == 0)
			{
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// 判断是否某个sid的公告有内容可以领取
	/// </summary>
	public bool isNoticeBySidDraw(int sid)
	{
		NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(sid);
		int type = sample.type;

		if (type == NoticeType.TOPUPNOTICE || type == NoticeType.TIME_RECHARGE)
		{
			List<Recharge> rechargeList = RechargeManagerment.Instance.getCanUseRecharges((sample.content as SidNoticeContent).sids);
			for (int i = 1; i < rechargeList.Count; i++)
			{
				if (rechargeList[i].isComplete() && rechargeList[i].isRecharge() && RechargeManagerment.Instance.testTime(rechargeList[i], ServerTimeKit.getSecondTime()))
				{
					return true;
				}
			}
		}
		else if (type == NoticeType.EXCHANGENOTICE)
		{
			List<Exchange> exchangeList = ExchangeManagerment.Instance.getCanUseExchanges((sample.content as SidNoticeContent).sids, sample.type);
			for (int i = 1; i < exchangeList.Count; i++)
			{
				ExchangeSample temp = exchangeList[i].getExchangeSample();
				if (ExchangeManagerment.Instance.isCheckPremises(temp) && ExchangeManagerment.Instance.isCheckConditions(temp) && ExchangeManagerment.Instance.testTime(exchangeList[i], ServerTimeKit.getSecondTime()))
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 获取购买月卡奖励状态
	/// </summary>
	public int getMonthCardRewardState()
	{
		int state = MONTHCARD_STATE_VALID;
		bool hasReward = monthCardDayRewardEnable;
		if (monthCardDueDate != null)
		{
			if (!hasReward)
			{
				state = MONTHCARD_STATE_FINISHED;
			}
			else
			{
				state = MONTHCARD_STATE_VALID;
			}
		}
		else
		{
			state = MONTHCARD_STATE_INVALID;
		}
		return state;
	}

	/// <summary>
	/// 月卡中每日奖励是否可领取
	/// </summary>
	public bool monthCardDayRewardEnable = false;
	/// <summary>
	/// 月卡到期时期
	/// </summary>
	public int[] monthCardDueDate = null;
	/// <summary>
	/// 月卡到期时期时间戳
	/// </summary>
	public int monthCardDueSeconds = 0;

	public int getFirstNotice(int[] _noticeIndexs)
	{

		//int currentTime=ServerTimeKit.getSecondTime();
		//int current = ServerTimeKit.getCurrentSecond ();
		int count = 0;
		int type = 0;
		int now = ServerTimeKit.getSecondTime();
		int current = ServerTimeKit.getCurrentSecond();

		Notice notice;
		NoticeSample noticeSample;
		for (int i = 0, length = _noticeIndexs.Length; i < length; i++)
		{
			type = _noticeIndexs[i];
			notice = getNoticeByType(type);
			if (notice == null)
			{
				continue;
			}
			noticeSample = notice.getSample();
			if (type == NoticeType.TOPUPNOTICE || type == NoticeType.TIME_RECHARGE)
			{
				List<Recharge> temps_0 = RechargeManagerment.Instance.getValidRechargesByTime((noticeSample.content as SidNoticeContent).sids, now);
				if (temps_0 != null)
				{
					count = temps_0.Count;
				}
			}
			else if (type == NoticeType.ALCHEMY)
			{
				if (!(NoticeManagerment.Instance.getAlchemyConsume() > 0))
				{
					count = 1;
				}
			}
			else if (type == NoticeType.COSTNOTICE)
			{
				List<Recharge> temps_1 = RechargeManagerment.Instance.getValidRechargesByTime((noticeSample.content as SidNoticeContent).sids, now);
				if (temps_1 != null)
				{
					count = temps_1.Count;
				}
			}
			else if (type == NoticeType.EXCHANGENOTICE)
			{
				List<Exchange> temps_2 = ExchangeManagerment.Instance.getValidExchangesByTime((noticeSample.content as SidNoticeContent).sids, noticeSample.type, now);
				if (temps_2 != null)
				{
					count = temps_2.Count;
				}
			}
			else if (type == NoticeType.MONTHCARD)
			{
				if (NoticeManagerment.Instance.getMonthCardRewardState() == NoticeManagerment.MONTHCARD_STATE_VALID)
				{
					count = 1;
				}
			}
			else if (type == NoticeType.HAPPY_TURN_SPRITE || type == NoticeType.XIANSHI_HAPPY_TURN)
			{
				if (UserManager.Instance.self.getUserLevel() >= noticeSample.levelLimit && HoroscopesManager.Instance.getBeginTime() < current &&
					current < HoroscopesManager.Instance.getEndTime() && now > HoroscopesManager.Instance.getPrayTime())
				{
					count = 1;
				}
			}
			else if (type == NoticeType.ONERMB)
			{
				if (RechargeManagerment.Instance.getOneRmbState() != RechargeManagerment.ONERMB_STATE_FINISHED)
					count = 1;
			}
			if (count > 0)
			{
				return notice.sid;
			}
		}
		return 0;
	}

	public Notice getNoticeByType(int _type)
	{
		List<Notice> tempNotice = getNoticeList();
		foreach (Notice item in tempNotice)
		{
			if (item.getSample().type == _type)
			{
				return item;
			}
		}
		return null;
	}


	public void getSuperDrawInfo()
	{
		Notice notice = getNoticeByType(NoticeType.SUPERDRAW);
		bool isvalid = notice.isValid();
		if (notice != null && isvalid)
		{
			SuperDrawGetInfoFPort fport = FPortManager.Instance.getFPort<SuperDrawGetInfoFPort>();
			fport.access(notice.sid, () =>
			{
			});
		}
	}
}