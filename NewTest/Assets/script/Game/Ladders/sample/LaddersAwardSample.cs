using System;
using System.Collections.Generic;

/// <summary>
/// 天梯奖励
/// </summary>

public class LaddersAwardSample
{
	public int sid;
	public string name;
	public int minRank;
	public PrizeSample[] samples;
	public int index;

	public LaddersAwardSample (string _str)
	{
		string[] strArr=_str.Split('|');
		sid=StringKit.toInt(strArr[0]);
		minRank=StringKit.toInt(strArr[1]);
		name=strArr[2];

		string[] prizeArr=strArr[3].Split('#');
		int length=prizeArr.Length;
		samples=new PrizeSample[length];
		PrizeSample sample;
		for(int i=0;i<length;i++)
		{
			sample = new PrizeSample(prizeArr[i],',');
			samples[i]=sample;
		}
	}
}
