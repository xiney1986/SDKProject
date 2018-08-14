using System;
/// <summary>
/// 天梯称号
/// </summary>
public class LaddersTitleSample
{
	public string[] addDescriptions;//加成描述
	public string name;//称号
	public int minPrestige;//最小声望
	public int sid;//编号
	public int factorNum;//系数
	public int index;

	public LaddersTitleSample (string _str)
	{
		string[] strArr=_str.Split('|');
		sid=StringKit.toInt(strArr[0]);
		minPrestige=StringKit.toInt(strArr[1]);
		name=strArr[2];
		addDescriptions=strArr[3].Split('#');
		factorNum=StringKit.toInt(strArr[5]);
	}
}


