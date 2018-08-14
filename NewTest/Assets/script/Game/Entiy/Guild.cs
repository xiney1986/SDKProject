using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会实体类 
 * @author 汤琦
 **/
public class Guild
{
    public Guild()
    {

    }
    public Guild(string uid, string name, int level, int membership, int membershipMax, int livenessing, int livenessed, string declaration, string notice, string presidentName, int job, int contributioning, int contributioned, List<GuildMsg> msgs, int todayDonateTimes, bool isCanRename, int autoJoin, int joinTime, int firstAward)
    {

        this.uid = uid;
        this.name = name;
        this.level = level;
        this.membership = membership;
        this.membershipMax = membershipMax;
        this.livenessing = livenessing;
        this.livenessed = livenessed;
        this.declaration = declaration;
        this.notice = notice;
        this.presidentName = presidentName;
        this.job = job;
        this.contributioning = contributioning;
        this.contributioned = contributioned;
        this.msgs = msgs;
        this.todayDonateTimes = todayDonateTimes;
        this.isCanRename = isCanRename;
        this.autoJoin = autoJoin;
        this.joinTime = joinTime;
        this.firstAward = firstAward;

    }
    public string uid = "";//uid
    public string name = "";//公会名称
    public int level = 0;//公会等级
    public int membership = 0;//人数
    public int membershipMax = 0;//人数上限
    public int livenessing = 0;//当前活跃度
    public int livenessed = 0;//历史活跃度
    public string declaration = "";//宣言
    public string notice = "";//公告
    public string presidentName = "";//公会会长名称
    public int job = 0;//职务
    public int contributioning = 0;//当前贡献值
    public int contributioned = 0;//历史贡献值 
    public List<GuildMsg> msgs;
    public int todayDonateTimes; //今天捐献次数
    public bool isCanRename; //是否可以重命名公会	
    public int autoJoin = 0;//自动允许加入默认0 不允许
    public int joinTime = 0;//入会时间
    public int firstAward = 0;//是否首次捐献过
}
/**
 * 公会公会消息集
 * @author 汤琦
 **/
public class GuildMsg
{
    public GuildMsg(string content)
    {
        this.content = content;
    }
    public int issueTime = 0;//发布时间
    public string content = "";//内容
}

/**
 * 公会成员实体类
 * @author 汤琦
 **/
public class GuildMember
{
    public GuildMember() { }
    public GuildMember(string uid, int icon, int vipLevel, string name, int level, int job, int contributioning, int contributioned, int lastLogin, int lastLogout, int donating, int donated)
    {
        this.uid = uid;
        this.icon = icon;
        this.name = name;
        this.level = level;
        this.job = job;
        this.contributioning = contributioning;
        this.contributioned = contributioned;
        this.lastLogin = lastLogin;
        this.lastLogout = lastLogout;
        this.donating = donating;
        this.donated = donated;
        this.vipLevel = vipLevel;
    }

    public string uid = "";
    public int icon = 0;//头像id
    public int vipLevel = 0;//VIP等级
    public string name = "";//玩家名称
    public int level = 0;//玩家等级
    public int job = 0;//4-会员 3-官员 2-副会长 1-会长
    public int contributioning = 0;//当前贡献值
    public int contributioned = 0;//历史贡献值
    public int lastLogin = 0;//最近登录时间
    public int lastLogout = 0;//最近登出时间
    public int donating = 0;//当前捐献值
    public int donated = 0;//历史捐献值
}
/**
 * 公会审批信息类
 * @author 汤琦
 **/
public class GuildApprovalInfo
{
    public GuildApprovalInfo(string uid, string name, int level, int _vipLevel, string headIcon)
    {
        this.uid = uid;
        this.name = name;
        this.level = level;
        this.vipLevel = _vipLevel;
        this.headIcon = headIcon;
    }
    public string uid;
    public string name;
    public int level;
    public int vipLevel;
    public string headIcon;
}
//公会建筑
public class GuildBuild
{
    public GuildBuild(string id, int level)
    {
        this.id = id;
        this.level = level;
    }
    public string id;
    public int level;//等级
}
//公会物品
public class GuildGood : CloneObject
{
    public GuildGood(int sid, int level)
    {
        this.sid = sid;
        this.level = level;
    }
    public int sid;//sid
    public int level;//限制等级

    public override void copy(object destObj)
    {
        base.copy(destObj);
    }
}
//公会成员伤害
public class GuildRastInfo
{
    public GuildRastInfo(int sid, string playerName, int rast)
    {
        this.sid = sid;
        this.playerName = playerName;
        this.rast = rast;
    }
    public int sid;//sid
    public string playerName;
    public int rast;//
}
//公会幸运女神主界面信息
public class GuildLuckyNvShenInfo
{
    public GuildLuckyNvShenInfo(int selfIntegral, int guildIntegral, int topIntegral, int shakeCount, int reShakeCount)
    {
        this.selfIntegral = selfIntegral;
        this.guildIntegral = guildIntegral;
        this.topIntegral = topIntegral;
        this.shakeCount = shakeCount;
        this.reShakeCount = reShakeCount;
    }
    public int selfIntegral = 0; //个人积分
    public int guildIntegral = 0; //公会积分
    public int topIntegral = 0;   //本周最高积分
    public int shakeCount = 0;    //投掷次数
    public int reShakeCount = 0; //重新投掷次数

}


//公会幸运女神投掷结果信息
public class GuildLuckyNvShenShakeResult
{
    /** 原始结果，用于骰子的结果显示*/
    private string[] results;
    /** 解析后的结果，用来计算奖励结果*/
    private Hashtable resultTable = new Hashtable();
    public GuildLuckyNvShenShakeResult(string[] results)
    {
        this.results = results;
        parseResult();
    }

    /// <summary>
    /// 解析结果，构成<sid,num>的KV组合
    /// </summary>
    private void parseResult()
    {
        int key = 0;
        int num = 0;
        for (int i = 0; i < results.Length; ++i)
        {
            key = ShakeEblowsRewardSampleManager.convertNameToSid(results[i]);
            num = 0;
            if (resultTable.ContainsKey(key))
                continue;
            for (int j = i; j < results.Length; ++j)
            {
                int tempKey = ShakeEblowsRewardSampleManager.convertNameToSid(results[j]);
                if (tempKey == key)
                    num++;
            }
            resultTable.Add(key, num);
        }

    }

    public string[] getResultsString()
    {
        return results;
    }

    public Hashtable getResultTable()
    {
        return resultTable;
    }

    /// <summary>
    /// 是否为5个不同的面
    /// </summary>
    public bool isFiveDifferentResult()
    {
        int noneSid = ShakeEblowsRewardSampleManager.convertNameToSid("none");
        //五个不同面，且不包含女
        if (resultTable.Count == GuildManagerment.EBLOWS_MAXNUM && !resultTable.ContainsKey(noneSid))
            return true;
        else
            return false;
    }


}

public class GuildFightInfo
{

    public GuildFightInfo(int state, List<GuildAreaPreInfo> areas, bool get_power, List<string> messageList, int openTime, int weekActivi, int selfWarNum,bool isDead,int curBlood,int maxBlood)
    {
        this.state = state;
        this.areas = areas;
        this.get_power = get_power;
        this.messageList = messageList;
        this.openTime = openTime;
        this.weekActivi = weekActivi;
        this.selfWarNum = selfWarNum;
        this.isDead = isDead;
        this.curBlood = curBlood;
        this.maxBlood = maxBlood;
    }
    /** 公会战状态 */
    public int state;
    /** 公会战开启时间 */
    public int openTime;
    /** 领地建筑信息 */
    public List<GuildAreaPreInfo> areas;
    /** 是否可领取行动值 */
    public bool get_power;
    /** 公会战消息列表 */
    public List<string> messageList;
    /** 自身战争点 */
    public int selfWarNum;
    /** 本周活跃度 */
    public int weekActivi;
    /** 是否已经死亡 */
    public bool isDead;
    //*公会当前血量**/
    public int curBlood;
    //*公会最大血量**/
    public int maxBlood;
    public int getMyRank()
    {
        foreach (GuildAreaPreInfo area in areas)
        {
            if (area.uid == GuildManagerment.Instance.getGuild().uid)
                return area.rank;
        }
        return 0;
    }
}


/// <summary>
/// 公会领地预览信息
/// </summary>
public class GuildAreaPreInfo
{
    public GuildAreaPreInfo(string uid, string server, string name, int warNum, int judge, int state, int time, int defense, int attack)
    {
        this.uid = uid;
        this.server = server;
        this.name = name;
        this.warNum = warNum;
        this.judgeScore = judge;
        this.state = state;
        this.time = time;
        this.rank = rank;
        this.defense = defense;
        this.attack = attack;
    }

    public string getJudeString()
    {
        return GuildFightSampleManager.Instance().getJudgeString(judgeScore);
    }

    /** 获取倒计时 */
    public int getBackTime()
    {
        int serverTime = ServerTimeKit.getSecondTime();
        int resultTime = time - serverTime;
        return resultTime;
    }


    public string getBackTimeString()
    {
        int second = getBackTime();
        System.DateTime date = System.DateTime.Parse("00:00:00");
        date = date.AddSeconds(second);
        return date.ToString("mm:ss");
    }
    /** uid */
    public string uid;
    /** 服务器名 */
    public string server;
    /** 名称 */
    public string name;
    /** 战争点 */
    public int warNum;
    /** 评分 */
    public int judgeScore;
    /** 状态  1.可出击 2.战斗中 3.休战 */
    public int state;
    /** 击破后下次时间 */
    public int time;
    /** 排名 */
    public int rank;
    /** 位置 */
    public int position;
    /** 被我打了几次 */
    public int defense;
    /** 打了我几次 */
    public int attack;
    //***队伍血量百分比/
    public int bloodPercent;
}

/// <summary>
/// 公会领地
/// </summary>
public class GuildArea
{
    /** 祈福次数 */
    public int wishNum;
    /** 鼓舞次数 */
    public int inspireNum;
    /** 领地防守者信息 */
    public List<GuildAreaPoint> pointList;
    /** 已击杀 */
    public int hasKilled;
    /// <summary>
    /// 获取当前对象
    /// </summary>
    public GuildAreaPoint getTarget()
    {
        if (hasKilled == pointList.Count)
            return null;
        return pointList[hasKilled];
    }

    /// <summary>
    /// 获取当前攻击对象的索引
    /// </summary>
    /// <returns>The current index.</returns>
    public int getCurrentIndex()
    {
        return hasKilled + 1;
    }
}

/// <summary>
/// 公会领地防守者节点
/// </summary>
public class GuildAreaPoint
{
    /** 是否是NPC */
    public bool isNpc;
    /** 名字 */
    public string name;
    /** 人物头像 */
    public int headIconId;
    /** 最大血量 */
    public int bloodMax;
    /** 当前血量 */
    public int bloodNow;
    /** vip等级 */
    public int vipLevel;

    public GuildAreaPoint(string name, int headIconId, int bloodMax, int bloodNow, int vipLevel, bool isNpc)
    {
        this.name = name;
        this.headIconId = headIconId;
        this.bloodMax = bloodMax;
        this.bloodNow = bloodNow;
        this.vipLevel = vipLevel;
        this.isNpc = isNpc;
    }
    public string getHeadIconPath()
    {
        return UserManager.Instance.getIconPath(headIconId);
    }

    public string getName()
    {
        if (isNpc)
        {
            return LanguageConfigManager.Instance.getLanguage("GuildArea_65");
        } else
        {
            return name;
        }

    }
}







