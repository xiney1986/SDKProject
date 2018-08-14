using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 超级奖池实体
/// </summary>
public class SuperDraw {

	/// <summary>
	/// 奖池奖卷数量
	/// </summary>
	public int poolNum = 0;
	/// <summary>
	/// 积分
	/// </summary>
	public int score = 0;
	/// <summary>
	/// 奖池sid
	/// </summary>
	public int poolSid = 0;
	/// <summary>
	/// 可抽奖次数
	/// </summary>
	public int canUseNum = 0;

	public List<SuperDrawAudio> list = new List<SuperDrawAudio>();

	public SuperDraw(){
	}
}
/// <summary>
/// 广播实体
/// </summary>
public class SuperDrawAudio {

	/// <summary>
	/// 服务器名
	/// </summary>
	public string serverName = "";
	/// <summary>
	/// 玩家名
	/// </summary>
	public string playerName = "";
	/// <summary>
	/// 抽中奖卷数量
	/// </summary>
	public int    DrawNum = 0;

	public SuperDrawAudio()
	{

	}
}
