using System;
/**
 * 经验模板管理器
 * @author longlingquan
 * */
using UnityEngine;

public class EXPSampleManager:SampleConfigManager
{
	//单例
	private static EXPSampleManager instance;
	private const int PART = 20;
	/** 玩家经验引索 */
	public const int SID_USER_EXP = 1;
	/** 玩家vip经验索引 */
	public const int SID_VIP_EXP = 3;
	/** 圣器经验SID */
	public const int SID_HALLOW_EXP = 4;
	/** 称号经验SID */
	public const int SID_PRESTIGE_EXP = 5;
	/** 玩家属性附加经验索引 */
    public const int SID_USER_ATTR_ADD_EXP = 66;
	/** 骑术经验索引 */
	public const int SID_MOUNTS_EXP = 78;
	/** 进化经验索引 */
	public const int SID_EVO_EXP = 79;

	public EXPSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_EXP);
	}

	public static EXPSampleManager Instance {
		get{
			if(instance==null)
				instance=new EXPSampleManager();
			return instance;
		}
	}
	
	//获得当前等级经验下限
	public long getEXPDown (int sid, int level)
	{
		if (level == 0)
			return 0;
		
		long[] exps = getEXPSampleBySid (sid).getExps ();
		
		if (level >= exps.Length)
			return exps [level - 1];
		else
		return exps [level - 1];
	}
    /// <summary>
    /// 获得精炼经验下限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public long getRefineEXPDown(int sid, int level)
    {
        if (level == 0)
            return 0;

        long[] exps = getEXPSampleBySid(sid).getExps();

        if (level >= exps.Length-1)
            return exps[level];
        else
            return exps[level];
    }
	//获得当前等级经验上限
	public long getEXPUp (int sid, int level)
	{
		long[] exps = getEXPSampleBySid (sid).getExps ();
		
		if (level >= exps.Length)
			return exps [level - 1];
		else
			return exps [level];
	}
    /// <summary>
    /// 获取精炼等级经验上限
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public long getRefineEXPUp(int sid, int level)
    {
        long[] exps = getEXPSampleBySid(sid).getExps();

        if (level >= exps.Length-1)
            return exps[level];
        else
            return exps[level+1];
    }
	//返回当前需要升级的经验总值
	public long getMaxEXPShow(int sid,long exp)
	{
		return getEXPUp(sid,getLevel(sid,exp)) - getEXPDown(sid,getLevel(sid,exp));
	}
	
	//返回已有的当前等级经验
	public long getNowEXPShow(int sid,long exp)
	{
		return (exp - getEXPDown(sid,getLevel(sid,exp)));
	}
	
	//返回经验条显示，通例
	public string getExpBarShow(int sid,long exp)
	{
		long expMax = getMaxEXPShow(sid,exp);
		long expNow = getNowEXPShow(sid,exp);
		if(expNow >= expMax)
			return "NA/NA";
		else
			return expNow + "/" + expMax;
	}
	
	//获得经验值模板对象
	public EXPSample getEXPSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as EXPSample;
	}
	
	//获得指定sid对象exp对应的等级 分块查询
	public int getLevel (int sid, long exp)
	{
		if (sid == 0)
			return 0;
		long[] exps = getEXPSampleBySid (sid).getExps (); 
		int len = exps.Length;
		int part = getEXPPart (len); 
		long[] points = new long[part];//分组后临界点值 有序数组
		
		for (int i=0; i<part; i++) { 
			if (i * PART >= len)
				points [i] = exps [len - 1];
			else
				points [i] = exps [i * PART];
		} 
		
		int start = 0;
		for (int i=0; i<points.Length; i++) {
			if (exp < points [i]) { 
				break;
			} 
			start = i * PART;
		}  
		
		for (int i=start; i<len; i++) {
			if (exps [i] > exp)
				return i;
		}  
		//max
		return len;
	}
	
	//获得经验数组分段数
	private int getEXPPart (int len)
	{ 
		return (int)len / PART + 1;
	}

	public long getMaxExp (int sid)
	{
		long[] exps = getEXPSampleBySid (sid).getExps ();
		return exps [exps.Length - 1];
	}
    /// <summary>
    /// 获取最大等级
    /// </summary>
    /// <returns></returns>
    public int getMaxLevel(int sid)
    {
        long[] exps = getEXPSampleBySid(sid).getExps();
        return exps.Length;
    }
	
	//获得指定sid对象exp对应的等级(提供原始level)  
	public int getLevel (int sid, long exp, int oldLevel)
	{ 
		if (oldLevel == 0)
			return getLevel (sid, exp);
		long[] exps = getEXPSampleBySid (sid).getExps ();
		int len = exps.Length;
		for (int i=oldLevel; i<len; i++) {
			if (exps [i] > exp)
				return i;
		}
		//max
		return len;
	}

    //获得精炼对象EXP的对应的等级
    public int getRefineLevel(int sid, long exp, int oldLevel)
    {
        if (oldLevel == 0)
            return getRefineLevel(sid, exp);
        long[] exps = getEXPSampleBySid(sid).getExps();
        int len = exps.Length;
        for (int i = oldLevel; i < len; i++)
        {
            if (exps[i] > exp)
                return i;
        }
        return len;
    }
    public int getRefineLevel(int sid, long exp)
    {
        if (sid == 0)
            return 0;
        long[] exps = getEXPSampleBySid(sid).getExps();//[0,1,2,4,8,10,14,16,18,20,100]
        int len = exps.Length;
        for (int i = 0; i < len-1; i++)
        {
            if (exps[i] <= exp&&exps[i+1]>exp)
                return i;
            
        }
        return len-1;
    } 
	 
	//解析经验值数据
	public override void parseSample (int sid)
	{
		EXPSample sample = new EXPSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}  