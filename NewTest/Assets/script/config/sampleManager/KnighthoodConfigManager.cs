using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**爵位配置文件
  *@author 汤琦
  **/
public class KnighthoodConfigManager : SampleConfigManager
{
	//单例
	private static KnighthoodConfigManager instance;
	private List<Knighthood> list;

	public KnighthoodConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_KNIGHTHOOD);
	}
	
	public static KnighthoodConfigManager Instance {
		get{
			if(instance==null)
				instance=new KnighthoodConfigManager();
			return instance;
		}
	}
	//获得所有爵位信息
	public Knighthood[] geKnighthoodInfos ()
	{
		return list.ToArray ();
	}
	//根据等级获得对应爵位
	public Knighthood getKnighthoodByGrade(int grade)
	{
		for (int i = 0; i < list.Count; i++) {
			if(list[i].grade == grade)
			{
				return list[i];
			}
		}
		return null;
	}
	//是否是最后一个爵位
	public bool isLastKnighthood(int grade)
	{
		Knighthood k = getKnighthoodByGrade(grade);
		if(k.grade == list[list.Count - 1].grade)
		{
			return true;
		}
		return false;
	}
	//根据等级获得对应爵位下一级的爵位
	public Knighthood getNextKnigthoodByGrade(int grade)
	{
		for (int i = 0; i < list.Count; i++) {
			if(list[i].grade == grade + 1)
			{
				return list[i];
			}
		}
		return null;
	}
	//解析配置
	public override void parseConfig (string str)
	{  
		Knighthood be = new Knighthood (str);
		if (list == null)
			list = new List<Knighthood> ();
		list.Add (be);
	}

}
