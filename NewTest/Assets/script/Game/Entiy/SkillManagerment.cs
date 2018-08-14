using System;
 
/**
 * 技能管理器
 * @author longlingquan
 * */
public class SkillManagerment
{

	public SkillManagerment ()
	{ 

	}
	 
	public static SkillManagerment Instance {
		get{return SingleManager.Instance.getObj("SkillManagerment") as SkillManagerment;}
	}

	public string  stateTypeToString (int stateID)
	{ 
		if (stateID == SkillStateType.ACTIVE) {
			return LanguageConfigManager.Instance.getLanguage ("s0037");
		}
		if (stateID == SkillStateType.BUFF) {
			return LanguageConfigManager.Instance.getLanguage ("s0039");
		}
		if (stateID == SkillStateType.ATTR) {
			return LanguageConfigManager.Instance.getLanguage ("s0038");
		}
		return "";
	}
	 
	public Skill createSkill (int sid)
	{ 
		return new Skill (sid, 0, 0);
	}
	
	public Skill createSkill (int sid, long exp, int type)
	{ 
		return new Skill (sid, exp, type);
	}
	
	//创建一组技能 ea所有技能信息
	public Skill[] createSkills (ErlArray ea, int type)
	{
		if (ea.Value.Length < 1)
			return null;
		Skill[] skills = new Skill[ea.Value.Length];
		ErlArray skillInfo;//单一技能信息
		for (int i = 0; i < skills.Length; i++) {
			skillInfo = ea.Value [i] as ErlArray;
			skills [i] = createSkill (StringKit.toInt (skillInfo.Value [0].getValueString ()),
				StringKit.toInt (skillInfo.Value [1].getValueString ()), type);
		}
		return skills;
	}
	 

	public Skill[] createSkillsLV (ErlArray ea, int type)
	{
		if (ea.Value.Length < 1)
			return null;
		Skill[] skills = new Skill[ea.Value.Length];
		ErlArray skillInfo;//单一技能信息
		for (int i = 0; i < skills.Length; i++) {
			skillInfo = ea.Value [i] as ErlArray;
			Skill sk=createSkill (StringKit.toInt (skillInfo.Value [0].getValueString ()),0, type);
			sk.setLevel(StringKit.toInt (skillInfo.Value [1].getValueString ()));
			skills [i] = sk;
		}
		return skills;
	}
	
	//子弹特效
	public string getSkillBulletPerfab (int sid)
	{
		int id = SkillSampleManager.Instance.getSkillSampleBySid (sid).bulletEffect;
		return EffectConfigManager.Instance.getEffectPerfab (id);
	} 
	//子弹特效
	public string getAroundEffectPerfab (int sid)
	{
		int id = SkillSampleManager.Instance.getSkillSampleBySid (sid).aroundEffect;
		return EffectConfigManager.Instance.getEffectPerfab (id);
	} 
	//被击中效果
	public string getDamageEffect (int sid)
	{
		int id = SkillSampleManager.Instance.getSkillSampleBySid (sid).damageEffect;
		return EffectConfigManager.Instance.getEffectPerfab (id);
	}
	
	//施法特效
	public string getSpellEffect (int sid)
	{
		int id = SkillSampleManager.Instance.getSkillSampleBySid (sid).spellEffect;
		return EffectConfigManager.Instance.getEffectPerfab (id);
	}

 

} 



