using System;
/// <summary>
/// 天梯奖章
/// </summary>
public class LaddersMedalSample
{
	public string[] addDescriptions;//加成描述
	public int minRank;	//最小排名
	public string name;//奖章名字
	public int sid;//编号
	public int index;

	public LaddersMedalSample (string str)
	{
		string[] strArr=str.Split('|');
		sid=StringKit.toInt(strArr[0]);
		minRank=StringKit.toInt(strArr[1]);
		name=strArr[2];
		addDescriptions=strArr[3].Split('#');
	}
}