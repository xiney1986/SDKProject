using System;

/**经验值模板 
  *@author longlingquan
  **/
public class ArenaFinalSample:Sample
{
	public ArenaFinalSample ()
	{
	}

	public int fightCount; //战斗场次
	public string guessAward; //竞猜奖励文字

	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 2);
		int pos = 1;
		fightCount = StringKit.toInt (strArr [pos++]);
		guessAward = strArr [pos++];

	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

