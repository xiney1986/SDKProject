using UnityEngine;
using System.Collections;

/// <summary>
/// 诸神战排名用户信息
/// gc
/// </summary>
public class GodsWarRankUserInfo
{
	#region 公用信息

	public string serverName;
	public string uid;
	public string name;
	/// <summary>
	/// 支持人数(小组赛是为积分)
	/// </summary>
	public int num;

	#endregion
	public GodsWarRankUserInfo(){}

	/// <summary>
	/// 解析元数据
	/// </summary>
	public void bytesRead(ErlArray erl)
	{
		int pos=0;
		serverName = erl.Value[pos++].getValueString();
		uid = erl.Value[pos++].getValueString();
		name = erl.Value[pos++].getValueString();
		num = StringKit.toInt(erl.Value[pos++].getValueString());
	}
}
