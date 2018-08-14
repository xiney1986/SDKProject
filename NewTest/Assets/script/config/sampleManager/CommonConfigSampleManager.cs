using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
/// <summary>
/// 通用简单配置模版
/// </summary>
public class CommonConfigSampleManager : SampleConfigManager {
	/** 精灵翻翻乐消耗模版SID */
	public const int HappyTurnSpriteCost_SID = 1;
    /** 行动力消耗模版SID */
    public const int PvePowerMax_SID = 2;
    /** 天梯好友助战消耗模版SID */
    public const int LadderFriendsHelp_SID = 4;
	/** 行动力购买消耗模版SID */
	public const int PveBuyCostRMB_SID = 7;
	/** 超级奖池获得兑换最大积分 */
	public const int SuperDraw_SID = 8;

	private static CommonConfigSampleManager instance;
	public static CommonConfigSampleManager Instance{
		get{
			if(instance == null)
				instance = new CommonConfigSampleManager();
			return instance;
		}
	}
	public CommonConfigSampleManager(){
		base.readConfig (ConfigGlobal.CONFIG_COMMON_CONFIG);
	}

	public T getSampleBySid<T> (int sid)where T : Sample {
		
		if (!samples.ContainsKey (sid)) {
			T t = Activator.CreateInstance<T> ();
			t.parse(sid,data[sid].ToString());
			samples.Add(sid,t);
		}
		return samples[sid] as T;
	}

}

/// <summary>
/// 精灵翻翻乐消耗模版
/// </summary>
public class HappyTurnSpriteCostSample :Sample{
	public int sid;
	public List<int> costs = new List<int>();
	public string des;
	public override void parse (int sid, string str)
	{
		base.parse (sid, str);
		string [] strs = str.Split ('|');
		this.sid = sid;
		parseCosts (strs [1]);
		des = strs [2];

	}

	void parseCosts(string str){
		string [] strs = str.Split ('#');
		foreach (string s in strs) {
			costs.Add(StringKit.toInt(s));
		}
	}

	public int getCostByCount(int count){
		if (costs.Count == 0)
			return 0;
		if (count > costs.Count)
			return costs [costs.Count - 1];
		return costs[count];
	}
}

public class PvePowerMaxSample : Sample {
    public int sid;
    public  int pvePowerMax;
	public int heroEatPve;
    public override void parse(int sid, string str)
    {
        this.sid = sid;
        string[] strs = str.Split('|');
        pvePowerMax = StringKit.toInt(strs[1]);
		heroEatPve = StringKit.toInt(strs[2]);
    }
}
public class SuperDrawMaxSample : Sample {
	public int sid;
	public  int max;
	public  int prizeSid;
	public int exchangeTime;
	public override void parse(int sid, string str)
	{
		this.sid = sid;
		string[] strs = str.Split('|');
		max = StringKit.toInt(strs[1]);
		prizeSid = StringKit.toInt(strs[2]);
		exchangeTime = StringKit.toInt (strs[3]);

	}
}
/// <summary>
/// 天梯好友助战消耗模版
/// </summary>
public class LadderFriendsHelpCostSample:Sample{
    public List<int> costs = new List<int>();
    public override void parse(int sid, string str)
    {
        base.parse(sid, str);
        string[] strs = str.Split('|');
        this.sid = sid;
        parseCosts(strs[1]);

    }

    void parseCosts(string str)
    {
        string[] strs = str.Split('#');
        foreach (string s in strs)
        {
            costs.Add(StringKit.toInt(s));
        }
    }

    public int getCostByCount(int count)
    {
        if (costs.Count == 0)
            return 0;
        if (count >= costs.Count)
            return costs[costs.Count - 1];
        return costs[count];
    }
}
/// <summary>
/// 行动力购买消耗模版
/// </summary>
public class PveBuyCostRMB:Sample{
	public List<int> costs = new List<int>();
	public override void parse(int sid, string str)
	{
		base.parse(sid, str);
		string[] strs = str.Split('|');
		this.sid = sid;
		parseCosts(strs[1]);
		
	}
	
	void parseCosts(string str)
	{
		string[] strs = str.Split(',');
		foreach (string s in strs)
		{
			costs.Add(StringKit.toInt(s));
		}
	}
	
	public int getCostByCount(int count)
	{
		if (costs.Count == 0)
			return 0;
		if (count >= costs.Count)
			return costs[costs.Count - 1];
		return costs[count];
	}
}
public class FubenBuyChallengeTimesCostSample : Sample
{
    public int sid;
    public string prices;
    public override void parse(int _sid, string str)
    {
        sid = _sid;
        string[] strs = str.Split('|');
        prices = strs[1];
    }
    public string getPricesString()
    {
        return prices;
    }
}
public class ArenaNumsSample : Sample
{
    public int sid;
    public string prices;
    public override void parse(int _sid, string str)
    {
        sid = _sid;
        string[] strs = str.Split('|');
        prices = strs[1];
    }
    public string getTimesString()
    {
        return prices;
    }
    public int getTimesInt()
    {
        return StringKit.toInt(prices);
    }
}
public class DailyRebateSample : Sample
{
    public int sid;
    public string[] times;
    public override void parse(int _sid, string str)
    {
        sid = _sid;
        times = str.Split('|');
    }
    public string getTimesString(int index)
    {
        return times[index];
    }
    public int getTimesInt(int index)
    {
        return StringKit.toInt(times[index]);
    }
}