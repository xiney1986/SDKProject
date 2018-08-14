using UnityEngine;
using System.Collections;

public class ArenaTimeSample : Sample {
	/** 类型 0开服竞技等待阶段,1海选,2(64-32),3(32-16),4(16-8),5(8-4),6(4-2),7决赛,8休赛,9海选休赛 */
	public int type;
	/** 持续时间 */
	public int time;
	/** 描述 */
	public string des;

	public override void parse (int sid, string str)
	{
		type = sid;
		string [] strs = str.Split ('|');
		time = StringKit.toInt (strs [1]);
		des = strs [2];
	}
}
