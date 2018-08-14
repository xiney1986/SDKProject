using UnityEngine;
using System.Collections;

/**公会BOSS奖励模板   
 * 详见配置说明文件
 *@author gc
 **/
public class GuildBossSinglePrizeSample : Sample
{

	public GuildBossSinglePrizeSample (string str)
	{
		parse (str);
	}
	public int hurtRank;//伤害排行
	public int prizeType;//奖励类型
	public int prizeSid;//奖励sid
	public int prizeSum;//奖励数量
	
	public void parse (string str)
	{
		int index = 0;
		string[] strArr = str.Split ('|');
		this.hurtRank = StringKit.toInt (strArr [index++]);	//伤害值
		this.prizeType = StringKit.toInt (strArr [index++]);//奖励类型
		this.prizeSid = StringKit.toInt (strArr [index++]);	//奖励sid
		this.prizeSum = StringKit.toInt (strArr [index++]);	//奖励数量
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
