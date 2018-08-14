using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 我的支持单条信息
/// </summary>
public class GodsWarMySuportInfo
{
	/// <summary>
	/// 用于确定几强赛
	/// </summary>
	public int localId;
	/// <summary>
	/// 服务器名字
	/// </summary>
	public string serverName;
	/// <summary>
	/// 暂时无用
	/// </summary>
	public string uid;
	/// <summary>
	/// 支持玩家名字
	/// </summary>
	public string name;
	/// <summary>
	/// 是否支持成功
	/// </summary>
	public int isWin;
	/// <summary>
	/// 获得的奖励
	/// </summary>
	public PrizeSample prizes;

    /// <summary>
    /// 免费得支持还是力挺
    /// </summary>
    public int freeState = 0;

	public GodsWarMySuportInfo()
	{

	}
	/// <summary>
	/// 解析元数据
	/// </summary>
	public void bytesRead(ErlArray empArray)
	{
		int pos=0;
		//获取列表中的位置
		localId = StringKit.toInt(empArray.Value[pos++].getValueString());
		serverName = empArray.Value[pos++].getValueString();
		uid = empArray.Value[pos++].getValueString();
		name = empArray.Value[pos++].getValueString();
		isWin = StringKit.toInt(empArray.Value[pos++].getValueString());
		//if(isWin==-1||isWin==0)return;
		ErlArray arr = empArray.Value[pos++] as ErlArray;
		if(arr.Value.Length>0)
		{
			//确定写死(只支持货币)
			ErlArray tmp = arr.Value[0] as ErlArray;
			int pot=0;
			string prop = tmp.Value[pot++].getValueString();

			ErlArray tt = tmp.Value[pot++] as ErlArray;
			int pt=0;
			string goods = tt.Value[pt++].getValueString();
			int sid = StringKit.toInt(tt.Value[pt++].getValueString());
			int num = StringKit.toInt(tt.Value[pt++].getValueString());
			string str = goods+","+sid+","+num; 
			prizes = new PrizeSample(str,',');

		}
	    freeState = StringKit.toInt(empArray.Value[pos++].getValueString());
	}
}
