using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**任务模板   
 * 详见配置说明文件
 *@author 汤琦
 **/
public class TaskSample : Sample
{
	public TaskSample ()
	{
		
	}

	public string name;//任务名
	public int showLv = 0;//任务显示等级
	public int taskType = 0;//任务类型
	public TaskCondition condition;//任务条件内容
	public string[] conditionDesc;//任务条件描述
	public List<PrizeSample> prizes;//任务奖励
	public int windowLinkSid; //
	public List<string> dailyPrizes; //每日返利奖励

	public override void parse (int sid, string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 7);
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] showSid
		this.showLv = StringKit.toInt (strArr [2]);
		//strArr[3] taskType
		this.taskType = StringKit.toInt (strArr [3]);
		//strArr[4] condition
		parseCondition (strArr [4]);
		//strArr[5] conditionDesc
		parseConditionDesc (strArr [5]);
		//strArr[6] prizes
		if (strArr [6] [0] != '$')
			parsePrize (strArr [6]);
		else
			parseDailyPrize (strArr [6].Substring(1,strArr [6].Length - 1));

		windowLinkSid = StringKit.toInt(strArr[7]);
	}
	
	private void parseCondition (string str)
	{
		string[] strs = str.Split (',');
		condition = new TaskCondition (strs);
	}

	private void parseConditionDesc (string str)
	{
		string[] strs = str.Split ('#');
		conditionDesc = new string[strs.Length];
		for (int i = 0; i < strs.Length; i++) {
			conditionDesc [i] = strs [i];
		}
	}

	private void parsePrize (string str)
	{
		prizes = new List<PrizeSample> ();
		string[] strs = str.Split ('#');
		for (int i = 0; i < strs.Length; i++) {			
			PrizeSample prize = new PrizeSample (strs [i], ',');
			prizes.Add (prize);
		}
	}
	/// <summary>
	/// 解析每日奖励
	/// </summary>
	/// <param name="str">String.</param>
	private void parseDailyPrize(string str)
	{
		dailyPrizes = new List<string> ();
		string[] strs = str.Split ('$');
		for (int i = 0; i < strs.Length; i++)
			dailyPrizes.Add (strs [i]);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		TaskSample dest = destObj as TaskSample;
		if (this.conditionDesc != null) {
			dest.conditionDesc = new string[this.conditionDesc.Length];
			for (int i = 0; i < this.conditionDesc.Length; i++)
				dest.conditionDesc [i] = this.conditionDesc [i];
		}
		if (this.prizes != null) {
			dest.prizes = new List<PrizeSample> ();
			for (int i = 0; i < this.prizes.Count; i++)
				dest.prizes.Add (this.prizes [i].Clone () as PrizeSample);
		}
	}
}

public class TaskCondition
{
	public TaskCondition (string[] strs)
	{
		parse (strs);
	}

	public string key;
	public int[] conditions;

	public void parse (string[] strs)
	{
		conditions = new int[strs.Length - 1];
		for (int i = 0; i < strs.Length; i++) {
			if (i == 0) {
				key = strs [i];
			} else {
				conditions [i - 1] = StringKit.toInt (strs [i]);
			}
		}
	}
}
