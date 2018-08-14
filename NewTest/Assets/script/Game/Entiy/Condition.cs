using System;
using System.Collections.Generic;

/**
 * 排序条件实体类
 * @author huangzhenghan
 * */
public class Condition
{
	public Condition (int type)
	{
		this.type = type;
		conditions = new List<int> ();
		
	}

	
	public Condition (int type, int[] limit)
	{
		this.type = type;
		conditions = new List<int> ();
		
		//YXZH-3601 bugFix
		//这里不用listKit.addRange是因为IOS上有BUG 
		//大意是传进的参数无法为实现Ilist里的某个接口
		foreach(int i in limit)
			conditions.Add(i);
	}

	
	public int type;
	public List<int> conditions;//筛选条件
	
	public int getType ()
	{
		return type;
	}
	
	public List<int> getConditions ()
	{
		return conditions;
	}
	
	//注入筛选的具体条件
	public void addCondition (int condition)
	{
		conditions.Add (condition);
	}
}
