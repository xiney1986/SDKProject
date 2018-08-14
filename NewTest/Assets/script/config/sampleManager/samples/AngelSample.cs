using UnityEngine;
using System.Collections;

/**
 * 坐骑模板
 **/
public class AngelSample : Sample {

	public AngelSample ()
	{
	}
	//** 天使名字 */
	public string name = "";
	//**天使index*/
	public int index;
	//** VIP等级要求*/
	public int vipLevelRequired;
	//** 3d形象 */
	public string modelID;
	//** 描述 */
	public string[] description;

	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		this.name = strArr [1];
		this.index = StringKit.toInt(strArr [2]);
		this.vipLevelRequired = StringKit.toInt (strArr [3]);
		this.modelID = strArr [4];
		this.description = strArr [5].Split ('#');
	}
}
