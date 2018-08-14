using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**公会建筑模板   
 * 详见配置说明文件
 *@author 汤琦
 **/
public class GuildBuildSample : Sample
{
	public GuildBuildSample ()
	{
		
	}

	public string buildName;//建筑名字
	private List<int> icon;//图标
	private List<string> desc;//升级描述
	public int levelMax;//等级上限
	public List<int> costs;//升级消耗,贡献值
	public List<int> hallLevel;//大厅等级限制
	public List<GuildGood> goods;//公会物品

	/// <summary>
	/// 获得对应等级的建筑升级描述
	/// </summary>
	public string getDesc (int Lv) {
		if (desc != null && desc.Count > 0) {
			if (Lv >= desc.Count) {
				Lv = desc.Count - 1;
			}
			return desc[Lv];
		}
		return "";
	}

	/// <summary>
	/// 获得对应等级的建筑图标
	/// </summary>
	public int getIconByLv (int Lv) {
		if (icon != null && icon.Count > 0) {
			if (Lv >= icon.Count) {
				Lv = icon.Count - 1;
			}
			return icon[Lv];
		}
		return 1;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		GuildBuildSample dest = destObj as GuildBuildSample;
		if (this.costs != null) {
			dest.costs = new List<int> ();
			for (int i = 0; i < this.costs.Count; i++)
				dest.costs.Add (this.costs [i]);
		}
		if (this.hallLevel != null) {
			dest.hallLevel = new List<int> ();
			for (int i = 0; i < this.hallLevel.Count; i++)
				dest.hallLevel.Add (this.hallLevel [i]);
		}
		if (this.goods != null) {
			dest.goods = new List<GuildGood> ();
			for (int i = 0; i < this.goods.Count; i++)
				dest.goods.Add (this.goods [i].Clone () as GuildGood);
		}
	}
	
	public override void parse (int sid, string str)
	{
		this.sid = sid; //strArr[0] is sid  
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 7);

		this.buildName = strArr [1];//建筑名字
		this.icon = parseInt (strArr [2]);//图标
		parseDesc (strArr [3]);//升级描述
		this.levelMax = StringKit.toInt (strArr [4]);//等级上限
		parseCost (strArr [5]);//升级消耗,贡献值
		parseHallLevel (strArr [6]);//大厅等级限制
		parseGood (strArr [7]);//公会物品
	}

	private void parseDesc (string str)
	{
		string[] strs = str.Split (',');
		desc = new List<string> ();
		for (int i = 0; i < strs.Length; i++) {
			desc.Add (strs [i]);
		}
	}

	private List<int> parseInt (string str)
	{
		string[] strs = str.Split (',');
		List<int> tmpInt = new List<int> ();
		for (int i = 0; i < strs.Length; i++) {
			tmpInt.Add (StringKit.toInt (strs [i]));
		}
		return tmpInt;
	}

	private void parseGood (string str)
	{
		if (str.Length < 2)
			return;
		goods = new List<GuildGood> ();
		string[] strs = str.Split ('#');
		for (int i = 0; i < strs.Length; i++) {
			string[] temp = strs [i].Split (',');
			for (int j = 0; j < temp.Length; j++) {
				GuildGood good = new GuildGood (StringKit.toInt (temp [j]), i + 1);
				goods.Add (good);
			}
		}
	}

	private void parseCost (string str)
	{
		string[] strs = str.Split (',');
		costs = new List<int> ();
		for (int i = 0; i < strs.Length; i++) {
			costs.Add (StringKit.toInt (strs [i]));
		}
	}

	private void parseHallLevel (string str)
	{
		string[] strs = str.Split (',');
		hallLevel = new List<int> ();
		for (int i = 0; i < strs.Length; i++) {
			hallLevel.Add (StringKit.toInt (strs [i]));
		}
	}

}
