using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 诸神战淘汰赛玩家信息
/// </summary>
public class GodsWarFinalUserInfo
{
	/// <summary>
	/// 玩家头像id
	/// </summary>
	public int headIcon;
	/// <summary>
	/// 玩家UID
	/// </summary>
	public string uid;
	/// <summary>
	/// 玩家所在服务器
	/// </summary>
	public string serverName;
	/// <summary>
	/// 被支持的数量
	/// </summary>
	public int suportPlayerNum;
	/// <summary>
	/// 上阵卡片sid和品质
	/// </summary>
	public List<string> cardSids;
	/// <summary>
	/// 基础信息列表中的位置(后台数据从1开始)
	/// </summary>
	public int index;
	/// <summary>
	/// 玩家等级
	/// </summary>
	public int level;
	/// <summary>
	/// 玩家名称
	/// </summary>
	public string name;
	/// <summary>
	/// 战斗力
	/// </summary>
	public int power;
	/// <summary>
	/// 所在域名(1:圣域 2：魔域)
	/// </summary>
	public int yu_ming;


	public GodsWarFinalUserInfo()
	{

	}
	/// <summary>
	/// 解析元数据{位置,{{服务器名字,RoleUid},Name,等级}}
	/// </summary>
	/// <param name="empArray">Emp array.</param>
	public void bytesRead(ErlArray empArray)
	{
		int pos=0;
		//获取列表中的位置
		index = StringKit.toInt(empArray.Value[pos++].getValueString());

		ErlArray arr = empArray.Value[pos++] as ErlArray;
		int posss=0;
		//获取服务器名和uid
		ErlArray rry = arr.Value[posss++] as ErlArray;
		int po = 0;
		serverName = rry.Value[po++].getValueString();
		uid = rry.Value[po++].getValueString();
		//获取姓名和等级
		name = arr.Value[posss++].getValueString();
		level =StringKit.toInt(arr.Value[posss++].getValueString());
	}

	/// <summary>
	/// 读取部分数据({severName,uid})
	/// </summary>
	public void bytesReadTwo(ErlArray erlArray)
	{
		int pos =0;
		serverName = erlArray.Value[pos++].getValueString();
		uid = erlArray.Value[pos++].getValueString();
	}
	/// <summary>
	/// 读取部分数据({SN,RU},Style,Name,Power,Lv,Guess)
	/// </summary>
	public void bytesReadThree(ErlArray erArray)
	{
		int pos = 0;
		//{SN,RN}
		ErlArray tmp = erArray.Value[pos++] as ErlArray;
		int pot = 0;
		serverName = tmp.Value[pot++].getValueString();
		uid = tmp.Value[pot++].getValueString();

		//Style,Name,Power,Lv,Guess
		headIcon = StringKit.toInt(erArray.Value[pos++].getValueString());
		name = erArray.Value[pos++].getValueString();
		power = StringKit.toInt(erArray.Value[pos++].getValueString());
		level = StringKit.toInt(erArray.Value[pos++].getValueString());
		suportPlayerNum = StringKit.toInt(erArray.Value[pos++].getValueString());
	}
	/// <summary>
	/// 读取部分数据({竞猜人数,战力,服务器名字,{{SId,2},SId,SId,SId,SId},{SId,SId,SId,SId,SId}})
	/// </summary>
	public void bytesReadFour(ErlArray erArray)
	{
		int pos = 0;
		suportPlayerNum = StringKit.toInt(erArray.Value[pos++].getValueString());
		power = StringKit.toInt(erArray.Value[pos++].getValueString());
		serverName = erArray.Value[pos++].getValueString();

		cardSids = new List<string>();
		ErlArray tmp = erArray.Value[pos++] as ErlArray;
		int pot = 0;
		if(tmp.Value.Length==0)return;
		for (int i = 0; i < tmp.Value.Length; i++)
		{
		    ErlArray sidorp = tmp.Value[i] as ErlArray;
            cardSids.Add(sidorp.Value[0].getValueString() + ":" + sidorp.Value[1].getValueString() + ":" + sidorp.Value[2].getValueString());
		}
		ErlArray amy = erArray.Value[pos++] as ErlArray;
		int pop = 0;
		for (int i = 0; i < amy.Value.Length; i++) {
            ErlArray sidorpp = amy.Value[i] as ErlArray;
            cardSids.Add(sidorpp.Value[0].getValueString() + ":" + sidorpp.Value[1].getValueString() + ":" + sidorpp.Value[2].getValueString());
		}
	}

	/// <summary>
	/// 读取部分数据{SN,RU,Yuming,Style,Name}
	/// </summary>
	public void bytesReadFive(ErlArray empArray)
	{
		int pos=0;
		//获取列表中的位置
		serverName = empArray.Value[pos++].getValueString();
		uid = empArray.Value[pos++].getValueString();
		yu_ming = StringKit.toInt(empArray.Value[pos++].getValueString());
		headIcon = StringKit.toInt(empArray.Value[pos++].getValueString());
		name = empArray.Value[pos++].getValueString();
	}
}
