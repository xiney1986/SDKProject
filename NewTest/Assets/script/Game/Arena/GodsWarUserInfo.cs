using UnityEngine;
using System.Collections;

/// <summary>
/// 诸神战小组赛用户信息
/// gc
/// </summary>
public class GodsWarUserInfo
{
	#region 公用信息

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

	#endregion

	#region 我的信息
   
	/** 大组 */
    public string bigTeam;
	/** 小组 */
	public string smallTeam;
	/** 排名 */
    public int rank;
	/** 今日积分 */
    public int todayIntegral;
	/** 总积分 */
	public int totalIntegral;
	/** 连胜次数 */
	public int numOfWinning;
	/** 已用挑战次数 */
	public int usedChallgeNum;

	#endregion

	#region 对手信息

	/** 对手等级 */
    public int level;
	/** 对手VIP等级 */
    public int vipLevel;
	/** 分组中的位置0-4 */
    public int massPosition = 0;
	/** 是否是NPC */
    public bool npc;
	/** 是否已经挑胜利战过 */
    public bool challengedWin;
	/** 战胜可获得积分 */
	public int winScore;
	/** 服务器名字 */
	public string serverName;

	#endregion

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
