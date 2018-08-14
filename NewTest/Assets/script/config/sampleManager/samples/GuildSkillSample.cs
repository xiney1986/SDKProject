using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**公会技能模板   
 * 详见配置说明文件
 *@author 汤琦
 **/
public class GuildSkillSample : Sample
{
	public GuildSkillSample ()
	{
		
	}
	
	public string skillName;//技能名字
	public string desc;//描述
	public string icon;//图标
	public int openLevel = 0;//解锁所需学院等级
	public List<int> costs;//升级消耗,贡献值
	public AttrChangeSample[] attr;//附加属性

	public override void parse (int sid, string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 6);
		//strArr[0] is sid  
		//strArr[1] skillName
		this.skillName = strArr [1];
		//strArr[2] desc
		this.desc = strArr [2];
		//strArr[3] icon
		this.icon = strArr [3];
		//strArr[4] openLevel
		this.openLevel = StringKit.toInt (strArr [4]);
		//strArr[5] upCost
		parseCost (strArr [5]);
		//strArr[6] attr
		parseEffects (strArr [6]);
	}

	private void parseCost (string str)
	{
		string[] strs = str.Split (',');
		if (costs == null)
			costs = new List<int> ();
		for (int i = 0; i < strs.Length; i++) {
			costs.Add (StringKit.toInt (strs [i]));
		}
	}

	private void parseEffects (string str)
	{
		//表示空
		if (str == 0 + "")
			return;
		string[] strArr = str.Split ('#');  
		attr = new AttrChangeSample[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			AttrChangeSample att = new AttrChangeSample ();
			att.parse (strArr [i]);
			attr [i] = att;
		}
	}

	//当没有学习技能时，显示1级的描述
	public string getDescribe ()
	{
		string str = desc;
		AttrChangeSample[] change = attr;
		return DescribeManagerment.getDescribe (str, 1, change); 
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		GuildSkillSample dest = destObj as GuildSkillSample;
		if (this.costs != null) {
			dest.costs = new List<int> ();
			for (int i = 0; i < this.costs.Count; i++)
				dest.costs.Add (this.costs [i]);
		}
		if (this.attr != null) {
			dest.attr = new AttrChangeSample[this.attr.Length];
			for (int i = 0; i < this.attr.Length; i++)
				dest.attr [i] = this.attr [i].Clone () as AttrChangeSample;
		}
	}
}
