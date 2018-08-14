using UnityEngine;
using System.Collections;

/// <summary>
/// 竞技场用户信息
/// 可用于自己,也可用于对手
/// yxl
/// </summary>
public class ArenaUserInfo
{

    //公用信息
	/** 我的UID */
    public string uid;
	/** 我的昵称 */
    public string name;
	/** 我的头像ID */
    public int headIcon;
	/** 当前决赛进度是否失败 */
    public bool lose;
	/** 当前决赛进度 */
    public int finalProgress;

    //我的信息
	/** 分组 */
    public int team;
	/** 排名 */
    public int rank;
	/** 积分 */
    public int integral;
    //对手信息
	/** 对手等级 */
    public int level;
	/** 对手VIP等级 */
    public int vipLevel;
	/** 海选中的位置0-4 */
    public int massPosition;
	/** 是否是NPC */
    public bool npc;
	/** 是否已经挑胜利战过 */
    public bool challengedWin;


    /// <summary>
    /// 获取积分奖励
    /// </summary>
    public int getIntegralAward()
    {
        return ArenaMassArardSampleManager.Instance.getSampleBySid(massPosition).integral;
    }
    
    /// <summary>
    /// 获取功勋奖励
    /// </summary>
    public int getMeritAward()
    {
        return ArenaMassArardSampleManager.Instance.getSampleBySid(massPosition).merit;
    }
}
