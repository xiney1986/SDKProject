/// <summary>
/// 技能类型 用于战斗模块
/// </summary>
public class SkillType
{
	public const int Normal = 1, //单体近战普通攻击
	Melee = 2, //单体近战技能攻击
	Remote = 3, //单体远程技能攻击
	MeleeAOE = 4, //近战AOE技能攻击(人物原地攻击.播放AOE子弹,对面受击)

	fixAOE = 5, //远程定点AOE技能攻击

	AddNumber = 6, //增加N次普通攻击技能 (策划配置H)
	HelpOther = 7, //援护 
	MedicalAfterHelp = 8, //援护急救
	FightBack = 9, //反击
	FightBackAfterHelp = 10, //援护反击	

	ComboAttack = 11, //连击(程序自创技能用),后台发addnumber指令,前台创建对应数量的类型为ComboAttack的skill
	GroupAttack = 12, //集合后的合击攻击(程序自创技能用),后台发GroupCombine指令,前台给各成员添加GroupAttack 技能.
	
	GroupCombine = 13, //合击集合	(策划配置H)
	Other = 14, 
	GameWin = 15,
	GameLost = 16,
	BuffCheck = 17,//buff检查
	AddCard=18,//添加替补
	AddNPC=19,//添加npc
	DelNPC=20,//移除npc
	NormalRemote=21,//远程普攻

	MeleeChargeAOE = 22, //近战冲锋AOE技能攻击(人物身上带环绕特效,冲到对面队伍中心点,敌人受伤)
	MeleeJumpAOE = 23, //近战跳跃打击AOE技能(人物放大缩小到敌人中信,敌人受伤,播放AOE子弹)
	bulletMultipleAOE = 24, //远程多发子弹//子弹不能选类型为fixAoe的

    Talk = 25,
    EffectExit = 26;

}