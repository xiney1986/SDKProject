using System.Collections.Generic;
using System;

/**
 * 卡片技能升级食物
 * @author 汤琦
 * */
public class CardsIntensifyData : IntensifyData
{
	//所吃卡片的id的集合
	private List<string> foods = new List<string>();
	
	public void addFood(string foodId)
	{
		foods.Add(foodId.ToString());
	}
	
	public override string ToFooding ()
	{
		string str = "";
		for (int i = 0; i < foods.Count; i++) {
			if(i==0)
			{
				str += foods[i];
			}
			else
			{
				str += "," + foods[i];
			}
		}
		return str;
	}
}
