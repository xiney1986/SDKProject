using UnityEngine;
using System.Collections;



public class BuffDataBase
{ 
	public int damage; //感染buff时的伤害
	public bool isDurationBuff;//是否为持续型buff 
	
	/**public int  attr_attack;//属性改变
	public int attr_defend;
	public int attr_magic;
	public int attr_dex;*/
	public int damagePerTurn;//每回合伤害 
	//public int duration ;//持续时间，按回合计算
	 
	public string  buffName;
	public int buffSid;
	public string DurationdamageEffect;//持续buff触发时候的效果

	public int BuffDisplayType = BuffIconType.None; //buff显示类型
	public string  DisplayResPath;//buff显示资源路径
	public BuffType buffType;//buff类型
}
