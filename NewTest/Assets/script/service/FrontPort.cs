using System;
 
/**
 * 前台通讯端口列表
 * @author longlingquan
 * */
public class FrontPort
{ 
	public const string LNGIN = "/yxzh/user_login";//登录接口
	public const string LNGINGM = "/yxzh/trust";//GM登录接口
	public const string CREATE_ROLE = "/yxzh/create_role";//创建角色接口
    public const string FIGHT = "/yxzh/duplicate/fight"; //战斗接口    /yxzh/fb/get_fbinfo
	public const string FIGHTGM = "/yxzh/fb/get_zhanbao"; //GM登录他人帐号战斗接口    

	public const string INIT_STORAGE = "/yxzh/role/get_bundles";//仓库初始化
	public const string INIT_USER = "/yxzh/role/get_user";//角色初始化
	public const string INIT_USER_NEW = "/yxzh/role/get_user_new";//新的角色初始化

	public const string INIT_USER_PVP_INFO="/yxzh/role/get_pvp_info";//玩家初始化pvp信息
	//======================fuben===============================
	public const string FUBEN_INFO = "/yxzh/fb/get_fbinfo";//获得副本初始化信息
	public const string FUBEN_INTO = "/yxzh/fb/into_fb";//进入副本
	public const string FUBEN_CONTINUE = "/yxzh/fb/continue_fb";//继续副本
	public const string FUBEN_GOTO = "/yxzh/fb/goto_point";//前往下一个点
	public const string FUBEN_EXECUTE_EVENT = "/yxzh/fb/execute_event";//执行事件
	public const string FUBEN_USE_ARRAY = "/yxzh/fb/use_array";//使用阵型
	public const string FUBEN_SUSPEND = "/yxzh/fb/suspend";//暂停副本
	public const string FUBEN_ABANDON = "/yxzh/fb/abandon";//放弃副本
	public const string FUBEN_PVP = "/yxzh/fb/pvp_pk";//副本pvp
	public const string FUBEN_GET_BOSS = "/yxzh/fb/get_boss";//获得当前boss信息
	public const string FUBEN_GET_CURRENT = "/yxzh/fb/get_currentfb";//获得当前存在的副本信息
	public const string FUBEN_BUY_WAR_NUM = "/yxzh/fb/buy_crusade_times";//购买讨伐副本挑战次数
	public const string FUBEN_WAR_ATTACK = "/yxzh/fb/crusade_fb";//挑战讨伐副本
	public const string FUBEN_GET_EVENT_INFO = "/yxzh/fb/get_event_info";//获得副本战前信息(包含本方 对方信息)
	public const string FUBEN_GET_SELF_HP = "/yxzh/fb/get_self_hp";//获得本方的血量
	public const string FUBEN_REBIRTH = "/yxzh/fb/prop_revive";//复活卡片 并且回满血
	public const string FUBEN_RECOVER = "/yxzh/fb/prop_recover";//恢复满卡片血量
	public const string FUBEN_GET_PLAYERS = "/yxzh/fb/get_players";//获得部分角色信息
	public const string CLOSECONTECT = "/yxzh/close";//关闭注册链接
	public const string FUBEN_BUY_CHALLENGE_TIMES = "/yxzh/fb/reset_nightmare_fb_times";//副本购买挑战次数
	public const string FUBEN_BOUGHT_CHALLENGE_TIMES = "/yxzh/fb/get_reset_nightmare_usedtimes";//副本已购买挑战次数
	//======================fuben over===========================

	//=========================army==============================
	public const string ARMY_ADD = "/yxzh/array/add_array";//添加队伍
	public const string ARMY_GET = "/yxzh/array/get_array";//获取所有队伍
	public const string ARMY_UPDATE = "/yxzh/array/update_array";//修改队伍
	//======================army over===========================

	//=========================mining army==============================
	public const string MINING_ARMY_UPDATE = "/yxzh/array/update_mining_array";//修改队伍

	//======================army over===========================

	//=========================mining==============================
	public const string OPEN_MINERAL = "/yxzh/mineral_port/open_mineral";//开始采矿
	public const string GET_MINERALS = "/yxzh/mineral_port/get_minerals";//获取矿坑数据
	public const string BALANCE_MINERAL ="/yxzh/mineral_port/balance_mineral";//开始结算
	public const string CLEARDATA = "/yxzh/mineral_port/test_clear_data";//清除数据
	public const string MINERAL_SEARCH_ENEMY = "/yxzh/mineral_port/search_enemy";//搜索敌人矿坑
	public const string MINERAL_FIGHT = "/yxzh/mineral_port/fight";//开始抢劫
	public const string MINERAL_GET_ENEMIES = "/yxzh/mineral_port/get_enemy";//获取敌人列表
	public const string MINERAL_SHOW_ENEMY_MINERAL="/yxzh/mineral_port/show_enemy_mineral_info";//获取敌人矿坑数据
	public const string MINERAL_GET_REPORT="/yxzh/mineral_port/get_report";//获取战斗回放
	//=========================mining over==============================
	
	//======================use prop===========================
	public const string USE_PROP = "/yxzh/storage/use";//使用道具
	public const string GET_PROP_USE_INFO = "/yxzh/prop/prop_state";//获取某个道具当日使用次数
	//======================use prop over===========================
	  
	//======================develop==============================
	public const string CARD_SKILL_ADVANCE = "/yxzh/card_update_ability";//技能升级
	public const string CARD_SKILL_STUDE = "/yxzh/learn_ability";//技能学习
	public const string CARD_EVOLVE = "/yxzh/evolution_card";//进化
	public const string EQUIP_INTENSIFY = "/yxzh/equipment/incr_equip";//装备强化
	public const string EQUIP_STAR = "/yxzh/equipment/equip_star";//装备升星
    public const string UPQUALITY = "/yxzh/equipment_port/equip_up_red";//装备升阶
	public const string BEAST_SKILL_UPDATE = "/yxzh/beast_update_ability";//召唤兽技能升级 
	//======================develop over==========================
	//======================shop =================================
	public const string SHOP_BUY = "/yxzh/shop/buy";//购买商品
	public const string SHOP_INIT = "/yxzh/shop/get_info";//初始化商品限制信息
	public const string MYSTICAL_SHOP_INIT = "/yxzh/mystic_shop/shop_list";//初始化神商店信息
	public const string MYSTICAL_BUY = "/yxzh/mystic_shop/buy";//神秘商店买东西
	public const string MYSTICAL_RUCH = "/yxzh/mystic_shop/draw";//刷新神秘商店
    public const string ACTIVE_BUY_LW = "/yxzh/fb/buy_active_times";//买限时活动的次数
	//======================shop over==============================
	//======================storage==================================
	public const string STORAGE_SERIALIZE = "/yxzh/storage/get_storage";//序列化指定仓库
	//=========================equip==================================
	public const string EQUIP_ONEKEY = "/yxzh/equipment/auto_equip";//装备一键换装
	public const string EQUIP_OPERATE = "/yxzh/equipment/equip";//装备操作（手动穿、脱、换）
	public const string EQUIP_SWAP = "/yxzh/card_port/exchange_prop";//装备操作（交换两张卡牌的装备）
    public const string EQUIP_REFINE = "/yxzh/equipment_port/refine";//装备精炼
	//=========================equip over==================================
	
	//======================luckyDraw==============================
	public const string LUCKY_GET_INFO = "/yxzh/ld/get_info";
	public const string LUCKY_DRAW = "/yxzh/ld/execute";
	//======================luckyDraw over==========================
	
	//======================sell====================================
	public const string SELL_GOODS = "/yxzh/prop_handle/sell";
	//======================sell over===============================
	
	//======================resolve=================================
	public const string RESOLVE_GOODS = "/yxzh/prop_handle/destroy";
	//======================resolve over=============================
	//======================beast==================================
	public const string BEAST_SUMMON = "/yxzh/summon_beast";//召唤兽召唤
	public const string BEAST_EVOLUTION = "/yxzh/evolution_beast";//召唤兽进化
	public const string BEAST_ADD = "/yxzh/get_beast_add";//获得召唤兽加成
	//======================beast over==============================
	//======================totallogin==============================
	public const string TOTAL_LOGIN_GET = "/yxzh/la/get_award";//领取累积登陆奖励
	public const string TOTAL_LOGIN_INFO = "/yxzh/la/get_info";//获得累积登陆奖励信息
	public const string HOLIDAY_AWARD_LIST = "/yxzh/festival_award/init";//获取节日送奖励信息
	public const string HOLIDAY_AWARD_BUTTON = "/yxzh/festival_award/get_award";//领取节日 送奖励
	public const string WEEKLY_AWARD_LIST = "/yxzh/week_award/init";//获取周末送奖励信息
	public const string WEEKLY_AWARD_BUTTON = "/yxzh/week_award/get_award";//领取周末送奖励信息
	//======================totallogin over==============================
	//======================mail===================================
	public const string MAIL_GET = "/yxzh/mail/get_mails";//获取所有邮件
	//=======================team===================================
	public const string TEAM_GET = "/yxzh/array/get_alternate_info";//获取替补开放席位
	public const string TEAM_OPEN = "/yxzh/array/relieve_alternate";//替补开放
	public const string TEAM_GUILDFIGHT = "/yxzh/array/init_guild_war_array";//获取公会战队伍信息
	//========================team over====================================
	//========================exchange==================================== 
	public const string EXCHANGE_GET = "/yxzh/exchange/execute_exchange";//获取所有邮件
	public const string EXCHANGE_INFO = "/yxzh/exchange/exchange_info";//获取所有邮件
	//========================exchange over====================================
	//==========================tempStorage==============================
	public const string TEMP_STORAGE_DELETE = "/yxzh/storage/delete";//删除
	public const string TEMP_STORAGE_ONEKEYDELETE = "/yxzh/storage/auto_delete";//一键删除
	public const string TEMP_STORAGE_EXTRACT = "/yxzh/storage/take";//提取
	public const string TEMP_STORAGE_ONEKEYEXTRACT = "/yxzh/storage/auto_take";//一键提取
	//=========================tempStorage over================================
	
	//========================mail============================================
	public const string MAIL_GET_ALL = "/yxzh/mail/get_mails";//获得所有邮件
	public const string MAIL_READ = "/yxzh/mail/read";//读取邮件
	public const string MAIL_EXTRACT = "/yxzh/mail/take";//提取邮件
	public const string MAIL_DELETE = "/yxzh/mail/delete";//删除邮件
	public const string MAIL_ONEKEYEXTRACT = "/yxzh/mail/take_all";//一键提取邮件
	public const string MAIL_ONEKEYDELETE = "/yxzh/mail/clear";//一键删除邮件
	//========================mail over=======================================
	
	//========================attribute======================================
	public const string ATTRIBUTE = "/yxzh/card/incr_strength";//附加属性
    public const string MAGIC_WEAPON_STRENG = "/yxzh/artifact/strengthen";//秘宝强化端口
    public const string MAGIC_WEAPON_PHASE = "/yxzh/artifact/forging";//秘宝锻造端口
    public const string MAGIC_WEAPON_PUTON = "/yxzh/artifact/wear";//秘宝穿戴端口
	//========================attribute over=================================
	
	//==========================task=========================================
	public const string TASK_GET = "/yxzh/achieve/get_achieves";//得到任务
	public const string TASK_COMPLETE = "/yxzh/achieve/complete_achieve";//完成任务
	//==========================task over====================================
	
	//==========================notice=======================================
	public const string NOTICE_GET = "/yxzh/affiche/get_affiche";//获得公告
	public const string NOTICE_READ = "/yxzh/affiche/read_affiche";//读取公告
	public const string NOTICE_TIME = "/yxzh/affiche/get_active_time";//获得公告活动条目时间
	public const string NOTICE_ALCHEMY_INFO = "/yxzh/gold/get_gold_info";//获得炼金信息
	public const string NOTICE_ALCHEMY_BUY = "/yxzh/gold/execute_gold";//执行炼金
	public const string NOTICE_HERO_EAT = "/yxzh/affiche/banquet";//吃
	public const string NOTICE_HERO_EAT_INFO = "/yxzh/affiche/get_banquet_info";//女神宴信息
	public const string STAR_MULTIPLE = "/yxzh/active_port/get_star_active_info";//多倍幸运星信息
	public const string NOTICE_LUCKY_DRAW = "/yxzh/active_port/lucky_notice_card";//限时抽卡,抽装备活动
	public const string NOTICE_LUCKY_XIANSHI_DRAW = "/yxzh/active_limit_port/get_info";//限时活动
	public const string NOTICE_LUCKY_XIANSHI_AWARD_SHOW = "/yxzh/active_limit_port/get_award_info";//查看限时活动奖励
	public const string NOTICE_SHOW_LUCKY_CARD_AWARD = "/yxzh/active_port/lucky_notice_card_awardedids";//查看限时抽卡奖励
	public const string NOTICE_GET_TURN_SPRITE = "/yxzh/star/get_turn_sprite";//精灵翻翻乐
	public const string NOTICE_PULL_SPRITE = "/yxzh/star/select_sprite_pool";//精灵翻翻乐洗牌接口
	public const string NOTICE_TURN_SPRITE = "/yxzh/star/turn_sprite";//精灵翻翻乐翻牌接口


	public const string HAPPYSUNDAY_RECEIVE = "/yxzh/weekend_active/get_award"; //欢畅周末活动,领取奖励
	public const string HAPPYSUNDAY_INIT = "/yxzh/weekend_active/init"; //欢畅周末活动初始化
	public const string NOTICE_ACTIVE_INFO = "/yxzh/active_port/get_active_info";//活动信息
	public const string NOTICE_GM_ACTIVE_INFO = "/yxzh/active_port/get_gm_active_all";//获得所有GM修改活动数据
	public const string NOTICE_GET_ACTIVE_AWARD = "/yxzh/active_port/get_active_award";//领取活动奖励

	public const string NOTICE_EQUIP_REMAKE_GETBACK = "/yxzh/active_port/promote_get_back";//装备找回
	public const string NOTICE_EQUIP_REMAKE_GETAWARD = "/yxzh/active_port/promote_get_award";//装备提升
	//===========================notice over=================================
	
	//============================recharge===================================
	public const string RECHARGE_INFO = "/yxzh/affiche/get_affiche_info";//获得充值
	public const string RECHARGE_ONERMB = "/yxzh/affiche/get_one_affiche";//获得首冲
	public const string RECHARGE_AWARD = "/yxzh/affiche/get_affiche_award";//设置充值
	public const string ACTIVE_LIST = "/yxzh/active_port/front_active_list";// 获取活动配置信息

	//============================recharge over==============================
	
	//============================pvp===================================
	public const string PVPGET_INFO = "/yxzh/fb/pvp_info";//获得PVP信息
	public const string PVP_FIGHT = "/yxzh/fb/pvp_pk";//PVP战斗
	public const string PVP_RANKINFO = "/yxzh/fb/pvp_basic_info";//PVP信息排名
	//============================pvp over==============================
	//============================guide=====================================
	public const string GUIDE_GET = "/yxzh/guide/get";//获得新手引导信息
	public const string GUIDE_SET = "/yxzh/guide/set";//设置新手引导信息
	//============================guide over=================================
	
	//============================rank====================================
	public const string RANK_PVP = "/yxzh/fb/pvp_rank";//获得PVP排名信息
	public const string RANK_GET = "/yxzh/rank/get_rank";//获得排名信息
	public const string RANK_RANK = "/yxzh/ladder/get_ladder_rank";//天梯排行榜

	//============================rank over================================
	
	//==========================chat=========================================
	public const string CHAT_SEND = "/yxzh/chat";//发送消息
	public const string CHAT_PLAYERINFO = "/yxzh/role/get_role_info";//获取玩家数据
	//==========================chat over====================================
    //===========================blood line===================================
    public const string BLOOD_LINE = "/yxzh/card/open_bloodline";//卡片血脉端口
    //===========================blood line over===============================
	
	//======================vip===========================
	public const string VIP_GET_GIFT = "/yxzh/vip/get_gift";//领取特权礼包
    public const string RMB_FIRST = "/yxzh/cash/get_cash_list";//首冲过得条目
	public const string VIP_GET_INFO = "/yxzh/vip/gift_info";//已领取特权礼包信息
	//======================vip over===========================
	
	//======================cash===========================
	public const string CASH = "/yxzh/cash/order";//充值
	//======================cash over===========================

	//======================bulletin===========================
	public const string BULLETIN = "/yxzh/affiche/get_active_notice";//宣传栏
	//======================bulletin over===========================
	
	//======================InviteCode===========================
	public const string INVITECODE_GET_INVITEINFO = "/yxzh/invite/invite_info";//获取后台信息
	public const string INVITECODE_INVITE = "/yxzh/invite/invite";//激活邀请码
	public const string INVITECODE_GET_INVITEAWARD = "/yxzh/invite/invite_award";//领取奖励
	public const string INVITECODE_GET_GIFT = "/yxzh/gift_code/award_gift_code";//领取奖励
	//======================InviteCode over===========================
	
	//======================pyx===========================
	public const string INTENSIFY_PYX = "/yxzh/intensify_pyx";//强化圣器
	public const string PYX_INFO = "/yxzh/pyx_info";//获得强化圣器信息
	//======================pyx over===========================

	//======================fb_star===========================
	public const string GET_FB_STAR = "/yxzh/fb/get_fb_star";//获得副本星星
	//======================fb_star over===========================
	//======================Share===========================
	public const string GET_SHARE = "/yxzh/share/get_share";//获取我的分享
	public const string GET_ONEKEY_SHARE = "/yxzh/share/one_key_share";//一键分享
	public const string GET_ONEKEY_PRAISE = "/yxzh/share/one_key_praise";//一键点赞
	//======================Share over===========================
	//======================friends===========================
	public const string GET_FRIENDS_INFO = "/yxzh/relations/get_relations";//获取好友信息
	public const string DELETE_FRIEND = "/yxzh/relations/delete";//删除好友
	public const string APPLY_FRIEND = "/yxzh/relations/apply";//申请为好友
	public const string HANDLE_APPLY_FRIEND = "/yxzh/relations/apply_handle";//拒绝申请
	public const string RECOMMEND_FRIEND = "/yxzh/relations/recommend";//推荐好友
	public const string FIND_FRIEND = "/yxzh/relations/search";//查找玩家
    public const string FRIENDS_GIFT_SEND = "/yxzh/relations/give_pve";//赠送好友行动力
    public const string FRIENDS_GIFT_RECEIVE = "/yxzh/relations/get_pve";//领取好友行动力
	//======================friends over===========================
	//=========================GetPlayerInfo(Card or Equip)==============================
	public const string GET_PLAYERINFO_CARD = "/yxzh/card/card_info";//获得玩家信息（卡片）
	//=========================GetPlayerInfo over=======================================
	
	//=========================Hero Road==============================
	public const string HERO_ROAD_INTO_HR = "/yxzh/hr/into_hr";//进入英雄之章
	public const string HERO_ROAD_GET_HR_ACTIVATION = "/yxzh/hr/get_hr_activation";//获得英雄之章激活信息
	public const string HERO_ROAD_GET_HR_AWAKEN = "/yxzh/hr/get_hr_awaken";//获得英雄之章觉醒信息
	public const string HONOR_UP = "/yxzh/role/upgrade_honor";//爵位提升
    public const string ACTIVE_FUBEN_INTO = "/yxzh/fb/active_fb";//活动副本进入接口
	//=========================Hero Road over==============================

	//=========================Star Pray==============================
	public const string STAR_GET = "/yxzh/star/get_star_info";//得到星座信息
	public const string STAR_PRAY = "/yxzh/star/pray";//星座祈祷
	public const string STAR_GET_PRAY2 = "/yxzh/star/get_pray2";//获得幸运星座
	public const string STAR_PRAY2 = "/yxzh/star/pray2";//钻石摇一摇

	//=========================Star Over==============================
	//======================Evolution===========================
	public const string EVOLUTION_MAIN = "/yxzh/evolution1";//主卡修身
	public const string EVOLUTION_MAIN_TP = "/yxzh/evolution2";//主卡突破
	public const string EVOLUTION_CARD = "/yxzh/evolution3";//普通卡进阶
	//======================Evolution over===========================
	//==========================Guild================================================
	public const string GET_GUILDLIST = "/yxzh/guild/guild_list";//获得公会列表
	public const string CREATE_GUILD = "/yxzh/guild/create_guild";//创建公会
	public const string GET_GUILDINFO = "/yxzh/guild/get_guild";//获得自己的公会信息
	public const string GET_GUILDSAMPLEINFO = "/yxzh/guild/get_simple_guild";//获得某公会的成员信息
	public const string GET_APPLYLIST = "/yxzh/guild/apply_list";//获得申请列表
	public const string JOIN_GUILD = "/yxzh/guild/apply_join";//申请加入公会
	public const string CANCEL_JOIN_GUILD = "/yxzh/guild/cancel_apply";//取消加入公会
	public const string APPROVE_JOIN_GUILD = "/yxzh/guild/ratify_apply";//批准加入公会
	public const string REJECT_JOIN_GUILD = "/yxzh/guild/refused_apply";//拒绝加入公会
	public const string GET_INVITEGUILD = "/yxzh/guild/invite_list";//获得公会邀请列表
	public const string GUILD_INVITE = "/yxzh/guild/invite_join";//邀请加入公会
	public const string GUILD_INVITE_ACCEPT = "/yxzh/guild/accept_invite";//接受邀请加入公会
	public const string GUILD_INVITE_REJECT = "/yxzh/guild/refused_invite";//拒绝邀请加入公会
	public const string GUILD_AMEND_DECLARATION = "/yxzh/guild/update_declaration";//修改公会宣言
	public const string GUILD_AMEND_NOTICE = "/yxzh/guild/update_notice";//修改公会公告
	public const string GUILD_GET_MEMBERS = "/yxzh/guild/get_self_mems";//获得公会成员列表
	public const string GUILD_EXIT = "/yxzh/guild/exit_guild";//主动退出公会
	public const string GUILD_CLEANMEM = "/yxzh/guild/clean_mem";//会长踢人出公会
	public const string APPOINT_VICE = "/yxzh/guild/appoint_vice";//任命副会长
	public const string RESIGN_VICE = "/yxzh/guild/resign_vice";//卸任副会长
	public const string GUILD_DONATION = "/yxzh/guild/donation";//捐献
	public const string APPOINT_PRESIDENT = "/yxzh/guild/appoint_president";//转交会长
	public const string GUILD_GET_APPROVALLIST = "/yxzh/guild/get_guild_apply_list";//获得公会审批列表
	public const string GUILD_IMPEACH = "/yxzh/guild/impeach";//弹劾会长
	public const string GUILD_BUILD_LEVEL = "/yxzh/guild/builds_level";//获得公会建筑等级
	public const string GUILD_CREATE_BUILD = "/yxzh/guild/create_build";//创建公会建筑
	public const string GUILD_UPGRADE_BUILD = "/yxzh/guild/upgrade_build";//升级公会建筑
	public const string GUILD_LEARNSKILL = "/yxzh/guild/learn_skill";//学习公会技能
	public const string GUILD_GETSKILL = "/yxzh/guild/get_guild_skill";//获得公会技能信息
	public const string GUILD_GETGOODS = "/yxzh/guild/get_guild_store";//获得公会商店自己物品信息
	public const string GUILD_SHOPBUY = "/yxzh/guild/store_buy";//公会商店购买
	public const string GUILD_ALTAR = "/yxzh/guild/get_guild_altar";//获得祭坛信息
	public const string GUILD_CHALLENGE = "/yxzh/guild/challenge";//挑战BOSS
	public const string GUILD_CHALLENGE_FORMATION = "/yxzh/guild/get_challenge_array";//获得上次挑战阵型
	public const string GUILD_GETRASTRANK = "/yxzh/guild/get_gb_rast_rank";//获得昨日伤害排名
	public const string GUILD_GETLUCKYNVSHEN = "/yxzh/guild/get_gls_info";//获得幸运女神信息
	public const string GUILD_SHAKEEBLOWS = "/yxzh/guild/do_cast";//投掷骰子
	public const string GUILD_GETREWARDS = "/yxzh/guild/get_cast_award";//获取奖励
	public const string GUILD_RENAME = "/yxzh/guild/update_guild_name";//公会改名
	public const string GUILD_SEARCH = "/yxzh/guild/search_guild";//搜索公会
	public const string GUILD_GET_GUILDAREA = "/yxzh/guild_war/get_enemy_info";//获取领地信息(自身,他人)
	public const string GUILD_ACTIVE_AREABUFF = "/yxzh/guild_war/active_buffer";//激活公会领地buff
	public const string GUILD_AREA_CHALLENGE = "/yxzh/guild_war/fight";//领地挑战
	public const string GUILD_AREA_HURT_RANK = "/yxzh/guild_war/get_member_rank";//获取公会战贡献排行榜
	public const string GUILD_GET_FIGHTINFO = "/yxzh/guild_war/get_info";//获取公会战信息
	public const string GUILD_AREA_GUILD_RANK = "/yxzh/guild/area_hurt_rank";//获取公会战排行榜
	public const string GUILDWAR_GET_GARRISON_INFO = "/yxzh/guild_war/get_garrison_info";//获取驻守者信息
	public const string GUILD_AUTO_JOIN="/yxzh/guild/update_auto_join";//自动批准接口
	public const string GUILDWAR_GET_WARPOWER = "/yxzh/guild_war/get_power_award";//领取行动值
    public const string GUILDWAR_REVIVE = "/yxzh/guild_war/revive";//公会战玩家复活
	public const string GUILDWAR_GET_HP = "/yxzh/array_port/get_guild_hp";//获取公会战队伍血量信息

	//==========================Arena================================================
	public const string ARENA_GET_STATE = "/yxzh/arena/get_arena_step";//获取当前竞技场状态
	public const string ARENA_MASS = "/yxzh/arena/arena_mass";//获取海选信息
	public const string ARENA_APPLY = "/yxzh/arena/arena_apply";//获取海选信息
	public const string ARENA_REFRESH_ENEMY = "/yxzh/arena/refresh_enemy";//免费刷新对手
	public const string ARENA_RMB_REFRESH_ENEMY = "/yxzh/arena/refresh_enemy2";//rmb刷新对手
	public const string ARENA_CHALLENGE = "/yxzh/arena/arena_challenge";//海选挑战
	public const string ARENA_BUY_CHALLENGE_COUNT = "/yxzh/arena/buy_c_count";//购买挑战次数
	public const string ARENA_RESET_CD = "/yxzh/arena/reset_cd";//秒cd
	public const string ARENA_FINAL_AWARD = "/yxzh/arena/get_final_award";//领决赛奖
	public const string ARENA_GUESS_AWARD = "/yxzh/arena/get_guess_award";//领决竞猜奖
	public const string ARENA_GET_REPLAY = "/yxzh/arena/get_zhanbao";//获得战报
	public const string ARENA_GET_REPLAY_INFO = "/yxzh/arena/get_zhanbao2";//获取回看信息
	public const string ARENA_GET_FINAL = "/yxzh/arena/get_final";//获得决赛数据
	public const string ARENA_GET_MASS_PLAYER_INFO = "/yxzh/arena/get_enemy";//获得海选对手信息
	public const string ARENA_GET_GUESS = "/yxzh/arena/get_guess_info";//获得竞猜信息
	public const string ARENA_SET_GUESS = "/yxzh/arena/arena_guess";//设置竞猜信息
	public const string ARENA_GET_AWARD_INFO_INTEGRAL = "/yxzh/arena/get_score_award_info";//获得积分奖励信息
	public const string ARENA_GET_AWARD_INFO_GUESS = "/yxzh/arena/get_guess_award_info";//获得竞猜奖励信息
	public const string ARENA_GET_AWARD_INFO_FINAL = "/yxzh/arena/get_final_award_info";//获得决赛奖励信息
	public const string ARENA_RECEIVE_AWARD_INTEGRAL = "/yxzh/arena/get_score_award";//领取积分奖励信息
	public const string ARENA_RECEIVE_AWARD_GUESS = "/yxzh/arena/get_guess_award";//领取竞猜奖励信息
	public const string ARENA_RECEIVE_AWARD_FINAL = "/yxzh/arena/get_final_award";//领取决赛奖励信息
	public const string ARENA_GET_RANK = "/yxzh/arena/get_mass_rank";//竞技榜
	//=========================Arena over==============================

	//======================goddessAstrolabe===========================
	public const string GODDESSASTROLABE_GET_INFO = "/yxzh/star/get_star_point";//获取星盘信息
	public const string GODDESSASTROLABE_OPEN_STAR = "/yxzh/star/goto_star";//激活星星
	//======================goddessAstrolabe over===========================
    
	//======================divine===========================
	public const string DIVINE_SEND = "/yxzh/star/divination";//占卜
	public const string DIVINE_INFO = "/yxzh/star/divination_info";//占卜信息
	//======================divine over===========================

	//=====================monthCard=========================
	public const string MONTHCARD_GET = "/yxzh/mmcard/get_mmcard_info";
	public const string MONTHCARD_BUY = "/yxzh/mmcard/buy_mmcard";
	public const string MONTHCARD_RECEIVE = "/yxzh/mmcard/mmcard_day_award";
	public const string MONTHCARD_TIMEINFO = "/yxzh/mmcard/get_buy_info";  //获取月卡购买时间信息
	//====================monthCard over=====================

	//====================levelupreward======================
	public const string LEVELUP_REWARD_GET = "/yxzh/role/get_uplevel_award_info";
	public const string LEVELUP_REWARD_RECEIVE = "/yxzh/role/get_uplevel_award";
	//========================================================

	//====================missionAward======================
	public const string MISSIONAWARD_INFO = "/yxzh/fb/get_fb_end_star";
	public const string MISSIONAWARD_GET = "/yxzh/fb/award_fb_end_star";
	//========================================================

	//====================Sweep======================
	public const string SWEEP_AWARD = "/yxzh/fb/sweep_settlement";
	public const string SWEEP_BEGIN = "/yxzh/fb/sweep";
	public const string SWEEP_GET = "/yxzh/fb/get_sweep";
	public const string SWEEP_FINISH = "/yxzh/fb/sweep_finish";
	public const string SWEEP_PK = "/yxzh/fb/sweep_pk";

	//====================Ladders===================
    public const string LADDERS_STATE = "/yxzh/ladder/get_ladder_info";
	public const string LADDERS_APPLY = "/yxzh/ladder/ladder_apply";
	public const string LADDERS_FRIENDSHELP = "/yxzh/ladder/get_player_info";
	public const string LADDERS_GETPLAYERS = "/yxzh/ladder/get_player_info";
	public const string LADDERS_FIGHT = "/yxzh/ladder/fight";
	public const string LADDERS_RECORD = "/yxzh/ladder/get_ladder_fight";
	public const string LADDERS_CHEST_RECEIVE = "/yxzh/ladder/open_box";
	public const string LADDERS_BUY_COUNT = "/yxzh/ladder/buy_fight_num";
	public const string LADDERS_AWARD_RECEIVE = "/yxzh/ladder/get_rank_award";
	public const string LADDERS_CHEST_REFRESH = "/yxzh/ladder/get_super_box";
	public const string LADDERS_Refresh_Free = "/yxzh/ladder/refresh_enemy1";
	public const string LADDERS_Refresh_Money = "/yxzh/ladder/refresh_enemy2";
	public const string LADDERS_ATTR_EFFECT = "/yxzh/title/get_title";
	public const string LADDERS_BUY_INVITE="/yxzh/ladder/buy_invite";
	//====================StarSoul======================
	public const string STARSOUL_HUNT = "/yxzh/star_soul/draw_star_soul"; //普通猎魂
	public const string STARSOUL_AUTO_HUNT = "/yxzh/star_soul/auto_draw_star_soul"; //一键猎魂
	public const string STARSOUL_GET = "/yxzh/star_soul/get_star_soul"; // 单个拾取
	public const string STARSOUL_AUTO_GET = "/yxzh/star_soul/auto_get_star_soul"; //一键拾取
	public const string STARSOUL_CONVERT_EXP = "/yxzh/star_soul/convert_exps"; // 筛选转化经验
	public const string STARSOUL_AUTO_CONVERT_EXP = "/yxzh/star_soul/auto_convert_exp"; // 一键转化
	public const string STARSOUL_INFO = "/yxzh/star_soul/star_soul_info"; // 获取星魂信息
	public const string STARSOUL_LOCK = "/yxzh/star_soul/update_locks"; // 更新星魂锁信息
	public const string STARSOUL_FLOOD_EXP = "/yxzh/star_soul/flood_exp"; // 星魂强化
	public const string STARSOUL_USE = "/yxzh/star_soul/use_star_soul"; // 星魂穿戴
	public const string STARSOUL_NONUSE = "/yxzh/star_soul/nonuse_star_soul"; // 星魂卸下
	//=====================Mounts===================================
	public const string MOUNTS_PUTON = "/yxzh/mounts/mounts_puton"; // 坐骑坐乘
	public const string MOUNTS_ACTIVE = "/yxzh/mounts/auto_mounts_active"; // 坐骑激活
	public const string MOUNTS_POWER = "/yxzh/mounts/mounts_power"; // 坐骑普通修炼
	public const string MOUNTS_TIME_INFO="/yxzh/guild_war/get_champion_info";//坐骑其他信息
	//=====================GrowupAward===================================
	public const string GrowupAward_GETAWARD="/yxzh/grow_plan/get_award";
	public const string GrowupAward_INVEST="/yxzh/grow_plan/invest";
	public const string GrowupAward_GETINFO="/yxzh/grow_plan/get_info";
	//======================inherit===================================
	public const string INHERIT = "/yxzh/inherit";//传承
	//======================inherit over===================================
	//==============================tower================================
    public const string TOWER_AWARD_INFO = "/yxzh/tower/open_box";//爬塔宝箱信息端口
    public const string TOWER_AWARD_GET = "/yxzh/tower/lottery_award";//爬塔宝箱翻牌
    public const string TOWER_AWARD_BEGIN = "/yxzh/tower/box_info";//拿信息接口（只拿信息）
    public const string TOWER_RESET = "/yxzh/tower/reset_tower_fb";//重置副本端口
    public const string TOWER_LO_BUY = "/yxzh/tower/buy_open_box_num";//开宝箱次数购买
    public const string TOWER_CLEAR = "/yxzh/tower/clear";//清理宝箱信息接口
	//======================quiz===================================
	public const string QUIZ_GETQUESTIONS = "/yxzh/question/get_questions";
	public const string QUIZ_ANSWER = "/yxzh/question/answer";
	public const string QUIZ_GETAWARD = "/yxzh/question/get_award";
	//======================quiz over===================================

	//======================card training===================================
	public const string CARDTRAINING_INIT = "/yxzh/traning/init"; //卡牌训练初始化
	public const string CARDTRAINING_SUBMIT = "/yxzh/traning/traning_card"; //提交卡牌训练
	public const string BEAST_TRANING="yxzh/traning/beast_traning";//女神训练接口
	public const string BEAST_INIT="yxzh/traning/beast_init";//女神训练初始化接口
	//======================card training over===================================

	//======================SDK===================================
	public const string SDK_UPDATE = "/yxzh_momo_sdk/sdk/update"; //上传用户sdk内的个人信息
	public const string SDK_GET_ALL_INFO = "/yxzh_momo_sdk/sdk/get_sdk"; //获取指定uid的所有sdk信息
	public const string SDK_GETINFO_BY_TYPE = "/yxzh_momo_sdk/sdk/get_sdk_by_type"; //获取指定uid的特定的sdk信息(小)
	//======================SDK over===================================

    //======================double rmb ===================================
    public const string DOUBLE_RMB_INFO = "/yxzh/double_cash/get_info";
    //======================double rmb over===================================

	//======================system settings ===================================
	public const string SYSTEMSETTINGS_GETINFO = "/yxzh/system_set_port/get_info";
	public const string SYSTEMSETTINGS_SUBMIT = "/yxzh/system_set_port/set_info";
	//======================system settings over===================================

	//======================SDKFirends===================================
	public const string SDK_FRIEND_UPDATE = "/yxzh_momo_sdk/sdk/friendsinfo"; //上传用户sdk内的个人信息
	public const string SDK_FRIEND_GET_INT = "/yxzh/momo_relations/get_relations_list";//获取服务器sdk好友信息
	public const string SDK_FRIEND_SEND = "/yxzh/momo_relations/give_pve"; //赠送指定uid的sdk消息
	public const string SDK_FRIEND_GET = "/yxzh/momo_relations/get_pve"; //获取指定uid的sdk消息
	public const string SDK_FRIEND_INVITE = "/yxzh/momo_relations/invite";//获取发送邀请后的消息
	public const string SDK_FRIEND_MONEYPRIZE = "/yxzh/momo_relations/get_mmi_cash_info";//获取奖励面板消息
	public const string SDK_FRIEND_LEVELPRIZE = "/yxzh/momo_relations/get_mmi_lv_info";//获取奖励面板消息
	public const string SDK_FRIEND_PRIZE_BACK = "/yxzh/momo_relations/get_mmi_cash_award";//发送返利领取消息
	public const string SDK_FRIEND_PRIZE_LEVEL = "/yxzh/momo_relations/get_mmi_lv_award";//发送等级领取消息
	public const string SDK_ADD_SDK_FRIEND = "/yxzh/role/get_pid";//发送等级领取消息
	//======================SDK over===================================

	//======================WorldBoss===================================
	public const string WorldBoss_GETINFO = "/yxzh/worldboss/get_info";
	public const string WorldBoss_ATTACK = "/yxzh/worldboss/attack";
	public const string WorldBoss_RESCD = "/yxzh/worldboss/resCD";
	//======================WorldBoss over===================================

    //======================SdkOneKeyShare===================================
    public const string SDK_SHARE_SEND_SHARE_SUCCESS = "/yxzh/share/platform_share";//发送分享成功的消息 
    public const string SDK_SHARE_GET_SHARE_COUNT = "/yxzh/share/get_platform_share";//获取分享次数的消息
    //======================SdkOneKeyShare over===================================

	//=====================LadderHeMoney===================================
	public const string LADDERHEGEMONEY_GET_INT = "/yxzh/ladder/get_ladder_score_and_rank"; //获取初始天梯争霸初始化信息
	public const string LADDERHEGEMONEY_GET_GOODS_BUY = "/yxzh/shop/buy";// 购买商品
	
	
	//======================LadderHeMoney over===================================

	//======================Active VSN===================================
	public const string ACTIVE_VSN = "/yxzh/server/active_vsn";//配置版本号
	//======================Active VSN over===================================
    //======================PVE ===================================
	public const string PVE_BUY = "/yxzh/role/buy_pve_energy";
    //======================PVE over===================================

	//======================Festival Wish ===================================
	public const string GET_FESTIVALWISH_INFO = "/yxzh/team_purchasing_port/get_team_purchasing";//获取节日许愿的信息
	public const string DO_FESTIVALWISH_INFO = "/yxzh/team_purchasing_port/join_team_purchasing";//许愿时间通信
	//======================Festival Wish over===================================

	//======================superDraw===================================
	public const string GET_SUPERDRAW_INFO = "/yxzh/super_pool/get_info";//获取超级奖池信息
	public const string DRAW_SUPERDRAW = "/yxzh/super_pool/lottery";//超级奖池抽奖
	//======================superDraw over===================================

	//======================GodsWar ===================================
	public const string GET_GODSWARGROUPSTAGE_INFO = "/yxzh/god_war_port/get_opponent";//诸神战获取信息
	public const string GET_GODSWARREFRESHENEMY = "/yxzh/god_war_port/refresh";//诸神战刷新对手信息
	public const string GODSWAR_CHALLENGE = "/yxzh/god_war_port/fight";//小组赛挑战
	public const string GODSWAR_CHALLENGE_FINAL = "/yxzh/god_war_port/get_report";//淘汰赛挑战
	public const string GODSWAR_GETINTERAL_AWARD = "/yxzh/god_war_port/get_score_award";//每日积分领取
	public const string GODSWAR_GETIFINALBASE_INFO = "/yxzh/god_war_port/get_fight_base";//获取淘汰赛基础信息
	public const string GODSWAR_GETIFINALPOINT_INFO = "/yxzh/god_war_port/get_fight_table";//获取淘汰赛对战点位信息
	public const string GODSWAR_GETFINALINFO = "/yxzh/god_war_port/get_finals_table";//神魔大战
	public const string GODSWAR_GETSUPORT_INFO = "/yxzh/god_war_port/get_guess_show";//获取对战玩家信息
	public const string GODSWAR_SENDSUPORT = "/yxzh/god_war_port/guess";//竞猜接口
	public const string GODSWAR_GETRANK = "/yxzh/god_war_port/get_rank";//获得排行榜
	public const string GODSWAR_GETMYSUPORTINFO = "/yxzh/god_war_port/get_total_guess";//获得我的支持
	public const string GODSWAR_GETSTATEINFO = "/yxzh/god_war_port/get_god_state";//获得状态信息
	public const string GODSWAR_GETGROUPRANKINFO = "/yxzh/god_war_port/get_group_rank";//获得小组赛积分排名信息

	//======================GodsWar over ===================================

	//======================Rebate===================================
	public const string GET_REBATE_INFO = "/yxzh/lucky_bag/init";//福袋返利初始化信息
	public const string SEND_REBATE = "/yxzh/lucky_bag/award";//领取福袋发送信息
	//======================Rebate over===================================

    //======================signIn===================================
    public const string SIGN_IN = "/yxzh/sign_in_port/sign_in";//签到通讯
    public const string GET_SIGNIN_INFO = "/yxzh/sign_in_port/get_sign_in";//获取签到状态
    //======================signIn over===================================

    //======================shareDraw===================================
    public const string SHAREDRAW = "/yxzh/share_port/get_share_award";//抽奖通讯
    //======================shareDraw over===================================

	//======================BackPrize===================================
	public const string BACKPRIZE_LOGININFO = "/yxzh/goback/init_login";//获取回归玩家登录信息
	public const string SEND_BACKPRIZE = "/yxzh/goback/login_award";//领回归登录奖励
	public const string BACKPRIZE_RECHARGEINFO = "/yxzh/goback/init_cash";//获取回归玩家累计充值信息
	public const string BACKPRIZE_SENDRECHARGE = "/yxzh/goback/cash_award";//领回归充值奖励
	//======================BackPrize over===================================

	//======================WeekCard===================================
	public const string WEEKCARD_INFO = "/yxzh/mmcard_port/get_week_card_info";//周卡信息
	public const string WEEKCARD_AWARD = "/yxzh/mmcard_port/week_card_day_award";//周卡领奖
	//======================WeekCard over===================================

    //======================boss Attack===================================
    public const string GET_ONEONONE_INFO = "/yxzh/fight_boss/init";//初始化恶魔挑战信息
    public const string ONEONEONE_FIGHT = "/yxzh/fight_boss/fight";//开始挑战
    public const string BOSSATTACKTIME_BUY = "/yxzh/fight_boss/buy_times";//挑战次数购买
    //======================boss Attack over===================================

	//======================sevenDaysHappy===================================
	public const string SEVENDAYSHAPPY_INFO = "/yxzh/task_port/get_task";//七日狂欢信息
	public const string SEVENDAYSHAPPY_REWARD	 = "/yxzh/task_port/task_award";//七日狂欢领奖
	public const string SEVENDAYSHAPPY_TOUCHTASK = "/yxzh/task_port/touch_task";
	//======================sevenDaysHappy===================================

	//======================lastBattle===================================
	public const string LASTBATTLEINIT = "/yxzh/armageddon/init";//末日决战初始化
	public const string LASTBATTLEDONATE = "/yxzh/armageddon/donate";// 末日决战捐献接口//
	public const string LASTBATTLEFIGHT = "/yxzh/armageddon/fight_monster";// 末日决战挑战小怪//
	public const string LASTBATTLEBOSSBATTLE = "/yxzh/armageddon/fight_boss";// 末日决战挑战boss//
	public const string LASTBATTLEPROCESSAWARD = "/yxzh/armageddon/materials_award";// 末日决战领取进度奖励//
	public const string LASTBATTLEKILLLOG = "/yxzh/armageddon/kill_log";// 末日决战击杀boss战报//
	public const string LASTBATTLEUPDATE = "/yxzh/armageddon/get_info";// 末日决战刷新接口//
	//======================lastBattle===================================

    //======================shenge===================================
    public const string GET_SHENGE_INFO = "/yxzh/godhood_port/get_godhood";//获取神格穿戴信息
    public const string SHENGE_GROUP = "/yxzh/godhood_port/godhood_marge";//神格的升级
    public const string SHENGE_PUTON = "/yxzh/godhood_port/godhood_wear";//神格穿戴，替换
    //======================shengeOver===================================

	//======================lottery===================================
	public const string LOTTERY_INFO = "/yxzh/lotto/init";//大乐透初始化信息
	public const string BUY_LOTTERY = "/yxzh/lotto/buy";//购买彩票
	public const string LOTTERY_AWARD = "/yxzh/lotto/get_award";//领取选注奖励
	//======================lottery===================================
} 

