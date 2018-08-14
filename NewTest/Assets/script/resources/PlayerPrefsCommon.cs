using UnityEngine;
using System.Collections;

/// <summary>
/// Player prefs持久化常量定义
/// </summary>
public class PlayerPrefsComm {

	/** 登陆常量 */
	public static readonly string LOGIN_LATELY_SERVER = "latelyServer", // 保存服务器信息配置文件名
		LOGIN_NAME = "login_name_", // 保存的登陆名
		LOGIN_IP = "login_ip_"; // 保存的登录服务器ip

	/** 副本常量 */
	public static readonly string MISSION_NEW = "new_mission"; // 副本

	/** 战斗常量 */
	public static readonly string BATTLE_PLAY_VELOCITY = "battlePlayVelocity"; // 战斗加速

	/** 星魂常量 */
	public static readonly string STARSOUL_HUNT_TAP = "StarSoulHuntTap", // 猎魂tap下标
		STARSOUL_CHOOSE_QUALITY = "StarSoulChooseQuality"; // 星魂选择拾取的品质

	/// <summary>
	/// 最近好友聊天
	/// </summary>
	public static readonly string CHAT_RECENT_FRIENDS = "ChatRecentFriends";
	/// <summary>
	/// 好友聊天记录
	/// </summary>
	public static readonly string CHAT_MSG_FRIENDS = "ChatMsgFriends";

	/// <summary>
	/// 战斗提示
	/// </summary>
	public static readonly string BATTLE_SMART_TIPS = "BattleSmartTips_";

	/// <summary>
	/// 公会信息栏隐藏常量
	/// </summary>
	public static readonly string GUILD_INFO_HIDE = "GuildInfoHide", //公会信息栏隐藏
						GUILD_FIGHT_INFO_HIDE = "GuildFightInfoHide"; //公会战信息栏隐藏

	/// <summary>
	/// 复制队伍一到队伍三
	/// </summary>
	public static readonly string COPY_ARMY = "_CopyArmy";

	/// <summary>
	/// 友善指引点击次数
	/// </summary>
	public static readonly string FRIEND_GUIDE_TIMES = "_FG";

	///<summary>
	/// vip兑换次数提示
	/// </summary>
	public static readonly string VIP_EXCHANGE_TIP = "vipExchange";

	///<summary>
	/// 守护天使常量
	/// </summary>
	public static readonly string ANGEL_USER_NAME = "angelUserName_";
}
