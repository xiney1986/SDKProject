using UnityEngine;
using System.Collections;

/**
 * 公会学院配置
 * @author 汤琦
 * */
public class GuildCollegeSample : Sample
{

	override public void parse (int sid, string str)
	{ 
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 29);

	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
