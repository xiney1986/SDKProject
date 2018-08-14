using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**充值模板   
 * 详见配置说明文件
 *@author 汤琦
 **/
public class RechargeSample : Sample
{
	public RechargeSample()
	{

	}

	public const int RECHARGE = 1;//单笔充值
	public const int RECHARGES = 2;//累积充值

	public int reType;//类型
	public int condition;//条件
	public string desc;//描述
	public int count;//次数
	public int startTime = 0;//开始时间
	public int overTime = 0;//结束时间
	public PrizeSample[] prizes;//奖励


	#region server set config
	private void parse_condition(string v)
	{
		this.condition = StringKit.toInt(v);
	}

	private void parse_prizes(string v)
	{
		string[] strs = v.Split('#');
		prizes = new PrizeSample[strs.Length];
		for (int i = 0; i < strs.Length; i++)
		{
			prizes[i] = new PrizeSample(strs[i], ',');
		}
	}

	#endregion

	public override void parse(int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split('|');
		checkLength(strArr.Length, 7);
		//strArr[0] is sid
		//strArr[1] reType
		this.reType = StringKit.toInt(strArr[1]);
		//strArr[2] condition
		this.condition = StringKit.toInt(strArr[2]);
		//strArr[3] desc
		this.desc = strArr[3];
		//strArr[4] count
		this.count = StringKit.toInt(strArr[4]);
		//strArr[5] startTime
		this.startTime = StringKit.toInt(strArr[5]);
		//strArr[6] overTime
		this.overTime = StringKit.toInt(strArr[6]);
		//strArr[7] prizes
		parse_prizes(strArr[7]);
	}


	public override void copy(object destObj)
	{
		base.copy(destObj);
		RechargeSample dest = destObj as RechargeSample;
		if (this.prizes != null)
		{
			dest.prizes = new PrizeSample[this.prizes.Length];
			for (int i = 0; i < this.prizes.Length; i++)
				dest.prizes[i] = this.prizes[i].Clone() as PrizeSample;
		}
	}

	public string ToString(NoticeSample noticeSample)
	{
		string str = "";
		for (int i = 0; i < prizes.Length; i++)
		{
			str += prizes[i].ToString();
			if (i < prizes.Length - 1)
			{
				str += "，";
			}
		}
		return ParseDesc(desc, condition, noticeSample) + " 次数：" + count + " 奖励：" + str;
	}

	private string ParseDesc(string desc, int condition, NoticeSample noticeSample)
	{
		if (string.IsNullOrEmpty(desc))
			return "";
		int value = condition;
		if (noticeSample.type == NoticeType.TOPUPNOTICE || noticeSample.type == NoticeType.TIME_RECHARGE ||
			noticeSample.type == NoticeType.NEW_RECHARGE)
		{
			value /= 10;
		}
		return desc.Replace("%1", value.ToString());
	}
}
