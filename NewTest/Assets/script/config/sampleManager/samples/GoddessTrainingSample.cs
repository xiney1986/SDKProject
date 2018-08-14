using UnityEngine;
using System.Collections;

/// <summary>
/// 女神训练模板 
/// </summary>
public class GoddessTrainingSample  {

	public GoddessTrainingSample()
	{
		
	}
	
	public int sid;
	public int lvCondition;//功能开启等级条件
	/// <summary>
	/// 开启条件 0:第一个栏位等级, 1:第二个栏位等级, 2:第三个栏位vip等级
	/// </summary>
	public string[] EnabledCondition;
	/// <summary>
	/// 人物每个等级给卡牌奖励的经验
	/// </summary>
	public string[] AwardExp;
	/// <summary>
	/// 训练时长
	/// </summary>
	public string[] TrainingTime;
	/// <summary>
	/// 训练时长双倍消耗RMB
	/// </summary>
	public string[] TimeRmb;
	
	public string[] TimeLimitLv;
	
	public void parse (int id, string str)
	{
		this.sid = id; 
		string[] strArr = str.Split ('|');
		lvCondition=StringKit.toInt(strArr[1]);
		TimeLimitLv = strArr[2].Split(',');
		TimeRmb = strArr[3].Split(',');
		TrainingTime = strArr[4].Split(',');
		EnabledCondition = strArr[5].Split(',');
		AwardExp = strArr[6].Split(',');
	}
}
