using System;

/**称号模板 
 * 
 * sid name  iconId     describe  buffId    
 * sid 名字  图标编号   描述	 	 buff
 * 称号数据结构 50001|斗神|1|全属性增加150%|5
 *@author longlingquan
 **/
public class TitleSample:Sample
{
	public TitleSample ()
	{
	}
	  
	public string name = "";//称号名字
	public int iconId = 0;//图标编号
	public string describe = "";//称号描述
	public int buffSid = 0;//称号附带buff sid
	
	override public void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 4);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]);  
		//strArr[3] describe
		this.describe = strArr [3]; 
		//strArr[4] buffSid
		this.buffSid = StringKit.toInt (strArr [4]); 
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

