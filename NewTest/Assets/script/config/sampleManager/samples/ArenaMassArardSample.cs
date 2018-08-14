using System;

/**经验值模板 
  *@author longlingquan
  **/
public class ArenaMassArardSample:Sample
{
	public ArenaMassArardSample ()
	{

	}

	public int integral; //积分奖励
	public int merit; //功勋奖励
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 2);
        
		integral = StringKit.toInt (strArr [1]);
		merit = StringKit.toInt (strArr [2]);
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

