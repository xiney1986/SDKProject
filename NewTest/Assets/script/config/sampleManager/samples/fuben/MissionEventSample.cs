using System;
using System.Collections.Generic;

/**
 * 副本事件模板
 * @author longlingquan
 * */
public class MissionEventSample:Sample
{
	public MissionEventSample ()
	{
		
	}
	
	public int eventType = 0;//事件类型
	public int cost;//事件花费
	public int costType = 0;//花费类型
	public int other = 0;//宝箱点(宝箱类型id),boss点(boss sid)
	public int battleType = 0;//战斗类型  如果不是战斗点则是0
	public Dictionary<int,Plot> npcPlots = null;//npc上场剧情对白
	public Dictionary<int,Plot> countPlots = null;//回合开始剧情对白
	public int battleNum = 0;//战斗人数
	public int showBattelPrepare = 0;//是否显示战斗准备

	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|'); 
		checkLength (strArr.Length, 8);
		//strArr[0] is sid  
		//strArr[1] eventType
		this.eventType = StringKit.toInt (strArr [1]);
		//strArr[2] cost
		this.cost = StringKit.toInt (strArr [2]);
		//strArr[3] costType
		this.costType = StringKit.toInt (strArr [3]);
		//strArr[4] otherId
		this.other = StringKit.toInt (strArr [4]);
		//strArr[5] battleType
		this.battleType = StringKit.toInt (strArr [5]);
		//strArr[6] plots
		parsePlots (strArr [6]);
		//strArr[7]
		this.battleNum = StringKit.toInt (strArr [7]);

		this.showBattelPrepare = StringKit.toInt (strArr [8]);
	}
	
	private void parsePlots (string str)
	{
		if (str == "0")
			return;
		npcPlots = new Dictionary<int, Plot> ();
		countPlots = new Dictionary<int, Plot> ();
		string[] strPlots = str.Split ('#');
		//#切分大类
		foreach (string each in strPlots) {
			string[] tmp = each.Split (',');
			Plot newPlot = new Plot ();
			newPlot.plotType = StringKit.toInt (tmp [0]);
			newPlot.count = StringKit.toInt (tmp [1]);
			newPlot.beginSid = StringKit.toInt (tmp [2]);
			newPlot.endSid = StringKit.toInt (tmp [3]);

			if (newPlot.plotType == 2)
				npcPlots.Add (newPlot.count, newPlot);
			else
				countPlots.Add (newPlot.count, newPlot);
		}


	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}

public class MissionEventType
{ 
	public const int NONE = 0, //无事件
		REST = 1, // 休息 一定几率触发事件(副本不同事件不同)  
		TREASURE = 2, //宝箱
		FIGHT = 3, //战斗
		RANDOM_FIGHT = 4, //随机战斗
		PLOT = 5, //剧情
		RESOURCES = 6, //资源
		BOSS = 7, //boss
		SWITCH = 8, //跳转
		PVP = 9, //必然触发PVP
		OVER = 10,//结束事件
        TOW_TREASURE=11,//爬塔宝箱事件
        TOW_OVER=12;//爬塔宝箱结束事件
}
