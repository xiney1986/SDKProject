using UnityEngine;
using System.Collections;

public class FilterManager 
{
	//筛选卡片  
    public delegate bool FilterHander(object obj);  
    //筛选出符合委托方法所要求的Card, 返回一个Card[]  
	public static ArrayList GetFilterArray(ArrayList list, FilterHander filterHander)  	
    {  
		ArrayList aList = new ArrayList();  
		foreach (object i in list)  
			if (filterHander(i))  
               aList.Add(i);
		return aList;  
	}  
}
public class MyCardFilter  
{  
	public static bool IsEpic(object obj)  
	{  
		Card card = obj as Card;
      	return card.getQualityId() == QualityType.EPIC;  
	}  
	 
	public static bool IsGood(object obj)  
    {  
		Card card = obj as Card;
		return card.getQualityId() == QualityType.GOOD;  
	} 

	public static bool IsCommon(object obj)
	{
		Card card = obj as Card;
		return card.getQualityId() == QualityType.COMMON;
	}

	public static bool IsExcellent(object obj)
	{
		Card card = obj as Card;
		return card.getQualityId() == QualityType.EXCELLENT;
	}

	public static bool IsLegend(object obj)
	{
		Card card = obj as Card;
		return card.getQualityId() == QualityType.LEGEND;
	}

	public static bool IsMagic(object obj)
	{
		Card card = obj as Card;
		return card.getJob() == JobType.MAGIC;
	}

	public static bool IsPower(object obj)
	{
		Card card = obj as Card;
		return card.getJob() == JobType.POWER;
	}

	public static bool IsAgile(object obj)
	{
		Card card = obj as Card;
		return card.getJob() == JobType.AGILE;
	}

	public static bool IsAttack(object obj)
	{
		Card card = obj as Card;
		return card.getJob() == JobType.COUNTER_ATTACK;
	}

	public static bool IsAssist(object obj)
	{
		Card card = obj as Card;
		return card.getJob() == JobType.ASSIST;
	}
}

