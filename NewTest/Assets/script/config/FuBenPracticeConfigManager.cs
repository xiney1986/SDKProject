using UnityEngine;
using System.Collections;

/**
 * 修炼副本配置文件
 * 讨伐副本只有一个章节
 * @author 汤琦
 * */
using System.Collections.Generic;


public class FuBenPracticeConfigManager : ConfigManager
{
	// 单例
	private static FuBenPracticeConfigManager instance;

	private int sid;//讨伐副本章节编号
	private int[] savePoints;//保存点索引
	private int[] propIds;//修炼中奖励道具的ID
	private string[] pointAwards;//索引点的奖励



	public static FuBenPracticeConfigManager Instance {
		get{
			if(instance==null)
				instance=new FuBenPracticeConfigManager();
			return instance;
		}
	}

	public FuBenPracticeConfigManager ()
	{  
		base.readConfig (ConfigGlobal.CONFIG_FUBEN_PRACTICE);
	}
	
	//获得讨伐活动章节sid
	public int getSid ()
	{
		return sid;
	}
	/// <summary>
	/// 判断索引是否是保存节点
	/// </summary>
	/// <returns><c>true</c>, if index is save point was checked, <c>false</c> otherwise.</returns>
	/// <param name="index">Index.</param>
	public bool checkIndexIsSavePoint(int index)
	{
		for(int i=0;i<savePoints.Length;i++)
		{
			if(index==savePoints[i])
			{
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 修炼中奖励
	/// </summary>
	/// <returns>The property I ds.</returns>
	public int[] getPropIDs()
	{
		return propIds;
	}
	/// <summary>
	/// 对应点位的奖励
	/// </summary>
	/// <returns>The prize by index.</returns>
	/// <param name="index">Index.</param>
	public PrizeSample getPrizeByIndex(int index)
	{
		//奖励是从玩家位置索引1对应奖励0，因为初始位置没有奖励
		index--;
		if(index>=pointAwards.Length)
			return null;
		if(index<0)
			return null;
		string str=pointAwards[index];
		string[] strArr=str.Split(',');
		int id=StringKit.toInt(strArr[0]);
		int count=StringKit.toInt(strArr[1]);
		PrizeSample prize=new PrizeSample(PrizeType.PRIZE_PROP,id,count);
		return prize;
	}
	/// <summary>
	/// 返回对应点之前所有的奖励
	/// </summary>
	/// <returns>The total prize by index.</returns>
	/// <param name="index">Index.</param>
	public List<PrizeSample> getTotalPrizeByIndex(int index)
	{
		List<PrizeSample> totalSample=getShowSample();
		PrizeSample prize;
		for(int i=0;i<pointAwards.Length;i++)
		{
			if(i<index&&index>0)
			{
				prize=getPrizeByIndex(i+1);
				if(prize==null)
				{
					continue;
				}
				for(int j=0;j<totalSample.Count;j++)
				{
					if(totalSample[j].pSid==prize.pSid)
					{
//						totalSample[j].num+=prize.num;
						totalSample[j].addNum(prize.getPrizeNumByInt());
						break;
					}
				}
			}
		}
		return totalSample;
	}
	
	//解析配置
	public override void parseConfig (string str)
	{   
		string[] arr=str.Split('|');
		sid = StringKit.toInt (arr[0]);
		savePoints=StringKit.toArrayInt(arr[1],',');
		propIds=StringKit.toArrayInt(arr[2],',');
		pointAwards=arr[3].Split('#');
	}


	private List<PrizeSample> getShowSample()
	{
		List<PrizeSample> ex_sample=new List<PrizeSample>();
		for(int i=0;i<propIds.Length;i++)
		{
			ex_sample.Add(new PrizeSample(PrizeType.PRIZE_PROP,propIds[i],0));			
		}
		return ex_sample;
	}
}
