using System;
 
/**
 * 战斗阵型实体对象
 * @author longlingquan
 * */
public class BattleFormation
{
	public BattleFormation ()
	{
	}
	
	public string id = "";//如果是本方阵型 则是uid 如果是敌方阵型则是sid
	public int hp = 0;//如果为-1 表示是满血
	public int maxHp = 0;
	public int lv = -1;
} 

