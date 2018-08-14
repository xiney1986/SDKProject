using UnityEngine;
using System.Collections;

/**公会BOSS信息模板   
 * 详见配置说明文件
 *@author 汤琦
 **/
public class GuildBossSample : Sample
{
	public GuildBossSample ()
	{
		
	}
	
	public string bossName;//BOSS名字
	public int icon;//图标
	public string blessing;//祝福
	public string weakness;//弱点
	
	public override void parse (int sid, string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 4);
		//strArr[0] is sid  
		//strArr[1] bossName
		this.bossName = strArr [1];
		//strArr[2] icon
		this.icon = StringKit.toInt (strArr [2]);
		//strArr[3] blessing
		this.blessing = strArr [3];
		//strArr[4] weakness
		this.weakness = strArr [4];
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
