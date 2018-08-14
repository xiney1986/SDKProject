
/*
 * 技能触发时机 activeType
 * @author longlingquan
 * */
public class SkillActiveType
{
	public const int None = 1,
	SkillUse = 2, //开始使用技能触发
	SpellEnd = 3, //吟唱结束时触发
	SkillHit = 4, //技能击中单位时触发
	Grouped = 5, //人物聚集时触发 （合击）
	FightBackStart = 6, //反击开始时
	HelpOtherOver = 7, //援护结束后
	AttackEnd = 8,//攻击结束后 
	Deaded = 9;//角色死亡的时候触发
	
}
