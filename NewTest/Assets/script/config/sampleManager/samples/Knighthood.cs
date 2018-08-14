using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 爵位
 * @author 汤琦
 * */
public class Knighthood
{
	public Knighthood (string str)
	{
		parse(str);
	}
	public int grade;//等级
	public string kName;//爵位名
	public int starValue;//星星值
	public int icon;//图标
	public string desc;//描述
	public int needHonorValue;//所需荣誉值
	public List<KnighthoodAddValue> values;//爵位附加值hp,1400,0#attack,260,0#defense,260,0#magic,260,0#agile,260,0

	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		//strArr[0] grade
		this.grade = StringKit.toInt(strArr [0]);
		//strArr[1] kName
		this.kName = strArr [1];
		//strArr[2] starValue
		this.starValue = StringKit.toInt(strArr [2]);
		//strArr[3] icon
		this.icon = StringKit.toInt(strArr [3]);
		//strArr[4] desc
		this.desc = strArr [4];
		//strArr[5] needHonorValue
		this.needHonorValue = StringKit.toInt(strArr [5]);
		//strArr[6] values
		parseContent(strArr[6]);
	}
	private void parseContent(string str)
	{
		if(values == null)
			values = new List<KnighthoodAddValue>();
		string[] strs = str.Split('#');
		for (int i = 0; i < strs.Length; i++) {
			values.Add(new KnighthoodAddValue(strs[i]));
		}
	}
}
public class KnighthoodAddValue
{
	public KnighthoodAddValue(string str)
	{
		parse (str);
	}
	public string valueType;
	public int currentValue;
	public int nextValue;

	private void parse(string str)
	{
		string[] strs = str.Split(',');
		valueType = strs[0];
		currentValue = StringKit.toInt(strs[1]);
		nextValue = StringKit.toInt(strs[2]);
	}
}
