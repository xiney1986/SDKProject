using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**诸神战奖励模板管理器
  *@author gc
  **/
public class GodsWarPrizeSampleManager : SampleConfigManager
{
	//单例
	private static GodsWarPrizeSampleManager instance;
	private List<GodsWarPrizeSample> listFinall;//冠军奖励
	private List<GodsWarPrizeSample> list;//连胜奖励
	private List<GodsWarPrizeSample> listIntegralRank;//积分排行榜奖励（铜）
    private List<GodsWarPrizeSample> listIntegralRankYin;//积分排行榜奖励（银）
    private List<GodsWarPrizeSample> listIntegralRankJin;//积分排行榜奖励(金)
	private List<GodsWarPrizeSample> listIntegral;//积分奖励
	private List<GodsWarPrizeSample> listSuport;//竞猜奖励
	private List<GodsWarPrizeSample> listFianlAwardBronze;//决赛奖励(青铜)
	private List<GodsWarPrizeSample> listFianlAwardSilver;//决赛奖励(白银)
	private List<GodsWarPrizeSample> listFianlAwardGold;//决赛奖励(黄金)
	private List<int> listScore;//积分列表
	public const int TYPE_1 = 1,//终极奖励
						TYPE_2 = 2,//连胜奖励
						TYPE_3 = 3,//积分排行榜奖励(铜)
                        TYPE_33 = 33,//积分排行榜奖励(银)
                        TYPE_333 = 333,//积分排行榜奖励(金)
						TYPE_4 = 4,//积分奖励
						TYPE_5 = 5,//竞猜奖励
						TYPE_6 = 6,//决赛奖励(青铜)
						TYPE_7 = 7,//决赛奖励(白银)
						TYPE_8 = 8;//决赛奖励(黄金)
	
	
	public GodsWarPrizeSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GODSWARPRIZE);
	}
	
	public static GodsWarPrizeSampleManager Instance {
		get{
			if(instance==null)
				instance=new GodsWarPrizeSampleManager();
			return instance;
		}
	}
	public GodsWarPrizeSample getSampleBySid(int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as GodsWarPrizeSample;
	}
	//获得连胜奖励信息
	public List<GodsWarPrizeSample> getWinStreak ()
	{
		return list;
	}
	//获得积分奖励信息
	public List<GodsWarPrizeSample> getIntegralRank ()
	{
		return listIntegralRank;
	}

    public List<GodsWarPrizeSample> getIntegralRankYin()
    {
        return listIntegralRankYin;
    }
    public List<GodsWarPrizeSample> getIntegralRankJin()
    {
        return listIntegralRankJin;
    }

	//获得最高连胜奖励信息
	public List<GodsWarPrizeSample> getChampionPrize ()
	{
		return listFinall;
	}
	//获得积分奖励信息
	public List<GodsWarPrizeSample> getIntegralPrize ()
	{
		return listIntegral;
	}
	//获得决赛青铜奖励信息
	public List<GodsWarPrizeSample> getFinalBronzePrize ()
	{
		return listFianlAwardBronze;
	}
	//获得决赛白银奖励信息
	public List<GodsWarPrizeSample> getFinalSilverPrize ()
	{
		return listFianlAwardSilver;
	}
	//获得决赛黄金奖励信息
	public List<GodsWarPrizeSample> getFinalGoldPrize ()
	{
		return listFianlAwardGold;
	}
	//获得竞猜奖励信息
	public List<GodsWarPrizeSample> getSuportPrize ()
	{
		return listSuport;
	}
	//获取每日最大积分
	public int getEveryDayMaxIntegral()
	{
		int max =0;
		foreach(GodsWarPrizeSample s in listIntegral)
		{
			if(max<s.integral)
				max=s.integral;
		}
		return max;
	}
	/// <summary>
	/// 获取积分阶段列表信息
	/// </summary>
	public List<int> getIntegralList()
	{
		List<int> aa = new List<int>();
		foreach(GodsWarPrizeSample s in listIntegral)
		{
			if(s.integral>0)
				aa.Add(s.integral);
		}
		return aa;
	}

	//解析配置
	public override void parseConfig (string str)
	{ 
		GodsWarPrizeSample be = new GodsWarPrizeSample (str);
		if(be.type == TYPE_1)
		{
			if(listFinall == null)
				listFinall = new List<GodsWarPrizeSample>();
			listFinall.Add(be);
		}
		else if(be.type == TYPE_2)
		{
			if(list == null)
				list = new List<GodsWarPrizeSample>();
			list.Add(be);
		}
		else if(be.type == TYPE_3)
		{
			if(listIntegralRank == null)
				listIntegralRank = new List<GodsWarPrizeSample>();
			listIntegralRank.Add(be);
		} else if (be.type == TYPE_33) {
            if (listIntegralRankYin == null)
                listIntegralRankYin = new List<GodsWarPrizeSample>();
            listIntegralRankYin.Add(be);
        } else if (be.type == TYPE_333) {
            if (listIntegralRankJin == null)
                listIntegralRankJin = new List<GodsWarPrizeSample>();
            listIntegralRankJin.Add(be);
        }
		else if(be.type == TYPE_4)
		{
			if(listIntegral == null)
				listIntegral = new List<GodsWarPrizeSample>();
			listIntegral.Add(be);
		}
		else if(be.type == TYPE_5)
		{
			if(listSuport == null)
				listSuport = new List<GodsWarPrizeSample>();
			listSuport.Add(be);
		}
		else if(be.type == TYPE_6)
		{
			if(listFianlAwardBronze == null)
				listFianlAwardBronze = new List<GodsWarPrizeSample>();
			listFianlAwardBronze.Add(be);
		}
		else if(be.type == TYPE_7)
		{
			if(listFianlAwardSilver == null)
				listFianlAwardSilver = new List<GodsWarPrizeSample>();
			listFianlAwardSilver.Add(be);
		}
		else if(be.type == TYPE_8)
		{
			if(listFianlAwardGold == null)
				listFianlAwardGold = new List<GodsWarPrizeSample>();
			listFianlAwardGold.Add(be);
		}

	}

}
