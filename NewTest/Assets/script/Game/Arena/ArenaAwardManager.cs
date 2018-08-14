using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 奖励管理器
/// </summary>
public class ArenaAwardManager {
	/** 单例 */
	private static ArenaAwardManager instance;
	public static ArenaAwardManager Instance{
		get{
			if(instance ==null)
				instance = new ArenaAwardManager();
			return instance;
		}
	}
	/** 积分奖励信息 */
	public readonly List<ArenaAwardInfo> integralAwardInfos = new List<ArenaAwardInfo>();
	/// <summary>
	/// 获取我的积分
	/// </summary>
	public int getMyIntegral(){
		if (integralAwardInfos == null || integralAwardInfos.Count == 0) 
			return 0;
		return integralAwardInfos [0].condition;
	}

	/// <summary>
	/// 获取当前档积分奖励信息
	/// </summary>
	public PrizeSample []  getCurrentIntegralAward(){
		if (integralAwardInfos == null)
			return null;
		foreach (ArenaAwardInfo info in integralAwardInfos) {
			if(info.received)
				continue;
			return info.sample.prizes;
		}
		/** 如果都领完了，显示最后一档 */
		return integralAwardInfos[integralAwardInfos.Count-1].sample.prizes;
	}
	/// <summary>
	/// 获取当前档的积分可领取信息
	/// </summary>
	public ArenaAwardInfo getCurrentIntegralAwardInfo(){
		if (integralAwardInfos == null)
			return null;
		foreach (ArenaAwardInfo info in integralAwardInfos) {
			if(info.received)
				continue;
			return info;
		}
		/** 如果都领完了，显示最后一档 */
		return integralAwardInfos[integralAwardInfos.Count-1];
	}
    /// <summary>
    /// 判断指定奖励是否可领取
    /// </summary>
    public bool awardCanReceive(ArenaAwardSample tmp)
    {
        if (integralAwardInfos == null)
            return false;
        int integralNum = ArenaManager.instance.finalMyIntergal;//(ArenaManager.instance.self == null ? 0 : ArenaManager.instance.self.integral);
        foreach (ArenaAwardInfo info in integralAwardInfos)
        {
            if (info.sample.sid == tmp.sid && tmp.condition <= integralNum)
                return true;
        }
        return false;
    }
    public ArenaAwardInfo getArenaAwardInfo(ArenaAwardSample tmp)
    {
        if (integralAwardInfos == null)
            return null;
        int integralNum = (ArenaManager.instance.self == null ? 0 : ArenaManager.instance.self.integral);
        foreach (ArenaAwardInfo info in integralAwardInfos)
        {
            if (info.sample.sid == tmp.sid && tmp.condition <= integralNum)
                return info;
        }
        return null;
    }
	
	/// <summary>
	/// 获取下一档的积分奖励信息
	/// </summary>
	public ArenaAwardInfo getNextCanGetIntegralAwardInfo(){
		if (integralAwardInfos == null)
			return null;
		for (int i=0; i<integralAwardInfos.Count; ++i) {
			if(integralAwardInfos[i].received)
				continue;
			if(i!=integralAwardInfos.Count -1){
				return integralAwardInfos[i+1];
			}
		}
		return null;
	}

	/// <summary>
	/// 当前档的积分奖励是否可以领取
	/// </summary>
	public bool isCanGetIntegralAward(){
		ArenaAwardInfo info = getCurrentIntegralAwardInfo ();		
		if (info == null )
			return false;
		if (info.condition >= info.sample.condition && !info.received)
			return true;
		else 
			return false;

	}
}
