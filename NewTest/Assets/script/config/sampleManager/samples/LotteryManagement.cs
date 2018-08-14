using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LotteryManagement
{
	private static LotteryManagement instance;

	public List<Lottery> playerLotteryList = new List<Lottery>();// 玩家购买彩票列表 //
	public List<AwardLottery> awardList = new List<AwardLottery>();// 中奖列表//
	public string awardResult;// 开奖结果//
	public long moneyAward;// 奖池奖金//
	public List<string> selectNumList = new List<string>();// 选号时存储号码用//
	public int currentDayBuyCount;// 当天购买次数//
	public int selectedAwardCount;// 选注奖励可领取次数//
	public int lastBuyCount;//  上一次活动中购买次数//

	public bool canGetInitFPort = true;// 在处于界面跨天到活动开放用//

	public static LotteryManagement Instance {
		get {
			if (instance == null)
				instance = new LotteryManagement ();
			return instance;
		}
	}


	private int getLotteryCountByVipLv(int vipLv)
	{
		List<LotteryBuyCountSample> sample = LotteryBuyCountConfigManager.Instance.getSample();
		for(int i=0;i<sample.Count;i++)
		{
			if(vipLv == sample[i].vipLv)
			{
				return sample[i].totalCount;
			}
		}

		return sample[0].totalCount;
	}

	// 得到可购买次数//
	public int getLotteryCount()
	{
		int count = getLotteryCountByVipLv(UserManager.Instance.self.getVipLevel()) - currentDayBuyCount;
		if(count <= 0)
			return 0;

		return count;
	}

	public LotteryData parseLotteryData(string str)
	{
		LotteryData data = new LotteryData();
		string[] strs = str.Split('#');
		data.selectNumEndTime = StringKit.toInt(strs[0]);
		data.awardNumEndTime = StringKit.toInt(strs[1]);
		data.openTime = parseTime(strs[2]);
		data.sid = StringKit.toInt(strs[3]);
		data.limitLv = StringKit.toInt(strs[4]);
		data.costGold = StringKit.toLong(strs[5]);
		data.costRmb = StringKit.toInt(strs[6]);
		data.updateLotteryMoneyTime = StringKit.toInt(strs[7]);
		data.sendAwardEndTime = StringKit.toInt(strs[8]);
		return data;
	}
	public int[] parseTime(string str)
	{
		int tmp;
		string[] strs = str.Split(',');
		int[] time = new int[strs.Length];
		for(int i=0;i<strs.Length;i++)
		{
			time[i] = StringKit.toInt(strs[i]);
		}
		for(int i=0;i<time.Length - 1;i++)
		{
			for(int j=0;j<time.Length - i -1;j++)
			{
				if(time[j] > time[j+1])
				{
					tmp = time[j];
					time[j] = time[j+1];
					time[j+1] = tmp;
				}
			}
		}
		return time;
	}
	// 是否有足够的金币钻币购买彩票//
	public bool enoughToBuy(int vipLv)
	{
		int costGoldCount;
		int costRmbCount;
		int hasBuyCount;// 购买次数//
		LotteryBuyCountSample sample = LotteryBuyCountConfigManager.Instance.getCountSample(vipLv);
		if(sample != null)
		{
			costGoldCount = sample.goldCount;
			costRmbCount = sample.rmbCount;
			hasBuyCount = LotteryManagement.instance.currentDayBuyCount;
			if(hasBuyCount < costGoldCount)
			{
				// 金币不足//
				if(UserManager.Instance.self.getMoney() < CommandConfigManager.Instance.getLotteryData().costGold)
				{
					UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_goldNotEough"));
					});
					return false;
				}
			}
			else
			{
				// 钻币不足//
				if(UserManager.Instance.self.getRMB() < CommandConfigManager.Instance.getLotteryData().costRmb)
				{
					UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_rmbNotEough"));
					});
					return false;
				}
			}
			return true;
		}
		return false;
	}
	public void setNumSprite(Transform parent,string numStr)
	{
		Char[] strs = numStr.ToCharArray();
		for(int i=0;i<strs.Length;i++)
		{
			switch (strs[i]) {
			case '0':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "0";
				break;
			case '1':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "1";
				break;
			case '2':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "2";
				break;
			case '3':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "3";
				break;
			case '4':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "4";
				break;
			case '5':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "5";
				break;
			case '6':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "6";
				break;
			case '7':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "7";
				break;
			case '8':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "8";
				break;
			case '9':
				parent.FindChild(i.ToString()).gameObject.GetComponent<UISprite>().spriteName = "9";
				break;
			}
		}
	}

}

public class Lottery
{
	public string time;
	public string num;
	public int state;

	public Lottery(string time, string num,int state)
	{
		this.time = time;
		this.num = num;
		this.state = state;
	}
}

public class AwardLottery
{
	public string time;// 中奖时间//
	public string playerName;// 中奖玩家名称//
	public string money;// 中奖奖金//
	public string awardNum;// 中奖号码//
	public string serName;// 服务器名字//

	public AwardLottery()
	{

	}

	public AwardLottery(string time,string playerName,string money,string awardNum)
	{
		this.time = time;
		this.playerName = playerName;
		this.money = money;
		this.awardNum = awardNum;
	}
}

public class LotteryState
{
	public const int NotOpenAward = -2;
	public const int NoAward = -1;
	public const int SpecialAward = 0;
	public const int FirstAward = 1;
	public const int SecondAward = 2;
	public const int ThirdAward = 3;
}

public class LotterCostType
{
	public const int GoldType = 0;
	public const int RmbType = 1;
}

public class LotteryPhaseType
{
	public const int FinishPhase = 0;// 结束阶段 预告阶段//
	public const int SelectNumPhase = 1;// 选号阶段//
	public const int AwardNumPhase = 2;// 开奖阶段//
	public const int SendAwardPhase = 3;//  发奖阶段//
}

public class LotteryData
{
	public int selectNumEndTime;// 选号结束时间//
	public int awardNumEndTime;//  开奖结束时间//
	public int[] openTime;//  开启时间 //
	public int sid;// 活动sid//
	public int limitLv;// 活动开启等级//
	public long costGold;// 买一注花费金币//
	public int costRmb;// 买一注花费钻币//
	public int updateLotteryMoneyTime;// 奖池奖金刷新时间//
	public int sendAwardEndTime;// 发奖结束时间//
}


