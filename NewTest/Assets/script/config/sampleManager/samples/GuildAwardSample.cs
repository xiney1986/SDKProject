using System;
using System.Collections.Generic;

/// <summary>
/// 公会奖励模板
/// </summary>

public class GuildAwardSample
{
	public int sid;
	public string name;
	public int minRank;
	public PrizeSample[] samples;
	public int index;

    public GuildAwardSample(string _str)
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
