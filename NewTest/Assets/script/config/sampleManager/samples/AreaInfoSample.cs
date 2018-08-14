using UnityEngine;
using System.Collections;


/// <summary>
/// 竞技场基础信息模版
/// </summary>
public class AreaInfoSample : Sample
{
	public int num;
	public string des;

	public override void parse (int sid, string str)
	{
		string []strs = str.Split ('|');
		checkLength (strs.Length, 2);
		num = StringKit.toInt (strs [1]);
		des = strs [2];
	}
}

