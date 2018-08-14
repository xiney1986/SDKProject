using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankManagerment
{
	public const int TYPE_COMBAT = 1;//队伍
	public const int TYPE_PVP = 2;//pvp
	public const int TYPE_MONEY = 3;//富豪
	public const int TYPE_ROLE = 4;//卡片
	public const int TYPE_GUILD = 5;//公会
	public const int TYPE_ROLE_LV = 6;//玩家等级
	public const int TYPE_ROLE_CARD = 99;//点击卡片信息
	public const int TYPE_GODDESS = 7;//女神
	public const int TYPE_LADDER = 8;//天梯
	public const int TYPE_LUCKY_CARD = 9;// 限时卡片积分
	public const int TYPE_LUCKY_EQUIP = 10;// 限时装备积分
	public const int TYPE_LUCKY_LIEHUN = 14;//限时猎魂
	public const int TYPE_LUCKY_LIANJIN = 15;//限时炼金
	public const int TYPE_GUILD_SHAKE = 11;//公会摇骰子积分
	public const int TYPE_GUILD_AREA_CONTRIBUTION = 12; //公会战贡献榜
	public const int TYPE_GUILD_FIGHT = 13;//公会战评分排行榜
    public const int TYPE_BOSSDAMAGE = 20;//恶魔挑战伤害排行榜
	public const int TYPE_LASTBATTLE = 21;// 末日决战积分排行榜//

	public readonly List<RankItemCombat> combatList = new List<RankItemCombat>();
	public readonly List<RankItemPVP> pvpList = new List<RankItemPVP>();
	public readonly List<RankItemMoney> moneyList = new List<RankItemMoney>();
	public readonly List<RankItemRole> roleList = new List<RankItemRole>();
	public readonly List<RankItemGuild> guildList = new List<RankItemGuild>();
	public readonly List<RankItemRoleLv> roleLvList = new List<RankItemRoleLv>();//角色等级排行
	public readonly List<RankItemGoddess> goddessList = new List<RankItemGoddess>();//女神
	public readonly List<PvpOppInfo> ladderList=new List<PvpOppInfo>();//天梯
	public readonly List<RankItemLuckyCard> luckyCardList=new List<RankItemLuckyCard>();//活动卡片积分排行
	public readonly List<RankItemLuckyEquip> luckyEquipList=new List<RankItemLuckyEquip>();//活动装备积分排行
	public readonly List<RankItemLuckyLiehun> luckyLiehunList = new List<RankItemLuckyLiehun>();//限时猎魂排行
	public readonly List<RankItemLuckyLianjin> luckyLianjinList = new List<RankItemLuckyLianjin>();//限时炼金排行
	public readonly List<GuildShakeRankItem> guildShakeList = new List<GuildShakeRankItem>();//公会幸运女神积分排名信息
	public readonly List<GuildAreaHurtRankItem> guildAreaHurtList = new List<GuildAreaHurtRankItem>();//公会战贡献榜
	public readonly List<RankItemGuildFight> guildFightJudgeList = new List<RankItemGuildFight>();//公会战评分排行榜
    public readonly List<RankItemTotalDamage> totalDamageList = new List<RankItemTotalDamage>();//恶魔挑战伤害排行榜
	public readonly List<LastBattleRank> lastBattleRankList = new List<LastBattleRank>();// 末日决战积分排行榜//


	public readonly Dictionary<int,int> myRank = new Dictionary<int,int>(); //缓存各榜自己的排名
	public readonly Dictionary<int,int> nextTime = new Dictionary<int,int>(); //缓存各榜下次刷新时间

	public bool updateRankItemTotalDamage = false;// 是否刷新魔挑战伤害排行榜//

	public static RankManagerment Instance{
		get{return SingleManager.Instance.getObj("RankManagerment") as RankManagerment;}
	}

	public void loadData(int type,CallBack callback)
	{
		if(type==TYPE_LADDER)
		{
			RankLadderFPort fport=FPortManager.Instance.getFPort<RankLadderFPort>();
			fport.apply(callback);
		}else
		{
			RankFPort fport = FPortManager.Instance.getFPort ("RankFPort") as RankFPort;
			fport.access (type,callback);
		}
	}

	public int getNextUpdateTime(int type)
	{
		if (nextTime.ContainsKey(type)) {
			return nextTime[type];
		}
		return 0;
	}

	/// <summary>
	/// 根据类型获取我的排名,不包含公会
	/// </summary>
	public int getMyRank(int type)
	{
		if (!myRank.ContainsKey (type))
			return 0;
		return myRank [type];
	}

    /// <summary>
    /// 获得我的公会排名
    /// </summary>
    public int getMyGuildRank()
    {
        Guild guild = GuildManagerment.Instance.getGuild();
        if (guild == null)
            return 0;
        int rank = 0;
        for(int i = 0; i < guildList.Count; i++)
        {
            if(guildList[i].gid == guild.uid)
            {
                rank = i + 1;
                break;
            }
        }
        return rank;
    }
}
