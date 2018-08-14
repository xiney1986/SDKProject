using System;

/**经验值模板 
  *@author longlingquan
  **/
public class ArenaRobotUserSample:Sample
{
	public ArenaRobotUserSample ()
	{
	}
    
	public int headIcon;
	public string name;
	public int level;
    
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 3);
        
		//strArr[0] is sid  
		//strArr[1] exps
		headIcon = StringKit.toInt (strArr [1]);
		name = strArr [2];
		level = StringKit.toInt (strArr [3]);
	}
   
	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

