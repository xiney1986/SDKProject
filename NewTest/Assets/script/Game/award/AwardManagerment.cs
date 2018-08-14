using System;
using System.Collections.Generic;
 
/**
 * 奖励管理器
 * @authro longlingquan  
 * */
public class AwardManagerment
{
	 //奖励事件，不需展示的奖励类型发nor 
	public const string AWARDS_FUBEN_RES = "fb_res";//副本资源 
	public const string AWARDS_FUBEN_OVER = "fb_over";//副本结束
	public const string AWARDS_BATTLE = "battle";//副本战斗
	public const string AWARDS_BOX = "box";//开道具礼包
	public const string AWARDS_INVITE_SDKAWARD = "momo_invite";//SDK邀请奖励
	public const string AWARDS_PVP_DOUBLE="pvp_double";//pvp连胜奖励
	public const string AWARDS_ACHIEVE = "achieve";//任务完成
    public const string AWARDS_HERO_ROAD = "hero_road";//英雄之章完成
    public const string AWARDS_STAR_PRAY = "star_pray";//星座祈祷
    public const string AWARDS_ARENA = "arena";//竞技场
	public const string DIVINATION = "divination";//占卜
	public const string AWARDS_INVITE_EVENT = "gift_code";//礼品码
	public const string AWARDS_LADDER = "ladder";//天梯奖励
	public const string AWARDS_LUCKY_CARD = "luckyCard";//限时抽卡片
	public const string AWARDS_LUCKY_EQUIP = "luckyEquip";//限时抽装备
	public const string AWARDS_GUILD_WAR = "guild_war";//公会战
	public const string AWARDS_MINERAL_WAR = "mineral_fight";//抢矿
	public const string AWARDS_GODSWAR_GROUP= "god_war_fight";//神战小组战
    public const string BOSS_INFO_AWARD = "fight_boss";//恶魔挑战
	public const string LASTBATTLE_AWARD = "armageddon_fight";//末日决战
	public const string LASTBATTLE_BOSS_AWARD = "armageddon_fight_boss";// 末日决战boss战奖励//
	public const string LASTBATTLE_BASE_AWARD = "base_award";// 末日决战boss战奖励中基础奖励类型//
	public const string LASTBATTLE_FINALKILL_AWARD = "kill_award";// 末日决战boss战奖励中最后一击奖励类型//

	//一次奖励事件发很多奖励,通过下面确认奖励类型
	//副本外的很多奖励基本不需要考虑这个参数
	public const string PVE = "pve";//副本pve奖励
	public const string PVP = "pvp";//副本pvp奖励
	public const string MNGV = "mngv";//宝箱奖励
	public const string RES = "resource";
    public const string FB_END = "fb_end";//副本完成奖励
    public const string FIRST = "first";//首通奖励
    public const string ARENA = "a_mass";//竞技场海选战斗奖励
    public Award[] miaoShaAward;


	public static AwardManagerment Instance {
		get{return SingleManager.Instance.getObj("AwardManagerment") as AwardManagerment;}
	}  


	//注入回调函数
	public void addFunc (string type, CallBackAwards callback)
	{  
		AwardCache cache = AwardsCacheManager.getAwardCache (type);
		cache.addFunc (callback);
	}

	
	public void clearCacheByType(string type)
	{
		AwardCache cache = AwardsCacheManager.getAwardCache(type);
		cache.clear();
	}
	 
	//注入奖励
	//添加新奖励类型时需要修改此方法
	public void addAwards (string type, Award[] awards)
	{  
		AwardCache cache = AwardsCacheManager.getAwardCache (type);
		//设置奖励
		if (type == AWARDS_FUBEN_RES || type == AWARDS_FUBEN_OVER
		    || type == AWARDS_BATTLE || type == AWARDS_BOX
		    || type==AWARDS_PVP_DOUBLE    ||type==AWARDS_ACHIEVE
            || type == AWARDS_HERO_ROAD || type == AWARDS_STAR_PRAY
		    || type == DIVINATION || type == AWARDS_INVITE_EVENT
		    || type == AWARDS_LADDER ||type == AWARDS_LUCKY_CARD 
		    ||type == AWARDS_LUCKY_EQUIP || type == AWARDS_INVITE_SDKAWARD 
		    || type == AWARDS_GUILD_WAR  || type == AWARDS_MINERAL_WAR
		    ||type == AWARDS_GODSWAR_GROUP || type == BOSS_INFO_AWARD || type == LASTBATTLE_AWARD || type == LASTBATTLE_BOSS_AWARD)
        { 
			cache.setAwards (awards);
		} 
		//添加奖励
		else
        {

		}
	} 
}
 
public delegate void CallBackAwards (Award[] awards);
 