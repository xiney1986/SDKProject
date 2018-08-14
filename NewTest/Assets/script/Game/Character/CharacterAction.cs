using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 角色行为类
  * @author 李程
  * */

/// <summary>
/// 角色显示控制器的动作行为枚举
/// </summary>
public enum enum_character_Action
{
	Dead, // 死亡
	Standy, // 标准
	ComboAttack,//连击
	HelpOther,//援护 
	Medical,//急救
	FightBack,//反击
	GroupAttack,//合击
	UseSkill, // 使用技能
}

public class CharacterAction
{ 
	public enum_character_Action action;
	public SkillCtrl Skill;
	public BuffCtrl buff;


	public void Remove ()
	{
		BattleManager.Instance.getActiveBattleInfo () .removeSkill (Skill);
		if (Skill != null && Skill.isOverSkill == true) {
			BattleManager.Instance.nextBattle ();
		} 
	}
	
	public CharacterAction (enum_character_Action _action)
	{
		action = _action;
	}
	
	public CharacterAction (enum_character_Action _action, SkillCtrl _Skill)
	{
		action = _action;
		Skill = _Skill;
	}

	public CharacterAction (enum_character_Action _action, BuffCtrl _buff)
	{
		action = _action;
		buff = _buff;
	}
}
